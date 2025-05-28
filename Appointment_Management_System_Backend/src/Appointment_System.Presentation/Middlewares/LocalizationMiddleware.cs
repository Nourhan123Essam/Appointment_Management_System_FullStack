namespace Appointment_System.Presentation.Middlewares
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var language = context.Request.Headers["Accept-Language"].ToString();

            // Default fallback
            if (string.IsNullOrWhiteSpace(language))
                language = "en-US";

            // Store in HttpContext.Items for downstream access
            context.Items["Language"] = language;

            await _next(context);
        }
    }

}
