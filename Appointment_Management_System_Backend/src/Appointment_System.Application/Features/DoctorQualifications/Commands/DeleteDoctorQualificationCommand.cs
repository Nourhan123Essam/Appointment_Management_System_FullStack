using Appointment_System.Application.Interfaces;
using MediatR;

namespace Appointment_System.Application.Features.DoctorQualifications.Commands
{
    //Command
    public record DeleteDoctorQualificationCommand(int Id) : IRequest<Unit>;

    //Handler
    public class DeleteDoctorQualificationHandler : IRequestHandler<DeleteDoctorQualificationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDoctorQualificationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDoctorQualificationCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid ID");

            var qualification = await _unitOfWork.QualificationRepository.GetByIdAsync(request.Id);
            if (qualification == null)
                throw new KeyNotFoundException("Doctor qualification not found");

            await _unitOfWork.QualificationRepository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
}
