namespace RhSensoERP.Shared.Application.Interfaces;

/// <summary>
/// Interface para serviço de email.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um email.
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
