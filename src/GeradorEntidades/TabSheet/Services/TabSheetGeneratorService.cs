// =============================================================================
// TABSHEET GENERATOR - SERVICE PRINCIPAL
// Versão: 1.0.1 (Corrigido - partial class)
// Autor: RhSensoERP Team
// Data: 2024
// 
// IMPORTANTE: Este serviço é ISOLADO do FullStackGeneratorService existente.
// Ele REUTILIZA o DatabaseService para ler metadados, mas gera código próprio.
// =============================================================================

using System.Diagnostics;
using System.Text.Json;
using GeradorEntidades.Models;
using GeradorEntidades.Services;
using GeradorEntidades.TabSheet.Models;
using GeradorEntidades.TabSheet.Templates;

namespace GeradorEntidades.TabSheet.Services;

/// <summary>
/// Serviço principal de geração de TabSheets (telas mestre/detalhe).
/// Orquestra a geração de todos os arquivos necessários.
/// </summary>
/// <remarks>
/// <para>
/// Este serviço é responsável por:
/// - Validar a configuração do TabSheet
/// - Ler metadados das tabelas do banco
/// - Gerar entidades com atributos de relacionamento
/// - Gerar Views com tabs (AdminLTE)
/// - Gerar JavaScript para interação
/// </para>
/// <para>
/// Exemplo de uso:
/// <code>
/// var result = await _tabSheetGenerator.GenerateAsync(configuration);
/// if (result.Success)
/// {
///     foreach (var file in result.GeneratedFiles)
///     {
///         // Salvar ou retornar arquivo
///     }
/// }
/// </code>
/// </para>
/// </remarks>
public partial class TabSheetGeneratorService
{
    private readonly ILogger<TabSheetGeneratorService> _logger;
    private readonly DatabaseService _databaseService;

    /// <summary>
    /// Inicializa o serviço de geração de TabSheets.
    /// </summary>
    /// <param name="logger">Logger para diagnóstico.</param>
    /// <param name="databaseService">Serviço de acesso ao banco de dados.</param>
    public TabSheetGeneratorService(
        ILogger<TabSheetGeneratorService> logger,
        DatabaseService databaseService)
    {
        _logger = logger;
        _databaseService = databaseService;
    }

    #region Geração Principal

