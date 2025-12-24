// =============================================================================
// GERADOR FULL-STACK v3.1 - MANIFEST CONTROLLER
// Controller para gerar frontend a partir do manifesto do backend
// =============================================================================

using GeradorEntidades.Models;
using GeradorEntidades.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;

namespace GeradorEntidades.Controllers;

/// <summary>
/// Controller para geração de frontend a partir do manifesto do backend.
/// Permite selecionar entidades já existentes no backend e gerar apenas o frontend.
/// </summary>
public class ManifestController : Controller
{
    private readonly ManifestService _manifestService;
    private readonly FullStackGeneratorService _fullStackGenerator;
    private readonly ILogger<ManifestController> _logger;

    public ManifestController(
        ManifestService manifestService,
        FullStackGeneratorService fullStackGenerator,
        ILogger<ManifestController> logger)
    {
        _manifestService = manifestService;
        _fullStackGenerator = fullStackGenerator;
        _logger = logger;
    }

    // =========================================================================
    // PÁGINA PRINCIPAL - Lista entidades do backend
    // =========================================================================

    /// <summary>
    /// Página com lista de entidades do backend (via manifesto).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var modules = await _manifestService.GetModulesAsync();
            var manifest = await _manifestService.GetManifestAsync();

            ViewBag.TotalEntidades = manifest?.Entities.Count ?? 0;
            ViewBag.TotalModulos = modules.Count;
            ViewBag.GeneratedAt = manifest?.GeneratedAt ?? "N/A";
            ViewBag.ApiUrl = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["ApiSettings:BaseUrl"] ?? "https://localhost:7193";

