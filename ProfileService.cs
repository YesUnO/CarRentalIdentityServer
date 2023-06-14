using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using System.Security.Claims;

namespace duende
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var additionalClaims = context.Subject.FindAll(FilterClaims);
            context.IssuedClaims.AddRange(additionalClaims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }

        bool FilterClaims(Claim claim)
        {
            return claim.Type.Equals(JwtClaimTypes.Name, StringComparison.OrdinalIgnoreCase) ||
                   claim.Type.Equals(JwtClaimTypes.Email, StringComparison.OrdinalIgnoreCase) ||
                   claim.Type.Equals(JwtClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                   claim.Type.Equals(JwtClaimTypes.EmailVerified, StringComparison.OrdinalIgnoreCase);
        }
    }
}
