using Appointment_System.Presentation.Middlewares;

namespace Appointment_System.Presentation.Extensions
{
    public static class TimeZoneMiddlewareExtensions
    {
        public static IApplicationBuilder UseTimeZoneMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TimeZoneMiddleware>();
        }
    }

}
