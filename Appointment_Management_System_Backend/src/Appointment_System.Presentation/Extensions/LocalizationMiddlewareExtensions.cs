using Appointment_System.Presentation.Middlewares;

namespace Appointment_System.Presentation.Extensions
{
    public static class LocalizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseLocalizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LocalizationMiddleware>();
        }
    }

}
