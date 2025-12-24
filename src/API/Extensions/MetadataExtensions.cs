// =============================================================================
// RHSENSOERP - METADATA EXTENSIONS
// =============================================================================
// Arquivo: src/RhSensoERP.API/Extensions/MetadataExtensions.cs
// Descrição: Extensões para registro de metadados no DI container
// =============================================================================

using RhSensoERP.Shared.Application.Metadata;

namespace RhSensoERP.API.Extensions;

/// <summary>
/// Extensões para configuração do sistema de metadados.
/// </summary>
public static class MetadataExtensions
{
    /// <summary>
    /// Adiciona o sistema de metadados ao container de DI.
    /// </summary>
    public static IServiceCollection AddEntityMetadata(this IServiceCollection services)
    {
        // Registra o MetadataRegistry como Singleton
        services.AddSingleton<IMetadataRegistry>(sp =>
        {
            var registry = new MetadataRegistry();

            // Registra todos os MetadataProviders gerados
            RegisterAllMetadata(registry);

            return registry;
        });

        return services;
    }

    /// <summary>
    /// Registra todos os MetadataProviders gerados pelo Source Generator.
    /// </summary>
    private static void RegisterAllMetadata(MetadataRegistry registry)
    {
        // =====================================================================
        // IDENTITY MODULE
        // =====================================================================
        // TODO: Descomentar quando o módulo Identity tiver entidades com [GenerateCrud]
       /////////  registry.Register(RhSensoERP.Identity.Application.Metadata.SistemaMetadataProvider.GetMetadata());
        // registry.Register(RhSensoERP.Identity.Application.Metadata.FuncaoMetadataProvider.GetMetadata());

        // =====================================================================
        // GESTÃO DE PESSOAS MODULE
        // =====================================================================
      //  registry.Register(RhSensoERP.Modules.GestaoDePessoas.Application.Metadata.BancoMetadataProvider.GetMetadata());

        // TODO: Adicionar outras entidades conforme forem criadas
        // registry.Register(RhSensoERP.Modules.GestaoDePessoas.Application.Metadata.AgenciaMetadataProvider.GetMetadata());
        // registry.Register(RhSensoERP.Modules.GestaoDePessoas.Application.Metadata.FuncionarioMetadataProvider.GetMetadata());

        // =====================================================================
        // CONTROLE DE PONTO MODULE
        // =====================================================================
        // TODO: Descomentar quando o módulo tiver entidades com [GenerateCrud]
        // registry.Register(RhSensoERP.Modules.ControleDePonto.Application.Metadata.XxxMetadataProvider.GetMetadata());

        // =====================================================================
        // OUTROS MÓDULOS
        // =====================================================================
        // Adicionar conforme novos módulos forem implementados
    }
}