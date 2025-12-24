// =============================================================================
// GERADOR DE ENTIDADES - DATABASE SERVICE v2.0
// Com detecção de colunas de display para relacionamentos
// =============================================================================

using GeradorEntidades.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;

namespace GeradorEntidades.Services;

/// <summary>
/// Serviço para leitura de metadados do banco SQL Server.
/// Inclui cache e detecção de colunas de display.
/// </summary>
public class DatabaseService
{
    private readonly IConfiguration _config;
    private readonly ILogger<DatabaseService> _logger;
    
    private static readonly ConcurrentDictionary<string, List<TabelaInfo>> _cache = new();
    private static readonly ConcurrentDictionary<string, TabelaDisplayInfo> _displayCache = new();
    private static DateTime _cacheExpiration = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public DatabaseService(IConfiguration config, ILogger<DatabaseService> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todas as tabelas com suas colunas, FKs e índices.
    /// </summary>
    public async Task<List<TabelaInfo>> GetTabelasAsync(bool forceRefresh = false)
    {
        var connectionString = GetConnectionString();
        var cacheKey = connectionString.GetHashCode().ToString();

        if (!forceRefresh && DateTime.UtcNow < _cacheExpiration && _cache.TryGetValue(cacheKey, out var cached))
        {
            _logger.LogDebug("Retornando {Count} tabelas do cache", cached.Count);
            return cached;
        }

        _logger.LogInformation("Carregando metadados do banco...");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var tabelas = new Dictionary<string, TabelaInfo>(StringComparer.OrdinalIgnoreCase);

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Carrega colunas
        await LoadColunasAsync(conn, tabelas);
        
        // 2. Carrega FKs
        await LoadForeignKeysAsync(conn, tabelas);
        
        // 3. Carrega índices
        await LoadIndicesAsync(conn, tabelas);
        
        // 4. Carrega descrições (Extended Properties)
        await LoadDescricoesAsync(conn, tabelas);
        
        // 5. NOVO: Detecta colunas de display para FKs
        await LoadDisplayColumnsAsync(conn, tabelas);

        var result = tabelas.Values
            .OrderBy(t => t.NomeTabela)
            .ToList();

        _cache[cacheKey] = result;
        _cacheExpiration = DateTime.UtcNow.Add(CacheDuration);

        sw.Stop();
        _logger.LogInformation("Carregadas {Count} tabelas em {Elapsed}ms", 
            result.Count, sw.ElapsedMilliseconds);

        return result;
    }

    /// <summary>
    /// Obtém uma tabela específica.
    /// </summary>
    public async Task<TabelaInfo?> GetTabelaAsync(string nomeTabela)
    {
        var tabelas = await GetTabelasAsync();
        return tabelas.FirstOrDefault(t => 
            t.NomeTabela.Equals(nomeTabela, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Pesquisa tabelas por nome.
    /// </summary>
    public async Task<List<TabelaInfo>> SearchTabelasAsync(string termo)
    {
        var tabelas = await GetTabelasAsync();
        
        if (string.IsNullOrWhiteSpace(termo))
            return tabelas;

        var termoLower = termo.ToLower();
        return tabelas
            .Where(t => t.NomeTabela.ToLower().Contains(termoLower) ||
                        t.Descricao.ToLower().Contains(termoLower))
            .ToList();
    }

    /// <summary>
    /// Invalida o cache.
    /// </summary>
    public void InvalidateCache()
    {
        _cache.Clear();
        _displayCache.Clear();
        _cacheExpiration = DateTime.MinValue;
        _logger.LogInformation("Cache de metadados invalidado");
    }

    #region Private Methods

    private string GetConnectionString()
    {
        return _config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }

    private async Task LoadColunasAsync(SqlConnection conn, Dictionary<string, TabelaInfo> tabelas)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT
                t.TABLE_SCHEMA,
                t.TABLE_NAME,
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.NUMERIC_PRECISION,
                c.NUMERIC_SCALE,
                c.IS_NULLABLE,
                c.COLUMN_DEFAULT,
                c.ORDINAL_POSITION,
                COLUMNPROPERTY(OBJECT_ID(t.TABLE_SCHEMA + '.' + t.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') AS IsIdentity,
                COLUMNPROPERTY(OBJECT_ID(t.TABLE_SCHEMA + '.' + t.TABLE_NAME), c.COLUMN_NAME, 'IsComputed') AS IsComputed,
                CASE 
                    WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 
                    ELSE 0 
                END AS IsPrimaryKey
            FROM INFORMATION_SCHEMA.TABLES t
            INNER JOIN INFORMATION_SCHEMA.COLUMNS c
                ON t.TABLE_NAME = c.TABLE_NAME
               AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
            LEFT JOIN (
                SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                   AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ) pk
                ON pk.TABLE_SCHEMA = c.TABLE_SCHEMA
               AND pk.TABLE_NAME = c.TABLE_NAME
               AND pk.COLUMN_NAME = c.COLUMN_NAME
            WHERE t.TABLE_TYPE = 'BASE TABLE'
            ORDER BY t.TABLE_SCHEMA, t.TABLE_NAME, c.ORDINAL_POSITION";

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var schema = reader.GetString(0);
            var tableName = reader.GetString(1);
            var key = $"{schema}.{tableName}";

            if (!tabelas.TryGetValue(key, out var tabela))
            {
                tabela = new TabelaInfo
                {
                    Schema = schema,
                    NomeTabela = tableName
                };
                tabelas[key] = tabela;
            }

            tabela.Colunas.Add(new ColunaInfo
            {
                Nome = reader.GetString(2),
                Tipo = reader.GetString(3),
                Tamanho = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Precisao = reader.IsDBNull(5) ? null : (int?)reader.GetByte(5),
                Escala = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                IsNullable = reader.GetString(7).Equals("YES", StringComparison.OrdinalIgnoreCase),
                DefaultValue = reader.IsDBNull(8) ? null : reader.GetString(8),
                OrdinalPosition = reader.GetInt32(9),
                IsIdentity = reader.GetInt32(10) == 1,
                IsComputed = reader.GetInt32(11) == 1,
                IsPrimaryKey = reader.GetInt32(12) == 1
            });
        }
    }

    private async Task LoadForeignKeysAsync(SqlConnection conn, Dictionary<string, TabelaInfo> tabelas)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                fk.name AS FK_Name,
                OBJECT_SCHEMA_NAME(fk.parent_object_id) AS Schema_Name,
                OBJECT_NAME(fk.parent_object_id) AS Table_Name,
                COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS Column_Name,
                OBJECT_NAME(fk.referenced_object_id) AS Referenced_Table,
                COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS Referenced_Column
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc 
                ON fk.object_id = fkc.constraint_object_id
            ORDER BY Schema_Name, Table_Name, FK_Name, fkc.constraint_column_id";

        // Primeiro, coletamos todas as colunas de cada FK para identificar compostas
        var fksPorNome = new Dictionary<string, List<(string Schema, string Table, string Column, string RefTable, string RefColumn)>>();

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var fkName = reader.GetString(0);
            var schema = reader.GetString(1);
            var tableName = reader.GetString(2);
            var columnName = reader.GetString(3);
            var refTable = reader.GetString(4);
            var refColumn = reader.GetString(5);

            var key = $"{schema}.{tableName}.{fkName}";
            if (!fksPorNome.ContainsKey(key))
                fksPorNome[key] = [];
            
            fksPorNome[key].Add((schema, tableName, columnName, refTable, refColumn));
        }

        // Agora processamos as FKs, marcando as compostas
        foreach (var (fkKey, colunas) in fksPorNome)
        {
            var isComposta = colunas.Count > 1;
            var todasColunas = colunas.Select(c => c.Column).ToList();
            
            foreach (var (schema, tableName, columnName, refTable, refColumn) in colunas)
            {
                var key = $"{schema}.{tableName}";

                if (tabelas.TryGetValue(key, out var tabela))
                {
                    var fkName = fkKey.Split('.').Last();
                    
                    var fk = new ForeignKeyInfo
                    {
                        Nome = fkName,
                        ColunaOrigem = columnName,
                        TabelaDestino = refTable,
                        ColunaDestino = refColumn,
                        IsParteDeFkComposta = isComposta,
                        TodasColunas = todasColunas
                    };

                    tabela.ForeignKeys.Add(fk);

                    var coluna = tabela.Colunas.FirstOrDefault(c => 
                        c.Nome.Equals(fk.ColunaOrigem, StringComparison.OrdinalIgnoreCase));
                    if (coluna != null)
                    {
                        coluna.ForeignKey = fk;
                    }
                }
            }
        }
    }

    private async Task LoadIndicesAsync(SqlConnection conn, Dictionary<string, TabelaInfo> tabelas)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                OBJECT_SCHEMA_NAME(i.object_id) AS Schema_Name,
                OBJECT_NAME(i.object_id) AS Table_Name,
                i.name AS Index_Name,
                i.is_unique,
                COL_NAME(ic.object_id, ic.column_id) AS Column_Name
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic 
                ON i.object_id = ic.object_id 
               AND i.index_id = ic.index_id
            WHERE i.is_primary_key = 0 
              AND i.type > 0
              AND OBJECT_NAME(i.object_id) NOT LIKE 'sys%'
            ORDER BY Schema_Name, Table_Name, Index_Name, ic.key_ordinal";

        await using var reader = await cmd.ExecuteReaderAsync();

        var indices = new Dictionary<string, IndexInfo>();

        while (await reader.ReadAsync())
        {
            var schema = reader.GetString(0);
            var tableName = reader.GetString(1);
            var indexName = reader.GetString(2);
            var isUnique = reader.GetBoolean(3);
            var columnName = reader.GetString(4);

            var tableKey = $"{schema}.{tableName}";
            var indexKey = $"{tableKey}.{indexName}";

            if (!indices.TryGetValue(indexKey, out var index))
            {
                index = new IndexInfo
                {
                    Nome = indexName,
                    IsUnique = isUnique
                };
                indices[indexKey] = index;

                if (tabelas.TryGetValue(tableKey, out var tabela))
                {
                    tabela.Indices.Add(index);
                }
            }

            index.Colunas.Add(columnName);
        }
    }

    private async Task LoadDescricoesAsync(SqlConnection conn, Dictionary<string, TabelaInfo> tabelas)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                SCHEMA_NAME(t.schema_id) AS Schema_Name,
                t.name AS Table_Name,
                NULL AS Column_Name,
                CAST(ep.value AS NVARCHAR(MAX)) AS Description
            FROM sys.tables t
            INNER JOIN sys.extended_properties ep 
                ON ep.major_id = t.object_id 
               AND ep.minor_id = 0
               AND ep.name = 'MS_Description'
            
            UNION ALL
            
            SELECT 
                SCHEMA_NAME(t.schema_id) AS Schema_Name,
                t.name AS Table_Name,
                c.name AS Column_Name,
                CAST(ep.value AS NVARCHAR(MAX)) AS Description
            FROM sys.tables t
            INNER JOIN sys.columns c ON c.object_id = t.object_id
            INNER JOIN sys.extended_properties ep 
                ON ep.major_id = t.object_id 
               AND ep.minor_id = c.column_id
               AND ep.name = 'MS_Description'";

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var schema = reader.GetString(0);
            var tableName = reader.GetString(1);
            var columnName = reader.IsDBNull(2) ? null : reader.GetString(2);
            var description = reader.IsDBNull(3) ? null : reader.GetString(3);

            var key = $"{schema}.{tableName}";

            if (tabelas.TryGetValue(key, out var tabela) && !string.IsNullOrEmpty(description))
            {
                if (columnName == null)
                {
                    tabela.Descricao = description;
                }
                else
                {
                    var coluna = tabela.Colunas.FirstOrDefault(c => 
                        c.Nome.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    if (coluna != null)
                    {
                        coluna.Descricao = description;
                    }
                }
            }
        }
    }

