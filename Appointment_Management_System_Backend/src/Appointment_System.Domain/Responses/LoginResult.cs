namespace Appointment_System.Domain.Responses
{
    public record LoginResult(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn
    );
}
