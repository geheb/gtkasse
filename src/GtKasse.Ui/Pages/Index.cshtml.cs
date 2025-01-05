using GtKasse.Core.User;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages;

[Node("Startseite", IsDefault = true)]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IAntiforgery _antiforgery;
    private readonly Core.Repositories.Users _users;
    private readonly string _fcmWorkerScript = string.Empty;

    public IndexModel(
        IAntiforgery antiforgery, 
        IOptions<FirebaseCloudMessagingSettings> fcmSettings,
        Core.Repositories.Users users)
    {
        _antiforgery = antiforgery;

        if (fcmSettings.Value.HasConfig)
        {
            _fcmWorkerScript = $$"""
            const firebaseConfig = {
                apiKey: '{{fcmSettings.Value.ApiKey}}',
                projectId: '{{fcmSettings.Value.ProjectId}}',
                messagingSenderId: '{{fcmSettings.Value.MessagingSenderId}}',
                appId: '{{fcmSettings.Value.AppId}}'
            };
            importScripts('/lib/gt-fcm-wrapper/gt-fcm-worker-1.1.0.min.js');
            """;
        }

        _users = users;
    }

    public IActionResult OnGetFcmWorker() => Content(_fcmWorkerScript, "text/javascript");

    public async Task<IActionResult> OnPostDeviceTokenAsync([Required] string token)
    {
        if (User.Identity?.IsAuthenticated != true) return Unauthorized();
        if (!await _antiforgery.IsRequestValidAsync(HttpContext)) return BadRequest();
        var result = await _users.UpdateFirebaseDeviceToken(User.GetId(), token);
        return new JsonResult(result);
    }
}