// =============================================================================
// GERADOR FULL-STACK v5.2 - WEB SERVICES TEMPLATE (ADAPTED FOR SOURCE GENERATOR)
// =============================================================================
// Arquivo: src/Generators/Templates/WebServicesTemplate.cs
// Baseado em RhSensoERP.CrudTool v3.0
// =============================================================================

using RhSensoERP.Generators.Models;
using System;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Gera Services que herdam de BaseApiService e implementam IBatchDeleteService.
/// Adaptado para Source Generator (retorna string).
/// </summary>
public static class WebServicesTemplate
{
    /// <summary>
    /// Gera a interface do ApiService.
    /// </summary>
    public static string GenerateInterface(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var modulePath = GetModulePath(info.ModuleName);

        // Verifica se tem campo "Ativo"
        var hasAtivoField = info.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

        var toggleAtivoMethod = hasAtivoField
            ? $@"
    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync({pkType} id, bool ativo, CancellationToken ct = default);"
            : "";

        return $@"// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE PELO SOURCE GENERATOR
// Entity: {info.EntityName}
// Module: {info.ModuleName}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using RhSensoERP.Web.Models.{info.ModuleName}.{info.EntityName};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{info.ModuleName}.{info.EntityName};

/// <summary>
/// Interface do serviço de API para {info.DisplayName}.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// </summary>
public interface I{info.EntityName}ApiService 
    : IApiService<{info.EntityName}Dto, Create{info.EntityName}Request, Update{info.EntityName}Request, {pkType}>,
      IBatchDeleteService<{pkType}>
{{{toggleAtivoMethod}
}}
";
    }

    /// <summary>
    /// Gera a implementação do ApiService.
    /// </summary>
    public static string GenerateImplementation(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var modulePath = GetModulePath(info.ModuleName);
        var apiRoute = info.ApiRoute?.TrimStart('/') ?? $"api/{modulePath.ToLowerInvariant()}/{info.EntityName.ToLowerInvariant()}";
        var entityUpper = info.EntityName.ToUpperInvariant();

        // Verifica se tem campo "Ativo"
        var hasAtivoField = info.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

        var toggleAtivoImplementation = hasAtivoField
            ? GenerateToggleAtivoMethod(info, entityUpper, pkType)
            : "";

        return $@"// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE PELO SOURCE GENERATOR
// Entity: {info.EntityName}
// Module: {info.ModuleName}
// ApiRoute: {apiRoute}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.{info.ModuleName}.{info.EntityName};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{info.ModuleName}.{info.EntityName};

/// <summary>
/// Serviço de API para {info.DisplayName}.
/// Herda implementação base de BaseApiService (GetPaged, GetAll, GetById, Create, Update, Delete).
/// </summary>
public class {info.EntityName}ApiService 
    : BaseApiService<{info.EntityName}Dto, Create{info.EntityName}Request, Update{info.EntityName}Request, {pkType}>,
      I{info.EntityName}ApiService
{{
    private const string ApiRoute = ""{apiRoute}"";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {{
        PropertyNameCaseInsensitive = true
    }};

    public {info.EntityName}ApiService(
        HttpClient httpClient,
        ILogger<{info.EntityName}ApiService> logger,
        IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, logger, httpContextAccessor, ApiRoute)
    {{
    }}

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
    }

    private static string GenerateToggleAtivoMethod(EntityInfo info, string entityUpper, string pkType)
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
    #region Toggle Ativo

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// Chamada via PATCH para /api/{{route}}/{{id}}/toggle-ativo
    /// </summary>
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

    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "Common";

        return moduleName;
    }
}