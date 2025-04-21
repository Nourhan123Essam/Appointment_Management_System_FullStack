using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorQualifications.Queries
{
    //Query
    public class GetDoctorQualificationByIdQuery : IRequest<DoctorQualificationDto?>
    {
        public readonly int Id;
        public GetDoctorQualificationByIdQuery(int _Id)
        {
            Id = _Id;
        }
    };

    //Handler
    public class GetDoctorQualificationByIdHandler : IRequestHandler<GetDoctorQualificationByIdQuery, DoctorQualificationDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDoctorQualificationByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorQualificationDto?> Handle(GetDoctorQualificationByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid ID");

            var qualification = await _unitOfWork.QualificationRepository.GetByIdAsync(request.Id);
            return qualification == null ? null : new DoctorQualificationDto(qualification);
        }
    }
}
