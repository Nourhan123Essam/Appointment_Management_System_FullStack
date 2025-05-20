using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    //Command
    public class RegisterCommand : IRequest<Result<string>>
    {
        public RegisterDTO RegisterDto { get; }

        public RegisterCommand(RegisterDTO registerDto)
        {
            RegisterDto = registerDto;
        }
    }

    //Handler
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizer;

        public RegisterCommandHandler(IUnitOfWork unitOfWork, ILocalizationService localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RegisterDto;
            var newUser = new Domain.Entities.Patient
            {
                Email = dto.Email,
                LastName = dto.LastName,
                Phone = dto.Phone,
                FirstName = dto.FirstName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
            };

            var result = await _unitOfWork.Authentication.Register(newUser, dto.Password);

            return result;
        }
    }

    //Validator
    public class RegisterDTOValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterDTOValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.RegisterDto.FirstName)
                .NotEmpty().WithMessage(localizer["FirstNameRequired"])
                .MaximumLength(100).WithMessage(localizer["FirstNameMaxLength"]);

            RuleFor(x => x.RegisterDto.LastName)
                .NotEmpty().WithMessage(localizer["LastNameRequired"])
                .MaximumLength(100).WithMessage(localizer["LastNameMaxLength"]);

            RuleFor(x => x.RegisterDto.Email)
                .NotEmpty().WithMessage(localizer["EmailRequired"])
                .EmailAddress().WithMessage(localizer["InvalidEmail"]);

            RuleFor(x => x.RegisterDto.DateOfBirth)
                .NotEmpty().WithMessage(localizer["DOBRequired"]);

            RuleFor(x => x.RegisterDto.Gender)
                .NotEmpty().WithMessage(localizer["GenderRequired"]);

            RuleFor(x => x.RegisterDto.Phone)
                .NotEmpty().WithMessage(localizer["PhoneRequired"])
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage(localizer["PhoneInvalid"]);

            RuleFor(x => x.RegisterDto.Password)
                .NotEmpty().WithMessage(localizer["PasswordRequired"])
                .MinimumLength(6).WithMessage(localizer["PasswordMinLength"])
                .Matches(@"[A-Z]").WithMessage(localizer["PasswordUpper"])
                .Matches(@"[a-z]").WithMessage(localizer["PasswordLower"])
                .Matches(@"\d").WithMessage(localizer["PasswordDigit"])
                .Matches(@"[^\w\d\s:]").WithMessage(localizer["PasswordSpecial"]);
        }
    }

}
