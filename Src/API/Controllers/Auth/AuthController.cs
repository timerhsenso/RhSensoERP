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
[ApiExplorerSettings(GroupName = "SEG")]

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

    /// <summary>
    /// Lista todas as permissões/habilitações do usuário logado
    /// </summary>
    [HttpGet("my-permissions")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserPermissions>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<UserPermissions>>> GetMyPermissions(
        CancellationToken ct)
    {
        var cdusuario = User.Identity?.Name;
        var ipAddress = GetClientIpAddress();

        if (string.IsNullOrEmpty(cdusuario))
        {
            _logger.LogWarning("Tentativa de listar permissões sem usuário autenticado de IP {IPAddress}",
                ipAddress);
            return Unauthorized();
        }

        try
        {
            var permissions = await _legacyAuthService.GetUserPermissionsAsync(cdusuario, ct);

            _logger.LogInformation(
                "SECURITY_EVENT: PERMISSIONS_LIST | User: {Usuario} | IP: {IPAddress} | TotalPermissions: {Total} | TotalGroups: {Groups}",
                cdusuario, ipAddress, permissions.Permissions.Count, permissions.Groups.Count);

            return Ok(ApiResponse<UserPermissions>.Ok(
                permissions,
                $"{permissions.Permissions.Count} permissão(ões) em {permissions.Groups.Count} grupo(s)"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao listar permissões para usuário {Usuario} de IP {IPAddress}",
                cdusuario, ipAddress);

            return StatusCode(500, ApiResponse<object>.Fail("Erro ao buscar permissões"));
        }
    }

    /// <summary>
    /// Lista permissões agrupadas por sistema
    /// </summary>
    [HttpGet("my-permissions/by-system")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, List<UserPermission>>>), 200)]
    public async Task<ActionResult<ApiResponse<Dictionary<string, List<UserPermission>>>>> GetMyPermissionsBySystem(
        CancellationToken ct)
    {
        var cdusuario = User.Identity?.Name;

        if (string.IsNullOrEmpty(cdusuario))
        {
            return Unauthorized();
        }

        try
        {
            var permissions = await _legacyAuthService.GetUserPermissionsAsync(cdusuario, ct);

            var groupedPermissions = permissions.Permissions
                .GroupBy(p => p.CdSistema)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            _logger.LogInformation(
                "Permissões agrupadas por sistema para usuário {Usuario}: {TotalSystems} sistema(s)",
                cdusuario, groupedPermissions.Count);

            return Ok(ApiResponse<Dictionary<string, List<UserPermission>>>.Ok(
                groupedPermissions,
                $"Permissões organizadas em {groupedPermissions.Count} sistema(s)"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar permissões agrupadas para usuário {Usuario}", cdusuario);
            return StatusCode(500, ApiResponse<object>.Fail("Erro ao buscar permissões"));
        }
    }

    /// <summary>
    /// Lista apenas os grupos do usuário
    /// </summary>
    [HttpGet("my-groups")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<UserGroup>>), 200)]
    public async Task<ActionResult<ApiResponse<List<UserGroup>>>> GetMyGroups(
        CancellationToken ct)
    {
        var cdusuario = User.Identity?.Name;

        if (string.IsNullOrEmpty(cdusuario))
        {
            return Unauthorized();
        }

        try
        {
            var permissions = await _legacyAuthService.GetUserPermissionsAsync(cdusuario, ct);

            _logger.LogInformation(
                "Grupos listados para usuário {Usuario}: {TotalGroups} grupo(s)",
                cdusuario, permissions.Groups.Count);

            return Ok(ApiResponse<List<UserGroup>>.Ok(
                permissions.Groups,
                $"{permissions.Groups.Count} grupo(s) encontrado(s)"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar grupos para usuário {Usuario}", cdusuario);
            return StatusCode(500, ApiResponse<object>.Fail("Erro ao buscar grupos"));
        }
    }

    /// <summary>
    /// Verifica se o usuário possui uma ação específica em uma função
    /// Ações: I=Incluir, A=Alterar, E=Excluir, C=Consultar
    /// </summary>
    [HttpGet("my-permissions/check-action")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<ActionResult<ApiResponse<object>>> CheckMyAction(
        [FromQuery] string cdsistema,
        [FromQuery] string cdfuncao,
        [FromQuery] char acao,
        CancellationToken ct)
    {
        var cdusuario = User.Identity?.Name;

        if (string.IsNullOrEmpty(cdusuario))
        {
            return Unauthorized();
        }

        if (!new[] { 'I', 'A', 'E', 'C' }.Contains(char.ToUpper(acao)))
        {
            return BadRequest(ApiResponse<object>.Fail("Ação inválida. Use: I (Incluir), A (Alterar), E (Excluir) ou C (Consultar)"));
        }

        try
        {
            var permissions = await _legacyAuthService.GetUserPermissionsAsync(cdusuario, ct);
            var hasPermission = _legacyAuthService.CheckBotao(cdsistema, cdfuncao, acao, permissions);
            var restricao = _legacyAuthService.CheckRestricao(cdsistema, cdfuncao, permissions);

            var result = new
            {
                HasPermission = hasPermission,
                Action = acao,
                Sistema = cdsistema,
                Funcao = cdfuncao,
                Restricao = restricao,
                RestricaoDescription = restricao == 'S' ? "Sem restrição (acesso total)" : "Normal (restrição padrão)"
            };

            _logger.LogInformation(
                "SECURITY_EVENT: ACTION_CHECK | User: {Usuario} | Sistema: {Sistema} | Funcao: {Funcao} | Acao: {Acao} | Resultado: {Resultado}",
                cdusuario, cdsistema, cdfuncao, acao, hasPermission ? "PERMITIDO" : "NEGADO");

            return Ok(ApiResponse<object>.Ok(result, hasPermission ? "Ação permitida" : "Ação negada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar ação para usuário {Usuario}", cdusuario);
            return StatusCode(500, ApiResponse<object>.Fail("Erro ao verificar ação"));
        }
    }

    /// <summary>
    /// [DEBUG] Verifica as claims do token atual
    /// </summary>
    [HttpGet("debug-token")]
    [Authorize]
    public ActionResult<object> DebugToken()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated,
            Username = User.Identity?.Name,
            AuthenticationType = User.Identity?.AuthenticationType,
            NameClaimType = User.Identity is System.Security.Claims.ClaimsIdentity ci ? ci.NameClaimType : null,
            AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }
}