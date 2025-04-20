using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Services.Interfaces;


namespace Appointment_System.Application.Services.Implementaions
{
    public class DoctorService: IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get Dcotor by Id 
        public async Task<DoctorDto?> GetDoctorByIdAsync(string doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorId);
            return doctor == null ? null : new DoctorDto(doctor);
        }

        // Create Docotor
        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto dto)
        {
            return await _unitOfWork.Doctors.CreateDoctorAsync(dto);
        }

        // Get All Doctors basic Data
        public async Task<List<DoctorsBasicDataDto>> GetAllDoctorsBasicDataAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetAllDoctorsBasicDataAsync();

            return doctors.Select(d => new DoctorsBasicDataDto(d)).ToList();
        }

        // Get All Doctors
        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetAllDoctorsAsync();

            return doctors.Select(d => new DoctorDto(d)).ToList();
        }

        //Update Doctor
        public async Task<bool> UpdateDoctorAsync(string doctorId, DoctorUpdateDto dto)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return false;

            // Update only doctor details
            doctor.FullName = dto.FullName;
            doctor.Email = dto.Email;
            doctor.YearsOfExperience = dto.YearsOfExperience;
            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.WorkplaceType = dto.WorkplaceType;
            doctor.TotalRatingsGiven = dto.TotalRatingsGiven;
            doctor.TotalRatingScore = dto.TotalRatingScore;

            return await _unitOfWork.Doctors.UpdateDoctorAsync(doctor);
        }

        // Delete Doctor
        public async Task<bool> DeleteDoctorAsync(string doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return false;

            return await _unitOfWork.Doctors.DeleteDoctorAsync(doctor);
        }
    }
}
