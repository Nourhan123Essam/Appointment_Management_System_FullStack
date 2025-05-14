using Appointment_System.Application.Interfaces.Services;
using StackExchange.Redis;

namespace Appointment_System.Infrastructure.Services
{
    public class SessionService : ISessionService
    {
        private readonly IDatabase _db;
        private readonly TimeSpan _sessionDuration = TimeSpan.FromMinutes(30);

        public SessionService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<string> CreateSessionAsync(string userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            await _db.StringSetAsync($"session:{userId}", sessionId, _sessionDuration);
            return sessionId;
        }

        public async Task<bool> ValidateSessionExistsAsync(string userId)
        {
            var session = await _db.StringGetAsync($"session:{userId}");
            return !session.IsNullOrEmpty;
        }

        public async Task<bool> ValidateSessionAsync(string userId, string sessionId)
        {
            var storedSession = await _db.StringGetAsync($"session:{userId}");
            return storedSession == sessionId;
        }

        public async Task<bool> ValidateAndExtendSessionAsync(string userId, string sessionId)
        {
            var key = $"session:{userId}";
            var storedSessionId = await _db.StringGetAsync(key);

            if (storedSessionId != sessionId)
                return false;

            await _db.KeyExpireAsync(key, _sessionDuration);
            return true;
        }

        public async Task RemoveSessionAsync(string userId)
        {
            await _db.KeyDeleteAsync($"session:{userId}");
        }
    }
}
