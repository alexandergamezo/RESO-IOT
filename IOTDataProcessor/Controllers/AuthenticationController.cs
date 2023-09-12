using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace IOTDataProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthenticationController(IIdentityService identityService)
        {
            _identityService = identityService;          
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var registratioResult = await _identityService.RegisterUserAsync(registerModel);

            if(registratioResult.Success)
            {
                return Ok("User created.");
            }

            return BadRequest($"User could not be created. {registratioResult.ErrorMessage}");
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var regisrationResult = await _identityService.LoginUserAsync(loginModel);

            if(regisrationResult != null)
            {

                return Ok(regisrationResult);
            }

            return BadRequest("Invalid login or password.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestModel tokenRequestModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var result = await _identityService.VerifyAndGenerateTokenAsync(tokenRequestModel);
            
            return Ok(result);
        }

        [HttpPost("sign-out")]
        public async Task<IActionResult> SigningOut([FromBody] TokenRequestModel tokenRequestModel)
        {
            await _identityService.SignOutAsync(tokenRequestModel);
            return Ok("Signed out");
        }
    }
}
