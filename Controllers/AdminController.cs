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
    public class Email : ControllerBase
    {
        private readonly ILogger<Email> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BaseApiUrls _baseApiUrls;

        public Email(ILogger<Email> logger, UserManager<IdentityUser> userManager, IOptions<BaseApiUrls> baseApiUrls)
        {
            _logger = logger;
            _userManager = userManager;
            _baseApiUrls = baseApiUrls.Value;
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
