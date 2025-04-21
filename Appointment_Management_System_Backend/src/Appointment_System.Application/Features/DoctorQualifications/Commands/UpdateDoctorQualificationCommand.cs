using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.DoctorQualifications.Commands
{
    //Command
    public record UpdateDoctorQualificationCommand(int Id, UpdateDoctorQualificationDto Dto) : IRequest<Unit>;

    //Handler
    public class UpdateDoctorQualificationHandler : IRequestHandler<UpdateDoctorQualificationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorQualificationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateDoctorQualificationCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid ID");

            if (request.Dto == null)
                throw new ArgumentNullException(nameof(request.Dto));

            var qualification = await _unitOfWork.QualificationRepository.GetByIdAsync(request.Id);
            if (qualification == null)
                throw new KeyNotFoundException("Doctor qualification not found");

            // Optional: You can move this to a validator
            if (request.Dto.YearEarned < 1900 || request.Dto.YearEarned > DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(request.Dto.YearEarned), "YearEarned must be between 1900 and the current year.");

            request.Dto.UpdateEntity(qualification);
            await _unitOfWork.QualificationRepository.UpdateAsync(qualification);

            return Unit.Value;
        }
    }

    //validator
    public class UpdateDoctorQualificationValidator : AbstractValidator<UpdateDoctorQualificationCommand>
    {
        public UpdateDoctorQualificationValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("ID must be greater than zero.");
            RuleFor(x => x.Dto).NotNull().WithMessage("DTO must not be null.");

            RuleFor(x => x.Dto.QualificationName).NotEmpty().WithMessage("Qualification name is required.");

            RuleFor(x => x.Dto.YearEarned)
                .InclusiveBetween(1900, DateTime.UtcNow.Year)
                .WithMessage($"YearEarned must be between 1900 and {DateTime.UtcNow.Year}.");
        }
    }
}
