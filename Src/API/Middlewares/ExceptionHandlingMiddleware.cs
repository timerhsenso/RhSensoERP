using System.Net;
using FluentValidation;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Middlewares;

/// <summary>
/// Middleware para tratamento global de exce��es
/// </summary>
/// <remarks>
/// Captura todas as exce��es n�o tratadas e retorna respostas padronizadas em JSON.
/// Trata especificamente ValidationException do FluentValidation e exce��es gerais.
/// </remarks>
public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa uma nova inst�ncia do middleware de tratamento de exce��es
    /// </summary>
    /// <param name="logger">Logger para registrar exce��es</param>
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    /// <summary>
    /// Executa o middleware para tratar exce��es
    /// </summary>
    /// <param name="context">Contexto HTTP da requisi��o</param>
    /// <param name="next">Pr�ximo middleware no pipeline</param>
    /// <returns>Task ass�ncrona</returns>
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