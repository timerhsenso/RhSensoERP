// =============================================================================
// RHSENSOERP - MANIFEST CONTROLLER
// =============================================================================
// Arquivo: src/API/Controllers/ManifestController.cs
// Descrição: Expõe manifesto das entidades para o GeradorfullStack consumir
// Versão: 1.1 - CORRIGIDO: IsNullable agora respeita atributo [Required]
// Data: 2025-12-25
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller que expõe manifesto das entidades do backend.
/// Usado pelo GeradorfullStack para gerar frontend automaticamente.
/// </summary>
/// <remarks>
/// Este controller usa Reflection em runtime para extrair metadados das entidades
/// marcadas com [GenerateCrud]. Os metadados incluem propriedades, tipos, validações
/// e configurações de UI que o frontend consome para renderização dinâmica.
/// </remarks>
#if DEBUG
[EnableCors("ManifestDev")]
#endif

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Sistema")]
public class ManifestController : ControllerBase
{
    private readonly ILogger<ManifestController> _logger;
    private static ManifestData? _cachedManifest;
    private static readonly object _lock = new();

    public ManifestController(ILogger<ManifestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Retorna o manifesto completo de entidades.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 300)]
    public IActionResult GetManifest()
    {
        var manifest = GetOrBuildManifest();
        return Ok(manifest);
    }

    /// <summary>
    /// Lista módulos disponíveis.
    /// </summary>
    [HttpGet("modules")]
    [AllowAnonymous]
    public IActionResult GetModules()
    {
        var manifest = GetOrBuildManifest();

        var modules = manifest.Entities
            .GroupBy(e => e.ModuleName)
            .Select(g => new
            {
                Name = g.Key,
                DisplayName = GetModuleDisplayName(g.Key),
                Count = g.Count(),
                Entities = g.Select(e => new
                {
                    e.EntityName,
                    e.DisplayName,
                    e.TableName,
                    e.Route
                }).OrderBy(e => e.DisplayName).ToList()
            })
            .OrderBy(m => m.DisplayName)
            .ToList();

        return Ok(modules);
    }

    /// <summary>
    /// Retorna entidades de um módulo.
    /// </summary>
    [HttpGet("modules/{moduleName}")]
    [AllowAnonymous]
    public IActionResult GetModuleEntities(string moduleName)
    {
        var manifest = GetOrBuildManifest();

        var entities = manifest.Entities
            .Where(e => e.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!entities.Any())
            return NotFound(new { error = $"Módulo '{moduleName}' não encontrado" });

        return Ok(entities);
    }

    /// <summary>
    /// Retorna uma entidade específica pelo nome.
    /// </summary>
    [HttpGet("{name}")]
    [AllowAnonymous]
    public IActionResult GetEntity(string name)
    {
        var manifest = GetOrBuildManifest();

        var entity = manifest.Entities
            .FirstOrDefault(e => e.EntityName.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (entity == null)
            return NotFound(new { error = $"Entidade '{name}' não encontrada" });

        return Ok(entity);
    }

    /// <summary>
    /// Força reconstrução do cache.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public IActionResult RefreshManifest()
    {
        lock (_lock)
        {
            _cachedManifest = null;
        }

        var manifest = GetOrBuildManifest();
        return Ok(new
        {
            message = "Cache atualizado",
            entityCount = manifest.Entities.Count,
            generatedAt = manifest.GeneratedAt
        });
    }

    /// <summary>
    /// Download do JSON.
    /// </summary>
    [HttpGet("download")]
    [AllowAnonymous]
    public IActionResult DownloadManifest()
    {
        var manifest = GetOrBuildManifest();
        var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "entity-manifest.json");
    }

    #region Build Manifest via Reflection

    private ManifestData GetOrBuildManifest()
    {
        if (_cachedManifest != null)
            return _cachedManifest;

        lock (_lock)
        {
            if (_cachedManifest != null)
                return _cachedManifest;

            _cachedManifest = BuildManifest();
            return _cachedManifest;
        }
    }

    private ManifestData BuildManifest()
    {
        _logger.LogInformation("Construindo manifesto de entidades via reflection...");

        var manifest = new ManifestData
        {
            GeneratedAt = DateTime.UtcNow.ToString("O"),
            Version = "1.0.0"
        };

        // Procura assemblies do RhSensoERP
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.StartsWith("RhSensoERP") == true)
            .ToList();

        foreach (var assembly in assemblies)
        {
            try
            {
                ProcessAssembly(assembly, manifest);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao processar assembly {Assembly}", assembly.FullName);
            }
        }

        // Ordena por módulo e nome
        manifest.Entities = manifest.Entities
            .OrderBy(e => e.ModuleName)
            .ThenBy(e => e.EntityName)
            .ToList();

        _logger.LogInformation("Manifesto construído: {Count} entidades de {Modules} módulos",
            manifest.Entities.Count,
            manifest.Entities.Select(e => e.ModuleName).Distinct().Count());

        return manifest;
    }

    private void ProcessAssembly(Assembly assembly, ManifestData manifest)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null).ToArray()!;
        }

        var entityTypes = types
            .Where(t => t != null && t.IsClass && !t.IsAbstract)
            .Where(t => HasGenerateCrudAttribute(t));

        foreach (var type in entityTypes)
        {
            try
            {
                var entityInfo = ExtractEntityInfo(type);
                if (entityInfo != null)
                {
                    manifest.Entities.Add(entityInfo);
                    _logger.LogDebug("Entidade extraída: {Entity} ({Module})",
                        entityInfo.EntityName, entityInfo.ModuleName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao extrair info de {Type}", type.Name);
            }
        }
    }

    private static bool HasGenerateCrudAttribute(Type type)
    {
        return type.GetCustomAttributes()
            .Any(a => a.GetType().Name == "GenerateCrudAttribute");
    }

    private EntityManifestItem? ExtractEntityInfo(Type type)
    {
        var attr = type.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "GenerateCrudAttribute");

        if (attr == null) return null;

        // Extrai valores do atributo via reflection
        var attrType = attr.GetType();
        string GetAttrValue(string propName, string defaultVal = "")
        {
            var prop = attrType.GetProperty(propName);
            return prop?.GetValue(attr)?.ToString() ?? defaultVal;
        }

        bool GetAttrBool(string propName, bool defaultVal = false)
        {
            var prop = attrType.GetProperty(propName);
            if (prop == null) return defaultVal;
            return (bool)(prop.GetValue(attr) ?? defaultVal);
        }

        // Detecta módulo pelo namespace
        var moduleName = ExtractModuleName(type.Namespace ?? "");
        var moduleRoute = moduleName.ToLowerInvariant();

        // Nome da entidade
        var entityName = type.Name;

        // Rota - usa ApiRoute do atributo ou gera automaticamente
        var apiRoute = GetAttrValue("ApiRoute");
        var usePluralRoute = GetAttrBool("UsePluralRoute", false);

        string routeName;
        if (!string.IsNullOrEmpty(apiRoute))
        {
            routeName = apiRoute;
        }
        else
        {
            routeName = entityName.ToLowerInvariant();
            // Remove prefixos comuns
            var prefixes = new[] { "cap_", "gtc_", "int_", "tre_", "doc_", "epi_", "base_" };
            foreach (var prefix in prefixes)
            {
                if (routeName.StartsWith(prefix))
                {
                    routeName = routeName.Substring(prefix.Length);
                    break;
                }
            }
        }

        // Cria o item do manifesto
        var item = new EntityManifestItem
        {
            EntityName = entityName,
            FullName = type.FullName ?? entityName,
            DisplayName = !string.IsNullOrEmpty(GetAttrValue("DisplayName"))
                ? GetAttrValue("DisplayName")
                : FormatDisplayName(entityName),
            TableName = !string.IsNullOrEmpty(GetAttrValue("TableName"))
                ? GetAttrValue("TableName")
                : entityName,
            Schema = GetAttrValue("Schema", "dbo"),
            ModuleName = moduleName,
            ModuleDisplayName = GetModuleDisplayName(moduleName),
            CdSistema = GetAttrValue("CdSistema"),
            CdFuncao = GetAttrValue("CdFuncao"),
            Route = $"api/{moduleRoute}/{routeName}",
            Icon = GetAttrValue("Icon", "fas fa-table"),
            RequiresAuth = GetAttrBool("ApiRequiresAuth", true),
            SupportsBatchDelete = GetAttrBool("SupportsBatchDelete", true),
            UsePluralRoute = usePluralRoute
        };

        // Extrai propriedades
        item.Properties = ExtractProperties(type).ToList();

        // PK
        var pk = item.Properties.FirstOrDefault(p => p.IsPrimaryKey);
        if (pk != null)
        {
            item.PrimaryKeyName = pk.Name;
            item.PrimaryKeyType = pk.Type;
            item.PrimaryKeyIsIdentity = pk.IsIdentity;
        }

        // Extrai navegações
        item.Navigations = ExtractNavigations(type).ToList();

        return item;
    }

    /// <summary>
    /// Extrai propriedades da entidade via Reflection.
    /// </summary>
    /// <param name="type">Tipo da entidade</param>
    /// <returns>Lista de metadados das propriedades</returns>
    /// <remarks>
    /// CORREÇÃO v1.1: IsNullable agora respeita o atributo [Required].
    /// Anteriormente, string com [Required] era marcada incorretamente como nullable=true.
    /// 
    /// Lógica de IsNullable:
    /// - Se tem [Required] → IsNullable = false (mesmo para reference types como string)
    /// - Se não tem [Required] → IsNullable baseado no tipo (string=true, int?=true, int=false)
    /// </remarks>
    private static IEnumerable<PropertyManifestItem> ExtractProperties(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in properties)
        {
            // Ignora navigation properties (tipos complexos, coleções)
            if (IsNavigationProperty(prop))
                continue;

            // ✅ CORREÇÃO v1.1: Detecta [Required] e sobrescreve IsNullable
            // ANTES: IsNullable = IsNullable(prop) - nunca verificava [Required]
            // DEPOIS: Se tem [Required], força IsNullable = false
            var hasRequiredAttribute = HasAttribute(prop, "RequiredAttribute");
            var isNullableFromType = IsNullable(prop);
            var isNullable = hasRequiredAttribute ? false : isNullableFromType;

            var item = new PropertyManifestItem
            {
                Name = prop.Name,
                Type = GetTypeName(prop.PropertyType),
                IsNullable = isNullable,  // ✅ Agora respeita [Required]
                IsPrimaryKey = HasAttribute(prop, "KeyAttribute") ||
                               prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase),
                IsRequired = hasRequiredAttribute ||
                             (!isNullableFromType && GetTypeName(prop.PropertyType) != "string"),
                IsIdentity = HasDatabaseGeneratedIdentity(prop),
                DisplayName = GetDisplayName(prop),
                ColumnName = GetColumnName(prop),
                MaxLength = GetMaxLength(prop)
            };

            // Determina InputType baseado no tipo
            item.InputType = DetermineInputType(item);

            yield return item;
        }
    }

    private static IEnumerable<NavigationManifestItem> ExtractNavigations(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        foreach (var prop in properties)
        {
            if (!IsNavigationProperty(prop))
                continue;

            var propType = prop.PropertyType;
            bool isCollection = propType.IsGenericType &&
                (propType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 propType.GetGenericTypeDefinition() == typeof(List<>) ||
                 propType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            string targetEntity;
            if (isCollection)
            {
                targetEntity = propType.GetGenericArguments()[0].Name;
            }
            else
            {
                targetEntity = propType.Name;
            }

            // Tenta encontrar a FK
            var fkAttr = prop.GetCustomAttributes()
                .FirstOrDefault(a => a.GetType().Name == "ForeignKeyAttribute");
            var fkProperty = "";
            if (fkAttr != null)
            {
                var nameProp = fkAttr.GetType().GetProperty("Name");
                fkProperty = nameProp?.GetValue(fkAttr)?.ToString() ?? "";
            }

            yield return new NavigationManifestItem
            {
                Name = prop.Name,
                TargetEntity = targetEntity,
                IsCollection = isCollection,
                RelationType = isCollection ? "OneToMany" : "ManyToOne",
                ForeignKeyProperty = fkProperty
            };
        }
    }

    #endregion

    #region Helpers

    private static string ExtractModuleName(string ns)
    {
        if (string.IsNullOrEmpty(ns)) return "Unknown";

        // RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities -> GestaoDeTerceiros
        if (ns.Contains(".Modules."))
        {
            var afterModules = ns.Split(".Modules.").LastOrDefault() ?? "";
            return afterModules.Split('.').FirstOrDefault() ?? "Unknown";
        }

        // RhSensoERP.Identity.Core.Entities -> Identity
        var parts = ns.Split('.');
        if (parts.Length >= 2 && parts[0] == "RhSensoERP")
        {
            return parts[1];
        }

        return "Unknown";
    }

    private static string GetModuleDisplayName(string moduleName) => moduleName switch
    {
        "GestaoDeTerceiros" => "Gestão de Terceiros",
        "GestaoDePessoas" => "Gestão de Pessoas",
        "AdministracaoPessoal" => "Administração de Pessoal",
        "TreinamentoDesenvolvimento" => "Treinamento e Desenvolvimento",
        "ControleAcessoPortaria" => "Controle de Acesso e Portaria",
        "ControleDePonto" => "Controle de Ponto",
        "SaudeOcupacional" => "Saúde Ocupacional",
        "Integracoes" => "Integrações",
        "Seguranca" => "Segurança",
        "Identity" => "Identidade e Segurança",
        "RecursosHumanos" => "Recursos Humanos",
        "Treinamentos" => "Treinamentos",
        "AuditoriaCompliance" => "Auditoria e Compliance",
        "Avaliacoes" => "Avaliações",
        "Esocial" => "eSocial",
        "GestaoDeEPI" => "Gestão de EPI",
        _ => moduleName
    };

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        // Remove prefixos comuns
        var prefixes = new[] { "CAP_", "GTC_", "INT_", "TRE_", "DOC_", "EPI_", "BASE_" };
        foreach (var prefix in prefixes)
        {
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(prefix.Length);
                break;
            }
        }

        // Adiciona espaços antes de maiúsculas
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                result.Append(' ');
            result.Append(name[i]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Determina se uma propriedade é de navegação (relacionamento EF).
    /// </summary>
    /// <remarks>
    /// Navigation properties incluem:
    /// - Coleções genéricas (ICollection, List, IEnumerable)
    /// - Classes de domínio (não System.*) 
    /// Exclui: string, byte[], tipos primitivos
    /// </remarks>
    private static bool IsNavigationProperty(PropertyInfo prop)
    {
        var propType = prop.PropertyType;

        // É coleção genérica
        if (propType.IsGenericType)
        {
            var genericDef = propType.GetGenericTypeDefinition();
            if (genericDef == typeof(ICollection<>) ||
                genericDef == typeof(List<>) ||
                genericDef == typeof(IEnumerable<>) ||
                genericDef == typeof(IList<>))
            {
                return true;
            }
        }

        // É classe de domínio (não é tipo primitivo/sistema)
        if (propType.IsClass &&
            propType != typeof(string) &&
            propType != typeof(byte[]) &&
            propType.Namespace != null &&
            !propType.Namespace.StartsWith("System"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converte Type em string amigável (ex: int?, DateTime, Guid).
    /// </summary>
    private static string GetTypeName(Type type)
    {
        if (type == typeof(string)) return "string";
        if (type == typeof(int)) return "int";
        if (type == typeof(int?)) return "int?";
        if (type == typeof(long)) return "long";
        if (type == typeof(long?)) return "long?";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(decimal?)) return "decimal?";
        if (type == typeof(double)) return "double";
        if (type == typeof(double?)) return "double?";
        if (type == typeof(float)) return "float";
        if (type == typeof(float?)) return "float?";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(bool?)) return "bool?";
        if (type == typeof(DateTime)) return "DateTime";
        if (type == typeof(DateTime?)) return "DateTime?";
        if (type == typeof(DateOnly)) return "DateOnly";
        if (type == typeof(DateOnly?)) return "DateOnly?";
        if (type == typeof(TimeOnly)) return "TimeOnly";
        if (type == typeof(TimeOnly?)) return "TimeOnly?";
        if (type == typeof(Guid)) return "Guid";
        if (type == typeof(Guid?)) return "Guid?";
        if (type == typeof(byte[])) return "byte[]";
        if (type == typeof(short)) return "short";
        if (type == typeof(short?)) return "short?";
        if (type == typeof(byte)) return "byte";
        if (type == typeof(byte?)) return "byte?";

        return type.Name;
    }

    /// <summary>
    /// Determina se uma propriedade é nullable baseado no tipo.
    /// </summary>
    /// <remarks>
    /// Para Value Types: nullable se for Nullable&lt;T&gt; (ex: int?)
    /// Para Reference Types: nullable por padrão (string, classes)
    /// NOTA: Este método NÃO verifica [Required]. Isso é feito em ExtractProperties.
    /// </remarks>
    private static bool IsNullable(PropertyInfo prop)
    {
        var type = prop.PropertyType;
        if (type.IsValueType)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        // Para reference types, verifica NullableAttribute ou contexto
        var nullable = prop.CustomAttributes
            .Any(a => a.AttributeType.Name == "NullableAttribute");

        return nullable || type == typeof(string);
    }

    private static bool HasAttribute(PropertyInfo prop, string attrName)
    {
        return prop.GetCustomAttributes()
            .Any(a => a.GetType().Name == attrName);
    }

    private static bool HasDatabaseGeneratedIdentity(PropertyInfo prop)
    {
        var attr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "DatabaseGeneratedAttribute");

        if (attr == null) return false;

        var optionProp = attr.GetType().GetProperty("DatabaseGeneratedOption");
        var value = optionProp?.GetValue(attr);
        return value?.ToString() == "Identity";
    }

    private static string GetDisplayName(PropertyInfo prop)
    {
        // Procura DisplayAttribute ou DisplayNameAttribute
        var displayAttr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name is "DisplayAttribute" or "DisplayNameAttribute");

        if (displayAttr != null)
        {
            var nameProp = displayAttr.GetType().GetProperty("Name");
            var val = nameProp?.GetValue(displayAttr)?.ToString();
            if (!string.IsNullOrEmpty(val)) return val;
        }

        // Procura FieldDisplayNameAttribute (custom)
        var fieldDisplayAttr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "FieldDisplayNameAttribute");

        if (fieldDisplayAttr != null)
        {
            var nameProp = fieldDisplayAttr.GetType().GetProperty("DisplayName");
            var val = nameProp?.GetValue(fieldDisplayAttr)?.ToString();
            if (!string.IsNullOrEmpty(val)) return val;
        }

        return FormatDisplayName(prop.Name);
    }

    private static string GetColumnName(PropertyInfo prop)
    {
        var columnAttr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "ColumnAttribute");

        if (columnAttr != null)
        {
            var nameProp = columnAttr.GetType().GetProperty("Name");
            var val = nameProp?.GetValue(columnAttr)?.ToString();
            if (!string.IsNullOrEmpty(val)) return val;
        }

        return prop.Name;
    }

    private static int? GetMaxLength(PropertyInfo prop)
    {
        var stringLengthAttr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "StringLengthAttribute");

        if (stringLengthAttr != null)
        {
            var maxProp = stringLengthAttr.GetType().GetProperty("MaximumLength");
            if (maxProp?.GetValue(stringLengthAttr) is int maxLen)
                return maxLen;
        }

        var maxLengthAttr = prop.GetCustomAttributes()
            .FirstOrDefault(a => a.GetType().Name == "MaxLengthAttribute");

        if (maxLengthAttr != null)
        {
            var lenProp = maxLengthAttr.GetType().GetProperty("Length");
            if (lenProp?.GetValue(maxLengthAttr) is int len)
                return len;
        }

        return null;
    }

    /// <summary>
    /// Determina o tipo de input HTML baseado no tipo da propriedade.
    /// </summary>
    private static string DetermineInputType(PropertyManifestItem prop)
    {
        if (prop.Type.Contains("DateTime")) return "datetime-local";
        if (prop.Type.Contains("DateOnly")) return "date";
        if (prop.Type.Contains("TimeOnly")) return "time";
        if (prop.Type == "bool" || prop.Type == "bool?") return "checkbox";
        if (prop.Type is "int" or "int?" or "long" or "long?" or "short" or "short?") return "number";
        if (prop.Type is "decimal" or "decimal?" or "double" or "double?" or "float" or "float?") return "number";
        if (prop.MaxLength.HasValue && prop.MaxLength > 255) return "textarea";

        return "text";
    }

    #endregion
}

