// =============================================================================
// TABSHEET GENERATOR v2.0 - SERVICE (MÉTODOS ADICIONAIS)
// Versão: 1.0.2 (DEFINITIVO - Substitui todas as versões anteriores)
// 
// IMPORTANTE: 
// - Este arquivo é PARTIAL CLASS do TabSheetGeneratorService.cs
// - Usa o _databaseService já injetado (NÃO usa _connectionString)
// - Usa tipos do namespace GeradorEntidades.Models (ColunaInfo, ForeignKeyInfo)
// =============================================================================

using GeradorEntidades.Models;

namespace GeradorEntidades.TabSheet.Services;

/// <summary>
/// Extensões do TabSheetGeneratorService para API v2.0
/// </summary>
public partial class TabSheetGeneratorService
{
    /// <summary>
    /// Obtém as colunas de uma tabela específica
    /// </summary>
    public async Task<List<ColunaInfo>> GetTableColumnsAsync(string tableName)
    {
        var tabela = await _databaseService.GetTabelaAsync(tableName);
        return tabela?.Colunas ?? new List<ColunaInfo>();
    }

    /// <summary>
    /// Obtém as Foreign Keys de uma tabela
    /// </summary>
    public async Task<List<ForeignKeyInfo>> GetTableForeignKeysAsync(string tableName)
    {
        var tabela = await _databaseService.GetTabelaAsync(tableName);
        return tabela?.ForeignKeys ?? new List<ForeignKeyInfo>();
    }

    /// <summary>
    /// Obtém tabelas que têm FK apontando para a tabela mestre
    /// </summary>
    public async Task<List<RelatedTableInfo>> GetRelatedTablesAsync(string masterTableName)
    {
        var tabelas = new List<RelatedTableInfo>();

        try
        {
            var allTables = await _databaseService.GetTabelasAsync();

            foreach (var table in allTables)
            {
                var fkToMaster = table.ForeignKeys.FirstOrDefault(fk =>
                    fk.TabelaDestino.Equals(masterTableName, StringComparison.OrdinalIgnoreCase));

                if (fkToMaster != null)
                {
                    tabelas.Add(new RelatedTableInfo
                    {
                        Nome = table.NomeTabela,
                        ForeignKeyColumn = fkToMaster.ColunaOrigem,
                        ColumnCount = table.Colunas.Count,
                        Descricao = table.Descricao
                    });
                }
            }

            _logger.LogDebug(
                "Encontradas {Count} tabelas relacionadas para {Master}",
                tabelas.Count,
                masterTableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tabelas relacionadas para: {Master}", masterTableName);
        }

        return tabelas;
    }

    /// <summary>
    /// Obtém metadados completos de uma tabela para a API v2
    /// </summary>
    public async Task<TableMetadataResult?> GetTableMetadataAsync(string tableName)
    {
        try
        {
            var tabela = await _databaseService.GetTabelaAsync(tableName);
            if (tabela == null) return null;

            var relatedTables = await GetRelatedTablesAsync(tableName);

            return new TableMetadataResult
            {
                TableName = tabela.NomeTabela,
                Schema = tabela.Schema,
                PrimaryKey = tabela.PrimaryKey?.Nome ?? "",
                PrimaryKeyType = tabela.PrimaryKey?.TipoCSharp ?? "int",
                Columns = tabela.Colunas,
                ForeignKeys = tabela.ForeignKeys,
                RelatedTables = relatedTables,
                Description = tabela.Descricao
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter metadados da tabela: {Table}", tableName);
            return null;
        }
    }

    /// <summary>
    /// Verifica se uma tabela existe no banco
    /// </summary>
    public async Task<bool> TableExistsAsync(string tableName)
    {
        var tabela = await _databaseService.GetTabelaAsync(tableName);
        return tabela != null;
    }
}

#region Classes de Suporte para API v2

/// <summary>
/// Informações de uma tabela relacionada (que tem FK para o mestre)
/// </summary>
public class RelatedTableInfo
{
    public string Nome { get; set; } = string.Empty;
    public string ForeignKeyColumn { get; set; } = string.Empty;
    public int ColumnCount { get; set; }
    public string? Descricao { get; set; }
}

/// <summary>
/// Resultado completo de metadados de uma tabela
/// </summary>
public class TableMetadataResult
{
    public string TableName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string PrimaryKey { get; set; } = string.Empty;
    public string PrimaryKeyType { get; set; } = "int";
    public List<ColunaInfo> Columns { get; set; } = new();
    public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
    public List<RelatedTableInfo> RelatedTables { get; set; } = new();
    public string? Description { get; set; }
}

#endregion