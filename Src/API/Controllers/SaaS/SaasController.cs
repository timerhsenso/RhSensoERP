using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.SaaS;

/// <summary>
/// Controller para operações SaaS - autenticação multi-tenant.
/// Implementa endpoints para registro, login, renovação de tokens,
/// confirmação e reenvio de confirmação de e-mail.
/// Segue Clean Architecture e padrões REST com documentação OpenAPI.
/// </summary>
[ApiController]
[Route("api/v1/saas")]
[Produces("application/json")]
public class SaasController : ControllerBase
{
    private readonly ISaasUserService _saasUserService;
    private readonly ILogger<SaasController> _logger;

    /// <summary>
    /// Construtor do controller SaaS com injeção de dependências.
    /// </summary>
    /// <param name="saasUserService">Serviço de negócio para operações de usuários SaaS.</param>
    /// <param name="logger">Logger estruturado para auditoria e debugging.</param>
    public SaasController(
        ISaasUserService saasUserService,
        ILogger<SaasController> logger)
    {
        _saasUserService = saasUserService ?? throw new ArgumentNullException(nameof(saasUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // =========================================================================
    //  AUTENTICAÇÃO
    // =========================================================================

    /// <summary>
    /// Autentica usuário no sistema SaaS.
    /// </summary>
    /// <param name="request">Dados de autenticação contendo e-mail e senha.</param>
    /// <returns>Token JWT, refresh token e dados do usuário autenticado.</returns>
    /// <remarks>
    /// Exemplo:
    /// POST /api/v1/saas/authenticate
    /// {
    ///   "email": "usuario@exemplo.com",
    ///   "password": "MinhaSenh@123"
    /// }
    /// - Requer e-mail confirmado.
    /// - Incrementa tentativas e pode bloquear após 5 falhas.
    /// </remarks>
    /// <response code="200">Autenticação realizada com sucesso.</response>
    /// <response code="400">Dados de entrada inválidos.</response>
    /// <response code="401">Credenciais inválidas ou conta bloqueada.</response>
    /// <response code="500">Erro interno do servidor.</response>
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

        _logger.LogInformation("Tentativa de autenticação SaaS para {Email} de IP {IP} (UA: {UA})",
            request?.Email, ipAddress, userAgent);

        try
        {
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
                _logger.LogWarning("Autenticação FALHOU para {Email} de IP {IP}: {Erro}",
                    request.Email, ipAddress, result.ErrorMessage);

                return Unauthorized(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Autenticação OK para {Email} de IP {IP}", request.Email, ipAddress);

            return Ok(ApiResponse<SaasAuthenticationResponse>.Ok(result.Value!, "Autenticação realizada com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante autenticação SaaS para {Email}", request?.Email);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    // =========================================================================
    //  REGISTRO
    // =========================================================================

    /// <summary>
    /// Registra um novo usuário no sistema SaaS.
    /// </summary>
    /// <param name="request">Nome, e-mail, senha e tenantId.</param>
    /// <returns>Dados do usuário criado e instruções para confirmação de e-mail.</returns>
    /// <remarks>
    /// Exemplo:
    /// POST /api/v1/saas/register
    /// {
    ///   "name": "João Silva",
    ///   "email": "joao@exemplo.com",
    ///   "password": "MinhaSenh@123",
    ///   "tenantId": "550e8400-e29b-41d4-a716-446655440000"
    /// }
    /// - Cria usuário com e-mail não confirmado.
    /// - Envia e-mail de confirmação automaticamente.
    /// </remarks>
    /// <response code="201">Usuário registrado com sucesso.</response>
    /// <response code="400">Dados inválidos ou e-mail já em uso.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost("register")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<SaasRegistrationResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RegisterAsync([FromBody] SaasRegistrationRequest request)
    {
        var ipAddress = GetClientIpAddress();
        _logger.LogInformation("Tentativa de registro SaaS para {Email} de IP {IP}", request?.Email, ipAddress);

        try
        {
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
                _logger.LogWarning("Registro FALHOU para {Email} de IP {IP}: {Erro}",
                    request.Email, ipAddress, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Registro OK para {Email} de IP {IP}", request.Email, ipAddress);

            // Created sem rota para evitar problemas de resolução (padrão simples)
            return StatusCode(201, ApiResponse<SaasRegistrationResponse>.Ok(result.Value!, "Usuário registrado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante registro SaaS para {Email}", request?.Email);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    // =========================================================================
    //  REFRESH TOKEN
    // =========================================================================

    /// <summary>
    /// Renova token de acesso usando um refresh token válido.
    /// </summary>
    /// <param name="request">Refresh token.</param>
    /// <returns>Novo token de acesso e refresh token.</returns>
    /// <remarks>
    /// Exemplo:
    /// POST /api/v1/saas/refresh-token
    /// {
    ///   "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    /// }
    /// - Refresh tokens tipicamente expiram em 7 dias.
    /// - O token antigo é invalidado ao renovar.
    /// </remarks>
    /// <response code="200">Token renovado com sucesso.</response>
    /// <response code="400">Dados de entrada inválidos.</response>
    /// <response code="401">Refresh token inválido ou expirado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<SaasAuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = GetClientIpAddress();
        _logger.LogDebug("Tentativa de refresh token SaaS de IP {IP}", ipAddress);

        try
        {
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
                _logger.LogWarning("Refresh token FALHOU de IP {IP}: {Erro}", ipAddress, result.ErrorMessage);
                return Unauthorized(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Refresh token OK de IP {IP}", ipAddress);

            return Ok(ApiResponse<SaasAuthenticationResponse>.Ok(result.Value!, "Token renovado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante refresh token SaaS de IP {IP}", ipAddress);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    // =========================================================================
    //  CONFIRMAÇÃO DE E-MAIL
    // =========================================================================

    /// <summary>
    /// Confirma o e-mail do usuário usando token enviado por e-mail.
    /// </summary>
    /// <param name="request">E-mail e token de confirmação.</param>
    /// <returns>Status da confirmação.</returns>
    /// <remarks>
    /// Exemplo:
    /// POST /api/v1/saas/confirm-email
    /// {
    ///   "email": "usuario@exemplo.com",
    ///   "token": "abc123def456789..."
    /// }
    /// - O token é criado no registro e enviado por e-mail.
    /// - Após confirmar, o usuário pode autenticar normalmente.
    /// </remarks>
    /// <response code="200">E-mail confirmado com sucesso.</response>
    /// <response code="400">Token inválido, dados inválidos ou e-mail já confirmado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(ApiResponse<ConfirmEmailResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request)
    {
        try
        {
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

            var result = await _saasUserService.ConfirmEmailAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Confirmação de e-mail FALHOU para {Email}: {Erro}",
                    request.Email, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("E-mail confirmado para {Email}", request.Email);

            return Ok(ApiResponse<ConfirmEmailResponse>.Ok(result.Value!, "E-mail confirmado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante confirmação de e-mail para {Email}", request?.Email);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Reenvia o e-mail de confirmação para usuários com e-mail não confirmado.
    /// </summary>
    /// <param name="request">E-mail do usuário.</param>
    /// <returns>Status do reenvio.</returns>
    /// <remarks>
    /// Exemplo:
    /// POST /api/v1/saas/resend-confirmation
    /// {
    ///   "email": "usuario@exemplo.com"
    /// }
    /// - Gera um novo token de confirmação por segurança antes de reenviar.
    /// - Apenas para usuários sem e-mail confirmado.
    /// </remarks>
    /// <response code="200">E-mail de confirmação reenviado com sucesso.</response>
    /// <response code="400">E-mail já confirmado ou dados inválidos.</response>
    /// <response code="404">Usuário não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost("resend-confirmation")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> ResendConfirmationEmailAsync([FromBody] ResendConfirmationRequest request)
    {
        try
        {
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

            var result = await _saasUserService.ResendConfirmationEmailAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Reenvio de confirmação FALHOU para {Email}: {Erro}",
                    request.Email, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("E-mail de confirmação REENVIADO para {Email}", request.Email);

            return Ok(ApiResponse<object>.Ok(null, result.Message!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante reenvio de confirmação para {Email}", request?.Email);
            return StatusCode(500, ApiResponse<object>.Fail("Erro interno do servidor"));
        }
    }

    // =========================================================================
    //  UTILITÁRIOS
    // =========================================================================

    /// <summary>
    /// Obtém o endereço IP real do cliente considerando proxies e load balancers.
    /// </summary>
    /// <returns>Endereço IP do cliente ou "unknown" se não puder determinar.</returns>
    /// <remarks>
    /// Ordem de verificação:
    /// 1) X-Forwarded-For  2) X-Real-IP  3) RemoteIpAddress
    /// </remarks>
    private string GetClientIpAddress()
    {
        // X-Forwarded-For: pode conter lista de IPs (cliente, proxies intermediários...)
        var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(xForwardedFor))
        {
            return xForwardedFor.Split(',')[0].Trim();
        }

        // X-Real-IP (ex.: nginx)
        var xRealIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(xRealIp))
        {
            return xRealIp;
        }

        // Fallback
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
