using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands 
{ 

    // Command
    public class UpdateDoctorTranslationCommand : IRequest<Result<string>>
    {
        public DoctorTranslationDto Translation { get; }

        public UpdateDoctorTranslationCommand(DoctorTranslationDto translation)
        {
            Translation = translation;
        }
    }

    // Handler
    public class UpdateDoctorTranslationCommandHandler : IRequestHandler<UpdateDoctorTranslationCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redis;

        public UpdateDoctorTranslationCommandHandler(IUnitOfWork unitOfWork, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _redis = redisCacheService; 
        }

        public async Task<Result<string>> Handle(UpdateDoctorTranslationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Translation;

            // Step 1: Check if the doctor exists
            var doctorExists = await _unitOfWork.DoctorRepository.GetDoctorByIdAsync(dto.DoctorId);
            if (doctorExists is null)
                return Result<string>.Fail("Doctor not found.");

            // Step 2: Get the existing translation entity
            var translation = await _unitOfWork.DoctorRepository.GetDoctorTranslationByIdAsync(dto.Id);
            if (translation == null)
                return Result<string>.Fail("Doctor translation not found.");

            // Step 3: Update translation fields
            translation.FirstName = dto.FirstName;
            translation.LastName = dto.LastName;
            translation.Bio = dto.Bio;

            // Step 4: Persist translation updates to DB
            await _unitOfWork.DoctorRepository.UpdateTranslationAsync(translation);
            await _unitOfWork.SaveChangesAsync();

            // Step 5: Try to update Redis cache if it exists
            var cachedDoctors = await _redis.GetAllDoctorsAsync();
            if (cachedDoctors is not null)
            {
                // Step 5.1: Find doctor in cache
                var cachedDoctor = cachedDoctors.FirstOrDefault(d => d.Id == dto.DoctorId);
                if (cachedDoctor != null)
                {
                    var lang = translation.Language.Value;

                    // Step 5.2: Update localized fields in cache
                    cachedDoctor.FirstNames[lang] = dto.FirstName;
                    cachedDoctor.LastNames[lang] = dto.LastName;
                    cachedDoctor.Bio[lang] = dto.Bio;

                    // Step 5.3: Save updated cache
                    await _redis.SetAllDoctorsAsync(cachedDoctors);
                }
            }

            return Result<string>.Success("", "Doctor translation updated successfully.");
        }
    }


    // Validator
    public class UpdateDoctorTranslationCommandValidator : AbstractValidator<UpdateDoctorTranslationCommand>
    {
        public UpdateDoctorTranslationCommandValidator()
        {
            RuleFor(x => x.Translation.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.Translation.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Translation.Bio)
                .NotEmpty().WithMessage("Bio is required.");

            RuleFor(x => x.Translation.Language)
                .NotEmpty().WithMessage("Language is required.")
                .Must(Language.IsSupported)
                .WithMessage("Unsupported language. Allowed: en-US, ar-EG");
        }
    }

}
