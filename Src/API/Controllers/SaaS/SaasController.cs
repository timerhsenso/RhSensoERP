using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.SaaS;

/// <summary>
/// Controller para operações SaaS - autenticação multi-tenant
/// Implementa endpoints para registro, login e renovação de tokens SaaS
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
    /// Autentica usuário SaaS
    /// </summary>
    /// <param name="request">Dados de autenticação (email/senha)</param>
    /// <returns>Token JWT e dados do usuário</returns>
    [HttpPost("authenticate")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<SaasAuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> AuthenticateAsync([FromBody] SaasAuthenticationRequest request)
    {
        var ipAddress = GetClientIpAddress();
        var userAgent = Request.Headers.UserAgent.ToString();

        _logger.LogInformation("Tentativa de autenticação SaaS para {Email} de IP {IPAddress}",
            request.Email, ipAddress);

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
    /// Registra novo usuário SaaS
    /// </summary>
    /// <param name="request">Dados de registro</param>
    /// <returns>Dados do usuário criado</returns>
    [HttpPost("register")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(ApiResponse<SaasRegistrationResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> RegisterAsync([FromBody] SaasRegistrationRequest request)
    {
        var ipAddress = GetClientIpAddress();

        _logger.LogInformation("Tentativa de registro SaaS para {Email} de IP {IPAddress}",
            request.Email, ipAddress);

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
                _logger.LogWarning("Registro SaaS FALHADO para {Email} de IP {IPAddress}: {Erro}",
                    request.Email, ipAddress, result.ErrorMessage);

                return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }

            _logger.LogInformation("Registro SaaS SUCESSO para {Email} de IP {IPAddress}",
                request.Email, ipAddress);

            return CreatedAtAction(
                nameof(AuthenticateAsync),
                ApiResponse<SaasRegistrationResponse>.Ok(result.Value!, "Usuário registrado com sucesso"));
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
    /// <param name="request">Refresh token</param>
    /// <returns>Novo token de acesso</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<SaasAuthenticationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = GetClientIpAddress();

        _logger.LogDebug("Tentativa de refresh token SaaS de IP {IPAddress}", ipAddress);

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
    /// Obtém o endereço IP real do cliente
    /// </summary>
    private string GetClientIpAddress()
    {
        // Verificar headers de proxy primeiro
        var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',')[0].Trim();
        }

        var xRealIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}