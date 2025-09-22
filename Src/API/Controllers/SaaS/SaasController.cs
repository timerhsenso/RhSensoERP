using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.SaaS;

/// <summary>
/// Controller para operações SaaS - autenticação multi-tenant
/// Implementa endpoints para registro, login e renovação de tokens SaaS
/// Segue os princípios de Clean Architecture e padrões REST
/// </summary>
[ApiController]
[Route("api/v1/saas")]
[Produces("application/json")]
public class SaasController : ControllerBase
{
    private readonly ISaasUserService _saasUserService;
    private readonly ILogger<SaasController> _logger;

    public SaasController(
        ISaasUserService saasUserService,
        ILogger<SaasController> logger)
    {
        _saasUserService = saasUserService ?? throw new ArgumentNullException(nameof(saasUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Autentica usuário no sistema SaaS
    /// </summary>
    /// <param name="request">Dados de autenticação contendo email e senha</param>
    /// <returns>Token JWT, refresh token e dados do usuário autenticado</returns>
    /// <remarks>
    /// Exemplo de uso:
    /// 
    ///     POST /api/v1/saas/authenticate
    ///     {
    ///         "email": "usuario@exemplo.com",
    ///         "password": "MinhaSenh@123"
    ///     }
    ///     
    /// Retorna HTTP 200 com tokens em caso de sucesso.
    /// Incrementa tentativas de login e pode bloquear conta após 5 tentativas falhas.
    /// </remarks>
    /// <response code="200">Autenticação realizada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Credenciais inválidas ou conta bloqueada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("authenticate")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<SaasAuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> AuthenticateAsync([FromBody] SaasAuthenticationRequest request)
    {
        var ipAddress = GetClientIpAddress();
        var userAgent = Request.Headers.UserAgent.ToString();

        _logger.LogInformation("Tentativa de autenticação SaaS para {Email} de IP {IPAddress}",
            request.Email, ipAddress);

        try
        {
            // Validação do ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.Fail("Dados de entrada inválidos", errors));
            }

            var result = await _saasUserService.AuthenticateAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Autenticação SaaS FALHADA para {Email} de IP {IPAddress}: {Erro}",
                    request.Email, ipAddress, result.ErrorMessage);

                return Unauthorized(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Autenticação SaaS SUCESSO para {Email} de IP {IPAddress}",
                request.Email, ipAddress);

            return Ok(ApiResponse<SaasAuthenticationResponse>.Ok(result.Value!, "Autenticação realizada com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante autenticação SaaS para {Email}", request.Email);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Registra novo usuário no sistema SaaS
    /// </summary>
    /// <param name="request">Dados de registro contendo nome completo, email e senha</param>
    /// <returns>Dados do usuário criado e instruções para confirmação de email</returns>
    /// <remarks>
    /// Exemplo de uso:
    /// 
    ///     POST /api/v1/saas/register
    ///     {
    ///         "fullName": "João Silva",
    ///         "email": "joao@exemplo.com",
    ///         "password": "MinhaSenh@123"
    ///     }
    ///     
    /// Cria usuário com email não confirmado por padrão.
    /// Associa automaticamente ao primeiro tenant ativo ou cria um tenant padrão.
    /// Senha deve ter no mínimo 6 caracteres.
    /// </remarks>
    /// <response code="201">Usuário registrado com sucesso</response>
    /// <response code="400">Dados inválidos ou email já em uso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("register")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<SaasRegistrationResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RegisterAsync([FromBody] SaasRegistrationRequest request)
    {
        var ipAddress = GetClientIpAddress();

        _logger.LogInformation("Tentativa de registro SaaS para {Email} de IP {IPAddress}",
            request.Email, ipAddress);

        try
        {
            // Validação do ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.Fail("Dados de entrada inválidos", errors));
            }

            var result = await _saasUserService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Registro SaaS FALHADO para {Email} de IP {IPAddress}: {Erro}",
                    request.Email, ipAddress, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Registro SaaS SUCESSO para {Email} de IP {IPAddress}",
                request.Email, ipAddress);

            // CORREÇÃO: Usar Created simples em vez de CreatedAtAction
            // Isso evita o erro "No route matches the supplied values"
            return Created("", ApiResponse<SaasRegistrationResponse>.Ok(result.Value!, "Usuário registrado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante registro SaaS para {Email}", request.Email);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Renova token de acesso usando refresh token
    /// </summary>
    /// <param name="request">Refresh token válido</param>
    /// <returns>Novo token de acesso e refresh token</returns>
    /// <remarks>
    /// Exemplo de uso:
    /// 
    ///     POST /api/v1/saas/refresh-token
    ///     {
    ///         "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ///     }
    ///     
    /// Permite renovar tokens sem necessidade de nova autenticação.
    /// Refresh tokens têm validade maior que access tokens.
    /// Token antigo é invalidado após renovação por segurança.
    /// </remarks>
    /// <response code="200">Token renovado com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Refresh token inválido ou expirado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<SaasAuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = GetClientIpAddress();

        _logger.LogDebug("Tentativa de refresh token SaaS de IP {IPAddress}", ipAddress);

        try
        {
            // Validação do ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.Fail("Dados de entrada inválidos", errors));
            }

            var result = await _saasUserService.RefreshTokenAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Refresh token SaaS FALHADO de IP {IPAddress}: {Erro}",
                    ipAddress, result.ErrorMessage);

                return Unauthorized(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Refresh token SaaS SUCESSO de IP {IPAddress}", ipAddress);

            return Ok(ApiResponse<SaasAuthenticationResponse>.Ok(result.Value!, "Token renovado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante refresh token SaaS de IP {IPAddress}", ipAddress);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém o endereço IP real do cliente considerando proxies e load balancers
    /// </summary>
    /// <returns>Endereço IP do cliente ou "unknown" se não puder determinar</returns>
    /// <remarks>
    /// Verifica headers de proxy na seguinte ordem de prioridade:
    /// 1. X-Forwarded-For (padrão para proxies reversos)
    /// 2. X-Real-IP (usado pelo nginx)
    /// 3. Connection.RemoteIpAddress (conexão direta)
    /// 
    /// Útil para logs de auditoria e controle de rate limiting por IP.
    /// </remarks>
    private string GetClientIpAddress()
    {
        // Verificar X-Forwarded-For primeiro (padrão para load balancers/proxies)
        var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            // Pega o primeiro IP da lista (cliente original)
            return xForwardedFor.Split(',')[0].Trim();
        }

        // Verificar X-Real-IP (usado pelo nginx)
        var xRealIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        // Fallback para IP da conexão direta
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}