// =============================================================================
// RHSENSOERP GENERATOR v3.8 - COMMANDS TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/CommandsTemplate.cs
// Versão: 3.8 - Multi-tenancy + Auditoria + Usings corretos
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Commands (MediatR).
/// </summary>
public static class CommandsTemplate
{
    /// <summary>
    /// Gera o Command de Create.
    /// </summary>
    public static string GenerateCreateCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var tenantAssignment = GenerateTenantAssignment(info);
        var creationAudit = GenerateCreationAudit(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.Namespace}};
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para criar {{info.DisplayName}}.
/// </summary>
public sealed record Create{{info.EntityName}}Command(Create{{info.EntityName}}Request Request)
    : IRequest<Result<{{pkType}}>>;

/// <summary>
/// Handler do command de criação.
/// </summary>
public sealed class Create{{info.EntityName}}Handler
    : IRequestHandler<Create{{info.EntityName}}Command, Result<{{pkType}}>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Create{{info.EntityName}}Handler> _logger;{{currentUserField}}

    public Create{{info.EntityName}}Handler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<Create{{info.EntityName}}Handler> logger{{currentUserParam}})
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;{{currentUserAssign}}
    }

    public async Task<Result<{{pkType}}>> Handle(
        Create{{info.EntityName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = command.Request;

            _logger.LogDebug("Criando novo {{info.DisplayName}}...");

            // Mapeia Request -> Entity
            var entity = _mapper.Map<{{info.EntityName}}>(request);

{{tenantAssignment}}
{{creationAudit}}

            // Persiste
            await _repository.AddAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} criado com sucesso: {Id}", entity.{{info.PrimaryKeyProperty}});

            return Result<{{pkType}}>.Success(entity.{{info.PrimaryKeyProperty}});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar {{info.DisplayName}}");
            return Result<{{pkType}}>.Failure(
                Error.Failure("{{info.EntityName}}.CreateError", $"Erro ao criar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera o Command de Update.
    /// </summary>
    public static string GenerateUpdateCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var tenantValidation = GenerateTenantValidation(info);
        var updateAudit = GenerateUpdateAudit(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.Namespace}};
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para atualizar {{info.DisplayName}}.
/// </summary>
public sealed record Update{{info.EntityName}}Command(
    {{pkType}} Id,
    Update{{info.EntityName}}Request Request)
    : IRequest<Result<bool>>;

/// <summary>
/// Handler do command de atualização.
/// </summary>
public sealed class Update{{info.EntityName}}Handler
    : IRequestHandler<Update{{info.EntityName}}Command, Result<bool>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Update{{info.EntityName}}Handler> _logger;{{currentUserField}}

    public Update{{info.EntityName}}Handler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<Update{{info.EntityName}}Handler> logger{{currentUserParam}})
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;{{currentUserAssign}}
    }

    public async Task<Result<bool>> Handle(
        Update{{info.EntityName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Atualizando {{info.DisplayName}} {Id}...", command.Id);

            // Busca entidade existente
            var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", command.Id);
                return Result<bool>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

{{tenantValidation}}

            // Aplica mudanças do Request na Entity
            _mapper.Map(command.Request, entity);

{{updateAudit}}

            // Atualiza
            await _repository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} {Id} atualizado com sucesso", command.Id);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar {{info.DisplayName}} {Id}", command.Id);
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.UpdateError", $"Erro ao atualizar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera o Command de Delete.
    /// </summary>
    public static string GenerateDeleteCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var tenantValidation = GenerateTenantValidation(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para excluir {{info.DisplayName}}.
/// </summary>
public sealed record Delete{{info.EntityName}}Command({{pkType}} Id)
    : IRequest<Result<bool>>;

/// <summary>
/// Handler do command de exclusão.
/// </summary>
public sealed class Delete{{info.EntityName}}Handler
    : IRequestHandler<Delete{{info.EntityName}}Command, Result<bool>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly ILogger<Delete{{info.EntityName}}Handler> _logger;{{currentUserField}}

    public Delete{{info.EntityName}}Handler(
        I{{info.EntityName}}Repository repository,
        ILogger<Delete{{info.EntityName}}Handler> logger{{currentUserParam}})
    {
        _repository = repository;
        _logger = logger;{{currentUserAssign}}
    }

    public async Task<Result<bool>> Handle(
        Delete{{info.EntityName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Excluindo {{info.DisplayName}} {Id}...", command.Id);

            var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", command.Id);
                return Result<bool>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

{{tenantValidation}}

            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} {Id} excluído com sucesso", command.Id);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", command.Id);
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.DeleteError", $"Erro ao excluir {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera o Command de Batch Delete.
    /// </summary>
    public static string GenerateBatchDeleteCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var tenantFilter = GenerateTenantFilterForBatch(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para excluir múltiplos {{info.DisplayName}} em lote.
/// </summary>
public sealed record Delete{{info.PluralName}}Command(List<{{pkType}}> Ids)
    : IRequest<Result<bool>>;

/// <summary>
/// Handler do command de exclusão em lote.
/// </summary>
public sealed class Delete{{info.PluralName}}Handler
    : IRequestHandler<Delete{{info.PluralName}}Command, Result<bool>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly ILogger<Delete{{info.PluralName}}Handler> _logger;{{currentUserField}}

    public Delete{{info.PluralName}}Handler(
        I{{info.EntityName}}Repository repository,
        ILogger<Delete{{info.PluralName}}Handler> logger{{currentUserParam}})
    {
        _repository = repository;
        _logger = logger;{{currentUserAssign}}
    }

    public async Task<Result<bool>> Handle(
        Delete{{info.PluralName}}Command command,
        CancellationToken cancellationToken)
    {
        try
        {
            var ids = command.Ids;

            if (ids == null || ids.Count == 0)
            {
                return Result<bool>.Failure(
                    Error.Validation("{{info.EntityName}}.EmptyList", "Nenhum ID fornecido para exclusão"));
            }

            _logger.LogDebug("Excluindo {Count} {{info.DisplayName}}...", ids.Count);

            // Busca entidades
{{tenantFilter}}

            var successCount = 0;
            var errors = new List<string>();

            foreach (var entity in entities)
            {
                try
                {
                    await _repository.DeleteAsync(entity, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Erro ao excluir {entity.{{info.PrimaryKeyProperty}}}: {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning("Exclusão em lote parcial: {Success}/{Total}", successCount, ids.Count);
                return Result<bool>.Failure(
                    Error.Failure("{{info.EntityName}}.PartialDelete",
                        $"Excluídos {successCount}/{ids.Count}. Erros: {string.Join("; ", errors)}"));
            }

            _logger.LogInformation("{Count} {{info.DisplayName}} excluídos com sucesso", successCount);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} em lote");
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.BatchDeleteError",
                    $"Erro ao excluir {{info.DisplayName}} em lote: {ex.Message}"));
        }
    }
}
""";
    }

    // =========================================================================
    // MÉTODOS AUXILIARES - TENANT
    // =========================================================================

    private static string GenerateTenantAssignment(EntityInfo info)
    {
        if (info.IsLegacyTable)
            return "            // Tabela legada: sem TenantId";

        var tenantProp = info.Properties.FirstOrDefault(p => p.Name == "TenantId");
        var isString = tenantProp?.IsString == true;
        var valueExpression = isString ? "tenantId.ToString()" : "tenantId";

        return $@"            // ✅ OBRIGATÓRIO: Atribui TenantId do contexto do usuário
            var tenantId = _currentUser.TenantId;
            if (tenantId == Guid.Empty)
            {{
                return Result<{info.PrimaryKeyType}>.Failure(
                    Error.Unauthorized(""User.TenantNotFound"", ""TenantId não encontrado no contexto do usuário""));
            }}
            entity.TenantId = {valueExpression};";
    }

    private static string GenerateTenantValidation(EntityInfo info)
    {
        if (info.IsLegacyTable)
            return "            // Tabela legada: sem validação de tenant";

        var tenantProp = info.Properties.FirstOrDefault(p => p.Name == "TenantId");
        var isString = tenantProp?.IsString == true;
        var comparison = isString ? "entity.TenantId != tenantId.ToString()" : "entity.TenantId != tenantId";

        return $@"            // ✅ OBRIGATÓRIO: Valida se pertence ao tenant do usuário
            var tenantId = _currentUser.TenantId;
            if ({comparison})
            {{
                _logger.LogWarning(
                    ""Tentativa de acesso cross-tenant: Usuário {{UserId}} (Tenant {{UserTenant}}) tentou acessar {{Id}} (Tenant {{RecordTenant}})"",
                    _currentUser.UserId,
                    tenantId,
                    command.Id,
                    entity.TenantId);
                
                return Result<bool>.Failure(
                    Error.Forbidden(""{info.EntityName}.Forbidden"", ""Acesso negado ao recurso""));
            }}";
    }

    private static string GenerateTenantFilterForBatch(EntityInfo info)
    {
        if (info.IsLegacyTable)
        {
            return @"            var entities = await _repository.Query()
                .Where(e => ids.Contains(e." + info.PrimaryKeyProperty + @"))
                .ToListAsync(cancellationToken);";
        }

        var tenantProp = info.Properties.FirstOrDefault(p => p.Name == "TenantId");
        var isString = tenantProp?.IsString == true;
        var comparison = isString ? "e.TenantId == tenantId.ToString()" : "e.TenantId == tenantId";

        return $@"            // ✅ OBRIGATÓRIO: Filtra por TenantId
            var tenantId = _currentUser.TenantId;
            if (tenantId == Guid.Empty)
            {{
                return Result<bool>.Failure(
                    Error.Unauthorized(""User.TenantNotFound"", ""TenantId não encontrado no contexto do usuário""));
            }}

            var entities = await _repository.Query()
                .Where(e => ids.Contains(e.{info.PrimaryKeyProperty}) && {comparison})
                .ToListAsync(cancellationToken);";
    }

    // =========================================================================
    // MÉTODOS AUXILIARES - AUDITORIA
    // =========================================================================

    private static string GenerateCreationAudit(EntityInfo info)
    {
        if (!info.HasCreationAudit)
            return "            // Sem campos de auditoria de criação";

        var lines = new List<string> { "            // Auditoria de criação" };

        if (!string.IsNullOrEmpty(info.CreatedAtField))
        {
            lines.Add($"            entity.{info.CreatedAtField} = DateTime.UtcNow;");
        }

        if (!string.IsNullOrEmpty(info.CreatedByField) && !info.IsLegacyTable)
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == info.CreatedByField);
            var isString = prop?.IsString == true;
            var value = isString ? "_currentUser.UserId.ToString()" : "_currentUser.UserId";
            
            lines.Add($"            entity.{info.CreatedByField} = {value};");
        }

        return string.Join("\n", lines);
    }

    private static string GenerateUpdateAudit(EntityInfo info)
    {
        if (!info.HasUpdateAudit)
            return "            // Sem campos de auditoria de atualização";

        var lines = new List<string> { "            // Auditoria de atualização" };

        if (!string.IsNullOrEmpty(info.UpdatedAtField))
        {
            lines.Add($"            entity.{info.UpdatedAtField} = DateTime.UtcNow;");
        }

        if (!string.IsNullOrEmpty(info.UpdatedByField) && !info.IsLegacyTable)
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == info.UpdatedByField);
            var isString = prop?.IsString == true;
            var value = isString ? "_currentUser.UserId.ToString()" : "_currentUser.UserId";

            lines.Add($"            entity.{info.UpdatedByField} = {value};");
        }

        return string.Join("\n", lines);
    }
}