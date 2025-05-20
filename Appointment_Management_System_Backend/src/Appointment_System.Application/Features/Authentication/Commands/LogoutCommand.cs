using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.Responses;
using MediatR;

namespace Appointment_System.Application.Features.Authentication.Commands
{
    // Command
    public class LogoutCommand : IRequest<Result<string>>
    {
        public string RefreshToken { get; set; }

        public LogoutCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    // Hnadler
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
    {
        private readonly IRedisService _redis;
        private readonly ISessionService _sessionService;
        private readonly ILocalizationService _localizer;

        public LogoutCommandHandler(IRedisService redis, ISessionService sessionService, ILocalizationService localization)
        {
            _redis = redis;
            _sessionService = sessionService;
            _localizer = localization;
        }

        public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = await _redis.GetRefreshTokenAsync(request.RefreshToken);
            if (string.IsNullOrEmpty(userId))
                return Result<string>.Fail(_localizer["InvalidOrExpiredRefreshToken"]);

            // Remove the refresh token
            await _redis.DeleteRefreshTokenAsync(userId);

            // Remove the sessionId
            await _sessionService.RemoveSessionAsync(userId);

            return Result<string>.Success("", _localizer["LogoutSuccessful"]);
        }
    }
}
