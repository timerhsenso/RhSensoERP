// =============================================================================
// RHSENSOERP GENERATOR v3.1 - CRUD SOURCE GENERATOR
// =============================================================================
// Arquivo: src/Generators/Generators/CrudGenerator.cs
// Versão: 3.1 - Com suporte a MetadataProvider para UI dinâmica
// =============================================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhSensoERP.Generators.Extractors;
using RhSensoERP.Generators.Models;
using RhSensoERP.Generators.Templates;
using System.Linq;

namespace RhSensoERP.Generators.Generators;

/// <summary>
/// Source Generator que processa Entities marcadas com [GenerateCrud]
/// e gera todos os arquivos necessários para CRUD completo.
/// </summary>
[Generator]
public class CrudGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Pipeline - Filtra classes com o atributo [GenerateCrud]
        var entityProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateClass(node),
                transform: static (ctx, _) => EntityInfoExtractor.Extract(ctx))
            .Where(static info => info != null);

        // Registra a geração de código
        context.RegisterSourceOutput(entityProvider, static (ctx, info) =>
        {
            if (info == null) return;
            GenerateAllFiles(ctx, info);
        });
    }

    /// <summary>
    /// Verifica se o nó é uma classe candidata (tem atributo que pode ser GenerateCrud).
    /// </summary>
    private static bool IsCandidateClass(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration &&
               classDeclaration.AttributeLists.Count > 0;
    }

    /// <summary>
    /// Gera todos os arquivos para uma Entity.
    /// </summary>
    private static void GenerateAllFiles(SourceProductionContext context, EntityInfo info)
    {
        // =====================================================================
        // BACKEND - DTOs
        // =====================================================================
        if (info.GenerateDto)
        {
            var dtoCode = DtoTemplate.GenerateDto(info);
            context.AddSource($"{info.EntityName}Dto.g.cs", dtoCode);
        }

        if (info.GenerateRequests)
        {
            var createRequestCode = DtoTemplate.GenerateCreateRequest(info);
            context.AddSource($"Create{info.EntityName}Request.g.cs", createRequestCode);

            var updateRequestCode = DtoTemplate.GenerateUpdateRequest(info);
            context.AddSource($"Update{info.EntityName}Request.g.cs", updateRequestCode);
        }

        // =====================================================================
        // BACKEND - Commands
        // =====================================================================
        if (info.GenerateCommands)
        {
            var createCommandCode = CommandsTemplate.GenerateCreateCommand(info);
            context.AddSource($"Create{info.EntityName}Command.g.cs", createCommandCode);

            var updateCommandCode = CommandsTemplate.GenerateUpdateCommand(info);
            context.AddSource($"Update{info.EntityName}Command.g.cs", updateCommandCode);

            var deleteCommandCode = CommandsTemplate.GenerateDeleteCommand(info);
            context.AddSource($"Delete{info.EntityName}Command.g.cs", deleteCommandCode);

            if (info.SupportsBatchDelete)
            {
                var batchDeleteCode = CommandsTemplate.GenerateBatchDeleteCommand(info);
                context.AddSource($"Delete{info.PluralName}Command.g.cs", batchDeleteCode);
            }
        }

        // =====================================================================
        // BACKEND - Queries
        // =====================================================================
        if (info.GenerateQueries)
        {
            var getByIdQueryCode = QueriesTemplate.GenerateGetByIdQuery(info);
            context.AddSource($"GetBy{info.EntityName}IdQuery.g.cs", getByIdQueryCode);

            var getPagedQueryCode = QueriesTemplate.GenerateGetPagedQuery(info);
            context.AddSource($"Get{info.PluralName}PagedQuery.g.cs", getPagedQueryCode);
        }

        // =====================================================================
        // BACKEND - Validators
        // =====================================================================
        if (info.GenerateValidators)
        {
            var createValidatorCode = ValidatorsTemplate.GenerateCreateValidator(info);
            context.AddSource($"Create{info.EntityName}RequestValidator.g.cs", createValidatorCode);

            var updateValidatorCode = ValidatorsTemplate.GenerateUpdateValidator(info);
            context.AddSource($"Update{info.EntityName}RequestValidator.g.cs", updateValidatorCode);
        }

        // =====================================================================
        // BACKEND - Repository
        // =====================================================================
        if (info.GenerateRepository)
        {
            var repoInterfaceCode = RepositoryTemplate.GenerateInterface(info);
            context.AddSource($"I{info.EntityName}Repository.g.cs", repoInterfaceCode);

            var repoImplCode = RepositoryTemplate.GenerateImplementation(info);
            context.AddSource($"{info.EntityName}Repository.g.cs", repoImplCode);
        }

        // =====================================================================
        // BACKEND - AutoMapper Profile
        // =====================================================================
        if (info.GenerateMapper)
        {
            var mapperCode = MapperTemplate.GenerateProfile(info);
            context.AddSource($"{info.EntityName}Profile.g.cs", mapperCode);
        }

        // =====================================================================
        // BACKEND - EF Configuration
        // =====================================================================
        if (info.GenerateEfConfig)
        {
            var efConfigCode = EfConfigTemplate.GenerateConfig(info);
            context.AddSource($"{info.EntityName}Configuration.g.cs", efConfigCode);
        }

        // =====================================================================
        // BACKEND - MetadataProvider (NOVO v3.1)
        // =====================================================================
        if (info.GenerateMetadata)
        {
            var metadataCode = MetadataTemplate.GenerateMetadataProvider(info);
            context.AddSource($"{info.EntityName}MetadataProvider.g.cs", metadataCode);
        }

        // =====================================================================
        // API - Controller
        // =====================================================================
        if (info.GenerateApiController)
        {
            var apiControllerCode = ApiControllerTemplate.GenerateController(info);
            context.AddSource($"{info.PluralName}Controller.Api.g.cs", apiControllerCode);
        }

        // =====================================================================
        // WEB - Controller
        // =====================================================================
        if (info.GenerateWebController)
        {
            var webControllerCode = WebControllerTemplate.GenerateController(info);
            context.AddSource($"{info.PluralName}Controller.Web.g.cs", webControllerCode);
        }

        // =====================================================================
        // WEB - Models
        // =====================================================================
        if (info.GenerateWebModels)
        {
            var webDtoCode = WebModelsTemplate.GenerateDto(info);
            context.AddSource($"{info.EntityName}Dto.Web.g.cs", webDtoCode);

            var webCreateDtoCode = WebModelsTemplate.GenerateCreateDto(info);
            context.AddSource($"Create{info.EntityName}Dto.Web.g.cs", webCreateDtoCode);

            var webUpdateDtoCode = WebModelsTemplate.GenerateUpdateDto(info);
            context.AddSource($"Update{info.EntityName}Dto.Web.g.cs", webUpdateDtoCode);

            var webViewModelCode = WebModelsTemplate.GenerateListViewModel(info);
            context.AddSource($"{info.PluralName}ListViewModel.g.cs", webViewModelCode);
        }

        // =====================================================================
        // WEB - Services
        // =====================================================================
        if (info.GenerateWebServices)
        {
            var serviceInterfaceCode = WebServicesTemplate.GenerateInterface(info);
            context.AddSource($"I{info.EntityName}ApiService.g.cs", serviceInterfaceCode);

            var serviceImplCode = WebServicesTemplate.GenerateImplementation(info);
            context.AddSource($"{info.EntityName}ApiService.g.cs", serviceImplCode);
        }
    }
}