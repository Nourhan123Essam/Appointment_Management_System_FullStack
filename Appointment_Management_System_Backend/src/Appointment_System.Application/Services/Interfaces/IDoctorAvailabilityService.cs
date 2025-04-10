﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Domain.Entities;

namespace Appointment_System.Application.Services.Interfaces
{
    public interface IDoctorAvailabilityService
    {
        Task<DoctorAvailabilityDto?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorAvailabilityDto>> GetByDoctorIdAsync(string doctorId);
        Task<DoctorAvailabilityDto> AddAsync(CreateDoctorAvailabilityDto dto);
        Task UpdateAsync(int id, UpdateDoctorAvailabilityDto dto);
        Task DeleteAsync(int id);
    }

}
