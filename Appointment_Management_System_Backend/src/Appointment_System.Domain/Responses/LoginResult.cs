namespace Appointment_System.Domain.Responses
{
    public record LoginResult(
        string AccessToken,
        string RefreshToken,
        string SessionId,
        int ExpiresIn
    );
}
