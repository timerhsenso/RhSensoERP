using System.Threading;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;

namespace RhSensoERP.Tests.Unit.Modules.Auth;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Should_Call_Authenticate_And_Return_Success_When_Credentials_Are_Valid()
    {
        // arrange
        var svc = new Mock<ILegacyAuthService>();
        var request = new LoginRequestDto("admin", "123456");

        var fakeUser = new UserSessionData(
            "admin",              // CdUsuario
            "Administrador",      // DcUsuario
            null,                 // NmImpCche
            'A',                  // TpUsuario
            null,                 // NoMatric
            null,                 // CdEmpresa
            null,                 // CdFilial
            1,                    // NoUser
            "admin@local",        // EmailUsuario
            'S',                  // FlAtivo
            Guid.NewGuid(),       // Id
            "ADMIN",              // NormalizedUsername
            null,                 // IdFuncionario
            null                  // FlNaoRecebeEmail
        );

        svc.Setup(s => s.AuthenticateAsync("admin", "123456", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new AuthResult(true, null, fakeUser, "fake-token"));

        // act
        var result = await svc.Object.AuthenticateAsync(request.CdUsuario, request.Senha, CancellationToken.None);

        // assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().Be("fake-token");
        result.UserData!.CdUsuario.Should().Be("admin");

        svc.Verify(s => s.AuthenticateAsync("admin", "123456", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_Fail_When_Credentials_Are_Invalid()
    {
        var svc = new Mock<ILegacyAuthService>();

        svc.Setup(s => s.AuthenticateAsync("admin", "wrong", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new AuthResult(false, "INVALID_CREDENTIALS", null, null));

        var result = await svc.Object.AuthenticateAsync("admin", "wrong", CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("INVALID_CREDENTIALS");
    }
}
