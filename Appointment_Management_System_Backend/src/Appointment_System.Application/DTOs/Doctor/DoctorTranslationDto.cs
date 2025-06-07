using Appointment_System.Domain.Entities;
using Appointment_System.Domain.ValueObjects;

namespace Appointment_System.Application.DTOs.Doctor
{
    public class DoctorTranslationDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string Language { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Bio { get; set; } = null!;

        public DoctorTranslation ToEntity()
        {
            return new DoctorTranslation
            {
                Id = Id,
                DoctorId = DoctorId,
                Language = new Domain.ValueObjects.Language(Language),
                FirstName = FirstName,
                LastName = LastName,
                Bio = Bio
            };
        }

        public static DoctorTranslationDto FromEntity(DoctorTranslation translation)
        {
            return new DoctorTranslationDto
            {
                Id = translation.Id,
                DoctorId = translation.DoctorId,
                Language = translation.Language.Value,
                FirstName = translation.FirstName,
                LastName = translation.LastName,
                Bio = translation.Bio
            };
        }
    }

}
