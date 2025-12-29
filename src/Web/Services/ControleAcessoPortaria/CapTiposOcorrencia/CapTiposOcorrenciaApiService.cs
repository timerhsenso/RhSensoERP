// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// ApiRoute: api/gestaoterceirosprestadores/captiposocorrencia
// Data: 2025-12-28 19:27:50
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
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// Implementação do serviço de API para CapTiposOcorrencia.
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
public class CapTiposOcorrenciaApiService : ICapTiposOcorrenciaApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CapTiposOcorrenciaApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    // =========================================================================
    // v3.2 - ROTA API DO MANIFESTO (não mais construída automaticamente)
    // =========================================================================
    private const string ApiRoute = "api/gestaoterceirosprestadores/captiposocorrencia";

    public CapTiposOcorrenciaApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CapTiposOcorrenciaApiService> logger)
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
                _logger.LogDebug("[CAPTIPOSOCORRENCIA] Token JWT configurado");
            }
            else
            {
                _logger.LogWarning("[CAPTIPOSOCORRENCIA] Token JWT não encontrado");
            }
        }
        else
        {
            _logger.LogWarning("[CAPTIPOSOCORRENCIA] Usuário não autenticado");
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
        
        _logger.LogDebug("[CAPTIPOSOCORRENCIA] {Op} - Status: {Status}, Content: {Content}", 
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
                return Success(backendResult.Value ?? backendResult.Data);
            }

            return Fail<T>(backendResult.Error?.Message ?? $"Erro HTTP {(int)response.StatusCode}");
        }
        catch (JsonException)
        {
            // Tenta como string simples
            return Fail<T>($"Erro: {content}");
        }
    }

    #endregion

    #region IApiService Implementation

    public async Task<ApiResponse<IEnumerable<CapTiposOcorrenciaDto>>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync(ApiRoute);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<IEnumerable<CapTiposOcorrenciaDto>>(response, "GetAll");

            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<List<CapTiposOcorrenciaDto>>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {
                IEnumerable<CapTiposOcorrenciaDto> items = backendResult.Value ?? backendResult.Data ?? new List<CapTiposOcorrenciaDto>();
                return Success(items);
            }

            return Fail<IEnumerable<CapTiposOcorrenciaDto>>(backendResult?.Error?.Message ?? "Erro ao buscar registros");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em GetAllAsync");
            return Fail<IEnumerable<CapTiposOcorrenciaDto>>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em GetAllAsync");
            return Fail<IEnumerable<CapTiposOcorrenciaDto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<PagedResult<CapTiposOcorrenciaDto>>> GetPagedAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            var queryParams = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search))
                queryParams += $"&search={Uri.EscapeDataString(search)}";
            
            var response = await _httpClient.GetAsync($"{ApiRoute}{queryParams}");
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<PagedResult<CapTiposOcorrenciaDto>>(response, "GetPaged");

            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<PagedResult<CapTiposOcorrenciaDto>>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {
                var pagedResult = backendResult.Value ?? backendResult.Data;
                if (pagedResult != null)
                    return Success(pagedResult);
            }

            return Fail<PagedResult<CapTiposOcorrenciaDto>>(backendResult?.Error?.Message ?? "Erro ao buscar registros paginados");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em GetPagedAsync");
            return Fail<PagedResult<CapTiposOcorrenciaDto>>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em GetPagedAsync");
            return Fail<PagedResult<CapTiposOcorrenciaDto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<CapTiposOcorrenciaDto>> GetByIdAsync(int id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}/{id}");
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<CapTiposOcorrenciaDto>(response, "GetById");

            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<CapTiposOcorrenciaDto>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {
                return Success(backendResult.Value ?? backendResult.Data);
            }

            return Fail<CapTiposOcorrenciaDto>(backendResult?.Error?.Message ?? "Registro não encontrado");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em GetByIdAsync");
            return Fail<CapTiposOcorrenciaDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em GetByIdAsync");
            return Fail<CapTiposOcorrenciaDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<CapTiposOcorrenciaDto>> CreateAsync(CreateCapTiposOcorrenciaRequest request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[CAPTIPOSOCORRENCIA] POST {Route} - Body: {Body}", ApiRoute, json);
            
            var response = await _httpClient.PostAsync(ApiRoute, content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<CapTiposOcorrenciaDto>(response, "Create");

            // Backend retorna Result<int> com o ID criado
            var responseJson = await response.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<BackendResult<int>>(responseJson, _jsonOptions);
            
            if (createResult?.IsSuccess == true)
            {
                // v3.3 FIX: Usa ternário em vez de ?? (value types não são nullable)
                var createdId = createResult.Value != 0 ? createResult.Value : createResult.Data;
                    
                if (createdId != 0)
                    return await GetByIdAsync(createdId);
            }

            return Fail<CapTiposOcorrenciaDto>(createResult?.Error?.Message ?? "Erro ao criar registro");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em CreateAsync");
            return Fail<CapTiposOcorrenciaDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em CreateAsync");
            return Fail<CapTiposOcorrenciaDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<CapTiposOcorrenciaDto>> UpdateAsync(int id, UpdateCapTiposOcorrenciaRequest request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[CAPTIPOSOCORRENCIA] PUT {Route}/{Id} - Body: {Body}", ApiRoute, id, json);
            
            var response = await _httpClient.PutAsync($"{ApiRoute}/{id}", content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<CapTiposOcorrenciaDto>(response, "Update");

            return await GetByIdAsync(id);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em UpdateAsync");
            return Fail<CapTiposOcorrenciaDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em UpdateAsync");
            return Fail<CapTiposOcorrenciaDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
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
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em DeleteAsync");
            return Fail<bool>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em DeleteAsync");
            return Fail<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<int> ids)
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
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em DeleteMultipleAsync");
            return Fail<bool>(ex.Message);
        }
    }

    #endregion

    #region IBatchDeleteService Implementation

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<int> ids)
    {
        try
        {
            await SetAuthHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[CAPTIPOSOCORRENCIA] DELETE {Route}/batch - Body: {Body}", ApiRoute, json);
            
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
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Erro de conexão em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPTIPOSOCORRENCIA] Exceção em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>(ex.Message);
        }
    }

    #endregion

    #region Backend DTOs

    /// <summary>
    /// DTO para deserializar resposta do backend.
    /// Backend pode retornar Value ou Data dependendo do endpoint.
    /// </summary>
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
