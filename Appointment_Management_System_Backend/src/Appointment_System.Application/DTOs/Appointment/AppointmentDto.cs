namespace Appointment_System.Application.DTOs.Appointment
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string DoctorName { get; set; } // Optional: include more info if needed
    }
}
