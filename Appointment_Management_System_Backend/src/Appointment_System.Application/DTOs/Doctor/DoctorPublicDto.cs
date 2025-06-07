using Appointment_System.Application.Helpers.Doctors;

namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorPublicDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public List<string> Specializations { get; set; } = new();
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int YearsOfExperience { get; set; }
        public double? AverageRating { get; set; }

        public static DoctorPublicDto? FromCacheModel(DoctorBasicDto doctor, string language)
        {
            // Check if translations for requested language exist
            var hasLang = doctor.FirstNames.ContainsKey(language)
                       && doctor.LastNames.ContainsKey(language)
                       && doctor.Bio.ContainsKey(language)
                       && doctor.TranslationIds.ContainsKey(language);

            if (!hasLang)
                language = "en-US"; // fallback to English

            return new DoctorPublicDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstNames.GetValueOrDefault(language, string.Empty),
                LastName = doctor.LastNames.GetValueOrDefault(language, string.Empty),
                Bio = doctor.Bio.GetValueOrDefault(language, string.Empty),
                Specializations = doctor.SpecializationNames.GetValueOrDefault(language, new List<string>()),

                Phone = doctor.Phone,
                Email = doctor.Email,
                ImageUrl = doctor.ImageUrl,
                InitialFee = doctor.InitialFee,
                FollowUpFee = doctor.FollowUpFee,
                YearsOfExperience = doctor.YearsOfExperience,
                AverageRating = DoctorRatingHelper.CalculateAverageRating(
                    doctor.RatingPoints, doctor.NumberOfRatings)
            };
        }

    }

}
