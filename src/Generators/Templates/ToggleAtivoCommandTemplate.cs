// =============================================================================
// RHSENSOERP GENERATOR v4.3 - TOGGLE ATIVO COMMAND TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/ToggleAtivoCommandTemplate.cs
// Versão: 4.3 - Command e Handler para Toggle Ativo
// 
// ✅ Gera automaticamente quando entidade tem campo Ativo:
// - Command: ToggleTreTiposTreinamentoAtivoCommand
// - Handler: ToggleTreTiposTreinamentoAtivoCommandHandler
// - Validações: NotFound, Forbidden (multi-tenant)
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Toggle Ativo Command e Handler.
/// </summary>
public static class ToggleAtivoCommandTemplate
{
    /// <summary>
    /// Gera o Command e Handler para Toggle Ativo.
    /// </summary>
    public static string GenerateCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        // Encontra o nome exato do campo Ativo
        var ativoField = info.Properties.FirstOrDefault(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

        var fieldName = ativoField?.Name ?? "Ativo";
        var isLegacy = info.IsLegacyTable;

        // ✅ Multi-tenant: valida se registro pertence ao tenant
        var tenantValidation = !isLegacy ? GenerateTenantValidation(info, fieldName) : "";
        var currentUserUsing = !isLegacy ? "\nusing RhSensoERP.Shared.Core.Abstractions;" : "";
        var currentUserField = !isLegacy ? "\n    private readonly ICurrentUser _currentUser;" : "";
        var currentUserParam = !isLegacy ? ",\n        ICurrentUser currentUser" : "";
        var currentUserAssign = !isLegacy ? "\n        _currentUser = currentUser;" : "";

        return $$"""
{{info.FileHeader}}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;{{currentUserUsing}}
using RhSensoERP.Shared.Core.Common;
using {{info.QueriesNamespace}};
using {{info.DtoNamespace}};

namespace {{info.CommandsNamespace}};

// =============================================================================
// COMMAND
// =============================================================================

/// <summary>
/// Command para alternar o status {{fieldName}} de {{info.DisplayName}}.
/// </summary>
public sealed record Toggle{{info.EntityName}}AtivoCommand(
    {{pkType}} Id,
    bool {{fieldName}}) : IRequest<Result<bool>>;

// =============================================================================
// HANDLER
// =============================================================================

/// <summary>
/// Handler para Toggle{{info.EntityName}}AtivoCommand.
/// </summary>
internal sealed class Toggle{{info.EntityName}}AtivoCommandHandler 
    : IRequestHandler<Toggle{{info.EntityName}}AtivoCommand, Result<bool>>
{
    private readonly IMediator _mediator;{{currentUserField}}

    public Toggle{{info.EntityName}}AtivoCommandHandler(
        IMediator mediator{{currentUserParam}})
    {
        _mediator = mediator;{{currentUserAssign}}
    }

    public async Task<Result<bool>> Handle(
        Toggle{{info.EntityName}}AtivoCommand request,
        CancellationToken cancellationToken)
    {
        // ✅ Busca o registro
        var getQuery = new GetBy{{info.EntityName}}IdQuery(request.Id);
        var getResult = await _mediator.Send(getQuery, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result<bool>.Failure(
                Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
        }

        var entity = getResult.Value;
        if (entity is null)
        {
            return Result<bool>.Failure(
                Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
        }
{{tenantValidation}}
        // ✅ Cria request de atualização copiando dados da entidade para evitar erro de validação
        var updateRequest = new Update{{info.EntityName}}Request
        {
            {{fieldName}} = request.{{fieldName}},
{{GeneratePropertyAssignments(info, fieldName)}}
        };

        // ✅ Atualiza usando o comando de Update existente
        var updateCommand = new Update{{info.EntityName}}Command(request.Id, updateRequest);
        var updateResult = await _mediator.Send(updateCommand, cancellationToken);

        return updateResult;
    }
}
""";
    }

    /// <summary>
    /// Gera validação de tenant (apenas para entidades não-legacy).
    /// </summary>
    private static string GenerateTenantValidation(EntityInfo info, string fieldName)
    {
        return $$"""

        // ✅ Valida se registro pertence ao tenant do usuário logado
        // NOTA: Implementar validação de tenant se DTO tiver campo IdSaaS
""";
    }
    /// <summary>
    /// Gera assignments de propriedades para preencher o UpdateRequest.
    /// </summary>
    private static string GeneratePropertyAssignments(EntityInfo info, string toggleField)
    {
        var assignments = new List<string>();

        foreach (var prop in info.Properties)
        {
            // Pula PK e o campo que já está sendo alterado (Toggle)
            if (prop.IsPrimaryKey || prop.Name.Equals(toggleField, StringComparison.OrdinalIgnoreCase))
                continue;

            // Pula campos de auditoria que normalmente não estão no Request
            if (IsAuditField(prop.Name))
                continue;

            assignments.Add($"            {prop.Name} = entity.{prop.Name}");
        }

        return string.Join(",\n", assignments);
    }

    private static bool IsAuditField(string name)
    {
        return name.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("UpdatedBy", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("DeletedAt", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase) ||
               // Campos extras que causam erro se mapeados
               name.Equals("CreatedAtUtc", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("UpdatedAtUtc", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("CreatedByUserId", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("UpdatedByUserId", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("DeletedByUserId", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("TenantId", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("IdSaaS", StringComparison.OrdinalIgnoreCase);
    }
}