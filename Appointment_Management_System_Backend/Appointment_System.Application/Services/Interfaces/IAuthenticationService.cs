using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Domain.Responses;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Response> RegisterAsync(RegisterDTO request);
        Task<Response> LoginAsync(LoginDTO request);
    }
}
