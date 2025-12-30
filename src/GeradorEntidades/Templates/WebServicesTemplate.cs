// =============================================================================
// GERADOR FULL-STACK v4.1 - WEB SERVICES TEMPLATE (COM TOGGLE ATIVO CORRIGIDO)
// =============================================================================
// Arquivo: GeradorEntidades/Templates/WebServicesTemplate.cs
// Baseado em RhSensoERP.CrudTool v3.0
// =============================================================================
// CHANGELOG:
// v4.1       - ✅ NOVO: Método ToggleAtivoAsync para alternar status Ativo/Desativo
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
/// v4.1: Adiciona método ToggleAtivoAsync para alternar status dinamicamente.
/// v4.0 FINAL: GetPagedAsync com parâmetros corretos (sortBy, desc).
/// </summary>
public static class WebServicesTemplate
{
    /// <summary>
    /// Gera a interface do ApiService.
    /// v4.1: Adiciona ToggleAtivoAsync se entidade tiver campo Ativo.
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
// ARQUIVO GERADO POR GeradorFullStack v4.1
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
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// v4.0 FINAL: Suporta ordenação server-side com sortBy/desc.
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
            FileType = "Service"
        };
    }

    /// <summary>
    /// Gera a implementação do ApiService.
    /// v4.1: Adiciona implementação do ToggleAtivoAsync.
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

        // v4.1: Verifica se tem campo "Ativo" e gera implementação
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase));

        var toggleAtivoImplementation = hasAtivoField ? $@"

    #region v4.1 - Toggle Ativo

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// Chamada via PATCH ou PUT para /api/{{route}}/{{id}}/toggle-ativo
    /// </summary>
    public async Task ToggleAtivoAsync({pkType} id, bool ativo, CancellationToken ct = default)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            
            // Payload simples com o novo valor de Ativo
            var payload = new {{ Ativo = ativo }};
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");
            
            _logger.LogDebug(
                ""[{entityUpper}] PATCH {{{{Route}}}}/{{{{Id}}}}/toggle-ativo - Body: {{{{Body}}"",
                ApiRoute,
                id,
                json);
            
            // Tenta PATCH primeiro (mais semântico)
            var request = new HttpRequestMessage(HttpMethod.Patch, $""{{ApiRoute}}/{{id}}/toggle-ativo"")
            {{
                Content = content
            }};

            var response = await _httpClient.SendAsync(request, ct);
            
            if (!response.IsSuccessStatusCode)
            {{
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning(
                    ""[{entityUpper}] Erro ao alternar Ativo - Status: {{{{Status}}}}, Content: {{{{Content}}"",
                    response.StatusCode,
                    errorContent);
                
                throw new HttpRequestException(
                    $""Erro ao alternar status Ativo: {{response.StatusCode}}"");
            }}

            _logger.LogInformation(
                ""[{entityUpper}] Status Ativo alterado para {{{{Ativo}}}} - ID: {{{{Id}}"",
                ativo,
                id);
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

    #endregion" : "";

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.1
// Entity: {entity.Name}
// Module: {entity.Module}
// ApiRoute: {apiRoute}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// v4.1: Adiciona ToggleAtivoAsync para alternar status Ativo/Desativo
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
/// v4.1: Implementa ToggleAtivoAsync para alternar status dinamicamente.
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
        
        _logger.LogDebug(""[{entityUpper}] {{{{Op}}}} - Status: {{{{Status}}}}, Content: {{{{Content}}"", 
            operation, response.StatusCode, content);

        if (string.IsNullOrEmpty(content))
            return Fail<T>($""Resposta vazia do servidor ({{{{(int)response.StatusCode}}}})"")
        try
        {{
            var backendResult = JsonSerializer.Deserialize<BackendResult<T>>(content, _jsonOptions);
            
            if (backendResult == null)
                return Fail<T>(""Falha ao desserializar resposta do backend"");

            if (backendResult.IsSuccess)
            {{
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                    return Success(data);
            }}
            
            var errorMsg = backendResult.Error?.Message ?? $""Erro desconhecido ({{{{(int)response.StatusCode}}}})"";
            return Fail<T>(errorMsg);
        }}
        catch (JsonException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro ao desserializar resposta"");
            return Fail<T>($""Erro ao processar resposta: {{{{ex.Message}}}}"");
        }}
    }}

    #endregion

    #region IApiService Implementation

    public async Task<ApiResponse<PagedResult<{entity.Name}Dto>>> GetPagedAsync(
        int page = 1, 
        int pageSize = 10, 
        string searchTerm = """", 
        string sortBy = """", 
        bool desc = false)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            
            // ✅ v4.0 FINAL: Parâmetros corretos (sortBy, desc)
            var url = $""{{{{ApiRoute}}}}/paged?page={{{{page}}}}&pageSize={{{{pageSize}}}}&searchTerm={{{{Uri.EscapeDataString(searchTerm)}}}}"";
            
            if (!string.IsNullOrEmpty(sortBy))
            {{
                url += $""&sortBy={{{{sortBy}}}}&desc={{{{desc}}}}"";
            }}

            _logger.LogDebug(""[{entityUpper}] GET {{{{Url}}"", url);
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<PagedResult<{entity.Name}Dto>>(response, ""GetPaged"");

            var responseJson = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<PagedResult<{entity.Name}Dto>>>(
                responseJson, _jsonOptions);

            if (backendResult?.IsSuccess == true)
            {{
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                    return Success(data);
            }}

            return Fail<PagedResult<{entity.Name}Dto>>(
                backendResult?.Error?.Message ?? ""Erro ao buscar dados paginados"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em GetPagedAsync"");
            return Fail<PagedResult<{entity.Name}Dto>>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetPagedAsync"");
            return Fail<PagedResult<{entity.Name}Dto>>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<List<{entity.Name}Dto>>> GetAllAsync()
    {{
        try
        {{
            await SetAuthHeaderAsync();
            
            var url = $""{{{{ApiRoute}}}}"";
            _logger.LogDebug(""[{entityUpper}] GET {{{{Url}}"", url);
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<List<{entity.Name}Dto>>(response, ""GetAll"");

            var responseJson = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<List<{entity.Name}Dto>>>(
                responseJson, _jsonOptions);

            if (backendResult?.IsSuccess == true)
            {{
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                    return Success(data);
            }}

            return Fail<List<{entity.Name}Dto>>(
                backendResult?.Error?.Message ?? ""Erro ao buscar todos os registros"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em GetAllAsync"");
            return Fail<List<{entity.Name}Dto>>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetAllAsync"");
            return Fail<List<{entity.Name}Dto>>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<{entity.Name}Dto>> GetByIdAsync({pkType} id)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($""{{{{ApiRoute}}}}/{{{{id}}}}"");
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<{entity.Name}Dto>(response, ""GetById"");

            var responseJson = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<{entity.Name}Dto>>(responseJson, _jsonOptions);

            if (backendResult?.IsSuccess == true)
            {{
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                    return Success(data);
            }}

            return Fail<{entity.Name}Dto>(backendResult?.Error?.Message ?? ""Registro não encontrado"");
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
            
            _logger.LogDebug(""[{entityUpper}] POST {{{{Route}}}} - Body: {{{{Body}}"", ApiRoute, json);
            
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
            
            _logger.LogDebug(""[{entityUpper}] PUT {{{{Route}}}}/{{{{Id}}}} - Body: {{{{Body}}"", ApiRoute, id, json);
            
            var response = await _httpClient.PutAsync($""{{{{ApiRoute}}}}/{{{{id}}}}"", content);
            
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
            var response = await _httpClient.DeleteAsync($""{{{{ApiRoute}}}}/{{{{id}}}}"");
            
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
            
            _logger.LogDebug(""[{entityUpper}] DELETE {{{{Route}}}}/batch - Body: {{{{Body}}"", ApiRoute, json);
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $""{{{{ApiRoute}}}}/batch"")
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
{toggleAtivoImplementation}
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