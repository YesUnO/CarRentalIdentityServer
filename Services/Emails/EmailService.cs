using CarRentalIdentityServer.Options;
using CarRentalIdentityServer.Services.Emails.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Web;

namespace CarRentalIdentityServer.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailsSettings _emailsSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly BaseApiUrls _baseApiUrls;

        public EmailService(IOptions<EmailsSettings> emailsSettings, ILogger<EmailService> logger, IOptions<BaseApiUrls >baseApiUrls)
        {
            _emailsSettings = emailsSettings.Value;
            _logger = logger;
            _baseApiUrls = baseApiUrls.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var emailMsg = new MimeMessage();
            emailMsg.From.Add(MailboxAddress.Parse(_emailsSettings.SenderAddress));
            emailMsg.To.Add(MailboxAddress.Parse(email));
            emailMsg.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            emailMsg.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient();

            try
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync(_emailsSettings.SenderAddress, _emailsSettings.Password);
                await client.SendAsync(emailMsg);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sending mail failed");
                throw;
            }
        }


        public async Task SendConfirmationMailAsync(string email, string confirmationEmailtoken, string name)
        {
            var subject = "Account confirmation";
            var tokenEncoded = HttpUtility.UrlEncode(confirmationEmailtoken);
            var baseUrl = new Uri(_baseApiUrls.HttpsUrl + "/api/email/ConfirmMail");
            var link = $"{baseUrl}?token={tokenEncoded}&email={email}";
            var body = $"Hello {name}," +
                $"<p>Confirm your mail address with this link: <a href=\"{link}\">confirm mail link</a></p>";
            await SendEmailAsync(email, subject, body);
        }
    }
}
