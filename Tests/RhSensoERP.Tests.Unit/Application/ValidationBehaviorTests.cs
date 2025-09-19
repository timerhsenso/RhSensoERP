using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Application.Common.Behaviors;
using Xunit;

namespace RhSensoERP.Tests.Unit.Application;

public class ValidationBehaviorTests
{
    private record TestRequest(string Name) : IRequest<string>;

    private class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private class EchoHandler : IRequestHandler<TestRequest, string>
    {
        public Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
            => Task.FromResult(request.Name);
    }

    [Fact]
    public async Task Should_Run_Validator_Before_Handler()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ValidationBehaviorTests>());
        services.AddValidatorsFromAssemblyContaining<ValidationBehaviorTests>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient<IRequestHandler<TestRequest, string>, EchoHandler>();

        using var sp = services.BuildServiceProvider();
        var mediator = sp.GetRequiredService<IMediator>();

        // ok
        (await mediator.Send(new TestRequest("ok"))).Should().Be("ok");

        // fail
        await FluentActions.Invoking(async () => await mediator.Send(new TestRequest(string.Empty)))
            .Should().ThrowAsync<ValidationException>();
    }
}