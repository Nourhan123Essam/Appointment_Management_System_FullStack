
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;

namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        public Task<bool> Register(Patient appUser, string password);
        public Task<Response> Login(string email, string password);
    }
}
