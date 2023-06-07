using CarRentalIdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalIdentityServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController: ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ILogger<AdminController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            try
            {
                var emailAvailable = await _userManager.FindByEmailAsync(model.Email) is null;
                var usernameAvailable = await _userManager.FindByNameAsync(model.UserName) is null;
                if (!emailAvailable)
                {
                    _logger.LogError("Email unavailable");
                    return BadRequest("Email unavailable");
                }

                if (!usernameAvailable)
                {
                    _logger.LogError("Username unavailable");
                    return BadRequest("Username unavailable");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Registration failed", ex);
                return BadRequest("Registration failed");
            }
        }


    }
}
