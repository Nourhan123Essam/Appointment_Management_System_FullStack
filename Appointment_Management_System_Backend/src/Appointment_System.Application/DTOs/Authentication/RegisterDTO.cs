namespace Appointment_System.Application.DTOs.Authentication
{
    public class RegisterDTO
    {
        public string Password { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string Gender { get; set; }
    }
}
