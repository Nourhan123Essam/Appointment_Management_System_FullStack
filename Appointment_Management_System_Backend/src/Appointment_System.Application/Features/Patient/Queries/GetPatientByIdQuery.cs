using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Patient.Queries
{
    //Query
    public class GetPatientByIdQuery : IRequest<PatientDto?>
    {
        public string PatientId { get; }

        public GetPatientByIdQuery(string patientId)
        {
            PatientId = patientId;
        }
    }

    //Handler
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetPatientByIdAsync(request.PatientId);
            return patient == null ? null : new PatientDto(patient);
        }
    }

}
