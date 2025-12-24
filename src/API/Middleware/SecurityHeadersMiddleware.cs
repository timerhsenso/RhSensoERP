// src/API/Middleware/SecurityHeadersMiddleware.cs

/// <summary>
/// Middleware para adicionar headers de segurança HTTP.
/// ✅ FASE 4: Aprimorado com CSP mais restritiva e headers adicionais
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(
        RequestDelegate next,
        IWebHostEnvironment environment,
        ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _environment = environment;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ====================================================================
        // ✅ FASE 4: CONTENT SECURITY POLICY (CSP) APRIMORADA
        // ====================================================================
        // CSP previne ataques XSS ao controlar quais recursos podem ser carregados.
        //
        // Configuração por ambiente:
        // - Desenvolvimento: Mais permissiva (permite Swagger, hot reload)
        // - Produção: Mais restritiva (segurança máxima)
        // ====================================================================
        var csp = _environment.IsDevelopment()
            ? BuildDevelopmentCSP()
            : BuildProductionCSP();

        context.Response.Headers.Append("Content-Security-Policy", csp);

        // ====================================================================
        // ✅ FASE 4: X-CONTENT-TYPE-OPTIONS
        // ====================================================================
        // Previne MIME-sniffing, forçando o browser a respeitar o Content-Type.
        // Protege contra ataques onde um arquivo .txt é interpretado como .js
        // ====================================================================
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // ====================================================================
        // ✅ FASE 4: X-FRAME-OPTIONS
        // ====================================================================
        // Previne clickjacking ao impedir que a página seja carregada em iframe.
        // DENY: Nunca permite iframe
        // SAMEORIGIN: Permite iframe apenas do mesmo domínio
        // ====================================================================
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // ====================================================================
        // ✅ FASE 4: X-XSS-PROTECTION
        // ====================================================================
        // Habilita o filtro XSS do browser (legacy, mas ainda útil).
        // mode=block: Bloqueia a página inteira se XSS for detectado
        // ====================================================================
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // ====================================================================
        // ✅ FASE 4: REFERRER-POLICY
        // ====================================================================
        // Controla quanto de informação de referência é enviada.
        // strict-origin-when-cross-origin: Envia apenas origem em HTTPS cross-origin
        // ====================================================================
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // ====================================================================
        // ✅ FASE 4: PERMISSIONS-POLICY (Feature-Policy)
        // ====================================================================
        // Controla quais APIs do browser podem ser usadas.
        // Desabilita recursos não utilizados para reduzir superfície de ataque.
        // ====================================================================
        context.Response.Headers.Append("Permissions-Policy",
            "geolocation=(), " +
            "microphone=(), " +
            "camera=(), " +
            "payment=(), " +
            "usb=(), " +
            "magnetometer=(), " +
            "gyroscope=(), " +
            "accelerometer=()");

        // ====================================================================
        // ✅ FASE 4: CROSS-ORIGIN-EMBEDDER-POLICY (COEP)
        // ====================================================================
        // Requer que recursos cross-origin tenham CORS habilitado.
        // require-corp: Exige Cross-Origin-Resource-Policy
        // ====================================================================
        if (_environment.IsProduction())
        {
            context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        }

        // ====================================================================
        // ✅ FASE 4: CROSS-ORIGIN-OPENER-POLICY (COOP)
        // ====================================================================
        // Isola o contexto de navegação de popups.
        // same-origin: Apenas popups da mesma origem podem acessar
        // ====================================================================
        if (_environment.IsProduction())
        {
            context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        }

        // ====================================================================
        // ✅ FASE 4: CROSS-ORIGIN-RESOURCE-POLICY (CORP)
        // ====================================================================
        // Controla quais origens podem carregar este recurso.
        // same-origin: Apenas a mesma origem pode carregar
        // ====================================================================
        if (_environment.IsProduction())
        {
            context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
        }

        // ====================================================================
        // ✅ FASE 4: REMOVER HEADERS QUE EXPÕEM INFORMAÇÕES
        // ====================================================================
        // Remove headers que revelam tecnologias usadas (fingerprinting).
        // Dificulta ataques direcionados a vulnerabilidades específicas.
        // ====================================================================
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");

        // ====================================================================
        // ✅ FASE 4: LOGGING DE SEGURANÇA
        // ====================================================================
        // Loga requisições suspeitas para análise posterior
        // ====================================================================
        LogSuspiciousRequests(context);

        await _next(context);
    }

    /// <summary>
    /// Constrói CSP para ambiente de desenvolvimento.
    /// Mais permissiva para permitir Swagger, hot reload, etc.
    /// </summary>
    private string BuildDevelopmentCSP()
    {
        return string.Join("; ",
            "default-src 'self'",
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'", // Swagger precisa de unsafe-inline/eval
            "style-src 'self' 'unsafe-inline'", // Swagger precisa de unsafe-inline
            "img-src 'self' data: https:",
            "font-src 'self' data:",
            "connect-src 'self'",
            "frame-ancestors 'none'",
            "base-uri 'self'",
            "form-action 'self'"
        );
    }

    /// <summary>
    /// Constrói CSP para ambiente de produção.
    /// Mais restritiva para máxima segurança.
    /// </summary>
    private string BuildProductionCSP()
    {
        return string.Join("; ",
            "default-src 'self'",
            "script-src 'self'", // ✅ Sem unsafe-inline/eval em produção
            "style-src 'self'", // ✅ Sem unsafe-inline em produção
            "img-src 'self' data: https:",
            "font-src 'self'",
            "connect-src 'self'",
            "frame-ancestors 'none'",
            "base-uri 'self'",
            "form-action 'self'",
            "upgrade-insecure-requests" // ✅ Força upgrade de HTTP para HTTPS
        );
    }

    /// <summary>
    /// Loga requisições suspeitas para análise de segurança.
    /// </summary>
    private void LogSuspiciousRequests(HttpContext context)
    {
        var request = context.Request;

        // Detecta tentativas de SQL injection nos query parameters
        if (request.QueryString.HasValue)
        {
            var queryString = request.QueryString.Value?.ToLower() ?? string.Empty;
            if (queryString.Contains("select") ||
                queryString.Contains("union") ||
                queryString.Contains("drop") ||
                queryString.Contains("insert") ||
                queryString.Contains("delete") ||
                queryString.Contains("update") ||
                queryString.Contains("exec") ||
                queryString.Contains("script"))
            {
                _logger.LogWarning(
                    "⚠️ Possível tentativa de SQL Injection/XSS detectada | " +
                    "Path: {Path} | Query: {Query} | IP: {IP} | UserAgent: {UserAgent}",
                    request.Path,
                    request.QueryString,
                    context.Connection.RemoteIpAddress,
                    request.Headers.UserAgent.ToString());
            }
        }

        // Detecta tentativas de path traversal
        if (request.Path.Value?.Contains("..") == true)
        {
            _logger.LogWarning(
                "⚠️ Possível tentativa de Path Traversal detectada | " +
                "Path: {Path} | IP: {IP}",
                request.Path,
                context.Connection.RemoteIpAddress);
        }

        // Detecta User-Agents suspeitos (scanners, bots maliciosos)
        var userAgent = request.Headers.UserAgent.ToString().ToLower();
        if (string.IsNullOrWhiteSpace(userAgent) ||
            userAgent.Contains("sqlmap") ||
            userAgent.Contains("nikto") ||
            userAgent.Contains("nmap") ||
            userAgent.Contains("masscan") ||
            userAgent.Contains("acunetix"))
        {
            _logger.LogWarning(
                "⚠️ User-Agent suspeito detectado | " +
                "UserAgent: {UserAgent} | Path: {Path} | IP: {IP}",
                request.Headers.UserAgent.ToString(),
                request.Path,
                context.Connection.RemoteIpAddress);
        }
    }
}
