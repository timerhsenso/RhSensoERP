// =============================================================================
// TABSHEET GENERATOR v1.0 - CONTROLLER
// Versão: 1.0.1 (Rota alterada para evitar conflito com API v2)
// Autor: RhSensoERP Team
// 
// Endpoints para configuração e geração de TabSheets (versão original).
// NOTA: Use /api/tabsheet-v1 para esta versão
//       Use /api/tabsheet para a versão v2 (TabSheetApiController)
// =============================================================================

using System.IO.Compression;
using System.Text;
using System.Text.Json;
using GeradorEntidades.Models;
using GeradorEntidades.Services;
using GeradorEntidades.TabSheet.Models;
using GeradorEntidades.TabSheet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeradorEntidades.TabSheet.Controllers;

/// <summary>
/// Controller para geração de TabSheets v1.0 (telas mestre/detalhe).
/// Usa TabSheetConfiguration e templates separados.
/// </summary>
[Route("api/tabsheet-v1")]
[ApiController]
public class TabSheetController : ControllerBase
{
    private readonly ILogger<TabSheetController> _logger;
    private readonly TabSheetGeneratorService _tabSheetGenerator;
    private readonly DatabaseService _databaseService;

    public TabSheetController(
        ILogger<TabSheetController> logger,
        TabSheetGeneratorService tabSheetGenerator,
        DatabaseService databaseService)
    {
        _logger = logger;
        _tabSheetGenerator = tabSheetGenerator;
        _databaseService = databaseService;
    }

    #region Endpoints de Configuração

