// =============================================================================
// TABSHEET GENERATOR v2.0 - API CONTROLLER COMPLETO
// Usa os templates existentes para gerar TODOS os arquivos
// Baseado em: WebModelsTemplate, WebServicesTemplate, WebControllerTemplate,
//             ViewTemplate, JavaScriptTemplate
// =============================================================================

using System.IO.Compression;
using System.Text;
using System.Text.Json;
using GeradorEntidades.Models;
using GeradorEntidades.Services;
using GeradorEntidades.Templates;
using GeradorEntidades.TabSheet.Models;
using GeradorEntidades.TabSheet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeradorEntidades.TabSheet.Controllers;

/// <summary>
/// Controller REST API para o TabSheet Generator v2.0
/// Geração de telas Mestre/Detalhe com CRUD completo usando templates existentes
/// </summary>
[ApiController]
[Route("api/tabsheet")]
public class TabSheetApiController : ControllerBase
{
    private readonly DatabaseService _databaseService;
    private readonly TabSheetGeneratorService _generatorService;
    private readonly ILogger<TabSheetApiController> _logger;

    public TabSheetApiController(
        DatabaseService databaseService,
        TabSheetGeneratorService generatorService,
        ILogger<TabSheetApiController> logger)
    {
        _databaseService = databaseService;
        _generatorService = generatorService;
        _logger = logger;
    }

    // =========================================================================
    // LISTAGEM DE TABELAS
    // =========================================================================

    /// <summary>
    /// Lista todas as tabelas disponíveis no banco de dados
    /// </summary>
    [HttpGet("tables")]
    public async Task<IActionResult> GetTables()
    {
        try
        {
            _logger.LogInformation("Listando tabelas disponíveis...");

            var tabelas = await _databaseService.GetTabelasAsync();

            var result = tabelas.Select(t => new
            {
                nomeTabela = t.NomeTabela,
                schema = t.Schema,
                nomePascalCase = t.NomePascalCase,
                colunas = t.Colunas.Select(c => new
                {
                    nome = c.Nome,
                    tipo = c.Tipo,
                    isPrimaryKey = c.IsPrimaryKey,
                    isForeignKey = c.ForeignKey != null
                }).ToList(),
                primaryKey = t.PrimaryKey?.Nome,
                descricao = t.Descricao
            }).OrderBy(t => t.nomeTabela).ToList();

            _logger.LogInformation("Retornando {Count} tabelas", result.Count);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tabelas");
            return StatusCode(500, new { error = "Erro ao carregar lista de tabelas" });
        }
    }

    // =========================================================================
    // METADADOS DA TABELA
    // =========================================================================

