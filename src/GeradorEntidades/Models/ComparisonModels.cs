namespace GeradorEntidades.Models;

public class EntityComparisonResult
{
    public string EntityName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public bool TableExists { get; set; }
    public List<PropertyMismatch> Mismatches { get; set; } = new();
    public List<string> MissingColumns { get; set; } = new();
    public List<string> ExtraColumns { get; set; } = new();
    public bool IsMatch => TableExists && Mismatches.Count == 0 && MissingColumns.Count == 0;
}

public class PropertyMismatch
{
    public string PropertyName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string ApiType { get; set; } = string.Empty;
    public string DbType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class GenerationSettings
{
    public string OutputPath { get; set; } = string.Empty;
}
