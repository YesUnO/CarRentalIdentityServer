// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace CarRentalIdentityServer.Pages.Account.Register
{
    public class ViewModel
    {

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public string ExternalLoginScheme => ExternalProviders?.SingleOrDefault()?.AuthenticationScheme;

        public class ExternalProvider
        {
            public string DisplayName { get; set; }
            public string AuthenticationScheme { get; set; }
        }
    }
}