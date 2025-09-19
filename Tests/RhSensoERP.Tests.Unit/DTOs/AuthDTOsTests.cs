// Tests/RhSensoERP.Tests.Unit/DTOs/AuthDTOsTests.cs
using Xunit;
using FluentAssertions;
using RhSensoERP.Application.Security.Auth.DTOs;

namespace RhSensoERP.Tests.Unit.DTOs;

/// <summary>
/// Testes da classe <c>AuthDTOsTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/DTOs/AuthDTOsTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class AuthDTOsTests
{
    [Fact]
/// <summary>
/// Loginrequestdto shouldcreatecorrectly.
/// </summary>
    public void LoginRequestDto_ShouldCreateCorrectly()
    {
        // Arrange & Act
        var dto = new LoginRequestDto("admin", "password");

        // Assert
        dto.CdUsuario.Should().Be("admin");
        dto.Senha.Should().Be("password");
    }

    [Fact]
/// <summary>
/// Authresult sucesso shouldhavecorrectproperties.
/// </summary>
    public void AuthResult_Success_ShouldHaveCorrectProperties()
    {
        // Arrange
        var userData = new UserSessionData("user", "User Name", null, 'U', null, 1, 1, 1, "user@test.com", 'S', Guid.NewGuid(), "USER", null, null);

        // Act
        var result = new AuthResult(true, null, userData, "token123");

        // Assert
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.UserData.Should().Be(userData);
        result.AccessToken.Should().Be("token123");
    }

    [Fact]
/// <summary>
/// Authresult failure shouldhavecorrectproperties.
/// </summary>
    public void AuthResult_Failure_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var result = new AuthResult(false, "Invalid credentials", null, null);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid credentials");
        result.UserData.Should().BeNull();
        result.AccessToken.Should().BeNull();
    }

    [Fact]
/// <summary>
/// Usersessiondata shouldcreatecorrectly.
/// </summary>
    public void UserSessionData_ShouldCreateCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var funcionarioId = Guid.NewGuid();

        // Act
        var data = new UserSessionData(
            "admin", "Administrator", "PRINTER01", 'A',
            "12345", 1, 2, 100, "admin@test.com", 'S',
            id, "ADMIN", funcionarioId, 'N');

        // Assert
        data.CdUsuario.Should().Be("admin");
        data.DcUsuario.Should().Be("Administrator");
        data.TpUsuario.Should().Be('A');
        data.CdEmpresa.Should().Be(1);
        data.CdFilial.Should().Be(2);
        data.Id.Should().Be(id);
        data.IdFuncionario.Should().Be(funcionarioId);
    }
}