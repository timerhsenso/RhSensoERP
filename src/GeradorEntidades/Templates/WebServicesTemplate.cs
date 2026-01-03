// =============================================================================
// GERADOR FULL-STACK v6.1 - WEB SERVICES TEMPLATE
// ⭐ v6.1 - CORRIGIDO: Lookup usa 'term' para compatibilidade Select2
// ⭐ v6.0 - SELECT2 LOOKUP AUTOMÁTICO
// v5.2 - Logs corrigidos
// v5.0 - Herança genérica correta de BaseApiService
// v4.1 - Método ToggleAtivoAsync
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Services que herdam de BaseApiService e implementam IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa parâmetro 'term' em vez de 'search'.
/// v6.0: Geração automática de métodos Select2 Lookup.
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

        // ⭐ v6.0: Gera métodos Select2
        var select2Methods = GenerateSelect2Methods(entity);

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
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
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface I{entity.Name}ApiService 
    : IApiService<{entity.Name}Dto, Create{entity.Name}Request, Update{entity.Name}Request, {pkType}>,
      IBatchDeleteService<{pkType}>
{{{toggleAtivoMethod}
{select2Methods}
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

        var toggleAtivoImplementation = hasAtivoField
            ? GenerateToggleAtivoMethod(entity, entityUpper, pkType)
            : "";

        // ⭐ v6.0: Gera implementações Select2
        var select2Implementations = GenerateSelect2Implementations(entity);

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: {entity.Name}
// Module: {entity.Module}
// ApiRoute: {apiRoute}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.{entity.Module}.{entity.Name};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{entity.Module}.{entity.Name};

/// <summary>
/// Serviço de API para {entity.DisplayName}.
/// Herda implementação base de BaseApiService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona implementações Select2 Lookup automáticas.
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
    // CONSTRUTOR
    // =========================================================================
    public {entity.Name}ApiService(
        HttpClient httpClient,
        ILogger<{entity.Name}ApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {{
    }}

    // =========================================================================
    // IBatchDeleteService Implementation
    // =========================================================================

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
{select2Implementations}

    // =========================================================================
    // Backend DTOs
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
    /// v5.2: Gera método ToggleAtivo.
    /// </summary>
    private static string GenerateToggleAtivoMethod(EntityConfig entity, string entityUpper, string pkType)
    {
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

    public async Task ToggleAtivoAsync({pkType} id, bool ativo, CancellationToken ct = default)
    {{
        try
        {{
            await AddAuthorizationHeaderAsync();
            
            var payload = new {{ Ativo = ativo }};
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            {logDebugLine}
            
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

    // =========================================================================
    // ⭐ v6.0: SELECT2 LOOKUP - GERAÇÃO AUTOMÁTICA
    // =========================================================================

    /// <summary>
    /// ⭐ v6.0: Gera declarações de métodos Select2 para a Interface.
    /// </summary>
    private static string GenerateSelect2Methods(EntityConfig entity)
    {
        if (!entity.Select2Lookups.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // ⭐ v6.0: MÉTODOS DE LOOKUP PARA SELECT2 (GERADO AUTOMATICAMENTE)");
        sb.AppendLine("    // =========================================================================");

        foreach (var lookup in entity.Select2Lookups)
        {
            sb.AppendLine($@"
    /// <summary>
    /// Busca {lookup.DisplayName} para Select2.
    /// Gerado automaticamente para o campo {lookup.PropertyName}.
    /// </summary>
    Task<object> {lookup.MethodName}Async(
        string term, 
        int page, 
        int pageSize, 
        CancellationToken ct = default);");
        }

        return sb.ToString();
    }

    /// <summary>
    /// ⭐ v6.1: CORRIGIDO - Gera implementações dos métodos Select2 usando 'term'.
    /// </summary>
    private static string GenerateSelect2Implementations(EntityConfig entity)
    {
        if (!entity.Select2Lookups.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // ⭐ v6.1: IMPLEMENTAÇÕES DE LOOKUP PARA SELECT2 (CORRIGIDO - USA 'term')");
        sb.AppendLine("    // =========================================================================");

        foreach (var lookup in entity.Select2Lookups)
        {
            var entityUpper = lookup.EntityName.ToUpperInvariant();

            sb.AppendLine($@"
    /// <summary>
    /// Busca {lookup.DisplayName} para componentes de seleção (Lookup).
    /// </summary>
    public async Task<object> {lookup.MethodName}Async(
        string term, 
        int page, 
        int pageSize, 
        CancellationToken ct = default)
    {{
        try
        {{
            await AddAuthorizationHeaderAsync();
            
            // ✅ v6.1: CORRIGIDO - USA 'term' em vez de 'search'
            var url = $""{lookup.ApiRoute}/lookup?term={{Uri.EscapeDataString(term ?? """")}}& page={{page}}&pageSize={{pageSize}}"";
            
            var response = await _httpClient.GetAsync(url, ct);
            
            if (!response.IsSuccessStatusCode)
            {{
                _logger.LogError(""[{entityUpper}] Erro ao buscar para Select2: {{Status}}"", response.StatusCode);
                return new {{ results = new List<object>(), pagination = new {{ more = false }} }};
            }}
            
            var content = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<object>(content, _jsonOptions) 
                ?? new {{ results = new List<object>(), pagination = new {{ more = false }} }};
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro em {lookup.MethodName}Async"");
            return new {{ results = new List<object>(), pagination = new {{ more = false }} }};
        }}
    }}");
        }

        return sb.ToString();
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