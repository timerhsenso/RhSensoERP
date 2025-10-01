using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RhSensoWeb.Configuration;
using RhSensoWeb.Extensions;
using RhSensoWeb.Models.SEG;
using RhSensoWeb.Models.Shared;
using RhSensoWeb.Services.Interfaces;

namespace RhSensoWeb.Services;

/// <summary>
/// Serviço para operações de usuários com a API
/// </summary>
public class UsuarioApiService : IUsuarioApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<UsuarioApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsuarioApiService(
        HttpClient httpClient,
        IOptionsMonitor<ApiSettings> apiSettings,
        ILogger<UsuarioApiService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _apiSettings = apiSettings.CurrentValue;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    /// <summary>
    /// Configura o HttpClient com token do usuário atual
    /// </summary>
    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_apiSettings.TimeoutSeconds);

        // Adicionar token de autorização se disponível
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var token = httpContext.User.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    /// <summary>
    /// Obtém lista paginada de usuários
    /// </summary>
    public async Task<DataTablesResponse<UsuarioListDto>> GetUsuariosAsync(
        DataTablesRequest request, 
        UsuarioFiltroDto? filtros = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var queryParams = new List<string>
            {
                $"page={request.Page}",
                $"pageSize={request.PageSize}",
                $"search={Uri.EscapeDataString(request.Search ?? "")}"
            };

            if (!string.IsNullOrEmpty(request.OrderColumn))
            {
                queryParams.Add($"orderBy={request.OrderColumn}");
                queryParams.Add($"orderDirection={request.OrderDirection}");
            }

            // Adicionar filtros
            if (filtros != null)
            {
                if (!string.IsNullOrEmpty(filtros.Nome))
                    queryParams.Add($"nome={Uri.EscapeDataString(filtros.Nome)}");
                
                if (!string.IsNullOrEmpty(filtros.Email))
                    queryParams.Add($"email={Uri.EscapeDataString(filtros.Email)}");
                
                if (!string.IsNullOrEmpty(filtros.Tipo))
                    queryParams.Add($"tipo={Uri.EscapeDataString(filtros.Tipo)}");
                
                if (filtros.Status.HasValue)
                    queryParams.Add($"status={filtros.Status}");
                
                if (!string.IsNullOrEmpty(filtros.Empresa))
                    queryParams.Add($"empresa={Uri.EscapeDataString(filtros.Empresa)}");
                
                if (!string.IsNullOrEmpty(filtros.Filial))
                    queryParams.Add($"filial={Uri.EscapeDataString(filtros.Filial)}");
            }

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"seg/usuarios?{queryString}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<DataTablesResponse<UsuarioListDto>>>(responseContent, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }
            }

            _logger.LogError("Erro ao obter usuários: {StatusCode} - {Content}", response.StatusCode, responseContent);
            
            return new DataTablesResponse<UsuarioListDto>
            {
                Draw = request.Draw,
                RecordsTotal = 0,
                RecordsFiltered = 0,
                Data = new List<UsuarioListDto>(),
                Error = "Erro ao carregar usuários"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao obter usuários");
            
            return new DataTablesResponse<UsuarioListDto>
            {
                Draw = request.Draw,
                RecordsTotal = 0,
                RecordsFiltered = 0,
                Data = new List<UsuarioListDto>(),
                Error = "Erro interno do sistema"
            };
        }
    }

    /// <summary>
    /// Obtém usuário por código
    /// </summary>
    public async Task<ApiResponse<UsuarioDetalheDto>> GetUsuarioByIdAsync(
        string cdUsuario, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var response = await _httpClient.GetAsync($"seg/usuarios/{Uri.EscapeDataString(cdUsuario)}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UsuarioDetalheDto>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<UsuarioDetalheDto>.Fail("Resposta inválida da API");
            }

            _logger.LogError("Erro ao obter usuário {CdUsuario}: {StatusCode}", cdUsuario, response.StatusCode);
            return ApiResponse<UsuarioDetalheDto>.Fail($"Erro HTTP: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao obter usuário {CdUsuario}", cdUsuario);
            return ApiResponse<UsuarioDetalheDto>.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Cria novo usuário
    /// </summary>
    public async Task<ApiResponse<UsuarioDetalheDto>> CreateUsuarioAsync(
        UsuarioViewModel usuario, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var json = JsonSerializer.Serialize(usuario, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("seg/usuarios", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UsuarioDetalheDto>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<UsuarioDetalheDto>.Fail("Resposta inválida da API");
            }

            _logger.LogError("Erro ao criar usuário: {StatusCode} - {Content}", response.StatusCode, responseContent);
            
            // Tentar deserializar erro da API
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<UsuarioDetalheDto>>(responseContent, _jsonOptions);
                return errorResponse ?? ApiResponse<UsuarioDetalheDto>.Fail($"Erro HTTP: {response.StatusCode}");
            }
            catch
            {
                return ApiResponse<UsuarioDetalheDto>.Fail($"Erro HTTP: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar usuário");
            return ApiResponse<UsuarioDetalheDto>.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Atualiza usuário existente
    /// </summary>
    public async Task<ApiResponse<UsuarioDetalheDto>> UpdateUsuarioAsync(
        string cdUsuario, 
        UsuarioViewModel usuario, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var json = JsonSerializer.Serialize(usuario, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"seg/usuarios/{Uri.EscapeDataString(cdUsuario)}", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UsuarioDetalheDto>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<UsuarioDetalheDto>.Fail("Resposta inválida da API");
            }

            _logger.LogError("Erro ao atualizar usuário {CdUsuario}: {StatusCode}", cdUsuario, response.StatusCode);
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<UsuarioDetalheDto>>(responseContent, _jsonOptions);
                return errorResponse ?? ApiResponse<UsuarioDetalheDto>.Fail($"Erro HTTP: {response.StatusCode}");
            }
            catch
            {
                return ApiResponse<UsuarioDetalheDto>.Fail($"Erro HTTP: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar usuário {CdUsuario}", cdUsuario);
            return ApiResponse<UsuarioDetalheDto>.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Exclui usuário
    /// </summary>
    public async Task<ApiResponse> DeleteUsuarioAsync(
        string cdUsuario, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var response = await _httpClient.DeleteAsync($"seg/usuarios/{Uri.EscapeDataString(cdUsuario)}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse.Ok("Usuário excluído com sucesso");
            }

            _logger.LogError("Erro ao excluir usuário {CdUsuario}: {StatusCode}", cdUsuario, response.StatusCode);
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, _jsonOptions);
                return errorResponse ?? ApiResponse.Fail($"Erro HTTP: {response.StatusCode}");
            }
            catch
            {
                return ApiResponse.Fail($"Erro HTTP: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao excluir usuário {CdUsuario}", cdUsuario);
            return ApiResponse.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Ativa/desativa usuário
    /// </summary>
    public async Task<ApiResponse> ToggleUsuarioStatusAsync(
        string cdUsuario, 
        bool ativo, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var json = JsonSerializer.Serialize(new { ativo }, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"seg/usuarios/{Uri.EscapeDataString(cdUsuario)}/status", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse.Ok("Status alterado com sucesso");
            }

            _logger.LogError("Erro ao alterar status do usuário {CdUsuario}: {StatusCode}", cdUsuario, response.StatusCode);
            return ApiResponse.Fail($"Erro HTTP: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao alterar status do usuário {CdUsuario}", cdUsuario);
            return ApiResponse.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Altera senha do usuário
    /// </summary>
    public async Task<ApiResponse> AlterarSenhaAsync(
        string cdUsuario, 
        string novaSenha, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var json = JsonSerializer.Serialize(new { novaSenha }, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"seg/usuarios/{Uri.EscapeDataString(cdUsuario)}/senha", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse.Ok("Senha alterada com sucesso");
            }

            _logger.LogError("Erro ao alterar senha do usuário {CdUsuario}: {StatusCode}", cdUsuario, response.StatusCode);
            return ApiResponse.Fail($"Erro HTTP: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao alterar senha do usuário {CdUsuario}", cdUsuario);
            return ApiResponse.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Obtém grupos disponíveis
    /// </summary>
    public async Task<ApiResponse<List<GrupoDto>>> GetGruposAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var response = await _httpClient.GetAsync("seg/grupos", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<GrupoDto>>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<List<GrupoDto>>.Fail("Resposta inválida da API");
            }

            return ApiResponse<List<GrupoDto>>.Fail($"Erro HTTP: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter grupos");
            return ApiResponse<List<GrupoDto>>.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Obtém empresas disponíveis
    /// </summary>
    public async Task<ApiResponse<List<EmpresaDto>>> GetEmpresasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var response = await _httpClient.GetAsync("rhu/empresas", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<EmpresaDto>>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<List<EmpresaDto>>.Fail("Resposta inválida da API");
            }

            return ApiResponse<List<EmpresaDto>>.Fail($"Erro HTTP: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter empresas");
            return ApiResponse<List<EmpresaDto>>.Fail("Erro interno do sistema");
        }
    }

    /// <summary>
    /// Obtém filiais de uma empresa
    /// </summary>
    public async Task<ApiResponse<List<FilialDto>>> GetFiliaisAsync(
        string cdEmpresa, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            ConfigureHttpClient();

            var response = await _httpClient.GetAsync($"rhu/empresas/{Uri.EscapeDataString(cdEmpresa)}/filiais", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<FilialDto>>>(responseContent, _jsonOptions);
                return apiResponse ?? ApiResponse<List<FilialDto>>.Fail("Resposta inválida da API");
            }

            return ApiResponse<List<FilialDto>>.Fail($"Erro HTTP: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter filiais da empresa {CdEmpresa}", cdEmpresa);
            return ApiResponse<List<FilialDto>>.Fail("Erro interno do sistema");
        }
    }
}
