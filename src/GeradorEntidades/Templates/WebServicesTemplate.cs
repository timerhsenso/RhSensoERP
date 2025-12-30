// =============================================================================
// GERADOR FULL-STACK v4.0 FINAL - WEB SERVICES TEMPLATE (COM ORDENAÇÃO CORRETA)
// =============================================================================
// Arquivo: GeradorEntidades/Templates/WebServicesTemplate.cs
// Baseado em RhSensoERP.CrudTool v3.0
// =============================================================================
// CHANGELOG:
// v4.0 FINAL - ✅ CORRIGIDO: orderBy → sortBy, ascending → desc
//              ✅ Compatível com backend PagedRequest (SortBy, Desc)
// v4.0       - ✅ ADICIONADO: GetPagedAsync com 5 parâmetros (orderBy, ascending) [BUGADO]
// v3.3       - Corrigido CS0019: operador ?? não pode ser usado com int
// v3.2       - Usa ApiRoute do manifesto (não mais construída automaticamente)
// v3.1       - Versão inicial
// =============================================================================

using GeradorEntidades.Models;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Services que implementam IApiService e IBatchDeleteService existentes.
/// v4.0 FINAL: GetPagedAsync com parâmetros corretos (sortBy, desc).
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

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0 FINAL
// Entity: {entity.Name}
// Module: {entity.Module}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.{modulePath}.{entity.Name};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{modulePath}.{entity.Name};

