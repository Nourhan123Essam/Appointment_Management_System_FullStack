using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;
using Appointment_System.Application.Interfaces.Repositories;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command to handle refresh token request
    public class RefreshTokenCommand : IRequest<Response<LoginResult>>
    {
        public string RefreshToken { get; set; }

        public RefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    // Handler
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<LoginResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IRedisService redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
        }

        public async Task<Response<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Get user ID from Redis by refresh token
            var userId = await _redis.GetRefreshTokenAsync(request.RefreshToken);
            if (string.IsNullOrEmpty(userId))
                return Response<LoginResult>.Fail("Refresh token is invalid or expired.");

            // 2. Generate new access token (will fail if user doesn't exist)
            var newAccessToken = await _unitOfWork.Authentication.GenerateTokenAsync(userId);
            if (newAccessToken == null)
                return Response<LoginResult>.Fail("User not found.");

            // 3. Generate new refresh token
            var newRefreshToken = _unitOfWork.Authentication.GenerateRefreshToken();

            // 4. Replace old refresh token in Redis
            await _redis.DeleteRefreshTokenAsync(request.RefreshToken);
            await _redis.SetRefreshTokenAsync(newRefreshToken, userId, TimeSpan.FromDays(7));

            // 5. Return new token pair
            const int accessTokenExpiryInSeconds = 86400;
            var result = new LoginResult(newAccessToken, newRefreshToken, accessTokenExpiryInSeconds);
            return Response<LoginResult>.Success(result);
        }

    }

}
