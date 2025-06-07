using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands
{
    //Command
    public class UpdateDoctorCommand : IRequest<Result<string>>
    {
        public UpdateDoctorDto Dto { get; set; }

        public UpdateDoctorCommand(UpdateDoctorDto dto)
        {
            Dto = dto;
        }
    }

    //Handler
    public class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redis;

        public UpdateDoctorHandler(IUnitOfWork unitOfWork, IRedisCacheService redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public async Task<Result<string>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // Step 1: Validate request
            if (dto is null)
                return Result<string>.Fail("Invalid request.");

            // Step 2: Fetch doctor from database
            var doctor = await _unitOfWork.DoctorRepository.GetDoctorByIdAsync(dto.Id);
            if (doctor is null)
                return Result<string>.Fail("Doctor not found.");

            // Step 3: Validate specialization IDs
            foreach (var id in dto.SpecializationIds)
            {
                var exists = await _unitOfWork.SpecializationRepository.ExistsAsync(id);
                if (!exists)
                    return Result<string>.Fail($"Specialization ID {id} does not exist.");
            }

            // Step 4: Update doctor basic info fields
            doctor.Phone = dto.Phone;
            doctor.ImageUrl = dto.ImageUrl;
            doctor.InitialFee = dto.InitialFee;
            doctor.FollowUpFee = dto.FollowUpFee;
            doctor.MaxFollowUps = dto.MaxFollowUps;
            doctor.YearsOfExperience = dto.YearsOfExperience;

            // Step 5: Update specializations
            doctor.DoctorSpecializations.Clear();
            foreach (var id in dto.SpecializationIds)
            {
                doctor.DoctorSpecializations.Add(new DoctorSpecialization
                {
                    DoctorId = doctor.Id,
                    SpecializationId = id
                });
            }

            // Step 6: Save changes to database
            await _unitOfWork.DoctorRepository.UpdateBasicAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            // Step 7: Check and update Redis cache if exists
            var cachedDoctors = await _redis.GetAllDoctorsAsync();

            if (cachedDoctors is not null)
            {
                var cachedDoctor = cachedDoctors.FirstOrDefault(d => d.Id == doctor.Id);

                if (cachedDoctor is not null)
                {
                    // Step 8: Update only the modified fields in the cached doctor
                    cachedDoctor.Phone = doctor.Phone;
                    cachedDoctor.ImageUrl = doctor.ImageUrl;
                    cachedDoctor.InitialFee = doctor.InitialFee;
                    cachedDoctor.FollowUpFee = doctor.FollowUpFee;
                    cachedDoctor.MaxFollowUps = doctor.MaxFollowUps;
                    cachedDoctor.YearsOfExperience = doctor.YearsOfExperience;

                    await _redis.SetAllDoctorsAsync(cachedDoctors);
                }
            }

            return Result<string>.Success("", "Doctor updated successfully.");
        }

    }


    //Validator
    public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorCommandValidator()
        {
           
            RuleFor(x => x.Dto.YearsOfExperience)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Dto.MaxFollowUps)
                .NotEmpty().WithMessage("Number of max follow-ups is required.");

            RuleFor(x => x.Dto.FollowUpFee)
                .NotEmpty().WithMessage("Follow-up fee is required.");

            RuleFor(x => x.Dto.InitialFee)
                .NotEmpty().WithMessage("Initial fee is required.");

            RuleFor(x => x.Dto.FollowUpFee)
                .GreaterThan(0).WithMessage("Follow-up fee must be greater than 0.");

            RuleFor(x => x.Dto.InitialFee)
                .GreaterThan(0).WithMessage("Initial fee must be greater than 0.");

            RuleFor(x => x.Dto.YearsOfExperience)
                   .GreaterThanOrEqualTo(0).WithMessage("Years of experience must be non-negative.");
        }
    }

}
