using System.Threading;
using System.Threading.Tasks;
using Moq;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;

namespace RhSensoERP.Tests.Unit.Modules.Auth;

/// <summary>
/// Testes unitários do fluxo de autenticação via ILegacyAuthService (mockado).
/// Verifica sucesso e falha sem tocar infraestrutura (DB/JWT reais).
/// </summary>
public class LoginCommandHandlerTests
{
    /// <summary>
    /// Sucesso: AuthenticateAsync retorna token e dados do usuário quando credenciais são válidas.
    /// </summary>
    [Fact]
    public async Task Should_Call_Authenticate_And_Return_Success_When_Credentials_Are_Valid()
    {
        // Arrange
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

        // Act
        var result = await svc.Object.AuthenticateAsync(request.CdUsuario, request.Senha, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().Be("fake-token");
        result.UserData!.CdUsuario.Should().Be("admin");

        svc.Verify(s => s.AuthenticateAsync("admin", "123456", It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Falha: AuthenticateAsync retorna erro quando a senha é inválida.
    /// </summary>
    [Fact]
    public async Task Should_Return_Fail_When_Credentials_Are_Invalid()
    {
        // Arrange
        var svc = new Mock<ILegacyAuthService>();
        svc.Setup(s => s.AuthenticateAsync("admin", "wrong", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new AuthResult(false, "INVALID_CREDENTIALS", null, null));

        // Act
        var result = await svc.Object.AuthenticateAsync("admin", "wrong", CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("INVALID_CREDENTIALS");
    }
}
