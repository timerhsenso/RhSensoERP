using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RhSensoWeb.Configuration;
using RhSensoWeb.Models.Auth;
using RhSensoWeb.Models.Shared;
using RhSensoWeb.Services.Interfaces;

namespace RhSensoWeb.Services;

/// <summary>
/// Serviço para autenticação com a API RhSensoERP
/// </summary>
public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<AuthApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthApiService(
        HttpClient httpClient,
        IOptionsMonitor<ApiSettings> apiSettings,
        ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient;
        _apiSettings = apiSettings.CurrentValue;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    /// <summary>
    /// Configura o HttpClient com as configurações da API
    /// </summary>
    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_apiSettings.TimeoutSeconds);
        
        // Headers padrão
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Headers customizados
        foreach (var header in _apiSettings.DefaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    /// <summary>
    /// Realiza login na API
    /// </summary>
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando login para usuário {Usuario}", request.CdUsuario);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/login", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (_apiSettings.EnableDetailedLogging)
            {
                _logger.LogDebug("Resposta da API: {StatusCode} - {Content}", 
                    response.StatusCode, responseContent);
            }

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(responseContent, _jsonOptions);
                
                if (apiResponse?.Success == true)
                {
                    _logger.LogInformation("Login realizado com sucesso para usuário {Usuario}", request.CdUsuario);
                    return apiResponse;
                }
                else
                {
                    _logger.LogWarning("Login falhou para usuário {Usuario}: {Message}", 
                        request.CdUsuario, apiResponse?.Message);
                    return ApiResponse<LoginResponseDto>.Fail(apiResponse?.Message ?? "Erro desconhecido");
                }
            }
            else
            {
                _logger.LogError("Erro HTTP no login para usuário {Usuario}: {StatusCode}", 
                    request.CdUsuario, response.StatusCode);

                // Tentar deserializar erro da API
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(responseContent, _jsonOptions);
                    return ApiResponse<LoginResponseDto>.Fail(errorResponse?.Message ?? "Erro de autenticação");
                }
                catch
                {
                    return ApiResponse<LoginResponseDto>.Fail($"Erro HTTP: {response.StatusCode}");
                }
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError("Timeout no login para usuário {Usuario}", request.CdUsuario);
            return ApiResponse<LoginResponseDto>.Fail("Timeout na requisição. Tente novamente.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede no login para usuário {Usuario}", request.CdUsuario);
            return ApiResponse<LoginResponseDto>.Fail("Erro de conexão com o servidor.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no login para usuário {Usuario}", request.CdUsuario);
            return ApiResponse<LoginResponseDto>.Fail("Erro interno. Tente novamente.");
        }
    }

    /// <summary>
    /// Valida se o token JWT ainda é válido
    /// </summary>
    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync("auth/validate", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar token");
            return false;
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    /// <summary>
    /// Obtém informações atualizadas do usuário
    /// </summary>
    public async Task<ApiResponse<UserDataDto>> GetUserInfoAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync("auth/userinfo", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserDataDto>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<UserDataDto>.Fail("Resposta inválida da API");
            }
            else
            {
                return ApiResponse<UserDataDto>.Fail($"Erro HTTP: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário");
            return ApiResponse<UserDataDto>.Fail("Erro interno. Tente novamente.");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    /// <summary>
    /// Realiza logout na API
    /// </summary>
    public async Task<ApiResponse> LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync("auth/logout", null, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse.Ok("Logout realizado com sucesso");
            }
            else
            {
                return ApiResponse.Fail($"Erro HTTP: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar logout");
            return ApiResponse.Fail("Erro interno durante logout");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    /// <summary>
    /// Renova o token JWT
    /// </summary>
    public async Task<ApiResponse<string>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync("auth/refresh", null, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<string>.Fail("Resposta inválida da API");
            }
            else
            {
                return ApiResponse<string>.Fail($"Erro HTTP: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return ApiResponse<string>.Fail("Erro interno ao renovar token");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
