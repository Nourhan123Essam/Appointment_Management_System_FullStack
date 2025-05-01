using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    //Command
    public class RegisterCommand : IRequest<Response>
    {
        public RegisterDTO RegisterDto { get; }

        public RegisterCommand(RegisterDTO registerDto)
        {
            RegisterDto = registerDto;
        }
    }

    //Handler
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RegisterDto;
            var newUser = new User
            {
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.TelephoneNumber,
                FullName = dto.FullName
            };

            var result = await _unitOfWork.Authentication.Register(newUser, dto.Password);

            return result
                ? new Response(true, "User registered successfully")
                : new Response(false, "Invalid data provided");
        }
    }

    //Validator
    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name must be less than 100 characters, and has no spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");

            RuleFor(x => x.TelephoneNumber)
                .NotEmpty().WithMessage("Telephone number is required (e.g., 01012345678)")
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("A valid phone number is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one number")
                .Matches(@"[^\w\d\s:]").WithMessage("Password must contain at least one special character (e.g. #, $, etc.)");
        }
    }
}
