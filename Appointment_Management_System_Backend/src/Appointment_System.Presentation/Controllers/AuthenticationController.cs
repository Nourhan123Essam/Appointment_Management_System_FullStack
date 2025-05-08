using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.DTOs.Captcha;
using Appointment_System.Application.Features.Authentication.Commands;
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

        public AuthenticationController(IMediator mediator, ICaptchaValidatorService captchaValidatorService)
        {
            _mediator = mediator;
            _captchaValidatorService = captchaValidatorService;
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

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromServices] IEmailService emailService)
        {
            await emailService.SendEmailAsync("nourhan.essam.makhlouf@gmail.com", "Test Email", "This is a test email.");
            return Ok("Email sent");
        }

    }
}