    /// <summary>
    /// Obtém configuração inicial para uma tabela mestre.
    /// </summary>
    /// <param name="tableName">Nome da tabela mestre.</param>
    [HttpGet("config/{tableName}")]
    public async Task<ActionResult<TabSheetConfigResponse>> GetConfiguration(string tableName)
    {
        try
        {
            var tabelas = await _databaseService.GetTabelasAsync();
            var masterTable = tabelas.FirstOrDefault(t =>
                t.NomeTabela.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            if (masterTable == null)
            {
                return NotFound(new { error = $"Tabela '{tableName}' não encontrada." });
            }

            // Buscar tabelas que podem ser detalhes (têm FK para o mestre)
            var possibleDetails = new List<PossibleDetailTable>();
            foreach (var tabela in tabelas)
            {
                var fkToMaster = tabela.ForeignKeys.FirstOrDefault(fk =>
                    fk.TabelaDestino.Equals(tableName, StringComparison.OrdinalIgnoreCase));

                if (fkToMaster != null)
                {
                    possibleDetails.Add(new PossibleDetailTable
                    {
                        TableName = tabela.NomeTabela,
                        Schema = tabela.Schema,
                        EntityName = tabela.NomePascalCase,
                        ForeignKey = fkToMaster.ColunaOrigem,
                        PrimaryKey = tabela.PrimaryKey?.Nome ?? "",
                        ColumnCount = tabela.Colunas.Count,
                        Description = tabela.Descricao
                    });
                }
            }

            var response = new TabSheetConfigResponse
            {
                MasterTable = new MasterTableInfo
                {
                    TableName = masterTable.NomeTabela,
                    Schema = masterTable.Schema,
                    EntityName = masterTable.NomePascalCase,
                    PrimaryKey = masterTable.PrimaryKey?.Nome ?? "",
                    PrimaryKeyType = masterTable.PrimaryKey?.TipoCSharp ?? "int",
                    DisplayColumn = DetectDisplayColumn(masterTable),
                    ColumnCount = masterTable.Colunas.Count,
                    Description = masterTable.Descricao
                },
                PossibleDetails = possibleDetails,
                AvailableIcons = GetAvailableIcons(),
                AvailableModules = GetAvailableModules()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter configuração para: {Table}", tableName);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Valida uma configuração de TabSheet.
    /// </summary>
    [HttpPost("validate")]
    public ActionResult<ValidationResponse> ValidateConfiguration([FromBody] TabSheetConfiguration config)
    {
        var response = new ValidationResponse
        {
            IsValid = config.IsValid,
            Errors = config.ValidationErrors
        };

        return Ok(response);
    }

    #endregion

    #region Endpoints de Geração

    /// <summary>
    /// Gera o TabSheet e retorna os arquivos.
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<TabSheetGenerationResult>> Generate([FromBody] TabSheetConfiguration config)
    {
        try
        {
            _logger.LogInformation("Gerando TabSheet v1: {Id}", config.Id);

            var result = await _tabSheetGenerator.GenerateAsync(config);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar TabSheet: {Id}", config.Id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gera o TabSheet e retorna como ZIP para download.
    /// </summary>
    [HttpPost("generate/zip")]
    public async Task<IActionResult> GenerateZip([FromBody] TabSheetConfiguration config)
    {
        try
        {
            _logger.LogInformation("Gerando TabSheet ZIP v1: {Id}", config.Id);

            var result = await _tabSheetGenerator.GenerateAsync(config);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            // Criar ZIP em memória
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in result.GeneratedFiles)
                {
                    var entryPath = Path.Combine(file.RelativePath, file.FileName).Replace("\\", "/");
                    var entry = archive.CreateEntry(entryPath, CompressionLevel.Optimal);

                    using var entryStream = entry.Open();
                    using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                    await writer.WriteAsync(file.Content);
                }

                // Adicionar README
                var readmeEntry = archive.CreateEntry("README.md", CompressionLevel.Optimal);
                using (var readmeStream = readmeEntry.Open())
                using (var writer = new StreamWriter(readmeStream, Encoding.UTF8))
                {
                    await writer.WriteAsync(GenerateReadme(config, result));
                }
            }

            memoryStream.Position = 0;
            var zipBytes = memoryStream.ToArray();

            var fileName = $"TabSheet_{config.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            return File(zipBytes, "application/zip", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar TabSheet ZIP: {Id}", config.Id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém preview de um arquivo específico.
    /// </summary>
    [HttpPost("preview/{fileType}")]
    public async Task<ActionResult<FilePreviewResponse>> GetPreview(
        string fileType,
        [FromBody] TabSheetConfiguration config)
    {
        try
        {
            var result = await _tabSheetGenerator.GenerateAsync(config);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            var file = result.GeneratedFiles.FirstOrDefault(f =>
                f.FileType.ToString().Equals(fileType, StringComparison.OrdinalIgnoreCase) &&
                f.IsMasterFile);

            if (file == null)
            {
                file = result.GeneratedFiles.FirstOrDefault(f =>
                    f.FileType.ToString().Equals(fileType, StringComparison.OrdinalIgnoreCase));
            }

            if (file == null)
            {
                return NotFound(new { error = $"Arquivo do tipo '{fileType}' não encontrado." });
            }

            return Ok(new FilePreviewResponse
            {
                FileName = file.FileName,
                RelativePath = file.RelativePath,
                Content = file.Content,
                FileType = file.FileType.ToString(),
                ContentSize = file.ContentSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar preview");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Helpers

    private static string? DetectDisplayColumn(TabelaInfo tabela)
    {
        var candidatos = new[] { "nome", "name", "descricao", "description", "titulo", "title", "dc", "nm" };

        foreach (var candidato in candidatos)
        {
            var coluna = tabela.Colunas.FirstOrDefault(c =>
                c.Nome.Contains(candidato, StringComparison.OrdinalIgnoreCase));
            if (coluna != null)
            {
                return coluna.Nome;
            }
        }

        return tabela.Colunas.FirstOrDefault(c => c.IsTexto)?.Nome;
    }

    private static List<IconOption> GetAvailableIcons()
    {
        return new List<IconOption>
        {
            new("fas fa-table", "Tabela"),
            new("fas fa-list", "Lista"),
            new("fas fa-users", "Usuários"),
            new("fas fa-user", "Usuário"),
            new("fas fa-file", "Arquivo"),
            new("fas fa-folder", "Pasta"),
            new("fas fa-calendar", "Calendário"),
            new("fas fa-clock", "Relógio"),
            new("fas fa-bell", "Notificação"),
            new("fas fa-cog", "Configuração"),
            new("fas fa-chart-bar", "Gráfico"),
            new("fas fa-dollar-sign", "Dinheiro"),
            new("fas fa-briefcase", "Trabalho"),
            new("fas fa-graduation-cap", "Educação"),
            new("fas fa-heart", "Saúde"),
            new("fas fa-home", "Casa"),
            new("fas fa-car", "Veículo"),
            new("fas fa-phone", "Telefone"),
            new("fas fa-envelope", "E-mail"),
            new("fas fa-map-marker-alt", "Localização"),
            new("fas fa-tags", "Tags"),
            new("fas fa-star", "Favorito"),
            new("fas fa-check", "Check"),
            new("fas fa-times", "Fechar"),
            new("fas fa-plus", "Adicionar"),
            new("fas fa-minus", "Remover"),
            new("fas fa-edit", "Editar"),
            new("fas fa-trash", "Excluir"),
            new("fas fa-search", "Pesquisar"),
            new("fas fa-filter", "Filtrar")
        };
    }

    private static List<ModuleOption> GetAvailableModules()
    {
        return new List<ModuleOption>
        {
            new("GestaoDePessoas", "gestaodepessoas", "Gestão de Pessoas"),
            new("ControleDePonto", "controledeponto", "Controle de Ponto"),
            new("Treinamentos", "treinamentos", "Treinamentos"),
            new("SaudeOcupacional", "saudeocupacional", "Saúde Ocupacional"),
            new("Seguranca", "seguranca", "Segurança"),
            new("Folha", "folha", "Folha de Pagamento"),
            new("Beneficios", "beneficios", "Benefícios"),
            new("Recrutamento", "recrutamento", "Recrutamento")
        };
    }

    private static string GenerateReadme(TabSheetConfiguration config, TabSheetGenerationResult result)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {config.Title}");
        sb.AppendLine();
        sb.AppendLine($"Gerado em: {result.GeneratedAt:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"Tempo de geração: {result.ElapsedMilliseconds}ms");
        sb.AppendLine();

        sb.AppendLine("## Tabela Mestre");
        sb.AppendLine();
        sb.AppendLine($"- **Tabela:** {config.MasterTable.TableName}");
        sb.AppendLine($"- **Entidade:** {config.MasterTable.EntityName}");
        sb.AppendLine($"- **PK:** {config.MasterTable.PrimaryKey}");
        sb.AppendLine();

        sb.AppendLine("## Abas (Detalhes)");
        sb.AppendLine();
        foreach (var tab in config.Tabs)
        {
            sb.AppendLine($"### {tab.Title}");
            sb.AppendLine($"- **Tabela:** {tab.TableName}");
            sb.AppendLine($"- **FK:** {tab.ForeignKey}");
            sb.AppendLine($"- **Ícone:** {tab.Icon}");
            sb.AppendLine();
        }

        sb.AppendLine("## Arquivos Gerados");
        sb.AppendLine();
        sb.AppendLine("| Arquivo | Tipo | Tamanho |");
        sb.AppendLine("|---------|------|---------|");
        foreach (var file in result.GeneratedFiles)
        {
            sb.AppendLine($"| {file.RelativePath}{file.FileName} | {file.FileType} | {file.ContentSize} bytes |");
        }

        if (result.Warnings.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("## Avisos");
            sb.AppendLine();
            foreach (var warning in result.Warnings)
            {
                sb.AppendLine($"- ⚠️ {warning}");
            }
        }

        return sb.ToString();
    }

    #endregion
}

#region DTOs

public class TabSheetConfigResponse
{
    public MasterTableInfo MasterTable { get; set; } = new();
    public List<PossibleDetailTable> PossibleDetails { get; set; } = [];
    public List<IconOption> AvailableIcons { get; set; } = [];
    public List<ModuleOption> AvailableModules { get; set; } = [];
}

public class MasterTableInfo
{
    public string TableName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string EntityName { get; set; } = string.Empty;
    public string PrimaryKey { get; set; } = string.Empty;
    public string PrimaryKeyType { get; set; } = "int";
    public string? DisplayColumn { get; set; }
    public int ColumnCount { get; set; }
    public string? Description { get; set; }
}

public class PossibleDetailTable
{
    public string TableName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string EntityName { get; set; } = string.Empty;
    public string ForeignKey { get; set; } = string.Empty;
    public string PrimaryKey { get; set; } = string.Empty;
    public int ColumnCount { get; set; }
    public string? Description { get; set; }
}

public class IconOption
{
    public string Value { get; set; }
    public string Label { get; set; }

    public IconOption(string value, string label)
    {
        Value = value;
        Label = label;
    }
}

public class ModuleOption
{
    public string Module { get; set; }
    public string Route { get; set; }
    public string Label { get; set; }

    public ModuleOption(string module, string route, string label)
    {
        Module = module;
        Route = route;
        Label = label;
    }
}

public class ValidationResponse
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class FilePreviewResponse
{
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public int ContentSize { get; set; }
}

#endregion