#region Manifest Models

/// <summary>
/// Modelo de dados do manifesto completo.
/// </summary>
public class ManifestData
{
    public string GeneratedAt { get; set; } = "";
    public string Version { get; set; } = "";
    public List<EntityManifestItem> Entities { get; set; } = new();
}

/// <summary>
/// Metadados de uma entidade individual.
/// </summary>
public class EntityManifestItem
{
    // Identificação
    public string EntityName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string DisplayName { get; set; } = "";

    // Banco
    public string TableName { get; set; } = "";
    public string Schema { get; set; } = "";

    // Módulo
    public string ModuleName { get; set; } = "";
    public string ModuleDisplayName { get; set; } = "";

    // Permissões
    public string CdSistema { get; set; } = "";
    public string CdFuncao { get; set; } = "";

    // Rota
    public string Route { get; set; } = "";
    public bool UsePluralRoute { get; set; }

    // UI
    public string Icon { get; set; } = "";
    public bool RequiresAuth { get; set; }
    public bool SupportsBatchDelete { get; set; }

    // PK
    public string PrimaryKeyName { get; set; } = "";
    public string PrimaryKeyType { get; set; } = "";
    public bool PrimaryKeyIsIdentity { get; set; }

    // Coleções
    public List<PropertyManifestItem> Properties { get; set; } = new();
    public List<NavigationManifestItem> Navigations { get; set; } = new();
}

/// <summary>
/// Metadados de uma propriedade individual.
/// </summary>
public class PropertyManifestItem
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string ColumnName { get; set; } = "";
    public string InputType { get; set; } = "text";

    /// <summary>
    /// Indica se a propriedade aceita null.
    /// Para campos com [Required], este valor será false mesmo se for reference type.
    /// </summary>
    public bool IsNullable { get; set; }

    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Indica se a propriedade tem [Required] ou é obrigatória por tipo.
    /// </summary>
    public bool IsRequired { get; set; }

    public bool IsIdentity { get; set; }
    public int? MaxLength { get; set; }
}

/// <summary>
/// Metadados de uma navigation property (relacionamento).
/// </summary>
public class NavigationManifestItem
{
    public string Name { get; set; } = "";
    public string TargetEntity { get; set; } = "";
    public string RelationType { get; set; } = "";
    public string ForeignKeyProperty { get; set; } = "";
    public bool IsCollection { get; set; }
}

#endregion