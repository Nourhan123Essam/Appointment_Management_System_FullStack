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
    public record CreateOfficeCommand(List<OfficeTranslationDto> Translations) : IRequest<Result<int>>;

    // Handler
    public class CreateOfficeCommandHandler : IRequestHandler<CreateOfficeCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOfficeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
        {
            // Defensive check — though FluentValidation already handles it
            if (request.Translations.Count == 0)
                return Result<int>.Fail("Translations are required.");

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

            return Result<int>.Success(office.Id, "Office created successfully.");
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
