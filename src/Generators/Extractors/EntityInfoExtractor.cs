// =============================================================================
// RHSENSOERP GENERATOR v3.9 - ENTITY INFO EXTRACTOR
// =============================================================================
// Arquivo: src/Generators/Extractors/EntityInfoExtractor.cs
// Versão: 3.9 - CORRIGIDO: Detecção de navegações ICollection<T>
// =============================================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhSensoERP.Generators.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Extractors;

/// <summary>
/// Extrai informações de uma Entity para geração de CRUD.
/// </summary>
public static class EntityInfoExtractor
{
    /// <summary>
    /// Extrai informações de uma classe marcada com [GenerateCrud].
    /// </summary>
    public static EntityInfo? Extract(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
        if (classSymbol == null)
            return null;

        // Busca o atributo [GenerateCrud]
        var generateCrudAttr = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "GenerateCrudAttribute");

        if (generateCrudAttr == null)
            return null;

        var info = new EntityInfo
        {
            EntityName = classSymbol.Name,
            Namespace = classSymbol.ContainingNamespace.ToDisplayString(),
            GeneratorVersion = $"v{DateTime.Now:yyyy.MM.dd.HHmm} - FIXED NULLABLE"
        };

        // =====================================================================
        // EXTRAÇÃO DE ATRIBUTOS
        // =====================================================================
        ExtractAttributeValues(generateCrudAttr, info);

        // =====================================================================
        // NOME PLURAL (fallback)
        // =====================================================================
        if (string.IsNullOrEmpty(info.PluralName))
        {
            info.PluralName = Pluralize(info.EntityName);
        }

        // =====================================================================
        // DISPLAY NAME (fallback)
        // =====================================================================
        if (string.IsNullOrEmpty(info.DisplayName))
        {
            info.DisplayName = info.EntityName;
        }

        // =====================================================================
        // MODULE NAME (fallback)
        // =====================================================================
        if (string.IsNullOrEmpty(info.ModuleName))
        {
            info.ModuleName = ExtractModuleName(info.Namespace);
        }

        // =====================================================================
        // API ROUTE (fallback)
        // =====================================================================
        if (string.IsNullOrEmpty(info.ApiRoute))
        {
            var routeName = info.UsePluralRoute
                ? info.PluralName.ToLower()
                : info.EntityName.ToLower();

            info.ApiRoute = $"{info.ModuleName.ToLower()}/{routeName}";
        }

        // =====================================================================
        // API FULL ROUTE
        // =====================================================================
        info.ApiFullRoute = info.ApiRoute.StartsWith("api/")
            ? info.ApiRoute
            : $"api/{info.ApiRoute}";

        // =====================================================================
        // API GROUP (fallback)
        // =====================================================================
        if (string.IsNullOrEmpty(info.ApiGroup))
        {
            info.ApiGroup = info.ModuleName;
        }

        // =====================================================================
        // EXTRAÇÃO DE PROPRIEDADES
        // =====================================================================
        var properties = new List<PropertyInfo>();
        var navigations = new List<NavigationInfo>();

        foreach (var member in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            var propertyInfo = ExtractProperty(member, info);
            if (propertyInfo != null)
            {
                properties.Add(propertyInfo);
            }

            // Extrai navegações (relacionamentos)
            var navInfo = ExtractNavigation(member, info);
            if (navInfo != null)
            {
                navigations.Add(navInfo);
            }
        }

        info.Properties = properties;
        info.Navigations = navigations;

        // =====================================================================
        // DETECÇÃO DE CHAVE PRIMÁRIA (se não foi especificada)
        // =====================================================================
        if (string.IsNullOrEmpty(info.PrimaryKeyProperty))
        {
            var pkProp = properties.FirstOrDefault(p => p.IsPrimaryKey);
            if (pkProp != null)
            {
                info.PrimaryKeyProperty = pkProp.Name;
                info.PrimaryKeyType = pkProp.Type;
                info.PrimaryKeyColumn = pkProp.ColumnName;
            }
            else
            {
                // Fallback: procura por "Id" ou "{EntityName}Id"
                pkProp = properties.FirstOrDefault(p =>
                    p.Name == "Id" || p.Name == $"{info.EntityName}Id");

                if (pkProp != null)
                {
                    pkProp.IsPrimaryKey = true;
                    info.PrimaryKeyProperty = pkProp.Name;
                    info.PrimaryKeyType = pkProp.Type;
                    info.PrimaryKeyColumn = pkProp.ColumnName;
                }
            }
        }
        else
        {
            // Se foi especificada, marca a propriedade
            var pkProp = properties.FirstOrDefault(p => p.Name == info.PrimaryKeyProperty);
            if (pkProp != null)
            {
                pkProp.IsPrimaryKey = true;
                if (string.IsNullOrEmpty(info.PrimaryKeyType))
                {
                    info.PrimaryKeyType = pkProp.Type;
                }
                if (string.IsNullOrEmpty(info.PrimaryKeyColumn))
                {
                    info.PrimaryKeyColumn = pkProp.ColumnName;
                }
            }
        }

