using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Patient.Commands
{
    //Command
    public class DeletePatientCommand : IRequest<bool>
    {
        public string PatientId { get; }

        public DeletePatientCommand(string patientId)
        {
            PatientId = patientId;
        }
    }

    //Handler
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePatientCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
                return false;

            await _unitOfWork.Patients.DeletePatientAsync(patient);
            return true;
        }
    }

}
