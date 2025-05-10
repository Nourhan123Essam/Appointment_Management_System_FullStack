using Appointment_System.Application.Helpers;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command
    public record RequestPasswordResetCommand(string Email) : IRequest<Response<string>>;

    // Hnadler
    public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, Response<string>>
    {
        private readonly IRedisService _redis;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        public RequestPasswordResetHandler(IUnitOfWork unitOfWork, IRedisService redis, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
            _emailService = emailService;
        }


        public async Task<Response<string>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var userId = await _unitOfWork.Authentication.GetUserIdByEmailAsync(request.Email);
            if (userId == null)
                return Response<string>.Fail("User not found");

            var token = Guid.NewGuid().ToString();
            await _redis.SetResetPasswordTokenAsync(token, userId, TimeSpan.FromMinutes(10));

            var baseUrl = Environment.GetEnvironmentVariable("CLIENT_URL")!;
            var resetUrl = $"{baseUrl}/reset-password?userId={userId}&token={token}";
            var html = EmailTemplateBuilder.BuildResetPasswordEmail(resetUrl);

            await _emailService.SendEmailAsync(request.Email, "Reset your password", html);
            return Response<string>.Success("");
        }
    }


}