    /// <summary>
    /// Obtém metadados completos de uma tabela (colunas, FKs, tabelas relacionadas)
    /// </summary>
    [HttpGet("metadata/{tableName}")]
    public async Task<IActionResult> GetTableMetadata(string tableName)
    {
        try
        {
            _logger.LogInformation("Buscando metadados da tabela: {TableName}", tableName);

            var tabelas = await _databaseService.GetTabelasAsync();
            var tabela = tabelas.FirstOrDefault(t =>
                t.NomeTabela.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            if (tabela == null)
            {
                return NotFound(new { error = $"Tabela '{tableName}' não encontrada" });
            }

            var columns = tabela.Colunas.Select(c => new ColumnMetadataDto
            {
                Name = c.Nome,
                Type = c.Tipo,
                MaxLength = c.Tamanho,
                IsPrimaryKey = c.IsPrimaryKey,
                IsForeignKey = c.ForeignKey != null,
                IsIdentity = c.IsIdentity,
                IsNullable = c.IsNullable,
                DefaultValue = c.DefaultValue,
                ReferencedTable = c.ForeignKey?.TabelaDestino,
                ReferencedColumn = c.ForeignKey?.ColunaDestino
            }).ToList();

            var foreignKeys = tabela.Colunas
                .Where(c => c.ForeignKey != null)
                .Select(c => new ForeignKeyDto
                {
                    Column = c.Nome,
                    ReferencedTable = c.ForeignKey!.TabelaDestino,
                    ReferencedColumn = c.ForeignKey.ColunaDestino
                }).ToList();

            var relatedTables = new List<RelatedTableDto>();
            var primaryKey = tabela.Colunas.FirstOrDefault(c => c.IsPrimaryKey)?.Nome ?? "id";

            foreach (var outraTabela in tabelas.Where(t => t.NomeTabela != tableName))
            {
                var fkParaEsta = outraTabela.Colunas.FirstOrDefault(c =>
                    c.ForeignKey != null &&
                    c.ForeignKey.TabelaDestino.Equals(tableName, StringComparison.OrdinalIgnoreCase));

                if (fkParaEsta != null)
                {
                    relatedTables.Add(new RelatedTableDto
                    {
                        TableName = outraTabela.NomeTabela,
                        FkColumn = fkParaEsta.Nome,
                        ColumnCount = outraTabela.Colunas.Count
                    });
                }
            }

            var result = new TableMetadataDto
            {
                TableName = tabela.NomeTabela,
                Schema = tabela.Schema,
                PrimaryKey = primaryKey,
                Columns = columns,
                ForeignKeys = foreignKeys,
                RelatedTables = relatedTables
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar metadados da tabela {TableName}", tableName);
            return StatusCode(500, new { error = "Erro interno ao processar requisição" });
        }
    }

    /// <summary>
    /// Obtém apenas as colunas de uma tabela
    /// </summary>
    [HttpGet("columns/{tableName}")]
    public async Task<IActionResult> GetTableColumns(string tableName)
    {
        try
        {
            var tabelas = await _databaseService.GetTabelasAsync();
            var tabela = tabelas.FirstOrDefault(t =>
                t.NomeTabela.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            if (tabela == null)
            {
                return NotFound(new { error = $"Tabela '{tableName}' não encontrada" });
            }

            var columns = tabela.Colunas.Select(c => new ColumnMetadataDto
            {
                Name = c.Nome,
                Type = c.Tipo,
                MaxLength = c.Tamanho,
                IsPrimaryKey = c.IsPrimaryKey,
                IsForeignKey = c.ForeignKey != null,
                IsIdentity = c.IsIdentity,
                IsNullable = c.IsNullable,
                DefaultValue = c.DefaultValue,
                ReferencedTable = c.ForeignKey?.TabelaDestino,
                ReferencedColumn = c.ForeignKey?.ColunaDestino
            }).ToList();

            return Ok(columns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar colunas da tabela {TableName}", tableName);
            return StatusCode(500, new { error = "Erro interno ao processar requisição" });
        }
    }

    // =========================================================================
    // GERAÇÃO DE CÓDIGO - USANDO TEMPLATES EXISTENTES
    // =========================================================================

    /// <summary>
    /// Gera código COMPLETO baseado na configuração do TabSheet
    /// Usa os templates existentes: WebModelsTemplate, WebServicesTemplate, etc.
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCode([FromBody] TabSheetConfigurationDto config)
    {
        try
        {
            _logger.LogInformation("Iniciando geração COMPLETA para: {ConfigId}", config.Id);

            // Validar configuração
            var validationErrors = ValidateConfiguration(config);
            if (validationErrors.Any())
            {
                return BadRequest(new { errors = validationErrors });
            }

            // Buscar metadados da tabela do banco
            var tabelas = await _databaseService.GetTabelasAsync();
            var tabelaMestre = tabelas.FirstOrDefault(t =>
                t.NomeTabela.Equals(config.MasterTable!.TableName, StringComparison.OrdinalIgnoreCase));

            if (tabelaMestre == null)
            {
                return NotFound(new { error = $"Tabela '{config.MasterTable!.TableName}' não encontrada" });
            }

            // Converter TabSheetConfigurationDto para EntityConfig
            var entityConfig = ConvertToEntityConfig(config, tabelaMestre);

            // Gerar arquivos usando templates existentes
            var files = GenerateAllFilesUsingTemplates(entityConfig, config);

            // Gerar arquivos para tabelas detalhe (abas)
            if (config.Tabs != null && config.Tabs.Any())
            {
                foreach (var tab in config.Tabs)
                {
                    var tabelaDetalhe = tabelas.FirstOrDefault(t =>
                        t.NomeTabela.Equals(tab.TableName, StringComparison.OrdinalIgnoreCase));

                    if (tabelaDetalhe != null)
                    {
                        var detailConfig = ConvertTabToEntityConfig(tab, tabelaDetalhe, config);
                        var detailFiles = GenerateDetailFiles(detailConfig, tab, config);
                        files.AddRange(detailFiles);
                    }
                }
            }

            // Criar ZIP em memória
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var entry = archive.CreateEntry(file.RelativePath, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    var bytes = Encoding.UTF8.GetBytes(file.Content);
                    await entryStream.WriteAsync(bytes);
                }

                // Adicionar arquivo de configuração JSON
                var configJson = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                var configEntry = archive.CreateEntry($"{config.Id}.tabsheet.json", CompressionLevel.Optimal);
                using var configStream = configEntry.Open();
                await configStream.WriteAsync(Encoding.UTF8.GetBytes(configJson));
            }

            memoryStream.Position = 0;

            _logger.LogInformation("Geração concluída: {FileCount} arquivos gerados", files.Count);

            return File(memoryStream.ToArray(), "application/zip", $"{config.Id}.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar código para {ConfigId}", config?.Id);
            return StatusCode(500, new { error = "Erro interno ao processar requisição", details = ex.Message });
        }
    }

    // =========================================================================
    // CONVERSÃO: TabSheetConfigurationDto → EntityConfig
    // =========================================================================

    /// <summary>
    /// Converte TabSheetConfigurationDto para EntityConfig usando TabelaInfo do banco
    /// </summary>
    private EntityConfig ConvertToEntityConfig(TabSheetConfigurationDto config, TabelaInfo tabela)
    {
        var options = config.Options ?? new GenerationOptionsDto();

        var entityConfig = new EntityConfig
        {
            Name = tabela.NomePascalCase,
            PluralName = tabela.NomePlural,
            DisplayName = config.Title,
            TableName = tabela.NomeTabela.ToLower(),
            CdFuncao = options.CdFuncao ?? $"CRUD_{tabela.NomePascalCase.ToUpper()}",
            CdSistema = options.CdSistema ?? "RHU",
            Module = config.Module ?? "GestaoDePessoas",
            ModuleRoute = options.ModuleRoute ?? config.Module?.ToLower() ?? "gestaodepessoas"
        };

        // Mapear propriedades a partir das configurações do TabSheet
        var listOrder = 0;
        var formOrder = 0;

        foreach (var coluna in tabela.Colunas)
        {
            // Buscar configuracao de listagem
            var listConfig = config.MasterTable?.Listagem?
                .FirstOrDefault(c => c.Column.Equals(coluna.Nome, StringComparison.OrdinalIgnoreCase));

            // Buscar configuracao de formulario
            var formConfig = config.MasterTable?.Formulario?
                .FirstOrDefault(c => c.Column.Equals(coluna.Nome, StringComparison.OrdinalIgnoreCase));

            // CORRIGIDO: Se coluna nao foi selecionada em nenhum lugar, pular
            if (listConfig == null && formConfig == null)
            {
                continue;
            }

            var prop = new PropertyConfig
            {
                Name = coluna.NomePascalCase,
                ColumnName = coluna.Nome,
                CSharpType = coluna.TipoCSharp,
                CSharpTypeSimple = coluna.TipoCSharpSimples,
                SqlType = coluna.Tipo,
                DisplayName = listConfig?.Title ?? formConfig?.Label ?? FormatDisplayName(coluna.NomePascalCase),
                IsPrimaryKey = coluna.IsPrimaryKey,
                IsIdentity = coluna.IsIdentity,
                IsNullable = coluna.IsNullable,
                IsReadOnly = coluna.IsComputed || coluna.IsIdentity,
                Required = formConfig?.Required ?? (!coluna.IsNullable && !coluna.IsPrimaryKey),
                MaxLength = coluna.Tamanho,
                IsGuid = coluna.IsGuid,
                IsString = coluna.IsString,
                IsInt = coluna.IsInt,
                IsLong = coluna.IsLong,
                IsDecimal = coluna.IsDecimal,
                IsBool = coluna.IsBool,
                IsDateTime = coluna.IsData,
                HasForeignKey = coluna.ForeignKey != null,
                ForeignKeyTable = coluna.ForeignKey?.TabelaDestino,
                ForeignKeyColumn = coluna.ForeignKey?.ColunaDestino
            };

            // Configuração de listagem
            if (listConfig != null)
            {
                prop.List = new ListConfig
                {
                    Show = true,
                    Order = listOrder++,
                    Title = listConfig.Title,
                    Format = listConfig.Format ?? "text",
                    Width = listConfig.Width,
                    Align = listConfig.Align ?? "left",
                    Sortable = listConfig.Sortable
                };
            }

            // Configuração de formulário
            if (formConfig != null)
            {
                prop.Form = new FormConfig
                {
                    Show = true,
                    Order = formOrder++,
                    InputType = formConfig.Type ?? GetDefaultInputType(coluna),
                    ColSize = formConfig.ColSize > 0 ? formConfig.ColSize : 6,
                    Disabled = formConfig.Disabled
                };
            }

            entityConfig.Properties.Add(prop);

            // Definir PrimaryKey
            if (coluna.IsPrimaryKey && entityConfig.PrimaryKey == null)
            {
                entityConfig.PrimaryKey = prop;
            }
        }

        return entityConfig;
    }

    /// <summary>
    /// Converte TabConfigDto (aba) para EntityConfig
    /// </summary>
    private EntityConfig ConvertTabToEntityConfig(TabConfigDto tab, TabelaInfo tabela, TabSheetConfigurationDto parentConfig)
    {
        var entityConfig = new EntityConfig
        {
            Name = tabela.NomePascalCase,
            PluralName = tabela.NomePlural,
            DisplayName = tab.Title,
            TableName = tabela.NomeTabela.ToLower(),
            CdFuncao = $"CRUD_{tabela.NomePascalCase.ToUpper()}",
            CdSistema = parentConfig.Options?.CdSistema ?? "RHU",
            Module = parentConfig.Module ?? "GestaoDePessoas",
            ModuleRoute = parentConfig.Options?.ModuleRoute ?? parentConfig.Module?.ToLower() ?? "gestaodepessoas"
        };

        var listOrder = 0;
        var formOrder = 0;

        foreach (var coluna in tabela.Colunas)
        {
            var listConfig = tab.Listagem?.FirstOrDefault(c => c.Column.Equals(coluna.Nome, StringComparison.OrdinalIgnoreCase));
            var formConfig = tab.Formulario?.FirstOrDefault(c => c.Column.Equals(coluna.Nome, StringComparison.OrdinalIgnoreCase));

            var prop = new PropertyConfig
            {
                Name = coluna.NomePascalCase,
                ColumnName = coluna.Nome,
                CSharpType = coluna.TipoCSharp,
                CSharpTypeSimple = coluna.TipoCSharpSimples,
                SqlType = coluna.Tipo,
                DisplayName = listConfig?.Title ?? formConfig?.Label ?? FormatDisplayName(coluna.NomePascalCase),
                IsPrimaryKey = coluna.IsPrimaryKey,
                IsIdentity = coluna.IsIdentity,
                IsNullable = coluna.IsNullable,
                IsReadOnly = coluna.IsComputed || coluna.IsIdentity,
                Required = formConfig?.Required ?? (!coluna.IsNullable && !coluna.IsPrimaryKey),
                MaxLength = coluna.Tamanho,
                IsGuid = coluna.IsGuid,
                IsString = coluna.IsString,
                IsInt = coluna.IsInt,
                IsLong = coluna.IsLong,
                IsDecimal = coluna.IsDecimal,
                IsBool = coluna.IsBool,
                IsDateTime = coluna.IsData,
                HasForeignKey = coluna.ForeignKey != null,
                ForeignKeyTable = coluna.ForeignKey?.TabelaDestino,
                ForeignKeyColumn = coluna.ForeignKey?.ColunaDestino
            };

            if (listConfig != null)
            {
                prop.List = new ListConfig
                {
                    Show = true,
                    Order = listOrder++,
                    Title = listConfig.Title,
                    Format = listConfig.Format ?? "text",
                    Width = listConfig.Width,
                    Align = listConfig.Align ?? "left",
                    Sortable = listConfig.Sortable
                };
            }

            if (formConfig != null)
            {
                prop.Form = new FormConfig
                {
                    Show = true,
                    Order = formOrder++,
                    InputType = formConfig.Type ?? GetDefaultInputType(coluna),
                    ColSize = formConfig.ColSize > 0 ? formConfig.ColSize : 6,
                    Disabled = formConfig.Disabled
                };
            }

            entityConfig.Properties.Add(prop);

            if (coluna.IsPrimaryKey && entityConfig.PrimaryKey == null)
            {
                entityConfig.PrimaryKey = prop;
            }
        }

        return entityConfig;
    }

    // =========================================================================
    // GERAÇÃO USANDO TEMPLATES EXISTENTES
    // =========================================================================

    /// <summary>
    /// Gera todos os arquivos usando os templates existentes
    /// </summary>
    private List<GeneratedFile> GenerateAllFilesUsingTemplates(EntityConfig entity, TabSheetConfigurationDto config)
    {
        var files = new List<GeneratedFile>();
        var options = config.Options ?? new GenerationOptionsDto();

        _logger.LogDebug("Gerando arquivos para {Entity} com opções: {@Options}", entity.Name, options);

        // =====================================================================
        // 1. MODELS (DTOs) - WebModelsTemplate
        // =====================================================================
        if (options.GenerateDTOs)
        {
            _logger.LogDebug("Gerando DTOs...");

            // DTO de leitura
            files.Add(WebModelsTemplate.GenerateDto(entity));

            // Request de criação
            files.Add(WebModelsTemplate.GenerateCreateRequest(entity));

            // Request de atualização
            files.Add(WebModelsTemplate.GenerateUpdateRequest(entity));

            // ListViewModel
            files.Add(WebModelsTemplate.GenerateListViewModel(entity));
        }

        // =====================================================================
        // 2. SERVICES - WebServicesTemplate
        // =====================================================================
        if (options.GenerateServices)
        {
            _logger.LogDebug("Gerando Services...");

            // Interface do Service
            files.Add(WebServicesTemplate.GenerateInterface(entity));

            // Implementação do Service
            files.Add(WebServicesTemplate.GenerateImplementation(entity));
        }

        // =====================================================================
        // 3. CONTROLLER - WebControllerTemplate
        // =====================================================================
        if (options.GenerateController)
        {
            _logger.LogDebug("Gerando Controller...");
            files.Add(WebControllerTemplate.Generate(entity));
        }

        // =====================================================================
        // 4. VIEW - ViewTemplate
        // =====================================================================
        if (options.GenerateMasterView)
        {
            _logger.LogDebug("Gerando View...");
            files.Add(ViewTemplate.Generate(entity));
        }

        // =====================================================================
        // 5. JAVASCRIPT - JavaScriptTemplate
        // =====================================================================
        if (options.GenerateJavaScript)
        {
            _logger.LogDebug("Gerando JavaScript...");
            files.Add(JavaScriptTemplate.Generate(entity));
        }

        _logger.LogInformation("Gerados {Count} arquivos para {Entity}", files.Count, entity.Name);

        return files;
    }

    /// <summary>
    /// Gera arquivos para tabela detalhe (aba)
    /// </summary>
    private List<GeneratedFile> GenerateDetailFiles(EntityConfig entity, TabConfigDto tab, TabSheetConfigurationDto parentConfig)
    {
        var files = new List<GeneratedFile>();
        var options = parentConfig.Options ?? new GenerationOptionsDto();

        if (options.GenerateDetailEntities)
        {
            // DTOs do detalhe
            files.Add(WebModelsTemplate.GenerateDto(entity));
            files.Add(WebModelsTemplate.GenerateCreateRequest(entity));
            files.Add(WebModelsTemplate.GenerateUpdateRequest(entity));
        }

        // Partial View para a aba
        if (options.GenerateTabPartials)
        {
            files.Add(GenerateTabPartialView(entity, tab, parentConfig));
        }

        return files;
    }

    /// <summary>
    /// Gera Partial View para uma aba
    /// </summary>
    private GeneratedFile GenerateTabPartialView(EntityConfig entity, TabConfigDto tab, TabSheetConfigurationDto parentConfig)
    {
        var masterName = ToPascalCase(parentConfig.MasterTable!.TableName);
        var sb = new StringBuilder();

        sb.AppendLine($"@* Partial View para aba {tab.Title} - Gerado pelo TabSheet v2.0 *@");
        sb.AppendLine();
        sb.AppendLine("<div class=\"d-flex justify-content-between align-items-center mb-3\">");
        sb.AppendLine($"    <h5><i class=\"{tab.Icon} me-2\"></i>{tab.Title}</h5>");

        if (tab.AllowCreate)
        {
            sb.AppendLine($"    <button class=\"btn btn-success btn-sm\" onclick=\"abrirModal{entity.Name}()\">");
            sb.AppendLine("        <i class=\"fas fa-plus me-1\"></i>Adicionar");
            sb.AppendLine("    </button>");
        }

        sb.AppendLine("</div>");
        sb.AppendLine();
        sb.AppendLine($"<table id=\"tbl{entity.Name}\" class=\"table table-striped table-hover table-sm\">");
        sb.AppendLine("    <thead>");
        sb.AppendLine("        <tr>");

        var listProps = entity.Properties.Where(p => p.List?.Show == true).OrderBy(p => p.List!.Order);
        foreach (var prop in listProps)
        {
            var align = prop.List!.Align == "center" ? " class=\"text-center\"" :
                       prop.List.Align == "right" ? " class=\"text-end\"" : "";
            sb.AppendLine($"            <th{align}>{prop.List.Title ?? prop.DisplayName}</th>");
        }

        if (tab.AllowEdit || tab.AllowDelete)
        {
            sb.AppendLine("            <th class=\"text-center\" style=\"width: 80px;\">Ações</th>");
        }

        sb.AppendLine("        </tr>");
        sb.AppendLine("    </thead>");
        sb.AppendLine("</table>");

        return new GeneratedFile
        {
            FileName = $"_Tab{entity.Name}.cshtml",
            RelativePath = $"Web/Views/{masterName}/_Tab{entity.Name}.cshtml",
            Content = sb.ToString(),
            FileType = "View"
        };
    }

    // =========================================================================
    // VALIDAÇÃO
    // =========================================================================

    private List<string> ValidateConfiguration(TabSheetConfigurationDto config)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(config.Id))
            errors.Add("ID da configuração é obrigatório");

        if (string.IsNullOrWhiteSpace(config.Title))
            errors.Add("Título é obrigatório");

        if (config.MasterTable == null)
            errors.Add("Configuração da tabela mestre é obrigatória");
        else
        {
            if (string.IsNullOrWhiteSpace(config.MasterTable.TableName))
                errors.Add("Nome da tabela mestre é obrigatório");
        }

        return errors;
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    private static string ToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;

        return string.Join("", str.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant()));
    }

