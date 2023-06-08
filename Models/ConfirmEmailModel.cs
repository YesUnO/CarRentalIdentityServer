namespace CarRentalIdentityServer.Models
{
    public class ConfirmEmailModel
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
