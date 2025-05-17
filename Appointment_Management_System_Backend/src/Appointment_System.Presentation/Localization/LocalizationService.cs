using Appointment_System.Application.Localization;
using Appointment_System.Presentation.Resources;
using Microsoft.Extensions.Localization;

namespace Appointment_System.Presentation.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizationService(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];
    }
}
