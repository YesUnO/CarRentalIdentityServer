using CarRentalIdentityServer.Models;
using CarRentalIdentityServer.Services.Emails;
using Core.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace CarRentalIdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController: ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly BaseApiUrls _baseApiUrls;

        public AdminController(ILogger<AdminController> logger, UserManager<IdentityUser> userManager, IEmailService emailService, BaseApiUrls baseApiUrls)
        {
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
            _baseApiUrls = baseApiUrls;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            try
            {
                var identityUser = new IdentityUser
                {
                    UserName= model.UserName,
                    Email= model.Email,
                };
                var creatingUserResult = await _userManager.CreateAsync(identityUser, model.Password);
                if (!creatingUserResult.Succeeded)
                {
                    _logger.LogError("Registration failed");
                    return BadRequest(creatingUserResult.Errors);
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
