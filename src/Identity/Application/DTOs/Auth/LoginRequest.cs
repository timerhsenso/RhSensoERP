// ============================================================================
// ARQUIVO ALTERADO - SUBSTITUIR: src/Identity/Application/DTOs/Auth/LoginRequest.cs
// ============================================================================

namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Request de login do usuário.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// ✅ ALTERADO - FASE 1: Identificador de login do usuário.
    /// Pode ser email (modo SaaS) ou cdusuario (modo Legacy/ADWin).
    /// O sistema determina automaticamente qual usar baseado no AuthMode do tenant.
    /// </summary>
    public string LoginIdentifier { get; init; } = string.Empty;

    /// <summary>Senha em texto plano.</summary>
    public string Senha { get; init; } = string.Empty;

    /// <summary>Lembrar-me (refresh token com duração estendida).</summary>
    public bool RememberMe { get; init; }

    /// <summary>Informações do dispositivo (opcional, para auditoria).</summary>
    public string? DeviceId { get; init; }

    /// <summary>Nome do dispositivo (opcional, para auditoria).</summary>
    public string? DeviceName { get; init; }
}
