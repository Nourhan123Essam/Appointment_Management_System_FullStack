using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Commands
{
    public record DeleteSpecializationCommand(int Id) : IRequest<Result<string>>;

    public class DeleteSpecializationCommandHandler : IRequestHandler<DeleteSpecializationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSpecializationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(DeleteSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.SpecializationRepository.GetByIdAsync(request.Id);

            if (specialization is null)
                return Result<string>.Fail("Specialization not found.");

            if (specialization.IsDeleted)
                return Result<string>.Fail("Specialization is already deleted.");

            specialization.IsDeleted = true;

            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Specialization deleted successfully.");
        }
    }

    public class DeleteSpecializationCommandValidator : AbstractValidator<DeleteSpecializationCommand>
    {
        public DeleteSpecializationCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
