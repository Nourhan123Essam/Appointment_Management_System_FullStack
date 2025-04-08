using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Responses;
using Appointment_System.Infrastructure.Data;

namespace Appointment_System.Infrastructure.Repositories.Interfaces
{
    public interface IAuthenticationRepository
    {
        public Task<ApplicationUser> GetUserByEmailAsync(string email);
        public Task<bool> Register(ApplicationUser appUser, string password);
        public Task<Response> Login(ApplicationUser login, string password);
    }
}
