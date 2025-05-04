using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    //Query
    public record GetDoctorByIdQuery(int DoctorId) : IRequest<DoctorDto?>;

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

            if (request.DoctorId <= 0)
                throw new ArgumentException("DoctorId should be greater than 0");

            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(request.DoctorId);

            return doctor == null ? null : new DoctorDto(doctor);
        }
    }


}
