using System.ComponentModel.DataAnnotations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MimeKit;

namespace RhSensoERP.API.Controllers.Diagnostics;

/// <summary>
/// Endpoint de diagnóstico de e-mail (apenas DEV/flagado). Permite testar SMTP, SSL/STARTTLS,
/// remetente, destinatários, HTML/TXT e anexos.
/// </summary>
[ApiController]
[Route("api/v1/diagnostics/email")]
[Produces("application/json")]
public sealed class EmailDiagnosticsController : ControllerBase
{
    private readonly IHostEnvironment _env;
    private readonly IConfiguration _cfg;
    private readonly ILogger<EmailDiagnosticsController> _logger;

    public EmailDiagnosticsController(IHostEnvironment env, IConfiguration cfg, ILogger<EmailDiagnosticsController> logger)
    {
        _env = env;
        _cfg = cfg;
        _logger = logger;
    }

    /// <summary>
    /// Envia um e-mail de teste com todas as opções configuráveis via corpo da requisição.
    /// </summary>
    /// <remarks>
    /// Segurança:
    /// - Por padrão, habilitado apenas em Development.
    /// - Para habilitar em outro ambiente, defina `Diagnostics:EnableEmailTester=true` e proteja com [Authorize].
    /// </remarks>
    [HttpPost("send")]
    // Descomente em produção para exigir autenticação/admin:
    // [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> SendAsync([FromBody, Required] EmailDiagnosticsRequest req, CancellationToken ct)
    {
        // Gate de segurança
        var allow = _env.IsDevelopment() || _cfg.GetValue<bool>("Diagnostics:EnableEmailTester");
        if (!allow)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Ajustes anti “Sender rejected”
        var username = req.Smtp.Username;
        if (req.ForceFromEqualsUsername && !string.IsNullOrWhiteSpace(username))
        {
            req.FromAddress = username!;
            req.SenderAddress ??= username;
        }
        var senderAddress = string.IsNullOrWhiteSpace(req.SenderAddress) ? req.FromAddress : req.SenderAddress!;

        // Montagem da mensagem
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(req.FromName ?? req.FromAddress, req.FromAddress));
        msg.Sender = new MailboxAddress(req.SenderName ?? req.FromName ?? req.FromAddress, senderAddress);

        foreach (var t in req.To?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>())
            msg.To.Add(MailboxAddress.Parse(t));

        foreach (var t in req.Cc?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>())
            msg.Cc.Add(MailboxAddress.Parse(t));

        foreach (var t in req.Bcc?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>())
            msg.Bcc.Add(MailboxAddress.Parse(t));

        foreach (var r in req.ReplyTo?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>())
            msg.ReplyTo.Add(MailboxAddress.Parse(r));

        if (req.Headers is not null)
        {
            foreach (var kv in req.Headers)
            {
                // Evita sobrescrever cabeçalhos críticos
                if (!string.Equals(kv.Key, "From", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(kv.Key, "To", StringComparison.OrdinalIgnoreCase))
                {
                    msg.Headers[kv.Key] = kv.Value;
                }
            }
        }

        msg.Subject = req.Subject;

        var body = new BodyBuilder();
        if (!string.IsNullOrWhiteSpace(req.HtmlBody)) body.HtmlBody = req.HtmlBody;
        if (!string.IsNullOrWhiteSpace(req.TextBody)) body.TextBody = req.TextBody;

        if (req.Attachments is not null)
        {
            foreach (var a in req.Attachments)
            {
                var bytes = Convert.FromBase64String(a.ContentBase64);
                body.Attachments.Add(a.FileName, bytes, ContentType.Parse(a.ContentType));
            }
        }

        msg.Body = body.ToMessageBody();

        // Envio
        var start = DateTimeOffset.UtcNow;
        string secureMode = req.Smtp.UseSsl ? "SslOnConnect" :
                            req.Smtp.UseStartTls ? "StartTls" : "None";

        using var smtp = new SmtpClient();
        smtp.Timeout = (req.Smtp.TimeoutSeconds <= 0 ? 30 : req.Smtp.TimeoutSeconds) * 1000;

        var socket = req.Smtp.UseSsl ? SecureSocketOptions.SslOnConnect :
                     req.Smtp.UseStartTls ? SecureSocketOptions.StartTls :
                                            SecureSocketOptions.Auto;

        try
        {
            await smtp.ConnectAsync(req.Smtp.Host, req.Smtp.Port, socket, ct);

            if (!string.IsNullOrWhiteSpace(req.Smtp.Username))
            {
                await smtp.AuthenticateAsync(req.Smtp.Username, req.Smtp.Password ?? "", ct);
            }

            await smtp.SendAsync(msg, ct);
            await smtp.DisconnectAsync(true, ct);

            var elapsed = (DateTimeOffset.UtcNow - start).TotalMilliseconds;
            _logger.LogInformation("EmailDiagnostics OK → {To} via {Host}:{Port} ({Mode}) em {Ms}ms",
                string.Join(",", msg.To.Select(x => x.ToString())), req.Smtp.Host, req.Smtp.Port, secureMode, elapsed);

            return Ok(new
            {
                ok = true,
                smtp = new { req.Smtp.Host, req.Smtp.Port, secureMode },
                from = new { req.FromName, req.FromAddress, sender = senderAddress },
                to = msg.To.Select(x => x.ToString()).ToArray(),
                cc = msg.Cc.Select(x => x.ToString()).ToArray(),
                bcc = msg.Bcc.Select(x => x.ToString()).ToArray(),
                elapsedMs = Math.Round(elapsed, 1)
            });
        }
        catch (Exception ex)
        {
            var elapsed = (DateTimeOffset.UtcNow - start).TotalMilliseconds;
            _logger.LogError(ex, "EmailDiagnostics FAIL via {Host}:{Port} ({Mode}) em {Ms}ms", req.Smtp.Host, req.Smtp.Port, secureMode, elapsed);

            return StatusCode(500, new
            {
                ok = false,
                error = ex.Message,
                smtp = new { req.Smtp.Host, req.Smtp.Port, secureMode },
                elapsedMs = Math.Round(elapsed, 1),
                hint = "Verifique Host/Porta/SSL/STARTTLS e se From/Sender=Username quando o servidor exige."
            });
        }
    }
}
