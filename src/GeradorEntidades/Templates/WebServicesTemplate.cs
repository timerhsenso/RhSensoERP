// =============================================================================
// GERADOR FULL-STACK v3.3 - WEB SERVICES TEMPLATE
// =============================================================================
// Arquivo: GeradorEntidades/Templates/WebServicesTemplate.cs
// Baseado em RhSensoERP.CrudTool v3.0
// =============================================================================
// CHANGELOG:
// v3.3 - Corrigido CS0019: operador ?? não pode ser usado com int
//        Agora usa operador ternário em vez de coalescência
// v3.2 - Usa ApiRoute do manifesto (não mais construída automaticamente)
// v3.1 - Versão inicial
// =============================================================================

using GeradorEntidades.Models;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Services que implementam IApiService e IBatchDeleteService existentes.
/// v3.3: Corrigido bug do ?? com value types (int, Guid).
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
// ARQUIVO GERADO POR GeradorFullStack v3.3
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
    /// v3.3: Corrigido bug CS0019 - usa ternário em vez de ?? para value types.
    /// </summary>
    public static GeneratedFile GenerateImplementation(EntityConfig entity)
    {
        var pkType = entity.PkTypeSimple;
        var modulePath = GetModulePath(entity.Module);
        var entityUpper = entity.Name.ToUpper();
        var isGuidPk = pkType.Equals("Guid", StringComparison.OrdinalIgnoreCase);
        var isStringPk = pkType.Equals("string", StringComparison.OrdinalIgnoreCase);
        var isLongPk = pkType.Equals("long", StringComparison.OrdinalIgnoreCase);

        // =====================================================================
        // v3.3 FIX: Lógica de extração de ID criado
        // =====================================================================
        // PROBLEMA: BackendResult<int>.Value é int (não int?)
        //           Operador ?? só funciona com tipos nullable
        // SOLUÇÃO:  Usar operador ternário (? :) em vez de coalescência (??)
        // =====================================================================

        string extractIdExpression;
        string idNotEmptyCheck;

        if (isGuidPk)
        {
            // Guid: verifica se é diferente de Guid.Empty
            extractIdExpression = "createResult.Value != Guid.Empty ? createResult.Value : createResult.Data";
            idNotEmptyCheck = "createdId != Guid.Empty";
        }
        else if (isStringPk)
        {
            // string: verifica se não é null ou vazia (string é nullable, pode usar ??)
            extractIdExpression = "!string.IsNullOrEmpty(createResult.Value) ? createResult.Value : createResult.Data ?? string.Empty";
            idNotEmptyCheck = "!string.IsNullOrEmpty(createdId)";
        }
        else if (isLongPk)
        {
            // long: verifica se é diferente de 0
            extractIdExpression = "createResult.Value != 0 ? createResult.Value : createResult.Data";
            idNotEmptyCheck = "createdId != 0";
        }
        else // int (default)
        {
            // int: verifica se é diferente de 0
            extractIdExpression = "createResult.Value != 0 ? createResult.Value : createResult.Data";
            idNotEmptyCheck = "createdId != 0";
        }

        // =====================================================================
        // v3.2 - USA ApiRoute DO MANIFESTO
        // =====================================================================
        var apiRoute = entity.ApiRoute;

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: {entity.Name}
// Module: {entity.Module}
// ApiRoute: {apiRoute}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
// NOTA: Este serviço usa HttpClient TIPADO injetado pelo DI.
// O Timeout e políticas de resiliência (Polly) são configurados em:
// ServiceCollectionExtensions.AddCrudToolServicesAutomatically()
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
/// Consome a API backend gerada pelo Source Generator.
/// </summary>
/// <remarks>
/// Este serviço é registrado automaticamente no DI via:
/// <code>services.AddCrudToolServicesAutomatically(apiSettings)</code>
/// 
/// HttpClient já vem configurado com:
/// - BaseAddress
/// - Timeout
/// - Retry Policy (Polly)
/// - Circuit Breaker (Polly)
/// </remarks>
public class {entity.Name}ApiService : I{entity.Name}ApiService
{{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<{entity.Name}ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    // =========================================================================
    // v3.2 - ROTA API DO MANIFESTO (não mais construída automaticamente)
    // =========================================================================
    private const string ApiRoute = ""{apiRoute}"";

    public {entity.Name}ApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<{entity.Name}ApiService> logger)
    {{
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        
        // NOTA: Timeout e BaseAddress já configurados pelo DI (ServiceCollectionExtensions)
        // NÃO configurar aqui para evitar conflito com Polly
        
        _jsonOptions = new JsonSerializerOptions
        {{
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }};
    }}

    #region Private Helpers

    /// <summary>
    /// Configura header de autenticação com token JWT.
    /// Token está em AuthenticationTokens (StoreTokens no AccountController).
    /// </summary>
    private async Task SetAuthHeaderAsync()
    {{
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {{
            // Token está em AuthenticationTokens, não em Claims
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

    /// <summary>
    /// Cria ApiResponse de sucesso.
    /// </summary>
    private static ApiResponse<T> Success<T>(T? data) => new()
    {{
        Success = true,
        Data = data
    }};

    /// <summary>
    /// Cria ApiResponse de erro.
    /// NOTA: Message é computed (=> Error?.Message), então usamos Error.
    /// </summary>
    private static ApiResponse<T> Fail<T>(string message) => new()
    {{
        Success = false,
        Error = new ApiError {{ Message = message }}
    }};

    /// <summary>
    /// Processa resposta HTTP do backend.
    /// </summary>
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
                return Success(backendResult.Value ?? backendResult.Data);
            }}

            return Fail<T>(backendResult.Error?.Message ?? $""Erro HTTP {{(int)response.StatusCode}}"");
        }}
        catch (JsonException)
        {{
            // Tenta como string simples
            return Fail<T>($""Erro: {{content}}"");
        }}
    }}

    #endregion

    #region IApiService Implementation

    public async Task<ApiResponse<IEnumerable<{entity.Name}Dto>>> GetAllAsync()
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync(ApiRoute);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<IEnumerable<{entity.Name}Dto>>(response, ""GetAll"");

            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<List<{entity.Name}Dto>>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {{
                IEnumerable<{entity.Name}Dto> items = backendResult.Value ?? backendResult.Data ?? new List<{entity.Name}Dto>();
                return Success(items);
            }}

            return Fail<IEnumerable<{entity.Name}Dto>>(backendResult?.Error?.Message ?? ""Erro ao buscar registros"");
        }}
        catch (HttpRequestException ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Erro de conexão em GetAllAsync"");
            return Fail<IEnumerable<{entity.Name}Dto>>(""Erro de conexão com o servidor"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetAllAsync"");
            return Fail<IEnumerable<{entity.Name}Dto>>(ex.Message);
        }}
    }}

    public async Task<ApiResponse<PagedResult<{entity.Name}Dto>>> GetPagedAsync(int page, int pageSize, string? search = null)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            
            var queryParams = $""?page={{page}}&pageSize={{pageSize}}"";
            if (!string.IsNullOrEmpty(search))
                queryParams += $""&search={{Uri.EscapeDataString(search)}}"";
            
            var response = await _httpClient.GetAsync($""{{ApiRoute}}{{queryParams}}"");
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<PagedResult<{entity.Name}Dto>>(response, ""GetPaged"");

            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<PagedResult<{entity.Name}Dto>>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {{
                var pagedResult = backendResult.Value ?? backendResult.Data;
                if (pagedResult != null)
                    return Success(pagedResult);
            }}

            return Fail<PagedResult<{entity.Name}Dto>>(backendResult?.Error?.Message ?? ""Erro ao buscar registros paginados"");
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

    public async Task<ApiResponse<{entity.Name}Dto>> GetByIdAsync({pkType} id)
    {{
        try
        {{
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($""{{ApiRoute}}/{{id}}"");
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<{entity.Name}Dto>(response, ""GetById"");

            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<{entity.Name}Dto>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {{
                return Success(backendResult.Value ?? backendResult.Data);
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
            
            _logger.LogDebug(""[{entityUpper}] POST {{Route}} - Body: {{Body}}"", ApiRoute, json);
            
            var response = await _httpClient.PostAsync(ApiRoute, content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<{entity.Name}Dto>(response, ""Create"");

            // Backend retorna Result<{pkType}> com o ID criado
            var responseJson = await response.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<BackendResult<{pkType}>>(responseJson, _jsonOptions);
            
            if (createResult?.IsSuccess == true)
            {{
                // v3.3 FIX: Usa ternário em vez de ?? (value types não são nullable)
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

    /// <summary>
    /// DTO para deserializar resposta do backend.
    /// Backend pode retornar Value ou Data dependendo do endpoint.
    /// </summary>
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

    /// <summary>
    /// Converte nome do módulo para path de pasta.
    /// Ex: "eSocial" -> "eSocial", "GestaoDePessoas" -> "GestaoDePessoas"
    /// </summary>
    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "Common";

        // Mantém o nome original do módulo
        return moduleName;
    }
}