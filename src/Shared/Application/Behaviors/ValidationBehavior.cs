using FluentValidation;
using MediatR;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Shared.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Any())
        {
            var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));

            // Se o tipo de resposta for Result<T>, retorna falha
            if (typeof(TResponse).IsGenericType)
            {
                var genericType = typeof(TResponse).GetGenericTypeDefinition();
                if (genericType == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var failureMethod = typeof(Result<>)
                        .MakeGenericType(resultType)
                        .GetMethod("Failure", new[] { typeof(string), typeof(string) });

                    if (failureMethod != null)
                    {
                        var result = failureMethod.Invoke(null, new object[] { "VALIDATION_ERROR", errors });
                        return (TResponse)result!;
                    }
                }
            }
        }

        return await next();
    }
}