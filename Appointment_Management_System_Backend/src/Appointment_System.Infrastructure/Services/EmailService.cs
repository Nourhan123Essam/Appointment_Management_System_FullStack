using Appointment_System.Application.Interfaces.Services;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Appointment_System.Infrastructure.Services
{
    
    //public class EmailService : IEmailService
    //{
    //    private readonly string _host;
    //    private readonly int _port;
    //    private readonly string _username;
    //    private readonly string _password;

    //    public EmailService()
    //    {
    //        _host = Environment.GetEnvironmentVariable("EMAIL_HOST")!;
    //        _port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!);
    //        _username = Environment.GetEnvironmentVariable("EMAIL_USERNAME")!;
    //        _password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")!;
    //    }

    //    public async Task SendEmailAsync(string to, string subject, string body)
    //    {
    //        var email = new MimeMessage();
    //        email.From.Add(MailboxAddress.Parse(_username));
    //        email.To.Add(MailboxAddress.Parse(to));
    //        email.Subject = subject;
    //        email.Body = new TextPart("html") { Text = body };

    //        using var smtp = new SmtpClient();
    //        await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
    //        await smtp.AuthenticateAsync(_username, _password);
    //        await smtp.SendAsync(email);
    //        await smtp.DisconnectAsync(true);
    //    }
    //}



    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_USERNAME")!));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(Environment.GetEnvironmentVariable("EMAIL_USERNAME")!,Environment.GetEnvironmentVariable("EMAIL_PASSWORD")!);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
