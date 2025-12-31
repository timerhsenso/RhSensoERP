// =============================================================================
// GERADOR FULL-STACK v5.2 - WEB SERVICES TEMPLATE (LOGS CORRIGIDOS - FINAL)
// =============================================================================
// Arquivo: GeradorEntidades/Templates/WebServicesTemplate.cs
// Baseado em RhSensoERP.CrudTool v3.0
// =============================================================================
// CHANGELOG:
// v5.2       - ✅ CRÍTICO: Logs corrigidos usando concatenação (não interpolação)
//            - ✅ Resolve FormatException "Input string was not in a correct format"
//            - ✅ Gera {Route} corretamente (não {{Route}})
// v5.1       - ❌ DESCARTADO: Tentativa com {{Route}} quebrou o template
// v5.0       - ✅ CORRIGIDO: Herança genérica correta de BaseApiService
//            - ✅ CORRIGIDO: Construtor usa IHttpContextAccessor
// v4.1       - ✅ NOVO: Método ToggleAtivoAsync para alternar status Ativo/Desativo
// =============================================================================

using GeradorEntidades.Models;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Services que herdam de BaseApiService e implementam IBatchDeleteService.
/// v5.2: Logs corrigidos - gera {Route} não {{Route}}.
/// </summary>
public static class WebServicesTemplate
{
    /// <summary>
    /// Gera a interface do ApiService.
    /// </summary>
    public static GeneratedFile GenerateInterface(EntityConfig entity)
    {
        var pkType = entity.PkTypeSimple;
        var modulePath = GetModulePath(entity.Module);

        // v4.1: Verifica se tem campo "Ativo"
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase));

        var toggleAtivoMethod = hasAtivoField
            ? $@"

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync({pkType} id, bool ativo, CancellationToken ct = default);"
            : "";

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: {entity.Name}
// Module: {entity.Module}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.{entity.Module}.{entity.Name};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{entity.Module}.{entity.Name};

/// <summary>
/// Interface do serviço de API para {entity.DisplayName}.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v5.2: Compatível com BaseApiService genérico.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// </summary>
public interface I{entity.Name}ApiService 
    : IApiService<{entity.Name}Dto, Create{entity.Name}Request, Update{entity.Name}Request, {pkType}>,
      IBatchDeleteService<{pkType}>
{{{toggleAtivoMethod}
}}
";

        return new GeneratedFile
        {
            FileName = $"I{entity.Name}ApiService.cs",
            RelativePath = $"Web/Services/{modulePath}/{entity.Name}/I{entity.Name}ApiService.cs",
            Content = content,
            FileType = "CSharp"
        };
    }

    /// <summary>
    /// Gera a implementação do ApiService.
    /// v5.2: Logs corrigidos usando concatenação.
    /// </summary>
    public static GeneratedFile GenerateImplementation(EntityConfig entity)
    {
        var pkType = entity.PkTypeSimple;
        var modulePath = GetModulePath(entity.Module);
        var apiRoute = entity.ApiRoute?.TrimStart('/') ?? $"api/{modulePath.ToLowerInvariant()}/{entity.NameLower}";
        var entityUpper = entity.Name.ToUpperInvariant();

        // v4.1: Verifica se tem campo "Ativo"
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase));

        // v5.2: Gera método ToggleAtivo com logs corretos
        var toggleAtivoImplementation = hasAtivoField
            ? GenerateToggleAtivoMethod(entity, entityUpper, pkType)
            : "";

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: {entity.Name}
// Module: {entity.Module}
// ApiRoute: {apiRoute}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// v5.2: Logs corrigidos - gera {{Route}} não {{{{Route}}}}
// v5.0: Herança genérica correta de BaseApiService + IHttpContextAccessor
// v4.1: Adiciona ToggleAtivoAsync para alternar status Ativo/Desativo
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.{entity.Module}.{entity.Name};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{entity.Module}.{entity.Name};

