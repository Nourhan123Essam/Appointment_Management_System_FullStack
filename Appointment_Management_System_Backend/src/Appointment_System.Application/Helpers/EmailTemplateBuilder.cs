using Appointment_System.Application.Localization;

namespace Appointment_System.Application.Helpers
{
    public static class EmailTemplateBuilder
    {
        public static string BuildResetPasswordEmail(string resetUrl, ILocalizationService localizer)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2 style='color: #004080;'>{localizer["ResetPasswordHeader"]}</h2>
                <p>{localizer["ResetPasswordInstruction"]}</p>
                <a href='{resetUrl}' style='
                    display: inline-block;
                    padding: 10px 20px;
                    background-color: #007BFF;
                    color: white;
                    text-decoration: none;
                    border-radius: 5px;
                    margin-top: 10px;'>
                    {localizer["ResetPasswordButton"]}
                </a>
                <p style='margin-top: 20px;'>{localizer["ResetPasswordIgnoreNote"]}</p>
            </div>";
        }

    }
}
