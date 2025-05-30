﻿using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    //Command
    public class LoginCommand : IRequest<Domain.Responses.Result<LoginResult>>
    {
        public LoginDTO LoginDto { get; set; }

        public LoginCommand(LoginDTO loginDto)
        {
            LoginDto = loginDto;
        }
    }

    //Handler
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var dto = request.LoginDto;

            return await _unitOfWork.Authentication.Login(dto.Email, dto.Password);
        }
    }


    //Validator
    public class LoginDTOValidator : AbstractValidator<LoginCommand>
    {
        public LoginDTOValidator(ILocalizationService _localizer)
        {
            RuleFor(x => x.LoginDto.Email)
                .NotEmpty().WithMessage(_localizer["EmailRequired"])
                .EmailAddress().WithMessage(_localizer["InvalidEmail"]);

            RuleFor(x => x.LoginDto.Password)
                .NotEmpty().WithMessage(_localizer["PasswordRequired"]);
        }
    }

}
