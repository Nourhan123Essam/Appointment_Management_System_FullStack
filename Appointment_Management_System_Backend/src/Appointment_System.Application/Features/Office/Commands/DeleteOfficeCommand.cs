using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Office.Commands
{
    // Command
    public record DeleteOfficeCommand(int OfficeId) : IRequest<Result<string>>;

    // Handler
    public class DeleteOfficeCommandHandler : IRequestHandler<DeleteOfficeCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOfficeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
        {
            var office = await _unitOfWork.OfficeRepository.GetByIdAsync(request.OfficeId);

            if (office is null || office.IsDeleted)
                return Result<string>.Fail($"Office with ID {request.OfficeId} not found.");

            await _unitOfWork.OfficeRepository.DeleteByIdAsync(office.Id);                                                                                            
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success("", "Office deleted successfully.");
        }
    }

    // Validator
    public class DeleteOfficeCommandValidator : AbstractValidator<DeleteOfficeCommand>
    {
        public DeleteOfficeCommandValidator()
        {
            RuleFor(x => x.OfficeId)
                .GreaterThan(0).WithMessage("Office ID must be greater than zero.");
        }
    }
}