    /// <summary>
    /// Gera todos os arquivos do TabSheet de forma assíncrona.
    /// </summary>
    /// <param name="config">Configuração do TabSheet.</param>
    /// <returns>Resultado da geração com todos os arquivos.</returns>
    public async Task<TabSheetGenerationResult> GenerateAsync(TabSheetConfiguration config)
    {
        var stopwatch = Stopwatch.StartNew();
        var generatedFiles = new List<GeneratedTabSheetFile>();
        var warnings = new List<string>();

        try
        {
            _logger.LogInformation("Iniciando geração TabSheet: {Id}", config.Id);

            // 1. Validar configuração
            if (!config.IsValid)
            {
                var errors = string.Join("; ", config.ValidationErrors);
                _logger.LogError("Configuração inválida: {Errors}", errors);
                return TabSheetGenerationResult.Fail($"Configuração inválida: {errors}");
            }

            // 2. Carregar metadados das tabelas
            var masterTable = await LoadTableMetadataAsync(config.MasterTable.TableName);
            if (masterTable == null)
            {
                return TabSheetGenerationResult.Fail($"Tabela mestre '{config.MasterTable.TableName}' não encontrada.");
            }

            var detailTables = new Dictionary<string, TabelaInfo>();
            foreach (var tab in config.Tabs)
            {
                var detailTable = await LoadTableMetadataAsync(tab.TableName);
                if (detailTable == null)
                {
                    warnings.Add($"Tabela '{tab.TableName}' da aba '{tab.Title}' não encontrada. Aba ignorada.");
                    continue;
                }
                detailTables[tab.TableName] = detailTable;
            }

            if (detailTables.Count == 0)
            {
                return TabSheetGenerationResult.Fail("Nenhuma tabela de detalhe encontrada.");
            }

            // 3. Atualizar configuração com metadados detectados
            EnrichConfiguration(config, masterTable, detailTables);

            // 4. Gerar arquivos
            var options = config.GenerationOptions;

            // 4.1 Entidade Mestre
            if (options.GenerateMasterEntity)
            {
                var masterEntity = TabSheetEntityTemplate.GenerateMasterEntity(config, masterTable);
                generatedFiles.Add(masterEntity);
                _logger.LogDebug("Gerada entidade mestre: {FileName}", masterEntity.FileName);
            }

            // 4.2 Entidades de Detalhe
            if (options.GenerateDetailEntities)
            {
                foreach (var tab in config.Tabs.Where(t => detailTables.ContainsKey(t.TableName)))
                {
                    var detailTable = detailTables[tab.TableName];
                    var detailEntity = TabSheetEntityTemplate.GenerateDetailEntity(config, tab, detailTable);
                    generatedFiles.Add(detailEntity);
                    _logger.LogDebug("Gerada entidade detalhe: {FileName}", detailEntity.FileName);
                }
            }

            // 4.3 View Principal com Tabs
            if (options.GenerateMasterView)
            {
                var masterView = TabSheetViewTemplate.GenerateMasterView(config, masterTable);
                generatedFiles.Add(masterView);
                _logger.LogDebug("Gerada view mestre: {FileName}", masterView.FileName);
            }

            // 4.4 Partial Views das Abas
            if (options.GenerateTabPartials)
            {
                foreach (var tab in config.Tabs.Where(t => detailTables.ContainsKey(t.TableName)))
                {
                    var detailTable = detailTables[tab.TableName];
                    var partialView = TabSheetViewTemplate.GenerateTabPartial(config, tab, detailTable);
                    generatedFiles.Add(partialView);
                    _logger.LogDebug("Gerada partial view: {FileName}", partialView.FileName);
                }
            }

            // 4.5 JavaScript
            if (options.GenerateJavaScript)
            {
                var jsFile = TabSheetJavaScriptTemplate.Generate(config, detailTables);
                generatedFiles.Add(jsFile);
                _logger.LogDebug("Gerado JavaScript: {FileName}", jsFile.FileName);
            }

            // 4.6 Configuração JSON (para referência futura)
            var configJson = GenerateConfigurationJson(config);
            generatedFiles.Add(configJson);

            stopwatch.Stop();

            _logger.LogInformation(
                "TabSheet gerado com sucesso: {FileCount} arquivos em {ElapsedMs}ms",
                generatedFiles.Count,
                stopwatch.ElapsedMilliseconds);

            return new TabSheetGenerationResult
            {
                Success = true,
                Configuration = config,
                GeneratedFiles = generatedFiles,
                Warnings = warnings,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar TabSheet: {Id}", config.Id);
            return TabSheetGenerationResult.Fail($"Erro interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Gera TabSheet de forma síncrona.
    /// </summary>
    public TabSheetGenerationResult Generate(TabSheetConfiguration config)
    {
        return GenerateAsync(config).GetAwaiter().GetResult();
    }

    #endregion

    #region Métodos de Suporte

    /// <summary>
    /// Carrega metadados de uma tabela do banco.
    /// </summary>
    private async Task<TabelaInfo?> LoadTableMetadataAsync(string tableName)
    {
        try
        {
            // Usa o DatabaseService existente - GetTabelasAsync já retorna tudo carregado
            var tabelas = await _databaseService.GetTabelasAsync();
            var tabela = tabelas.FirstOrDefault(t =>
                t.NomeTabela.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            // Colunas, FKs e índices já vêm carregados pelo GetTabelasAsync()
            return tabela;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar metadados da tabela: {TableName}", tableName);
            return null;
        }
    }

    /// <summary>
    /// Enriquece a configuração com metadados detectados do banco.
    /// </summary>
    private void EnrichConfiguration(
        TabSheetConfiguration config,
        TabelaInfo masterTable,
        Dictionary<string, TabelaInfo> detailTables)
    {
        // Detectar PK do mestre se não informada
        if (string.IsNullOrWhiteSpace(config.MasterTable.PrimaryKeyType))
        {
            var pk = masterTable.PrimaryKey;
            if (pk != null)
            {
                config.MasterTable.PrimaryKeyType = pk.TipoCSharp;
            }
        }

        // Detectar DisplayColumn do mestre se não informada
        if (string.IsNullOrWhiteSpace(config.MasterTable.DisplayColumn))
        {
            config.MasterTable.DisplayColumn = DetectDisplayColumn(masterTable);
        }

        // Detectar PKs das tabelas de detalhe
        foreach (var tab in config.Tabs)
        {
            if (detailTables.TryGetValue(tab.TableName, out var detailTable))
            {
                if (string.IsNullOrWhiteSpace(tab.PrimaryKey))
                {
                    var pk = detailTable.PrimaryKey;
                    if (pk != null)
                    {
                        tab.PrimaryKey = pk.Nome;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Detecta a coluna de display mais provável.
    /// </summary>
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

        // Fallback: primeira coluna string
        return tabela.Colunas.FirstOrDefault(c => c.IsTexto)?.Nome;
    }

    /// <summary>
    /// Gera arquivo JSON com a configuração usada.
    /// </summary>
    private GeneratedTabSheetFile GenerateConfigurationJson(TabSheetConfiguration config)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(config, options);

        return new GeneratedTabSheetFile
        {
            FileName = $"{config.Id}.tabsheet.json",
            RelativePath = "Config/",
            Content = json,
            FileType = TabSheetFileType.Configuration,
            IsMasterFile = true
        };
    }

    #endregion

    #region Métodos de Conveniência

    /// <summary>
    /// Cria uma configuração básica a partir de uma tabela mestre e suas FKs inversas.
    /// </summary>
    /// <param name="masterTableName">Nome da tabela mestre.</param>
    /// <returns>Configuração pré-preenchida.</returns>
    public async Task<TabSheetConfiguration?> CreateConfigurationFromTableAsync(string masterTableName)
    {
        try
        {
            var masterTable = await LoadTableMetadataAsync(masterTableName);
            if (masterTable == null) return null;

            var config = new TabSheetConfiguration
            {
                Id = $"{masterTable.NomePascalCase}_TabSheet",
                Title = $"Cadastro de {masterTable.NomePascalCase}",
                MasterTable = new MasterTableConfig
                {
                    TableName = masterTable.NomeTabela,
                    Schema = masterTable.Schema,
                    PrimaryKey = masterTable.PrimaryKey?.Nome ?? "",
                    PrimaryKeyType = masterTable.PrimaryKey?.TipoCSharp,
                    DisplayName = masterTable.NomePascalCase,
                    DisplayColumn = DetectDisplayColumn(masterTable)
                }
            };

            // Buscar tabelas que referenciam esta (FKs inversas)
            var possibleTabs = await GetPossibleDetailTablesAsync(masterTableName, config.MasterTable.PrimaryKey);
            config.Tabs = possibleTabs;

            _logger.LogInformation("Configuração base criada para: {Table} com {TabCount} abas possíveis",
                masterTableName, possibleTabs.Count);

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar configuração para: {Table}", masterTableName);
            return null;
        }
    }

    /// <summary>
    /// Obtém lista de tabelas que podem ser detalhes de uma tabela mestre.
    /// (Tabelas que possuem FK para a tabela mestre)
    /// </summary>
    /// <param name="masterTableName">Nome da tabela mestre.</param>
    /// <param name="masterPrimaryKey">Nome da PK do mestre.</param>
    /// <returns>Lista de tabelas candidatas a detalhe.</returns>
    public async Task<List<TabDefinition>> GetPossibleDetailTablesAsync(
        string masterTableName,
        string masterPrimaryKey)
    {
        var possibleTabs = new List<TabDefinition>();

        try
        {
            var allTables = await _databaseService.GetTabelasAsync();

            foreach (var table in allTables)
            {
                // FKs já vêm carregadas pelo GetTabelasAsync()
                // Verificar se tem FK apontando para o mestre
                var fkToMaster = table.ForeignKeys.FirstOrDefault(fk =>
                    fk.TabelaDestino.Equals(masterTableName, StringComparison.OrdinalIgnoreCase));

                if (fkToMaster != null)
                {
                    possibleTabs.Add(new TabDefinition
                    {
                        Title = table.NomePascalCase,
                        TableName = table.NomeTabela,
                        Schema = table.Schema,
                        ForeignKey = fkToMaster.ColunaOrigem,
                        Order = possibleTabs.Count + 1
                    });
                }
            }

            _logger.LogInformation(
                "Encontradas {Count} tabelas de detalhe para {Master}",
                possibleTabs.Count,
                masterTableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tabelas de detalhe para: {Master}", masterTableName);
        }

        return possibleTabs;
    }

    #endregion
}