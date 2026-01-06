namespace GeradorEntidades.Models;

public class ManifestData
{
    public string GeneratedAt { get; set; } = "";
    public string Version { get; set; } = "";
    public List<EntityManifestItem> Entities { get; set; } = new();
}

public class EntityManifestItem
{
    public string EntityName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string TableName { get; set; } = "";
    public string Schema { get; set; } = "";
    public string ModuleName { get; set; } = "";
    public string ModuleDisplayName { get; set; } = "";
    public string CdSistema { get; set; } = "";
    public string CdFuncao { get; set; } = "";
    public string Route { get; set; } = "";
    public bool UsePluralRoute { get; set; }
    public string Icon { get; set; } = "";
    public bool RequiresAuth { get; set; }
    public bool SupportsBatchDelete { get; set; }
    public string PrimaryKeyName { get; set; } = "";
    public string PrimaryKeyType { get; set; } = "";
    public bool PrimaryKeyIsIdentity { get; set; }
    public List<PropertyManifestItem> Properties { get; set; } = new();
    public List<NavigationManifestItem> Navigations { get; set; } = new();
}

public class PropertyManifestItem
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string ColumnName { get; set; } = "";
    public string InputType { get; set; } = "text";
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsRequired { get; set; }
    public bool IsIdentity { get; set; }
    public int? MaxLength { get; set; }
}

public class NavigationManifestItem
{
    public string Name { get; set; } = "";
    public string TargetEntity { get; set; } = "";
    public string RelationType { get; set; } = "";
    public string ForeignKeyProperty { get; set; } = "";
    public bool IsCollection { get; set; }
}

public class ModuleInfo
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public int EntityCount { get; set; }
    public List<string> Entities { get; set; } = new();
}
