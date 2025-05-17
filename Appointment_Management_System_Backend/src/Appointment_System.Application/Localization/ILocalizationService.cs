namespace Appointment_System.Application.Localization
{
    public interface ILocalizationService
    {
        string this[string key] { get; }
    }
}
