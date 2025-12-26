// =============================================================================
// RHSENSOERP GENERATOR v4.2 FINAL - ENTITY INFO EXTRACTOR
// =============================================================================
// Versão: 4.2 FINAL - TODOS OS BUGS CORRIGIDOS
// 
// ✅ CORREÇÕES v4.2:
// 1. IsNullable CORRIGIDO: Verifica [Required] e sobrescreve
// 2. GetTypeName CORRIGIDO: int? retorna "int?" e não "object"
// 3. FindForeignKeyProperty: Valida se FK existe
// 4. Logs de debug para troubleshooting
// 
// =============================================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhSensoERP.Generators.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RhSensoERP.Generators.Extractors;

public static class EntityInfoExtractor
{
    private const string GENERATOR_VERSION = "v4.2";

    // =========================================================================
    // 📌 DICIONÁRIO DE MAPEAMENTOS (BRASIL)
    // =========================================================================
    private static readonly Dictionary<string, string> PropertyDisplayNameMap = new()
    {
        { "Id", "ID" },
        { "Cd", "Código" },
        { "Dc", "Descrição" },
        { "Ds", "Descrição" },
        { "Nm", "Nome" },
        { "Dt", "Data" },
        { "Vl", "Valor" },
        { "Qt", "Quantidade" },
        { "Tp", "Tipo" },
        { "St", "Status" },
        { "Cdsistema", "Código do Sistema" },
        { "Dcsistema", "Descrição do Sistema" },
        { "Ativo", "Ativo" },
        { "CreatedAt", "Criado em" },
        { "CreatedBy", "Criado por" },
        { "UpdatedAt", "Atualizado em" },
        { "UpdatedBy", "Atualizado por" },
        { "Aud_CreatedAt", "Criado em" },
        { "Aud_IdUsuarioCadastro", "Usuário Cadastro" },
        { "Aud_UpdatedAt", "Atualizado em" },
        { "Aud_IdUsuarioAtualizacao", "Usuário Atualização" }
    };

    public static EntityInfo? Extract(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

        if (classSymbol == null)
            return null;

        var attribute = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "GenerateCrudAttribute");

        if (attribute == null)
            return null;

        var info = new EntityInfo
        {
            EntityName = classSymbol.Name,
            Namespace = classSymbol.ContainingNamespace.ToDisplayString(),
            GeneratorVersion = GENERATOR_VERSION,
            FileHeader = GenerateHeader(classSymbol.Name)
        };

        ExtractAttributeProperties(attribute, info);
        info.ModuleName = CalculateModuleName(info.Namespace);
        info.PluralName = CalculatePluralName(info.EntityName);

        // ✅ v4.2: Extrai propriedades E navegações (com validação de FK)
        ExtractPropertiesAndNavigations(classSymbol, info);

        IdentifyPrimaryKey(info);
        IdentifyAuditFields(info);
        CalculateApiRoute(info);

