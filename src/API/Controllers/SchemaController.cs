// =============================================================================
// RHSENSOERP - SCHEMA CONTROLLER
// =============================================================================
// Arquivo: src/API/Controllers/SchemaController.cs
// Descrição: Retorna schema de tabelas do banco para validação no Gerador
// Ambiente: APENAS DEVELOPMENT (bloqueado em produção)
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para consulta de schema do banco de dados.
/// Usado pelo Gerador de Razor Pages para validar entidades.
/// ⚠️ APENAS DISPONÍVEL EM AMBIENTE DE DESENVOLVIMENTO
/// </summary>
[ApiController]
[Route("api/[controller]")]
[EnableCors("ManifestDev")]
[ApiExplorerSettings(GroupName = "Sistema")]
[Tags("Schema")]
public class SchemaController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<SchemaController> _logger;

    // Prefixos de tabelas bloqueadas (sistema)
    private static readonly string[] BlockedTablePrefixes = new[]
    {
        "sys.",
        "INFORMATION_SCHEMA.",
        "__EF",
        "__Migration",
        "sysdiagrams",
        "dtproperties"
    };

    public SchemaController(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<SchemaController> logger)
    {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Retorna o schema de uma tabela específica.
    /// </summary>
    /// <param name="tableName">Nome da tabela (ex: TB_FUNCIONARIOS)</param>
    /// <param name="database">Nome do banco (opcional, usa DefaultConnection se não informado)</param>
    /// <returns>Schema da tabela com colunas, tipos, PK, FK, etc.</returns>
    /// <response code="200">Schema retornado com sucesso</response>
    /// <response code="400">Nome da tabela inválido ou bloqueado</response>
    /// <response code="404">Tabela não encontrada ou endpoint indisponível</response>
    /// <response code="500">Erro interno ao consultar banco</response>
    [HttpGet("table/{tableName}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TableSchemaResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTableSchema(
        [FromRoute] string tableName,
        [FromQuery] string? database = null)
    {
        // ═══════════════════════════════════════════════════════════════════
        // BLOQUEIO EM PRODUÇÃO - CRÍTICO!
        // ═══════════════════════════════════════════════════════════════════
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning(
                "⛔ Tentativa de acesso ao SchemaController em ambiente {Environment}. IP: {IP}",
                _environment.EnvironmentName,
                HttpContext.Connection.RemoteIpAddress);

            return NotFound(new { error = "ENDPOINT_NOT_AVAILABLE" });
        }

        // ═══════════════════════════════════════════════════════════════════
        // VALIDAÇÃO DO NOME DA TABELA
        // ═══════════════════════════════════════════════════════════════════
        if (string.IsNullOrWhiteSpace(tableName))
        {
            return BadRequest(new { error = "TABLE_NAME_REQUIRED", message = "Nome da tabela é obrigatório." });
        }

        // Sanitiza: remove caracteres perigosos
        tableName = SanitizeTableName(tableName);

        // Verifica se é tabela de sistema
        if (IsBlockedTable(tableName))
        {
            _logger.LogWarning("🚫 Tentativa de acesso a tabela bloqueada: {TableName}", tableName);
            return BadRequest(new
            {
                error = "BLOCKED_TABLE",
                message = "Acesso a tabelas de sistema não é permitido."
            });
        }

        // ═══════════════════════════════════════════════════════════════════
        // CONEXÃO COM O BANCO
        // ═══════════════════════════════════════════════════════════════════
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("❌ DefaultConnection não configurada");
            return StatusCode(500, new { error = "CONNECTION_NOT_CONFIGURED" });
        }

        // Se informou banco específico, altera na connection string
        if (!string.IsNullOrWhiteSpace(database))
        {
            database = SanitizeDatabaseName(database);
            connectionString = ReplaceDatabase(connectionString, database);
        }

        try
        {
            var result = await GetSchemaFromDatabaseAsync(connectionString, tableName);

            if (result == null)
            {
                return NotFound(new
                {
                    error = "TABLE_NOT_FOUND",
                    message = $"Tabela '{tableName}' não encontrada no banco."
                });
            }

            _logger.LogInformation(
                "✅ Schema retornado: {TableName} ({ColumnCount} colunas)",
                tableName,
                result.ColumnCount);

            return Ok(result);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "❌ Erro SQL ao consultar schema de {TableName}", tableName);
            return StatusCode(500, new
            {
                error = "DATABASE_ERROR",
                message = "Erro ao consultar banco de dados.",
                details = _environment.IsDevelopment() ? ex.Message : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro inesperado ao consultar schema de {TableName}", tableName);
            return StatusCode(500, new
            {
                error = "INTERNAL_ERROR",
                message = "Erro interno ao processar requisição."
            });
        }
    }

    /// <summary>
    /// Lista todas as tabelas disponíveis no banco.
    /// </summary>
    /// <param name="database">Nome do banco (opcional)</param>
    /// <param name="filter">Filtro por nome (ex: TB_)</param>
    /// <returns>Lista de tabelas</returns>
    [HttpGet("tables")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TablesListResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTables(
        [FromQuery] string? database = null,
        [FromQuery] string? filter = null)
    {
        // Bloqueio em produção
        if (!_environment.IsDevelopment())
        {
            return NotFound(new { error = "ENDPOINT_NOT_AVAILABLE" });
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            return StatusCode(500, new { error = "CONNECTION_NOT_CONFIGURED" });
        }

        if (!string.IsNullOrWhiteSpace(database))
        {
            database = SanitizeDatabaseName(database);
            connectionString = ReplaceDatabase(connectionString, database);
        }

        try
        {
            var tables = await GetTablesListAsync(connectionString, filter);

            return Ok(new TablesListResult
            {
                Database = database ?? GetDatabaseName(connectionString),
                Count = tables.Count,
                Tables = tables
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao listar tabelas");
            return StatusCode(500, new { error = "DATABASE_ERROR" });
        }
    }

    #region Private Methods

    /// <summary>
    /// Consulta o schema da tabela no banco de dados.
    /// </summary>
    private async Task<TableSchemaResult?> GetSchemaFromDatabaseAsync(
        string connectionString,
        string tableName)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Primeiro verifica se a tabela existe
        var existsQuery = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = @TableName 
              AND TABLE_TYPE = 'BASE TABLE'";

        await using var existsCmd = new SqlCommand(existsQuery, connection);
        existsCmd.Parameters.AddWithValue("@TableName", tableName);

        var exists = (int)await existsCmd.ExecuteScalarAsync() > 0;
        if (!exists) return null;

        // Query principal: colunas + PK + Identity
        var columnsQuery = @"
            SELECT 
                c.COLUMN_NAME as ColumnName,
                c.DATA_TYPE as SqlType,
                c.CHARACTER_MAXIMUM_LENGTH as MaxLength,
                c.NUMERIC_PRECISION as NumericPrecision,
                c.NUMERIC_SCALE as NumericScale,
                c.IS_NULLABLE as IsNullable,
                c.COLUMN_DEFAULT as DefaultValue,
                c.TABLE_SCHEMA as TableSchema,
                c.TABLE_CATALOG as DatabaseName,
                CASE 
                    WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 
                    ELSE 0 
                END as IsPrimaryKey,
                COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') as IsIdentity
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    AND tc.TABLE_NAME = ku.TABLE_NAME
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
            WHERE c.TABLE_NAME = @TableName
            ORDER BY c.ORDINAL_POSITION";

        // Query para Foreign Keys
        var fkQuery = @"
            SELECT 
                ccu.COLUMN_NAME as ColumnName,
                pk_table.TABLE_NAME as ReferencedTable,
                pk_ccu.COLUMN_NAME as ReferencedColumn
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
            INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
                ON rc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk_table
                ON rc.UNIQUE_CONSTRAINT_NAME = pk_table.CONSTRAINT_NAME
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE pk_ccu
                ON pk_table.CONSTRAINT_NAME = pk_ccu.CONSTRAINT_NAME
            WHERE ccu.TABLE_NAME = @TableName";

        // Busca FKs
        var foreignKeys = new Dictionary<string, (string Table, string Column)>();
        await using (var fkCmd = new SqlCommand(fkQuery, connection))
        {
            fkCmd.Parameters.AddWithValue("@TableName", tableName);
            await using var fkReader = await fkCmd.ExecuteReaderAsync();
            while (await fkReader.ReadAsync())
            {
                var colName = fkReader.GetString(0);
                var refTable = fkReader.GetString(1);
                var refColumn = fkReader.GetString(2);
                foreignKeys[colName] = (refTable, refColumn);
            }
        }

        // Busca colunas
        var columns = new List<ColumnSchemaItem>();
        string? schema = null;
        string? databaseName = null;

        await using (var cmd = new SqlCommand(columnsQuery, connection))
        {
            cmd.Parameters.AddWithValue("@TableName", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                schema ??= reader.GetString(reader.GetOrdinal("TableSchema"));
                databaseName ??= reader.GetString(reader.GetOrdinal("DatabaseName"));

                var columnName = reader.GetString(reader.GetOrdinal("ColumnName"));
                var sqlType = reader.GetString(reader.GetOrdinal("SqlType"));
                var maxLength = reader.IsDBNull(reader.GetOrdinal("MaxLength"))
                    ? (int?)null
                    : reader.GetInt32(reader.GetOrdinal("MaxLength"));
                var isNullable = reader.GetString(reader.GetOrdinal("IsNullable")) == "YES";
                var isPrimaryKey = reader.GetInt32(reader.GetOrdinal("IsPrimaryKey")) == 1;
                var isIdentity = reader.IsDBNull(reader.GetOrdinal("IsIdentity"))
                    ? false
                    : reader.GetInt32(reader.GetOrdinal("IsIdentity")) == 1;

                var hasFk = foreignKeys.TryGetValue(columnName, out var fkInfo);

                columns.Add(new ColumnSchemaItem
                {
                    Name = columnName,
                    Type = MapSqlTypeToCSharp(sqlType, isNullable),
                    SqlType = sqlType,
                    MaxLength = maxLength == -1 ? null : maxLength, // -1 = MAX
                    IsNullable = isNullable,
                    IsPrimaryKey = isPrimaryKey,
                    IsIdentity = isIdentity,
                    IsForeignKey = hasFk,
                    ForeignKeyTable = hasFk ? fkInfo.Table : null,
                    ForeignKeyColumn = hasFk ? fkInfo.Column : null
                });
            }
        }

        return new TableSchemaResult
        {
            TableName = tableName,
            Schema = schema ?? "dbo",
            Database = databaseName ?? "Unknown",
            ColumnCount = columns.Count,
            Columns = columns
        };
    }

    /// <summary>
    /// Lista tabelas do banco.
    /// </summary>
    private async Task<List<TableInfo>> GetTablesListAsync(string connectionString, string? filter)
    {
        var tables = new List<TableInfo>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT 
                t.TABLE_NAME,
                t.TABLE_SCHEMA,
                (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS c WHERE c.TABLE_NAME = t.TABLE_NAME) as ColumnCount
            FROM INFORMATION_SCHEMA.TABLES t
            WHERE t.TABLE_TYPE = 'BASE TABLE'
              AND t.TABLE_NAME NOT LIKE 'sys%'
              AND t.TABLE_NAME NOT LIKE '__EF%'
              AND t.TABLE_NAME NOT LIKE 'sysdiagrams'
              AND (@Filter IS NULL OR t.TABLE_NAME LIKE '%' + @Filter + '%')
            ORDER BY t.TABLE_SCHEMA, t.TABLE_NAME";

        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Filter", (object?)filter ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(new TableInfo
            {
                TableName = reader.GetString(0),
                Schema = reader.GetString(1),
                ColumnCount = reader.GetInt32(2)
            });
        }

        return tables;
    }

    /// <summary>
    /// Mapeia tipo SQL para tipo C#.
    /// </summary>
    private static string MapSqlTypeToCSharp(string sqlType, bool isNullable)
    {
        var csharpType = sqlType.ToLowerInvariant() switch
        {
            "int" => "int",
            "bigint" => "long",
            "smallint" => "short",
            "tinyint" => "byte",
            "bit" => "bool",
            "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
            "float" => "double",
            "real" => "float",
            "datetime" or "datetime2" or "smalldatetime" => "DateTime",
            "date" => "DateOnly",
            "time" => "TimeOnly",
            "datetimeoffset" => "DateTimeOffset",
            "uniqueidentifier" => "Guid",
            "varbinary" or "binary" or "image" => "byte[]",
            "char" or "nchar" or "varchar" or "nvarchar" or "text" or "ntext" => "string",
            "xml" => "string",
            _ => "object"
        };

        // Adiciona ? para nullable (exceto string e byte[] que já são nullable)
        if (isNullable && csharpType != "string" && csharpType != "byte[]" && csharpType != "object")
        {
            csharpType += "?";
        }

        return csharpType;
    }

    /// <summary>
    /// Sanitiza nome da tabela (remove caracteres perigosos).
    /// </summary>
    private static string SanitizeTableName(string tableName)
    {
        // Remove tudo exceto letras, números, underscore e ponto
        return new string(tableName
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '.')
            .ToArray());
    }

    /// <summary>
    /// Sanitiza nome do banco.
    /// </summary>
    private static string SanitizeDatabaseName(string database)
    {
        return new string(database
            .Where(c => char.IsLetterOrDigit(c) || c == '_')
            .ToArray());
    }

    /// <summary>
    /// Verifica se é tabela bloqueada.
    /// </summary>
    private static bool IsBlockedTable(string tableName)
    {
        return BlockedTablePrefixes.Any(prefix =>
            tableName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Substitui o banco na connection string.
    /// </summary>
    private static string ReplaceDatabase(string connectionString, string newDatabase)
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = newDatabase
        };
        return builder.ConnectionString;
    }

    /// <summary>
    /// Extrai nome do banco da connection string.
    /// </summary>
    private static string GetDatabaseName(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        return builder.InitialCatalog;
    }

    #endregion
}

