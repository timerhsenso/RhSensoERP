// =============================================================================
// BASE API SERVICE - VERSÃO CORRIGIDA COM ORDENAÇÃO FUNCIONAL
// =============================================================================
// Versão: 4.0 FINAL
// Atualizado: 2025-12-30
// Changelog v4.0:
//   ✅ CORRIGIDO: orderBy → sortBy
//   ✅ CORRIGIDO: ascending → desc (invertido)
//   ✅ CORRIGIDO: Parâmetros compatíveis com PagedRequest do backend
//   ✅ Mantém compatibilidade com todos os ApiServices existentes
// =============================================================================

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Models.Common;

namespace RhSensoERP.Web.Services.Base;

/// <summary>
/// Serviço base genérico para comunicação com a API REST.
/// Implementa IApiService com suporte completo a paginação, ordenação e CRUD.
/// v4.0: Parâmetros corretos para ordenação (sortBy, desc).
/// </summary>
/// <typeparam name="TDto">Tipo do DTO completo</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto, TKey>
    : IApiService<TDto, TCreateDto, TUpdateDto, TKey>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    #region Fields & Constructor

    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly string _baseEndpoint;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    protected BaseApiService(
        HttpClient httpClient,
        ILogger logger,
        IHttpContextAccessor httpContextAccessor,
        string baseEndpoint)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _baseEndpoint = baseEndpoint ?? throw new ArgumentNullException(nameof(baseEndpoint));
    }

    #endregion

    #region Authentication

    protected virtual async Task AddAuthorizationHeaderAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var token = await httpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            "access_token"
        );

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    #endregion

    #region Query String Builder

    protected virtual string BuildQueryString(Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return string.Empty;

        var queryParams = parameters
            .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}");

        return string.Join("&", queryParams);
    }

    #endregion

    #region IApiService Implementation

    /// <inheritdoc/>
    /// <summary>
    /// ✅ v4.0: Corrigido para usar sortBy e desc (compatível com backend PagedRequest).
    /// </summary>
    public virtual async Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        string? sortBy = null,        // ✅ RENOMEADO: orderBy → sortBy
        bool desc = false)             // ✅ RENOMEADO E INVERTIDO: ascending → desc
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            // ================================================================
            // ✅ PARÂMETROS CORRETOS: sortBy, desc (não orderBy, ascending)
            // ================================================================
            var queryParams = new Dictionary<string, string>
            {
                ["page"] = page.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["desc"] = desc.ToString().ToLower()  // ✅ CORRETO
            };

            if (!string.IsNullOrWhiteSpace(search))
                queryParams["search"] = search;

            if (!string.IsNullOrWhiteSpace(sortBy))   // ✅ CORRETO
                queryParams["sortBy"] = sortBy;       // ✅ CORRETO

            var queryString = BuildQueryString(queryParams);
            var url = $"{_baseEndpoint}?{queryString}";

            _logger.LogDebug(
                "📊 GetPagedAsync - Page: {Page}, SortBy: {SortBy}, Desc: {Desc}",
                page, sortBy ?? "null", desc
            );

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<TDto>>>(content, _jsonOptions);
                return result ?? new ApiResponse<PagedResult<TDto>>
                {
                    Success = false,
                    Error = new ApiError { Message = "Erro ao deserializar resposta" }
                };
            }

            _logger.LogWarning("⚠️ GetPagedAsync - Erro {StatusCode}", response.StatusCode);
            return new ApiResponse<PagedResult<TDto>>
            {
                Success = false,
                Error = new ApiError { Message = $"Erro ao buscar dados: {response.StatusCode}" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao buscar dados paginados");
            return new ApiResponse<PagedResult<TDto>>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar dados" }
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var url = $"{_baseEndpoint}/all";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<TDto>>>(content, _jsonOptions);
                return result ?? new ApiResponse<IEnumerable<TDto>>
                {
                    Success = false,
                    Error = new ApiError { Message = "Erro ao deserializar resposta" }
                };
            }

            _logger.LogWarning("⚠️ GetAllAsync - Erro {StatusCode}", response.StatusCode);
            return new ApiResponse<IEnumerable<TDto>>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registros" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao buscar todos os registros");
            return new ApiResponse<IEnumerable<TDto>>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registros" }
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TDto>> GetByIdAsync(TKey id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var url = $"{_baseEndpoint}/{id}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                return result ?? new ApiResponse<TDto>
                {
                    Success = false,
                    Error = new ApiError { Message = "Erro ao deserializar resposta" }
                };
            }

            _logger.LogWarning("⚠️ GetByIdAsync - ID: {Id}, Erro {StatusCode}", id, response.StatusCode);
            return new ApiResponse<TDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao buscar registro {Id}", id);
            return new ApiResponse<TDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao buscar registro" }
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TDto>> CreateAsync(TCreateDto dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseEndpoint, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // ✅ v4.4: Backend retorna Result<TKey> (ID criado), não o DTO completo
                var resultKey = JsonSerializer.Deserialize<ApiResponse<TKey>>(content, _jsonOptions);
                
                if (resultKey != null && resultKey.Success && resultKey.Data != null)
                {
                    // Busca o objeto completo pelo ID recém-criado
                    return await GetByIdAsync(resultKey.Data);
                }

                // Se falhou ou não tem dados, retorna erro
                return new ApiResponse<TDto>
                {
                    Success = false,
                    Error = resultKey?.Error ?? new ApiError { Message = "Erro ao criar registro (ID não retornado)" },
                    Errors = resultKey?.Errors
                };
            }

            _logger.LogWarning("⚠️ CreateAsync - Erro {StatusCode}", response.StatusCode);

            // Tenta deserializar erro da API
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<TDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao criar registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao criar registro");
            return new ApiResponse<TDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao criar registro" }
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TDto>> UpdateAsync(TKey id, TUpdateDto dto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{_baseEndpoint}/{id}";
            var response = await _httpClient.PutAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // ✅ v4.4: Backend retorna Result<bool>, não o DTO completo
                var resultBool = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);

                if (resultBool != null && resultBool.Success)
                {
                    // Busca o objeto completo atualizado
                    return await GetByIdAsync(id);
                }

                return new ApiResponse<TDto>
                {
                    Success = false,
                    Error = resultBool?.Error ?? new ApiError { Message = "Erro ao atualizar registro" },
                    Errors = resultBool?.Errors
                };
            }

            _logger.LogWarning("⚠️ UpdateAsync - ID: {Id}, Erro {StatusCode}", id, response.StatusCode);

            // Tenta deserializar erro da API
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<TDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao atualizar registro" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao atualizar registro {Id}", id);
            return new ApiResponse<TDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao atualizar registro" }
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<bool>> DeleteAsync(TKey id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var url = $"{_baseEndpoint}/{id}";
            var response = await _httpClient.DeleteAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Registro {Id} excluído", id);
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true
                };
            }

            _logger.LogWarning("⚠️ DeleteAsync - ID: {Id}, Erro {StatusCode}", id, response.StatusCode);

            // Tenta deserializar erro da API
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
            _logger.LogError(ex, "❌ Erro ao excluir registro {Id}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao excluir registro" }
            };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<TKey> ids)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var idsList = ids?.ToList() ?? new List<TKey>();

            if (!idsList.Any())
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Error = new ApiError { Message = "Nenhum ID fornecido" }
                };
            }

            var json = JsonSerializer.Serialize(new { ids = idsList }, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseEndpoint}/multiple")
            {
                Content = httpContent
            };

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ {Count} registros excluídos", idsList.Count);
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true
                };
            }

            _logger.LogWarning("⚠️ DeleteMultipleAsync - Erro {StatusCode}", response.StatusCode);

            // Tenta deserializar erro da API
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao excluir registros" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao excluir múltiplos registros");
            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao excluir registros" }
            };
        }
    }

    #endregion
}