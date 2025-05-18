
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        public Task<bool> Register(Patient appUser, string password);
        public Task<bool> IsUserExist(string userId);
        public Task<Result<LoginResult>> Login(string email, string password);
        public Task<string?> GenerateTokenAsync(string userId);
        public string GenerateRefreshToken();
        Task<bool> UpdatePasswordAsync(string userId, string newPassword);
        public Task<string?>  GetUserIdByEmailAsync(string email);


    }
}
