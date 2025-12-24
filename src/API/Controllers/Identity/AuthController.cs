// C:\Users\eduardo\source\repos\RhSAnalise\src\API\Controllers\Identity\AuthController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.Features.Auth.Commands;
using RhSensoERP.Identity.Application.Features.Auth.Queries;
using RhSensoERP.Identity.Application.Services; // ✅ FASE 5: Adicionar
using RhSensoERP.Identity.Core.Entities; // ✅ FASE 5: Adicionar
using System.Security.Claims;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para autenticação e gestão de tokens.
/// ✅ FASE 5: Auditoria completa implementada
/// </summary>
[ApiController]
[Route("api/identity/auth")]
[ApiExplorerSettings(GroupName = "Identity")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    private readonly ISecurityAuditService _auditService; // ✅ FASE 5: Adicionar

    public AuthController(
        IMediator mediator,
        ILogger<AuthController> logger,
        ISecurityAuditService auditService) // ✅ FASE 5: Adicionar
    {
        _mediator = mediator;
        _logger = logger;
        _auditService = auditService; // ✅ FASE 5: Adicionar
    }

    /// <summary>
    /// Autentica um usuário e retorna tokens JWT.
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Tokens de acesso e refresh</returns>
    /// <response code="200">Login bem-sucedido</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Credenciais inválidas ou conta bloqueada</response>
    /// <response code="429">Limite de requisições excedido</response>
    /// <response code="504">Timeout na requisição</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ipAddress = GetIpAddress();
        var userAgent = GetUserAgent();

        _logger.LogInformation(
            "🔐 Tentativa de login: {LoginIdentifier} | IP: {IpAddress}",
            request.LoginIdentifier,
            ipAddress);

        try
        {
            var command = new LoginCommand(request, ipAddress, userAgent);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "❌ Falha no login: {LoginIdentifier} | Erro: {ErrorCode} - {ErrorMessage}",
                    request.LoginIdentifier,
                    result.Error.Code,
                    result.Error.Message);

                // ✅ FASE 5: Auditar falha de login
                await _auditService.LogAsync(
                    eventType: SecurityEventType.LoginFailed,
                    eventCategory: SecurityEventCategory.Authentication,
                    severity: result.Error.Code == "ACCOUNT_LOCKED"
                        ? SecuritySeverity.Critical
                        : SecuritySeverity.Warning,
                    description: $"Falha de login para '{request.LoginIdentifier}'",
                    httpContext: HttpContext,
                    isSuccess: false,
                    username: request.LoginIdentifier,
                    errorMessage: $"{result.Error.Code}: {result.Error.Message}",
                    ct: ct
                );

                return result.Error.Code switch
                {
                    "TIMEOUT" => StatusCode(504, new { error = result.Error.Code, message = result.Error.Message }),
                    "VALIDATION_ERROR" => BadRequest(new { error = result.Error.Code, message = result.Error.Message }),
                    "INVALID_CREDENTIALS" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "USER_INACTIVE" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "ACCOUNT_LOCKED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "EMAIL_NOT_CONFIRMED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "2FA_REQUIRED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message, require2FA = true }),
                    _ => StatusCode(500, new { error = "LOGIN_ERROR", message = "Erro ao processar login." })
                };
            }

            _logger.LogInformation("✅ Login bem-sucedido: {LoginIdentifier}", request.LoginIdentifier);

            // ✅ FASE 5: Auditar login bem-sucedido
            // Extrair IdUserSecurity do resultado (assumindo que está disponível)
            var authResponse = result.Value;
            await _auditService.LogAsync(
                eventType: SecurityEventType.Login,
                eventCategory: SecurityEventCategory.Authentication,
                severity: SecuritySeverity.Info,
                description: $"Login bem-sucedido para '{request.LoginIdentifier}'",
                httpContext: HttpContext,
                isSuccess: true,
                username: request.LoginIdentifier,
                ct: ct
            );

            return Ok(result.Value);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "⏱️ Timeout/Cancelamento na requisição de login: {LoginIdentifier} | IP: {IpAddress}",
                request.LoginIdentifier,
                ipAddress);

            // ✅ FASE 5: Auditar timeout
            await _auditService.LogAsync(
                eventType: SecurityEventType.LoginFailed,
                eventCategory: SecurityEventCategory.Authentication,
                severity: SecuritySeverity.Warning,
                description: $"Timeout na tentativa de login para '{request.LoginIdentifier}'",
                httpContext: HttpContext,
                isSuccess: false,
                username: request.LoginIdentifier,
                errorMessage: "TIMEOUT: Requisição cancelada ou excedeu tempo limite",
                ct: ct
            );

            return StatusCode(504, new
            {
                error = "TIMEOUT",
                message = "A requisição foi cancelada ou excedeu o tempo limite."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "💥 Erro inesperado no login: {LoginIdentifier} | IP: {IpAddress}",
                request.LoginIdentifier,
                ipAddress);

            // ✅ FASE 5: Auditar erro interno
            await _auditService.LogAsync(
                eventType: SecurityEventType.LoginFailed,
                eventCategory: SecurityEventCategory.Authentication,
                severity: SecuritySeverity.Error,
                description: $"Erro interno na tentativa de login para '{request.LoginIdentifier}'",
                httpContext: HttpContext,
                isSuccess: false,
                username: request.LoginIdentifier,
                errorMessage: $"INTERNAL_ERROR: {ex.Message}",
                ct: ct
            );

            return StatusCode(500, new
            {
                error = "INTERNAL_ERROR",
                message = "Erro interno ao processar login."
            });
        }
    }

    /// <summary>
    /// Renova tokens usando refresh token.
    /// </summary>
    /// <param name="request">Access token expirado e refresh token válido</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Novos tokens de acesso e refresh</returns>
    /// <response code="200">Tokens renovados com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Refresh token inválido ou expirado</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var ipAddress = GetIpAddress();

        _logger.LogDebug("🔄 Tentativa de refresh token | IP: {IpAddress}", ipAddress);

        try
        {
            var command = new RefreshTokenCommand(request, ipAddress);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "❌ Falha no refresh token | Erro: {ErrorCode} - {ErrorMessage}",
                    result.Error.Code,
                    result.Error.Message);

                // ✅ FASE 5: Auditar falha no refresh token
                await _auditService.LogAsync(
                    eventType: SecurityEventType.TokenRefreshed,
                    eventCategory: SecurityEventCategory.TokenManagement,
                    severity: SecuritySeverity.Warning,
                    description: "Falha ao renovar token",
                    httpContext: HttpContext,
                    isSuccess: false,
                    errorMessage: $"{result.Error.Code}: {result.Error.Message}",
                    ct: ct
                );

                return result.Error.Code switch
                {
                    "INVALID_REFRESH_TOKEN" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "USER_NOT_FOUND" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "ACCOUNT_LOCKED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "VALIDATION_ERROR" => BadRequest(new { error = result.Error.Code, message = result.Error.Message }),
                    _ => StatusCode(500, new { error = "REFRESH_ERROR", message = "Erro ao renovar tokens." })
                };
            }

            _logger.LogInformation("✅ Tokens renovados com sucesso");

            // ✅ FASE 5: Auditar refresh token bem-sucedido
            await _auditService.LogAsync(
                eventType: SecurityEventType.TokenRefreshed,
                eventCategory: SecurityEventCategory.TokenManagement,
                severity: SecuritySeverity.Info,
                description: "Token renovado com sucesso",
                httpContext: HttpContext,
                isSuccess: true,
                ct: ct
            );

            return Ok(result.Value);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("⏱️ Timeout na renovação de tokens | IP: {IpAddress}", ipAddress);

            // ✅ FASE 5: Auditar timeout no refresh
            await _auditService.LogAsync(
                eventType: SecurityEventType.TokenRefreshed,
                eventCategory: SecurityEventCategory.TokenManagement,
                severity: SecuritySeverity.Warning,
                description: "Timeout ao renovar token",
                httpContext: HttpContext,
                isSuccess: false,
                errorMessage: "TIMEOUT: Requisição cancelada",
                ct: ct
            );

            return StatusCode(504, new { error = "TIMEOUT", message = "A requisição foi cancelada." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro inesperado no refresh token | IP: {IpAddress}", ipAddress);

            // ✅ FASE 5: Auditar erro interno no refresh
            await _auditService.LogAsync(
                eventType: SecurityEventType.TokenRefreshed,
                eventCategory: SecurityEventCategory.TokenManagement,
                severity: SecuritySeverity.Error,
                description: "Erro interno ao renovar token",
                httpContext: HttpContext,
                isSuccess: false,
                errorMessage: $"INTERNAL_ERROR: {ex.Message}",
                ct: ct
            );

            return StatusCode(500, new { error = "INTERNAL_ERROR", message = "Erro ao renovar tokens." });
        }
    }

    /// <summary>
    /// Logout do usuário (revoga refresh tokens).
    /// </summary>
    /// <param name="request">Parâmetros de logout</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de logout</returns>
    /// <response code="200">Logout realizado com sucesso</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        // ✅ CORREÇÃO: Usar o claim "sub" ou "Id" que contém o GUID do usuário
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub")
                  ?? User.FindFirstValue("Id");

        var cdUsuario = User.FindFirstValue("cdusuario"); // Para logs e auditoria

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { error = "INVALID_TOKEN", message = "Token inválido." });
        }

        _logger.LogInformation("🚪 Logout iniciado: {CdUsuario} (Id: {UserId})", cdUsuario, userId);

        try
        {
            var command = new LogoutCommand(userId, request); // ✅ Passa o GUID
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "❌ Falha no logout: {CdUsuario} | Erro: {ErrorCode} - {ErrorMessage}",
                    cdUsuario,
                    result.Error.Code,
                    result.Error.Message);

                // ✅ FASE 5: Auditar falha no logout
                await _auditService.LogAsync(
                    eventType: SecurityEventType.Logout,
                    eventCategory: SecurityEventCategory.Authentication,
                    severity: SecuritySeverity.Warning,
                    description: $"Falha no logout para '{cdUsuario}'",
                    httpContext: HttpContext,
                    isSuccess: false,
                    username: cdUsuario,
                    errorMessage: $"{result.Error.Code}: {result.Error.Message}",
                    ct: ct
                );

                return BadRequest(new { error = result.Error.Code, message = result.Error.Message });
            }

            _logger.LogInformation("✅ Logout realizado com sucesso: {CdUsuario}", cdUsuario);

            // ✅ FASE 5: Auditar logout bem-sucedido
            await _auditService.LogAsync(
                eventType: SecurityEventType.Logout,
                eventCategory: SecurityEventCategory.Authentication,
                severity: SecuritySeverity.Info,
                description: $"Logout realizado para '{cdUsuario}'",
                httpContext: HttpContext,
                isSuccess: true,
                username: cdUsuario,
                ct: ct
            );

            return Ok(new { message = "Logout realizado com sucesso." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro inesperado no logout: {CdUsuario}", cdUsuario);

            // ✅ FASE 5: Auditar erro interno no logout
            await _auditService.LogAsync(
                eventType: SecurityEventType.Logout,
                eventCategory: SecurityEventCategory.Authentication,
                severity: SecuritySeverity.Error,
                description: $"Erro interno no logout para '{cdUsuario}'",
                httpContext: HttpContext,
                isSuccess: false,
                username: cdUsuario,
                errorMessage: $"INTERNAL_ERROR: {ex.Message}",
                ct: ct
            );

            return StatusCode(500, new { error = "INTERNAL_ERROR", message = "Erro ao processar logout." });
        }
    }

    /// <summary>
    /// Obtém informações do usuário autenticado.
    /// </summary>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Dados do usuário retornados com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var cdUsuario = User.FindFirstValue("cdusuario");

        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            return Unauthorized(new { error = "INVALID_TOKEN", message = "Token inválido." });
        }

        try
        {
            var query = new GetCurrentUserQuery(cdUsuario);
            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.Error.Code, message = result.Error.Message });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro ao obter dados do usuário: {CdUsuario}", cdUsuario);
            return StatusCode(500, new { error = "INTERNAL_ERROR", message = "Erro ao obter dados do usuário." });
        }
    }

    /// <summary>
    /// Valida se o token JWT atual ainda é válido (health check de autenticação).
    /// </summary>
    /// <returns>Status de validação</returns>
    /// <response code="200">Token válido</response>
    /// <response code="401">Token inválido ou expirado</response>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new
        {
            valid = true,
            userId,
            userName,
            expiresAt = User.FindFirstValue("exp")
        });
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
        }

        if (Request.Headers.ContainsKey("X-Real-IP"))
        {
            return Request.Headers["X-Real-IP"].ToString();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string? GetUserAgent()
    {
        return Request.Headers.UserAgent.ToString();
    }
}
