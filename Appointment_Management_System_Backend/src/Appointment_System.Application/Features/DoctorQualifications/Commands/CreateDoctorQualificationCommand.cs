using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.DoctorQualifications.Commands
{
    //Command
    public record CreateDoctorQualificationCommand(CreateDoctorQualificationDto Dto)
    : IRequest<DoctorQualificationDto>;

    //Handler
    public class CreateDoctorQualificationHandler : IRequestHandler<CreateDoctorQualificationCommand, DoctorQualificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDoctorQualificationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorQualificationDto> Handle(CreateDoctorQualificationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (request.Dto == null)
                throw new ArgumentNullException(nameof(request.Dto), "Doctor qualification data must not be null.");

            if (dto.DoctorId <= 0)
                throw new ArgumentException("DoctorId should be greater than 0");

            var doctor = await _unitOfWork.DoctorRepository.GetDoctorByIdAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");

            if (dto.YearEarned < 1900 || dto.YearEarned > DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(dto.YearEarned), "Invalid year earned");

            var entity = dto.ToEntity();
            entity.Id = await _unitOfWork.QualificationRepository.AddAsync(entity);

            return new DoctorQualificationDto(entity);
        }
    }

    //Validator
    public class CreateDoctorQualificationValidator : AbstractValidator<CreateDoctorQualificationCommand>
    {
        public CreateDoctorQualificationValidator()
        {
            RuleFor(x => x.Dto.QualificationName)
           .NotEmpty()
           .WithMessage("Qualification name is required.");

            RuleFor(x => x.Dto.YearEarned)
                .InclusiveBetween(1900, DateTime.UtcNow.Year)
                .WithMessage($"Year earned must be between 1900 and {DateTime.UtcNow.Year}.");
        }
    }
}
