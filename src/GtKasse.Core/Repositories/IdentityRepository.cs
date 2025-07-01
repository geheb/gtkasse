using FluentResults;
using GtKasse.Core.Converter;
using GtKasse.Core.Database;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading;

namespace GtKasse.Core.Repositories;

public sealed class IdentityRepository
{
    private readonly Result _userNotFound = Result.Fail("Benutzer wurde nicht gefunden.");
    private readonly UuidPkGenerator _pkGenerator = new();
    private readonly TimeProvider _timeProvider;
    private readonly UserManager<IdentityUserGuid> _userManager;

    public IdentityRepository(
        TimeProvider timeProvider,
        UserManager<IdentityUserGuid> userManager)
    {
        _timeProvider = timeProvider;
        _userManager = userManager;
    }
    public async Task<Result> Create(IdentityDto dto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email!);
        if (user is not null)
        {
            return Result.Fail(_userManager.ErrorDescriber.DuplicateEmail(dto.Email!).Description);
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
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        result = await _userManager.AddToRolesAsync(user, dto.Roles!);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<IdentityDto[]> GetAll(CancellationToken cancellationToken)
    {
        var entities = await _userManager.Users
            .AsNoTracking()
            .Include(e => e.UserRoles!).ThenInclude(e => e.Role)
            .OrderBy(e => e.Name)
            .Where(e => e.LeftOn == null)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return [.. entities.Select(e => e.ToDto(dc))];
    }

    public async Task<IdentityDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _userManager.Users
            .AsNoTracking()
            .Include(e => e.UserRoles!).ThenInclude(e => e.Role)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return entity.ToDto(new());
    }

    public async Task<Result> Update(IdentityDto dto, CancellationToken cancellationToken)
    {
        var entity = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == dto.Id, cancellationToken);
        if (entity is null)
        {
            return _userNotFound;
        }

        if (dto.Email is not null && 
            !dto.Email.Equals(entity.Email))
        {
            var token = await _userManager.GenerateChangeEmailTokenAsync(entity, dto.Email);
            var result = await _userManager.ChangeEmailAsync(entity, dto.Email, token);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        if (dto.PhoneNumber is not null && 
            !dto.PhoneNumber.Equals(entity.PhoneNumber))
        {
            IdentityResult result;
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                result = await _userManager.SetPhoneNumberAsync(entity, string.Empty);
            }
            else
            {
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(entity, dto.PhoneNumber);
                result = await _userManager.ChangePhoneNumberAsync(entity, dto.PhoneNumber, token);
            }

            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        int count = 0;
        if (dto.Name is not null &&
            !dto.Name.Equals(entity.Name))
        {
            entity.Name = dto.Name;
            count++;
        }
        if (dto.DebtorNumber is not null &&
            !dto.DebtorNumber.Equals(entity.DebtorNumber))
        {
            entity.DebtorNumber = string.IsNullOrEmpty(dto.DebtorNumber) ? null : dto.DebtorNumber;
            count++;
        }
        if (dto.AddressNumber is not null && 
            !dto.AddressNumber.Equals(entity.AddressNumber))
        {
            entity.AddressNumber = string.IsNullOrEmpty(dto.AddressNumber) ? null : dto.AddressNumber;
            count++;
        }

        if (count > 0)
        {
            var result = await _userManager.UpdateAsync(entity);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        if (dto.Roles?.Length > 0)
        {
            var roles = await _userManager.GetRolesAsync(entity);
            var removeRoles = roles.Except(dto.Roles).ToArray();
            var addRoles = dto.Roles != null ? dto.Roles.Except(roles).ToArray() : [];

            if (removeRoles.Length > 0)
            {
                var result = await _userManager.RemoveFromRolesAsync(entity, removeRoles);
                if (!result.Succeeded)
                {
                    return Result.Fail(result.Errors.Select(e => e.Description));
                }
            }

            if (addRoles.Length > 0)
            {
                var result = await _userManager.AddToRolesAsync(entity, addRoles);
                if (!result.Succeeded)
                {
                    return Result.Fail(result.Errors.Select(e => e.Description));
                }
            }
        }

        return Result.Ok();
    }

    public async Task<Result> Remove(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
        {
            return _userNotFound;
        }

        var result = await _userManager.RemovePasswordAsync(entity);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        var roles = await _userManager.GetRolesAsync(entity);
        if (roles.Any())
        {
            result = await _userManager.RemoveFromRolesAsync(entity, roles);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        entity.Email = entity.UserName + "@removed";
        entity.DebtorNumber = null;
        entity.AddressNumber = null;
        entity.LeftOn = _timeProvider.GetUtcNow();
        entity.Name = new string(entity.Name?.Split(' ', '-').Select(u => u[0]).ToArray()) + "*";

        result = await _userManager.UpdateAsync(entity);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> UpdateLoginSucceeded(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
        {
            return _userNotFound;
        }

        entity.LastLogin = _timeProvider.GetUtcNow();
        entity.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(entity);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }
}