    /// <summary>
    /// NOVO: Detecta colunas de display para tabelas referenciadas por FKs.
    /// Procura por padrões: dc*, nm*, descricao, nome
    /// </summary>
    private async Task LoadDisplayColumnsAsync(SqlConnection conn, Dictionary<string, TabelaInfo> tabelas)
    {
        // Coleta todas as tabelas referenciadas por FKs
        var tabelasReferenciadas = tabelas.Values
            .SelectMany(t => t.ForeignKeys)
            .Select(fk => fk.TabelaDestino.ToLower())
            .Distinct()
            .ToList();

        if (tabelasReferenciadas.Count == 0)
            return;

        // Query para encontrar colunas de display em tabelas referenciadas
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                t.name AS TableName,
                c.name AS ColumnName,
                c.column_id AS OrdinalPosition,
                CASE 
                    WHEN c.name LIKE 'dc%' THEN 1
                    WHEN c.name LIKE 'nm%' THEN 2
                    WHEN c.name LIKE 'descricao%' THEN 3
                    WHEN c.name LIKE 'nome%' THEN 4
                    WHEN c.name LIKE 'titulo%' THEN 5
                    ELSE 99
                END AS Priority
            FROM sys.tables t
            INNER JOIN sys.columns c ON c.object_id = t.object_id
            WHERE t.name IN ('" + string.Join("','", tabelasReferenciadas) + @"')
              AND (
                  c.name LIKE 'dc%' 
                  OR c.name LIKE 'nm%'
                  OR c.name LIKE 'descricao%'
                  OR c.name LIKE 'nome%'
                  OR c.name LIKE 'titulo%'
              )
            ORDER BY t.name, Priority, c.column_id";

        var displayColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(0).ToLower();
            var columnName = reader.GetString(1);

            // Guarda apenas a primeira coluna encontrada (maior prioridade)
            if (!displayColumns.ContainsKey(tableName))
            {
                displayColumns[tableName] = columnName;
            }
        }

