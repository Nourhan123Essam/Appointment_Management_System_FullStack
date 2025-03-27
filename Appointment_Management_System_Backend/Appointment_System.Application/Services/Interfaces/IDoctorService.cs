using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.Doctor;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<bool> CreateDoctorAsync(DoctorCreateDto dto);
    }
}
