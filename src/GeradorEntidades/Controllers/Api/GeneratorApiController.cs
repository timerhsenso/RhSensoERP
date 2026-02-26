// =============================================================================
// GENERATOR API CONTROLLER - Recebe dados do Wizard e gera código
// ✅ v4.7: CORRIGIDO - Usa ModuleName do manifesto (não depende só do JS)
// =============================================================================

using GeradorEntidades.Models;
using GeradorEntidades.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace GeradorEntidades.Controllers.Api;

/// <summary>
/// API Controller que recebe configurações do Wizard e gera código usando os templates C#.
/// </summary>
[ApiController]
[Route("api/generator")]
public class GeneratorApiController : ControllerBase
{
    private readonly FullStackGeneratorService _generatorService;
    private readonly ManifestService _manifestService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeneratorApiController> _logger;

    public GeneratorApiController(
        FullStackGeneratorService generatorService,
        ManifestService manifestService,
        IConfiguration configuration,
        ILogger<GeneratorApiController> logger)
    {
        _generatorService = generatorService;
        _manifestService = manifestService;
        _configuration = configuration;
        _logger = logger;
    }

    // =========================================================================
    // POST /api/generator/generate
    // =========================================================================

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] WizardRequest request)
    {
        try
        {
            _logger.LogInformation("Gerando código para {Entity}", request.EntityName);

            // Validação
            if (string.IsNullOrWhiteSpace(request.EntityName))
            {
                return BadRequest(new WizardResponse
                {
                    Success = false,
                    Error = "Nome da entidade é obrigatório."
                });
            }

            if (string.IsNullOrWhiteSpace(request.CdFuncao))
            {
                return BadRequest(new WizardResponse
                {
                    Success = false,
                    Error = "CdFuncao é obrigatório."
                });
            }

            // Busca entidade do manifesto ou cria do request
            TabelaInfo tabela;
            var manifestEntity = await _manifestService.GetEntityAsync(request.EntityName);

            if (manifestEntity != null)
            {
                tabela = _manifestService.ConvertToTabelaInfo(manifestEntity);
                _logger.LogDebug("Entidade encontrada no manifesto: {Entity}", request.EntityName);
            }
            else
            {
                tabela = CreateTabelaInfoFromRequest(request);
                _logger.LogDebug("TabelaInfo criado do request: {Entity}", request.EntityName);
            }

            // Converte e gera
            var fullStackRequest = request.ToFullStackRequest();

            // ✅ v4.7: SUPLEMENTA módulo do manifesto (mais confiável que o JS)
            SupplementModuleFromManifest(fullStackRequest, manifestEntity);

            var result = _generatorService.Generate(tabela, fullStackRequest);

            // Resposta
            var response = new WizardResponse
            {
                Success = result.Success,
                Error = result.Error,
                Warnings = result.Warnings ?? new List<string>(),
                Files = result.AllFiles.Select(f => new WizardGeneratedFile
                {
                    FileName = f.FileName,
                    RelativePath = f.RelativePath,
                    FileType = f.FileType,
                    Content = f.Content
                }).ToList()
            };

            _logger.LogInformation("Geração concluída: {FileCount} arquivos", response.Files.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar código para {Entity}", request.EntityName);
            return StatusCode(500, new WizardResponse
            {
                Success = false,
                Error = $"Erro interno: {ex.Message}"
            });
        }
    }

    // =========================================================================
    // POST /api/generator/generate-to-disk
    // =========================================================================

    [HttpPost("generate-to-disk")]
    public async Task<IActionResult> GenerateToDisk([FromBody] WizardRequest request)
    {
        try
        {
            _logger.LogInformation("Gerando código no disco para {Entity}", request.EntityName);

            // 1. Get Params
            var outputPath = _configuration["GenerationSettings:OutputPath"];
            if (string.IsNullOrEmpty(outputPath))
                return BadRequest(new WizardResponse { Success = false, Error = "OutputPath não configurado no appsettings.json" });

            // Validação
            if (string.IsNullOrWhiteSpace(request.EntityName))
                return BadRequest(new WizardResponse { Success = false, Error = "Nome da entidade é obrigatório." });

            if (string.IsNullOrWhiteSpace(request.CdFuncao))
                return BadRequest(new WizardResponse { Success = false, Error = "CdFuncao é obrigatório." });

            // Busca entidade do manifesto ou cria do request
            TabelaInfo tabela;
            var manifestEntity = await _manifestService.GetEntityAsync(request.EntityName);

            if (manifestEntity != null)
                tabela = _manifestService.ConvertToTabelaInfo(manifestEntity);
            else
                tabela = CreateTabelaInfoFromRequest(request);

            // Converte e gera
            var fullStackRequest = request.ToFullStackRequest();

            // =====================================================================
            // ✅ v4.7: SUPLEMENTA módulo do manifesto (mais confiável que o JS)
            // O JS manda "Module" mas o C# espera "moduleName" → valor se perde.
            // O manifestEntity JÁ tem o ModuleName correto → usa ele.
            // =====================================================================
            SupplementModuleFromManifest(fullStackRequest, manifestEntity);

            _logger.LogInformation("📦 Módulo FINAL: '{Modulo}', ModuloRota: '{ModuloRota}', ApiRoute: '{ApiRoute}'",
                fullStackRequest.Modulo, fullStackRequest.ModuloRota, fullStackRequest.ApiRoute);

            var result = _generatorService.Generate(tabela, fullStackRequest);

            if (!result.Success)
                return BadRequest(new WizardResponse { Success = false, Error = result.Error });

            // 4. Write to Disk
            var savedFiles = new List<string>();
            foreach (var file in result.AllFiles)
            {
                var fullPath = Path.Combine(outputPath, file.RelativePath);

                var dir = Path.GetDirectoryName(fullPath);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                await System.IO.File.WriteAllTextAsync(fullPath, file.Content, System.Text.Encoding.UTF8);
                savedFiles.Add(fullPath);
            }

            // Resposta
            var response = new WizardResponse
            {
                Success = true,
                Warnings = result.Warnings ?? new List<string>(),
                Files = result.AllFiles.Select(f => new WizardGeneratedFile
                {
                    FileName = f.FileName,
                    RelativePath = f.RelativePath,
                    FileType = f.FileType
                }).ToList()
            };

            _logger.LogInformation("Geração executada no disco: {FileCount} arquivos em {OutputPath}", savedFiles.Count, outputPath);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar código no disco para {Entity}", request.EntityName);
            return StatusCode(500, new WizardResponse
            {
                Success = false,
                Error = $"Erro interno: {ex.Message}"
            });
        }
    }

    // =========================================================================
    // POST /api/generator/download-zip
    // =========================================================================

    [HttpPost("download-zip")]
    public async Task<IActionResult> DownloadZip([FromBody] WizardRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.EntityName))
                return BadRequest("Nome da entidade é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.CdFuncao))
                return BadRequest("CdFuncao é obrigatório.");

            // Busca ou cria TabelaInfo
            TabelaInfo tabela;
            var manifestEntity = await _manifestService.GetEntityAsync(request.EntityName);

            if (manifestEntity != null)
                tabela = _manifestService.ConvertToTabelaInfo(manifestEntity);
            else
                tabela = CreateTabelaInfoFromRequest(request);

            // Gera
            var fullStackRequest = request.ToFullStackRequest();

            // ✅ v4.7: SUPLEMENTA módulo do manifesto
            SupplementModuleFromManifest(fullStackRequest, manifestEntity);

            var result = _generatorService.Generate(tabela, fullStackRequest);

            if (!result.Success)
                return BadRequest(result.Error);

            // Cria ZIP
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in result.AllFiles)
                {
                    var entry = archive.CreateEntry(file.RelativePath, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    using var writer = new StreamWriter(entryStream);
                    await writer.WriteAsync(file.Content);
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/zip", $"{request.EntityName}_Frontend.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar ZIP");
            return StatusCode(500, $"Erro: {ex.Message}");
        }
    }

    // =========================================================================
    // ✅ v4.7 NOVO: Suplementa módulo do manifesto no FullStackRequest
    // =========================================================================
    // PROBLEMA RESOLVIDO:
    //   O JS envia chave "module" (ou "moduleName"), que após convertToPascalCase
    //   vira "Module" (ou "ModuleName"). Mas o C# espera [JsonPropertyName("moduleName")]
    //   no WizardRequest.Modulo. "Module" ≠ "moduleName" → valor NUNCA chega.
    //   
    //   SOLUÇÃO: O manifestEntity JÁ foi buscado pelo EntityName e tem o ModuleName
    //   correto. Usamos ele para preencher o que o JS não conseguiu passar.
    // =========================================================================

    private void SupplementModuleFromManifest(FullStackRequest fullStackRequest, EntityManifestItem? manifestEntity)
    {
        if (manifestEntity == null) return;

        // Se Modulo está vazio (JS não conseguiu passar), usa o do manifesto
        if (string.IsNullOrWhiteSpace(fullStackRequest.Modulo))
        {
            fullStackRequest.Modulo = manifestEntity.ModuleName;
            _logger.LogInformation("✅ Modulo suplementado do manifesto: '{Modulo}'", manifestEntity.ModuleName);
        }

        // Se ModuloRota está vazio, extrai do Route do manifesto
        if (string.IsNullOrWhiteSpace(fullStackRequest.ModuloRota) && !string.IsNullOrWhiteSpace(manifestEntity.Route))
        {
            var route = manifestEntity.Route.TrimStart('/');
            var parts = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var startIndex = parts.Length > 0 && parts[0].Equals("api", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            if (parts.Length > startIndex)
            {
                fullStackRequest.ModuloRota = parts[startIndex];
                _logger.LogInformation("✅ ModuloRota suplementado do manifesto: '{ModuloRota}'", parts[startIndex]);
            }
        }

        // Se ApiRoute está vazio, usa o do manifesto
        if (string.IsNullOrWhiteSpace(fullStackRequest.ApiRoute) && !string.IsNullOrWhiteSpace(manifestEntity.Route))
        {
            fullStackRequest.ApiRoute = manifestEntity.Route;
            _logger.LogInformation("✅ ApiRoute suplementado do manifesto: '{ApiRoute}'", manifestEntity.Route);
        }

        // Se CdFuncao está vazio, usa o do manifesto
        if (string.IsNullOrWhiteSpace(fullStackRequest.CdFuncao) && !string.IsNullOrWhiteSpace(manifestEntity.CdFuncao))
        {
            fullStackRequest.CdFuncao = manifestEntity.CdFuncao;
            _logger.LogInformation("✅ CdFuncao suplementado do manifesto: '{CdFuncao}'", manifestEntity.CdFuncao);
        }

        // Se DisplayName está vazio, usa o do manifesto
        if (string.IsNullOrWhiteSpace(fullStackRequest.DisplayName) && !string.IsNullOrWhiteSpace(manifestEntity.DisplayName))
        {
            fullStackRequest.DisplayName = manifestEntity.DisplayName;
            _logger.LogInformation("✅ DisplayName suplementado do manifesto: '{DisplayName}'", manifestEntity.DisplayName);
        }
    }

    // =========================================================================
    // HELPER
    // =========================================================================

    private static TabelaInfo CreateTabelaInfoFromRequest(WizardRequest request)
    {
        var tabela = new TabelaInfo
        {
            NomeTabela = request.TableName ?? $"TB_{request.EntityName.ToUpper()}",
            Schema = "dbo",
            Descricao = request.DisplayName ?? request.EntityName
        };

        var ordem = 0;
        foreach (var field in request.FormFields)
        {
            tabela.Colunas.Add(new ColunaInfo
            {
                Nome = field.Name,
                Tipo = MapTypeToSql(field.Type),
                IsNullable = !field.Required,
                IsPrimaryKey = field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase),
                IsIdentity = field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && field.Type == "int",
                Tamanho = field.MaxLength,
                OrdinalPosition = ordem++
            });
        }

        if (tabela.Colunas.Count == 0)
        {
            tabela.Colunas.Add(new ColunaInfo
            {
                Nome = "Id",
                Tipo = "int",
                IsPrimaryKey = true,
                IsIdentity = true
            });
        }

        return tabela;
    }

    private static string MapTypeToSql(string type) => type?.ToLower() switch
    {
        "string" => "varchar",
        "int" => "int",
        "long" => "bigint",
        "decimal" => "decimal",
        "bool" or "boolean" => "bit",
        "datetime" => "datetime",
        "guid" => "uniqueidentifier",
        _ => "varchar"
    };
}