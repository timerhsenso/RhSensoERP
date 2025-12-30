// =============================================================================
// RHSENSOERP GENERATOR v4.2.1 - COMMANDS TEMPLATE (HOTFIX)
// =============================================================================
// Arquivo: src/Generators/Templates/CommandsTemplate.cs
// Versão: 4.2.1 - HOTFIX: Error.Validation aceita apenas 2 argumentos
// 
// ✅ CORREÇÃO v4.2.1:
// - Removido terceiro argumento inválido de Error.Validation
// - DELETE BATCH retorna Success quando pelo menos 1 foi deletado
// - DELETE BATCH retorna Failure quando NENHUM foi deletado
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

        // ✅ v4.0: Implementar IUniqueValidatable se houver campos únicos
        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var uniqueInterface = hasUniqueProps ? ", IUniqueValidatable" : "";
        var uniqueUsing = hasUniqueProps ? "\nusing RhSensoERP.Shared.Application.Interfaces;" : "";

        // ✅ v4.1: Command precisa armazenar TenantId para IUniqueValidatable
        var tenantIdProperty = hasUniqueProps && !info.IsLegacyTable
            ? "\n    public Guid TenantId { get; init; }"
            : "";

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
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}{{uniqueUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para criar {{info.DisplayName}}.
/// </summary>
public sealed record Create{{info.EntityName}}Command(Create{{info.EntityName}}Request Request)
    : IRequest<Result<{{pkType}}>>{{uniqueInterface}}
{{{tenantIdProperty}}
{{(hasUniqueProps ? GenerateUniqueValidatableImplementation(info, isCreate: true) : "")}}
}

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

        // ✅ v4.0: Implementar IUniqueValidatable se houver campos únicos
        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var uniqueInterface = hasUniqueProps ? ", IUniqueValidatable" : "";
        var uniqueUsing = hasUniqueProps ? "\nusing RhSensoERP.Shared.Application.Interfaces;" : "";

        // ✅ v4.1: Command precisa armazenar TenantId para IUniqueValidatable
        var tenantIdProperty = hasUniqueProps && !info.IsLegacyTable
            ? "\n    public Guid TenantId { get; init; }"
            : "";

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
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}{{uniqueUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para atualizar {{info.DisplayName}}.
/// </summary>
public sealed record Update{{info.EntityName}}Command(
    {{pkType}} Id,
    Update{{info.EntityName}}Request Request)
    : IRequest<Result<bool>>{{uniqueInterface}}
{{{tenantIdProperty}}
{{(hasUniqueProps ? GenerateUniqueValidatableImplementation(info, isCreate: false) : "")}}
}

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

            // Mapeia Request -> Entity
            _mapper.Map(command.Request, entity);

{{updateAudit}}

            // Persiste
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
    /// ✅ v4.2: Gera o Command de Delete com tratamento de Foreign Key.
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

            // ✅ Tenta deletar
            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} {Id} excluído com sucesso", command.Id);

            return Result<bool>.Success(true);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            // ✅ CAPTURA VIOLAÇÃO DE FK - Retorna HTTP 409 Conflict
            var tableName = ExtractTableNameFromException(ex);
            var errorMessage = string.IsNullOrEmpty(tableName)
                ? "Não é possível excluir este registro porque existem registros relacionados em outras tabelas."
                : $"Não é possível excluir este registro porque existem registros relacionados na tabela '{tableName}'.";

            _logger.LogWarning(
                "Tentativa de exclusão bloqueada por FK: {{info.DisplayName}} {Id} | Tabela: {Table}",
                command.Id, tableName);

            return Result<bool>.Failure(
                Error.Conflict("{{info.EntityName}}.ForeignKeyViolation", errorMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", command.Id);
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.DeleteError", $"Erro ao excluir {{info.DisplayName}}: {ex.Message}"));
        }
    }

    /// <summary>
    /// Verifica se a exceção é uma violação de Foreign Key.
    /// </summary>
    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return false;

        // Error Number 547 = Foreign Key Violation
        return sqlException.Number == 547;
    }

    /// <summary>
    /// Extrai o nome da tabela referenciada da mensagem de erro SQL.
    /// </summary>
    private static string ExtractTableNameFromException(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return string.Empty;

        var message = sqlException.Message;

        // Regex para extrair: tabela "dbo.nome_tabela"
        var match = System.Text.RegularExpressions.Regex.Match(
            message,
            @"tabela\s+[""'](?:dbo\.)?([^""']+)[""']",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}
""";
    }

    /// <summary>
    /// ✅ v4.2.1: Gera o Command de Batch Delete com processamento item por item (HOTFIX).
    /// </summary>
    public static string GenerateBatchDeleteCommand(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.CommandsNamespace}};

/// <summary>
/// Command para excluir múltiplos {{info.DisplayName}}.
/// </summary>
public sealed record Delete{{info.PluralName}}Command(List<{{pkType}}> Ids)
    : IRequest<Result<BatchDeleteResult>>;

/// <summary>
/// Handler do command de exclusão em lote.
/// </summary>
public sealed class Delete{{info.PluralName}}Handler
    : IRequestHandler<Delete{{info.PluralName}}Command, Result<BatchDeleteResult>>
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

    public async Task<Result<BatchDeleteResult>> Handle(
        Delete{{info.PluralName}}Command command,
        CancellationToken cancellationToken)
    {
        var ids = command.Ids;

        if (ids == null || ids.Count == 0)
        {
            return Result<BatchDeleteResult>.Failure(
                Error.Validation("{{info.EntityName}}.EmptyList", "Nenhum ID fornecido para exclusão"));
        }

        _logger.LogDebug("Excluindo {Count} {{info.DisplayName}}...", ids.Count);

        var result = new BatchDeleteResult
        {
            TotalProcessados = ids.Count
        };

        // ✅ PROCESSA UM POR UM (nunca em bloco)
        foreach (var id in ids)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id, cancellationToken);

                if (entity == null)
                {
                    result.TotalNaoDeletados++;
                    result.Erros.Add($"ID {id}: Registro não encontrado");
                    _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", id);
                    continue;
                }

{{GenerateTenantValidationForBatch(info)}}

                // ✅ Tenta deletar
                await _repository.DeleteAsync(entity, cancellationToken);

                result.TotalDeletados++;
                _logger.LogDebug("{{info.DisplayName}} {Id} excluído com sucesso", id);
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                // ✅ CAPTURA VIOLAÇÃO DE FK - NÃO INTERROMPE O LOOP
                var tableName = ExtractTableNameFromException(ex);
                var errorMessage = string.IsNullOrEmpty(tableName)
                    ? $"ID {id}: Não pode excluir - existem registros relacionados"
                    : $"ID {id}: Não pode excluir - existem registros relacionados na tabela '{tableName}'";

                result.TotalNaoDeletados++;
                result.Erros.Add(errorMessage);

                _logger.LogWarning(
                    "Exclusão bloqueada por FK: {{info.DisplayName}} {Id} | Tabela: {Table}",
                    id, tableName);
            }
            catch (Exception ex)
            {
                // ✅ OUTROS ERROS - NÃO INTERROMPE O LOOP
                result.TotalNaoDeletados++;
                result.Erros.Add($"ID {id}: Erro ao excluir - {ex.Message}");

                _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", id);
            }
        }

        // ✅ v4.2.1 HOTFIX: Error.Validation aceita apenas 2 argumentos (code, message)
        // Se NENHUM foi deletado → FAILURE
        if (result.TotalDeletados == 0)
        {
            _logger.LogWarning(
                "Nenhum {{info.DisplayName}} foi excluído. Total não deletados: {Count}",
                result.TotalNaoDeletados);

            return Result<BatchDeleteResult>.Failure(
                Error.Validation(
                    "{{info.EntityName}}.BatchDeleteFailed",
                    $"Nenhum registro foi excluído. {result.TotalNaoDeletados} erro(s): {string.Join("; ", result.Erros.Take(3))}"));
        }

        // ✅ Se PELO MENOS UM foi deletado → SUCCESS (mesmo que parcial)
        _logger.LogInformation(
            "Exclusão em lote concluída: {Deletados} deletados, {NaoDeletados} não deletados",
            result.TotalDeletados, result.TotalNaoDeletados);

        return Result<BatchDeleteResult>.Success(result);
    }

    /// <summary>
    /// Verifica se a exceção é uma violação de Foreign Key.
    /// </summary>
    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return false;

        // Error Number 547 = Foreign Key Violation
        return sqlException.Number == 547;
    }

    /// <summary>
    /// Extrai o nome da tabela referenciada da mensagem de erro SQL.
    /// </summary>
    private static string ExtractTableNameFromException(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return string.Empty;

        var message = sqlException.Message;

        // Regex para extrair: tabela "dbo.nome_tabela"
        var match = System.Text.RegularExpressions.Regex.Match(
            message,
            @"tabela\s+[""'](?:dbo\.)?([^""']+)[""']",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value : string.Empty;
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

    /// <summary>
    /// ✅ v4.2: Validação de tenant INLINE para batch delete (não retorna, só adiciona erro).
    /// </summary>
    private static string GenerateTenantValidationForBatch(EntityInfo info)
    {
        if (info.IsLegacyTable)
            return "                // Tabela legada: sem validação de tenant";

        var tenantProp = info.Properties.FirstOrDefault(p => p.Name == "TenantId");
        var isString = tenantProp?.IsString == true;
        var comparison = isString ? "entity.TenantId != tenantId.ToString()" : "entity.TenantId != tenantId";

        return $@"                // ✅ Valida tenant
                var tenantId = _currentUser.TenantId;
                if ({comparison})
                {{
                    result.TotalNaoDeletados++;
                    result.Erros.Add($""ID {{id}}: Acesso negado - registro de outro tenant"");
                    _logger.LogWarning(
                        ""Tentativa cross-tenant: ID {{id}} pertence ao tenant {{RecordTenant}}, usuário é {{UserTenant}}"",
                        entity.TenantId, tenantId);
                    continue;
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

    // =========================================================================
    // ✅ v4.1 CORRIGIDO: UNIQUE VALIDATION - Gera implementação de IUniqueValidatable
    // =========================================================================
    /// <summary>
    /// Gera a implementação automática de IUniqueValidatable.
    /// Permite que o UniqueValidationBehavior valide campos únicos ANTES do SaveChanges.
    /// </summary>
    private static string GenerateUniqueValidatableImplementation(EntityInfo info, bool isCreate)
    {
        var entityIdValue = isCreate ? "null" : "Id";

        return $"""
    
    // ✅ Implementação de IUniqueValidatable (validação automática)
    public Type EntityType => typeof({info.EntityName});
    public object? EntityId => {entityIdValue};
    Guid? IUniqueValidatable.TenantId => TenantId;
""";
    }
}