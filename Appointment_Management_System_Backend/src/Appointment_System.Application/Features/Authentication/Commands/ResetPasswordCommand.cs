using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.Responses;
using FluentValidation;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command
    public class ResetPasswordCommand : IRequest<Result<string>>
    {
        public ResetPasswordCommand(ResetPasswordDto resetPassword)
        {
            ResetPassword = resetPassword;
        }

        public ResetPasswordDto ResetPassword { get; }
    }



    // Handler
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;
        private readonly ILocalizationService _localizer;

        public ResetPasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IRedisService redis,
            ILocalizationService localizer)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.ResetPassword;

            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<string>.Fail(_localizer["PasswordsDoNotMatch"]);

            var userId = await _redis.GetUserIdByResetPasswordTokenAsync(dto.Token);
            if (string.IsNullOrEmpty(userId))
                return Result<string>.Fail(_localizer["InvalidOrExpiredToken"]);

            var result = await _unitOfWork.Authentication.UpdatePasswordAsync(userId, dto.NewPassword);
            if (!result)
                return Result<string>.Fail(_localizer["UserNotFound"]);

            await _redis.DeleteResetPasswordTokenAsync(dto.Token);

            return Result<string>.Success("", _localizer["PasswordResetSuccess"]);
        }
    }



    // Validator
    public class ResetPasswordDtoValidator: AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordDtoValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ResetPassword.NewPassword)
                .NotEmpty().WithMessage(localizer["PasswordRequired"])
                .MinimumLength(6).WithMessage(localizer["PasswordMinLength"])
                .Matches(@"[A-Z]").WithMessage(localizer["PasswordUpper"])
                .Matches(@"[a-z]").WithMessage(localizer["PasswordLower"])
                .Matches(@"\d").WithMessage(localizer["PasswordDigit"])
                .Matches(@"[^\w\d\s:]").WithMessage(localizer["PasswordSpecial"]);
        }
    }
}
