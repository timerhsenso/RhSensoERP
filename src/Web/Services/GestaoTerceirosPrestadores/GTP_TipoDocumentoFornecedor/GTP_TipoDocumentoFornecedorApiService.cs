// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: GTP_TipoDocumentoFornecedor
// Module: GestaoTerceirosPrestadores
// ApiRoute: api/gestaoterceirosprestadores/gtp_tipodocumentofornecedor
// Data: 2026-03-04 22:56:13
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;

/// <summary>
/// Serviço de API para Tipo de Documento de Fornecedor.
/// Herda implementação base de BaseApiService.
/// ⭐ v6.3: CamelCase + logging detalhado + BackendResult para erros.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona implementações Select2 Lookup automáticas.
/// </summary>
public class GTP_TipoDocumentoFornecedorApiService 
    : BaseApiService<GTP_TipoDocumentoFornecedorDto, CreateGTP_TipoDocumentoFornecedorRequest, UpdateGTP_TipoDocumentoFornecedorRequest, int>,
      IGTP_TipoDocumentoFornecedorApiService
{
    private const string ApiRoute = "api/gestaoterceirosprestadores/gtp_tipodocumentofornecedor";

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
    public GTP_TipoDocumentoFornecedorApiService(
        HttpClient httpClient,
        ILogger<GTP_TipoDocumentoFornecedorApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {
    }

    // =========================================================================
    // IBatchDeleteService Implementation
    // =========================================================================

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<int> ids)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[GTP_TIPODOCUMENTOFORNECEDOR] DELETE {Route}/batch - Body: {Body}", _baseEndpoint, json);
            
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
            _logger.LogError(ex, "[GTP_TIPODOCUMENTOFORNECEDOR] Erro de conexão em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro de conexão com o servidor" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GTP_TIPODOCUMENTOFORNECEDOR] Exceção em DeleteBatchAsync");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = ex.Message }
            };
        }
    }


    #region v4.1 - Toggle Ativo

    public async Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default)
    {
        try
        {
            await AddAuthorizationHeaderAsync();
            
            var payload = new { Ativo = ativo };
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug(
                "[GTP_TIPODOCUMENTOFORNECEDOR] PATCH {Route}/{Id}/toggle-ativo - Body: {Body}",
                _baseEndpoint,
                id,
                json);
            
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseEndpoint}/{id}/toggle-ativo")
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning(
                    "[GTP_TIPODOCUMENTOFORNECEDOR] Erro ao alternar Ativo - Status: {Status}, Content: {Content}",
                    response.StatusCode,
                    errorContent);
                
                throw new HttpRequestException(
                    $"Erro ao alternar status Ativo: {response.StatusCode}");
            }

            _logger.LogInformation(
                "[GTP_TIPODOCUMENTOFORNECEDOR] Status Ativo alterado para {Ativo} - ID: {Id}",
                ativo,
                id);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[GTP_TIPODOCUMENTOFORNECEDOR] Erro de conexão em ToggleAtivoAsync");
            throw new InvalidOperationException("Erro de conexão com o servidor", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GTP_TIPODOCUMENTOFORNECEDOR] Exceção em ToggleAtivoAsync");
            throw;
        }
    }

    #endregion


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
