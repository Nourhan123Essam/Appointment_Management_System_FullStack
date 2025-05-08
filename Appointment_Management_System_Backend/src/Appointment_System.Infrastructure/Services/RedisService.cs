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
    }

}
