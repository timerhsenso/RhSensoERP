/**/

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
using RhSensoERP.Infrastructure.Auth;
using Microsoft.Extensions.DependencyInjection; // (necessário para GetRequiredService)

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

    // ✅ ADICIONAR ESTAS 2 LINHAS:
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        IAuthenticationService authService,
        ILegacyAuthService legacyAuthService,
        IOptionsMonitor<AuthOptions> authOptions,
        ILogger<AuthController> logger,

        // ✅ ADICIONAR ESTES 2 PARÂMETROS:
        JwtTokenService jwtTokenService,
        IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _legacyAuthService = legacyAuthService;
        _authOptions = authOptions.CurrentValue;
        _logger = logger;

        // ✅ ADICIONAR ESTAS 2 LINHAS:
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.CurrentValue;
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


            // Gerar Refresh Token
            var accessTokenJti = _jwtTokenService.GetTokenId(result.AccessToken!) ?? Guid.NewGuid().ToString();
            var refreshTokenService = HttpContext.RequestServices.GetRequiredService<IRefreshTokenService>();
            var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(
                request.CdUsuario,
                accessTokenJti,
                ipAddress,
                userAgent,
                ct);

            // Sucesso - montar resposta
            var response = new LoginResponseDto(
                result.AccessToken!,
                result.UserData!,
                result.Groups ?? new List<UserGroup>(),
                result.Permissions ?? new List<UserPermission>(), refreshToken);

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

            // === ADIÇÃO MÍNIMA: gerar refresh token e passar 5º argumento ===
            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();
            var accessTokenJti = _jwtTokenService.GetTokenId(result.AccessToken!) ?? Guid.NewGuid().ToString();
            var refreshTokenService = HttpContext.RequestServices.GetRequiredService<IRefreshTokenService>();
            var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(
                request.CdUsuario,
                accessTokenJti,
                ipAddress,
                userAgent,
                ct);

            var response = new LoginResponseDto(
                result.AccessToken!,
                result.UserData!,
                result.Groups ?? new List<UserGroup>(),
                result.Permissions ?? new List<UserPermission>(),
                refreshToken);

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


    // ✅ ADICIONAR ESTE MÉTODO AO AuthController.cs

    /// <summary>
    /// Renova o Access Token usando um Refresh Token válido
    /// ✅ SEGURANÇA: Implementa rotation de refresh tokens (o antigo é invalidado)
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        [FromServices] IRefreshTokenService refreshTokenService,
        CancellationToken ct)
    {
        var ipAddress = GetClientIpAddress();
        var userAgent = Request.Headers.UserAgent.ToString();

        _logger.LogInformation(
            "SECURITY_EVENT: REFRESH_TOKEN_ATTEMPT | IP: {IP} | UserAgent: {UserAgent}",
            ipAddress, userAgent);

        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(ApiResponse<object>.Fail("Refresh token é obrigatório"));
            }

            // 1. Validar e obter dados do refresh token
            var refreshTokenData = await refreshTokenService.ValidateRefreshTokenAsync(
                request.RefreshToken,
                ct);

            if (refreshTokenData == null || !refreshTokenData.IsValid)
            {
                _logger.LogWarning(
                    "SECURITY_EVENT: REFRESH_TOKEN_INVALID | IP: {IP} | Reason: {Reason}",
                    ipAddress, refreshTokenData?.ErrorMessage ?? "Token inválido");

                return Unauthorized(ApiResponse<object>.Fail(
                    refreshTokenData?.ErrorMessage ?? "Refresh token inválido ou expirado"));
            }

            var userId = refreshTokenData.UserId;

            // 2. Carregar permissões atualizadas do usuário
            var permissions = await _legacyAuthService.GetUserPermissionsAsync(userId, ct);

            // 3. Gerar novo Access Token
            var tenantId = Guid.Parse($"{permissions.UserData?.CdEmpresa ?? 1:D8}-0000-0000-0000-000000000000");
            var permissionClaims = permissions.Permissions
                .Select(p => $"{p.CdSistema.Trim()}.{p.CdFuncao.Trim()}.{p.CdAcoes.Trim()}")
                .Distinct()
                .ToList();

            var newAccessToken = _jwtTokenService.CreateAccessToken(userId, tenantId, permissionClaims);

            // 4. Gerar novo Refresh Token (rotation)
            var newRefreshToken = await refreshTokenService.RotateRefreshTokenAsync(
                request.RefreshToken,
                ipAddress,
                userAgent,
                ct);

            if (newRefreshToken == null)
            {
                _logger.LogError(
                    "SECURITY_EVENT: REFRESH_TOKEN_ROTATION_FAILED | User: {User} | IP: {IP}",
                    userId, ipAddress);

                return StatusCode(500, ApiResponse<object>.Fail("Erro ao renovar token"));
            }

            _logger.LogInformation(
                "SECURITY_EVENT: REFRESH_TOKEN_SUCCESS | User: {User} | IP: {IP}",
                userId, ipAddress);

            var response = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                //ExpiresIn = _authOptions.Jwt.AccessTokenMinutes * 60 // em segundos
                ExpiresIn = _jwtOptions.AccessTokenMinutes * 60 // em segundos

            };

            return Ok(ApiResponse<RefreshTokenResponseDto>.Ok(response, "Token renovado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SECURITY_EVENT: REFRESH_TOKEN_ERROR | IP: {IP}",
                ipAddress);

            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Revoga um Refresh Token específico
    /// ✅ SEGURANÇA: Permite ao usuário invalidar tokens comprometidos
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<object>>> RevokeToken(
        [FromBody] RevokeTokenRequestDto request,
        [FromServices] IRefreshTokenService refreshTokenService,
        CancellationToken ct)
    {
        var userId = User.Identity?.Name;
        var ipAddress = GetClientIpAddress();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var success = await refreshTokenService.RevokeRefreshTokenAsync(
                request.RefreshToken,
                userId,
                null, // sem replacement
                ct);

            if (!success)
            {
                _logger.LogWarning(
                    "SECURITY_EVENT: REVOKE_TOKEN_FAILED | User: {User} | IP: {IP}",
                    userId, ipAddress);

                return BadRequest(ApiResponse<object>.Fail("Token não encontrado ou já revogado"));
            }

            _logger.LogInformation(
                "SECURITY_EVENT: REVOKE_TOKEN_SUCCESS | User: {User} | IP: {IP}",
                userId, ipAddress);

            return Ok(ApiResponse<object>.Ok("Token revogado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao revogar token para usuário {User}", userId);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Revoga TODOS os Refresh Tokens do usuário atual
    /// ✅ SEGURANÇA: Útil quando o usuário detecta atividade suspeita
    /// </summary>
    [HttpPost("revoke-all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<ActionResult<ApiResponse<object>>> RevokeAllTokens(
        [FromServices] IRefreshTokenService refreshTokenService,
        CancellationToken ct)
    {
        var userId = User.Identity?.Name;
        var ipAddress = GetClientIpAddress();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var count = await refreshTokenService.RevokeAllUserTokensAsync(userId, ct);

            _logger.LogWarning(
                "SECURITY_EVENT: REVOKE_ALL_TOKENS | User: {User} | IP: {IP} | TokensRevoked: {Count}",
                userId, ipAddress, count);

            return Ok(ApiResponse<object>.Ok(
                new { tokensRevoked = count },
                $"{count} token(s) revogado(s) com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao revogar todos os tokens do usuário {User}", userId);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

}
