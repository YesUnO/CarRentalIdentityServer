using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRentalIdentityServer.Pages.Account.Register
{
    public class Index : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;


        [BindProperty]
        public InputModel Input { get; set; }

        public Index(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost() 
        {
            return Page();
        }
    }
}
