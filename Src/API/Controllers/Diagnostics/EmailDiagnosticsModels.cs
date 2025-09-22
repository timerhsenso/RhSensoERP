using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.API.Controllers.Diagnostics;

public sealed class EmailSmtpOptionsDto
{
    [Required] public string Host { get; set; } = "smtp.gmail.com";
    [Range(1, 65535)] public int Port { get; set; } = 587;

    /// <summary>Se true, usa SSL direto (porta 465). Se false e UseStartTls=true, usa STARTTLS (porta 587).</summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>Se true (e UseSsl=false), negocia STARTTLS (porta 587).</summary>
    public bool UseStartTls { get; set; } = true;

    public int TimeoutSeconds { get; set; } = 30;

    public string? Username { get; set; }
    public string? Password { get; set; }
}

public sealed class EmailAttachmentDto
{
    [Required] public string FileName { get; set; } = "";
    [Required] public string ContentType { get; set; } = "application/octet-stream";
    /// <summary>Conteúdo Base64</summary>
    [Required] public string ContentBase64 { get; set; } = "";
}

public sealed class EmailDiagnosticsRequest
{
    // SMTP
    [Required] public EmailSmtpOptionsDto Smtp { get; set; } = new();

    // Envelope / cabeçalhos
    /// <summary>Remetente exibido e envelope. Muitos servidores exigem que seja igual ao Username.</summary>
    [Required, EmailAddress] public string FromAddress { get; set; } = "";
    public string? FromName { get; set; } = "RhSensoERP";

    /// <summary>Opcional; se não setado, usa FromAddress como Sender.</summary>
    [EmailAddress] public string? SenderAddress { get; set; }
    public string? SenderName { get; set; }

    [MinLength(1)] public List<string> To { get; set; } = new();
    public List<string> Cc { get; set; } = new();
    public List<string> Bcc { get; set; } = new();
    public List<string> ReplyTo { get; set; } = new();

    public Dictionary<string, string>? Headers { get; set; }

    // Conteúdo
    [Required] public string Subject { get; set; } = "Teste de e-mail";
    public string? TextBody { get; set; } = "Corpo em texto";
    public string? HtmlBody { get; set; } = "<p><strong>Corpo</strong> em HTML</p>";

    public List<EmailAttachmentDto> Attachments { get; set; } = new();

    /// <summary>Se true, força From/Sender = SMTP Username (evita “Sender rejected”)</summary>
    public bool ForceFromEqualsUsername { get; set; } = true;
}