        // =====================================================================
        // DETECÇÃO DE CAMPOS DE AUDITORIA
        // =====================================================================
        DetectAuditFields(info);

        return info;
    }

    /// <summary>
    /// Extrai valores dos parâmetros do atributo [GenerateCrud].
    /// </summary>
    private static void ExtractAttributeValues(AttributeData attr, EntityInfo info)
    {
        foreach (var namedArg in attr.NamedArguments)
        {
            var value = namedArg.Value.Value;

            switch (namedArg.Key)
            {
                // Identificação
                case "TableName":
                    info.TableName = value?.ToString() ?? "";
                    break;
                case "Schema":
                    info.Schema = value?.ToString() ?? "dbo";
                    break;
                case "DisplayName":
                    info.DisplayName = value?.ToString() ?? "";
                    break;

                // Permissões
                case "CdSistema":
                    info.CdSistema = value?.ToString() ?? "";
                    break;
                case "CdFuncao":
                    info.CdFuncao = value?.ToString() ?? "";
                    break;

                // Rotas
                case "ApiRoute":
                    info.ApiRoute = value?.ToString() ?? "";
                    break;
                case "ApiGroup":
                    info.ApiGroup = value?.ToString() ?? "";
                    break;
                case "UsePluralRoute":
                    info.UsePluralRoute = value is bool b && b;
                    break;

                // Chave Primária
                case "PrimaryKeyProperty":
                    info.PrimaryKeyProperty = value?.ToString() ?? "";
                    break;
                case "PrimaryKeyType":
                    info.PrimaryKeyType = value?.ToString() ?? "";
                    break;

                // Flags de Geração
                case "GenerateDto":
                    info.GenerateDto = value is bool gd && gd;
                    break;
                case "GenerateRequests":
                    info.GenerateRequests = value is bool gr && gr;
                    break;
                case "GenerateCommands":
                    info.GenerateCommands = value is bool gc && gc;
                    break;
                case "GenerateQueries":
                    info.GenerateQueries = value is bool gq && gq;
                    break;
                case "GenerateValidators":
                    info.GenerateValidators = value is bool gv && gv;
                    break;
                case "GenerateRepository":
                    info.GenerateRepository = value is bool grep && grep;
                    break;
                case "GenerateMapper":
                    info.GenerateMapper = value is bool gm && gm;
                    break;
                case "GenerateEfConfig":
                    info.GenerateEfConfig = value is bool gef && gef;
                    break;
                case "GenerateMetadata":
                    info.GenerateMetadata = value is bool gmet && gmet;
                    break;
                case "GenerateApiController":
                    info.GenerateApiController = value is bool gac && gac;
                    break;
                case "GenerateWebController":
                    info.GenerateWebController = value is bool gwc && gwc;
                    break;
                case "GenerateWebModels":
                    info.GenerateWebModels = value is bool gwm && gwm;
                    break;
                case "GenerateWebServices":
                    info.GenerateWebServices = value is bool gws && gws;
                    break;

                // Comportamentos
                case "SupportsBatchDelete":
                    info.SupportsBatchDelete = value is bool sbd && sbd;
                    break;
                case "ApiRequiresAuth":
                    info.ApiRequiresAuth = value is bool ara && ara;
                    break;
                case "IsLegacyTable":
                    info.IsLegacyTable = value is bool ilt && ilt;
                    break;
            }
        }
    }

    /// <summary>
    /// Extrai informações de uma propriedade.
    /// </summary>
    private static PropertyInfo? ExtractProperty(IPropertySymbol property, EntityInfo entityInfo)
    {
        var propertyType = property.Type;
        var propertyTypeName = propertyType.ToDisplayString();

        // ✅ DETECÇÃO ANTECIPADA DE NAVEGAÇÕES
        var isCollectionType =
            propertyTypeName.Contains("ICollection<") ||
            propertyTypeName.Contains("IEnumerable<") ||
            propertyTypeName.Contains("List<") ||
            propertyTypeName.Contains("HashSet<");

        var hasVirtualModifier = property.IsVirtual;

        // ✅ Se é navegação, não processa como propriedade normal
        if (isCollectionType || (hasVirtualModifier && !propertyType.IsValueType && propertyTypeName != "string"))
        {
            // Será processado por ExtractNavigation
            return null;
        }

        var prop = new PropertyInfo
        {
            Name = property.Name,
            Type = GetCSharpTypeName(propertyType),
            ColumnName = GetColumnName(property),
            DisplayName = GetDisplayName(property),
            IsNullable = propertyType.NullableAnnotation == NullableAnnotation.Annotated ||
                         (propertyType is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T),
            IsString = propertyType.SpecialType == SpecialType.System_String,
            IsBool = propertyType.SpecialType == SpecialType.System_Boolean,
            IsNumeric = IsNumericType(propertyType),
            IsDateTime = IsDateTimeType(propertyType),
            IsGuid = propertyTypeName.Contains("Guid"),
            IsPrimaryKey = HasAttribute(property, "Key") || HasAttribute(property, "KeyAttribute"),
            IsRequired = HasAttribute(property, "Required") || HasAttribute(property, "RequiredAttribute"),
            IsReadOnly = property.IsReadOnly,
            IsNavigation = false, // ✅ Já foi filtrado acima
            ExcludeFromDto = HasAttribute(property, "NotMapped") || HasAttribute(property, "NotMappedAttribute"),
            IsIdentity = HasAttribute(property, "DatabaseGenerated") &&
                        GetDatabaseGeneratedOption(property) == "Identity",
            MaxLength = GetMaxLength(property),
            MinLength = GetMinLength(property),
            DefaultValue = GetDefaultValue(property),
            RequiredOnCreate = HasAttribute(property, "RequiredOnCreate")
        };

        return prop;
    }

    /// <summary>
    /// Extrai informações de navegação (relacionamentos).
    /// </summary>
    private static NavigationInfo? ExtractNavigation(IPropertySymbol property, EntityInfo entityInfo)
    {
        var propertyType = property.Type;
        var propertyTypeName = propertyType.ToDisplayString();

        // ✅ Detecção de ICollection, IEnumerable, List, etc
        var isCollectionType =
            propertyTypeName.Contains("ICollection<") ||
            propertyTypeName.Contains("IEnumerable<") ||
            propertyTypeName.Contains("List<") ||
            propertyTypeName.Contains("HashSet<");

        var hasVirtualModifier = property.IsVirtual;

        // Se não é virtual e não é coleção, não é navegação
        if (!hasVirtualModifier && !isCollectionType)
            return null;

        // Se é tipo valor (int, bool, etc) e virtual, também não é navegação
        if (propertyType.IsValueType && !isCollectionType)
            return null;

        // Se é string virtual, também não é navegação
        if (propertyTypeName == "string")
            return null;

        var nav = new NavigationInfo
        {
            Name = property.Name,
            IsNullable = propertyType.NullableAnnotation == NullableAnnotation.Annotated
        };

        if (isCollectionType)
        {
            // ✅ OneToMany - Extrai o tipo genérico
            nav.RelationshipType = NavigationRelationshipType.OneToMany;
            nav.TargetEntity = ExtractGenericTypeName(propertyTypeName);
            nav.TargetEntityFullName = ExtractGenericTypeFullName(propertyType);
        }
        else
        {
            // ✅ ManyToOne ou OneToOne
            nav.RelationshipType = NavigationRelationshipType.ManyToOne;
            nav.TargetEntity = propertyType.Name;
            nav.TargetEntityFullName = propertyType.ToDisplayString();

            // Tenta detectar a FK
            var fkAttr = property.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "ForeignKeyAttribute" ||
                                    a.AttributeClass?.Name == "ForeignKey");

            if (fkAttr != null && fkAttr.ConstructorArguments.Length > 0)
            {
                nav.ForeignKeyProperty = fkAttr.ConstructorArguments[0].Value?.ToString() ?? "";
            }
            else
            {
                // Convenção: Id{PropertyName} ou {PropertyName}Id
                nav.ForeignKeyProperty = $"Id{property.Name}";
            }
        }

        // Inverse Property
        var inverseAttr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "InversePropertyAttribute" ||
                                a.AttributeClass?.Name == "InverseProperty");

        if (inverseAttr != null && inverseAttr.ConstructorArguments.Length > 0)
        {
            nav.InverseProperty = inverseAttr.ConstructorArguments[0].Value?.ToString();
        }

        return nav;
    }

    /// <summary>
    /// Extrai o nome do tipo genérico de uma coleção.
    /// Ex: "ICollection<AgenciaBancaria>" -> "AgenciaBancaria"
    /// </summary>
    private static string ExtractGenericTypeName(string fullTypeName)
    {
        var startIndex = fullTypeName.IndexOf('<');
        var endIndex = fullTypeName.LastIndexOf('>');

        if (startIndex > 0 && endIndex > startIndex)
        {
            var genericPart = fullTypeName.Substring(startIndex + 1, endIndex - startIndex - 1);
            var lastDot = genericPart.LastIndexOf('.');
            return lastDot > 0 ? genericPart.Substring(lastDot + 1) : genericPart;
        }

        return "";
    }

    /// <summary>
    /// Extrai o nome completo do tipo genérico.
    /// </summary>
    private static string ExtractGenericTypeFullName(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var genericArg = namedType.TypeArguments.FirstOrDefault();
            return genericArg?.ToDisplayString() ?? "";
        }

        return "";
    }

    /// <summary>
    /// Detecta campos de auditoria na entidade.
    /// </summary>
    private static void DetectAuditFields(EntityInfo info)
    {
        var auditFieldMappings = new Dictionary<string, Action<string>>
        {
            ["CreatedAt"] = field => info.CreatedAtField = field,
            ["DataCriacao"] = field => info.CreatedAtField = field,
            ["Aud_CreatedAt"] = field => info.CreatedAtField = field,

            ["CreatedBy"] = field => info.CreatedByField = field,
            ["IdUsuarioCriacao"] = field => info.CreatedByField = field,
            ["Aud_IdUsuarioCadastro"] = field => info.CreatedByField = field,

            ["UpdatedAt"] = field => info.UpdatedAtField = field,
            ["DataAtualizacao"] = field => info.UpdatedAtField = field,
            ["Aud_UpdatedAt"] = field => info.UpdatedAtField = field,

            ["UpdatedBy"] = field => info.UpdatedByField = field,
            ["IdUsuarioAtualizacao"] = field => info.UpdatedByField = field,
            ["Aud_IdUsuarioAtualizacao"] = field => info.UpdatedByField = field
        };

        foreach (var prop in info.Properties)
        {
            if (auditFieldMappings.TryGetValue(prop.Name, out var setter))
            {
                setter(prop.Name);
            }
        }

        info.HasAuditFields = !string.IsNullOrEmpty(info.CreatedAtField) ||
                             !string.IsNullOrEmpty(info.CreatedByField) ||
                             !string.IsNullOrEmpty(info.UpdatedAtField) ||
                             !string.IsNullOrEmpty(info.UpdatedByField);
    }

    // =========================================================================
    // MÉTODOS AUXILIARES
    // =========================================================================

    private static string GetColumnName(IPropertySymbol property)
    {
        var columnAttr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "ColumnAttribute" ||
                                a.AttributeClass?.Name == "Column");

        if (columnAttr?.ConstructorArguments.Length > 0)
        {
            return columnAttr.ConstructorArguments[0].Value?.ToString() ?? property.Name;
        }

        return property.Name;
    }

    private static string GetDisplayName(IPropertySymbol property)
    {
        var displayAttr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "DisplayAttribute" ||
                                a.AttributeClass?.Name == "Display");

        if (displayAttr != null)
        {
            var nameArg = displayAttr.NamedArguments
                .FirstOrDefault(arg => arg.Key == "Name");

            if (nameArg.Value.Value != null)
            {
                return nameArg.Value.Value.ToString()!;
            }
        }

        return SplitCamelCase(property.Name);
    }

    private static bool HasAttribute(IPropertySymbol property, string attributeName)
    {
        return property.GetAttributes()
            .Any(a => a.AttributeClass?.Name == attributeName ||
                     a.AttributeClass?.Name == $"{attributeName}Attribute");
    }

    private static string? GetDatabaseGeneratedOption(IPropertySymbol property)
    {
        var attr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "DatabaseGeneratedAttribute" ||
                                a.AttributeClass?.Name == "DatabaseGenerated");

        if (attr?.ConstructorArguments.Length > 0)
        {
            return attr.ConstructorArguments[0].Value?.ToString();
        }

        return null;
    }

    private static string GetDefaultValue(IPropertySymbol property)
    {
        // 1. Sintaxe (Initializer) -> public string Prop { get; set; } = "Valor";
        var syntaxRef = property.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxRef != null)
        {
            var syntax = syntaxRef.GetSyntax() as PropertyDeclarationSyntax;
            if (syntax?.Initializer != null)
            {
                return syntax.Initializer.Value.ToString();
            }
        }

        // 2. Atributo [DefaultValue("Valor")]
        var attr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "DefaultValueAttribute" ||
                                a.AttributeClass?.Name == "DefaultValue");

        if (attr?.ConstructorArguments.Length > 0)
        {
            var arg = attr.ConstructorArguments[0];
            var val = arg.Value;

            if (val is string s) return $"\"{s}\"";
            if (val is bool b) return b.ToString().ToLower();
            if (val is char c) return $"'{c}'";
            // Para outros tipos (int, double, etc) o ToString() geralmente resolve
            return val?.ToString() ?? "";
        }

        return "";
    }

    private static int? GetMaxLength(IPropertySymbol property)
    {
        var attr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "MaxLengthAttribute" ||
                                a.AttributeClass?.Name == "MaxLength" ||
                                a.AttributeClass?.Name == "StringLengthAttribute" ||
                                a.AttributeClass?.Name == "StringLength");

        if (attr?.ConstructorArguments.Length > 0 &&
            attr.ConstructorArguments[0].Value is int maxLen)
        {
            return maxLen;
        }

        return null;
    }

    private static int? GetMinLength(IPropertySymbol property)
    {
        var attr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "MinLengthAttribute" ||
                                a.AttributeClass?.Name == "MinLength");

        if (attr?.ConstructorArguments.Length > 0 &&
            attr.ConstructorArguments[0].Value is int minLen)
        {
            return minLen;
        }

        var stringLengthAttr = property.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "StringLengthAttribute" ||
                                a.AttributeClass?.Name == "StringLength");

        if (stringLengthAttr != null)
        {
            var minLenArg = stringLengthAttr.NamedArguments
                .FirstOrDefault(arg => arg.Key == "MinimumLength");

            if (minLenArg.Value.Value is int min)
            {
                return min;
            }
        }

        return null;
    }

    private static bool IsNumericType(ITypeSymbol type)
    {
        // Unwrap Nullable<T> if needed
        if (type is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            type = namedType.TypeArguments.First();
        }

        return type.SpecialType switch
        {
            SpecialType.System_Int16 => true,
            SpecialType.System_Int32 => true,
            SpecialType.System_Int64 => true,
            SpecialType.System_Decimal => true,
            SpecialType.System_Double => true,
            SpecialType.System_Single => true,
            SpecialType.System_Byte => true,
            _ => false
        };
    }

    private static bool IsDateTimeType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString();
        return typeName.Contains("DateTime") ||
               typeName.Contains("DateTimeOffset") ||
               typeName.Contains("DateOnly") ||
               typeName.Contains("TimeOnly");
    }

    private static string GetCSharpTypeName(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType)
        {
            if (namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T || 
                namedType.Name == "Nullable")
            {
                var underlyingType = namedType.TypeArguments.FirstOrDefault();
                if (underlyingType != null)
                {
                    return $"{GetCSharpTypeName(underlyingType)}?";
                }
            }
        }

        // Fix for explicit TimeSpan recognition
        if (type.Name == "TimeSpan" || type.ToDisplayString() == "System.TimeSpan")
        {
            return "TimeSpan";
        }

        return type.SpecialType switch
        {
            SpecialType.System_String => "string",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Int16 => "short",
            SpecialType.System_Decimal => "decimal",
            SpecialType.System_Double => "double",
            SpecialType.System_Single => "float",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Byte => "byte",
            SpecialType.System_DateTime => "DateTime",
            _ => type.Name
        };
    }

    private static string Pluralize(string name)
    {
        if (name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("z"))
            return name + "es";

        if (name.EndsWith("y") && name.Length > 1 && !IsVowel(name[name.Length - 2]))
            return name.Substring(0, name.Length - 1) + "ies";

        return name + "s";
    }

    private static bool IsVowel(char c)
    {
        return "aeiouAEIOU".IndexOf(c) >= 0;
    }

    private static string SplitCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToUpper(str[0]));

        for (int i = 1; i < str.Length; i++)
        {
            if (char.IsUpper(str[i]) && i > 0 && !char.IsUpper(str[i - 1]))
            {
                result.Append(' ');
            }
            result.Append(str[i]);
        }

        return result.ToString();
    }

    private static string ExtractModuleName(string namespaceName)
    {
        // Ex: RhSensoERP.Modules.AdministracaoPessoal.Core.Entities
        var parts = namespaceName.Split('.');

        if (parts.Length >= 3 && parts[1] == "Modules")
        {
            return parts[2]; // AdministracaoPessoal
        }

        // Ex: RhSensoERP.Identity.Core.Entities
        if (parts.Length >= 2)
        {
            return parts[1]; // Identity
        }

        return "Unknown";
    }
}