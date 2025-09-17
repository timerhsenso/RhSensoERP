using System.Net;
using FluentValidation;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Middlewares;

/// <summary>
/// Middleware para tratamento global de exceções
/// </summary>
/// <remarks>
/// Captura todas as exceções não tratadas e retorna respostas padronizadas em JSON.
/// Trata especificamente ValidationException do FluentValidation e exceções gerais.
/// </remarks>
public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa uma nova instância do middleware de tratamento de exceções
    /// </summary>
    /// <param name="logger">Logger para registrar exceções</param>
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    /// <summary>
    /// Executa o middleware para tratar exceções
    /// </summary>
    /// <param name="context">Contexto HTTP da requisição</param>
    /// <param name="next">Próximo middleware no pipeline</param>
    /// <returns>Task assíncrona</returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Internal server error"));
        }
    }
}