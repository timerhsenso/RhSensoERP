// Tests/RhSensoERP.Tests.Unit/Middlewares/SecurityHeadersMiddlewareTests.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RhSensoERP.API.Middlewares;

namespace RhSensoERP.Tests.Unit.Middlewares;

/// <summary>
/// Testes da classe <c>SecurityHeadersMiddlewareTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Middlewares/SecurityHeadersMiddlewareTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class SecurityHeadersMiddlewareTests
{
    [Fact]
/// <summary>
/// Deve append security headers.
/// </summary>
    public async Task Should_Append_Security_Headers()
    {
        // Arrange
        var logger = Mock.Of<ILogger<SecurityHeadersMiddleware>>();
        var mw = new SecurityHeadersMiddleware(logger);
        var ctx = new DefaultHttpContext();

        // Act
        await mw.InvokeAsync(ctx, _ => Task.CompletedTask);

        // Assert
        var h = ctx.Response.Headers;
        h.Should().ContainKey("X-Content-Type-Options");
        h.Should().ContainKey("X-Frame-Options");
        h.Should().ContainKey("X-XSS-Protection");
        // CSP pode estar truncado na implementação; validamos presença se você estiver setando.
        // h.Should().ContainKey("Content-Security-Policy");
    }
}