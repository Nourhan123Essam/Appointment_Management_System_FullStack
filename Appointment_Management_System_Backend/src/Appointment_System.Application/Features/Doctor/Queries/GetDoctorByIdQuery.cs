using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    //Query
    public record GetDoctorByIdQuery(string DoctorId) : IRequest<DoctorDto?>;

    //Handler
    public class GetDoctorByIdHandler : IRequestHandler<GetDoctorByIdQuery, DoctorDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDoctorByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorDto?> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.DoctorId))
                throw new ArgumentException("Doctor ID cannot be null or empty.");

            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(request.DoctorId);

            return doctor == null ? null : new DoctorDto(doctor);
        }
    }


}
