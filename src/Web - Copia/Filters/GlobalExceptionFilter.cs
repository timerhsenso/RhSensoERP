// src/Web/Filters/GlobalExceptionFilter.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RhSensoERP.Web.Filters;

/// <summary>
/// Filtro global para tratamento de exceções.
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Erro não tratado: {Message}", context.Exception.Message);

        // Se for uma requisição AJAX, retorna JSON
        if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
            context.HttpContext.Request.ContentType?.Contains("application/json") == true)
        {
            var errorResponse = new
            {
                success = false,
                message = _environment.IsDevelopment()
                    ? context.Exception.Message
                    : "Ocorreu um erro inesperado. Por favor, tente novamente.",
                details = _environment.IsDevelopment()
                    ? context.Exception.StackTrace
                    : null
            };

            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        else
        {
            // Redireciona para a página de erro
            context.Result = new RedirectToActionResult("Error", "Home", null);
        }

        context.ExceptionHandled = true;
    }
}

/// <summary>
/// Filtro para validação de ModelState.
/// </summary>
public class ValidateModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // Se for uma requisição AJAX, retorna JSON com os erros
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                context.HttpContext.Request.ContentType?.Contains("application/json") == true)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                var errorResponse = new
                {
                    success = false,
                    message = "Dados inválidos",
                    errors
                };

                context.Result = new JsonResult(errorResponse)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Não precisa fazer nada aqui
    }
}
