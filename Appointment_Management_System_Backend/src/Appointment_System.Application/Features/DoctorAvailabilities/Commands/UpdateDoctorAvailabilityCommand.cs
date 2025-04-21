using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.DoctorAvailabilities.Commands
{
    //Command
    public class UpdateDoctorAvailabilityCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public UpdateDoctorAvailabilityDto Dto { get; set; }

        public UpdateDoctorAvailabilityCommand(int id, UpdateDoctorAvailabilityDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }

    //Handler
    public class UpdateDoctorAvailabilityCommandHandler : IRequestHandler<UpdateDoctorAvailabilityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorAvailabilityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateDoctorAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be earlier than EndTime");

            var availability = await _unitOfWork.AvailabilityRepository.GetByIdAsync(request.Id);
            if (availability == null)
                throw new KeyNotFoundException("Doctor availability not found.");

            availability.DayOfWeek = request.Dto.DayOfWeek;
            availability.StartTime = request.Dto.StartTime;
            availability.EndTime = request.Dto.EndTime;

            await _unitOfWork.AvailabilityRepository.UpdateAsync(availability);
            return true;  // Indicating that the update was successful
        }
    }


    //Validator
    public class UpdateDoctorAvailabilityDtoValidator : AbstractValidator<UpdateDoctorAvailabilityDto>
    {
        public UpdateDoctorAvailabilityDtoValidator()
        {
            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("StartTime must be earlier than EndTime");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("EndTime must be later than StartTime");
        }
    }

}
