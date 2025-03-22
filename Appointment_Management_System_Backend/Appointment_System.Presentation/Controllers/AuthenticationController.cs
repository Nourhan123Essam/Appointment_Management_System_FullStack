using System.Text.Json;
using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.DTOs.Captcha;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly RecaptchaSettings _recaptchaSettings;

        public AuthenticationController(IAuthenticationService authService, IOptions<RecaptchaSettings> recaptchaSettings)
        {
            _authService = authService;
            _recaptchaSettings = recaptchaSettings.Value;
        }

        [HttpOptions]
        public IActionResult Preflight()
        {
            return NoContent(); // Respond with 204 No Content
        }


        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(RegisterDTO appUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(appUserDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify-captcha")]
        public async Task<IActionResult> VerifyCaptcha([FromBody] CaptchaRequest request)
        {
            var secretKey = _recaptchaSettings.SecretKey; // Read from appsettings.json
            var googleVerifyUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={request.RecaptchaToken}";

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(googleVerifyUrl, null);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var captchaResult = JsonSerializer.Deserialize<CaptchaResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (captchaResult?.Success == true)
            {
                return Ok(new { message = "Captcha verified successfully" });
            }
            else
            {
                return BadRequest(new { message = "Captcha verification failed" });
            }
        }


    }
}
