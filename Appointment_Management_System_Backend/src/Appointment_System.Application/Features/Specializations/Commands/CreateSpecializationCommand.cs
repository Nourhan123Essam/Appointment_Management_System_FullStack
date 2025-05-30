using Appointment_System.Application.DTOs.Specialization;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Specializations.Commands
{
    // === Command ===
    public record CreateSpecializationCommand(List<SpecializationTranslationDto> Translations)
        : IRequest<Result<SpecializationWithTranslationsDto>>;

    // === Handler ===
    public class CreateSpecializationCommandHandler : IRequestHandler<CreateSpecializationCommand, Result<SpecializationWithTranslationsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSpecializationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SpecializationWithTranslationsDto>> Handle(CreateSpecializationCommand request, CancellationToken cancellationToken)
        {
            if (request.Translations.Count == 0)
                return Result<SpecializationWithTranslationsDto>.Fail("Translations are required.");

            var specialization = new Specialization
            {
                CreatedAt = DateTime.UtcNow
            };

            var translations = request.Translations.Select(t => new SpecializationTranslation
            {
                Name = t.Name,
                LanguageValue = Language.From(t.Language),
                Specialization = specialization
            }).ToList();

            specialization.Translations = translations;

            await _unitOfWork.SpecializationRepository.AddAsync(specialization);
            await _unitOfWork.SaveChangesAsync();

            var dto = new SpecializationWithTranslationsDto
            {
                Id = specialization.Id,
                Translations = specialization.Translations
                    .Select(t => new SpecializationTranslationDto
                    {
                        Id = t.Id,
                        SpecializationId = specialization.Id,
                        Language = t.LanguageValue.Value,
                        Name = t.Name
                    }).ToList()
            };

            return Result<SpecializationWithTranslationsDto>.Success(dto, "Specialization created successfully.");
        }
    }

    // === Validator ===
    public class CreateSpecializationCommandValidator : AbstractValidator<CreateSpecializationCommand>
    {
        public CreateSpecializationCommandValidator()
        {
            RuleFor(c => c.Translations)
                .NotEmpty().WithMessage("All translations are required.")
                .Must(t => Language.AreAllSupported(t.Select(x => x.Language)))
                .WithMessage($"Translations must include: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");

            RuleForEach(c => c.Translations).ChildRules(t =>
            {
                t.RuleFor(x => x.Language)
                    .NotEmpty().WithMessage("Language is required.")
                    .Must(Language.IsSupported)
                    .WithMessage($"Unsupported language. Allowed: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");

                t.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            });
        }
    }
}
