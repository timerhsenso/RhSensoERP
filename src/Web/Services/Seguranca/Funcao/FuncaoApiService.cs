// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: Funcao
// Module: Seguranca
// ApiRoute: api/seguranca/funcao
// Data: 2026-03-02 19:25:55
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.Seguranca.Funcao;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Funcao;

/// <summary>
/// Serviço de API para Funções do Sistema.
/// Herda implementação base de BaseApiService.
/// ⭐ v6.3: CamelCase + logging detalhado + BackendResult para erros.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona implementações Select2 Lookup automáticas.
/// </summary>
public class FuncaoApiService 
    : BaseApiService<FuncaoDto, CreateFuncaoRequest, UpdateFuncaoRequest, string>,
      IFuncaoApiService
{
    private const string ApiRoute = "api/seguranca/funcao";

    // =========================================================================
    // ⭐ v6.3 FIX: CamelCase OBRIGATÓRIO
    // Sem isso, Serialize envia PascalCase (ex: "DcGrUser")
    // mas backend espera camelCase (ex: "dcGrUser") e ignora os campos.
    // BaseApiService já usa CamelCase - o override deve ser consistente.
    // =========================================================================
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================
    public FuncaoApiService(
        HttpClient httpClient,
        ILogger<FuncaoApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {
    }

    // =========================================================================
    // ⭐ v6.3: COMPOSITE KEY OVERRIDES (COM LOGGING DETALHADO)
    // Converte PK pipe-separated para path segments na URL.
    // Ex: "RHU|ADMIN" → URL ".../RHU/ADMIN"
    // =========================================================================

    /// <summary>
    /// Converte chave composta pipe-separated em path segments.
    /// "A|B|C" → "A/B/C"
    /// </summary>
    private static string CompositeKeyToPath(string compositeKey)
    {
        if (compositeKey is string strKey && !string.IsNullOrWhiteSpace(strKey))
        {
            var parts = strKey.Split('|');
            return string.Join("/", parts.Select(p => Uri.EscapeDataString(p.Trim())));
        }
        return compositeKey?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// ⭐ Override: GET com PK composta → path segments.
    /// </summary>
    public override async Task<ApiResponse<FuncaoDto>> GetByIdAsync(string id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var path = CompositeKeyToPath(id);
            var url = $"{_baseEndpoint}/{path}";
            _logger.LogDebug("🔍 [FUNCAO] GetByIdAsync - Key: {Id} → URL: {Url}", id, url);

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<FuncaoDto>>(content, _jsonOptions);
                return result ?? new ApiResponse<FuncaoDto>
                {
                    Success = false,
                    Error = new ApiError { Message = "Erro ao deserializar resposta" }
                };
            }

            _logger.LogWarning("⚠️ [FUNCAO] GetByIdAsync - ID: {Id}, Status: {Status}, Body: {Body}",
                id, response.StatusCode, content);
            return new ApiResponse<FuncaoDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [FUNCAO] Erro GetByIdAsync {Id}", id);
            return new ApiResponse<FuncaoDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registro" }
            };
        }
    }

    /// <summary>
    /// ⭐ v6.3: Override: PUT com PK composta + logging detalhado + BackendResult.
    /// </summary>
    public override async Task<ApiResponse<FuncaoDto>> UpdateAsync(string id, UpdateFuncaoRequest dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var path = CompositeKeyToPath(id);
            var url = $"{_baseEndpoint}/{path}";

            // ⭐ v6.3: Loga request body para debugging
            _logger.LogWarning("✏️ [FUNCAO] UpdateAsync - ID: {Id} → URL: {Url} | Body: {Body}", id, url, json);

            var response = await _httpClient.PutAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            // ⭐ v6.3: Loga response raw
            _logger.LogDebug("✏️ [FUNCAO] UpdateAsync - Status: {Status} | Response: {Body}",
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                // ⭐ v6.3: Usa BackendResult<bool> (formato real do backend)
                var backendResult = JsonSerializer.Deserialize<BackendResult<bool>>(content, _jsonOptions);
                if (backendResult?.IsSuccess == true)
                {
                    return await GetByIdAsync(id);
                }
                return new ApiResponse<FuncaoDto>
                {
                    Success = false,
                    Error = new ApiError { Message = backendResult?.Error?.Message ?? "Erro ao atualizar registro" }
                };
            }

            // ⭐ v6.3: Extrai erro real do Result<T> do backend
            _logger.LogWarning("⚠️ [FUNCAO] UpdateAsync FALHOU - ID: {Id}, Status: {Status}, Response: {Body}",
                id, response.StatusCode, content);

            try
            {
                var errorResult = JsonSerializer.Deserialize<BackendResult<bool>>(content, _jsonOptions);
                if (errorResult?.Error != null)
                {
                    return new ApiResponse<FuncaoDto>
                    {
                        Success = false,
                        Error = new ApiError
                        {
                            Code = errorResult.Error.Code ?? string.Empty,
                            Message = errorResult.Error.Message ?? "Erro ao atualizar registro"
                        }
                    };
                }
            }
            catch { }

            return new ApiResponse<FuncaoDto>
            {
                Success = false,
                Error = new ApiError { Message = $"Erro ao atualizar registro (HTTP {(int)response.StatusCode})" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [FUNCAO] Erro UpdateAsync {Id}", id);
            return new ApiResponse<FuncaoDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao atualizar registro" }
            };
        }
    }

    /// <summary>
    /// ⭐ v6.3: Override: DELETE com PK composta + BackendResult para erros.
    /// </summary>
    public override async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var path = CompositeKeyToPath(id);
            var url = $"{_baseEndpoint}/{path}";
            _logger.LogDebug("🗑️ [FUNCAO] DeleteAsync - Key: {Id} → URL: {Url}", id, url);

            var response = await _httpClient.DeleteAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ [FUNCAO] Registro {Id} excluído", id);
                return new ApiResponse<bool> { Success = true, Data = true };
            }

            _logger.LogWarning("⚠️ [FUNCAO] DeleteAsync - ID: {Id}, Status: {Status}, Body: {Body}",
                id, response.StatusCode, content);

            // ⭐ v6.3: Usa BackendResult para extrair mensagem de erro real
            try
            {
                var errorResult = JsonSerializer.Deserialize<BackendResult<bool>>(content, _jsonOptions);
                if (errorResult?.Error != null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Error = new ApiError
                        {
                            Code = errorResult.Error.Code ?? string.Empty,
                            Message = errorResult.Error.Message ?? "Erro ao excluir registro"
                        }
                    };
                }
            }
            catch { }

            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = $"Erro ao excluir registro (HTTP {(int)response.StatusCode})" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [FUNCAO] Erro DeleteAsync {Id}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao excluir registro" }
            };
        }
    }
    // =========================================================================
    // IBatchDeleteService Implementation
    // =========================================================================

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<string> ids)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[FUNCAO] DELETE {Route}/batch - Body: {Body}", _baseEndpoint, json);
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseEndpoint}/batch")
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
                    
                    return new ApiResponse<BatchDeleteResultDto>
                    {
                        Success = true,
                        Data = dto
                    };
                }
            }
            
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = backendResult?.Error?.Message ?? "Erro ao excluir em lote" }
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[FUNCAO] Erro de conexão em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro de conexão com o servidor" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FUNCAO] Exceção em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = ex.Message }
            };
        }
    }




    // =========================================================================
    // Backend DTOs
    // =========================================================================
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
}
