using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Application.DTOs.Doctor
{
    public class CreateDoctorDto
    {
        public List<DoctorTranslationDto> Translations { get; set; } = new();

        public List<int> SpecializationIds { get; set; } = new();

        public string UserId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string ImageUrl { get; set; }

        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int MaxFollowUps { get; set; }
        public int YearsOfExperience { get; set; }

        public List<CreateQualificationDto> Qualifications { get; set; } = new();
        public List<CreateDoctorAvailabilityDto> Availabilities { get; set; } = new();

        ///////////////////////////////////////////////////////////////////////////////
        
        public Domain.Entities.Doctor ToEntity(string userId)
        {
            var doctor = new Domain.Entities.Doctor
            {
                UserId = userId,
                Email = Email,
                Phone = Phone,
                InitialFee = InitialFee,
                FollowUpFee = FollowUpFee,
                MaxFollowUps = MaxFollowUps,
                YearsOfExperience = YearsOfExperience,
                ImageUrl = ImageUrl,
                CreatedAt = DateTime.UtcNow,
                RatingPoints = 0,
                NumberOfRatings = 0,

                Translations = Translations.Select(t => new DoctorTranslation
                {
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    Bio = t.Bio,
                    Language = Language.From(t.Language)
                }).ToList(),

                DoctorSpecializations = SpecializationIds.Select(id => new DoctorSpecialization
                {
                    SpecializationId = id,
                    Specialization = null // optionally set to be safe not override the SpecializationId
                }).ToList(),

                Qualifications = Qualifications.Select(q => new Qualification
                {
                    YearEarned = q.Date,
                    Translations = q.Translations.Select(t => new QualificationTranslation
                    {
                        Language = Language.From(t.Language),
                        QualificationName = t.Title,
                        IssuingInstitution = t.Institution
                    }).ToList()
                }).ToList(),

                Availabilities = Availabilities.Select(a => new Availability
                {
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    OfficeId = a.OfficeId,
                }).ToList()
            };

            return doctor;
        }

    }

}
