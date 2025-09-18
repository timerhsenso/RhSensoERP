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

    public AuthController(ILegacyAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login no sistema legacy
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("login")] // ADICIONAR ESTA LINHA
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request, 
        CancellationToken ct)
    {
        var result = await _authService.AuthenticateAsync(request.CdUsuario, request.Senha, ct);
        
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));

        var permissions = await _authService.GetUserPermissionsAsync(request.CdUsuario, ct);
        
        var response = new LoginResponse(
            result.AccessToken!,
            result.UserData!,
            permissions.Groups,
            permissions.Permissions);

        return Ok(ApiResponse<LoginResponse>.Ok(response));
    }

    /// <summary>
    /// Verifica habilitação
    /// </summary>
    [HttpGet("check-habilitacao")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> CheckHabilitacao(
        [FromQuery] string cdsistema,
        [FromQuery] string cdfuncao,
        CancellationToken ct)
    {
        var cdusuario = User.Identity?.Name;
        if (string.IsNullOrEmpty(cdusuario))
            return Unauthorized();

        var permissions = await _authService.GetUserPermissionsAsync(cdusuario, ct);
        var hasAccess = _authService.CheckHabilitacao(cdsistema, cdfuncao, permissions);

        return Ok(ApiResponse<bool>.Ok(hasAccess));
    }
}

public record LoginRequest(string CdUsuario, string Senha);
public record LoginResponse(string AccessToken, UserSessionData UserData, List<UserGroup> Groups, List<UserPermission> Permissions);