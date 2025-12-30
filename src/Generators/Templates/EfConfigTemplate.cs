// =============================================================================
// RHSENSOERP GENERATOR v4.2 - EF CONFIG TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RhSensoERP.Generators.Templates;

public static class EfConfigTemplate
{
    public static string GenerateConfig(EntityInfo info)
    {
        var entityNs = info.Namespace;
        var propertyConfigs = GeneratePropertyConfigurations(info);
        var uniqueIndexes = GenerateUniqueIndexes(info);
        var relationshipConfigs = GenerateRelationshipConfigurations(info);
        var additionalUsings = GenerateAdditionalUsings(info);

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using {{entityNs}};
{{additionalUsings}}

namespace {{info.EfConfigNamespace}};

/// <summary>
/// Configuração do Entity Framework para {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Configuration : IEntityTypeConfiguration<{{info.EntityName}}>
{
    public void Configure(EntityTypeBuilder<{{info.EntityName}}> builder)
    {
        // =====================================================================
        // Tabela
        // =====================================================================
        builder.ToTable("{{info.TableName}}", "{{info.Schema}}");

        // =====================================================================
        // Chave primária
        // =====================================================================
        builder.HasKey(e => e.{{info.PrimaryKeyProperty}});

        // =====================================================================
        // Propriedades
        // =====================================================================
{{propertyConfigs}}

        // =====================================================================
        // Índices Únicos
        // =====================================================================
{{uniqueIndexes}}

        // =====================================================================
        // Relacionamentos
        // =====================================================================
{{relationshipConfigs}}
    }
}
""";
    }

    private static string GeneratePropertyConfigurations(EntityInfo info)
    {
        var configs = new List<string>();

        foreach (var prop in info.Properties)
        {
            var propConfig = new List<string>();

            if (!string.IsNullOrEmpty(prop.ColumnName) &&
                !prop.ColumnName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))
            {
                propConfig.Add($".HasColumnName(\"{prop.ColumnName}\")");
            }

            if (prop.IsRequired && !prop.IsNullable)
            {
                propConfig.Add(".IsRequired()");
            }

            if (prop.IsString && prop.MaxLength.HasValue)
            {
                propConfig.Add($".HasMaxLength({prop.MaxLength.Value})");
            }

            if (propConfig.Count > 0)
            {
                var config = $"        builder.Property(e => e.{prop.Name})\n            {string.Join("\n            ", propConfig)};";
                configs.Add(config);
            }
        }

        return configs.Count > 0
            ? string.Join("\n\n", configs)
            : "        // Configurações de propriedades usam convenções padrão";
    }

    // =========================================================================
    // ✅ CORRIGIDO: GenerateUniqueIndexes (sem chaves duplas problemáticas)
    // =========================================================================
    private static string GenerateUniqueIndexes(EntityInfo info)
    {
        var uniqueProps = info.Properties
            .Where(p => p.IsUnique)
            .ToList();

        if (uniqueProps.Count == 0)
        {
            return "        // Sem índices únicos configurados";
        }

        var configs = new List<string>();

        foreach (var prop in uniqueProps)
        {
            var columnName = string.IsNullOrEmpty(prop.ColumnName) ? prop.Name : prop.ColumnName;
            var indexName = $"UX_{info.EntityName}_{prop.Name}";
            var filterClause = prop.UniqueAllowNull
                ? $"\n            .HasFilter(\"{columnName} IS NOT NULL\")"
                : "";

            // ✅ CORRIGIDO: Construção da string sem problemas de raw literal
            if (prop.UniqueScope == "Tenant" && info.Properties.Any(p => p.Name == "TenantId"))
            {
                indexName = $"UX_{info.EntityName}_Tenant_{prop.Name}";

                // ✅ Usa interpolação simples ao invés de raw literal aninhado
                var indexDef = $"builder.HasIndex(e => new {{ e.TenantId, e.{prop.Name} }}, \"{indexName}\")";

                var config = $"""
        // Índice único: {prop.UniqueDisplayName} (por tenant)
        {indexDef}
            .IsUnique(){filterClause};
""";
                configs.Add(config);
            }
            else
            {
                var config = $"""
        // Índice único: {prop.UniqueDisplayName} (global)
        builder.HasIndex(e => e.{prop.Name}, "{indexName}")
            .IsUnique(){filterClause};
""";
                configs.Add(config);
            }
        }

        return string.Join("\n\n", configs);
    }

    private static string GenerateRelationshipConfigurations(EntityInfo info)
    {
        if (!info.HasNavigations)
        {
            return "        // Sem relacionamentos configurados";
        }

        var configs = new List<string>();

        foreach (var nav in info.ManyToOneNavigations)
        {
            var deleteBehavior = nav.OnDelete switch
            {
                NavigationDeleteBehavior.Cascade => "DeleteBehavior.Cascade",
                NavigationDeleteBehavior.SetNull => "DeleteBehavior.SetNull",
                NavigationDeleteBehavior.NoAction => "DeleteBehavior.NoAction",
                _ => "DeleteBehavior.Restrict"
            };

            var withMany = string.IsNullOrEmpty(nav.InverseProperty)
                ? ".WithMany()"
                : $".WithMany(e => e.{nav.InverseProperty})";

            string config;

            if (!string.IsNullOrEmpty(nav.ForeignKeyProperty))
            {
                config = $"""
        // Relacionamento: {info.EntityName} -> {nav.TargetEntity}
        builder.HasOne(e => e.{nav.Name})
            {withMany}
            .HasForeignKey(e => e.{nav.ForeignKeyProperty})
            .OnDelete({deleteBehavior});
""";
            }
            else
            {
                config = $"""
        // Relacionamento: {info.EntityName} -> {nav.TargetEntity}
        // ⚠️ ATENÇÃO: FK não encontrada na entidade.
        // Se usar shadow property, configure manualmente no DbContext.
        // Caso contrário, adicione a propriedade FK na entidade.
        builder.Ignore(e => e.{nav.Name});
""";
            }

            configs.Add(config);
        }

        foreach (var nav in info.OneToManyNavigations)
        {
            var config = $"""
        // Ignorar navegação sem FK configurada: {nav.Name}
        builder.Ignore(e => e.{nav.Name});
""";
            configs.Add(config);
        }

        return configs.Count > 0
            ? string.Join("\n\n", configs)
            : "        // Relacionamentos configurados via convenção";
    }

    private static string GenerateAdditionalUsings(EntityInfo info)
    {
        if (!info.HasNavigations)
            return "";

        var namespaces = new HashSet<string>();

        foreach (var nav in info.Navigations)
        {
            if (string.IsNullOrEmpty(nav.TargetEntityFullName))
                continue;

            var lastDot = nav.TargetEntityFullName.LastIndexOf('.');
            if (lastDot > 0)
            {
                var ns = nav.TargetEntityFullName.Substring(0, lastDot);
                if (!string.IsNullOrEmpty(ns) && ns != info.Namespace)
                {
                    namespaces.Add(ns);
                }
            }
        }

        if (namespaces.Count == 0)
            return "";

        var usings = namespaces
            .OrderBy(ns => ns)
            .Select(ns => $"using {ns};");

        return string.Join("\n", usings);
    }
}