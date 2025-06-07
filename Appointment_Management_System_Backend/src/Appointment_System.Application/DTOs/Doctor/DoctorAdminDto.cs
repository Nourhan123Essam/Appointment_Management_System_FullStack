namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorAdminDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int YearsOfExperience { get; set; }
        public int? RatingPoints { get; set; }
        public int? NumberOfRatings { get; set; }

        public List<string> Specializations { get; set; } = new();
        public List<DoctorTranslationDto> Translations { get; set; } = new();

        public static DoctorAdminDto FromCacheModel(DoctorBasicDto doctor)
        {
            return new DoctorAdminDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                Phone = doctor.Phone,
                Email = doctor.Email,
                ImageUrl = doctor.ImageUrl,
                InitialFee = doctor.InitialFee,
                FollowUpFee = doctor.FollowUpFee,
                YearsOfExperience = doctor.YearsOfExperience,
                RatingPoints = doctor.RatingPoints,
                NumberOfRatings = doctor.NumberOfRatings,

                // Prefer "en-US" for admin specialization view
                Specializations = doctor.SpecializationNames.TryGetValue("en-US", out var specs) ? specs : new List<string>(),

                // Map all translations available in dictionaries
                Translations = doctor.FirstNames.Keys
                .Where(lang =>
                    doctor.LastNames.ContainsKey(lang) &&
                    doctor.Bio.ContainsKey(lang) &&
                    doctor.TranslationIds.ContainsKey(lang)) 
                .Select(lang => new DoctorTranslationDto
                {
                    Id = doctor.TranslationIds[lang],         
                    DoctorId = doctor.Id,                     
                    Language = lang,
                    FirstName = doctor.FirstNames[lang],
                    LastName = doctor.LastNames[lang],
                    Bio = doctor.Bio[lang]
                })
                .ToList()

            };
        }
    }


}
