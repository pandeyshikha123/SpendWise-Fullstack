using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SpendWiseAPI.Services.Interfaces;

namespace SpendWiseAPI.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendWelcomeEmail(string toEmail, string username)
        {
            var from = _config["Smtp:From"] ?? throw new InvalidOperationException("Missing 'From' in config");
            var host = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(_config["Smtp:Port"], out int p) ? p : 587;
            var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out bool ssl) && ssl;
            var usernameConfig = _config["Smtp:Username"];
            var passwordConfig = _config["Smtp:Password"];

            var subject = "Welcome to SpendWise!";
            var body = $"<h2>Hi {username},</h2><p>Welcome to SpendWise. Thank you for registering!</p>";

            using var mail = new MailMessage(from, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(usernameConfig, passwordConfig)
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
