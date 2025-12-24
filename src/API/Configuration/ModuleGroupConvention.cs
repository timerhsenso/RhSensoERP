// ============================================================================
// src/API/Configuration/ModuleGroupConvention.cs
// ============================================================================
// Convenção que atribui automaticamente o GroupName aos controllers baseado
// no namespace. Isso permite que o Swagger agrupe endpoints por módulo.
//
// ✅ SUPORTA:
// - Controllers tradicionais em RhSensoERP.Modules.{Modulo}.API.Controllers
// - Controllers gerados via SourceGenerator
// - Controllers em RhSensoERP.Identity.*
// - Controllers em RhSensoERP.API.Controllers.*
// ============================================================================
#nullable enable
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Define o ApiExplorer.GroupName automaticamente a partir do namespace do controller.
/// Não sobrescreve quando o controller já definiu via atributo [ApiExplorerSettings].
/// 
/// Hierarquia de detecção:
/// 1. Controllers em RhSensoERP.Modules.* → Nome do módulo
/// 2. Controllers em RhSensoERP.Identity.* → "Identity"
/// 3. Controllers em RhSensoERP.API.Controllers.* → Nome da pasta/segmento
/// 4. Controllers em RhSensoERP.API.Controllers → "Diagnostics" (fallback)
/// </summary>
public sealed class ModuleGroupConvention : IControllerModelConvention
{
    private static bool _loggedOnce = false;

    public void Apply(ControllerModel controller)
    {
        var ns = controller.ControllerType.Namespace ?? string.Empty;
        var controllerFullName = controller.ControllerType.FullName ?? controller.ControllerName;

        // ✅ SEMPRE sobrescreve o GroupName para garantir consistência no Swagger
        // Isso é necessário porque o SourceGenerator pode gerar GroupNames diferentes
        // (ex: "FRE", "RHU") que não correspondem aos documentos do Swagger

        // Atribui o GroupName baseado no namespace
        var group = ResolveGroupName(ns, controllerFullName);

        if (group is not null)
        {
            controller.ApiExplorer.GroupName = group;

            // Log apenas uma vez no startup para não poluir
            if (!_loggedOnce)
            {
                Log.Information("📌 ModuleGroupConvention ativa - agrupando controllers por módulo");
                _loggedOnce = true;
            }

            Log.Debug("📌 {Controller} ({Namespace}) → GroupName: {GroupName}",
                controller.ControllerName, ns, group);
        }
        else
        {
            Log.Warning("⚠️ {Controller} ({Namespace}) → GroupName não resolvido, usando 'API'",
                controller.ControllerName, ns);
            controller.ApiExplorer.GroupName = "API";
        }
    }

    private static string? ResolveGroupName(string namespaceName, string fullName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            return null;

        // Helper para verificar se contém um token (case-insensitive)
        static bool Contains(string source, string token) =>
            source.Contains(token, StringComparison.OrdinalIgnoreCase);

        // ===== PRIORIDADE 1: Módulos em RhSensoERP.Modules.* =====
        // Suporta tanto controllers tradicionais quanto gerados por SourceGenerator
        if (Contains(namespaceName, ".Modules.") || Contains(fullName, ".Modules."))
        {
            var sourceToCheck = Contains(namespaceName, ".Modules.") ? namespaceName : fullName;

            // Lista de módulos conhecidos (ordem importa para evitar conflitos)
            var knownModules = new[]
            {
                "GestaoDePessoas",
                "ControleDePonto",
                "Avaliacoes",
                "Esocial",
                "SaudeOcupacional",
                "Treinamentos",
                "AuditoriaCompliance",
                "ControleAcessoPortaria",
                "GestaoDeTerceiros",
                "GestaoDeEPI",
                "Integracoes"
            };

            foreach (var module in knownModules)
            {
                if (Contains(sourceToCheck, module))
                    return module;
            }

            // Fallback: extrai o nome do módulo do namespace
            return ExtractModuleNameFromNamespace(sourceToCheck);
        }

        // ===== PRIORIDADE 2: Identity Module =====
        if (Contains(namespaceName, ".Identity.") || Contains(namespaceName, ".Identity") ||
            Contains(fullName, ".Identity.") || Contains(fullName, ".Identity"))
        {
            return "Identity";
        }

        // ===== PRIORIDADE 3: Controllers em RhSensoERP.API.Controllers.* =====
        if (Contains(namespaceName, ".API.Controllers."))
        {
            var parts = namespaceName.Split('.');
            var controllersIndex = Array.FindIndex(parts, p =>
                p.Equals("Controllers", StringComparison.OrdinalIgnoreCase));

            if (controllersIndex >= 0 && controllersIndex < parts.Length - 1)
            {
                var segment = parts[controllersIndex + 1];

                // Mapeia segmentos conhecidos
                return segment switch
                {
                    "Identity" => "Identity",
                    "Diagnostics" => "Diagnostics",
                    "GestaoDePessoas" => "GestaoDePessoas",
                    "ControleDePonto" => "ControleDePonto",
                    "Avaliacoes" => "Avaliacoes",
                    "Esocial" => "Esocial",
                    "SaudeOcupacional" => "SaudeOcupacional",
                    "Treinamentos" => "Treinamentos",
                    "Tabelas" => "GestaoDePessoas", // Subpasta comum
                    _ => segment
                };
            }
        }

        // ===== PRIORIDADE 4: Controllers diretos em RhSensoERP.API.Controllers =====
        if (namespaceName.EndsWith(".API.Controllers", StringComparison.OrdinalIgnoreCase) ||
            namespaceName.Equals("RhSensoERP.API.Controllers", StringComparison.OrdinalIgnoreCase))
        {
            // Verifica se o nome do controller indica o módulo
            if (Contains(fullName, "Diagnostic") || Contains(fullName, "Health"))
                return "Diagnostics";

            return "Diagnostics";
        }

        // ===== FALLBACK =====
        return null;
    }

    /// <summary>
    /// Extrai o nome do módulo do namespace.
    /// Ex: "RhSensoERP.Modules.GestaoDePessoas.API.Controllers" → "GestaoDePessoas"
    /// </summary>
    private static string? ExtractModuleNameFromNamespace(string ns)
    {
        var parts = ns.Split('.');
        var modulesIndex = Array.FindIndex(parts, p =>
            p.Equals("Modules", StringComparison.OrdinalIgnoreCase));

        if (modulesIndex >= 0 && modulesIndex + 1 < parts.Length)
        {
            return parts[modulesIndex + 1];
        }

        return null;
    }
}
