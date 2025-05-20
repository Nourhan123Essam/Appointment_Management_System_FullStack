using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.DTOs.Captcha;
using Appointment_System.Application.Features.Authentication.Commands;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Appointment_System.Infrastructure.Services;
using Appointment_System.Application.Features.Authentication.Queries;
using Microsoft.AspNetCore.Authentication;
using Appointment_System.Presentation.Middlewares;
using Appointment_System.Application.Localization;


namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICaptchaValidatorService _captchaValidatorService;
        private readonly ISessionService _sessionService;
        private readonly ILocalizationService _localization;


        public AuthenticationController(
            IMediator mediator, 
            ICaptchaValidatorService captchaValidatorService,
            ISessionService sessionService,
            ILocalizationService localization
        ){
            _mediator = mediator;
            _captchaValidatorService = captchaValidatorService;
            _sessionService = sessionService;
            _localization = localization;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Result<string>>> Register(RegisterDTO appUserDTO)
        {
            var result = await _mediator.Send(new RegisterCommand(appUserDTO));
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result<LoginResult>>> Login(LoginDTO loginDTO)
        {
            var userId = await _mediator.Send(new GetUserIdByEmailQuery(loginDTO.Email));
            if (userId == null)
                return BadRequest(new { message = _localization["InvalidEmail"] });

            var sessionExist = await _sessionService.ValidateSessionExistsAsync(userId);
            if(sessionExist)
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Status = "Forbidden",
                    Message = _localization["AlreadySignedInOnAnotherDevice"]
                });


            var result = await _mediator.Send(new LoginCommand(loginDTO));

            if (!result.Succeeded)
                return BadRequest(result);

            var loginData = result.Data;
  
            HttpContext.Response.Cookies.Append("SessionId", loginData.SessionId, new CookieOptions
            {
                HttpOnly = true,           // Prevent access from JavaScript
                Secure = true,             // Use only over HTTPS
                SameSite = SameSiteMode.None, // cross-site cookie use
            });

            return Ok(result);
        }


        [HttpPost("verify-captcha")]
        public async Task<IActionResult> VerifyCaptcha([FromBody] CaptchaRequest request)
        {
            var isValid = await _captchaValidatorService.ValidateCaptchaAsync(request.RecaptchaToken);

            if (isValid)
                return Ok(new { message = "Captcha verified successfully" });

            return BadRequest(new { message = "Captcha verification failed" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var sessionId = Request.Cookies["SessionId"]; // Read session ID from cookie

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken, sessionId));

            if (!result.Succeeded)
                return Unauthorized(result.Message);

            return Ok(result.Data);
        }


        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword([FromBody] RequestPasswordResetCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var result = await _mediator.Send(new LogoutCommand(refreshToken));
            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

    }
}
