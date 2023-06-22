using CarRentalIdentityServer.Options;
using CarRentalIdentityServer.Services.Emails;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Web;

namespace CarRentalIdentityServer.Pages.Account.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEventService _events;
        private readonly BaseApiUrls _baseApiUrls;
        private readonly IEmailService _emailService;

        public ViewModel View { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public Index(UserManager<IdentityUser> userManager,
                     SignInManager<IdentityUser> signInManager,
                     IIdentityServerInteractionService interaction,
                     IEventService events,
                     IOptions<BaseApiUrls> baseApiUrls,
                     IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
            _baseApiUrls = baseApiUrls.Value;
            _emailService = emailService;
        }

        public void OnGet(string returnUrl)
        {

            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = Input.Username,
                    Email = Input.Email
                };

                var creatingResult = await _userManager.CreateAsync(identityUser, Input.Password);
                if (!creatingResult.Succeeded)
                {
                    foreach (var error in creatingResult.Errors)
                    {
                       ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    await _userManager.AddToRoleAsync(identityUser, "Customer");

                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    await SendConfirmationMailAsync(identityUser.Email, confirmEmailToken, identityUser.UserName);

                    var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
                    var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByNameAsync(Input.Username);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                return this.LoadingPage(Input.ReturnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(Input.ReturnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(Input.ReturnUrl))
                        {
                            return Redirect(Input.ReturnUrl);
                        }
                        else if (string.IsNullOrEmpty(Input.ReturnUrl))
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            // user might have clicked on a malicious link - should be logged
                            throw new Exception("invalid return URL");
                        }
                    }
                }
            }
            var returnUrl = Input.ReturnUrl;
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }

        private async Task SendConfirmationMailAsync(string email, string confirmationEmailtoken, string name)
        {
            var subject = "Account confirmation";
            var tokenEncoded = HttpUtility.UrlEncode(confirmationEmailtoken);
            var baseUrl = new Uri(_baseApiUrls.HttpsUrl + "/api/email/ConfirmMail");
            var link = $"{baseUrl}?token={tokenEncoded}&email={email}";
            var body = $"Hello {name}," +
                $"<p>Confirm your mail address with this link: <a href=\"{link}\">confirm mail link</a></p>";
            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
