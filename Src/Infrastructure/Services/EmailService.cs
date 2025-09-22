// ============================================================================
// 6. Implementação do EmailService para Desenvolvimento
// ============================================================================

// src/Infrastructure/Services/EmailService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RhSensoERP.Core.Abstractions.Services;
using System.Net;
using System.Net.Mail;

namespace RhSensoERP.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableEmailSending;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configurações do email
        _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@rhsensorp.com";
        _fromName = _configuration["Email:FromName"] ?? "RhSensoERP System";
        _enableEmailSending = _configuration.GetValue<bool>("Email:EnableSending", false);
    }

    public async Task<bool> SendEmailConfirmationAsync(string toEmail, string fullName, string confirmationToken)
    {
        try
        {
            var subject = "Confirme seu email - RhSensoERP";
            var body = GenerateEmailConfirmationTemplate(fullName, confirmationToken, toEmail);

            return await SendEmailAsync(toEmail, fullName, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de confirmação para {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetAsync(string toEmail, string fullName, string resetToken)
    {
        try
        {
            var subject = "Redefinição de senha - RhSensoERP";
            var body = GeneratePasswordResetTemplate(fullName, resetToken, toEmail);

            return await SendEmailAsync(toEmail, fullName, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de redefinição de senha para {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName)
    {
        try
        {
            var subject = "Bem-vindo ao RhSensoERP!";
            var body = GenerateWelcomeTemplate(fullName);

            return await SendEmailAsync(toEmail, fullName, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de boas-vindas para {Email}", toEmail);
            return false;
        }
    }

    private async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        if (!_enableEmailSending)
        {
            // Para desenvolvimento: apenas loggar o email
            _logger.LogInformation("EMAIL SIMULADO (EnableSending=false):");
            _logger.LogInformation("Para: {ToEmail} ({ToName})", toEmail, toName);
            _logger.LogInformation("Assunto: {Subject}", subject);
            _logger.LogInformation("Conteúdo: {Body}", body);
            return true;
        }

        try
        {
            // Configuração SMTP
            var smtpServer = _configuration["Email:SmtpServer"];
            var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUsername))
            {
                _logger.LogWarning("Configurações SMTP incompletas. Email não enviado.");
                return false;
            }

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(new MailAddress(toEmail, toName));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await client.SendMailAsync(message);

            _logger.LogInformation("Email enviado com sucesso para {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email via SMTP para {Email}", toEmail);
            return false;
        }
    }

    private string GenerateEmailConfirmationTemplate(string fullName, string token, string email)
    {
        var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:57148";
        var confirmationUrl = $"{baseUrl}/api/v1/saas/confirm-email";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Confirme seu Email</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #2c5aa0;'>Bem-vindo ao RhSensoERP!</h2>
        
        <p>Olá, <strong>{fullName}</strong>!</p>
        
        <p>Obrigado por se cadastrar no RhSensoERP. Para completar seu cadastro, confirme seu endereço de email clicando no link abaixo ou usando o código fornecido.</p>
        
        <div style='background: #f4f4f4; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>Seu código de confirmação:</strong></p>
            <p style='font-size: 24px; font-weight: bold; color: #2c5aa0; letter-spacing: 2px;'>{token}</p>
        </div>
        
        <p>Você pode usar este código no aplicativo ou fazer uma requisição para:</p>
        <p><code>POST {confirmationUrl}</code></p>
        <p>Com o payload:</p>
        <pre style='background: #f8f8f8; padding: 10px; border-radius: 3px;'>{{
  ""email"": ""{email}"",
  ""token"": ""{token}""
}}</pre>
        
        <p>Se você não solicitou este cadastro, pode ignorar este email.</p>
        
        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
        <p style='font-size: 12px; color: #666;'>
            RhSensoERP System<br>
            Este é um email automático, não responda.
        </p>
    </div>
</body>
</html>";
    }

    private string GeneratePasswordResetTemplate(string fullName, string token, string email)
    {
        var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:57148";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Redefinição de Senha</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #d32f2f;'>Redefinição de Senha</h2>
        
        <p>Olá, <strong>{fullName}</strong>!</p>
        
        <p>Recebemos uma solicitação para redefinir a senha da sua conta no RhSensoERP.</p>
        
        <div style='background: #f4f4f4; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>Seu código de redefinição:</strong></p>
            <p style='font-size: 24px; font-weight: bold; color: #d32f2f; letter-spacing: 2px;'>{token}</p>
        </div>
        
        <p>Use este código para redefinir sua senha. O código expira em 1 hora.</p>
        
        <p>Se você não solicitou esta redefinição, pode ignorar este email com segurança.</p>
        
        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
        <p style='font-size: 12px; color: #666;'>
            RhSensoERP System<br>
            Este é um email automático, não responda.
        </p>
    </div>
</body>
</html>";
    }

    private string GenerateWelcomeTemplate(string fullName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Bem-vindo!</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #2c5aa0;'>Bem-vindo ao RhSensoERP!</h2>
        
        <p>Olá, <strong>{fullName}</strong>!</p>
        
        <p>Seu email foi confirmado com sucesso! Agora você pode aproveitar todos os recursos do RhSensoERP.</p>
        
        <p>Recursos disponíveis:</p>
        <ul>
            <li>✅ Autenticação segura</li>
            <li>✅ Gestão multi-tenant</li>
            <li>✅ APIs RESTful</li>
            <li>✅ Logs de auditoria</li>
        </ul>
        
        <p>Se tiver dúvidas ou precisar de suporte, entre em contato conosco.</p>
        
        <p>Bem-vindo à bordo!</p>
        
        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
        <p style='font-size: 12px; color: #666;'>
            RhSensoERP System<br>
            Este é um email automático, não responda.
        </p>
    </div>
</body>
</html>";
    }
}

// ============================================================================
// 7. Configuração da Injeção de Dependência
// ============================================================================

// Adicione no Program.cs ou Startup.cs:
// services.AddScoped<IEmailService, EmailService>();

// ============================================================================
// 8. Configurações no appsettings.json
// ============================================================================

/*
{
  "Email": {
    "EnableSending": false,  // true para produção, false para desenvolvimento
    "FromEmail": "noreply@rhsensoerp.com",
    "FromName": "RhSensoERP System",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "seu-email@gmail.com",
    "SmtpPassword": "sua-senha-ou-app-password"
  },
  "App": {
    "BaseUrl": "https://localhost:57148"
  }
}
*/