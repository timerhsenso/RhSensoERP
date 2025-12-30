// =============================================================================
// RHSENSOERP API - EXCEPTION HANDLING MIDDLEWARE
// =============================================================================
// Middleware global para tratamento centralizado de exceções
// =============================================================================
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Application.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace RhSensoERP.API.Middleware;

/// <summary>
/// Middleware para tratamento centralizado de exceções.
/// Captura exceções e retorna respostas HTTP padronizadas.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DuplicateEntityException ex)
        {
            _logger.LogWarning(ex,
                "[DUPLICATE-ENTITY] Tentativa de inserir/atualizar registro duplicado | " +
                "Entity: {EntityName} | Property: {PropertyName} | Value: {PropertyValue}",
                ex.EntityName, ex.PropertyName, ex.PropertyValue);

            await HandleDuplicateEntityException(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "[UNAUTHORIZED] Acesso não autorizado");
            await HandleUnauthorizedException(context, ex);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "[BAD-REQUEST] Argumento inválido");
            await HandleBadRequestException(context, ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "[INVALID-OPERATION] Operação inválida");
            await HandleInvalidOperationException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[INTERNAL-ERROR] Erro não tratado");
            await HandleUnknownException(context, ex);
        }
    }

    // =========================================================================
    // 🔴 DUPLICATE ENTITY (409 Conflict)
    // =========================================================================
    private static async Task HandleDuplicateEntityException(
        HttpContext context,
        DuplicateEntityException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Conflict; // 409
        context.Response.ContentType = "application/json";

        var response = new
        {
            value = (object?)null,
            isSuccess = false,
            error = new
            {
                code = "Duplicate.Constraint",
                message = ex.Message,
                propertyName = ex.PropertyName,
                propertyValue = ex.PropertyValue,
                entityName = ex.EntityName,
                type = 2 // ValidationError
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }

    // =========================================================================
    // 🔴 UNAUTHORIZED (401)
    // =========================================================================
    private static async Task HandleUnauthorizedException(
        HttpContext context,
        UnauthorizedAccessException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
        context.Response.ContentType = "application/json";

        var response = new
        {
            value = (object?)null,
            isSuccess = false,
            error = new
            {
                code = "Unauthorized.Access",
                message = ex.Message ?? "Acesso não autorizado.",
                type = 3 // AuthorizationError
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }

    // =========================================================================
    // 🔴 BAD REQUEST (400)
    // =========================================================================
    private static async Task HandleBadRequestException(
        HttpContext context,
        ArgumentException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
        context.Response.ContentType = "application/json";

        var response = new
        {
            value = (object?)null,
            isSuccess = false,
            error = new
            {
                code = "BadRequest.InvalidArgument",
                message = ex.Message,
                parameterName = ex.ParamName,
                type = 2 // ValidationError
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }

    // =========================================================================
    // 🔴 INVALID OPERATION (400)
    // =========================================================================
    private static async Task HandleInvalidOperationException(
        HttpContext context,
        InvalidOperationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
        context.Response.ContentType = "application/json";

        var response = new
        {
            value = (object?)null,
            isSuccess = false,
            error = new
            {
                code = "InvalidOperation",
                message = ex.Message,
                type = 2 // ValidationError
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }

    // =========================================================================
    // 🔴 INTERNAL SERVER ERROR (500)
    // =========================================================================
    private static async Task HandleUnknownException(
        HttpContext context,
        Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
        context.Response.ContentType = "application/json";

        var response = new
        {
            value = (object?)null,
            isSuccess = false,
            error = new
            {
                code = "Internal.ServerError",
                message = "Ocorreu um erro interno no servidor. Tente novamente mais tarde.",
                // ⚠️ NÃO exponha detalhes do erro em produção
#if DEBUG
                details = ex.Message,
                stackTrace = ex.StackTrace
#endif
                ,
                type = 1 // InternalError
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }
}