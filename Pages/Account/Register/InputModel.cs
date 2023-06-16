using System.ComponentModel.DataAnnotations;

namespace CarRentalIdentityServer.Pages.Account.Register
{
    public class InputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }

        public string Button { get; set; }
    }
}