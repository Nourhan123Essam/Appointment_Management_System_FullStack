namespace Appointment_System.Application.Interfaces.Services
{
    public interface IRedisService
    {
        Task SetRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiry);
        Task<string?> GetRefreshTokenAsync(string userId);
        Task DeleteRefreshTokenAsync(string userId);
    }

}
