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
                new IdentityResource("cred", new string[] {"role", "email"}),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("register"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
                {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "register",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("yo") },

                    AllowedScopes = { "register" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("yo") },
                    RequireClientSecret= false,

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5173/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5173/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5173/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "cred" }
                },
            };
    }
}