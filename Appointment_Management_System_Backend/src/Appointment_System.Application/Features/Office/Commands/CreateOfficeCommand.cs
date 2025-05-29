using Appointment_System.Application.DTOs.Office;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Office.Commands
{
    // Command
    public record CreateOfficeCommand(List<OfficeTranslationDto> Translations): IRequest<Result<OfficeWithTranslationsDto>>;

    // Handler
    public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, Result<OfficeWithTranslationsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOfficeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OfficeWithTranslationsDto>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
        {
            // Defensive check — though FluentValidation already handles it
            if (request.Translations.Count == 0)
                return Result<OfficeWithTranslationsDto>.Fail("Translations are required.");

            var office = new Domain.Entities.Office
            {
                CreatedAt = DateTime.UtcNow
            };

            var translations = request.Translations.Select(t => new OfficeTranslation
            {
                Language = Language.From(t.Language),
                Name = t.Name,
                StreetName = t.StreetName,
                City = t.City,
                State = t.State,
                Country = t.Country,
                Office = office
            }).ToList();

            office.Translations = translations;

            await _unitOfWork.OfficeRepository.AddAsync(office);
            await _unitOfWork.SaveChangesAsync();

            // Build the DTO
            var dto = new OfficeWithTranslationsDto
            {
                Id = office.Id,
                Translations = office.Translations
                    .Select(t => new OfficeTranslationDto
                    {
                        Id = t.Id,
                        OfficeId = office.Id,
                        Language = t.Language.Value,
                        Name = t.Name,
                        City = t.City,
                        StreetName = t.StreetName,
                        State = t.State,
                        Country = t.Country
                    })
                    .ToList()
            };

            return Result<OfficeWithTranslationsDto>.Success(dto, "Office created successfully.");
        }
    }

    // Validator
    public class CreateOfficeCommandValidator : AbstractValidator<CreateOfficeCommand>
    {
        public CreateOfficeCommandValidator()
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
                t.RuleFor(x => x.StreetName).NotEmpty().WithMessage("Street name is required.");
                t.RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
                t.RuleFor(x => x.State).NotEmpty().WithMessage("State is required.");
                t.RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
            });
        }
    }


}
