using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        [Key]
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }
        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int MaxFollowUps { get; set; }
        public int RatingPoints { get; set; }
        public int NumberOfRatings { get; set; }

        // Navigation
        public virtual ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
        public virtual ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }

}