    private static string FormatDisplayName(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase)) return pascalCase;

        var sb = new StringBuilder();
        for (int i = 0; i < pascalCase.Length; i++)
        {
            if (i > 0 && char.IsUpper(pascalCase[i]) && !char.IsUpper(pascalCase[i - 1]))
                sb.Append(' ');
            sb.Append(pascalCase[i]);
        }

        return sb.ToString()
            .Replace("Cd ", "Código ")
            .Replace("Dt ", "Data ")
            .Replace("Nr ", "Número ")
            .Replace("Nm ", "Nome ")
            .Replace("Vl ", "Valor ")
            .Replace("Qt ", "Quantidade ")
            .Replace("Id ", "")
            .Trim();
    }

    private static string GetDefaultInputType(ColunaInfo coluna)
    {
        if (coluna.IsData) return coluna.Tipo.ToLower() == "date" ? "date" : "datetime-local";
        if (coluna.IsBool) return "checkbox";
        if (coluna.IsNumerico) return "number";
        if (coluna.IsTexto && coluna.Tamanho > 255) return "textarea";
        if (coluna.ForeignKey != null) return "select";
        return "text";
    }
}

// =========================================================================
// DTOs COMPLETOS
// =========================================================================

public class TableMetadataDto
{
    public string TableName { get; set; } = "";
    public string Schema { get; set; } = "dbo";
    public string PrimaryKey { get; set; } = "id";
    public List<ColumnMetadataDto> Columns { get; set; } = new();
    public List<ForeignKeyDto> ForeignKeys { get; set; } = new();
    public List<RelatedTableDto> RelatedTables { get; set; } = new();
}