#region DTOs

/// <summary>
/// Resultado do schema de uma tabela.
/// </summary>
public class TableSchemaResult
{
    /// <summary>Nome da tabela.</summary>
    public string TableName { get; set; } = "";

    /// <summary>Schema (default: dbo).</summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>Nome do banco de dados.</summary>
    public string Database { get; set; } = "";

    /// <summary>Quantidade de colunas.</summary>
    public int ColumnCount { get; set; }

    /// <summary>Lista de colunas com seus metadados.</summary>
    public List<ColumnSchemaItem> Columns { get; set; } = new();
}

/// <summary>
/// Metadados de uma coluna.
/// </summary>
public class ColumnSchemaItem
{
    /// <summary>Nome da coluna.</summary>
    public string Name { get; set; } = "";

    /// <summary>Tipo C# equivalente (int, string, DateTime, etc).</summary>
    public string Type { get; set; } = "";

    /// <summary>Tipo SQL original (nvarchar, int, datetime2, etc).</summary>
    public string SqlType { get; set; } = "";

    /// <summary>Permite nulo?</summary>
    public bool IsNullable { get; set; }

    /// <summary>É chave primária?</summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>É identity (auto-increment)?</summary>
    public bool IsIdentity { get; set; }

    /// <summary>É chave estrangeira?</summary>
    public bool IsForeignKey { get; set; }

    /// <summary>Tabela referenciada (se FK).</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ForeignKeyTable { get; set; }

    /// <summary>Coluna referenciada (se FK).</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ForeignKeyColumn { get; set; }

    /// <summary>Tamanho máximo (para strings).</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxLength { get; set; }
}

/// <summary>
/// Resultado da listagem de tabelas.
/// </summary>
public class TablesListResult
{
    /// <summary>Nome do banco de dados.</summary>
    public string Database { get; set; } = "";

    /// <summary>Quantidade de tabelas.</summary>
    public int Count { get; set; }

    /// <summary>Lista de tabelas.</summary>
    public List<TableInfo> Tables { get; set; } = new();
}

/// <summary>
/// Informações básicas de uma tabela.
/// </summary>
public class TableInfo
{
    /// <summary>Nome da tabela.</summary>
    public string TableName { get; set; } = "";

    /// <summary>Schema da tabela.</summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>Quantidade de colunas.</summary>
    public int ColumnCount { get; set; }
}

#endregion