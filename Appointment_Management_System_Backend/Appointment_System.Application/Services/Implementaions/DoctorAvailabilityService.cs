using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Repositories.Interfaces;

namespace Appointment_System.Application.Services.Implementaions
{

    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly IDoctorAvailabilityRepository _repository;

        public DoctorAvailabilityService(IDoctorAvailabilityRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<DoctorAvailabilityDto?> GetByIdAsync(int id)
        {
            var availability = await _repository.GetByIdAsync(id);
            return availability == null ? null : new DoctorAvailabilityDto(availability);
        }

        public async Task<IEnumerable<DoctorAvailabilityDto>> GetByDoctorIdAsync(string doctorId)
        {
            var availabilities = await _repository.GetByDoctorIdAsync(doctorId);
            return availabilities.Select(a => new DoctorAvailabilityDto(a));
        }

        public async Task AddAsync(CreateDoctorAvailabilityDto dto)
        {
            var availability = new DoctorAvailability
            {
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DoctorId = dto.DoctorId
            };
            await _repository.AddAsync(availability);
        }

      
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task UpdateAsync(int id, UpdateDoctorAvailabilityDto dto)
        {
            var availability = await _repository.GetByIdAsync(id);
            if (availability == null)
                throw new KeyNotFoundException("Doctor availability not found");

            availability.DayOfWeek = dto.DayOfWeek;
            availability.StartTime = dto.StartTime;
            availability.EndTime = dto.EndTime;

            await _repository.UpdateAsync(availability);
        }

    }

}
