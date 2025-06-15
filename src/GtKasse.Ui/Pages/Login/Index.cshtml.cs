using GtKasse.Ui.Annotations;
using GtKasse.Ui.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Login;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly Core.Repositories.Users _users;

    [BindProperty]
    public string? UserName { get; set; }

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Passwort")]
    [RequiredField, PasswordLengthField(MinimumLength = 8)] // old passwords has 8
    public string? Password { get; set; }

    public string? Message { get; set; }

    public IndexModel(ILogger<IndexModel> logger, Core.Repositories.Users users)
    {
        _logger = logger;
        _users = users;
    }

    public void OnGet(int message = 0)
    {
        if (message == 1)
        {
            Message = "Das Passwort wurde geändert. Melde dich jetzt mit dem neuen Passwort an.";
        }
        else if (message == 2)
        {
            Message = "Falls deine E-Mail-Adresse existiert, wird eine E-Mail an diese versendet. Mit Hilfe der E-Mail kannst du das Passwort ändern.";
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            _logger.LogWarning($"suspicious activity: {HttpContext.Connection.RemoteIpAddress} ({UserName})");
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        var result = await _users.SignIn(Email!, Password!);
        if (!string.IsNullOrEmpty(result.Error))
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("ConfirmTwoFactor", new { returnUrl });
        }

        return !string.IsNullOrEmpty(returnUrl) ? LocalRedirect(returnUrl) : Redirect("/");
    }
}
