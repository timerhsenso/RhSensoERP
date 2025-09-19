using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Xunit;
using FluentAssertions;

namespace RhSensoERP.Tests.Unit.Application
{
    // ajuste o using abaixo se o ValidationBehavior estiver em outro namespace
    using RhSensoERP.Application.Common.Behaviors;

    /// <summary>
/// Testes da classe <c>ValidationBehaviorTests</c>.
/// Este arquivo documenta o objetivo de cada teste e o resultado esperado, sem alterar a lógica.
/// </summary>
/// <remarks>
/// Local: Tests/RhSensoERP.Tests.Unit/Application/ValidationBehaviorTests.cs
/// Diretrizes: nome claro de teste; Arrange-Act-Assert explícito; asserts específicos.
/// </remarks>
public class ValidationBehaviorTests
    {
        private class DummyRequest : IRequest<string> { public string? Name { get; set; } }

        private class FailingValidator : AbstractValidator<DummyRequest>
        {
            public bool WasCalled { get; private set; }

            public FailingValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Name is required");
            }

            // O ValidationBehavior usa ValidateAsync → marcamos aqui
            public override Task<ValidationResult> ValidateAsync(
                ValidationContext<DummyRequest> context,
                CancellationToken cancellation = default)
            {
                WasCalled = true;
                return base.ValidateAsync(context, cancellation);
            }
        }

        [Fact]
/// <summary>
/// Deve executar validador e lançar quando inválido.
/// </summary>
/// <remarks>
/// Resultado esperado: lançar ValidationException.
/// </remarks>
        public async Task Should_Run_Validator_And_Throw_When_Invalid()
        {
            // Arrange
            var validator = new FailingValidator();
            var validators = new[] { (IValidator<DummyRequest>)validator };

            var nextCalled = false;
            RequestHandlerDelegate<string> next = () =>
            {
                nextCalled = true;
                return Task.FromResult("OK");
            };

            var behavior = new ValidationBehavior<DummyRequest, string>(validators);

            // Act
            var act = () => behavior.Handle(new DummyRequest { Name = null }, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Name is required*");

            validator.WasCalled.Should().BeTrue("o ValidationBehavior deve executar os validadores");
            nextCalled.Should().BeFalse("requisição inválida não deve atingir o handler");
        }
    }
}