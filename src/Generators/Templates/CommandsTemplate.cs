// =============================================================================
// RHSENSOERP GENERATOR v4.7 - COMMANDS TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/CommandsTemplate.cs
// Versão: 4.7 - ADICIONADO: Suporte a chave primária composta
//
// ✅ NOVIDADES v4.7:
// 1. CreateCommand: retorna string para PK composta (ex: "SEG|SEG_FM_FUNCAO")
// 2. UpdateCommand: recebe múltiplos parâmetros de PK
// 3. DeleteCommand: recebe múltiplos parâmetros de PK
// 4. BatchDelete: NÃO gerado para PK composta (sem sentido com List<string>)
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
    /// v4.7: Para PK composta, retorna string concatenada.
    /// </summary>
    public static string GenerateCreateCommand(EntityInfo info)
    {
        // ✅ v4.7: Tipo de retorno do Create
        var returnType = info.HasCompositeKey ? "string" : info.PrimaryKeyType;

        var tenantAssignment = GenerateTenantAssignment(info, returnType);
        var creationAudit = GenerateCreationAudit(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        // ✅ v4.0: IUniqueValidatable
        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var uniqueInterface = hasUniqueProps ? ", IUniqueValidatable" : "";
        var uniqueUsing = hasUniqueProps ? "\nusing RhSensoERP.Shared.Application.Interfaces;" : "";
        var tenantIdProperty = hasUniqueProps && !info.IsLegacyTable
            ? "\n    public Guid TenantId { get; init; }"
            : "";

        // ✅ v4.7: Expressão de retorno e log
        var returnExpression = info.PrimaryKeyResultExpression("entity");
        var logExpression = info.PrimaryKeyResultExpression("entity");

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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
    : IRequest<Result<{{returnType}}>>{{uniqueInterface}}
{{{tenantIdProperty}}
{{(hasUniqueProps ? GenerateUniqueValidatableImplementation(info, isCreate: true) : "")}}
}

/// <summary>
/// Handler do command de criação.
/// </summary>
public sealed class Create{{info.EntityName}}Handler
    : IRequestHandler<Create{{info.EntityName}}Command, Result<{{returnType}}>>
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

    public async Task<Result<{{returnType}}>> Handle(
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

            _logger.LogInformation("{{info.DisplayName}} criado com sucesso: {Id}", {{logExpression}});

            return Result<{{returnType}}>.Success({{returnExpression}});
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            // ✅ CAPTURA VIOLAÇÃO DE UNICIDADE (SQL 2601/2627)
            var constraintName = ExtractConstraintNameFromException(ex);
            
            // Tenta identificar o campo amigável
            var fieldName = IdentifyFieldFromConstraint(constraintName, "{{info.EntityName}}");
            var friendlyMessage = !string.IsNullOrEmpty(fieldName)
                ? $"Já existe um registro com este '{fieldName}'."
                : "Já existe um registro com estas informações (duplicidade detectada).";

            _logger.LogWarning("Tentativa de duplicidade em Create{{info.EntityName}}: {Constraint}", constraintName);

            return Result<{{returnType}}>.Failure(
                Error.Conflict("{{info.EntityName}}.Duplicate", friendlyMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar {{info.DisplayName}}. Ex: {ExType} | Inner: {InnerType} | Msg: {Msg}", 
                ex.GetType().Name, ex.InnerException?.GetType().Name, ex.Message);
            
            return Result<{{returnType}}>.Failure(
                Error.Failure("{{info.EntityName}}.CreateError", $"Erro ao criar {{info.DisplayName}}: {ex.Message}"));
        }
    }

    /// <summary>
    /// Verifica se a exceção é uma violação de Unicidade (Duplicate Key).
    /// </summary>
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        var sqlException = ex.GetBaseException() as SqlException;
        
        if (sqlException == null && ex.InnerException is SqlException innerSql)
            sqlException = innerSql;

        if (sqlException == null) return false;

        return sqlException.Number == 2601 || sqlException.Number == 2627;
    }

    /// <summary>
    /// Extrai o nome da constraint/index da mensagem de erro SQL.
    /// </summary>
    private static string ExtractConstraintNameFromException(DbUpdateException ex)
    {
        var sqlException = ex.GetBaseException() as SqlException;
        
        if (sqlException == null && ex.InnerException is SqlException innerSql)
            sqlException = innerSql;

        if (sqlException == null) return string.Empty;

        var message = sqlException.Message;

        var match = System.Text.RegularExpressions.Regex.Match(
            message,
            @"['""]([^'""]*?)(?:_Tenant_|_)([^'""]*?)['""]", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (!match.Success)
        {
             match = System.Text.RegularExpressions.Regex.Match(
                message,
                @"index\s+['""]([^'""]+)['""]",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    /// <summary>
    /// Tenta adivinhar o nome do campo baseado no nome da constraint.
    /// </summary>
    private static string IdentifyFieldFromConstraint(string constraintName, string entityName)
    {
        if (string.IsNullOrEmpty(constraintName)) return "";
        var parts = constraintName.Split('_');
        if (parts.Length > 0)
        {
            var candidate = parts.Last();
            if (candidate.Equals("TenantId", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                candidate = parts[parts.Length - 2];
            return candidate;
        }
        return "";
    }
}
""";
    }

    /// <summary>
    /// Gera o Command de Update.
    /// v4.7: Suporte a PK composta nos parâmetros do record.
    /// </summary>
    public static string GenerateUpdateCommand(EntityInfo info)
    {
        var tenantValidation = GenerateTenantValidation(info);
        var updateAudit = GenerateUpdateAudit(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var uniqueInterface = hasUniqueProps ? ", IUniqueValidatable" : "";
        var uniqueUsing = hasUniqueProps ? "\nusing RhSensoERP.Shared.Application.Interfaces;" : "";
        var tenantIdProperty = hasUniqueProps && !info.IsLegacyTable
            ? "\n    public Guid TenantId { get; init; }"
            : "";

        // ✅ v4.7: Record params e GetByIdAsync args
        var updateRecordParams = GenerateUpdateRecordParams(info);
        var getByIdArgs = GenerateGetByIdArgs(info, "command");
        var logExpr = GenerateCommandLogExpression(info, "command");

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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
    {{updateRecordParams}},
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
            _logger.LogDebug("Atualizando {{info.DisplayName}} {Id}...", {{logExpr}});

            // Busca entidade existente
            var entity = await _repository.GetByIdAsync({{getByIdArgs}}, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} não encontrado");
                return Result<bool>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

{{tenantValidation}}

            // Mapeia Request -> Entity
            _mapper.Map(command.Request, entity);

{{updateAudit}}

            // Persiste
            await _repository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} atualizado com sucesso");

            return Result<bool>.Success(true);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            var constraintName = ExtractConstraintNameFromException(ex);
            var fieldName = IdentifyFieldFromConstraint(constraintName, "{{info.EntityName}}");
            var friendlyMessage = !string.IsNullOrEmpty(fieldName)
                ? $"Já existe um registro com este '{fieldName}'."
                : "Já existe um registro com estas informações (duplicidade detectada).";

            _logger.LogWarning("Tentativa de duplicidade em Update{{info.EntityName}}: {Constraint}", constraintName);

            return Result<bool>.Failure(
                Error.Conflict("{{info.EntityName}}.Duplicate", friendlyMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar {{info.DisplayName}}. Ex: {ExType} | Inner: {InnerType} | Msg: {Msg}", 
                ex.GetType().Name, ex.InnerException?.GetType().Name, ex.Message);
                
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.UpdateError", $"Erro ao atualizar {{info.DisplayName}}: {ex.Message}"));
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        var sqlException = ex.GetBaseException() as SqlException;
        if (sqlException == null && ex.InnerException is SqlException innerSql)
            sqlException = innerSql;
        if (sqlException != null)
        {
            if (sqlException.Number == 2601 || sqlException.Number == 2627) return true;
        }
        var msg = ex.GetBaseException()?.Message ?? ex.Message;
        if (string.IsNullOrEmpty(msg)) return false;
        return msg.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
               msg.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ||
               msg.Contains("Violation of UNIQUE KEY", StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractConstraintNameFromException(DbUpdateException ex)
    {
        var sqlException = ex.GetBaseException() as SqlException;
        if (sqlException == null && ex.InnerException is SqlException innerSql)
            sqlException = innerSql;
        if (sqlException == null) return string.Empty;
        var message = sqlException.Message;
        var match = System.Text.RegularExpressions.Regex.Match(
            message, @"['""]([^'""]*?)(?:_Tenant_|_)([^'""]*?)['""]", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (!match.Success)
        {
             match = System.Text.RegularExpressions.Regex.Match(
                message, @"index\s+['""]([^'""]+)['""]",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static string IdentifyFieldFromConstraint(string constraintName, string entityName)
    {
        if (string.IsNullOrEmpty(constraintName)) return "";
        var parts = constraintName.Split('_');
        if (parts.Length > 0)
        {
            var candidate = parts.Last();
            if (candidate.Equals("TenantId", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                candidate = parts[parts.Length - 2];
            return candidate;
        }
        return "";
    }
}
""";
    }

    /// <summary>
    /// ✅ v4.7: Gera o Command de Delete com tratamento de Foreign Key.
    /// Suporte a PK composta.
    /// </summary>
    public static string GenerateDeleteCommand(EntityInfo info)
    {
        var tenantValidation = GenerateTenantValidation(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        // ✅ v4.7: Record params para PK composta
        var deleteRecordParams = GenerateDeleteRecordParams(info);
        var getByIdArgs = GenerateGetByIdArgs(info, "command");
        var logExpr = GenerateCommandLogExpression(info, "command");

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
public sealed record Delete{{info.EntityName}}Command({{deleteRecordParams}})
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
            _logger.LogDebug("Excluindo {{info.DisplayName}} {Id}...", {{logExpr}});

            var entity = await _repository.GetByIdAsync({{getByIdArgs}}, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} não encontrado");
                return Result<bool>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

{{tenantValidation}}

            // ✅ Tenta deletar
            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("{{info.DisplayName}} excluído com sucesso");

            return Result<bool>.Success(true);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            var tableName = ExtractTableNameFromException(ex);
            var errorMessage = string.IsNullOrEmpty(tableName)
                ? "Não é possível excluir este registro porque existem registros relacionados em outras tabelas."
                : $"Não é possível excluir este registro porque existem registros relacionados na tabela '{tableName}'.";

            _logger.LogWarning("Tentativa de exclusão bloqueada por FK: {{info.DisplayName}}");

            return Result<bool>.Failure(
                Error.Conflict("{{info.EntityName}}.ForeignKeyViolation", errorMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}}");
            return Result<bool>.Failure(
                Error.Failure("{{info.EntityName}}.DeleteError", $"Erro ao excluir {{info.DisplayName}}: {ex.Message}"));
        }
    }

    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return false;
        return sqlException.Number == 547;
    }

    private static string ExtractTableNameFromException(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return string.Empty;
        var message = sqlException.Message;
        var match = System.Text.RegularExpressions.Regex.Match(
            message, @"tabela\s+[""'](?:dbo\.)?([^""']+)[""']",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}
""";
    }

    /// <summary>
    /// ✅ v4.7: Gera o Command de Batch Delete.
    /// NÃO gerado para PK composta (retorna null).
    /// </summary>
    public static string? GenerateBatchDeleteCommand(EntityInfo info)
    {
        // ✅ v4.7: Batch delete não suportado para PK composta
        if (info.HasCompositeKey)
            return null;

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

                await _repository.DeleteAsync(entity, cancellationToken);

                result.TotalDeletados++;
                _logger.LogDebug("{{info.DisplayName}} {Id} excluído com sucesso", id);
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
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
                result.TotalNaoDeletados++;
                result.Erros.Add($"ID {id}: Erro ao excluir - {ex.Message}");
                _logger.LogError(ex, "Erro ao excluir {{info.DisplayName}} {Id}", id);
            }
        }

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

        _logger.LogInformation(
            "Exclusão em lote concluída: {Deletados} deletados, {NaoDeletados} não deletados",
            result.TotalDeletados, result.TotalNaoDeletados);

        return Result<BatchDeleteResult>.Success(result);
    }

    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return false;
        return sqlException.Number == 547;
    }

    private static string ExtractTableNameFromException(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlException)
            return string.Empty;
        var message = sqlException.Message;
        var match = System.Text.RegularExpressions.Regex.Match(
            message, @"tabela\s+[""'](?:dbo\.)?([^""']+)[""']",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}
""";
    }

    // =========================================================================
    // ✅ v4.7: HELPERS PARA PK COMPOSTA
    // =========================================================================

    /// <summary>
    /// Gera parâmetros do record Update.
    /// Simples: "int Id" | Composta: "string CdSistema, string CdFuncao"
    /// </summary>
    private static string GenerateUpdateRecordParams(EntityInfo info)
    {
        if (!info.HasCompositeKey)
            return $"{info.PrimaryKeyType} Id";

        var parts = new List<string>();
        for (int i = 0; i < info.CompositeKeyProperties.Count; i++)
            parts.Add($"{info.CompositeKeyTypes[i]} {info.CompositeKeyProperties[i]}");
        return string.Join(",\n    ", parts);
    }

    /// <summary>
    /// Gera parâmetros do record Delete.
    /// Simples: "int Id" | Composta: "string CdSistema, string CdFuncao"
    /// </summary>
    private static string GenerateDeleteRecordParams(EntityInfo info)
    {
        if (!info.HasCompositeKey)
            return $"{info.PrimaryKeyType} Id";

        var parts = new List<string>();
        for (int i = 0; i < info.CompositeKeyProperties.Count; i++)
            parts.Add($"{info.CompositeKeyTypes[i]} {info.CompositeKeyProperties[i]}");
        return string.Join(", ", parts);
    }

    /// <summary>
    /// Gera argumentos para GetByIdAsync a partir de um command/query.
    /// Simples: "command.Id" | Composta: "command.CdSistema, command.CdFuncao"
    /// </summary>
    private static string GenerateGetByIdArgs(EntityInfo info, string sourceVar)
    {
        if (!info.HasCompositeKey)
            return $"{sourceVar}.Id";

        return string.Join(", ", info.CompositeKeyProperties.Select(p => $"{sourceVar}.{p}"));
    }

    /// <summary>
    /// Gera expressão de log para PK a partir de um command.
    /// Simples: command.Id | Composta: $"{command.CdSistema}|{command.CdFuncao}"
    /// </summary>
    private static string GenerateCommandLogExpression(EntityInfo info, string sourceVar)
    {
        if (!info.HasCompositeKey)
            return $"{sourceVar}.Id";

        var interpolations = info.CompositeKeyProperties
            .Select(p => $"{{{sourceVar}.{p}}}");
        return "$\"" + string.Join("|", interpolations) + "\"";
    }

    // =========================================================================
    // MÉTODOS AUXILIARES - TENANT
    // =========================================================================

    private static string GenerateTenantAssignment(EntityInfo info, string returnType)
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
                return Result<{returnType}>.Failure(
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
                    ""Tentativa de acesso cross-tenant detectada"");
                
                return Result<bool>.Failure(
                    Error.Forbidden(""{info.EntityName}.Forbidden"", ""Acesso negado ao recurso""));
            }}";
    }

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

        return $@"            var tenantId = _currentUser.TenantId;
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
            lines.Add($"            entity.{info.CreatedAtField} = DateTime.UtcNow;");

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
            lines.Add($"            entity.{info.UpdatedAtField} = DateTime.UtcNow;");

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
    // ✅ v4.1: UNIQUE VALIDATION
    // =========================================================================
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