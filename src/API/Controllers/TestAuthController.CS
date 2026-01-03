using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Identity.Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RhSensoERP.API.Controllers;

[ApiController]
[Route("api/test-auth")]
public class TestAuthController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TestAuthController> _logger;

    public TestAuthController(
        IOptions<JwtSettings> jwtSettings,
        ILogger<TestAuthController> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    [HttpPost("generate-test-token")]
    [AllowAnonymous]
    public IActionResult GenerateTestToken()
    {
        try
        {
            _logger.LogInformation("🔐 Gerando token de teste...");
            _logger.LogInformation("   SecretKey Length: {Length}", _jwtSettings.SecretKey.Length);
            _logger.LogInformation("   Issuer: {Issuer}", _jwtSettings.Issuer);
            _logger.LogInformation("   Audience: {Audience}", _jwtSettings.Audience);

            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var symmetricKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, "1"),
                new(JwtRegisteredClaimNames.UniqueName, "test-user"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("userId", "1"),
                new("username", "test-user")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("✅ Token gerado com sucesso!");

            return Ok(new
            {
                token = tokenString,
                expiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
                keyInfo = new
                {
                    length = _jwtSettings.SecretKey.Length,
                    preview = _jwtSettings.SecretKey.Substring(0, 20) + "...",
                    issuer = _jwtSettings.Issuer,
                    audience = _jwtSettings.Audience
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao gerar token");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("validate-test")]
    [Authorize]
    public IActionResult ValidateTest()
    {
        var userId = User.FindFirst("userId")?.Value;
        var username = User.Identity?.Name;

        _logger.LogInformation("✅ Token validado! UserId={UserId}, Username={Username}", userId, username);

        return Ok(new
        {
            success = true,
            message = "Token válido! ✅",
            userId,
            username,
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }
}