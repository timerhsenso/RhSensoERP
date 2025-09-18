namespace RhSensoERP.API.Middlewares;

/// <summary>
/// Middleware que adiciona headers de segurança a todas as respostas
/// Protege contra ataques XSS, clickjacking e outros vetores de ataque
/// </summary>
public class SecurityHeadersMiddleware : IMiddleware
{
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(ILogger<SecurityHeadersMiddleware> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Adiciona headers de segurança a cada resposta HTTP
    /// </summary>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Headers básicos de segurança
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // HSTS apenas para conexões HTTPS
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Append("Strict-Transport-Security",
                "max-age=31536000; includeSubDomains; preload");
        }

        // CSP básico mas seguro
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' cdnjs.cloudflare.com; " +
            "style-src 'self' 'unsafe-inline' cdnjs.cloudflare.com; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' cdnjs.cloudflare.com; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'");

        // Remove headers que revelam informações do servidor
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");

        _logger.LogDebug("Headers de segurança aplicados para {Path}", context.Request.Path);

        await next(context);
    }
}