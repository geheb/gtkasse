using FluentResults;
using GtKasse.Core.Converter;
using GtKasse.Core.Database;
using GtKasse.Core.Email;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using GtKasse.Core.User;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace GtKasse.Core.Repositories;

public sealed class Users
{
    private readonly ILogger _logger;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly UserManager<IdentityUserGuid> _userManager;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly LinkGenerator _linkGenerator;
    private readonly EmailValidatorService _emailValidator;
    private readonly SignInManager<IdentityUserGuid> _signInManager;
    private readonly EmailService _emailService;
    private readonly double _defaultLockoutMinutes;
    private readonly UuidPkGenerator _pkGenerator = new();

    public Users(
        ILogger<Users> logger,
        IDataProtectionProvider dataProtectionProvider,
        UserManager<IdentityUserGuid> userManager,
        IHttpContextAccessor httpContext,
        IdentityErrorDescriber errorDescriber,
        LinkGenerator linkGenerator,
        EmailValidatorService emailValidator,
        SignInManager<IdentityUserGuid> signInManager,
        EmailService emailService,
        IOptions<IdentityOptions> identityOptions)
    {
        _logger = logger;
        _dataProtectionProvider = dataProtectionProvider;
        _userManager = userManager;
        _httpContext = httpContext;
        _errorDescriber = errorDescriber;
        _linkGenerator = linkGenerator;
        _emailValidator = emailValidator;
        _signInManager = signInManager;
        _emailService = emailService;
        _defaultLockoutMinutes = (identityOptions.Value ?? new IdentityOptions()).Lockout.DefaultLockoutTimeSpan.TotalMinutes;
    }

