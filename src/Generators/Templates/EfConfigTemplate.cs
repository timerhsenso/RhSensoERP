// =============================================================================
// RHSENSOERP GENERATOR v3.3 - EF CONFIG TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/EfConfigTemplate.cs
// Versão: 3.3 - Suporte a navegações/relacionamentos
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Entity Framework Configuration.
/// </summary>
public static class EfConfigTemplate
{
    /// <summary>
    /// Gera a Configuration do Entity Framework.
    /// </summary>
    public static string GenerateConfig(EntityInfo info)
    {
        var entityNs = info.Namespace;
        var propertyConfigs = GeneratePropertyConfigurations(info);
        var relationshipConfigs = GenerateRelationshipConfigurations(info);
        var additionalUsings = GenerateAdditionalUsings(info);

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.3
// Entity: {{info.EntityName}}
// =============================================================================
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
        // Relacionamentos
        // =====================================================================
{{relationshipConfigs}}
    }
}
""";
    }

    /// <summary>
    /// Gera as configurações de propriedades.
    /// </summary>
    private static string GeneratePropertyConfigurations(EntityInfo info)
    {
        var configs = new List<string>();

        foreach (var prop in info.Properties)
        {
            var propConfig = new List<string>();

            // Nome da coluna (se diferente)
            if (!string.IsNullOrEmpty(prop.ColumnName) &&
                !prop.ColumnName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))
            {
                propConfig.Add($".HasColumnName(\"{prop.ColumnName}\")");
            }

            // Required
            if (prop.IsRequired && !prop.IsNullable)
            {
                propConfig.Add(".IsRequired()");
            }

            // MaxLength para strings
            if (prop.IsString && prop.MaxLength.HasValue)
            {
                propConfig.Add($".HasMaxLength({prop.MaxLength.Value})");
            }

            // Só adiciona se tem configuração
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

    /// <summary>
    /// Gera as configurações de relacionamentos.
    /// </summary>
    private static string GenerateRelationshipConfigurations(EntityInfo info)
    {
        if (!info.HasNavigations)
        {
            return "        // Sem relacionamentos configurados";
        }

        var configs = new List<string>();

        // =====================================================================
        // ManyToOne (HasOne) - Ex: Agencia -> Banco
        // =====================================================================
        foreach (var nav in info.ManyToOneNavigations)
        {
            var deleteBehavior = nav.OnDelete switch
            {
                NavigationDeleteBehavior.Cascade => "DeleteBehavior.Cascade",
                NavigationDeleteBehavior.SetNull => "DeleteBehavior.SetNull",
                NavigationDeleteBehavior.NoAction => "DeleteBehavior.NoAction",
                _ => "DeleteBehavior.Restrict"
            };

            // Se tem propriedade inversa, usa WithMany(e => e.Propriedade)
            // Senão, usa WithMany() sem parâmetros
            var withMany = string.IsNullOrEmpty(nav.InverseProperty)
                ? ".WithMany()"
                : $".WithMany(e => e.{nav.InverseProperty})";

            var config = $"""
        // Relacionamento: {info.EntityName} -> {nav.TargetEntity}
        builder.HasOne(e => e.{nav.Name})
            {withMany}
            .HasForeignKey(e => e.{nav.ForeignKeyProperty})
            .OnDelete({deleteBehavior});
""";
            configs.Add(config);
        }

        // =====================================================================
        // OneToMany / Navegações sem FK - Gera Ignore para evitar ambiguidade
        // =====================================================================
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

    /// <summary>
    /// Gera usings adicionais para entidades relacionadas.
    /// </summary>
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
