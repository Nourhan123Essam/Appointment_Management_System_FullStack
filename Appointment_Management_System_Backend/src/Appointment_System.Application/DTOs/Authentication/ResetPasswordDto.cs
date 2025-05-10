namespace Appointment_System.Application.DTOs.Authentication
{
    public record ResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
