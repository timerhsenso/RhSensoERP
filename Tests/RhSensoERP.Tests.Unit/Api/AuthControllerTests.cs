using System.Net;
using System.Net.Http.Json;
using RhSensoERP.Tests.Unit.Common;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.Tests.Unit.Api;

/// <summary>
/// Testes de integração do endpoint de autenticação (/api/v1/auth/login).
/// Verifica contrato (status code + envelope + payload) sem depender de token real.
/// </summary>
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public AuthControllerTests(CustomWebApplicationFactory factory) => _factory = factory;

    /// <summary>
    /// Cenário feliz: credenciais corretas retornam 200 + ApiResponse com AccessToken e UserData.
    /// </summary>
    [Fact]
    public async Task Login_Should_Return_200_And_Token_When_Credentials_Are_Valid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequestDto("admin", "123456");

        // Act
        var resp = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
        envelope!.Success.Should().BeTrue("credenciais válidas devem retornar sucesso");
        envelope.Data!.AccessToken.Should().NotBeNullOrEmpty("o token é gerado pelo fluxo de autenticação");
        envelope.Data!.UserData.CdUsuario.Should().Be("admin");
    }

    /// <summary>
    /// Cenário de erro: credenciais inválidas retornam 400.
    /// </summary>
    [Fact]
    public async Task Login_Should_Return_400_When_Credentials_Are_Invalid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new LoginRequestDto("admin", "wrong");

        // Act
        var resp = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
