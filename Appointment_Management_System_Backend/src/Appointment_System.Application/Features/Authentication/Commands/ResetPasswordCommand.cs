using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command
    // Command
    public class ResetPasswordCommand : IRequest<Result<string>>
    {
        public ResetPasswordDto ResetPassword { get; set; }
    }


    // Handler
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;
        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IRedisService redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
        }


        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.ResetPassword;                                                                                                                                                                                                                            
            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<string>.Fail("Passwords do not match.");

            var userId = await _redis.GetResetPasswordTokenAsync(dto.Token);
            if (string.IsNullOrEmpty(userId))
                return Result<string>.Fail("Invalid or expired token.");

            var result = await _unitOfWork.Authentication.UpdatePasswordAsync(userId, dto.NewPassword);
            if (!result)
                return Result<string>.Fail("User not found.");

            await _redis.DeleteResetPasswordTokenAsync(dto.Token);

            return Result<string>.Success("Password reset successfully.");
        }
    }


    // Validator
    public class ResetPasswordDtoValidator: AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one number")
                .Matches(@"[^\w\d\s:]").WithMessage("Password must contain at least one special character (e.g. #, $, etc.)");
        }
    }
}
