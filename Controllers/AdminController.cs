using CarRentalIdentityServer.Models;
using CarRentalIdentityServer.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Duende.IdentityServer.IdentityServerConstants;

namespace CarRentalIdentityServer.Controllers
{
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
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
                    UserName = model.Username,
                    Email = model.Email,
                };
                var creatingUserResult = await _userManager.CreateAsync(identityUser, model.Password);
                if (!creatingUserResult.Succeeded)
                {
                    _logger.LogError("Registration failed", creatingUserResult.Errors);
                    return BadRequest(new RegisterResponseModel { Succeeded = false, Errors = creatingUserResult.Errors });
                }
                await _userManager.AddToRoleAsync(identityUser, "Customer");
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                return Ok(new RegisterResponseModel { Succeeded = true, EmailConfirmationToken = confirmEmailToken });
            }
            catch (Exception ex)
            {
                _logger.LogError("Registration failed", ex);
                return BadRequest(new RegisterResponseModel { Succeeded = false, Exception = ex });
            }
        }

        [HttpGet]
        [Route("EmailConfirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmailConfirmationToken([FromQuery] string email)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                return Ok(new { ConfirmEmailToken = confirmEmailToken });
            }
            catch (Exception ex)
            {
                _logger.LogError("Get email tonfirmation token failed.", ex);
                return BadRequest("Get email tonfirmation token failed.");
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
