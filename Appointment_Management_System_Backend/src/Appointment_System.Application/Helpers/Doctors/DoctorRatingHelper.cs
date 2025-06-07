namespace Appointment_System.Application.Helpers.Doctors
{
    public static class DoctorRatingHelper
    {
        public static double? CalculateAverageRating(int? points, int? count)
        {
            if (points.HasValue && count.HasValue && count != 0)
                return Math.Round((double)points.Value / count.Value, 2);

            return null;
        }
    }
}
