namespace Appointment_System.Application.Helpers
{
    public static class EmailTemplateBuilder
    {
        public static string BuildResetPasswordEmail(string resetUrl)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2 style='color: #004080;'>Reset Your Password</h2>
                <p>You requested to reset your password. Click the button below to proceed:</p>
                <a href='{resetUrl}' style='
                    display: inline-block;
                    padding: 10px 20px;
                    background-color: #007BFF;
                    color: white;
                    text-decoration: none;
                    border-radius: 5px;
                    margin-top: 10px;'>
                    Reset Password
                </a>
                <p style='margin-top: 20px;'>If you didn’t request this, you can safely ignore this email.</p>
            </div>";
        }
    }
}
