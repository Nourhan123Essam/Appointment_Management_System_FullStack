namespace Appointment_System.Application.Interfaces.Services
{
    public interface IRedisService
    {
        Task SetRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiry);
        Task<string?> GetUserIdByRefreshTokenAsync(string refreshToken);
        Task DeleteRefreshTokenAsync(string refreshToken);

        public Task SetResetPasswordTokenAsync(string token, string userId, TimeSpan expiry);
        public Task<string?> GetUserIdByResetPasswordTokenAsync(string token);
        public Task DeleteResetPasswordTokenAsync(string token);

    }

}
