using Appointment_System.Application.Interfaces.Services;
using StackExchange.Redis;

namespace Appointment_System.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        // Refresh token
        public async Task SetRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiry)
        {
            // Key = refreshToken, Value = userId
            await _db.StringSetAsync($"refreshToken:{refreshToken}", userId, expiry);

        }

        public async Task<string?> GetRefreshTokenAsync(string refreshToken)
        {
            return await _db.StringGetAsync($"refreshToken:{refreshToken}");
        }

        public async Task DeleteRefreshTokenAsync(string userId)
        {
            await _db.KeyDeleteAsync($"refreshToken:{userId}");
        }

        /////////////////////////////////////////////////////////
        // Reset password
        // Set token with token as key and userId as value
        public Task SetResetPasswordTokenAsync(string token, string userId, TimeSpan expiry) =>
            _db.StringSetAsync($"resetPasswordToken:{token}", userId, expiry);

        public async Task<string> GetResetPasswordTokenAsync(string token)
        {
            return await _db.StringGetAsync($"resetPasswordToken:{token}");
        }

        public async Task DeleteResetPasswordTokenAsync(string token)
        {
            await _db.KeyDeleteAsync($"resetPasswordToken:{token}");
        }

    }

}
