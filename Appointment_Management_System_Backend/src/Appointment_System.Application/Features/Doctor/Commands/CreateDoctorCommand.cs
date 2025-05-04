using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands
{
    //Command
    public record CreateDoctorCommand(DoctorCreateDto Dto) : IRequest<DoctorDto>;

    //Handler
    public class CreateDoctorHandler : IRequestHandler<CreateDoctorCommand, DoctorDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDoctorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorDto> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            if (request.Dto == null)
                throw new ArgumentNullException(nameof(request.Dto), "Doctor data must be provided.");

            return await _unitOfWork.Doctors.CreateDoctorAsync(request.Dto, request.Dto.Password);
        }
    }


    // Doctor validator
    public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
    {
        public CreateDoctorCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull().WithMessage("Doctor information is required.");

            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.FirstName)
                 .NotEmpty().WithMessage("First name is required.")
                 .MaximumLength(50);

                RuleFor(x => x.Dto.LastName)
                    .NotEmpty().WithMessage("Last name is required.")
                    .MaximumLength(50);

                RuleFor(x => x.Dto.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Email must be valid.");

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


                RuleFor(x => x.Dto.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

                RuleFor(x => x.Dto.YearsOfExperience)
                    .GreaterThanOrEqualTo(0).WithMessage("Years of experience must be non-negative.");

                RuleForEach(x => x.Dto.Availabilities)
                    .SetValidator(new DoctorAvailabilityDtoValidator());

                RuleForEach(x => x.Dto.Qualifications)
                    .SetValidator(new DoctorQualificationDtoValidator());
            });
        }
    }


    //availability validator
    public class DoctorAvailabilityDtoValidator : AbstractValidator<DoctorAvailabilityDto>
    {
        public DoctorAvailabilityDtoValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid day of the week.");

            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("Start time must be before end time.");
        }
    }

    //qualification validator
    public class DoctorQualificationDtoValidator : AbstractValidator<DoctorQualificationDto>
    {
        public DoctorQualificationDtoValidator()
        {
            RuleFor(x => x.QualificationName)
                .NotEmpty().WithMessage("Qualification name is required.");

            RuleFor(x => x.IssuingInstitution)
                .NotEmpty().WithMessage("Institution is required.");

            RuleFor(x => x.YearEarned)
                .InclusiveBetween(1950, DateTime.Now.Year)
                .WithMessage("Year must be between 1950 and current year.");
        }
    }


}