    public async Task<(string? Error, bool RequiresTwoFactor)> SignIn(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning($"user {email} not found");
            return ("Ungültiger Anmeldeversuch. Bitte E-Mail und/oder Passwort überprüfen.", false);
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            user.LastLogin = DateTimeOffset.UtcNow;
            user.LockoutEnd = null;
            await _signInManager.UserManager.UpdateAsync(user);

            _logger.LogInformation($"user {user.Id} logged in");
            return (null, false);
        }
        else if (result.RequiresTwoFactor)
        {
            return (null, true);
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning($"user {user.Id} is locked out");
            return ($"Dein Login ist vorübergehend gesperrt, versuche in {_defaultLockoutMinutes} Minuten wieder.", false);
        }
        else if (result.IsNotAllowed)
        {
            _logger.LogWarning($"user {user.Id} is not allowed to log in");
            return ("Login fehlgeschlagen, du kannst dich leider noch nicht anmelden.", false);
        }
        else
        {
            _logger.LogWarning($"user {user.Id} failed to log in");
            return ("Login fehlgeschlagen, bitte E-Mail und/oder Passwort überprüfen.", false);
        }
    }

    public async Task<Result> SignInTwoFactor(string code, bool rememberClient, CancellationToken cancellationToken)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
        {
            return Result.Fail("Login fehlgeschlagen, bitte erneut anmelden");
        }
        var signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, false, rememberClient);
        if (!signInResult.Succeeded)
        {
            return Result.Fail("Login fehlgeschlagen, der Code ist ungültig.");
        }

        user.LastLogin = DateTimeOffset.UtcNow;
        user.LockoutEnd = null;
        await _signInManager.UserManager.UpdateAsync(user);

        _logger.LogInformation($"user {user.Id} via 2FA logged in");

        return Result.Ok();
    }

    public async Task<bool> RemoveUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;

        var result = await _userManager.RemovePasswordAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError($"remove password for user {id} failed");
            return false;
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                _logger.LogError($"remove roles for user {id} failed");
                return false;
            }
        }

        user.Email = user.UserName + "@removed";
        user.DebtorNumber = null;
        user.AddressNumber = null;
        user.LeftOn = DateTimeOffset.UtcNow;
        user.Name = new string(user.Name?.Split(' ', '-').Select(u => u[0]).ToArray()) + "*";

        result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError($"anonymize user {id} failed");
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError($"delete user {id} failed");
            return false;
        }

        return true;
    }

    public async Task SignOutCurrentUser()
    {
        if (_httpContext.HttpContext == null) return;

        var userId = _httpContext.HttpContext.User.GetId();

        await _signInManager.SignOutAsync();

        _logger.LogInformation($"user {userId} logged out");
    }

    public async Task NotifyPasswordForgotten(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning($"user {email} not found");
            return;
        }

        await NotifyConfirmPasswordForgotten(user, cancellationToken);
    }

    public async Task<string[]?> Update(UserDto dto, string? password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user == null)
        {
            return new[] { "Benutzer wurde nicht gefunden" };
        }

        if (dto.Email != null && !dto.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var isValid = await _emailValidator.Validate(dto.Email, cancellationToken);
            if (!isValid) return new[] { "Die E-Mail ist ungültig" };
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
            var result = await _userManager.ChangeEmailAsync(user, dto.Email, token);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        if (dto.PhoneNumber != null && !dto.PhoneNumber.Equals(user.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            IdentityResult result;
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                result = await _userManager.SetPhoneNumberAsync(user, string.Empty);
            }
            else
            {
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, dto.PhoneNumber);
                result = await _userManager.ChangePhoneNumberAsync(user, dto.PhoneNumber, token);
            }

            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        if (!string.IsNullOrEmpty(password))
        {
            foreach (var validator in _userManager.PasswordValidators)
            {
                var r = await validator.ValidateAsync(_userManager, user, password);
                if (!r.Succeeded)
                {
                    return r.Errors.Select(e => e.Description).ToArray();
                }
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }

            if (!user.EmailConfirmed)
            {
                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    return result.Errors.Select(e => e.Description).ToArray();
                }
            }
        }

        int count = 0;
        if (!(dto.Name ?? string.Empty).Equals(user.Name))
        {
            user.Name = dto.Name;
            count++;
        }
        if (!(dto.DebtorNumber ?? string.Empty).Equals(user.DebtorNumber))
        {
            user.DebtorNumber = string.IsNullOrEmpty(dto.DebtorNumber) ? null : dto.DebtorNumber;
            count++;
        }
        if (!(dto.AddressNumber ?? string.Empty).Equals(user.AddressNumber))
        {
            user.AddressNumber = string.IsNullOrEmpty(dto.AddressNumber) ? null : dto.AddressNumber;
            count++;
        }

        if (count > 0)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var removeRoles = roles.Except(dto.Roles ?? new string[0]).ToArray();
        var addRoles = dto.Roles != null ? dto.Roles.Except(roles).ToArray() : new string[0];

        if (removeRoles.Length > 0)
        {
            var result = await _userManager.RemoveFromRolesAsync(user, removeRoles);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        if (addRoles.Length > 0)
        {
            var result = await _userManager.AddToRolesAsync(user, addRoles);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        return null;
    }

    public async Task<string[]?> UpdateName(Guid id, string? name)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return new[] { "Benutzer wurde nicht gefunden" };
        }

        bool hasChanges = false;

        if (!(name ?? string.Empty).Equals(user.Name))
        {
            hasChanges = true;
            user.Name = name;
        }

        if (hasChanges)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        return null;
    }

    public async Task<string[]?> UpdatePhoneNumber(Guid id, string? phoneNumber)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return new[] { "Benutzer wurde nicht gefunden" };
        }

        IdentityResult? result = null;
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            result = await _userManager.SetPhoneNumberAsync(user, string.Empty);
        }
        else if(!phoneNumber.Equals(user.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, token);
        }

        if (result != null && !result.Succeeded)
        {
            return result.Errors.Select(e => e.Description).ToArray();
        }
        
        return null;
    }

    public async Task<string?> VerfiyChangePassword(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning($"user {id} not found");
            return null;
        }

        token = HttpUtility.UrlDecode(token);

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<IdentityUserGuid>.ResetPasswordTokenPurpose,
            token);
        

        if (!isTokenValid)
        {
            _logger.LogError($"verify token for user {id} failed");
            return default;
        }

        return user.Email;
    }

    public async Task<(string[]? Error, string? Email)> ChangePassword(Guid id, string? token, string password)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning($"user {id} not found");
            return (null, null);
        }

        if (string.IsNullOrEmpty(token))
        {
            token = await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        else
        {
            token = HttpUtility.UrlDecode(token);

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<IdentityUserGuid>.ResetPasswordTokenPurpose,
                token);

            if (!isTokenValid)
            {
                _logger.LogError($"verify token for user {id} failed");
                return (null, null);
            }
        }

        foreach (var validator in _userManager.PasswordValidators)
        {
            var r = await validator.ValidateAsync(_userManager, user, password);
            if (!r.Succeeded)
            {
                return (r.Errors.Select(r => r.Description).ToArray(), user.Email);
            }
        }

        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            return (result.Errors.Select(r => r.Description).ToArray(), user.Email);
        }

        return (null, user.Email);
    }

    public async Task<(string? Error, bool IsFatal)> NotifyConfirmChangeEmail(Guid id, string newEmail, string currentPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return ("Benutzer wurde nicht gefunden", true);
        }

        if (await _userManager.FindByEmailAsync(newEmail) != null)
        {
            return ("Die neue E-Mail-Adresse kann nicht genutzt werden", false);
        }

        if (!await _emailValidator.Validate(newEmail, cancellationToken))
        {
            return ("Die neue E-Mail-Adresse ist ungültig", false);
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
        if (result != PasswordVerificationResult.Success)
        {
            return ("Das angegebene Passwort stimmt nicht überein", false);
        }

        if (!await NotifyConfirmChangeEmail(user, newEmail, cancellationToken))
        {
            return ("Fehler beim Speichern", true);
        }

        return (null, false);
    }

    public async Task<string?> VerifyConfirmRegistration(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogError($"user {id} not found");
            return null;
        }

        token = HttpUtility.UrlDecode(token);

        var isUserTokenValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<IdentityUserGuid>.ConfirmEmailTokenPurpose,
            token);

        if (!isUserTokenValid)
        {
            _logger.LogError($"user {id} has invalid token");
            return null;
        }

        return user.Email;
    }

    public async Task<string[]?> ConfirmRegistrationAndSetPassword(Guid id, string token, string password)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogError($"user {id} not found");
            return new[] { "Benutzer wurde nicht gefunden" };
        }

        token = HttpUtility.UrlDecode(token);

        var isUserTokenValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<IdentityUserGuid>.ConfirmEmailTokenPurpose,
            token);

        if (!isUserTokenValid)
        {
            _logger.LogError($"user {id} has invalid token");
            return new[] { "Der Link ist ungültig oder abgelaufen." };
        }

        var identityResult = await _userManager.ConfirmEmailAsync(user, token);
        if (!identityResult.Succeeded)
        {
            return identityResult.Errors.Select(r => r.Description).ToArray();
        }

        var result = await ChangePassword(id, null, password);
        return result.Error;
    }

    public async Task<bool> NotifyConfirmRegistration(Guid id, bool registerExtended, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogError($"user {id} not found");
            return false;
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation($"user {id} is already confirmed");
            return true;
        }

        return await NotifyConfirmRegistration(user, registerExtended, cancellationToken);
    }

    public async Task<UserDto[]> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .Include(e => e.UserRoles!).ThenInclude(e => e.Role)
            .OrderBy(e => e.Name)
            .Where(e => e.LeftOn == null)
            .Select(e => new 
            { 
                entity = e, 
                canBeDeleted = 
                    e.Bookings!.Count == 0 && 
                    e.Trips!.Count == 0 && 
                    e.Invoices!.Count == 0 &&
                    e.TripBookings!.Count == 0 &&
                    e.TripChats!.Count == 0 &&
                    e.VehicleBookings!.Count == 0 &&
                    e.WikiArticles!.Count == 0 &&
                    e.Tryouts!.Count == 0 &&
                    e.TryoutBookings!.Count == 0 &&
                    e.BoatRentals!.Count == 0
            })
            .ToArrayAsync(cancellationToken);

        var idn = new IdnMapping();
        var dc = new GermanDateTimeConverter();

        return users.Select(e => new UserDto(e.entity, e.canBeDeleted, idn, dc)).ToArray();
    }

    public async Task<UserDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Include(e => e.UserRoles!).ThenInclude(e => e.Role)
            .Where(e => e.Id == id)
            .Select(e => new 
            { 
                entity = e,
                canBeDeleted =
                    e.Bookings!.Count == 0 &&
                    e.Trips!.Count == 0 &&
                    e.TripBookings!.Count == 0 &&
                    e.VehicleBookings!.Count == 0 &&
                    e.TripChats!.Count == 0 &&
                    e.WikiArticles!.Count == 0,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null) return null;

        var idn = new IdnMapping();
        var dc = new GermanDateTimeConverter();

        return new UserDto(user.entity, user.canBeDeleted, idn, dc);
    }

    public async Task<string[]?> Create(UserDto dto, bool registerExtended, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email!);
        if (user != null)
        {
            return new[] { "Der Benutzer mit der E-Mail-Adresse existiert bereits." };
        }

        if (dto.Email == null || !await _emailValidator.Validate(dto.Email, cancellationToken))
        {
            return new[] { "Die E-Mail-Adresse ist ungültig." };
        }

        user = new IdentityUserGuid
        {
            Id = _pkGenerator.Generate(),
            UserName = Guid.NewGuid().ToString().Replace("-", string.Empty),
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DebtorNumber = dto.DebtorNumber,
            AddressNumber = dto.AddressNumber
        };
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return result.Errors.Select(e => e.Description).ToArray();
        }

        result = await _userManager.AddToRolesAsync(user, dto.Roles!);
        if (!result.Succeeded)
        {
            return result.Errors.Select(e => e.Description).ToArray();
        }

        if (!await NotifyConfirmRegistration(user, registerExtended, cancellationToken))
        {
            return new[] { "Fehler beim Speichern" };
        }

        return null;
    }

    public async Task<string?> ConfirmChangeEmail(Guid id, string token, string encodedEmail)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return null;
        }

        string? newEmail = null;

        try
        {
            var protector = _dataProtectionProvider.CreateProtector(user.SecurityStamp!);

            newEmail = Encoding.UTF8.GetString(protector.Unprotect(Convert.FromBase64String(encodedEmail)));

            token = HttpUtility.UrlDecode(token);

            var isUserTokenValid = await _userManager.VerifyUserTokenAsync(user,
                _userManager.Options.Tokens.ChangeEmailTokenProvider,
                UserManager<IdentityUserGuid>.GetChangeEmailTokenPurpose(newEmail),
                token);

            if (!isUserTokenValid)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "change email failed");
            return null;
        }

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            var error = string.Join(";", result.Errors.Select(e => e.Description));
            _logger.LogError($"change email failed: {error}");

            return null;
        }

        return newEmail;
    }

    public async Task<bool> UpdateFirebaseDeviceToken(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return false;
        }
        const string deviceTokenClaimType = "FirebaseDeviceToken";
        var claims = await _userManager.GetClaimsAsync(user);
        var oldClaim = claims.FirstOrDefault(c => c.Type == deviceTokenClaimType);
        IdentityResult result;
        if (oldClaim == null)
        {
            var newClaim = new Claim(deviceTokenClaimType, token);
            result = await _userManager.AddClaimAsync(user, newClaim);
        }
        else
        {
            var combinedToken = oldClaim.Value;
            if (!combinedToken.Contains(token))
            {
                combinedToken += "\n" + token;
            }
            var newClaim = new Claim(deviceTokenClaimType, combinedToken);
            result = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
        }
        return result.Succeeded;
    }

    private async Task<bool> NotifyConfirmRegistration(IdentityUserGuid user, bool registerExtended, CancellationToken cancellationToken)
    {
        if (_httpContext.HttpContext == null)
        {
            return false;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = HttpUtility.UrlEncode(token);

        var callbackUrl = _linkGenerator.GetUriByPage(_httpContext.HttpContext, "/Login/ConfirmRegistration", null, new { id = user.Id, token });

        if (!await _emailService.EnqueueConfirmRegistration(user, callbackUrl!, registerExtended, cancellationToken))
        {
            _logger.LogWarning($"enqueue registration email for user {user.Id} failed");
            return false;
        }

        return true;
    }

    private async Task<bool> NotifyConfirmPasswordForgotten(IdentityUserGuid user, CancellationToken cancellationToken)
    {
        if (_httpContext.HttpContext == null)
        {
            return false;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = HttpUtility.UrlEncode(token);

        var callbackUrl = _linkGenerator.GetUriByPage(_httpContext.HttpContext, "/Login/ConfirmChangePassword", null, new { id = user.Id, token });

        if (!await _emailService.EnqueueChangePassword(user, callbackUrl!, cancellationToken))
        {
            _logger.LogWarning($"enqueue change password email for user {user.Id} failed");
            return false;
        }

        return true;
    }

    private async Task<bool> NotifyConfirmChangeEmail(IdentityUserGuid user, string newEmail, CancellationToken cancellationToken)
    {
        if (_httpContext.HttpContext == null)
        {
            return false;
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        token = HttpUtility.UrlEncode(token);

        var protector = _dataProtectionProvider.CreateProtector(user.SecurityStamp!);
        var newEmailProtected = Convert.ToBase64String(protector.Protect(Encoding.UTF8.GetBytes(newEmail)));

        var callbackUrl = _linkGenerator.GetUriByPage(_httpContext.HttpContext, "/Login/ConfirmChangeEmail", null,
            new { id = user.Id, token, email = newEmailProtected });

        if (!await _emailService.EnqueueChangeEmail(user, callbackUrl!, cancellationToken))
        {
            _logger.LogWarning($"enqueue change address email for user {user.Id} failed");
            return false;
        }

        return true;
    }

    public async Task<Result<UserTwoFactor>> CreateTwoFactor(Guid id, string appName)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return Result.Fail("Benutzer wurde nicht gefunden");
        }

        var isTwoFactorEnabled = await _signInManager.IsTwoFactorEnabledAsync(user);

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            var result = await userManager.ResetAuthenticatorKeyAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
            key = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                return Result.Fail(_errorDescriber.DefaultError().Description);
            }
        }

        var uri = GenerateOtpAuthUri(appName, user.Email!, key);
        return Result.Ok(new UserTwoFactor(isTwoFactorEnabled, key, uri));
    }

    public async Task<Result> EnableTwoFactor(Guid id, bool enable, string code)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return Result.Fail("Benutzer wurde nicht gefunden");
        }

        var isValid = await userManager.VerifyTwoFactorTokenAsync(user,
            userManager.Options.Tokens.AuthenticatorTokenProvider, code);

        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidToken().Description);
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, enable);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> ResetTwoFactor(Guid id)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return Result.Fail("Benutzer wurde nicht gefunden");
        }

        user.AuthenticatorKey = null;
        var result = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    private static string GenerateOtpAuthUri(string issuer, string user, string secret)
    {
        var dictionary = new Dictionary<string, string>
        {
            { "secret", secret },
            { "issuer", Uri.EscapeDataString(issuer) },
            { "algorithm","SHA1" },
            { "digits", "6" },
            { "period", "30" }
        };

        var uri = new StringBuilder("otpauth://totp/");
        uri.Append(Uri.EscapeDataString(issuer));
        uri.Append(':');
        uri.Append(Uri.EscapeDataString(user));
        uri.Append('?');
        foreach (var item in dictionary)
        {
            uri.Append(item.Key);
            uri.Append('=');
            uri.Append(item.Value);
            uri.Append('&');
        }

        // remove '&' at the end
        uri.Remove(uri.Length - 1, 1);
        return uri.ToString();
    }
}
