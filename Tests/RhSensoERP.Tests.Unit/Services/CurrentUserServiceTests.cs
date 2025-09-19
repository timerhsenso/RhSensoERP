// Tests/RhSensoERP.Tests.Unit/Services/CurrentUserServiceTests.cs
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using RhSensoERP.Infrastructure.Services;

namespace RhSensoERP.Tests.Unit.Services;

/// <summary>
/// Testes da classe <c>CurrentUserServiceTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Services/CurrentUserServiceTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class CurrentUserServiceTests
{
    [Fact]
/// <summary>
/// Userid withauthenticateduser shouldreturncorrectid.
/// </summary>
    public void UserId_WithAuthenticatedUser_ShouldReturnCorrectId()
    {
        // Arrange
        var expectedUserId = "testuser";
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, expectedUserId)
        }, "test");

        context.User = new ClaimsPrincipal(identity);
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var service = new CurrentUserService(mockHttpContextAccessor.Object);

        // Act
        var result = service.UserId;

        // Assert
        result.Should().Be(expectedUserId);
    }

    [Fact]
/// <summary>
/// Tenantid withvalidtenantclaim shouldreturncorrectid.
/// </summary>
    public void TenantId_WithValidTenantClaim_ShouldReturnCorrectId()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        var identity = new ClaimsIdentity(new[]
        {
            new Claim("tenant", expectedTenantId.ToString())
        }, "test");

        context.User = new ClaimsPrincipal(identity);
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var service = new CurrentUserService(mockHttpContextAccessor.Object);

        // Act
        var result = service.TenantId;

        // Assert
        result.Should().Be(expectedTenantId);
    }

    [Fact]
/// <summary>
/// Tenantid withinvalidtenantclaim shouldreturnnull.
/// </summary>
    public void TenantId_WithInvalidTenantClaim_ShouldReturnNull()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        var identity = new ClaimsIdentity(new[]
        {
            new Claim("tenant", "invalid-guid")
        }, "test");

        context.User = new ClaimsPrincipal(identity);
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var service = new CurrentUserService(mockHttpContextAccessor.Object);

        // Act
        var result = service.TenantId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
/// <summary>
/// Userid withnohttpcontext shouldreturnnull.
/// </summary>
    public void UserId_WithNoHttpContext_ShouldReturnNull()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

        var service = new CurrentUserService(mockHttpContextAccessor.Object);

        // Act
        var result = service.UserId;

        // Assert
        result.Should().BeNull();
    }
}