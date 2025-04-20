
namespace Appointment_System.Presentation.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Logs request and response details to trace flow.
        // Useful for diagnosing errors and understanding user interactions.
        public async Task InvokeAsync(HttpContext httpContext)
        {
            _logger.LogInformation("Request {method} {url} started at {time}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                DateTime.UtcNow);

            await _next(httpContext);

            _logger.LogInformation("Request {method} {url} completed at {time}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                DateTime.UtcNow);
        }
    }

}

