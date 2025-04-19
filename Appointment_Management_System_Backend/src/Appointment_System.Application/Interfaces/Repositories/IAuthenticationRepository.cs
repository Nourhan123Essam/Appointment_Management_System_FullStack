
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        public Task<User> GetUserByEmailAsync(string email);
        public Task<bool> Register(User appUser, string password);
        public Task<Response> Login(User login, string password);
    }
}
