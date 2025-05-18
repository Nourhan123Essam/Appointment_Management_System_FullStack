namespace Appointment_System.Presentation.Middlewares
{
    public class TimeZoneMiddleware
    {
        private readonly RequestDelegate _next;
        private const string DefaultTimeZone = "UTC";

        public TimeZoneMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headerTimeZone = context.Request.Headers["X-Timezone"].FirstOrDefault();

            TimeZoneInfo timeZone;
            try
            {
                timeZone = string.IsNullOrWhiteSpace(headerTimeZone)
                    ? TimeZoneInfo.Utc
                    : TimeZoneInfo.FindSystemTimeZoneById(headerTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                timeZone = TimeZoneInfo.Utc;
            }

            context.Items["UserTimeZone"] = timeZone;

            await _next(context);
        }
    }

}
