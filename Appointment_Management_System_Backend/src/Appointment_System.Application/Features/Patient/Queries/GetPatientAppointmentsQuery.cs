using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Patient.Queries
{
    //Query
    public class GetPatientAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public int PatientId { get; }

        public GetPatientAppointmentsQuery(int patientId)
        {
            PatientId = patientId;
        }
    }

    //Handler
    public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPatientAppointmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _unitOfWork.PatientRepository.GetPatientAppointmentsAsync(request.PatientId);

            var doctors = await _unitOfWork.DoctorRepository.GetAllForCacheAsync();
            var doctorDict = doctors.ToDictionary(d => d.Id);

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                DateTime = a.DateTime,
                DoctorName = doctorDict.TryGetValue(a.DoctorId, out var doc) ? "" : "Unknown"
            }).ToList();
        }
    }

}
