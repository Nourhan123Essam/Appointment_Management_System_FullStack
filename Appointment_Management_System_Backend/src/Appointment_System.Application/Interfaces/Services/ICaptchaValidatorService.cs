
namespace Appointment_System.Application.Interfaces.Services
{
    public interface ICaptchaValidatorService
    {
        Task<bool> ValidateCaptchaAsync(string token);
    }

}
