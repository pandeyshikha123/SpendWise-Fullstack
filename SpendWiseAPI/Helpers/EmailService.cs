using RazorLight;
using System.Net;
using System.Net.Mail;
using SpendWiseAPI.Services.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly RazorLightEngine _razor;

    public EmailService(IConfiguration config)
{
    _config = config;

    // 👇 This tells RazorLight to look in your local Views/EmailTemplates folder
    _razor = new RazorLightEngineBuilder()
        .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Views/EmailTemplates"))
        .UseMemoryCachingProvider()
        .Build();
}
    public async Task SendWelcomeEmail(string toEmail, string userName)
    {
        var model = new { UserName = userName };

        // var htmlBody = await _razor.CompileRenderAsync("Views/EmailTemplates/Welcome", model);
        var htmlBody = await _razor.CompileRenderAsync("Welcome.cshtml", model);


        await SendEmail(toEmail, "🎉 Welcome to SpendWise!", htmlBody);
    }

    private async Task SendEmail(string toEmail, string subject, string htmlBody)
    {
        var from = _config["EmailSettings:From"];
        var smtpHost = _config["EmailSettings:Host"];
        var port = int.Parse(_config["EmailSettings:Port"]);
        var ssl = bool.Parse(_config["EmailSettings:EnableSsl"]);
        var username = _config["EmailSettings:Username"];
        var password = _config["EmailSettings:Password"];

        using var message = new MailMessage(from, toEmail)
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        using var smtp = new SmtpClient(smtpHost, port)
        {
            EnableSsl = ssl,
            Credentials = new NetworkCredential(username, password)
        };

        await smtp.SendMailAsync(message);
    }
}
