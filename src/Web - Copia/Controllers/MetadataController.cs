// =============================================================================
// RHSENSOERP.WEB - METADATA CONTROLLER
// =============================================================================
// Arquivo: src/RhSensoERP.Web/Controllers/MetadataController.cs
// Descrição: Controller que faz proxy das chamadas de metadados para a API
// =============================================================================

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Services;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller que faz proxy das chamadas de metadados e CRUD para a API.
/// Necessário porque o JavaScript no browser não tem acesso direto à API.
/// </summary>
[Authorize]
[Route("[controller]")]
public class MetadataController : Controller
{
    private readonly IMetadataService _metadataService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MetadataController> _logger;
    private readonly string _apiBaseUrl;

    public MetadataController(
        IMetadataService metadataService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MetadataController> logger)
    {
        _metadataService = metadataService;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
        _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7193";
    }

    /// <summary>
    /// Obtém os metadados de uma entidade.
    /// GET /Metadata/GetEntityMetadata?entityName=Banco
    /// </summary>
    [HttpGet("GetEntityMetadata")]
    public async Task<IActionResult> GetEntityMetadata([FromQuery] string entityName)
    {
        try
        {
            _logger.LogDebug("Buscando metadados para entidade: {EntityName}", entityName);

            var metadata = await _metadataService.GetByNameAsync(entityName);

            if (metadata == null)
            {
                return NotFound(new { message = $"Metadados não encontrados para '{entityName}'." });
            }

            return Json(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar metadados de {EntityName}", entityName);
            return StatusCode(500, new { message = "Erro ao buscar metadados." });
        }
    }

    /// <summary>
    /// Obtém os dados de uma entidade (lista).
    /// GET /Metadata/GetEntityData?entityName=Banco
    /// </summary>
    [HttpGet("GetEntityData")]
    public async Task<IActionResult> GetEntityData([FromQuery] string entityName)
    {
        try
        {
            _logger.LogDebug("Buscando dados para entidade: {EntityName}", entityName);

            // Obtém metadados para saber o endpoint
            var metadata = await _metadataService.GetByNameAsync(entityName);

            if (metadata == null)
            {
                return NotFound(new { message = $"Metadados não encontrados para '{entityName}'." });
            }

            // Chama a API
            var apiUrl = metadata.Endpoints.BaseUrl;
            _logger.LogDebug("Chamando API: {ApiUrl}", apiUrl);

            // Adiciona token de autenticação
            AddAuthHeader();

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("API retornou {StatusCode}: {Content}", response.StatusCode, errorContent);
                return StatusCode((int)response.StatusCode, new { message = "Erro ao buscar dados da API." });
            }

            var content = await response.Content.ReadAsStringAsync();

            // Retorna o JSON diretamente
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados de {EntityName}", entityName);
            return StatusCode(500, new { message = "Erro ao buscar dados." });
        }
    }

