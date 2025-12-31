// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: CapOcorrencias
// Module: ControleAcessoPortaria
// ApiRoute: api/gestaoterceirosprestadores/capocorrencias
// Data: 2025-12-30 21:29:17
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// v5.2: Logs corrigidos - gera {Route} não {{Route}}
// v5.0: Herança genérica correta de BaseApiService + IHttpContextAccessor
// v4.1: Adiciona ToggleAtivoAsync para alternar status Ativo/Desativo
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapOcorrencias;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapOcorrencias;

/// <summary>
/// Serviço de API para CapOcorrencias.
/// Herda implementação base de BaseApiService (GetPaged, GetAll, GetById, Create, Update, Delete).
/// v5.2: Logs corrigidos.
/// v5.0: Herança genérica correta + construtor compatível com BaseApiService.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// AUTO-REGISTRADO: Compatível com AddCrudToolServicesAutomatically().
/// </summary>
public class CapOcorrenciasApiService 
    : BaseApiService<CapOcorrenciasDto, CreateCapOcorrenciasRequest, UpdateCapOcorrenciasRequest, int>,
      ICapOcorrenciasApiService
{
    private const string ApiRoute = "api/gestaoterceirosprestadores/capocorrencias";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // =========================================================================
    // ✅ v5.0: CONSTRUTOR CORRIGIDO - USA IHttpContextAccessor
    // =========================================================================
    public CapOcorrenciasApiService(
        HttpClient httpClient,
        ILogger<CapOcorrenciasApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {
        // ✅ Construtor base recebe: httpClient, logger, httpContextAccessor, baseEndpoint
        // ✅ BaseApiService já implementa: GetPagedAsync, GetAllAsync, GetByIdAsync, 
        //    CreateAsync, UpdateAsync, DeleteAsync, DeleteMultipleAsync
        // ✅ Autenticação (JWT via Cookie) já é tratada por AddAuthorizationHeaderAsync() do base
    }

    // =========================================================================
    // ✅ MÉTODOS CRUD JÁ IMPLEMENTADOS NO BaseApiService
    // =========================================================================
    // - GetPagedAsync(page, pageSize, search, sortBy, desc)
    // - GetAllAsync()
    // - GetByIdAsync(id)
    // - CreateAsync(createDto)
    // - UpdateAsync(id, updateDto)
    // - DeleteAsync(id)
    // - DeleteMultipleAsync(ids)

    // =========================================================================
    // IBatchDeleteService Implementation
    // =========================================================================

    /// <summary>
    /// Exclusão em lote via endpoint /batch.
    /// </summary>
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<int> ids)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[CAPOCORRENCIAS] DELETE {Route}/batch - Body: {Body}", _baseEndpoint, json);
            
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
            _logger.LogError(ex, "[CAPOCORRENCIAS] Erro de conexão em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro de conexão com o servidor" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPOCORRENCIAS] Exceção em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = ex.Message }
            };
        }
    }


    #region v4.1 - Toggle Ativo

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// Chamada via PATCH para /api/{route}/{id}/toggle-ativo
    /// </summary>
    public async Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            
            // Payload simples com o novo valor de Ativo
            var payload = new { Ativo = ativo };
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug(
                "[CAPOCORRENCIAS] PATCH {Route}/{Id}/toggle-ativo - Body: {Body}",
                _baseEndpoint,
                id,
                json);
            
            // Tenta PATCH primeiro (mais semântico)
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseEndpoint}/{id}/toggle-ativo")
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning(
                    "[CAPOCORRENCIAS] Erro ao alternar Ativo - Status: {Status}, Content: {Content}",
                    response.StatusCode,
                    errorContent);
                
                throw new HttpRequestException(
                    $"Erro ao alternar status Ativo: {response.StatusCode}");
            }

            _logger.LogInformation(
                "[CAPOCORRENCIAS] Status Ativo alterado para {Ativo} - ID: {Id}",
                ativo,
                id);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[CAPOCORRENCIAS] Erro de conexão em ToggleAtivoAsync");
            throw new InvalidOperationException("Erro de conexão com o servidor", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CAPOCORRENCIAS] Exceção em ToggleAtivoAsync");
            throw;
        }
    }

    #endregion
    // =========================================================================
    // Backend DTOs (para DeleteBatchAsync)
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
