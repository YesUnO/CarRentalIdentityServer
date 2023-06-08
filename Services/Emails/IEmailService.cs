namespace CarRentalIdentityServer.Services.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
