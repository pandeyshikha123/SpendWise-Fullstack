using SpendWiseAPI.Models;

namespace SpendWiseAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmail(string toEmail, string username);
         Task SendExpenseAddedEmail(string email, string username, string category, decimal amount, DateTime date, string description);
        Task SendExpenseUpdatedEmail(string email, string username, string category, decimal amount, DateTime date, string description);
        // Task SendPasswordResetEmail(string toEmail, string resetToken);
    }

}
