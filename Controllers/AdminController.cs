using CarRentalIdentityServer.Models;
using CarRentalIdentityServer.Options;
using CarRentalIdentityServer.Services.Emails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;
using static Duende.IdentityServer.IdentityServerConstants;

namespace CarRentalIdentityServer.Controllers
{
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController: ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BaseApiUrls _baseApiUrls;

        public AdminController(ILogger<AdminController> logger, UserManager<IdentityUser> userManager, IOptions<BaseApiUrls> baseApiUrls)
        {
            _logger = logger;
            _userManager = userManager;
            _baseApiUrls = baseApiUrls.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            try
            {
                var identityUser = new IdentityUser
                {
                    UserName= model.Username,
                    Email= model.Email,
                };
                var creatingUserResult = await _userManager.CreateAsync(identityUser, model.Password);
                if (!creatingUserResult.Succeeded)
                {
                    _logger.LogError("Registration failed", creatingUserResult.Errors);
                    return BadRequest(creatingUserResult.Errors);
                }
                var confirmAccountToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                return Ok(confirmAccountToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Registration failed", ex);
                return BadRequest("Registration failed");
            }
        }


        [HttpGet]
        [Route("ConfirmMail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmMail([FromQuery] ConfirmEmailModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var res = await _userManager.ConfirmEmailAsync(user, model.Token);

                var frontEndUrl = _baseApiUrls.FrontEndUrl;
                return Redirect($"{frontEndUrl}/confirmEmail");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Confirming mail failed.");
                return BadRequest();
            }

        }
    }
}
