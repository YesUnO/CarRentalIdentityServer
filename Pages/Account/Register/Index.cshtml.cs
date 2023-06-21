using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalIdentityServer.Pages.Account.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ViewModel View { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public Index(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
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

                    var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
                    var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);
                }
            }
            var returnUrl = Input.ReturnUrl;
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }


        private string ParseIdentityErrorCodesToFields(string code) => code switch
        {
            "ConcurrencyFailure" => "concurrency",
            "DefaultError" => "default",
            "DuplicateEmail" => "Email",
            "DuplicateRoleName" => "roleName",
            "DuplicateUserName" => "Username",
            "InvalidEmail" => "email",
            "InvalidRoleName" => "roleName",
            "InvalidToken" => "token",
            "InvalidUserName" => "UserName",
            "LoginAlreadyAssociated" => "login",
            "PasswordMismatch" => "Password",
            "PasswordRequiresDigit" => "Password",
            "PasswordRequiresLower" => "Password",
            "PasswordRequiresNonAlphanumeric" => "Password",
            "PasswordRequiresUniqueChars" => "Password",
            "PasswordRequiresUpper" => "Password",
            "PasswordTooShort" => "Password",
            "RecoveryCodeRedemptionFailed" => "recoveryCode",
            "UserAlreadyHasPassword" => "Password",
            "UserAlreadyInRole" => "roleName",
            "UserLockoutNotEnabled" => "lockout",
            "UserNotInRole" => "roleName"
        };
    }
}
