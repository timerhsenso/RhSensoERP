// Tests/RhSensoERP.Tests.Unit/Core/Entities/UserTests.cs
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Tests.Unit.Core.Entities;

/// <summary>
/// Testes da classe <c>UserTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Core/Entities/UserTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class UserTests
{
    [Fact]
/// <summary>
/// Convenience properties Deve reflect underlying fields.
/// </summary>
    public void Convenience_Properties_Should_Reflect_Underlying_Fields()
    {
        // Arrange
        var u = new User
        {
            CdUsuario = "carlos",
            DcUsuario = "Carlos Eduardo",
            EmailUsuario = "carlos@test.com",
            SenhaUser = "123",
            FlAtivo = 'S'
        };

        // Assert
        u.Username.Should().Be("carlos");
        u.DisplayName.Should().Be("Carlos Eduardo");
        u.Email.Should().Be("carlos@test.com");
        u.PasswordHash.Should().Be("123");
        u.Active.Should().BeTrue();

        u.FlAtivo = 'N';
        u.Active.Should().BeFalse();
    }
}