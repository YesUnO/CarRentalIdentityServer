using Microsoft.AspNetCore.Identity;

namespace CarRentalIdentityServer.Models
{
    public class RegisterResponseModel
    {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
        public Exception Exception { get; set; }
        public string EmailConfirmationToken { get; set; }
    }
}
