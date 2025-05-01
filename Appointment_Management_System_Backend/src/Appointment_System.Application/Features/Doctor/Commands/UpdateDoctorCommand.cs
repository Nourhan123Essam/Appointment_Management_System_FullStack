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
            var doctor = new Domain.Entities.User();

            doctor.Id = request.DoctorId;
            doctor.FullName = request.Dto.FullName;
            doctor.Email = request.Dto.Email;
            doctor.YearsOfExperience = request.Dto.YearsOfExperience;
            doctor.Specialization = request.Dto.Specialization;
            doctor.LicenseNumber = request.Dto.LicenseNumber;
            doctor.ConsultationFee = request.Dto.ConsultationFee;
            doctor.WorkplaceType = request.Dto.WorkplaceType;
            doctor.TotalRatingsGiven = request.Dto.TotalRatingsGiven;
            doctor.TotalRatingScore = request.Dto.TotalRatingScore;

            return await _unitOfWork.Doctors.UpdateDoctorAsync(doctor);
        }
    }

    //Validator
    public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorCommandValidator()
        {
            RuleFor(x => x.Dto.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid.");

            RuleFor(x => x.Dto.YearsOfExperience)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Dto.Specialization)
                .NotEmpty().WithMessage("Specialization is required.");

            RuleFor(x => x.Dto.LicenseNumber)
                .NotEmpty().WithMessage("License number is required.");

            RuleFor(x => x.Dto.ConsultationFee)
                .GreaterThan(0).WithMessage("Consultation fee must be greater than 0.");

            RuleFor(x => x.Dto.WorkplaceType)
                .IsInEnum().WithMessage("Invalid workplace type.");

            
        }
    }

}
