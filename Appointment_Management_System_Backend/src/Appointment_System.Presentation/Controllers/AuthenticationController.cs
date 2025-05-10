using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.DTOs.Captcha;
using Appointment_System.Application.Features.Authentication.Commands;
using Appointment_System.Application.Helpers;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RecaptchaSettings _recaptchaSettings;
        private readonly ICaptchaValidatorService _captchaValidatorService;

        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;


        public AuthenticationController(IMediator mediator, 
            ICaptchaValidatorService captchaValidatorService,
            IEmailService emailService, IConfiguration configuration)
        {
            _mediator = mediator;
            _captchaValidatorService = captchaValidatorService;

            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response<string>>> Register(RegisterDTO appUserDTO)
        {
            var result = await _mediator.Send(new RegisterCommand(appUserDTO));
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response<LoginResult>>> Login(LoginDTO loginDTO)
        {
            var result = await _mediator.Send(new LoginCommand(loginDTO));
            return result.Succeeded ? Ok(result) : BadRequest(result);
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
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));
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
