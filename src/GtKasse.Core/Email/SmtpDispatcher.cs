namespace GtKasse.Core.Email;

using GtKasse.Core.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

public sealed class SmtpDispatcher
{
    private readonly SmtpConnectionOptions _connectionOptions;

    public SmtpDispatcher(IOptions<SmtpConnectionOptions> options)
    {
        _connectionOptions = options.Value;
    }

    public async Task Send(
        EmailItem[] items,
        CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_connectionOptions.Server, _connectionOptions.Port);
        if (!string.IsNullOrEmpty(_connectionOptions.LoginName))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true; // only Explicit SSL supported
            client.Credentials = new NetworkCredential(_connectionOptions.LoginName, _connectionOptions.LoginPassword);
        }

        foreach (var item in items)
        {
            var from = new MailAddress(_connectionOptions.SenderEmail, _connectionOptions.SenderName);
            var to = new MailAddress(item.Recipient);

            using var message = new MailMessage(from, to);
            if (!string.IsNullOrEmpty(item.ReplyTo))
            {
                message.ReplyToList.Add(new(item.ReplyTo));
            }
            message.Subject = item.Subject;
            message.Body = item.HtmlBody;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;

            await client.SendMailAsync(message, cancellationToken);
        }        
    }
}
