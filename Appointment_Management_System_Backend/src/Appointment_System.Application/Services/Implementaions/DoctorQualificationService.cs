using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Services.Interfaces;

namespace Appointment_System.Application.Services.Implementaions
{
    public class DoctorQualificationService : IDoctorQualificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorQualificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DoctorQualificationDto>> GetByDoctorIdAsync(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
                throw new ArgumentException("Doctor ID cannot be null or empty");

            var qualifications = await _unitOfWork.QualificationRepository.GetByDoctorIdAsync(doctorId);
            return qualifications.Select(q => new DoctorQualificationDto(q)).ToList();
        }

        public async Task<DoctorQualificationDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID");

            var qualification = await _unitOfWork.QualificationRepository.GetByIdAsync(id);
            return qualification == null ? null : new DoctorQualificationDto(qualification);
        }

        public async Task<DoctorQualificationDto> AddAsync(CreateDoctorQualificationDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.DoctorId))
                throw new ArgumentException("Doctor ID is required");

            var doctorExists = await _unitOfWork.Doctors.GetDoctorByIdAsync(dto.DoctorId);
            if (doctorExists == null)
                throw new KeyNotFoundException("Doctor not found");

            if (dto.YearEarned < 1900 || dto.YearEarned > DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(dto.YearEarned), "YearEarned must be between 1900 and the current year.");

            var qualification = dto.ToEntity();
            qualification.Id = await _unitOfWork.QualificationRepository.AddAsync(qualification);

            return new DoctorQualificationDto(qualification);
        }

        public async Task UpdateAsync(int id, UpdateDoctorQualificationDto dto)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var qualification = await _unitOfWork.QualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                throw new KeyNotFoundException("Doctor qualification not found");

            if (dto.YearEarned < 1900 || dto.YearEarned > DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(dto.YearEarned), "YearEarned must be between 1900 and the current year.");

            dto.UpdateEntity(qualification);
            await _unitOfWork.QualificationRepository.UpdateAsync(qualification);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID");

            var qualification = await _unitOfWork.QualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                throw new KeyNotFoundException("Doctor qualification not found");

            await _unitOfWork.QualificationRepository.DeleteAsync(id);
        }
    }


}
