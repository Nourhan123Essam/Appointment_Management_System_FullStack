namespace Appointment_System.Application.Helpers
{
    public static class TimeZoneHelper
    {
        public static DateTime ConvertFromUtc(DateTime utcTime, string timeZoneId)
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
            }
            catch
            {
                return utcTime; // fallback to UTC if invalid
            }
        }

        public static DateTime ConvertToUtc(DateTime localTime, string timeZoneId)
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
            }
            catch
            {
                return localTime.ToUniversalTime(); // fallback
            }
        }
    }
}
