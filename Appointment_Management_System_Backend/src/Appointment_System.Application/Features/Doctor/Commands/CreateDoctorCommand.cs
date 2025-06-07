using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands
{
    // === Command ===
    public class CreateDoctorCommand: IRequest<Result<string>>
    {
        public CreateDoctorDto DoctorDto;
        public string password;
        public CreateDoctorCommand(CreateDoctorDto _DoctorDto, string _password)
        {
            DoctorDto = _DoctorDto;             
            password = _password;
        }
    }

    // === Handler ===
    public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityRepository _identity;
        private readonly IRedisCacheService _redis;

        public CreateDoctorCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identity, IRedisCacheService redis)
        {
            _unitOfWork = unitOfWork;
            _identity = identity;
            _redis = redis;
        }

        public async Task<Result<string>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var dto = request.DoctorDto;

            // Step 1: Pre-check if email exists before creating user
            if (await _identity.UserExistsAsync(dto.Email))
                return Result<string>.Fail("A user with this email already exists.");

            // Step 2: Validate specialization IDs
            foreach (var id in dto.SpecializationIds)
            {
                if (!await _unitOfWork.SpecializationRepository.ExistsAsync(id))
                    return Result<string>.Fail($"Specialization ID {id} does not exist.");
            }

            // Step 3: Validate office IDs in availabilities
            foreach (var availability in dto.Availabilities)
            {
                if (!await _unitOfWork.OfficeRepository.ExistsAsync(availability.OfficeId))
                    return Result<string>.Fail($"Office ID {availability.OfficeId} does not exist.");
            }

            // Step 4 + 5: Execute creation in transaction (Identity + DB)
            // NOTE: This ensures user creation, role assignment, and DB insert all succeed or fail together.
            var creationResult = await _unitOfWork.DoctorRepository.CreateDoctorWithUserAsync(dto, dto.Password);
            if (!creationResult.Succeeded || creationResult.Data is null)
                return Result<string>.Fail(creationResult.Message);

            var doctor = creationResult.Data!;

            // Step 6: Update Redis cache
            var cachedDoctors = await _redis.GetAllDoctorsAsync();
            var newDoctorDto = DoctorBasicDto.FromEntity(doctor);

            if (cachedDoctors is not null)
            {
                cachedDoctors.Add(newDoctorDto);
                await _redis.SetAllDoctorsAsync(cachedDoctors);
            }
            else
            {
                await _redis.SetAllDoctorsAsync(new List<DoctorBasicDto> { newDoctorDto });
            }

            return Result<string>.Success(string.Empty, "Doctor created successfully.");
        }

    }

    // === Validator ===
    public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
    {
        public CreateDoctorCommandValidator()
        {
            RuleFor(c => c.DoctorDto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email.");

            RuleFor(c => c.DoctorDto.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Minimum 6 characters.");

            RuleFor(c => c.DoctorDto.Phone)
                .NotEmpty().WithMessage("Phone is required.");

            RuleFor(c => c.DoctorDto.InitialFee).GreaterThanOrEqualTo(0);
            RuleFor(c => c.DoctorDto.FollowUpFee).GreaterThanOrEqualTo(0);
            RuleFor(c => c.DoctorDto.MaxFollowUps).GreaterThanOrEqualTo(0);
            RuleFor(c => c.DoctorDto.YearsOfExperience).GreaterThanOrEqualTo(0);

            RuleFor(c => c.DoctorDto.SpecializationIds)
                .NotEmpty().WithMessage("At least one specialization must be selected.");

            RuleFor(c => c.DoctorDto.Translations)
                .NotEmpty().WithMessage("Translations are required.")
                .Must(t => Language.AreAllSupported(t.Select(x => x.Language)))
                .WithMessage($"Supported languages are: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");

            RuleForEach(c => c.DoctorDto.Translations).ChildRules(t =>
            {
                t.RuleFor(x => x.Language)
                    .NotEmpty().WithMessage("Language is required.")
                    .Must(Language.IsSupported)
                    .WithMessage("Unsupported language.");

                t.RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
                t.RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
                t.RuleFor(x => x.Bio).NotEmpty().WithMessage("Bio is required.");
            });

            RuleForEach(c => c.DoctorDto.Qualifications).ChildRules(q =>
            {
                q.RuleFor(x => x.Translations)
                    .NotEmpty().WithMessage("Qualification translations are required.")
                    .Must(t => Language.AreAllSupported(t.Select(x => x.Language)))
                    .WithMessage($"Qualification translations must include supported languages: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");

                q.RuleForEach(x => x.Translations).ChildRules(t =>
                {
                    t.RuleFor(x => x.Language)
                        .NotEmpty().WithMessage("Language is required.")
                        .Must(Language.IsSupported)
                        .WithMessage($"Unsupported language. Allowed: {string.Join(", ", Language.SupportedLanguages.Select(l => l.Value))}");

                    t.RuleFor(x => x.Title)
                        .NotEmpty().WithMessage("Qualification name is required.");

                    t.RuleFor(x => x.Institution)
                        .NotEmpty().WithMessage("Issuing institution is required.");
                });

                q.RuleFor(x => x.Date)
                    .InclusiveBetween(1950, DateTime.UtcNow.Year)
                    .WithMessage($"Year must be between 1950 and {DateTime.UtcNow.Year}.");
            });


            RuleForEach(c => c.DoctorDto.Availabilities).ChildRules(a =>
            {
                a.RuleFor(x => x.DayOfWeek).IsInEnum();
                a.RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("Start time must be before end time.");
            });
        }
    }

}
