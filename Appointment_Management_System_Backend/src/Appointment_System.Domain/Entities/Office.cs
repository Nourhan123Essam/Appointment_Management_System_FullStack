using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment_System.Domain.Entities
{
    public class Office : BaseEntity
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Zip { get; set; } = null!;

        // Navigation
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
