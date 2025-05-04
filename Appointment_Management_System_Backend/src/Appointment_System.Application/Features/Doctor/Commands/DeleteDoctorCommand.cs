using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands
{
    //Command
    public class DeleteDoctorCommand : IRequest<bool>
    {
        public int DoctorId { get; set; }

        public DeleteDoctorCommand(int doctorId)
        {
            DoctorId = doctorId;
        }
    }

    //Handler
    public class DeleteDoctorHandler : IRequestHandler<DeleteDoctorCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDoctorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
                return false;

            return await _unitOfWork.Doctors.DeleteDoctorAsync(doctor);
        }
    }

}
