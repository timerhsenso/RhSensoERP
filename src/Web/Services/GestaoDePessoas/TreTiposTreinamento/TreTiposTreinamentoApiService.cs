// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0 FINAL
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// ApiRoute: api/treinamentodesenvolvimento/tretipostreinamento
// Data: 2025-12-30 05:27:05
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// v4.0 FINAL: Parâmetros corretos (sortBy, desc) para ordenação server-side
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// Implementação do serviço de API para Tipos de Treinamento.
/// v4.0 FINAL: Corrigido para usar sortBy e desc (compatível com backend).
/// </summary>
public class TreTiposTreinamentoApiService : ITreTiposTreinamentoApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TreTiposTreinamentoApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = "api/treinamentodesenvolvimento/tretipostreinamento";

    public TreTiposTreinamentoApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TreTiposTreinamentoApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Private Helpers

    private async Task SetAuthHeaderAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var token = await context.GetTokenAsync("access_token");
            
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("[TRETIPOSTREINAMENTO] Token JWT configurado");
            }
            else
            {
                _logger.LogWarning("[TRETIPOSTREINAMENTO] Token JWT não encontrado");
            }
        }
        else
        {
            _logger.LogWarning("[TRETIPOSTREINAMENTO] Usuário não autenticado");
        }
    }

    private static ApiResponse<T> Success<T>(T? data) => new()
    {
        Success = true,
        Data = data
    };

    private static ApiResponse<T> Fail<T>(string message) => new()
    {
        Success = false,
        Error = new ApiError { Message = message }
    };

    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(
        HttpResponseMessage response, 
        string operation)
    {
        var content = await response.Content.ReadAsStringAsync();
        
        _logger.LogDebug("[TRETIPOSTREINAMENTO] {Op} - Status: {Status}, Content: {Content}", 
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
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro JSON em {Op}", operation);
            return Fail<T>("Erro ao processar resposta do servidor");
        }
    }

    #endregion

    #region IApiService Implementation

    /// <summary>
    /// ✅ v4.0 FINAL: GetPagedAsync com parâmetros corretos (sortBy, desc).
    /// </summary>
    public async Task<ApiResponse<PagedResult<TreTiposTreinamentoDto>>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null,
        string? sortBy = null,      // ✅ CORRETO: sortBy (não orderBy)
        bool desc = false)          // ✅ CORRETO: desc (não ascending)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            // ================================================================
            // ✅ CONSTRÓI QUERY STRING COM PARÂMETROS CORRETOS
            // ================================================================
            var query = $"?page={page}&pageSize={pageSize}";
            
            if (!string.IsNullOrWhiteSpace(search))
                query += $"&search={Uri.EscapeDataString(search)}";
            
            // ✅ CORRETO: sortBy (não orderBy)
            if (!string.IsNullOrWhiteSpace(sortBy))
                query += $"&sortBy={Uri.EscapeDataString(sortBy)}";
            
            // ✅ CORRETO: desc (não ascending)
            query += $"&desc={desc.ToString().ToLower()}";

            _logger.LogDebug(
                "[TRETIPOSTREINAMENTO] GET {Route}{Query} - SortBy: {SortBy}, Desc: {Desc}", 
                ApiRoute, query, sortBy ?? "null", desc
            );

            var response = await _httpClient.GetAsync($"{ApiRoute}{query}");
            return await ProcessResponseAsync<PagedResult<TreTiposTreinamentoDto>>(response, "GetPaged");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro de conexão em GetPagedAsync");
            return Fail<PagedResult<TreTiposTreinamentoDto>>("Erro de conexão com o servidor");
        }
        catch (TaskCanceledException)
        {
            return Fail<PagedResult<TreTiposTreinamentoDto>>("Tempo limite excedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em GetPagedAsync");
            return Fail<PagedResult<TreTiposTreinamentoDto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<TreTiposTreinamentoDto>>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}?page=1&pageSize=10000");
            var result = await ProcessResponseAsync<PagedResult<TreTiposTreinamentoDto>>(response, "GetAll");
            
            if (result.Success && result.Data != null)
                return Success<IEnumerable<TreTiposTreinamentoDto>>(result.Data.Items);
            
            return Fail<IEnumerable<TreTiposTreinamentoDto>>(result.Error?.Message ?? "Erro ao buscar dados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em GetAllAsync");
            return Fail<IEnumerable<TreTiposTreinamentoDto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<TreTiposTreinamentoDto>> GetByIdAsync(int id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}/{id}");
            return await ProcessResponseAsync<TreTiposTreinamentoDto>(response, "GetById");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro de conexão em GetByIdAsync");
            return Fail<TreTiposTreinamentoDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em GetByIdAsync");
            return Fail<TreTiposTreinamentoDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<TreTiposTreinamentoDto>> CreateAsync(CreateTreTiposTreinamentoRequest request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[TRETIPOSTREINAMENTO] POST {Route} - Body: {Body}", ApiRoute, json);
            
            var response = await _httpClient.PostAsync(ApiRoute, content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<TreTiposTreinamentoDto>(response, "Create");

            // Backend retorna Result<int> com o ID criado
            var responseJson = await response.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<BackendResult<int>>(responseJson, _jsonOptions);
            
            if (createResult?.IsSuccess == true)
            {
                var createdId = createResult.Value != 0 ? createResult.Value : createResult.Data;
                    
                if (createdId != 0)
                    return await GetByIdAsync(createdId);
            }

            return Fail<TreTiposTreinamentoDto>(createResult?.Error?.Message ?? "Erro ao criar registro");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro de conexão em CreateAsync");
            return Fail<TreTiposTreinamentoDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em CreateAsync");
            return Fail<TreTiposTreinamentoDto>(ex.Message);
        }
    }

    public async Task<ApiResponse<TreTiposTreinamentoDto>> UpdateAsync(int id, UpdateTreTiposTreinamentoRequest request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[TRETIPOSTREINAMENTO] PUT {Route}/{Id} - Body: {Body}", ApiRoute, id, json);
            
            var response = await _httpClient.PutAsync($"{ApiRoute}/{id}", content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<TreTiposTreinamentoDto>(response, "Update");

            return await GetByIdAsync(id);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro de conexão em UpdateAsync");
            return Fail<TreTiposTreinamentoDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em UpdateAsync");
            return Fail<TreTiposTreinamentoDto>(ex.Message);
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
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro de conexão em DeleteAsync");
            return Fail<bool>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em DeleteAsync");
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
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em DeleteMultipleAsync");
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
            
            _logger.LogDebug("[TRETIPOSTREINAMENTO] DELETE {Route}/batch - Body: {Body}", ApiRoute, json);
            
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
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Erro de conexão em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRETIPOSTREINAMENTO] Exceção em DeleteBatchAsync");
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
