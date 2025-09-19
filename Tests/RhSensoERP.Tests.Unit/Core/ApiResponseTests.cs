using Xunit;
using FluentAssertions;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.Tests.Unit.Core;

/// <summary>
/// Testes da classe <c>ApiResponseTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Core/ApiResponseTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class ApiResponseTests
{
    [Fact]
/// <summary>
/// Ok withdata shouldcreatesuccessresponse.
/// </summary>
    public void Ok_WithData_ShouldCreateSuccessResponse()
    {
        // Arrange
        var testData = new { Id = 1, Name = "Test" };

        // Act
        var response = ApiResponse<object>.Ok(testData);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be(testData);
        response.Message.Should().BeNull();
        response.ValidationErrors.Should().BeNull();
    }

    [Fact]
/// <summary>
/// Ok withdataandmessage shouldincludeboth.
/// </summary>
    public void Ok_WithDataAndMessage_ShouldIncludeBoth()
    {
        // Arrange
        var testData = "success data";
        var message = "Operation completed";

        // Act
        var response = ApiResponse<string>.Ok(testData, message);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be(testData);
        response.Message.Should().Be(message);
        response.ValidationErrors.Should().BeNull();
    }

    [Fact]
/// <summary>
/// Falha withmessage shouldcreatefailureresponse.
/// </summary>
    public void Fail_WithMessage_ShouldCreateFailureResponse()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var response = ApiResponse<object>.Fail(errorMessage);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Be(errorMessage);
        response.Data.Should().BeNull();
        response.ValidationErrors.Should().BeNull();
    }

    [Fact]
/// <summary>
/// Falha withvalidationerrors shouldincludeerrors.
/// </summary>
    public void Fail_WithValidationErrors_ShouldIncludeErrors()
    {
        // Arrange
        var message = "Validation failed";
        var validationErrors = new
        {
            CdUsuario = new[] { "Campo obrigatório" },
            Senha = new[] { "Muito curta" }
        };

        // Act
        var response = ApiResponse<object>.Fail(message, validationErrors);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Be(message);
        response.Data.Should().BeNull();
        response.ValidationErrors.Should().Be(validationErrors);
    }

    [Fact]
/// <summary>
/// Apiresponse shouldworkwithdifferenttypes.
/// </summary>
    public void ApiResponse_ShouldWorkWithDifferentTypes()
    {
        // Arrange & Act
        var stringResponse = ApiResponse<string>.Ok("test");
        var intResponse = ApiResponse<int>.Ok(42);
        var objectResponse = ApiResponse<object>.Ok(new { Test = true });

        // Assert
        stringResponse.Data.Should().Be("test");
        intResponse.Data.Should().Be(42);
        objectResponse.Data.Should().BeEquivalentTo(new { Test = true });
    }
}