using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Doctor.Commands
{
    //Command
    public class UpdateDoctorCommand : IRequest<bool>
    {
        public string DoctorId { get; set; }
        public DoctorUpdateDto Dto { get; set; }

        public UpdateDoctorCommand(string doctorId, DoctorUpdateDto dto)
        {
            DoctorId = doctorId;
            Dto = dto;
        }
    }

    //Handler
    public class UpdateDoctorHandler : IRequestHandler<UpdateDoctorCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            if(request.Dto == null) return false;

            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(request.Dto.Id);
            request.Dto.ToEnitiy(doctor);

            return await _unitOfWork.Doctors.UpdateDoctorAsync(doctor);
        }
    }

    //Validator
    public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorCommandValidator()
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

            RuleFor(x => x.Dto.YearsOfExperience)
                   .GreaterThanOrEqualTo(0).WithMessage("Years of experience must be non-negative.");
        }
    }

}
