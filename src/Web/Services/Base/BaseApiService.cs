// src/Web/Services/Base/BaseApiService.cs

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Models.Common;

namespace RhSensoERP.Web.Services.Base;

/// <summary>
/// Serviço base genérico para comunicação com a API REST.
/// </summary>
/// <typeparam name="TDto">Tipo do DTO completo</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto, TKey> : IApiService<TDto, TCreateDto, TUpdateDto, TKey>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly string _baseEndpoint;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected BaseApiService(
        HttpClient httpClient,
        ILogger logger,
        IHttpContextAccessor httpContextAccessor,
        string baseEndpoint)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _baseEndpoint = baseEndpoint;
    }

    /// <summary>
    /// Adiciona o token de autenticação no header da requisição.
    /// </summary>
    protected async Task AddAuthorizationHeaderAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var token = await httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            }

            var url = $"{_baseEndpoint}?{string.Join("&", queryParams)}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<TDto>>>(content, _jsonOptions);
                return result ?? new ApiResponse<PagedResult<TDto>> { Success = false, Error = new ApiError { Message = "Erro ao deserializar resposta" } };
            }

            _logger.LogWarning("Erro ao buscar dados paginados: {StatusCode} - {Content}", response.StatusCode, content);
            return new ApiResponse<PagedResult<TDto>> { Success = false, Error = new ApiError { Message = "Erro ao buscar dados" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados paginados");
            return new ApiResponse<PagedResult<TDto>> { Success = false, Error = new ApiError { Message = "Erro ao buscar dados" } };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync()
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"{_baseEndpoint}/all");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<TDto>>>(content, _jsonOptions);
                return result ?? new ApiResponse<IEnumerable<TDto>> { Success = false, Error = new ApiError { Message = "Erro ao deserializar resposta" } };
            }

            _logger.LogWarning("Erro ao buscar todos os registros: {StatusCode} - {Content}", response.StatusCode, content);
            return new ApiResponse<IEnumerable<TDto>> { Success = false, Error = new ApiError { Message = "Erro ao buscar registros" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os registros");
            return new ApiResponse<IEnumerable<TDto>> { Success = false, Error = new ApiError { Message = "Erro ao buscar registros" } };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<TDto>> GetByIdAsync(TKey id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"{_baseEndpoint}/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                return result ?? new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao deserializar resposta" } };
            }

            _logger.LogWarning("Erro ao buscar registro {Id}: {StatusCode} - {Content}", id, response.StatusCode, content);
            return new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Registro não encontrado" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar registro {Id}", id);
            return new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao buscar registro" } };
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
                var result = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                return result ?? new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao deserializar resposta" } };
            }

            _logger.LogWarning("Erro ao criar registro: {StatusCode} - {Content}", response.StatusCode, content);

            // Tenta deserializar a resposta de erro
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                if (errorResponse != null)
                {
                    return errorResponse;
                }
            }
            catch
            {
                // Ignora erro de deserialização
            }

            return new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao criar registro" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar registro");
            return new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao criar registro" } };
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

            var response = await _httpClient.PutAsync($"{_baseEndpoint}/{id}", httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                return result ?? new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao deserializar resposta" } };
            }

            _logger.LogWarning("Erro ao atualizar registro {Id}: {StatusCode} - {Content}", id, response.StatusCode, content);

            // Tenta deserializar a resposta de erro
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<TDto>>(content, _jsonOptions);
                if (errorResponse != null)
                {
                    return errorResponse;
                }
            }
            catch
            {
                // Ignora erro de deserialização
            }

            return new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao atualizar registro" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar registro {Id}", id);
            return new ApiResponse<TDto> { Success = false, Error = new ApiError { Message = "Erro ao atualizar registro" } };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<bool>> DeleteAsync(TKey id)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool> { Success = true, Error = new ApiError { Message = "Registro excluído com sucesso" }, Data = true };
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Erro ao excluir registro {Id}: {StatusCode} - {Content}", id, response.StatusCode, content);

            // Tenta deserializar a resposta de erro
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                if (errorResponse != null)
                {
                    return errorResponse;
                }
            }
            catch
            {
                // Ignora erro de deserialização
            }

            return new ApiResponse<bool> { Success = false, Error = new ApiError { Message = "Erro ao excluir registro" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir registro {Id}", id);
            return new ApiResponse<bool> { Success = false, Error = new ApiError { Message = "Erro ao excluir registro" } };
        }
    }

    /// <inheritdoc/>
    public virtual async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<TKey> ids)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var json = JsonSerializer.Serialize(new { ids }, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseEndpoint}/multiple")
            {
                Content = httpContent
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool> { Success = true, Error = new ApiError { Message = "Registros excluídos com sucesso" }, Data = true };
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Erro ao excluir múltiplos registros: {StatusCode} - {Content}", response.StatusCode, content);

            // Tenta deserializar a resposta de erro
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                if (errorResponse != null)
                {
                    return errorResponse;
                }
            }
            catch
            {
                // Ignora erro de deserialização
            }

            return new ApiResponse<bool> { Success = false, Error = new ApiError { Message = "Erro ao excluir registros" } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir múltiplos registros");
            return new ApiResponse<bool> { Success = false, Error = new ApiError { Message = "Erro ao excluir registros" } };
        }
    }
}