            return View(modules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar manifesto");
            ViewBag.Error = $"Erro ao conectar com a API: {ex.Message}";
            return View(new List<ModuleInfo>());
        }
    }

    // =========================================================================
    // APIs AJAX
    // =========================================================================

    /// <summary>
    /// Lista módulos do backend.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetModules()
    {
        try
        {
            var modules = await _manifestService.GetModulesAsync();
            return Json(modules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar módulos");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Lista entidades de um módulo.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEntities(string module)
    {
        try
        {
            var entities = await _manifestService.GetEntitiesByModuleAsync(module);
            return Json(entities.Select(e => new
            {
                e.EntityName,
                e.DisplayName,
                e.TableName,
                e.Route,
                e.CdSistema,
                e.CdFuncao,
                e.Icon,
                PropertiesCount = e.Properties.Count,
                NavigationsCount = e.Navigations.Count,
                HasCdFuncao = !string.IsNullOrEmpty(e.CdFuncao)
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar entidades do módulo {Module}", module);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém detalhes de uma entidade.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEntity(string name)
    {
        try
        {
            var entity = await _manifestService.GetEntityAsync(name);
            if (entity == null)
                return NotFound(new { error = $"Entidade '{name}' não encontrada" });

            return Json(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar entidade {Name}", name);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Força atualização do cache do manifesto.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RefreshManifest()
    {
        try
        {
            var manifest = await _manifestService.GetManifestAsync(forceRefresh: true);
            return Json(new
            {
                success = true,
                count = manifest?.Entities.Count ?? 0,
                generatedAt = manifest?.GeneratedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar manifesto");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // =========================================================================
    // GERAÇÃO DE FRONTEND
    // =========================================================================

    /// <summary>
    /// Gera frontend para uma entidade do backend.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GerarFrontend([FromBody] ManifestGenerateRequest request)
    {
        _logger.LogInformation("GerarFrontend iniciado para entidade: {Entity}", request.EntityName);

        try
        {
            // 1. Busca entidade do manifesto
            var entity = await _manifestService.GetEntityAsync(request.EntityName);
            if (entity == null)
            {
                return NotFound(new { error = $"Entidade '{request.EntityName}' não encontrada no manifesto" });
            }

            // 2. Converte para TabelaInfo
            var tabela = _manifestService.ConvertToTabelaInfo(entity);

            // 3. Converte para FullStackRequest (apenas frontend!)
            var fullRequest = _manifestService.ConvertToFullStackRequest(entity);

            // 4. Aplica overrides do request (se fornecidos)
            if (!string.IsNullOrEmpty(request.CdFuncao))
                fullRequest.CdFuncao = request.CdFuncao;
            if (!string.IsNullOrEmpty(request.DisplayName))
                fullRequest.DisplayName = request.DisplayName;
            if (!string.IsNullOrEmpty(request.Icone))
                fullRequest.Icone = request.Icone;
            if (request.MenuOrder > 0)
                fullRequest.MenuOrder = request.MenuOrder;

            // Configurações de geração (apenas frontend)
            fullRequest.GerarEntidade = false;
            fullRequest.GerarApiController = false;
            fullRequest.GerarWebController = request.GerarWebController;
            fullRequest.GerarWebModels = request.GerarWebModels;
            fullRequest.GerarWebServices = request.GerarWebServices;
            fullRequest.GerarView = request.GerarView;
            fullRequest.GerarJavaScript = request.GerarJavaScript;

            // 5. Gera os arquivos
            var result = _fullStackGenerator.Generate(tabela, fullRequest);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation(
                "Frontend gerado para {Entity}: {FileCount} arquivos",
                request.EntityName,
                result.AllFiles.Count());

            return Json(new
            {
                success = true,
                nomeEntidade = result.NomeEntidade,
                warnings = result.Warnings,
                files = result.AllFiles.Select(f => new
                {
                    f.FileName,
                    f.RelativePath,
                    f.FileType,
                    f.Content
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar frontend para {Entity}", request.EntityName);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Download ZIP do frontend gerado.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadFrontendZip([FromBody] ManifestGenerateRequest request)
    {
        try
        {
            // 1. Busca entidade
            var entity = await _manifestService.GetEntityAsync(request.EntityName);
            if (entity == null)
                return NotFound($"Entidade '{request.EntityName}' não encontrada");

            // 2. Converte
            var tabela = _manifestService.ConvertToTabelaInfo(entity);
            var fullRequest = _manifestService.ConvertToFullStackRequest(entity);

            // Aplica overrides
            if (!string.IsNullOrEmpty(request.CdFuncao))
                fullRequest.CdFuncao = request.CdFuncao;
            if (!string.IsNullOrEmpty(request.DisplayName))
                fullRequest.DisplayName = request.DisplayName;

            // Apenas frontend
            fullRequest.GerarEntidade = false;
            fullRequest.GerarApiController = false;

            // 3. Gera
            var result = _fullStackGenerator.Generate(tabela, fullRequest);
            if (!result.Success)
                return BadRequest(result.Error);

            // 4. Cria ZIP
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in result.AllFiles)
                {
                    var entry = archive.CreateEntry(file.RelativePath);
                    using var entryStream = entry.Open();
                    using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                    await writer.WriteAsync(file.Content);
                }

                // README
                var readmeEntry = archive.CreateEntry("README.md");
                using var readmeStream = readmeEntry.Open();
                using var readmeWriter = new StreamWriter(readmeStream, Encoding.UTF8);
                await readmeWriter.WriteAsync(GenerateReadme(result, entity));
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/zip", $"{result.NomeEntidade}_Frontend.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar ZIP para {Entity}", request.EntityName);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Geração em lote para múltiplas entidades.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GerarLote([FromBody] List<ManifestGenerateRequest> requests)
    {
        if (requests == null || requests.Count == 0)
            return BadRequest(new { error = "Nenhuma entidade selecionada" });

        var results = new List<object>();

        foreach (var request in requests)
        {
            try
            {
                var entity = await _manifestService.GetEntityAsync(request.EntityName);
                if (entity == null)
                {
                    results.Add(new { entityName = request.EntityName, success = false, error = "Não encontrada" });
                    continue;
                }

                var tabela = _manifestService.ConvertToTabelaInfo(entity);
                var fullRequest = _manifestService.ConvertToFullStackRequest(entity);

                // Apenas frontend
                fullRequest.GerarEntidade = false;
                fullRequest.GerarApiController = false;

                var result = _fullStackGenerator.Generate(tabela, fullRequest);

                results.Add(new
                {
                    entityName = request.EntityName,
                    success = result.Success,
                    error = result.Error,
                    filesCount = result.AllFiles.Count(),
                    warnings = result.Warnings
                });
            }
            catch (Exception ex)
            {
                results.Add(new { entityName = request.EntityName, success = false, error = ex.Message });
            }
        }

        return Json(new { results });
    }

    /// <summary>
    /// Download ZIP em lote.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DownloadLoteZip([FromBody] List<ManifestGenerateRequest> requests)
    {
        if (requests == null || requests.Count == 0)
            return BadRequest("Nenhuma entidade selecionada");

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var request in requests)
            {
                var entity = await _manifestService.GetEntityAsync(request.EntityName);
                if (entity == null) continue;

                var tabela = _manifestService.ConvertToTabelaInfo(entity);
                var fullRequest = _manifestService.ConvertToFullStackRequest(entity);

                fullRequest.GerarEntidade = false;
                fullRequest.GerarApiController = false;

                var result = _fullStackGenerator.Generate(tabela, fullRequest);
                if (!result.Success) continue;

                foreach (var file in result.AllFiles)
                {
                    var entry = archive.CreateEntry($"{result.NomeEntidade}/{file.RelativePath}");
                    using var entryStream = entry.Open();
                    using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                    await writer.WriteAsync(file.Content);
                }
            }
        }

        memoryStream.Position = 0;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return File(memoryStream.ToArray(), "application/zip", $"Frontend_Lote_{timestamp}.zip");
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    private static string GenerateReadme(FullStackResult result, EntityManifestItem entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {result.NomeEntidade} - Frontend Generated from Backend Manifest");
        sb.AppendLine();
        sb.AppendLine($"**Gerado em:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"**Entidade:** {entity.EntityName}");
        sb.AppendLine($"**Módulo:** {entity.ModuleName}");
        sb.AppendLine($"**Rota API:** {entity.Route}");
        sb.AppendLine($"**CdFuncao:** {entity.CdFuncao}");
        sb.AppendLine();

        sb.AppendLine("## ⚠️ IMPORTANTE");
        sb.AppendLine();
        sb.AppendLine("Este código foi gerado a partir do **manifesto do backend**.");
        sb.AppendLine("O backend já existe e está funcional. Estes arquivos são apenas o **FRONTEND**.");
        sb.AppendLine();

        sb.AppendLine("## Arquivos Gerados");
        sb.AppendLine();

        foreach (var file in result.AllFiles)
        {
            sb.AppendLine($"- `{file.RelativePath}` ({file.FileType})");
        }

        sb.AppendLine();
        sb.AppendLine("## Instruções de Instalação");
        sb.AppendLine();
        sb.AppendLine("1. Copie os arquivos para o projeto **RhSensoERP.Web**:");
        sb.AppendLine("   - `Web/Controllers/` → Controllers");
        sb.AppendLine("   - `Web/Models/` → Models");
        sb.AppendLine("   - `Web/Services/` → Services");
        sb.AppendLine("   - `Web/Views/` → Views");
        sb.AppendLine("   - `wwwroot/js/` → wwwroot/js");
        sb.AppendLine();
        sb.AppendLine("2. Registre o Service no DI (`Program.cs` ou `ServiceCollectionExtensions.cs`):");
        sb.AppendLine($"   ```csharp");
        sb.AppendLine($"   services.AddHttpClient<I{result.NomeEntidade}ApiService, {result.NomeEntidade}ApiService>();");
        sb.AppendLine($"   ```");
        sb.AppendLine();
        sb.AppendLine("3. A rota da API já está configurada corretamente:");
        sb.AppendLine($"   ```");
        sb.AppendLine($"   {entity.Route}");
        sb.AppendLine($"   ```");
        sb.AppendLine();

        if (result.Warnings.Count > 0)
        {
            sb.AppendLine("## Avisos");
            sb.AppendLine();
            foreach (var warning in result.Warnings)
            {
                sb.AppendLine($"⚠️ {warning}");
            }
        }

        return sb.ToString();
    }
}

/// <summary>
/// Request para geração de frontend a partir do manifesto.
/// </summary>
public class ManifestGenerateRequest
{
    public string EntityName { get; set; } = string.Empty;

    // Overrides opcionais
    public string? CdFuncao { get; set; }
    public string? DisplayName { get; set; }
    public string? Icone { get; set; }
    public int MenuOrder { get; set; } = 10;

    // Flags de geração (default: todos true para frontend)
    public bool GerarWebController { get; set; } = true;
    public bool GerarWebModels { get; set; } = true;
    public bool GerarWebServices { get; set; } = true;
    public bool GerarView { get; set; } = true;
    public bool GerarJavaScript { get; set; } = true;
}