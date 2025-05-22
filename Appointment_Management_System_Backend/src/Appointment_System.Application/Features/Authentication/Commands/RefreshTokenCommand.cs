using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Interfaces;
using Appointment_System.Domain.Responses;
using MediatR;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Localization;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command to handle refresh token request
    public class RefreshTokenCommand : IRequest<Result<LoginResult>>
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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redis;
        private readonly ISessionService _sessionService;
        private readonly ILocalizationService _localizer;

        public RefreshTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IRedisService redis,
            ISessionService sessionService,
            ILocalizationService localizer)
        {
            _unitOfWork = unitOfWork;
            _redis = redis;
            _sessionService = sessionService;
            _localizer = localizer;
        }

        public async Task<Result<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Get user ID from Redis by refresh token
            var userId = await _redis.GetUserIdByRefreshTokenAsync(request.RefreshToken);
            if (string.IsNullOrEmpty(userId))
                return Result<LoginResult>.Fail(_localizer["InvalidOrExpiredRefreshToken"]);

            // 2. Generate new access token
            var newAccessToken = await _unitOfWork.Authentication.GenerateTokenAsync(userId);
            if (newAccessToken == null)
                return Result<LoginResult>.Fail(_localizer["UserNotFound"]);

            // 3. Generate new refresh token
            var newRefreshToken = _unitOfWork.Authentication.GenerateRefreshToken();

            // 4. Replace old refresh token in Redis
            await _redis.DeleteRefreshTokenAsync(request.RefreshToken);
            await _redis.SetRefreshTokenAsync(newRefreshToken, userId, TimeSpan.FromDays(7));

            // 5. Extend or regenerate session in Redis
            string sessionIdToReturn;

            if (!string.IsNullOrEmpty(request.SessionId))
            {
                var isSessionValid = await _sessionService.ValidateSessionAsync(userId, request.SessionId);

                if (isSessionValid)
                {
                    var sessionValid = await _sessionService.ValidateAndExtendSessionAsync(userId, request.SessionId);
                    if (!sessionValid)
                        return Result<LoginResult>.Fail(_localizer["SessionExpiredOrInvalid"]);

                    sessionIdToReturn = request.SessionId;
                }
                else
                {
                    await _sessionService.RemoveSessionAsync(userId);
                    sessionIdToReturn = await _sessionService.CreateSessionAsync(userId);
                }
            }
            else
            {
                sessionIdToReturn = await _sessionService.CreateSessionAsync(userId);
            }

            // 6. Return new token pair
            var result = new LoginResult(
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken,
                SessionId: sessionIdToReturn,
                ExpiresIn: 900
            );

            return Result<LoginResult>.Success(result);
        }
    }

}
