// Tests/RhSensoERP.Tests.Unit/Middlewares/ExceptionHandlingMiddlewareTests.cs
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RhSensoERP.API.Middlewares;
using RhSensoERP.Core.Shared.Errors; // ajuste se o DomainException estiver em outro namespace

namespace RhSensoERP.Tests.Unit.Middlewares;

/// <summary>
/// Testes da classe <c>ExceptionHandlingMiddlewareTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Middlewares/ExceptionHandlingMiddlewareTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class ExceptionHandlingMiddlewareTests
{
    private static DefaultHttpContext NewContext()
    {
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();
        return ctx;
    }

    private static JsonDocument ReadBody(DefaultHttpContext ctx)
    {
        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(ctx.Response.Body);
        var json = sr.ReadToEnd();
        return JsonDocument.Parse(json);
    }

    [Fact]
/// <summary>
/// Deve tratar DomainException como erro interno do servidor (500).
/// </summary>
/// <remarks>
/// Resultado esperado: status HTTP InternalServerError.
/// </remarks>
    public async Task Should_Handle_DomainException_As_InternalServerError()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ExceptionHandlingMiddleware>>();
        var mw = new ExceptionHandlingMiddleware(logger);
        var ctx = NewContext();

        // Act
        await mw.InvokeAsync(ctx, _ => throw new DomainException("Erro de negócio"));

        // Assert
        ctx.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        using var body = ReadBody(ctx);
        var root = body.RootElement;
        root.TryGetProperty("success", out var success).Should().BeTrue();
        root.TryGetProperty("message", out var message).Should().BeTrue();

        success.GetBoolean().Should().BeFalse();
        message.GetString().Should().NotBeNullOrWhiteSpace();
        // se o middleware propaga a mensagem, isto também passa:
        // message.GetString().Should().Contain("Erro de negócio");
    }

    [Fact]
/// <summary>
/// Deve tratar genérica exceção como erro interno do servidor (500).
/// </summary>
/// <remarks>
/// Resultado esperado: status HTTP InternalServerError.
/// </remarks>
    public async Task Should_Handle_Generic_Exception_As_InternalServerError()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ExceptionHandlingMiddleware>>();
        var mw = new ExceptionHandlingMiddleware(logger);
        var ctx = NewContext();

        // Act
        await mw.InvokeAsync(ctx, _ => throw new Exception("boom"));

        // Assert
        ctx.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        using var body = ReadBody(ctx);
        var root = body.RootElement;
        root.TryGetProperty("success", out var success).Should().BeTrue();
        success.GetBoolean().Should().BeFalse();
        root.TryGetProperty("message", out var message).Should().BeTrue();
        message.GetString().Should().NotBeNullOrWhiteSpace();
    }
}