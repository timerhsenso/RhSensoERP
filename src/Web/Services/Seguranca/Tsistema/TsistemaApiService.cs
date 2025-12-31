// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: Tsistema
// Module: Seguranca
// ApiRoute: api/identity/tsistema
// Data: 2025-12-30 17:48:37
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// v5.2: Logs corrigidos - gera {Route} não {{Route}}
// v5.0: Herança genérica correta de BaseApiService + IHttpContextAccessor
// v4.1: Adiciona ToggleAtivoAsync para alternar status Ativo/Desativo
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.Seguranca.Tsistema;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Tsistema;

/// <summary>
/// Serviço de API para Tabela de Sistemas.
/// Herda implementação base de BaseApiService (GetPaged, GetAll, GetById, Create, Update, Delete).
/// v5.2: Logs corrigidos.
/// v5.0: Herança genérica correta + construtor compatível com BaseApiService.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// AUTO-REGISTRADO: Compatível com AddCrudToolServicesAutomatically().
/// </summary>
public class TsistemaApiService 
    : BaseApiService<TsistemaDto, CreateTsistemaRequest, UpdateTsistemaRequest, string>,
      ITsistemaApiService
{
    private const string ApiRoute = "api/identity/tsistema";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // =========================================================================
    // ✅ v5.0: CONSTRUTOR CORRIGIDO - USA IHttpContextAccessor
    // =========================================================================
    public TsistemaApiService(
        HttpClient httpClient,
        ILogger<TsistemaApiService> logger,
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
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<string> ids)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[TSISTEMA] DELETE {Route}/batch - Body: {Body}", _baseEndpoint, json);
            
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
            _logger.LogError(ex, "[TSISTEMA] Erro de conexão em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro de conexão com o servidor" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TSISTEMA] Exceção em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = ex.Message }
            };
        }
    }


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
