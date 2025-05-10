namespace Appointment_System.Application.Interfaces.Services
{
    public interface IRedisService
    {
        Task SetRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiry);
        Task<string?> GetRefreshTokenAsync(string userId);
        Task DeleteRefreshTokenAsync(string userId);

        public Task SetResetPasswordTokenAsync(string token, string userId, TimeSpan expiry);
        public Task<string?> GetResetPasswordTokenAsync(string token);
        public Task DeleteResetPasswordTokenAsync(string token);

    }

}