public class ColumnMetadataDto
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int? MaxLength { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsForeignKey { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsNullable { get; set; }
    public string? DefaultValue { get; set; }
    public string? ReferencedTable { get; set; }
    public string? ReferencedColumn { get; set; }
}

public class ForeignKeyDto
{
    public string Column { get; set; } = "";
    public string ReferencedTable { get; set; } = "";
    public string ReferencedColumn { get; set; } = "";
}

public class RelatedTableDto
{
    public string TableName { get; set; } = "";
    public string FkColumn { get; set; } = "";
    public int ColumnCount { get; set; }
}

public class TabSheetConfigurationDto
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Module { get; set; }
    public MasterTableConfigDto? MasterTable { get; set; }
    public List<TabConfigDto>? Tabs { get; set; }
    public GenerationOptionsDto? Options { get; set; }
}

public class MasterTableConfigDto
{
    public string TableName { get; set; } = "";
    public string? PrimaryKey { get; set; }
    public List<ColumnConfigDto>? Listagem { get; set; }
    public List<ColumnConfigDto>? Formulario { get; set; }
    public List<ForeignKeyDto>? ForeignKeys { get; set; }
}

public class TabConfigDto
{
    public string TableName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "fas fa-list";
    public int Order { get; set; }
    public string? FkColumn { get; set; }
    public bool AllowCreate { get; set; } = true;
    public bool AllowEdit { get; set; } = true;
    public bool AllowDelete { get; set; } = true;
    public List<ColumnConfigDto>? Listagem { get; set; }
    public List<ColumnConfigDto>? Formulario { get; set; }
}

