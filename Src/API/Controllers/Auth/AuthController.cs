using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Application.Security.Auth.Validators;
using RhSensoERP.Core.Security.Auth;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.Auth;

/// <summary>
/// Autenticação multi-estratégia (OnPrem, SaaS, Windows)
/// Usa o padrão Strategy para suportar diferentes modos de autenticação
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILegacyAuthService _legacyAuthService;
    private readonly AuthOptions _authOptions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthenticationService authService,
        ILegacyAuthService legacyAuthService,
        IOptionsMonitor<AuthOptions> authOptions,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _legacyAuthService = legacyAuthService;
        _authOptions = authOptions.CurrentValue;
        _logger = logger;
    }

    /// <summary>
    /// Login usando estratégia configurada (OnPrem, SaaS ou Windows)
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
        var sessionId = Guid.NewGuid().ToString("N")[..8];

        // Log da tentativa de login
        using var loginScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["SessionId"] = sessionId,
            ["IPAddress"] = ipAddress,
            ["UserAgent"] = userAgent,
            ["Usuario"] = request.CdUsuario,
            ["AuthMode"] = _authOptions.Mode.ToString(),
            ["Timestamp"] = startTime
        });

        _logger.LogInformation("Tentativa de login iniciada para usuário {Usuario} de IP {IPAddress} usando modo {AuthMode}",
            request.CdUsuario, ipAddress, _authOptions.Mode);

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

            // Converte para o novo formato, processando domínio se necessário
            var (user, password, domain) = request.GetAuthData();
            var loginRequest = new LoginRequest(user, password, domain);

            // Executa autenticação usando o novo sistema de estratégias
            var result = await _authService.AuthenticateAsync(loginRequest, ct);
            var duration = DateTime.UtcNow - startTime;

            if (!result.Success)
            {
                // Log de falha de autenticação
                _logger.LogWarning("Login FALHADO para usuário {Usuario} de IP {IPAddress} em {Duration}ms via {Provider}: {Motivo}",
                    request.CdUsuario, ipAddress, duration.TotalMilliseconds, result.Provider, result.ErrorMessage);

                // Log de segurança para análise de padrões
                _logger.LogInformation("SECURITY_EVENT: LOGIN_FAILED | User: {Usuario} | IP: {IPAddress} | UserAgent: {UserAgent} | Provider: {Provider} | Reason: {Motivo}",
                    request.CdUsuario, ipAddress, userAgent, result.Provider, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            // Sucesso - montar resposta
            var response = new LoginResponseDto(
                result.AccessToken!,
                result.UserData!,
                result.Groups ?? new List<UserGroup>(),
                result.Permissions ?? new List<UserPermission>());

            // Log de sucesso
            _logger.LogInformation("Login SUCESSO para usuário {Usuario} de IP {IPAddress} em {Duration}ms via {Provider}",
                request.CdUsuario, ipAddress, duration.TotalMilliseconds, result.Provider);

            // Log de segurança para auditoria
            _logger.LogInformation("SECURITY_EVENT: LOGIN_SUCCESS | User: {Usuario} | IP: {IPAddress} | UserAgent: {UserAgent} | Provider: {Provider} | Groups: {Groups}",
                request.CdUsuario, ipAddress, userAgent, result.Provider,
                string.Join(",", result.Groups?.Select(g => g.CdGrUser) ?? new string[0]));

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
    /// Login forçando uma estratégia específica (para testes ou casos especiais)
    /// </summary>
    [HttpPost("login/{mode}")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginWithMode(
        [FromRoute] string mode,
        [FromBody] LoginRequestDto request,
        [FromServices] IValidator<LoginRequestDto> validator,
        CancellationToken ct)
    {
        _logger.LogInformation("Login com modo forçado {Mode} para usuário {Usuario}", mode, request.CdUsuario);

        try
        {
            // Validação de entrada
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

                return BadRequest(ApiResponse<object>.Fail("Dados de entrada inválidos", errors));
            }

            // Parse do modo
            if (!Enum.TryParse<AuthMode>(mode, true, out var authMode))
            {
                return BadRequest(ApiResponse<object>.Fail($"Modo de autenticação '{mode}' inválido. Valores aceitos: OnPrem, SaaS, Windows"));
            }

            // Converte para o novo formato, processando domínio se necessário
            var (user, password, domain) = request.GetAuthData();
            var loginRequest = new LoginRequest(user, password, domain);

            // Executa autenticação com modo específico
            var result = await _authService.AuthenticateAsync(loginRequest, authMode, ct);

            if (!result.Success)
            {
                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            var response = new LoginResponseDto(
                result.AccessToken!,
                result.UserData!,
                result.Groups ?? new List<UserGroup>(),
                result.Permissions ?? new List<UserPermission>());

            return Ok(ApiResponse<LoginResponseDto>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante login com modo {Mode} para usuário {Usuario}", mode, request.CdUsuario);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Verifica habilitação com log de auditoria (mantém compatibilidade)
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
            var permissions = await _legacyAuthService.GetUserPermissionsAsync(cdusuario, ct);
            var hasAccess = _legacyAuthService.CheckHabilitacao(cdsistema, cdfuncao, permissions);

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
    /// Endpoint de informações sobre a configuração atual de autenticação
    /// </summary>
    [HttpGet("info")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public ActionResult<ApiResponse<object>> GetAuthInfo()
    {
        var info = new
        {
            CurrentMode = _authOptions.Mode.ToString(),
            AvailableModes = Enum.GetNames<AuthMode>(),
            Configuration = new
            {
                OnPrem = new
                {
                    UserTable = _authOptions.OnPrem.UserTable,
                    AllowPasswordReset = _authOptions.OnPrem.AllowPasswordReset
                },
                SaaS = new
                {
                    RequireEmailConfirmed = _authOptions.SaaS.RequireEmailConfirmed,
                    AllowSelfRegistration = _authOptions.SaaS.AllowSelfRegistration
                },
                Windows = new
                {
                    RequireDomainMembership = _authOptions.Windows.RequireDomainMembership,
                    FallbackToLocal = _authOptions.Windows.FallbackToLocal,
                    DefaultDomain = _authOptions.Windows.DefaultDomain,
                    AllowedDomains = _authOptions.Windows.AllowedDomains,
                    DomainController = _authOptions.Windows.DomainController
                }
            }
        };

        return Ok(ApiResponse<object>.Ok(info, "Informações de autenticação"));
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