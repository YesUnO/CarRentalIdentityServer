using CarRentalIdentityServer.Services.Emails.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CarRentalIdentityServer.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailsSettings _emailsSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailsSettings> emailsSettings, ILogger<EmailService> logger)
        {
            _emailsSettings = emailsSettings.Value;
            _logger = logger;
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
    }
}
