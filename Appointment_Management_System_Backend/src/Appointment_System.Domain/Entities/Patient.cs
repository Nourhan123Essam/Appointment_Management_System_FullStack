using System.ComponentModel.DataAnnotations;
using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Patient : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
       public string Gender { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
