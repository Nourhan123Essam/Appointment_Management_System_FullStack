using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    //Command
    public class LoginCommand : IRequest<Response>
    {
        public LoginDTO LoginDto { get; set; }

        public LoginCommand(LoginDTO loginDto)
        {
            LoginDto = loginDto;
        }
    }

    //Handler
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var dto = request.LoginDto;

            var user = await _unitOfWork.Authentication.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return new Response(false, "Invalid credentials");

            return await _unitOfWork.Authentication.Login(user, dto.Password);
        }
    }


    //Validator
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Enter a valid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }

}
