// =============================================================================
// GENERATOR API CONTROLLER - Recebe dados do Wizard e gera código
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
    private readonly ILogger<GeneratorApiController> _logger;

    public GeneratorApiController(
        FullStackGeneratorService generatorService,
        ManifestService manifestService,
        ILogger<GeneratorApiController> logger)
    {
        _generatorService = generatorService;
        _manifestService = manifestService;
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