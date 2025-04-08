using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Infrastructure.Repositories.Interfaces;

namespace Appointment_System.Application.Services.Implementaions
{
    public class DoctorQualificationService : IDoctorQualificationService
    {
        private readonly IDoctorQualificationRepository _repository;

        public DoctorQualificationService(IDoctorQualificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DoctorQualificationDto>> GetByDoctorIdAsync(string doctorId)
        {
            var qualifications = await _repository.GetByDoctorIdAsync(doctorId);
            return qualifications.Select(q => 
                new DoctorQualificationDto(q)).ToList();
        }

        public async Task<DoctorQualificationDto?> GetByIdAsync(int id)
        {
            var qualification = await _repository.GetByIdAsync(id);
            return qualification == null ? null : 
                new DoctorQualificationDto(qualification);
        }

        public async Task<DoctorQualificationDto> AddAsync(CreateDoctorQualificationDto dto)
        {
            var qualification = dto.ToEntity();
            qualification.Id = await _repository.AddAsync(qualification);
            var newQualification = new DoctorQualificationDto(qualification);
            return newQualification;
        }

        public async Task UpdateAsync(int id, UpdateDoctorQualificationDto dto)
        {
            var qualification = await _repository.GetByIdAsync(id);
            if (qualification == null)
                throw new KeyNotFoundException("Doctor qualification not found");

            dto.UpdateEntity(qualification);
            await _repository.UpdateAsync(qualification);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }

}
