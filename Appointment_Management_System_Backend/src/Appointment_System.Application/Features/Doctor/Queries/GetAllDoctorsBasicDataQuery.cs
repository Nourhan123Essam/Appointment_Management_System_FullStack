using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Queries
{

    //Query
    public class GetAllDoctorsBasicDataQuery : IRequest<List<DoctorsBasicDataDto>>
    {
    }

    //Handler
    public class GetAllDoctorsBasicDataHandler : IRequestHandler<GetAllDoctorsBasicDataQuery, List<DoctorsBasicDataDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDoctorsBasicDataHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DoctorsBasicDataDto>> Handle(GetAllDoctorsBasicDataQuery request, CancellationToken cancellationToken)
        {
            var doctors = await _unitOfWork.Doctors.GetAllDoctorsBasicDataAsync();
            return doctors.Select(d => new DoctorsBasicDataDto(d)).ToList();
        }
    }

}
