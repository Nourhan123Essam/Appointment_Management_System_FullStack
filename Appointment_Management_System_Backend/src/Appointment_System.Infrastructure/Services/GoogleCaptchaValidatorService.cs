using System.Text.Json;
using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.DTOs.Captcha;
using Appointment_System.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Appointment_System.Infrastructure.Services
{
    public class GoogleCaptchaValidatorService : ICaptchaValidatorService
    {
        private readonly RecaptchaSettings _settings;
        private readonly HttpClient _httpClient;

        public GoogleCaptchaValidatorService(IOptions<RecaptchaSettings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
        }

        public async Task<bool> ValidateCaptchaAsync(string token)
        {
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_settings.SecretKey}&response={token}";
            var response = await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<CaptchaResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Success ?? false;
        }
    }

}
