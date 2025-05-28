using Appointment_System.Application.DTOs.Office;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Office.Commands
{
    // Command
    public record UpdateOfficeTranslationCommand(int OfficeId, OfficeTranslationDto Translation) : IRequest<Result<Unit>>;

    // Handler
    public class UpdateOfficeTranslationCommandHandler : IRequestHandler<UpdateOfficeTranslationCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOfficeTranslationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(UpdateOfficeTranslationCommand request, CancellationToken cancellationToken)
        {
            // Check if the office exists
            var officeExists = await _unitOfWork.OfficeRepository.ExistsAsync(request.OfficeId);
            if (!officeExists)
                return Result<Unit>.Fail("Office not found.");

            // Normalize and validate language
            var language = Language.From(request.Translation.Language);

            // Fetch existing translation
            var translation = await _unitOfWork.OfficeRepository
                .GetTranslationAsync(request.OfficeId, language.Value);

            if (translation == null)
                return Result<Unit>.Fail($"Translation for language '{language.Value}' not found.");

            // Update fields
            translation.Name = request.Translation.Name;
            translation.StreetName = request.Translation.StreetName;
            translation.City = request.Translation.City;
            translation.State = request.Translation.State;
            translation.Country = request.Translation.Country;

            _unitOfWork.OfficeRepository.UpdateTranslation(translation);
            await _unitOfWork.SaveChangesAsync();

            return Result<Unit>.Success(Unit.Value, "Translation updated successfully.");
        }
    }


    // Validator
    public class UpdateOfficeTranslationCommandValidator : AbstractValidator<UpdateOfficeTranslationCommand>
    {
        public UpdateOfficeTranslationCommandValidator()
        {
            RuleFor(x => x.OfficeId)
                .GreaterThan(0).WithMessage("Office ID must be greater than 0.");

            RuleFor(x => x.Translation).NotNull().WithMessage("Translation is required.");

            When(x => x.Translation != null, () =>
            {
                RuleFor(x => x.Translation.Language)
                    .NotEmpty().WithMessage("Language is required.")
                    .Must(Language.IsSupported).WithMessage("Unsupported language. Allowed: en-US, ar-EG");

                RuleFor(x => x.Translation.Name).NotEmpty().WithMessage("Name is required.");
                RuleFor(x => x.Translation.StreetName).NotEmpty().WithMessage("Street name is required.");
                RuleFor(x => x.Translation.City).NotEmpty().WithMessage("City is required.");
                RuleFor(x => x.Translation.State).NotEmpty().WithMessage("State is required.");
                RuleFor(x => x.Translation.Country).NotEmpty().WithMessage("Country is required.");
            });
        }
    }

}
