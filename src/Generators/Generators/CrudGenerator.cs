// =============================================================================
// RHSENSOERP GENERATOR v3.2 - CRUD SOURCE GENERATOR
// =============================================================================
// Arquivo: src/Generators/Generators/CrudGenerator.cs
// Versão: 3.2 - FIX: Proteção contra ArgumentNullException + Diagnósticos
//
// ✅ CORREÇÕES v3.2:
// 1. SafeAddSource() com null-check antes de AddSource
// 2. Try-catch por entidade (uma entidade com erro NÃO aborta as outras)
// 3. ReportDiagnostic para identificar qual template falhou
// 4. Mantém toda funcionalidade da v3.1
// =============================================================================
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhSensoERP.Generators.Extractors;
using RhSensoERP.Generators.Models;
using RhSensoERP.Generators.Templates;
using System;
using System.Linq;

namespace RhSensoERP.Generators.Generators;

/// <summary>
/// Source Generator que processa Entities marcadas com [GenerateCrud]
/// e gera todos os arquivos necessários para CRUD completo.
/// </summary>
[Generator]
public class CrudGenerator : IIncrementalGenerator
{
    // =========================================================================
    // ✅ v3.2: Descriptors para diagnóstico de erros no Generator
    // =========================================================================
    private static readonly DiagnosticDescriptor TemplateNullWarning = new(
        id: "RHGEN001",
        title: "Template retornou null",
        messageFormat: "O template '{0}' retornou null para a entidade '{1}'. O arquivo '{2}' não será gerado.",
        category: "RhSensoERP.Generators",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor GeneratorExceptionError = new(
        id: "RHGEN002",
        title: "Exceção no Generator",
        messageFormat: "Exceção ao gerar código para '{0}': {1}",
        category: "RhSensoERP.Generators",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

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

            // ✅ v3.2: Try-catch por entidade para não abortar o Generator inteiro
            try
            {
                GenerateAllFiles(ctx, info);
            }
            catch (Exception ex)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorExceptionError,
                    Location.None,
                    info.EntityName,
                    $"{ex.GetType().Name}: {ex.Message}"));
            }
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

    // =========================================================================
    // ✅ v3.2: Helper seguro - NUNCA deixa null chegar ao AddSource
    // =========================================================================
    private static void SafeAddSource(
        SourceProductionContext context,
        string hintName,
        string? sourceCode,
        string templateName,
        string entityName)
    {
        if (string.IsNullOrWhiteSpace(sourceCode))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                TemplateNullWarning,
                Location.None,
                templateName,
                entityName,
                hintName));
            return;
        }

        context.AddSource(hintName, sourceCode);
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
            SafeAddSource(context, $"{info.EntityName}Dto.g.cs", dtoCode,
                "DtoTemplate.GenerateDto", info.EntityName);
        }

        if (info.GenerateRequests)
        {
            var createRequestCode = DtoTemplate.GenerateCreateRequest(info);
            SafeAddSource(context, $"Create{info.EntityName}Request.g.cs", createRequestCode,
                "DtoTemplate.GenerateCreateRequest", info.EntityName);

            var updateRequestCode = DtoTemplate.GenerateUpdateRequest(info);
            SafeAddSource(context, $"Update{info.EntityName}Request.g.cs", updateRequestCode,
                "DtoTemplate.GenerateUpdateRequest", info.EntityName);
        }

        // =====================================================================
        // BACKEND - Commands
        // =====================================================================
        if (info.GenerateCommands)
        {
            var createCommandCode = CommandsTemplate.GenerateCreateCommand(info);
            SafeAddSource(context, $"Create{info.EntityName}Command.g.cs", createCommandCode,
                "CommandsTemplate.GenerateCreateCommand", info.EntityName);

            var updateCommandCode = CommandsTemplate.GenerateUpdateCommand(info);
            SafeAddSource(context, $"Update{info.EntityName}Command.g.cs", updateCommandCode,
                "CommandsTemplate.GenerateUpdateCommand", info.EntityName);

            var deleteCommandCode = CommandsTemplate.GenerateDeleteCommand(info);
            SafeAddSource(context, $"Delete{info.EntityName}Command.g.cs", deleteCommandCode,
                "CommandsTemplate.GenerateDeleteCommand", info.EntityName);

            if (info.SupportsBatchDelete)
            {
                var batchDeleteCode = CommandsTemplate.GenerateBatchDeleteCommand(info);
                SafeAddSource(context, $"Delete{info.PluralName}Command.g.cs", batchDeleteCode,
                    "CommandsTemplate.GenerateBatchDeleteCommand", info.EntityName);
            }

            // ✅ v4.3: Toggle Ativo Command
            var hasAtivoField = info.Properties.Any(p =>
                p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

            if (hasAtivoField)
            {
                var toggleCommandCode = ToggleAtivoCommandTemplate.GenerateCommand(info);
                SafeAddSource(context, $"Toggle{info.EntityName}AtivoCommand.g.cs", toggleCommandCode,
                    "ToggleAtivoCommandTemplate.GenerateCommand", info.EntityName);
            }
        }

        // =====================================================================
        // BACKEND - Queries
        // =====================================================================
        if (info.GenerateQueries)
        {
            var getByIdQueryCode = QueriesTemplate.GenerateGetByIdQuery(info);
            SafeAddSource(context, $"GetBy{info.EntityName}IdQuery.g.cs", getByIdQueryCode,
                "QueriesTemplate.GenerateGetByIdQuery", info.EntityName);

            var getPagedQueryCode = QueriesTemplate.GenerateGetPagedQuery(info);
            SafeAddSource(context, $"Get{info.PluralName}PagedQuery.g.cs", getPagedQueryCode,
                "QueriesTemplate.GenerateGetPagedQuery", info.EntityName);
        }

        // =====================================================================
        // BACKEND - Validators
        // =====================================================================
        if (info.GenerateValidators)
        {
            var createValidatorCode = ValidatorsTemplate.GenerateCreateValidator(info);
            SafeAddSource(context, $"Create{info.EntityName}RequestValidator.g.cs", createValidatorCode,
                "ValidatorsTemplate.GenerateCreateValidator", info.EntityName);

            var updateValidatorCode = ValidatorsTemplate.GenerateUpdateValidator(info);
            SafeAddSource(context, $"Update{info.EntityName}RequestValidator.g.cs", updateValidatorCode,
                "ValidatorsTemplate.GenerateUpdateValidator", info.EntityName);
        }

        // =====================================================================
        // BACKEND - Repository
        // =====================================================================
        if (info.GenerateRepository)
        {
            var repoInterfaceCode = RepositoryTemplate.GenerateInterface(info);
            SafeAddSource(context, $"I{info.EntityName}Repository.g.cs", repoInterfaceCode,
                "RepositoryTemplate.GenerateInterface", info.EntityName);

            var repoImplCode = RepositoryTemplate.GenerateImplementation(info);
            SafeAddSource(context, $"{info.EntityName}Repository.g.cs", repoImplCode,
                "RepositoryTemplate.GenerateImplementation", info.EntityName);
        }

        // =====================================================================
        // BACKEND - AutoMapper Profile
        // =====================================================================
        if (info.GenerateMapper)
        {
            var mapperCode = MapperTemplate.GenerateProfile(info);
            SafeAddSource(context, $"{info.EntityName}Profile.g.cs", mapperCode,
                "MapperTemplate.GenerateProfile", info.EntityName);
        }

        // =====================================================================
        // BACKEND - EF Configuration
        // =====================================================================
        if (info.GenerateEfConfig)
        {
            var efConfigCode = EfConfigTemplate.GenerateConfig(info);
            SafeAddSource(context, $"{info.EntityName}Configuration.g.cs", efConfigCode,
                "EfConfigTemplate.GenerateConfig", info.EntityName);
        }

        // =====================================================================
        // BACKEND - MetadataProvider (v3.1)
        // =====================================================================
        if (info.GenerateMetadata)
        {
            var metadataCode = MetadataTemplate.GenerateMetadataProvider(info);
            SafeAddSource(context, $"{info.EntityName}MetadataProvider.g.cs", metadataCode,
                "MetadataTemplate.GenerateMetadataProvider", info.EntityName);
        }

        // =====================================================================
        // API - Controller
        // =====================================================================
        if (info.GenerateApiController)
        {
            var apiControllerCode = ApiControllerTemplate.GenerateController(info);
            SafeAddSource(context, $"{info.PluralName}Controller.Api.g.cs", apiControllerCode,
                "ApiControllerTemplate.GenerateController", info.EntityName);
        }

        // =====================================================================
        // WEB - Controller
        // =====================================================================
        if (info.GenerateWebController)
        {
            var webControllerCode = WebControllerTemplate.GenerateController(info);
            SafeAddSource(context, $"{info.PluralName}Controller.Web.g.cs", webControllerCode,
                "WebControllerTemplate.GenerateController", info.EntityName);
        }

        // =====================================================================
        // WEB - Models
        // =====================================================================
        if (info.GenerateWebModels)
        {
            var webDtoCode = WebModelsTemplate.GenerateDto(info);
            SafeAddSource(context, $"{info.EntityName}Dto.Web.g.cs", webDtoCode,
                "WebModelsTemplate.GenerateDto", info.EntityName);

            var webCreateDtoCode = WebModelsTemplate.GenerateCreateDto(info);
            SafeAddSource(context, $"Create{info.EntityName}Dto.Web.g.cs", webCreateDtoCode,
                "WebModelsTemplate.GenerateCreateDto", info.EntityName);

            var webUpdateDtoCode = WebModelsTemplate.GenerateUpdateDto(info);
            SafeAddSource(context, $"Update{info.EntityName}Dto.Web.g.cs", webUpdateDtoCode,
                "WebModelsTemplate.GenerateUpdateDto", info.EntityName);

            var webViewModelCode = WebModelsTemplate.GenerateListViewModel(info);
            SafeAddSource(context, $"{info.PluralName}ListViewModel.g.cs", webViewModelCode,
                "WebModelsTemplate.GenerateListViewModel", info.EntityName);
        }

        // =====================================================================
        // WEB - Services
        // =====================================================================
        if (info.GenerateWebServices)
        {
            var serviceInterfaceCode = WebServicesTemplate.GenerateInterface(info);
            SafeAddSource(context, $"I{info.EntityName}ApiService.g.cs", serviceInterfaceCode,
                "WebServicesTemplate.GenerateInterface", info.EntityName);

            var serviceImplCode = WebServicesTemplate.GenerateImplementation(info);
            SafeAddSource(context, $"{info.EntityName}ApiService.g.cs", serviceImplCode,
                "WebServicesTemplate.GenerateImplementation", info.EntityName);
        }
    }
}