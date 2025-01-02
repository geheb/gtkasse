using GtKasse.Ui.Converter;
using GtKasse.Ui.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKasse.Ui.Pages.Login;

[AllowAnonymous]
public class ConfirmChangeEmailModel : PageModel
{
    private readonly Core.Repositories.Users _users;
    private readonly ILogger<ConfirmChangeEmailModel> _logger;

    public string? ConfirmedEmail { get; set; }

    public ConfirmChangeEmailModel(Core.Repositories.Users users, ILogger<ConfirmChangeEmailModel> logger)
    {
        _users = users;
        _logger = logger;
    }

    public async Task OnGetAsync(Guid id, string token, string email)
    {
        if (id == Guid.Empty || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            _logger.LogWarning($"suspicious activity: {HttpContext.Connection.RemoteIpAddress} ({id}, {token}, {email})");
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRequest);
            return;
        }

        var newEmail = await _users.ConfirmChangeEmail(id, token, email);
        if (string.IsNullOrEmpty(newEmail))
        {
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidNewEmailConfirmationLink);
            return;
        }

        ConfirmedEmail = new EmailConverter().Anonymize(newEmail);
    }
}
