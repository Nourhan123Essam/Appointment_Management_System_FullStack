using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Patient : BaseEntity
    {
        [Key]
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }

        // Navigation
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
