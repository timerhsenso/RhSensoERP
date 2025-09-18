using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.Auth;

/// <summary>
/// Autenticação usando sistema legacy existente
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ILegacyAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILegacyAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login no sistema legacy com logs de segurança
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        [FromServices] IValidator<LoginRequestDto> validator,
        CancellationToken ct)
    {
        var startTime = DateTime.UtcNow;
        var ipAddress = GetClientIpAddress();
        var userAgent = Request.Headers.UserAgent.ToString();
        var sessionId = Guid.NewGuid().ToString("N")[..8]; // ID curto para correlação

        // Log da tentativa de login
        using var loginScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["SessionId"] = sessionId,
            ["IPAddress"] = ipAddress,
            ["UserAgent"] = userAgent,
            ["Usuario"] = request.CdUsuario,
            ["Timestamp"] = startTime
        });

        _logger.LogInformation("Tentativa de login iniciada para usuário {Usuario} de IP {IPAddress}",
            request.CdUsuario, ipAddress);

        try
        {
            // Validação de entrada
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

                _logger.LogWarning("Validação de entrada falhou para usuário {Usuario}: {ValidationErrors}",
                    request.CdUsuario, string.Join(", ", errors.SelectMany(kvp => kvp.Value)));

                return BadRequest(ApiResponse<object>.Fail("Dados de entrada inválidos", errors));
            }

            // Tentativa de autenticação
            var result = await _authService.AuthenticateAsync(request.CdUsuario, request.Senha, ct);
            var duration = DateTime.UtcNow - startTime;

            if (!result.Success)
            {
                // Log de falha de autenticação
                _logger.LogWarning("Login FALHADO para usuário {Usuario} de IP {IPAddress} em {Duration}ms: {Motivo}",
                    request.CdUsuario, ipAddress, duration.TotalMilliseconds, result.ErrorMessage);

                // Log de segurança para análise de padrões
                _logger.LogInformation("SECURITY_EVENT: LOGIN_FAILED | User: {Usuario} | IP: {IPAddress} | UserAgent: {UserAgent} | Reason: {Motivo}",
                    request.CdUsuario, ipAddress, userAgent, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            // Carregar permissões
            var permissions = await _authService.GetUserPermissionsAsync(request.CdUsuario, ct);

            var response = new LoginResponseDto(
                result.AccessToken!,
                result.UserData!,
                permissions.Groups,
                permissions.Permissions);

            // Log de sucesso
            _logger.LogInformation("Login SUCESSO para usuário {Usuario} de IP {IPAddress} em {Duration}ms",
                request.CdUsuario, ipAddress, duration.TotalMilliseconds);

            // Log de segurança para auditoria
            _logger.LogInformation("SECURITY_EVENT: LOGIN_SUCCESS | User: {Usuario} | IP: {IPAddress} | UserAgent: {UserAgent} | Groups: {Groups}",
                request.CdUsuario, ipAddress, userAgent, string.Join(",", permissions.Groups.Select(g => g.CdGrUser)));

            return Ok(ApiResponse<LoginResponseDto>.Ok(response));
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;

            _logger.LogError(ex, "ERRO INTERNO durante login do usuário {Usuario} de IP {IPAddress} em {Duration}ms",
                request.CdUsuario, ipAddress, duration.TotalMilliseconds);

            // Log de segurança para erros críticos
            _logger.LogError("SECURITY_EVENT: LOGIN_ERROR | User: {Usuario} | IP: {IPAddress} | Error: {ErrorType}",
                request.CdUsuario, ipAddress, ex.GetType().Name);

            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Verifica habilitação com log de auditoria
    /// </summary>
    [HttpGet("check-habilitacao")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> CheckHabilitacao(
        [FromQuery] string cdsistema,
        [FromQuery] string cdfuncao,
        CancellationToken ct)
    {
        var cdusuario = User.Identity?.Name;
        var ipAddress = GetClientIpAddress();

        if (string.IsNullOrEmpty(cdusuario))
        {
            _logger.LogWarning("Tentativa de verificação de habilitação sem usuário autenticado de IP {IPAddress}",
                ipAddress);
            return Unauthorized();
        }

        try
        {
            var permissions = await _authService.GetUserPermissionsAsync(cdusuario, ct);
            var hasAccess = _authService.CheckHabilitacao(cdsistema, cdfuncao, permissions);

            // Log de auditoria de permissões
            _logger.LogInformation("SECURITY_EVENT: PERMISSION_CHECK | User: {Usuario} | IP: {IPAddress} | Sistema: {Sistema} | Funcao: {Funcao} | Resultado: {Resultado}",
                cdusuario, ipAddress, cdsistema, cdfuncao, hasAccess ? "PERMITIDO" : "NEGADO");

            if (!hasAccess)
            {
                _logger.LogWarning("Acesso NEGADO para usuário {Usuario} ao sistema {Sistema}, função {Funcao}",
                    cdusuario, cdsistema, cdfuncao);
            }

            return Ok(ApiResponse<bool>.Ok(hasAccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar habilitação para usuário {Usuario}, sistema {Sistema}, função {Funcao}",
                cdusuario, cdsistema, cdfuncao);

            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém o endereço IP real do cliente, considerando proxies e load balancers
    /// </summary>
    private string GetClientIpAddress()
    {
        // Verificar headers de proxy primeiro
        var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            // X-Forwarded-For pode conter múltiplos IPs, o primeiro é o cliente original
            return xForwardedFor.Split(',')[0].Trim();
        }

        var xRealIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        // Fallback para IP da conexão direta
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}