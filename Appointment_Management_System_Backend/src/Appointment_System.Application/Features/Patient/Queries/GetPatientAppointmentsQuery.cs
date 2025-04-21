using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Patient.Queries
{
    //Query
    public class GetPatientAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public string PatientId { get; }

        public GetPatientAppointmentsQuery(string patientId)
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
            var appointments = await _unitOfWork.Patients.GetPatientAppointmentsAsync(request.PatientId);

            var doctors = await _unitOfWork.Doctors.GetAllDoctorsAsync();
            var doctorDict = doctors.ToDictionary(d => d.Id);

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                AppointmentTime = a.AppointmentTime,
                DoctorName = doctorDict.TryGetValue(a.DoctorId, out var doc) ? doc.FullName : "Unknown"
            }).ToList();
        }
    }

}
