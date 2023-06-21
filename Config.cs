using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace CarRentalIdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("verification",new string[] { "emailConfirmationToken" } )
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
                {
                new Client
                {
                    ClientId = "register",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("yo") },
                    RequireClientSecret= false,

                    AllowedScopes = { IdentityServerConstants.LocalApi.ScopeName }
                },

                new Client
                {
                    ClientId = "customer",
                    ClientSecrets = { new Secret("yo") },
                    RequireClientSecret= false,

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5173/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5173/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5173/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "verification" }
                },
            };
    }
}