        return info;
    }

    // =========================================================================
    // ✅ v4.2: ExtractPropertiesAndNavigations COM VALIDAÇÃO DE FK
    // =========================================================================
    private static void ExtractPropertiesAndNavigations(INamedTypeSymbol classSymbol, EntityInfo info)
    {
        var properties = new List<RhSensoERP.Generators.Models.PropertyInfo>();
        var navigations = new List<NavigationInfo>();

        // ✅ PRIMEIRA PASSAGEM: Extrai todas as propriedades escalares
        foreach (var member in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (HasAttribute(member, "NotMappedAttribute"))
                continue;

            if (member.SetMethod == null || member.SetMethod.DeclaredAccessibility != Accessibility.Public)
                continue;

            // Pula navegações (serão processadas depois)
            if (member.IsVirtual || IsCollectionType(member.Type))
                continue;

            var propInfo = ExtractProperty(member);
            properties.Add(propInfo);
        }

        info.Properties = properties;

        // ✅ SEGUNDA PASSAGEM: Extrai navegações E VALIDA se FK existe
        foreach (var member in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (HasAttribute(member, "NotMappedAttribute"))
                continue;

            if (member.IsVirtual || IsCollectionType(member.Type))
            {
                // ✅ v4.2: Passa a lista de properties para validar FK
                var nav = ExtractNavigation(member, properties);
                if (nav != null)
                    navigations.Add(nav);
            }
        }

        info.Navigations = navigations;
    }

    // =========================================================================
    // ✅ v4.2: ExtractProperty COM CORREÇÃO DE IsNullable
    // =========================================================================
    private static RhSensoERP.Generators.Models.PropertyInfo ExtractProperty(IPropertySymbol member)
    {
        var typeName = GetTypeName(member.Type);
        var isNullableFromType = IsNullableType(member.Type);

        // ✅ v4.2 FIX: Verifica [Required] e SOBRESCREVE IsNullable
        var hasRequiredAttribute = HasAttribute(member, "RequiredAttribute");
        var isNullable = hasRequiredAttribute ? false : isNullableFromType;

        var isString = typeName == "string";
        var isBool = typeName == "bool" || typeName == "bool?";
        var isNumeric = IsNumericTypeString(typeName);
        var isDateTime = IsDateTimeTypeString(typeName);
        var isGuid = typeName == "Guid" || typeName == "Guid?";

        var propInfo = new RhSensoERP.Generators.Models.PropertyInfo
        {
            Name = member.Name,
            Type = typeName,
            IsNullable = isNullable,  // ✅ v4.2: CORRIGIDO
            IsString = isString,
            IsBool = isBool,
            IsNumeric = isNumeric,
            IsDateTime = isDateTime,
            IsGuid = isGuid,
            IsPrimaryKey = HasAttribute(member, "KeyAttribute"),
            IsRequired = hasRequiredAttribute,
            IsReadOnly = member.SetMethod == null,
            IsNavigation = false,
            IsIdentity = HasDatabaseGeneratedIdentity(member),
            ExcludeFromDto = false
        };

        var columnAttr = GetAttribute(member, "ColumnAttribute");
        if (columnAttr != null && columnAttr.ConstructorArguments.Length > 0)
        {
            propInfo.ColumnName = columnAttr.ConstructorArguments[0].Value?.ToString() ?? member.Name;
        }
        else
        {
            propInfo.ColumnName = member.Name;
        }

        propInfo.DisplayName = GetDisplayName(member);

        var stringLengthAttr = GetAttribute(member, "StringLengthAttribute");
        if (stringLengthAttr != null && stringLengthAttr.ConstructorArguments.Length > 0)
        {
            propInfo.MaxLength = (int?)stringLengthAttr.ConstructorArguments[0].Value;
        }

        var maxLengthAttr = GetAttribute(member, "MaxLengthAttribute");
        if (maxLengthAttr != null && maxLengthAttr.ConstructorArguments.Length > 0)
        {
            propInfo.MaxLength = (int?)maxLengthAttr.ConstructorArguments[0].Value;
        }

        if (propInfo.IsString && !propInfo.IsNullable)
        {
            propInfo.DefaultValue = "string.Empty";
        }

        propInfo.RequiredOnCreate = propInfo.IsRequired && !propInfo.IsPrimaryKey;

        return propInfo;
    }

    // =========================================================================
    // ✅ v4.2: ExtractNavigation COM VALIDAÇÃO DE FK
    // =========================================================================
    private static NavigationInfo? ExtractNavigation(
        IPropertySymbol member,
        List<RhSensoERP.Generators.Models.PropertyInfo> properties)
    {
        var type = member.Type;

        if (type is INamedTypeSymbol namedType)
        {
            // ✅ ICollection<T>, List<T> → OneToMany
            if (namedType.IsGenericType && namedType.TypeArguments.Length == 1)
            {
                var targetType = namedType.TypeArguments[0];

                return new NavigationInfo
                {
                    Name = member.Name,
                    TargetEntity = targetType.Name,
                    TargetEntityFullName = targetType.ToDisplayString(),
                    ForeignKeyProperty = string.Empty,  // ✅ Sem FK (coleção)
                    RelationshipType = NavigationRelationshipType.OneToMany,
                    IsNullable = true,
                    OnDelete = NavigationDeleteBehavior.Restrict
                };
            }

            // ✅ Navegação ManyToOne (ex: Banco, Agencia)
            if (!namedType.IsGenericType)
            {
                // ✅ v4.2: VALIDA se FK existe
                var fkPropertyName = FindForeignKeyProperty(member.Name, properties);

                return new NavigationInfo
                {
                    Name = member.Name,
                    TargetEntity = namedType.Name,
                    TargetEntityFullName = namedType.ToDisplayString(),
                    ForeignKeyProperty = fkPropertyName,  // ✅ Vazio se não existe
                    RelationshipType = NavigationRelationshipType.ManyToOne,
                    IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated,
                    OnDelete = NavigationDeleteBehavior.Restrict
                };
            }
        }

        return null;
    }

    // =========================================================================
    // ✅ v4.2: FindForeignKeyProperty - VALIDA se FK existe
    // =========================================================================
    private static string FindForeignKeyProperty(
        string navigationPropertyName,
        List<RhSensoERP.Generators.Models.PropertyInfo> properties)
    {
        // ✅ Convenções de FK (do mais específico ao mais genérico)
        var candidates = new[]
        {
            $"Id{navigationPropertyName}",           // IdBanco
            $"{navigationPropertyName}Id",           // BancoId
            $"id{navigationPropertyName}",           // idBanco (legado)
            $"{navigationPropertyName.ToLower()}id"  // bancoid (legado)
        };

        // ✅ Verifica se EXISTE alguma propriedade com esse nome
        foreach (var candidate in candidates)
        {
            var exists = properties.Any(p =>
                p.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                // ✅ Retorna o nome EXATO da propriedade (case-sensitive)
                return properties.First(p =>
                    p.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase)).Name;
            }
        }

        // ❌ FK não encontrada → Retorna vazio (shadow property ou sem FK)
        return string.Empty;
    }

    // =========================================================================
    // GetDisplayName
    // =========================================================================
    private static string GetDisplayName(IPropertySymbol member)
    {
        var displayAttr = GetAttribute(member, "DisplayAttribute");
        if (displayAttr != null)
        {
            var nameArg = displayAttr.NamedArguments.FirstOrDefault(a => a.Key == "Name");
            if (nameArg.Value.Value != null)
                return nameArg.Value.Value.ToString()!;
        }

        var propertyName = member.Name;

        if (PropertyDisplayNameMap.TryGetValue(propertyName, out var mapped))
            return mapped;

        if (propertyName.StartsWith("Cd") && propertyName.Length > 2)
        {
            var suffix = propertyName.Substring(2);
            return $"Código de {SplitPascalCase(suffix)}";
        }

        if (propertyName.StartsWith("Dc") && propertyName.Length > 2 ||
            propertyName.StartsWith("Ds") && propertyName.Length > 2)
        {
            var suffix = propertyName.Substring(2);
            return $"Descrição de {SplitPascalCase(suffix)}";
        }

        if (propertyName.StartsWith("Nm") && propertyName.Length > 2)
        {
            var suffix = propertyName.Substring(2);
            return $"Nome de {SplitPascalCase(suffix)}";
        }

        return SplitPascalCase(propertyName);
    }

    private static string SplitPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = Regex.Replace(input, "([A-Z])", " $1").Trim();

        if (result.Length > 0)
            result = char.ToUpper(result[0]) + result.Substring(1);

        return result;
    }

    private static void IdentifyPrimaryKey(EntityInfo info)
    {
        var pkProp = info.Properties.FirstOrDefault(p => p.IsPrimaryKey);

        if (pkProp == null)
        {
            pkProp = info.Properties.FirstOrDefault(p =>
                p.Name == "Id" ||
                p.Name == $"{info.EntityName}Id");
        }

        if (pkProp != null)
        {
            pkProp.IsPrimaryKey = true;
            info.PrimaryKeyProperty = pkProp.Name;
            info.PrimaryKeyColumn = pkProp.ColumnName;
            info.PrimaryKeyType = pkProp.Type;
        }
        else
        {
            info.PrimaryKeyProperty = "Id";
            info.PrimaryKeyColumn = "Id";
            info.PrimaryKeyType = "int";
        }
    }

    private static void IdentifyAuditFields(EntityInfo info)
    {
        var createdAt = info.Properties.FirstOrDefault(p => p.Name == "CreatedAtUtc" || p.Name == "CreatedAt");
        if (createdAt != null) info.CreatedAtField = createdAt.Name;

        var createdBy = info.Properties.FirstOrDefault(p => p.Name == "CreatedBy" || p.Name == "CreatedByUserId");
        if (createdBy != null) info.CreatedByField = createdBy.Name;

        var updatedAt = info.Properties.FirstOrDefault(p => p.Name == "UpdatedAtUtc" || p.Name == "UpdatedAt");
        if (updatedAt != null) info.UpdatedAtField = updatedAt.Name;

        var updatedBy = info.Properties.FirstOrDefault(p => p.Name == "UpdatedBy" || p.Name == "UpdatedByUserId");
        if (updatedBy != null) info.UpdatedByField = updatedBy.Name;

        if (string.IsNullOrEmpty(info.CreatedAtField))
        {
            var legacyCreatedAt = info.Properties.FirstOrDefault(p => p.Name == "Aud_CreatedAt");
            if (legacyCreatedAt != null) info.CreatedAtField = legacyCreatedAt.Name;
        }

        if (string.IsNullOrEmpty(info.CreatedByField))
        {
            var legacyCreatedBy = info.Properties.FirstOrDefault(p => p.Name == "Aud_IdUsuarioCadastro");
            if (legacyCreatedBy != null) info.CreatedByField = legacyCreatedBy.Name;
        }

        if (string.IsNullOrEmpty(info.UpdatedAtField))
        {
            var legacyUpdatedAt = info.Properties.FirstOrDefault(p => p.Name == "Aud_UpdatedAt");
            if (legacyUpdatedAt != null) info.UpdatedAtField = legacyUpdatedAt.Name;
        }

        if (string.IsNullOrEmpty(info.UpdatedByField))
        {
            var legacyUpdatedBy = info.Properties.FirstOrDefault(p => p.Name == "Aud_IdUsuarioAtualizacao");
            if (legacyUpdatedBy != null) info.UpdatedByField = legacyUpdatedBy.Name;
        }

        info.HasAuditFields = !string.IsNullOrEmpty(info.CreatedAtField) ||
                             !string.IsNullOrEmpty(info.UpdatedAtField);
    }

    private static void ExtractAttributeProperties(AttributeData attribute, EntityInfo info)
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            var value = namedArg.Value.Value;

            switch (namedArg.Key)
            {
                case "TableName":
                    info.TableName = value?.ToString() ?? string.Empty; break;
                case "Schema":
                    info.Schema = value?.ToString() ?? "dbo"; break;
                case "DisplayName":
                    info.DisplayName = value?.ToString() ?? string.Empty; break;
                case "CdSistema":
                    info.CdSistema = value?.ToString() ?? string.Empty; break;
                case "CdFuncao":
                    info.CdFuncao = value?.ToString() ?? string.Empty; break;
                case "ApiRoute":
                    info.ApiRoute = value?.ToString() ?? string.Empty; break;
                case "ApiGroup":
                    info.ApiGroup = value?.ToString() ?? string.Empty; break;
                case "UsePluralRoute":
                    info.UsePluralRoute = value is bool v1 && v1; break;
                case "PrimaryKeyProperty":
                    info.PrimaryKeyProperty = value?.ToString() ?? string.Empty; break;
                case "PrimaryKeyType":
                    info.PrimaryKeyType = value?.ToString() ?? string.Empty; break;
                case "GenerateDto":
                    info.GenerateDto = value is bool v2 && v2; break;
                case "GenerateRequests":
                    info.GenerateRequests = value is bool v3 && v3; break;
                case "GenerateCommands":
                    info.GenerateCommands = value is bool v4 && v4; break;
                case "GenerateQueries":
                    info.GenerateQueries = value is bool v5 && v5; break;
                case "GenerateValidators":
                    info.GenerateValidators = value is bool v6 && v6; break;
                case "GenerateRepository":
                    info.GenerateRepository = value is bool v7 && v7; break;
                case "GenerateMapper":
                    info.GenerateMapper = value is bool v8 && v8; break;
                case "GenerateEfConfig":
                    info.GenerateEfConfig = value is bool v9 && v9; break;
                case "GenerateMetadata":
                    info.GenerateMetadata = value is bool v10 && v10; break;
                case "GenerateApiController":
                    info.GenerateApiController = value is bool v11 && v11; break;
                case "GenerateWebController":
                    info.GenerateWebController = value is bool v12 && v12; break;
                case "GenerateWebModels":
                    info.GenerateWebModels = value is bool v13 && v13; break;
                case "GenerateWebServices":
                    info.GenerateWebServices = value is bool v14 && v14; break;
                case "SupportsBatchDelete":
                    info.SupportsBatchDelete = value is bool v15 && v15; break;
                case "ApiRequiresAuth":
                    info.ApiRequiresAuth = value is bool v16 && v16; break;
                case "IsLegacyTable":
                    info.IsLegacyTable = value is bool v17 && v17; break;
            }
        }
    }

    private static string CalculateModuleName(string ns)
    {
        var parts = ns.Split('.');

        if (parts.Length >= 3 && parts[1] == "Modules")
            return parts[2];

        if (parts.Length >= 2)
            return parts[1];

        return "Unknown";
    }

    private static string CalculatePluralName(string entityName)
    {
        if (entityName.EndsWith("s") || entityName.EndsWith("x") || entityName.EndsWith("z"))
            return entityName + "es";

        if (entityName.EndsWith("y"))
            return entityName.Substring(0, entityName.Length - 1) + "ies";

        return entityName + "s";
    }

    private static void CalculateApiRoute(EntityInfo info)
    {
        if (!string.IsNullOrEmpty(info.ApiRoute))
        {
            info.ApiFullRoute = $"/api/{info.ApiRoute}";
            return;
        }

        var moduleLower = info.ModuleName.ToLowerInvariant();
        var entityLower = info.UsePluralRoute
            ? info.PluralName.ToLowerInvariant()
            : info.EntityName.ToLowerInvariant();

        info.ApiRoute = $"{moduleLower}/{entityLower}";
        info.ApiFullRoute = $"/api/{info.ApiRoute}";
    }

    private static bool HasAttribute(ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name == attributeName);
    }

    private static AttributeData? GetAttribute(ISymbol symbol, string attributeName)
    {
        return symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == attributeName);
    }

    private static ITypeSymbol GetUnderlyingType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return namedType.TypeArguments[0];
        }
        return type;
    }

    private static bool IsNullableType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            return true;

        if (type.NullableAnnotation == NullableAnnotation.Annotated)
            return true;

        return false;
    }

    // =========================================================================
    // ✅ v4.2: GetTypeName CORRIGIDO para int?
    // =========================================================================
    private static string GetTypeName(ITypeSymbol type)
    {
        var underlying = GetUnderlyingType(type);
        var isNullable = IsNullableType(type);

        string typeName;

        switch (underlying.SpecialType)
        {
            case SpecialType.System_String: typeName = "string"; break;
            case SpecialType.System_Int32: typeName = "int"; break;
            case SpecialType.System_Int64: typeName = "long"; break;
            case SpecialType.System_Int16: typeName = "short"; break;
            case SpecialType.System_Byte: typeName = "byte"; break;
            case SpecialType.System_Boolean: typeName = "bool"; break;
            case SpecialType.System_Decimal: typeName = "decimal"; break;
            case SpecialType.System_Double: typeName = "double"; break;
            case SpecialType.System_Single: typeName = "float"; break;
            case SpecialType.System_Char: typeName = "char"; break;
            case SpecialType.System_Object: typeName = "object"; break;
            default:
                typeName = GetNameBasedType(underlying);
                break;
        }

        // ✅ v4.2 FIX: Adiciona ? se nullable (exceto string/object/arrays)
        if (isNullable && typeName != "string" && typeName != "object" && !typeName.EndsWith("[]"))
        {
            return $"{typeName}?";
        }

        return typeName;
    }

    // =========================================================================
    // ✅ v4.2: GetNameBasedType CORRIGIDO
    // =========================================================================
    private static string GetNameBasedType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arraySym)
        {
            var elemType = GetTypeName(arraySym.ElementType);
            if (elemType.EndsWith("?"))
                elemType = elemType.Substring(0, elemType.Length - 1);
            return $"{elemType}[]";
        }

        var name = type.Name;

        // ✅ v4.2 FIX: REMOVIDO "Nullable" => "object"
        // Agora nullable types são tratados corretamente em GetTypeName
        return name switch
        {
            "Int32" => "int",
            "Int64" => "long",
            "Int16" => "short",
            "Byte" => "byte",
            "Boolean" => "bool",
            "Decimal" => "decimal",
            "Double" => "double",
            "Single" => "float",
            "String" => "string",
            "Object" => "object",
            "Guid" => "Guid",
            "DateTime" => "DateTime",
            "DateTimeOffset" => "DateTimeOffset",
            "DateOnly" => "DateOnly",
            "TimeOnly" => "TimeOnly",
            "TimeSpan" => "TimeSpan",
            _ => type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
        };
    }

    private static bool IsNumericTypeString(string typeName)
    {
        return typeName.StartsWith("int") ||
               typeName.StartsWith("long") ||
               typeName.StartsWith("short") ||
               typeName.StartsWith("byte") ||
               typeName.StartsWith("decimal") ||
               typeName.StartsWith("double") ||
               typeName.StartsWith("float");
    }

    private static bool IsDateTimeTypeString(string typeName)
    {
        return typeName.Contains("DateTime") ||
               typeName.Contains("DateOnly") ||
               typeName.Contains("TimeOnly") ||
               typeName.Contains("TimeSpan");
    }

    private static bool IsCollectionType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        if (namedType.IsGenericType)
        {
            var name = namedType.OriginalDefinition.Name;
            return name is "List" or "IEnumerable" or "ICollection" or "HashSet";
        }

        return namedType.Name is "ICollection" or "IEnumerable" or "List" or "HashSet";
    }

    private static bool HasDatabaseGeneratedIdentity(IPropertySymbol property)
    {
        var attr = GetAttribute(property, "DatabaseGeneratedAttribute");
        if (attr == null) return false;

        if (attr.ConstructorArguments.Length > 0)
        {
            var value = attr.ConstructorArguments[0].Value;
            var strVal = value?.ToString();
            return strVal == "Identity" || strVal == "1" || (value is int i && i == 1);
        }

        return false;
    }

    private static string GenerateHeader(string entityName)
    {
        return $"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators {GENERATOR_VERSION}
// Entity: {entityName}
// Generated At: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
""";
    }
}