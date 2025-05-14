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
        public string SessionId { get; set; }

        public RefreshTokenCommand(string refreshToken, string sessionId)
        {
            RefreshToken = refreshToken;
            SessionId = sessionId;
        }
    }

    // Handler
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<LoginResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;
        private readonly ISessionService _sessionService;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IRedisService redis, ISessionService sessionService)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
            _sessionService = sessionService;
        }

        //public async Task<Response<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        //{
        //    // 1. Get user ID from Redis by refresh token
        //    var userId = await _redis.GetRefreshTokenAsync(request.RefreshToken);
        //    if (string.IsNullOrEmpty(userId))
        //        return Response<LoginResult>.Fail("Refresh token is invalid or expired.");

        //    // 2. Generate new access token (will fail if user doesn't exist)
        //    var newAccessToken = await _unitOfWork.Authentication.GenerateTokenAsync(userId);
        //    if (newAccessToken == null)
        //        return Response<LoginResult>.Fail("User not found.");

        //    // 3. Generate new refresh token
        //    var newRefreshToken = _unitOfWork.Authentication.GenerateRefreshToken();

        //    // 4. Replace old refresh token in Redis
        //    await _redis.DeleteRefreshTokenAsync(request.RefreshToken);
        //    await _redis.SetRefreshTokenAsync(newRefreshToken, userId, TimeSpan.FromDays(7));

        //    // 5. Return new token pair
        //    const int accessTokenExpiryInSeconds = 900;
        //    var result = new LoginResult(newAccessToken, newRefreshToken, accessTokenExpiryInSeconds);
        //    return Response<LoginResult>.Success(result);
        //}

        public async Task<Response<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Get user ID from Redis by refresh token
            var userId = await _redis.GetRefreshTokenAsync(request.RefreshToken);
            if (string.IsNullOrEmpty(userId))
                return Response<LoginResult>.Fail("Refresh token is invalid or expired.");

            // 2. Generate new access token
            var newAccessToken = await _unitOfWork.Authentication.GenerateTokenAsync(userId);
            if (newAccessToken == null)
                return Response<LoginResult>.Fail("User not found.");

            // 3. Generate new refresh token
            var newRefreshToken = _unitOfWork.Authentication.GenerateRefreshToken();

            // 4. Replace old refresh token in Redis
            await _redis.DeleteRefreshTokenAsync(request.RefreshToken);
            await _redis.SetRefreshTokenAsync(newRefreshToken, userId, TimeSpan.FromDays(7));

            // 5. Extend or regenerate session in Redis
            var currentSessionId = request.SessionId;
            string sessionIdToReturn;

            if (!string.IsNullOrEmpty(currentSessionId))
            {
                var isSessionValid = await _sessionService.ValidateSessionAsync(userId, currentSessionId);

                if (isSessionValid)
                {
                    var sessionValid = await _sessionService.ValidateAndExtendSessionAsync(userId, request.SessionId);
                    if (!sessionValid)
                        return Response<LoginResult>.Fail("Session expired or invalid.");

                    sessionIdToReturn = currentSessionId;
                }
                else
                {
                    // Session is invalid, so delete old and create new
                    await _sessionService.RemoveSessionAsync(userId);
                    sessionIdToReturn = await _sessionService.CreateSessionAsync(userId);
                }
            }
            else
            {
                // No session ID was sent — create a fresh one
                sessionIdToReturn = await _sessionService.CreateSessionAsync(userId);
            }


            // 6. Return new token pair
            const int accessTokenExpiryInSeconds = 900;

            var result = new LoginResult(
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken,
                SessionId: sessionIdToReturn,
                ExpiresIn: accessTokenExpiryInSeconds
            );

            return Response<LoginResult>.Success(result);

        }
    }

}
