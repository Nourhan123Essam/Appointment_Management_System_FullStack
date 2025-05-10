using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command
    public class LogoutCommand : IRequest<Response<string>>
    {
        public string RefreshToken { get; set; }

        public LogoutCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    // Hnadler
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Response<string>>
    {
        private readonly IRedisService _redis;

        public LogoutCommandHandler(IRedisService redis)
        {
            _redis = redis;
        }

        public async Task<Response<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Option 1: You stored refresh token with userId as value
            var userId = await _redis.GetRefreshTokenAsync(request.RefreshToken);
            if (string.IsNullOrEmpty(userId))
                return Response<string>.Fail("Invalid or expired refresh token.");

            // Remove the refresh token
            await _redis.DeleteRefreshTokenAsync(userId);

            return Response<string>.Success("Logout successful.");
        }
    }


}