public class ColumnConfigDto
{
    public string Column { get; set; } = "";
    public string Title { get; set; } = "";
    public string Label { get; set; } = "";
    public string Type { get; set; } = "text";
    public string Format { get; set; } = "text";
    public string? Width { get; set; }
    public string Align { get; set; } = "left";
    public bool Sortable { get; set; } = true;
    public int ColSize { get; set; } = 6;
    public bool Required { get; set; }
    public bool Disabled { get; set; }
}

/// <summary>
/// Opções de geração COMPLETAS - inclui DTOs e Services
/// </summary>
public class GenerationOptionsDto
{
    // Backend
    public bool GenerateMasterEntity { get; set; } = true;
    public bool GenerateDetailEntities { get; set; } = true;
    public bool GenerateDTOs { get; set; } = true;          // ← NOVO: Gera DTOs
    public bool GenerateServices { get; set; } = true;       // ← NOVO: Gera Services
    public bool GenerateNavigations { get; set; } = true;
    public bool GenerateController { get; set; } = true;

    // Frontend
    public bool GenerateMasterView { get; set; } = true;
    public bool GenerateTabPartials { get; set; } = true;
    public bool GenerateJavaScript { get; set; } = true;
    public bool UseDataTables { get; set; } = true;

    // Configurações
    public string? ModuleRoute { get; set; }
    public string? Icon { get; set; }
    public string? CdFuncao { get; set; }
    public string? CdSistema { get; set; }
}