/// <summary>
/// Interface do serviço de API para {entity.DisplayName}.
/// Herda de IApiService e IBatchDeleteService existentes.
/// v4.0 FINAL: Suporta ordenação server-side com sortBy/desc.
/// </summary>
public interface I{entity.Name}ApiService 
    : IApiService<{entity.Name}Dto, Create{entity.Name}Request, Update{entity.Name}Request, {pkType}>,
      IBatchDeleteService<{pkType}>
{{
}}
";

        return new GeneratedFile
        {
            FileName = $"I{entity.Name}ApiService.cs",
            RelativePath = $"Web/Services/{modulePath}/{entity.Name}/I{entity.Name}ApiService.cs",
            Content = content,
            FileType = "Service"
        };
    }

    /// <summary>
    /// Gera a implementação do ApiService.
    /// v4.0 FINAL: GetPagedAsync com parâmetros corretos (sortBy, desc).
    /// </summary>
    public static GeneratedFile GenerateImplementation(EntityConfig entity)
    {
        var pkType = entity.PkTypeSimple;
        var modulePath = GetModulePath(entity.Module);
        var entityUpper = entity.Name.ToUpper();
        var isGuidPk = pkType.Equals("Guid", StringComparison.OrdinalIgnoreCase);
        var isStringPk = pkType.Equals("string", StringComparison.OrdinalIgnoreCase);
        var isLongPk = pkType.Equals("long", StringComparison.OrdinalIgnoreCase);

        // Lógica de extração de ID criado
        string extractIdExpression;
        string idNotEmptyCheck;

        if (isGuidPk)
        {
            extractIdExpression = "createResult.Value != Guid.Empty ? createResult.Value : createResult.Data";
            idNotEmptyCheck = "createdId != Guid.Empty";
        }
        else if (isStringPk)
        {
            extractIdExpression = "!string.IsNullOrEmpty(createResult.Value) ? createResult.Value : createResult.Data ?? string.Empty";
            idNotEmptyCheck = "!string.IsNullOrEmpty(createdId)";
        }
        else if (isLongPk)
        {
            extractIdExpression = "createResult.Value != 0 ? createResult.Value : createResult.Data";
            idNotEmptyCheck = "createdId != 0";
        }
        else // int (default)
        {
            extractIdExpression = "createResult.Value != 0 ? createResult.Value : createResult.Data";
            idNotEmptyCheck = "createdId != 0";
        }

        var apiRoute = entity.ApiRoute;

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0 FINAL
// Entity: {entity.Name}
// Module: {entity.Module}
// ApiRoute: {apiRoute}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// v4.0 FINAL: Parâmetros corretos (sortBy, desc) para ordenação server-side
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using RhSensoERP.Web.Models.{modulePath}.{entity.Name};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{modulePath}.{entity.Name};

/// <summary>
/// Implementação do serviço de API para {entity.DisplayName}.
/// v4.0 FINAL: Corrigido para usar sortBy e desc (compatível com backend).
/// </summary>
public class {entity.Name}ApiService : I{entity.Name}ApiService
{{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<{entity.Name}ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = ""{apiRoute}"";

    public {entity.Name}ApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<{entity.Name}ApiService> logger)
    {{
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {{
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }};
    }}

    #region Private Helpers

    private async Task SetAuthHeaderAsync()
    {{
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {{
            var token = await context.GetTokenAsync(""access_token"");
            
            if (!string.IsNullOrEmpty(token))
            {{
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue(""Bearer"", token);
                _logger.LogDebug(""[{entityUpper}] Token JWT configurado"");
            }}
            else
            {{
                _logger.LogWarning(""[{entityUpper}] Token JWT não encontrado"");
            }}
        }}
        else
        {{
            _logger.LogWarning(""[{entityUpper}] Usuário não autenticado"");
        }}
    }}

    private static ApiResponse<T> Success<T>(T? data) => new()
    {{
        Success = true,
        Data = data
    }};

    private static ApiResponse<T> Fail<T>(string message) => new()
    {{
        Success = false,
        Error = new ApiError {{ Message = message }}
    }};

    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(
        HttpResponseMessage response, 
        string operation)
    {{
        var content = await response.Content.ReadAsStringAsync();
        
        _logger.LogDebug(""[{entityUpper}] {{Op}} - Status: {{Status}}, Content: {{Content}}"", 
            operation, response.StatusCode, content);

        if (string.IsNullOrEmpty(content))
            return Fail<T>($""Resposta vazia do servidor ({{(int)response.StatusCode}})"");

        try
        {{
            var backendResult = JsonSerializer.Deserialize<BackendResult<T>>(content, _jsonOptions);
            
            if (backendResult == null)
                return Fail<T>(""Resposta inválida do servidor"");

            if (backendResult.IsSuccess)
            {{
                var data = backendResult.Value ?? backendResult.Data;
                return Success(data);
            }}

            return Fail<T>(backendResult.Error?.Message ?? ""Erro desconhecido"");
        }}
        catch (JsonException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro JSON em {{Op}}"", operation);
            return Fail<T>(""Erro ao processar resposta do servidor"");
        }}
    }}

    #endregion

    #region IApiService Implementation

    /// <summary>
    /// ✅ v4.0 FINAL: GetPagedAsync com parâmetros corretos (sortBy, desc).
    /// </summary>
    public async Task<ApiResponse<PagedResult<{entity.Name}Dto>>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null,
        string? sortBy = null,      // ✅ CORRETO: sortBy (não orderBy)
        bool desc = false)          // ✅ CORRETO: desc (não ascending)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            
            // ================================================================
            // ✅ CONSTRÓI QUERY STRING COM PARÂMETROS CORRETOS
            // ================================================================
            var query = $""?page={{page}}&pageSize={{pageSize}}"";
            
            if (!string.IsNullOrWhiteSpace(search))
                query += $""&search={{Uri.EscapeDataString(search)}}"";
            
            // ✅ CORRETO: sortBy (não orderBy)
            if (!string.IsNullOrWhiteSpace(sortBy))
                query += $""&sortBy={{Uri.EscapeDataString(sortBy)}}"";
            
            // ✅ CORRETO: desc (não ascending)
            query += $""&desc={{desc.ToString().ToLower()}}"";

            _logger.LogDebug(
                ""[{entityUpper}] GET {{Route}}{{Query}} - SortBy: {{SortBy}}, Desc: {{Desc}}"", 
                ApiRoute, query, sortBy ?? ""null"", desc
            );

            var response = await _httpClient.GetAsync($""{{ApiRoute}}{{query}}"");
            return await ProcessResponseAsync<PagedResult<{entity.Name}Dto>>(response, ""GetPaged"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em GetPagedAsync"");
            return Fail<PagedResult<{entity.Name}Dto>>(""Erro de conexão com o servidor"");
        }}
        catch (TaskCanceledException)
        {{
            return Fail<PagedResult<{entity.Name}Dto>>(""Tempo limite excedido"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetPagedAsync"");
            return Fail<PagedResult<{entity.Name}Dto>>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<IEnumerable<{entity.Name}Dto>>> GetAllAsync()
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($""{{ApiRoute}}?page=1&pageSize=10000"");
            var result = await ProcessResponseAsync<PagedResult<{entity.Name}Dto>>(response, ""GetAll"");
            
            if (result.Success && result.Data != null)
                return Success<IEnumerable<{entity.Name}Dto>>(result.Data.Items);
            
            return Fail<IEnumerable<{entity.Name}Dto>>(result.Error?.Message ?? ""Erro ao buscar dados"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetAllAsync"");
            return Fail<IEnumerable<{entity.Name}Dto>>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<{entity.Name}Dto>> GetByIdAsync({pkType} id)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($""{{ApiRoute}}/{{id}}"");
            return await ProcessResponseAsync<{entity.Name}Dto>(response, ""GetById"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em GetByIdAsync"");
            return Fail<{entity.Name}Dto>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetByIdAsync"");
            return Fail<{entity.Name}Dto>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<{entity.Name}Dto>> CreateAsync(Create{entity.Name}Request request)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            _logger.LogDebug(""[{entityUpper}] POST {{Route}} - Body: {{Body}}"", ApiRoute, json);
            
            var response = await _httpClient.PostAsync(ApiRoute, content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<{entity.Name}Dto>(response, ""Create"");

            // Backend retorna Result<{pkType}> com o ID criado
            var responseJson = await response.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<BackendResult<{pkType}>>(responseJson, _jsonOptions);
            
            if (createResult?.IsSuccess == true)
            {{
                var createdId = {extractIdExpression};
                    
                if ({idNotEmptyCheck})
                    return await GetByIdAsync(createdId);
            }}

            return Fail<{entity.Name}Dto>(createResult?.Error?.Message ?? ""Erro ao criar registro"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em CreateAsync"");
            return Fail<{entity.Name}Dto>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em CreateAsync"");
            return Fail<{entity.Name}Dto>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<{entity.Name}Dto>> UpdateAsync({pkType} id, Update{entity.Name}Request request)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            _logger.LogDebug(""[{entityUpper}] PUT {{Route}}/{{Id}} - Body: {{Body}}"", ApiRoute, id, json);
            
            var response = await _httpClient.PutAsync($""{{ApiRoute}}/{{id}}"", content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<{entity.Name}Dto>(response, ""Update"");

            return await GetByIdAsync(id);
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em UpdateAsync"");
            return Fail<{entity.Name}Dto>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em UpdateAsync"");
            return Fail<{entity.Name}Dto>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<bool>> DeleteAsync({pkType} id)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($""{{ApiRoute}}/{{id}}"");
            
            if (response.IsSuccessStatusCode)
                return Success(true);
            
            return await ProcessResponseAsync<bool>(response, ""Delete"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em DeleteAsync"");
            return Fail<bool>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteAsync"");
            return Fail<bool>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<{pkType}> ids)
    {{
        try
        {{
            var result = await DeleteBatchAsync(ids);
            
            if (result.Success && result.Data != null)
                return Success(result.Data.FailureCount == 0);
            
            return Fail<bool>(result.Error?.Message ?? ""Erro ao excluir registros"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteMultipleAsync"");
            return Fail<bool>(ex.Message);
        }}
    }}

    #endregion

    #region IBatchDeleteService Implementation

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<{pkType}> ids)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug(""[{entityUpper}] DELETE {{Route}}/batch - Body: {{Body}}"", ApiRoute, json);
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $""{{ApiRoute}}/batch"")
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
                    
                    return Success(dto);
                }}
            }}
            
            return Fail<BatchDeleteResultDto>(backendResult?.Error?.Message ?? ""Erro ao excluir em lote"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em DeleteBatchAsync"");
            return Fail<BatchDeleteResultDto>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteBatchAsync"");
            return Fail<BatchDeleteResultDto>(ex.Message);
        }}
    }}

    #endregion

    #region Backend DTOs

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

    #endregion
}}
";

        return new GeneratedFile
        {
            FileName = $"{entity.Name}ApiService.cs",
            RelativePath = $"Web/Services/{modulePath}/{entity.Name}/{entity.Name}ApiService.cs",
            Content = content,
            FileType = "Service"
        };
    }

    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "Common";

        return moduleName;
    }
}