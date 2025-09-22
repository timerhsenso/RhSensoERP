namespace RhSensoERP.Core.Abstractions.Services;

public interface IEmailService
{
    Task<bool> SendEmailConfirmationAsync(string toEmail, string fullName, string confirmationToken);
    Task<bool> SendPasswordResetAsync(string toEmail, string fullName, string resetToken);
    Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName);
}