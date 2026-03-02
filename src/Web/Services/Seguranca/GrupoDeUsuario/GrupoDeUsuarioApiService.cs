// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.2
// Entity: GrupoDeUsuario
// Module: Seguranca
// ApiRoute: api/seguranca/grupodeusuario
// Data: 2026-03-01 22:11:05
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.Seguranca.GrupoDeUsuario;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.GrupoDeUsuario;

/// <summary>
/// Serviço de API para Grupo de Usuários.
/// Herda implementação base de BaseApiService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona implementações Select2 Lookup automáticas.
/// </summary>
public class GrupoDeUsuarioApiService 
    : BaseApiService<GrupoDeUsuarioDto, CreateGrupoDeUsuarioRequest, UpdateGrupoDeUsuarioRequest, string>,
      IGrupoDeUsuarioApiService
{
    private const string ApiRoute = "api/seguranca/grupodeusuario";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================
    public GrupoDeUsuarioApiService(
        HttpClient httpClient,
        ILogger<GrupoDeUsuarioApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {
    }

    // =========================================================================
    // ⭐ v6.2: COMPOSITE KEY OVERRIDES
    // Converte PK pipe-separated para path segments na URL.
    // Ex: "RHU|ADMIN" → URL ".../RHU/ADMIN"
    // Funciona com N campos de PK composta.
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
    public override async Task<ApiResponse<GrupoDeUsuarioDto>> GetByIdAsync(string id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var path = CompositeKeyToPath(id);
            var url = $"{_baseEndpoint}/{path}";
            _logger.LogDebug("🔍 [GRUPODEUSUARIO] GetByIdAsync - CompositeKey: {Id} → URL: {Url}", id, url);

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<GrupoDeUsuarioDto>>(content, _jsonOptions);
                return result ?? new ApiResponse<GrupoDeUsuarioDto>
                {
                    Success = false,
                    Error = new ApiError { Message = "Erro ao deserializar resposta" }
                };
            }

            _logger.LogWarning("⚠️ [GRUPODEUSUARIO] GetByIdAsync - ID: {Id}, Erro {StatusCode}", id, response.StatusCode);
            return new ApiResponse<GrupoDeUsuarioDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GRUPODEUSUARIO] Erro ao buscar registro {Id}", id);
            return new ApiResponse<GrupoDeUsuarioDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registro" }
            };
        }
    }

    /// <summary>
    /// ⭐ Override: PUT com PK composta → path segments.
    /// </summary>
    public override async Task<ApiResponse<GrupoDeUsuarioDto>> UpdateAsync(string id, UpdateGrupoDeUsuarioRequest dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var path = CompositeKeyToPath(id);
            var url = $"{_baseEndpoint}/{path}";
            _logger.LogDebug("✏️ [GRUPODEUSUARIO] UpdateAsync - CompositeKey: {Id} → URL: {Url}", id, url);

            var response = await _httpClient.PutAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var resultBool = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                if (resultBool != null && resultBool.Success)
                {
                    return await GetByIdAsync(id);
                }
                return new ApiResponse<GrupoDeUsuarioDto>
                {
                    Success = false,
                    Error = resultBool?.Error ?? new ApiError { Message = "Erro ao atualizar registro" },
                    Errors = resultBool?.Errors
                };
            }

            _logger.LogWarning("⚠️ [GRUPODEUSUARIO] UpdateAsync - ID: {Id}, Erro {StatusCode}", id, response.StatusCode);
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<GrupoDeUsuarioDto>>(content, _jsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<GrupoDeUsuarioDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao atualizar registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GRUPODEUSUARIO] Erro ao atualizar registro {Id}", id);
            return new ApiResponse<GrupoDeUsuarioDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao atualizar registro" }
            };
        }
    }

    /// <summary>
    /// ⭐ Override: DELETE com PK composta → path segments.
    /// </summary>
    public override async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var path = CompositeKeyToPath(id);
            var url = $"{_baseEndpoint}/{path}";
            _logger.LogDebug("🗑️ [GRUPODEUSUARIO] DeleteAsync - CompositeKey: {Id} → URL: {Url}", id, url);

            var response = await _httpClient.DeleteAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ [GRUPODEUSUARIO] Registro {Id} excluído", id);
                return new ApiResponse<bool> { Success = true, Data = true };
            }

            _logger.LogWarning("⚠️ [GRUPODEUSUARIO] DeleteAsync - ID: {Id}, Erro {StatusCode}", id, response.StatusCode);
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao excluir registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [GRUPODEUSUARIO] Erro ao excluir registro {Id}", id);
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
            
            _logger.LogDebug("[GRUPODEUSUARIO] DELETE {Route}/batch - Body: {Body}", _baseEndpoint, json);
            
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
            _logger.LogError(ex, "[GRUPODEUSUARIO] Erro de conexão em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro de conexão com o servidor" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GRUPODEUSUARIO] Exceção em DeleteBatchAsync");
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
