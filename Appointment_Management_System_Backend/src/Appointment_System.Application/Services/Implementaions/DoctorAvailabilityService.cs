using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Repositories.Interfaces;

namespace Appointment_System.Application.Services.Implementaions
{

    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly IDoctorAvailabilityRepository _availabilityRepository;
        private readonly IDoctorRepository _doctorRepository;

        public DoctorAvailabilityService(IDoctorAvailabilityRepository availabilityRepository, IDoctorRepository doctorRepository)
        {
            _availabilityRepository = availabilityRepository;
            _doctorRepository = doctorRepository;
        }


        public async Task<DoctorAvailabilityDto?> GetByIdAsync(int id)
        {
            var availability = await _availabilityRepository.GetByIdAsync(id);
            return availability == null ? null : new DoctorAvailabilityDto(availability);
        }

        public async Task<IEnumerable<DoctorAvailabilityDto>> GetByDoctorIdAsync(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
                throw new ArgumentNullException(nameof(doctorId), "DoctorId cannot be null or empty.");

            var doctorExists = await _doctorRepository.GetDoctorByIdAsync(doctorId) != null;
            if (!doctorExists)
                throw new KeyNotFoundException($"Doctor with ID {doctorId} does not exist.");

            var availabilities = await _availabilityRepository.GetByDoctorIdAsync(doctorId);
            return availabilities.Select(a => new DoctorAvailabilityDto(a));
        }

        public async Task<DoctorAvailabilityDto> AddAsync(CreateDoctorAvailabilityDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.DoctorId))
                throw new ArgumentException("DoctorId is required.", nameof(dto.DoctorId));

            var doctorExists = await _doctorRepository.GetDoctorByIdAsync(dto.DoctorId) != null;
            if (!doctorExists)
                throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} does not exist.");


            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be earlier than EndTime");

            var availability = dto.ToEntity();

            availability.Id = await _availabilityRepository.AddAsync(availability);
            return new DoctorAvailabilityDto(availability);
        }

        public async Task DeleteAsync(int id)
        {
            var availability = await _availabilityRepository.GetByIdAsync(id);
            if (availability == null)
                throw new KeyNotFoundException($"Doctor availability with ID {id} not found.");

            await _availabilityRepository.DeleteAsync(id);
        }

        public async Task UpdateAsync(int id, UpdateDoctorAvailabilityDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be earlier than EndTime");

            var availability = await _availabilityRepository.GetByIdAsync(id);
            if (availability == null)
                throw new KeyNotFoundException("Doctor availability not found.");

            availability.DayOfWeek = (DayOfWeek)dto.DayOfWeek;
            availability.StartTime = dto.StartTime;
            availability.EndTime = dto.EndTime;

            await _availabilityRepository.UpdateAsync(availability);
        }
    }


}
