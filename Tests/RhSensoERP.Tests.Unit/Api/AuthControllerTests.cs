using System.Net;
using System.Net.Http.Json;
using RhSensoERP.Tests.Unit.Common;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.Tests.Unit.Api;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public AuthControllerTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_Should_Return_200_And_Token_When_Credentials_Are_Valid()
    {
        var client = _factory.CreateClient();
        var request = new LoginRequestDto("admin", "123456");

        var resp = await client.PostAsJsonAsync("/api/v1/auth/login", request);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = await resp.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
        envelope!.Success.Should().BeTrue();
        envelope.Data!.AccessToken.Should().NotBeNullOrEmpty();
        envelope.Data!.UserData.CdUsuario.Should().Be("admin");
    }

    [Fact]
    public async Task Login_Should_Return_400_When_Credentials_Are_Invalid()
    {
        var client = _factory.CreateClient();
        var request = new LoginRequestDto("admin", "wrong");

        var resp = await client.PostAsJsonAsync("/api/v1/auth/login", request);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