    /// <summary>
    /// Obtém um registro por ID.
    /// GET /Metadata/GetEntityById?entityName=Banco&id=123
    /// </summary>
    [HttpGet("GetEntityById")]
    public async Task<IActionResult> GetEntityById([FromQuery] string entityName, [FromQuery] string id)
    {
        try
        {
            _logger.LogDebug("Buscando {EntityName} com ID: {Id}", entityName, id);

            var metadata = await _metadataService.GetByNameAsync(entityName);

            if (metadata == null)
            {
                return NotFound(new { message = $"Metadados não encontrados para '{entityName}'." });
            }

            // Monta URL: /api/bancos/{id}
            var apiUrl = metadata.Endpoints.GetById.Replace("{id}", id);
            _logger.LogDebug("Chamando API: {ApiUrl}", apiUrl);

            AddAuthHeader();

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = "Registro não encontrado." });
                }
                return StatusCode((int)response.StatusCode, new { message = "Erro ao buscar registro." });
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} por ID {Id}", entityName, id);
            return StatusCode(500, new { message = "Erro ao buscar registro." });
        }
    }

    /// <summary>
    /// Cria um novo registro.
    /// POST /Metadata/CreateEntity?entityName=Banco
    /// </summary>
    [HttpPost("CreateEntity")]
    public async Task<IActionResult> CreateEntity([FromQuery] string entityName, [FromBody] JsonElement data)
    {
        try
        {
            _logger.LogDebug("Criando {EntityName}", entityName);

            var metadata = await _metadataService.GetByNameAsync(entityName);

            if (metadata == null)
            {
                return NotFound(new { message = $"Metadados não encontrados para '{entityName}'." });
            }

            var apiUrl = metadata.Endpoints.Create;
            _logger.LogDebug("Chamando API: {ApiUrl}", apiUrl);

            AddAuthHeader();

            var jsonContent = new StringContent(
                data.GetRawText(),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API retornou {StatusCode}: {Content}", response.StatusCode, content);
                return StatusCode((int)response.StatusCode,
                    string.IsNullOrEmpty(content)
                        ? new { message = "Erro ao criar registro." }
                        : JsonSerializer.Deserialize<object>(content));
            }

            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar {EntityName}", entityName);
            return StatusCode(500, new { message = "Erro ao criar registro." });
        }
    }

    /// <summary>
    /// Atualiza um registro.
    /// PUT /Metadata/UpdateEntity?entityName=Banco&id=123
    /// </summary>
    [HttpPut("UpdateEntity")]
    public async Task<IActionResult> UpdateEntity([FromQuery] string entityName, [FromQuery] string id, [FromBody] JsonElement data)
    {
        try
        {
            _logger.LogDebug("Atualizando {EntityName} com ID: {Id}", entityName, id);

            var metadata = await _metadataService.GetByNameAsync(entityName);

            if (metadata == null)
            {
                return NotFound(new { message = $"Metadados não encontrados para '{entityName}'." });
            }

            var apiUrl = metadata.Endpoints.Update.Replace("{id}", id);
            _logger.LogDebug("Chamando API: {ApiUrl}", apiUrl);

            AddAuthHeader();

            var jsonContent = new StringContent(
                data.GetRawText(),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync(apiUrl, jsonContent);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API retornou {StatusCode}: {Content}", response.StatusCode, content);
                return StatusCode((int)response.StatusCode,
                    string.IsNullOrEmpty(content)
                        ? new { message = "Erro ao atualizar registro." }
                        : JsonSerializer.Deserialize<object>(content));
            }

            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar {EntityName} com ID {Id}", entityName, id);
            return StatusCode(500, new { message = "Erro ao atualizar registro." });
        }
    }

    /// <summary>
    /// Exclui um registro.
    /// DELETE /Metadata/DeleteEntity?entityName=Banco&id=123
    /// </summary>
    [HttpDelete("DeleteEntity")]
    public async Task<IActionResult> DeleteEntity([FromQuery] string entityName, [FromQuery] string id)
    {
        try
        {
            _logger.LogDebug("Excluindo {EntityName} com ID: {Id}", entityName, id);

            var metadata = await _metadataService.GetByNameAsync(entityName);

            if (metadata == null)
            {
                return NotFound(new { message = $"Metadados não encontrados para '{entityName}'." });
            }

            var apiUrl = metadata.Endpoints.Delete.Replace("{id}", id);
            _logger.LogDebug("Chamando API: {ApiUrl}", apiUrl);

            AddAuthHeader();

            var response = await _httpClient.DeleteAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("API retornou {StatusCode}: {Content}", response.StatusCode, content);
                return StatusCode((int)response.StatusCode, new { message = "Erro ao excluir registro." });
            }

            return Ok(new { message = "Registro excluído com sucesso." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {EntityName} com ID {Id}", entityName, id);
            return StatusCode(500, new { message = "Erro ao excluir registro." });
        }
    }

    /// <summary>
    /// Adiciona o header de autenticação JWT.
    /// </summary>
    private void AddAuthHeader()
    {
        // Obtém o token do cookie ou claim
        var token = User.FindFirst("JwtToken")?.Value;

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}