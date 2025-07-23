namespace SpendWiseAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmail(string toEmail, string username);
        // Task SendPasswordResetEmail(string toEmail, string resetToken);
    }

}
