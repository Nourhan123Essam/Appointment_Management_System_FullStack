using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorQualifications.Queries
{
    // Query
    public record GetDoctorQualificationsByDoctorIdQuery(string DoctorId) : IRequest<List<DoctorQualificationDto>>;

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
            if (string.IsNullOrWhiteSpace(request.DoctorId))
                throw new ArgumentException("Doctor ID cannot be null or empty");

            var qualifications = await _unitOfWork.QualificationRepository.GetByDoctorIdAsync(request.DoctorId);
            return qualifications.Select(q => new DoctorQualificationDto(q)).ToList();
        }
    }

}
