// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Tsistema
// Data: 2025-12-02 02:25:04
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// NOTA: Este serviço usa HttpClient TIPADO injetado pelo DI.
// O Timeout e políticas de resiliência (Polly) são configurados em:
// ServiceCollectionExtensions.AddCrudToolServicesAutomatically()
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using RhSensoERP.Web.Models.Tsistemas;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Tsistemas;

/// <summary>
/// Implementação do serviço de API para Tsistema.
/// Consome a API backend gerada pelo Source Generator.
/// </summary>
/// <remarks>
/// Este serviço é registrado automaticamente no DI via:
/// <code>services.AddCrudToolServicesAutomatically(apiSettings)</code>
/// 
/// HttpClient já vem configurado com:
/// - BaseAddress
/// - Timeout
/// - Retry Policy (Polly)
/// - Circuit Breaker (Polly)
/// </remarks>
public class TsistemaApiService : ITsistemaApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TsistemaApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = "api/identity/tsistemas";

    public TsistemaApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TsistemaApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        
        // NOTA: Timeout e BaseAddress já configurados pelo DI (ServiceCollectionExtensions)
        // NÃO configurar aqui para evitar conflito com Polly
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Private Helpers

    /// <summary>
    /// Configura header de autenticação com token JWT.
    /// Token está em AuthenticationTokens (StoreTokens no AccountController).
    /// </summary>
    private async Task SetAuthHeaderAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            // Token está em AuthenticationTokens, não em Claims
            var token = await context.GetTokenAsync("access_token");
            
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("[TSISTEMA] Token JWT configurado");
            }
            else
            {
                _logger.LogWarning("[TSISTEMA] Token JWT não encontrado");
            }
        }
        else
        {
            _logger.LogWarning("[TSISTEMA] Usuário não autenticado");
        }
    }

    /// <summary>
    /// Cria ApiResponse de sucesso.
    /// </summary>
    private static ApiResponse<T> Success<T>(T? data) => new()
    {
        Success = true,
        Data = data
    };

    /// <summary>
    /// Cria ApiResponse de erro.
    /// NOTA: Message é computed (=> Error?.Message), então usamos Error.
    /// </summary>
    private static ApiResponse<T> Fail<T>(string message) => new()
    {
        Success = false,
        Error = new ApiError { Message = message }
    };

    /// <summary>
    /// Processa resposta HTTP do backend.
    /// </summary>
    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(
        HttpResponseMessage response, 
        string operation)
    {
        var content = await response.Content.ReadAsStringAsync();
        
        _logger.LogDebug("[TSISTEMA] {Op} - Status: {Status}, Content: {Content}", 
            operation, response.StatusCode, content);

        if (string.IsNullOrEmpty(content))
            return Fail<T>($"Resposta vazia do servidor ({(int)response.StatusCode})");

        try
        {
            var backendResult = JsonSerializer.Deserialize<BackendResult<T>>(content, _jsonOptions);
            
            if (backendResult == null)
                return Fail<T>("Resposta inválida do servidor");

            if (backendResult.IsSuccess)
            {
                var data = backendResult.Value ?? backendResult.Data;
                return Success(data);
            }

            return Fail<T>(backendResult.Error?.Message ?? "Erro desconhecido");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro JSON em {Op}", operation);
            return Fail<T>("Erro ao processar resposta do servidor");
        }
    }

    #endregion

    #region IApiService Implementation

    public async Task<ApiResponse<PagedResult<TsistemaDto>>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var query = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                query += $"&search={Uri.EscapeDataString(search)}";

            _logger.LogDebug("[TSISTEMA] GET {Route}{Query}", ApiRoute, query);

            var response = await _httpClient.GetAsync($"{ApiRoute}{query}");
            return await ProcessResponseAsync<PagedResult<TsistemaDto>>(response, "GetPaged");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em GetPagedAsync");
            return Fail<PagedResult<TsistemaDto>>("Erro de conexão com o servidor");
        }
        catch (TaskCanceledException)
        {
            return Fail<PagedResult<TsistemaDto>>("Tempo limite excedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em GetPagedAsync");
            return Fail<PagedResult<TsistemaDto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<TsistemaDto>>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}?page=1&pageSize=10000");
            var result = await ProcessResponseAsync<PagedResult<TsistemaDto>>(response, "GetAll");
            
            if (result.Success && result.Data != null)
                return Success<IEnumerable<TsistemaDto>>(result.Data.Items);
            
            return Fail<IEnumerable<TsistemaDto>>(result.Error?.Message ?? "Erro ao buscar dados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em GetAllAsync");
            return Fail<IEnumerable<TsistemaDto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<TsistemaDto>> GetByIdAsync(string id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}/{id}");
            return await ProcessResponseAsync<TsistemaDto>(response, "GetById");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em GetByIdAsync");
            return Fail<TsistemaDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em GetByIdAsync");
            return Fail<TsistemaDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<TsistemaDto>> CreateAsync(CreateTsistemaRequest request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[TSISTEMA] POST {Route} - Body: {Body}", ApiRoute, json);
            
            var response = await _httpClient.PostAsync(ApiRoute, content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<TsistemaDto>(response, "Create");

            // Backend retorna Result<string> com o ID criado
            var responseJson = await response.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<BackendResult<string>>(responseJson, _jsonOptions);
            
            if (createResult?.IsSuccess == true)
            {
                var createdId = createResult.Value;
                if (string.IsNullOrEmpty(createdId) && !string.IsNullOrEmpty(createResult.Data))
                    createdId = createResult.Data;
                    
                if (!string.IsNullOrEmpty(createdId))
                    return await GetByIdAsync(createdId);
            }

            return Fail<TsistemaDto>(createResult?.Error?.Message ?? "Erro ao criar registro");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em CreateAsync");
            return Fail<TsistemaDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em CreateAsync");
            return Fail<TsistemaDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<TsistemaDto>> UpdateAsync(string id, UpdateTsistemaRequest request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[TSISTEMA] PUT {Route}/{Id} - Body: {Body}", ApiRoute, id, json);
            
            var response = await _httpClient.PutAsync($"{ApiRoute}/{id}", content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<TsistemaDto>(response, "Update");

            return await GetByIdAsync(id);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em UpdateAsync");
            return Fail<TsistemaDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em UpdateAsync");
            return Fail<TsistemaDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{ApiRoute}/{id}");
            
            if (response.IsSuccessStatusCode)
                return Success(true);
            
            return await ProcessResponseAsync<bool>(response, "Delete");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em DeleteAsync");
            return Fail<bool>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em DeleteAsync");
            return Fail<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<string> ids)
    {
        try
        {
            var result = await DeleteBatchAsync(ids);
            
            if (result.Success && result.Data != null)
                return Success(result.Data.FailureCount == 0);
            
            return Fail<bool>(result.Error?.Message ?? "Erro ao excluir registros");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em DeleteMultipleAsync");
            return Fail<bool>(ex.Message);
        }
    }

    #endregion

    #region IBatchDeleteService Implementation

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<string> ids)
    {
        try
        {
            await SetAuthHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[TSISTEMA] DELETE {Route}/batch - Body: {Body}", ApiRoute, json);
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiRoute}/batch")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<BackendBatchDeleteResult>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                {
                    var dto = new BatchDeleteResultDto
                    {
                        SuccessCount = data.SuccessCount,
                        FailureCount = data.FailureCount,
                        Errors = data.Errors?.Select(e => new BatchDeleteErrorDto
                        {
                            Code = e.Id ?? e.Code ?? string.Empty,
                            Message = e.Message ?? string.Empty
                        }).ToList() ?? []
                    };
                    
                    return Success(dto);
                }
            }
            
            return Fail<BatchDeleteResultDto>(backendResult?.Error?.Message ?? "Erro ao excluir em lote");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>(ex.Message);
        }
    }

    #endregion

    #region Backend DTOs

    private sealed class BackendResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public T? Data { get; set; }
        public BackendError? Error { get; set; }
    }

    private sealed class BackendError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    private sealed class BackendBatchDeleteResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<BackendBatchDeleteError>? Errors { get; set; }
    }

    private sealed class BackendBatchDeleteError
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    #endregion
}
