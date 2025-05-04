using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorQualifications.Queries
{
    // Query
    public record GetDoctorQualificationsByDoctorIdQuery(int DoctorId) : IRequest<List<DoctorQualificationDto>>;

    //Handler
    public class GetDoctorQualificationsByDoctorIdHandler : IRequestHandler<GetDoctorQualificationsByDoctorIdQuery, List<DoctorQualificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDoctorQualificationsByDoctorIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DoctorQualificationDto>> Handle(GetDoctorQualificationsByDoctorIdQuery request, CancellationToken cancellationToken)
        {

            if (request.DoctorId <= 0)
                throw new ArgumentException("DoctorId should be greater than 0");

            var qualifications = await _unitOfWork.QualificationRepository.GetByDoctorIdAsync(request.DoctorId);
            return qualifications.Select(q => new DoctorQualificationDto(q)).ToList();
        }
    }

}
