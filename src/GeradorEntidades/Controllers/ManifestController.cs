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
    private readonly DatabaseService _dbService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ManifestController> _logger;

    public ManifestController(
        ManifestService manifestService,
        FullStackGeneratorService fullStackGenerator,
        DatabaseService dbService,
        IConfiguration configuration,
        ILogger<ManifestController> logger)
    {
        _manifestService = manifestService;
        _fullStackGenerator = fullStackGenerator;
        _dbService = dbService;
        _configuration = configuration;
        _logger = logger;
    }

    // =========================================================================
    // ✅ ENDPOINTS PARA O WIZARD - LISTAGEM DE MÓDULOS E ENTIDADES
    // =========================================================================

    /// <summary>
    /// Lista todos os módulos disponíveis.
    /// GET: /Manifest/GetModules
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetModules()
    {
        try
        {
            _logger.LogInformation("📦 Carregando lista de módulos");

            var modules = await _manifestService.GetModulesAsync();

            _logger.LogInformation("✅ {Count} módulos carregados", modules.Count);

            return Json(modules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao listar módulos");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Lista entidades de um módulo específico.
    /// GET: /Manifest/GetEntities?module=NomeModulo
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEntities([FromQuery] string module)
    {
        try
        {
            _logger.LogInformation("📋 Carregando entidades do módulo: {Module}", module);

            if (string.IsNullOrEmpty(module))
            {
                return BadRequest(new { error = "Parâmetro 'module' é obrigatório" });
            }

            var entities = await _manifestService.GetEntitiesByModuleAsync(module);

            _logger.LogInformation("✅ {Count} entidades encontradas em {Module}",
                entities.Count, module);

            return Json(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao listar entidades do módulo {Module}", module);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém detalhes completos de uma entidade.
    /// GET: /Manifest/GetEntityDetails?entityName=NomeEntidade
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEntityDetails([FromQuery] string entityName)
    {
        try
        {
            _logger.LogInformation("🔍 Carregando detalhes da entidade: {Entity}", entityName);

            if (string.IsNullOrEmpty(entityName))
            {
                return BadRequest(new { error = "Parâmetro 'entityName' é obrigatório" });
            }

            var entity = await _manifestService.GetEntityAsync(entityName);

            if (entity == null)
            {
                _logger.LogWarning("⚠️ Entidade '{Entity}' não encontrada", entityName);
                return NotFound(new { error = $"Entidade '{entityName}' não encontrada" });
            }

            _logger.LogInformation("✅ Entidade {Entity} carregada com {Props} propriedades",
                entityName, entity.Properties.Count);

            return Json(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao obter entidade {Entity}", entityName);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // =========================================================================
    // COMPARAÇÃO E GERAÇÃO EM DISCO
    // =========================================================================

    [HttpPost]
    public async Task<IActionResult> CompareEntity([FromBody] ManifestGenerateRequest request)
    {
        try
        {
            var entity = await _manifestService.GetEntityAsync(request.EntityName);
            if (entity == null)
                return NotFound(new { error = $"Entidade '{request.EntityName}' não encontrada no manifesto" });

            var dbTable = await _dbService.GetTabelaAsync(entity.TableName);
            var result = new EntityComparisonResult
            {
                EntityName = entity.EntityName,
                TableName = entity.TableName,
                TableExists = dbTable != null
            };

            if (dbTable != null)
            {
                // Verify Columns
                var dbColumns = dbTable.Colunas.ToDictionary(c => c.Nome, StringComparer.OrdinalIgnoreCase);

                foreach (var prop in entity.Properties)
                {
                    var colName = prop.ColumnName ?? prop.Name;
                    if (!dbColumns.TryGetValue(colName, out var dbCol))
                    {
                        result.MissingColumns.Add(colName);
                        continue;
                    }

                    // Simple Type Comparison
                    var apiTypeRaw = prop.Type.Replace("?", "");
                    var dbTypeRaw = dbCol.TipoCSharp.Replace("?", "");

                    // Normalize for comparison (e.g. Int32 vs int)
                    if (!AreTypesCompatible(apiTypeRaw, dbTypeRaw))
                    {
                        result.Mismatches.Add(new PropertyMismatch
                        {
                            PropertyName = prop.Name,
                            ColumnName = colName,
                            ApiType = prop.Type,
                            DbType = dbCol.TipoCSharp,
                            Message = $"Tipo incompatível: API espera {prop.Type}, Banco é {dbCol.TipoCSharp}"
                        });
                    }
                }

                // Check for extra columns in DB
                var entityCols = entity.Properties.Select(p => p.ColumnName ?? p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                foreach (var dbColName in dbColumns.Keys)
                {
                    if (!entityCols.Contains(dbColName))
                    {
                        result.ExtraColumns.Add(dbColName);
                    }
                }
            }

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao comparar entidade {Entity}", request.EntityName);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém detalhes de uma entidade (compatibilidade com JavaScript).
    /// GET: /Manifest/GetEntity?name=NomeEntidade
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEntity([FromQuery] string name)
    {
        try
        {
            _logger.LogInformation("🔍 Carregando entidade: {Entity}", name);

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new { error = "Parâmetro 'name' é obrigatório" });
            }

            var entity = await _manifestService.GetEntityAsync(name);

            if (entity == null)
            {
                _logger.LogWarning("⚠️ Entidade '{Entity}' não encontrada", name);
                return NotFound(new { error = $"Entidade '{name}' não encontrada" });
            }

            _logger.LogInformation("✅ Entidade {Entity} carregada com {Props} propriedades",
                name, entity.Properties.Count);

            return Json(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao obter entidade {Entity}", name);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToDisk([FromBody] ManifestGenerateRequest request)
    {
        try
        {
            // 1. Get Params
            var outputPath = _configuration["GenerationSettings:OutputPath"];
            if (string.IsNullOrEmpty(outputPath))
                return BadRequest(new { error = "OutputPath não configurado no appsettings.json" });

            // 2. Refresh Entity
            var entity = await _manifestService.GetEntityAsync(request.EntityName);
            if (entity == null) return NotFound($"Entidade '{request.EntityName}' não encontrada");

            // 3. Convert & Generate
            var tabela = _manifestService.ConvertToTabelaInfo(entity);
            var fullRequest = _manifestService.ConvertToFullStackRequest(entity);

            // Apply overrides
            if (!string.IsNullOrEmpty(request.CdFuncao)) fullRequest.CdFuncao = request.CdFuncao;
            if (!string.IsNullOrEmpty(request.DisplayName)) fullRequest.DisplayName = request.DisplayName;
            if (request.MenuOrder > 0) fullRequest.MenuOrder = request.MenuOrder;

            // Frontend Only
            fullRequest.GerarEntidade = false;
            fullRequest.GerarApiController = false;
            fullRequest.GerarWebController = request.GerarWebController;
            fullRequest.GerarWebModels = request.GerarWebModels;
            fullRequest.GerarWebServices = request.GerarWebServices;
            fullRequest.GerarView = request.GerarView;
            fullRequest.GerarJavaScript = request.GerarJavaScript;

            var result = _fullStackGenerator.Generate(tabela, fullRequest);
            if (!result.Success) return BadRequest(new { error = result.Error });

            // 4. Write to Disk
            var savedFiles = new List<string>();
            foreach (var file in result.AllFiles)
            {
                var fullPath = Path.Combine(outputPath, file.RelativePath);

                var dir = Path.GetDirectoryName(fullPath);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                await System.IO.File.WriteAllTextAsync(fullPath, file.Content, Encoding.UTF8);
                savedFiles.Add(fullPath);
            }

            return Json(new
            {
                success = true,
                message = $"{savedFiles.Count} arquivos gerados com sucesso em {outputPath}",
                files = savedFiles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar arquivos no disco");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    private bool AreTypesCompatible(string apiType, string dbType)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "int", "int" },
            { "Int32", "int" },
            { "long", "long" },
            { "Int64", "long" },
            { "string", "string" },
            { "bool", "bool" },
            { "Boolean", "bool" },
            { "DateTime", "DateTime" },
            { "decimal", "decimal" },
            { "double", "double" },
            { "Guid", "Guid" }
        };

        var normApi = map.TryGetValue(apiType, out var v1) ? v1 : apiType;
        var normDb = map.TryGetValue(dbType, out var v2) ? v2 : dbType;

        return normApi.Equals(normDb, StringComparison.OrdinalIgnoreCase);
    }

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