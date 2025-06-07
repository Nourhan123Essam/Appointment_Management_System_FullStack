namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorBasicDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public Dictionary<string, string> FirstNames { get; set; } = new();
        public Dictionary<string, string> LastNames { get; set; } = new();
        public Dictionary<string, string> Bio { get; set; } = new();
        public Dictionary<string, int> TranslationIds { get; set; } = new(); 

        public Dictionary<string, List<string>> SpecializationNames { get; set; } = new();

        public string Phone { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; } = null!;

        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int? FollowUpCount { get; set; } = 0;
        public int MaxFollowUps { get; set; }
        public int YearsOfExperience { get; set; }
        public int? RatingPoints { get; set; } = 0;
        public int? NumberOfRatings { get; set; } = 0;

        public static DoctorBasicDto FromEntity(Domain.Entities.Doctor doctor)
        {
            var specializationNames = new Dictionary<string, List<string>>();

            if (doctor.DoctorSpecializations != null)
            {
                foreach (var ds in doctor.DoctorSpecializations)
                {
                    if (ds?.Specialization?.Translations == null) continue;

                    foreach (var translation in ds.Specialization.Translations)
                    {
                        if (translation?.LanguageValue?.Value == null) continue;

                        if (!specializationNames.ContainsKey(translation.LanguageValue.Value))
                            specializationNames[translation.LanguageValue.Value] = new List<string>();

                        specializationNames[translation.LanguageValue.Value].Add(translation.Name);
                    }
                }
            }

            var firstNames = new Dictionary<string, string>();
            var lastNames = new Dictionary<string, string>();
            var bios = new Dictionary<string, string>();
            var translationIds = new Dictionary<string, int>();

            if (doctor.Translations != null)
            {
                foreach (var t in doctor.Translations)
                {
                    if (t?.Language?.Value == null) continue;

                    firstNames[t.Language.Value] = t.FirstName;
                    lastNames[t.Language.Value] = t.LastName;
                    bios[t.Language.Value] = t.Bio;
                    translationIds[t.Language.Value] = t.Id;
                }
            }

            return new DoctorBasicDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                Phone = doctor.Phone,
                Email = doctor.Email,
                ImageUrl = doctor.ImageUrl,
                InitialFee = doctor.InitialFee,
                FollowUpFee = doctor.FollowUpFee,
                FollowUpCount = doctor.FollowUpCount,
                MaxFollowUps = doctor.MaxFollowUps,
                YearsOfExperience = doctor.YearsOfExperience,
                RatingPoints = doctor.RatingPoints,
                NumberOfRatings = doctor.NumberOfRatings,

                FirstNames = firstNames,
                LastNames = lastNames,
                Bio = bios,
                TranslationIds = translationIds,
                SpecializationNames = specializationNames
            };
        }

    }

}

