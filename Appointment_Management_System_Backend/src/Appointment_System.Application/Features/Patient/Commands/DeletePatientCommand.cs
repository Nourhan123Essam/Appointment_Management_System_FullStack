using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Patient.Commands
{
    //Command
    public class DeletePatientCommand : IRequest<bool>
    {
        public int PatientId { get; }

        public DeletePatientCommand(int patientId)
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
            var patient = await _unitOfWork.PatientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
                return false;

            await _unitOfWork.PatientRepository.DeletePatientAsync(patient);
            return true;
        }
    }

}
