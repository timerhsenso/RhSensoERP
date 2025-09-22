using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Common;

namespace RhSensoERP.API.Controllers.SaaS;

[ApiController]
[Route("api/[controller]")]
public class SaasController : ControllerBase
{
    private readonly ISaasUserService _saasUserService;

    public SaasController(ISaasUserService saasUserService)
    {
        _saasUserService = saasUserService ?? throw new ArgumentNullException(nameof(saasUserService));
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] SaasAuthenticationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _saasUserService.AuthenticateAsync(request);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] SaasRegistrationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _saasUserService.RegisterAsync(request);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(AuthenticateAsync), result);

        return BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _saasUserService.RefreshTokenAsync(request);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized(result);
    }
}