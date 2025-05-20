using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Localization;
using System.Security.Claims;
using System.Text.Json;

namespace Appointment_System.Presentation.Middlewares
{
    public class SessionValidationMiddleware : IMiddleware
    {
        private readonly ISessionService _sessionService;
        private const string SessionCookieName = "SessionId";
        private readonly ILocalizationService _localizer;

        public SessionValidationMiddleware(ISessionService sessionService, ILocalizationService localizationService)
        {
            _sessionService = sessionService;
            _localizer = localizationService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
       {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!context.User.Identity?.IsAuthenticated ?? true || string.IsNullOrEmpty(userId))
            {
                await next(context);
                return;
            }

            var sessionIdFromCookie = context.Request.Cookies[SessionCookieName];
            var sessionExist = await _sessionService.ValidateSessionExistsAsync(userId);
            if (!sessionExist)
            {
                if (string.IsNullOrEmpty(sessionIdFromCookie))
                {
                    var newSessionId = await _sessionService.CreateSessionAsync(userId);
                    context.Response.Cookies.Append("SessionId", newSessionId, new CookieOptions
                    {
                        HttpOnly = true,           // Prevent access from JavaScript
                        Secure = true,             // Use only over HTTPS
                        SameSite = SameSiteMode.None, // cross-site cookie use
                    });
                }
                else
                {
                    var valid = await _sessionService.ValidateAndExtendSessionAsync(userId, sessionIdFromCookie);
                    if (!valid)
                    {
                        await RespondForbiddenAsync(context, _localizer["SessionExpiredOrInvalid"]);
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(sessionIdFromCookie))
            {
                // Invalid session
                await RespondForbiddenAsync(context, _localizer["AlreadySignedInOnAnotherDevice"]);
                return;
            }

            var isValid = await _sessionService.ValidateSessionAsync(userId, sessionIdFromCookie);
            if (isValid)
            {
                // Extend session expiration and proceed with the request
                var valid = await _sessionService.ValidateAndExtendSessionAsync(userId, sessionIdFromCookie);
                if (!valid)
                {
                    await RespondForbiddenAsync(context, _localizer["SessionExpiredOrInvalid"]);
                    return;
                }

                await next(context);
                return;
            }

            // Invalid session
            await RespondForbiddenAsync(context, _localizer["AlreadySignedInOnAnotherDevice"]);
        }

        public static async Task RespondForbiddenAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var result = new
            {
                Status = "Forbidden",
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}
