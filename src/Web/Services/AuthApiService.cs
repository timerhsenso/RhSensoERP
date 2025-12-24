// =============================================================================
// RHSENSOERP WEB - AUTH API SERVICE
// =============================================================================
// Arquivo: src/Web/Services/AuthApiService.cs
// Descrição: Implementação do serviço de autenticação via API
// Versão: 3.1 (CORRIGIDO - Endpoint /permissoes)
// =============================================================================

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Implementação do serviço de autenticação via API REST.
/// </summary>
public sealed class AuthApiService : IAuthApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuthApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<AuthApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AuthResponse?> LoginAsync(LoginViewModel model, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("🔐 [LOGIN] Iniciando autenticação para usuário: {CdUsuario}", model.CdUsuario);

            var client = _httpClientFactory.CreateClient("AuthApiClient");

            if (client.BaseAddress == null)
            {
                _logger.LogError("❌ [LOGIN] BaseAddress não configurado no HttpClient");
                return null;
            }

            var endpoint = "/api/identity/auth/login";
            _logger.LogInformation("🔐 [LOGIN] Enviando requisição para: {BaseAddress}{Endpoint}",
                client.BaseAddress, endpoint);

            // DEBUG: Logar os valores recebidos do model
            _logger.LogInformation("🔍 [LOGIN] DEBUG - CdUsuario: '{CdUsuario}', Senha: '{Senha}' (Length: {SenhaLength})",
                model.CdUsuario,
                string.IsNullOrEmpty(model.Senha) ? "[VAZIO]" : "***",
                model.Senha?.Length ?? 0);

            // Monta o payload conforme esperado pela API (LoginRequest)
            // API espera "senha" (português), não "password" (inglês)
            var loginRequest = new
            {
                loginIdentifier = model.CdUsuario,
                senha = model.Senha
            };

            var json = JsonSerializer.Serialize(loginRequest, JsonOptions);

            // DEBUG: Logar o JSON sendo enviado (com senha mascarada)
            _logger.LogInformation("🔍 [LOGIN] DEBUG - JSON Payload: {Json}",
                json.Replace(model.Senha ?? "", "***"));

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content, ct);
            stopwatch.Stop();

            _logger.LogInformation(
                "🔐 [LOGIN] Tempo de resposta da API: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning(
                    "❌ [LOGIN] Falha na autenticação | Status: {StatusCode} | Erro: {Error}",
                    response.StatusCode,
                    errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, JsonOptions);

            _logger.LogInformation(
                "✅ [LOGIN] Autenticação bem-sucedida | Usuário: {CdUsuario} | Tempo total: {ElapsedMs}ms",
                model.CdUsuario,
                stopwatch.ElapsedMilliseconds);

            return authResponse;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "⏱️ [LOGIN] Timeout na requisição de login");
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "🌐 [LOGIN] Erro de conexão com a API");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [LOGIN] Erro inesperado no login");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> LogoutAsync(string accessToken, string refreshToken, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("🚪 [LOGOUT] Iniciando logout");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("⚠️ [LOGOUT] AccessToken não fornecido - logout apenas local");
                return true;
            }

            var client = _httpClientFactory.CreateClient("AuthApiClient");

            if (client.BaseAddress == null)
            {
                _logger.LogError("❌ [LOGOUT] BaseAddress não configurado no HttpClient");
                return false;
            }

            var endpoint = "/api/identity/auth/logout";

            // Adiciona o token JWT no header Authorization
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var logoutRequest = new { refreshToken };
            var json = JsonSerializer.Serialize(logoutRequest, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content, ct);
            stopwatch.Stop();

            _logger.LogInformation(
                "🚪 [LOGOUT] Tempo de resposta: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "⚠️ [LOGOUT] Logout retornou status: {StatusCode}",
                    response.StatusCode);
                return true; // Retorna true - o logout local já foi feito
            }

            _logger.LogInformation("✅ [LOGOUT] Logout realizado com sucesso na API");
            return true;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning(ex, "⏱️ [LOGOUT] Timeout no logout - continuando com logout local");
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "🌐 [LOGOUT] Erro de conexão no logout - continuando com logout local");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [LOGOUT] Erro inesperado no logout");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("🔄 [REFRESH] Renovando token");

            var client = _httpClientFactory.CreateClient("AuthApiClient");

            if (client.BaseAddress == null)
            {
                _logger.LogError("❌ [REFRESH] BaseAddress não configurado");
                return null;
            }

            var endpoint = "/api/identity/auth/refresh-token";
            var request = new { refreshToken };
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ [REFRESH] Falha ao renovar token: {StatusCode}", response.StatusCode);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, JsonOptions);

            _logger.LogDebug("✅ [REFRESH] Token renovado com sucesso");
            return authResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [REFRESH] Erro ao renovar token");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfoViewModel?> GetCurrentUserAsync(string accessToken, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("👤 [USER] Obtendo informações do usuário");

            var client = _httpClientFactory.CreateClient("ApiClient");

            if (client.BaseAddress == null)
            {
                _logger.LogError("❌ [USER] BaseAddress não configurado");
                return null;
            }

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var endpoint = "/api/identity/users/me";
            var response = await client.GetAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ [USER] Falha ao obter usuário: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var userInfo = JsonSerializer.Deserialize<UserInfoViewModel>(content, JsonOptions);

            _logger.LogDebug("✅ [USER] Informações obtidas com sucesso");
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [USER] Erro ao obter informações do usuário");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<UserPermissionsViewModel?> GetUserPermissionsAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("🔑 [PERMISSIONS] Obtendo permissões para: {CdUsuario}", cdUsuario);

            var client = _httpClientFactory.CreateClient("ApiClient");

            if (client.BaseAddress == null)
            {
                _logger.LogError("❌ [PERMISSIONS] BaseAddress não configurado");
                return null;
            }

            // ✅ CORREÇÃO APLICADA: /permissoes em vez de /permissions
            var endpoint = $"/api/identity/permissoes/{cdUsuario}";
            if (!string.IsNullOrWhiteSpace(cdSistema))
            {
                endpoint += $"?cdSistema={Uri.EscapeDataString(cdSistema)}";
            }

            _logger.LogDebug("🔍 [PERMISSIONS] Endpoint: {Endpoint}", endpoint);

            var response = await client.GetAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "❌ [PERMISSIONS] Falha ao obter permissões: {StatusCode} - {ReasonPhrase}",
                    response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var permissions = JsonSerializer.Deserialize<UserPermissionsViewModel>(content, JsonOptions);

            _logger.LogInformation(
                "✅ [PERMISSIONS] Permissões obtidas com sucesso. Funções: {Count}",
                permissions?.Funcoes?.Count ?? 0);

            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [PERMISSIONS] Erro ao obter permissões");
            return null;
        }
    }
}