        // Atualiza as FKs com as colunas de display encontradas
        foreach (var tabela in tabelas.Values)
        {
            foreach (var fk in tabela.ForeignKeys)
            {
                var tabelaDestino = fk.TabelaDestino.ToLower();
                if (displayColumns.TryGetValue(tabelaDestino, out var displayColumn))
                {
                    fk.ColunaDisplay = displayColumn;
                }
            }
        }

        _logger.LogDebug("Detectadas colunas de display para {Count} tabelas", displayColumns.Count);
    }

    #endregion

    #region Funções do Sistema

    /// <summary>
    /// Obtém lista de funções do sistema (tabela fucn1).
    /// </summary>
    public async Task<List<FuncaoInfo>> GetFuncoesAsync()
    {
        var funcoes = new List<FuncaoInfo>();
        var connectionString = GetConnectionString();

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT DISTINCT
                RTRIM(LTRIM(cdfuncao)) AS CdFuncao,
                RTRIM(LTRIM(ISNULL(dcfuncao, ''))) AS DcFuncao,
                RTRIM(LTRIM(ISNULL(cdsistema, 'RHU'))) AS CdSistema
            FROM fucn1
            WHERE cdfuncao IS NOT NULL 
              AND cdfuncao <> ''
            ORDER BY CdFuncao";

        try
        {
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                funcoes.Add(new FuncaoInfo
                {
                    CdFuncao = reader.GetString(0),
                    DcFuncao = reader.GetString(1),
                    CdSistema = reader.GetString(2)
                });
            }

            _logger.LogInformation("Carregadas {Count} funções do sistema", funcoes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar funções. Verifique se a tabela fucn1 existe.");
            throw;
        }

        return funcoes;
    }

    #endregion
}