/// <summary>
/// Serviço de API para {entity.DisplayName}.
/// Herda implementação base de BaseApiService (GetPaged, GetAll, GetById, Create, Update, Delete).
/// v5.2: Logs corrigidos.
/// v5.0: Herança genérica correta + construtor compatível com BaseApiService.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// AUTO-REGISTRADO: Compatível com AddCrudToolServicesAutomatically().
/// </summary>
public class {entity.Name}ApiService 
    : BaseApiService<{entity.Name}Dto, Create{entity.Name}Request, Update{entity.Name}Request, {pkType}>,
      I{entity.Name}ApiService
{{
    private const string ApiRoute = ""{apiRoute}"";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {{
        PropertyNameCaseInsensitive = true
    }};

    // =========================================================================
    // ✅ v5.0: CONSTRUTOR CORRIGIDO - USA IHttpContextAccessor
    // =========================================================================
    public {entity.Name}ApiService(
        HttpClient httpClient,
        ILogger<{entity.Name}ApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {{
        // ✅ Construtor base recebe: httpClient, logger, httpContextAccessor, baseEndpoint
        // ✅ BaseApiService já implementa: GetPagedAsync, GetAllAsync, GetByIdAsync, 
        //    CreateAsync, UpdateAsync, DeleteAsync, DeleteMultipleAsync
        // ✅ Autenticação (JWT via Cookie) já é tratada por AddAuthorizationHeaderAsync() do base
    }}

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
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<{pkType}> ids)
    {{
        try
        {{
            await AddAuthorizationHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug(""[{entityUpper}] DELETE {{Route}}/batch - Body: {{Body}}"", _baseEndpoint, json);
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $""{{_baseEndpoint}}/batch"")
            {{
                Content = new StringContent(json, Encoding.UTF8, ""application/json"")
            }};

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<BackendBatchDeleteResult>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {{
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                {{
                    var dto = new BatchDeleteResultDto
                    {{
                        SuccessCount = data.SuccessCount,
                        FailureCount = data.FailureCount,
                        Errors = data.Errors?.Select(e => new BatchDeleteErrorDto
                        {{
                            Code = e.Id ?? e.Code ?? string.Empty,
                            Message = e.Message ?? string.Empty
                        }}).ToList() ?? []
                    }};
                    
                    return new ApiResponse<BatchDeleteResultDto>
                    {{
                        Success = true,
                        Data = dto
                    }};
                }}
            }}
            
            return new ApiResponse<BatchDeleteResultDto>
            {{
                Success = false,
                Error = new ApiError {{ Message = backendResult?.Error?.Message ?? ""Erro ao excluir em lote"" }}
            }};
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em DeleteBatchAsync"");
            return new ApiResponse<BatchDeleteResultDto>
            {{
                Success = false,
                Error = new ApiError {{ Message = ""Erro de conexão com o servidor"" }}
            }};
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteBatchAsync"");
            return new ApiResponse<BatchDeleteResultDto>
            {{
                Success = false,
                Error = new ApiError {{ Message = ex.Message }}
            }};
        }}
    }}

{toggleAtivoImplementation}
    // =========================================================================
    // Backend DTOs (para DeleteBatchAsync)
    // =========================================================================
    private sealed class BackendResult<T>
    {{
        public bool IsSuccess {{ get; set; }}
        public T? Value {{ get; set; }}
        public T? Data {{ get; set; }}
        public BackendError? Error {{ get; set; }}
    }}

    private sealed class BackendError
    {{
        public string? Code {{ get; set; }}
        public string? Message {{ get; set; }}
    }}

    private sealed class BackendBatchDeleteResult
    {{
        public int SuccessCount {{ get; set; }}
        public int FailureCount {{ get; set; }}
        public List<BackendBatchDeleteError>? Errors {{ get; set; }}
    }}

    private sealed class BackendBatchDeleteError
    {{
        public string? Id {{ get; set; }}
        public string? Code {{ get; set; }}
        public string? Message {{ get; set; }}
    }}
}}
";

        return new GeneratedFile
        {
            FileName = $"{entity.Name}ApiService.cs",
            RelativePath = $"Web/Services/{modulePath}/{entity.Name}/{entity.Name}ApiService.cs",
            Content = content,
            FileType = "CSharp"
        };
    }

    /// <summary>
    /// v5.2: Gera método ToggleAtivo com logs CORRETOS usando concatenação.
    /// </summary>
    private static string GenerateToggleAtivoMethod(EntityConfig entity, string entityUpper, string pkType)
    {
        // ✅ v5.2: Usa concatenação para evitar conflito de interpolação
        var logDebugLine = @"_logger.LogDebug(
                ""[" + entityUpper + @"] PATCH {Route}/{Id}/toggle-ativo - Body: {Body}"",
                _baseEndpoint,
                id,
                json);";

        var logWarningLine = @"_logger.LogWarning(
                    ""[" + entityUpper + @"] Erro ao alternar Ativo - Status: {Status}, Content: {Content}"",
                    response.StatusCode,
                    errorContent);";

        var logInformationLine = @"_logger.LogInformation(
                ""[" + entityUpper + @"] Status Ativo alterado para {Ativo} - ID: {Id}"",
                ativo,
                id);";

        return $@"
    #region v4.1 - Toggle Ativo

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// Chamada via PATCH para /api/{{route}}/{{id}}/toggle-ativo
    /// </summary>
    public async Task ToggleAtivoAsync({pkType} id, bool ativo, CancellationToken ct = default)
    {{
        try
        {{
            await AddAuthorizationHeaderAsync();
            
            // Payload simples com o novo valor de Ativo
            var payload = new {{ Ativo = ativo }};
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            {logDebugLine}
            
            // Tenta PATCH primeiro (mais semântico)
            var request = new HttpRequestMessage(HttpMethod.Patch, $""{{_baseEndpoint}}/{{id}}/toggle-ativo"")
            {{
                Content = content
            }};

            var response = await _httpClient.SendAsync(request, ct);
            
            if (!response.IsSuccessStatusCode)
            {{
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                {logWarningLine}
                
                throw new HttpRequestException(
                    $""Erro ao alternar status Ativo: {{response.StatusCode}}"");
            }}

            {logInformationLine}
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em ToggleAtivoAsync"");
            throw new InvalidOperationException(""Erro de conexão com o servidor"", ex);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em ToggleAtivoAsync"");
            throw;
        }}
    }}

    #endregion";
    }

    /// <summary>
    /// Converte nome do módulo para path de pasta.
    /// </summary>
    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "Common";

        return moduleName;
    }
}