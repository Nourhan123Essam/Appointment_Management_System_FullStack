using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Appointment_System.Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<DoctorBasicDto>?> GetAllDoctorsAsync()
        {
            var value = await _db.StringGetAsync("cache:doctors:basic");
            return value.HasValue ? JsonSerializer.Deserialize<List<DoctorBasicDto>>(value!) : null;
        }

        public async Task SetAllDoctorsAsync(List<DoctorBasicDto> doctors)
        {
            var json = JsonSerializer.Serialize(doctors);
            await _db.StringSetAsync("cache:doctors:basic", json, TimeSpan.FromHours(6));
        }

        public async Task RemoveDoctorsCacheAsync()
        {
            await _db.KeyDeleteAsync("cache:doctors:basic");
        }


        //*****************************************************
        private const string TopDoctorsKey = "cache:doctors:top5";

        public async Task<List<DoctorBasicDto>?> GetTop5DoctorsAsync()
        {
            var value = await _db.StringGetAsync(TopDoctorsKey);
            return value.HasValue ? JsonSerializer.Deserialize<List<DoctorBasicDto>>(value!) : null;
        }

        public async Task SetTop5DoctorsAsync(List<DoctorBasicDto> topDoctors)
        {
            var json = JsonSerializer.Serialize(topDoctors);
            await _db.StringSetAsync(TopDoctorsKey, json, TimeSpan.FromMinutes(30));
        }

    }


}
