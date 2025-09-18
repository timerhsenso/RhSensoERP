// Tests/RhSensoERP.Tests.Unit/Services/JwtTokenServiceTests.cs
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RhSensoERP.Application.Auth;
using RhSensoERP.Infrastructure.Auth;

namespace RhSensoERP.Tests.Unit.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _jwtService;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenServiceTests()
    {
        _jwtOptions = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "SuperSecretKeyForTestingPurposesOnly123456789",
            AccessTokenMinutes = 30
        };

        var options = Options.Create(_jwtOptions);
        _jwtService = new JwtTokenService(options);
    }

    [Fact]
    public void CreateAccessToken_ShouldGenerateValidToken()
    {
        // Arrange
        var userId = "testuser";
        var tenantId = Guid.NewGuid();
        var permissions = new[] { "read", "write" };

        // Act
        var token = _jwtService.CreateAccessToken(userId, tenantId, permissions);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(token);

        jwt.Subject.Should().Be(userId);
        jwt.Issuer.Should().Be(_jwtOptions.Issuer);
        jwt.Audiences.Should().Contain(_jwtOptions.Audience);
        jwt.Claims.Should().Contain(c => c.Type == "tenant" && c.Value == tenantId.ToString());
        jwt.Claims.Where(c => c.Type == "perm").Should().HaveCount(2);
    }

    [Fact]
    public void CreateAccessToken_ShouldSetCorrectExpiration()
    {
        // Arrange
        var userId = "testuser";
        var tenantId = Guid.NewGuid();
        var permissions = new[] { "read" };

        // Act
        var token = _jwtService.CreateAccessToken(userId, tenantId, permissions);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(token);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        jwt.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromMinutes(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("user123")]
    [InlineData("admin@company.com")]
    public void CreateAccessToken_WithDifferentUserIds_ShouldWork(string userId)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var permissions = new[] { "test" };

        // Act
        var token = _jwtService.CreateAccessToken(userId, tenantId, permissions);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(token);
        jwt.Subject.Should().Be(userId);
    }
}