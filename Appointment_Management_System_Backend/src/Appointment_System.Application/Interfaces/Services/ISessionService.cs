namespace Appointment_System.Application.Interfaces.Services
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(string userId);
        Task<bool> ValidateSessionExistsAsync(string userId);
        Task<bool> ValidateSessionAsync(string userId, string sessionId);
        Task<bool> ValidateAndExtendSessionAsync(string userId, string sessionId);
        Task RemoveSessionAsync(string userId);
    }
}
