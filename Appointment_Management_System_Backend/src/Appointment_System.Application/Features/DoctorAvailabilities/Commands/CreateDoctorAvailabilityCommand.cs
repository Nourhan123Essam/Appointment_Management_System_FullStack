using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.DoctorAvailabilities.Commands
{
    //Command
    public class CreateDoctorAvailabilityCommand : IRequest<DoctorAvailabilityDto>
    {
        public CreateDoctorAvailabilityDto Dto { get; set; }

        public CreateDoctorAvailabilityCommand(CreateDoctorAvailabilityDto dto)
        {
            Dto = dto;
        }
    }

    //Handler
    public class CreateDoctorAvailabilityCommandHandler : IRequestHandler<CreateDoctorAvailabilityCommand, DoctorAvailabilityDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDoctorAvailabilityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorAvailabilityDto> Handle(CreateDoctorAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Doctor availability data must not be null.");


            if (dto.DoctorId <= 0)
                throw new ArgumentException("DoctorId should be greater than 0");

            var doctorExists = await _unitOfWork.Doctors.GetDoctorByIdAsync(dto.DoctorId) != null;
            if (!doctorExists)
                throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} does not exist.");


            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be earlier than EndTime");

            var availability = request.Dto.ToEntity();
            availability.Id = await _unitOfWork.AvailabilityRepository.AddAsync(availability);

            return new DoctorAvailabilityDto(availability);
        }
    }


    //Validator
    public class CreateDoctorAvailabilityDtoValidator : AbstractValidator<CreateDoctorAvailabilityDto>
    {
        public CreateDoctorAvailabilityDtoValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("DoctorId is required.")
                .NotNull().WithMessage("DoctorId cannot be null.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("StartTime is required.")
                .Must((dto, startTime) => startTime < dto.EndTime).WithMessage("StartTime must be earlier than EndTime.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("EndTime is required.");

            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("DayOfWeek must be a valid value.");
        }
    }


}
