using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorAvailabilities.Commands
{
    //Command
    public class DeleteDoctorAvailabilityCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteDoctorAvailabilityCommand(int id)
        {
            Id = id;
        }
    }

    //Handler
    public class DeleteDoctorAvailabilityCommandHandler : IRequestHandler<DeleteDoctorAvailabilityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDoctorAvailabilityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDoctorAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var availability = await _unitOfWork.AvailabilityRepository.GetByIdAsync(request.Id);
            if (availability == null)
                throw new KeyNotFoundException($"Doctor availability with ID {request.Id} not found.");

            await _unitOfWork.AvailabilityRepository.DeleteAsync(request.Id);
            return true;  // Indicating that the deletion was successful
        }
    }

}
