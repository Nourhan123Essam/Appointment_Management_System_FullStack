using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{
    //Query
    public class GetAllDoctorsQuery : IRequest<List<DoctorDto>>
    {
    }

    //Handler
    public class GetAllDoctorsHandler : IRequestHandler<GetAllDoctorsQuery, List<DoctorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDoctorsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DoctorDto>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            var doctors = await _unitOfWork.Doctors.GetAllDoctorsAsync();
            return doctors.Select(d => new DoctorDto(d)).ToList();
        }
    }

}
