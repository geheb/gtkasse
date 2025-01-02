using GtKasse.Core.Models;
using System.Reflection;

namespace GtKasse.Core.Email;

public class AccountEmailTemplateRenderer
{
    private readonly TemplateRenderer _templateRenderer;

    public AccountEmailTemplateRenderer()
    {
        _templateRenderer = new TemplateRenderer(GetType().GetTypeInfo().Assembly);
    }

    public Task<string> Render(AccountEmailTemplate emailTemplate, object model)
    {
        var templateFile = GetTemplateFile(emailTemplate);
        return _templateRenderer.Render(templateFile, model);
    }

    private static string GetTemplateFile(AccountEmailTemplate emailTemplate)
    {
        return emailTemplate switch
        {
            AccountEmailTemplate.ConfirmRegistration => "ConfirmRegistration.html",
            AccountEmailTemplate.ConfirmRegistrationExtended => "ConfirmRegistrationExtended.html",
            AccountEmailTemplate.ConfirmPasswordForgotten => "ConfirmPasswordForgotten.html",
            AccountEmailTemplate.ConfirmChangeEmail => "ConfirmChangeEmail.html",
            _ => throw new NotImplementedException($"unknown {nameof(AccountEmailTemplate)} {emailTemplate}")
        };
    }
}
