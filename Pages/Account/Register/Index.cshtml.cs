using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalIdentityServer.Pages.Account.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ViewModel View { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public Index(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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
                var user = new IdentityUser
                {
                    UserName = Input.Username,
                    Email = Input.Email
                };

                var creatingResult = await _userManager.CreateAsync(user, Input.Password);
                if (!creatingResult.Succeeded)
                {
                    foreach (var error in creatingResult.Errors)
                    {
                       ModelState.AddModelError(ParseIdentityErrorCodesToFields(error.Code), error.Description);
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
