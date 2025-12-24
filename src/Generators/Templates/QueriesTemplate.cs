// =============================================================================
// RHSENSOERP GENERATOR v3.7 - QUERIES TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/QueriesTemplate.cs
// Versão: 3.7 - Multi-tenancy com filtro automático de tenant
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

public static class QueriesTemplate
{
    /// <summary>
    /// Gera a Query GetById com validação de tenant.
    /// </summary>
    public static string GenerateGetByIdQuery(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var tenantValidation = GenerateTenantValidationForQuery(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.7
// Entity: {{info.EntityName}}
// =============================================================================
using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.QueriesNamespace}};

/// <summary>
/// Query para buscar {{info.DisplayName}} por ID.
/// </summary>
public sealed record GetBy{{info.EntityName}}IdQuery({{pkType}} Id)
    : IRequest<Result<{{info.EntityName}}Dto>>;

/// <summary>
/// Handler da query de busca por ID.
/// </summary>
public sealed class GetBy{{info.EntityName}}IdHandler
    : IRequestHandler<GetBy{{info.EntityName}}IdQuery, Result<{{info.EntityName}}Dto>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBy{{info.EntityName}}IdHandler> _logger;{{currentUserField}}

    public GetBy{{info.EntityName}}IdHandler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<GetBy{{info.EntityName}}IdHandler> logger{{currentUserParam}})
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;{{currentUserAssign}}
    }

    public async Task<Result<{{info.EntityName}}Dto>> Handle(
        GetBy{{info.EntityName}}IdQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Buscando {{info.DisplayName}} {Id}...", query.Id);

            var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("{{info.DisplayName}} {Id} não encontrado", query.Id);
                return Result<{{info.EntityName}}Dto>.Failure(
                    Error.NotFound("{{info.EntityName}}.NotFound", "{{info.DisplayName}} não encontrado"));
            }

{{tenantValidation}}

            var dto = _mapper.Map<{{info.EntityName}}Dto>(entity);

            return Result<{{info.EntityName}}Dto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {{info.DisplayName}} {Id}", query.Id);
            return Result<{{info.EntityName}}Dto>.Failure(
                Error.Failure("{{info.EntityName}}.Error", $"Erro ao buscar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    /// <summary>
    /// Gera a Query GetPaged com filtro de tenant.
    /// </summary>
    public static string GenerateGetPagedQuery(EntityInfo info)
    {
        var searchProperty = info.Properties
            .FirstOrDefault(p => p.IsString && !p.IsPrimaryKey);

        var searchField = searchProperty?.Name ?? null;
        var hasSearchField = searchField != null;

        var searchFilter = hasSearchField
            ? $@"                queryable = queryable.Where(e =>
                    e.{searchField} != null && 
                    EF.Functions.Like(e.{searchField}.ToLower(), $""%{{search}}%""));"
            : "                // Sem campo de texto para busca";

        // ✅ Filtro de tenant (se não for legada)
        var tenantFilter = GenerateTenantFilterForPaged(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.7
// Entity: {{info.EntityName}}
// =============================================================================
using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using {{info.DtoNamespace}};
using {{info.RepositoryInterfaceNamespace}};
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;{{currentUserUsing}}

namespace {{info.QueriesNamespace}};

/// <summary>
/// Query para listagem paginada de {{info.DisplayName}}.
/// </summary>
public sealed record Get{{info.PluralName}}PagedQuery(PagedRequest Request)
    : IRequest<Result<PagedResult<{{info.EntityName}}Dto>>>;

/// <summary>
/// Handler da query de listagem paginada.
/// </summary>
public sealed class Get{{info.PluralName}}PagedHandler
    : IRequestHandler<Get{{info.PluralName}}PagedQuery, Result<PagedResult<{{info.EntityName}}Dto>>>
{
    private readonly I{{info.EntityName}}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Get{{info.PluralName}}PagedHandler> _logger;{{currentUserField}}

    public Get{{info.PluralName}}PagedHandler(
        I{{info.EntityName}}Repository repository,
        IMapper mapper,
        ILogger<Get{{info.PluralName}}PagedHandler> logger{{currentUserParam}})
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;{{currentUserAssign}}
    }

    public async Task<Result<PagedResult<{{info.EntityName}}Dto>>> Handle(
        Get{{info.PluralName}}PagedQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = query.Request;
            _logger.LogDebug(
                "Buscando {{info.DisplayName}} - Página {Page}, Tamanho {Size}, Busca '{Search}'",
                request.Page,
                request.PageSize,
                request.Search);

{{tenantFilter}}

            // Aplica filtro de busca
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
{{searchFilter}}
            }

            // Conta total
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Aplica paginação
            var items = await queryable
                .OrderBy(e => e.{{info.PrimaryKeyProperty}})
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Mapeia para DTO
            var dtos = _mapper.Map<List<{{info.EntityName}}Dto>>(items);

            var result = new PagedResult<{{info.EntityName}}Dto>(
                dtos,
                totalCount,
                request.Page,
                request.PageSize);

            return Result<PagedResult<{{info.EntityName}}Dto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {{info.DisplayName}} paginado");
            return Result<PagedResult<{{info.EntityName}}Dto>>.Failure(
                Error.Failure("{{info.EntityName}}.Error", $"Erro ao buscar {{info.DisplayName}}: {ex.Message}"));
        }
    }
}
""";
    }

    // =========================================================================
    // MÉTODOS AUXILIARES
    // =========================================================================

    private static string GenerateTenantValidationForQuery(EntityInfo info)
    {
        if (info.IsLegacyTable)
            return "            // Tabela legada: sem validação de tenant";

        return $@"            // ✅ OBRIGATÓRIO: Valida se pertence ao tenant do usuário
            var tenantId = _currentUser.TenantId;
            if (entity.TenantId != tenantId)
            {{
                _logger.LogWarning(
                    ""Tentativa de acesso cross-tenant: Usuário {{UserId}} (Tenant {{UserTenant}}) tentou acessar registro {{Id}} (Tenant {{RecordTenant}})"",
                    _currentUser.UserId,
                    tenantId,
                    query.Id,
                    entity.TenantId);
                
                return Result<{info.EntityName}Dto>.Failure(
                    Error.Forbidden(""{info.EntityName}.Forbidden"", ""Acesso negado ao recurso""));
            }}";
    }

    private static string GenerateTenantFilterForPaged(EntityInfo info)
    {
        if (info.IsLegacyTable)
        {
            return @"            // Query base
            var queryable = _repository.Query();";
        }

        return $@"            // ✅ OBRIGATÓRIO: Filtra por TenantId
            var tenantId = _currentUser.TenantId;
            if (tenantId == Guid.Empty)
            {{
                return Result<PagedResult<{info.EntityName}Dto>>.Failure(
                    Error.Unauthorized(""User.TenantNotFound"", ""TenantId não encontrado no contexto do usuário""));
            }}
            
            // Query base com filtro de tenant
            var queryable = _repository.Query()
                .Where(e => e.TenantId == tenantId);";
    }
}