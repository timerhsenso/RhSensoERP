using Xunit;
using FluentAssertions;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Validators;

namespace RhSensoERP.Tests.Unit.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator;

    public LoginRequestValidatorTests()
    {
        _validator = new LoginRequestValidator();
    }

    [Fact]
    public void ValidRequest_ShouldPassValidation()
    {
        // Arrange
        var request = new LoginRequestDto("admin", "senha123");

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Código do usuário é obrigatório")]
    [InlineData(null, "Código do usuário é obrigatório")]
    public void EmptyUsuario_ShouldFail(string usuario, string expectedMessage)
    {
        // Arrange
        var request = new LoginRequestDto(usuario!, "senha123");

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "CdUsuario" &&
            e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData("a")]  // 1 char - mínimo válido
    [InlineData("admin")]  // normal
    [InlineData("user.test_name-123")]  // caracteres especiais permitidos
    [InlineData("123456789012345678901234567890")]  // 30 chars - máximo válido
    public void ValidUsuario_ShouldPass(string usuario)
    {
        // Arrange
        var request = new LoginRequestDto(usuario, "senha123");

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "CdUsuario");
    }

    [Theory]
    [InlineData("1234567890123456789012345678901")]  // 31 chars - muito longo
    [InlineData("user@domain")]  // @ não permitido
    [InlineData("user#test")]  // # não permitido
    [InlineData("user test")]  // espaço não permitido
    [InlineData("user$name")]  // $ não permitido
    public void InvalidUsuario_ShouldFail(string usuario)
    {
        // Arrange
        var request = new LoginRequestDto(usuario, "senha123");

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CdUsuario");
    }

    [Theory]
    [InlineData("", "Senha é obrigatória")]
    [InlineData(null, "Senha é obrigatória")]
    [InlineData("123", "Senha deve ter no mínimo 4 caracteres")]
    public void InvalidSenha_ShouldFailWithCorrectMessage(string senha, string expectedMessage)
    {
        // Arrange
        var request = new LoginRequestDto("admin", senha!);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Senha" &&
            e.ErrorMessage == expectedMessage);
    }

    [Theory]
    [InlineData("1234")]  // 4 chars - mínimo válido
    [InlineData("senhacomplexa123")]  // normal
    [InlineData("12345678901234567890123456789012345678901234567890")]  // 50 chars - máximo válido
    public void ValidSenha_ShouldPass(string senha)
    {
        // Arrange
        var request = new LoginRequestDto("admin", senha);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Senha");
    }

    [Fact]
    public void SenhaTooLong_ShouldFail()
    {
        // Arrange - 51 caracteres
        var senhaMuitoLonga = new string('a', 51);
        var request = new LoginRequestDto("admin", senhaMuitoLonga);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Senha" &&
            e.ErrorMessage == "Senha deve ter no máximo 50 caracteres");
    }

    [Fact]
    public void MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new LoginRequestDto("", "12");  // usuário vazio + senha muito curta

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
        result.Errors.Should().Contain(e => e.PropertyName == "CdUsuario");
        result.Errors.Should().Contain(e => e.PropertyName == "Senha");
    }

    [Fact]
    public void RegexValidation_ShouldWorkCorrectly()
    {
        // Arrange - testa especificamente a regex
        var validChars = "admin.test_name-123";
        var invalidChars = "admin@test.com";

        // Act
        var validResult = _validator.Validate(new LoginRequestDto(validChars, "senha123"));
        var invalidResult = _validator.Validate(new LoginRequestDto(invalidChars, "senha123"));

        // Assert
        validResult.Errors.Should().NotContain(e =>
            e.PropertyName == "CdUsuario" &&
            e.ErrorMessage.Contains("caracteres inválidos"));

        invalidResult.Errors.Should().Contain(e =>
            e.PropertyName == "CdUsuario" &&
            e.ErrorMessage.Contains("caracteres inválidos"));
    }
}