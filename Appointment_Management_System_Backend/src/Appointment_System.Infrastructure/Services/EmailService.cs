using Appointment_System.Application.DTOs.Email;
using Appointment_System.Application.Interfaces.Services;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Net;

namespace Appointment_System.Infrastructure.Services
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailService()
        {
            _host = Environment.GetEnvironmentVariable("EMAIL_HOST")!;
            _port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!);
            _username = Environment.GetEnvironmentVariable("EMAIL_USERNAME")!;
            _password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")!;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_username, _password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
