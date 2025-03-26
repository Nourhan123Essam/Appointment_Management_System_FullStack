using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.DoctorQualification;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IDoctorQualificationService
    {
        Task<List<DoctorQualificationDto>> GetByDoctorIdAsync(string doctorId);
        Task<DoctorQualificationDto?> GetByIdAsync(int id);
        Task AddAsync(CreateDoctorQualificationDto dto);
        Task UpdateAsync(int id, UpdateDoctorQualificationDto dto);
        Task DeleteAsync(int id);
    }

}
