using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Commands
{
    public record UpdateSpecializationTranslationCommand(int SpecializationId, string Language, string Name)
        : IRequest<Result<string>>;

    public class UpdateSpecializationTranslationCommandHandler
        : IRequestHandler<UpdateSpecializationTranslationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSpecializationTranslationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateSpecializationTranslationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.SpecializationRepository.GetByIdAsync(request.SpecializationId);

            if (specialization is null)
                return Result<string>.Fail("Specialization not found.");

            var lang = Language.From(request.Language);
            var translation = specialization.Translations.FirstOrDefault(t => t.LanguageValue.Value == lang.Value);

            if (translation is null)
                return Result<string>.Fail($"Translation in language '{lang.Value}' not found.");

            translation.Name = request.Name;

            _unitOfWork.SpecializationRepository.UpdateTranslation(translation);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success("Translation updated successfully.");
        }
    }

    public class UpdateSpecializationTranslationCommandValidator : AbstractValidator<UpdateSpecializationTranslationCommand>
    {
        public UpdateSpecializationTranslationCommandValidator()
        {
            RuleFor(x => x.SpecializationId).GreaterThan(0);

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Language is required.")
                .Must(Language.IsSupported)
                .WithMessage($"Unsupported language. Allowed: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}
