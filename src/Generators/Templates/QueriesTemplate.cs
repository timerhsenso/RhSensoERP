// =============================================================================
// RHSENSOERP GENERATOR v4.0 - QUERIES TEMPLATE (FINAL WORKING)
// =============================================================================
// Arquivo: src/Generators/Templates/QueriesTemplate.cs
// Versão: 4.0 - FINAL - Usa SortBy/Desc da classe PagedRequest
// Changelog:
//   v4.0 - ✅ CORRIGIDO: Usa SortBy (não OrderBy) e Desc (não Ascending)
//   v4.0 - ✅ Ordenação dinâmica ASC/DESC completa
//   v4.0 - ✅ Zero dependências externas
//   v3.7 - Multi-tenancy com filtro automático de tenant
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
{{info.FileHeader}}
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
    /// Gera a Query GetPaged com filtro de tenant e ordenação dinâmica.
    /// v4.0: Ordenação dinâmica ASC/DESC usando SortBy e Desc.
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

        // ✅ v4.0: Gera casos do switch para ordenação ASC
        var orderByAscCases = GenerateOrderByCases(info, ascending: true);

        // ✅ v4.0: Gera casos do switch para ordenação DESC
        var orderByDescCases = GenerateOrderByCases(info, ascending: false);

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using System.Linq;
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
/// v4.0: Suporte a ordenação dinâmica via SortBy/Desc.
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
                "Buscando {{info.DisplayName}} - Página {Page}, Tamanho {Size}, Busca '{Search}', SortBy: {SortBy}, Desc: {Desc}",
                request.Page,
                request.PageSize,
                request.Search,
                request.SortBy ?? "null",
                request.Desc);

{{tenantFilter}}

            // Aplica filtro de busca
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
{{searchFilter}}
            }

            // Conta total
            var totalCount = await queryable.CountAsync(cancellationToken);

            // =========================================================================
            // ✅ v4.0: ORDENAÇÃO DINÂMICA ASC/DESC
            // =========================================================================
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                if (request.Desc)
                {
                    // Ordenação DESCENDENTE
                    switch (request.SortBy)
                    {
{{orderByDescCases}}
                        default:
                            queryable = queryable.OrderByDescending(e => e.{{info.PrimaryKeyProperty}});
                            break;
                    }
                    
                    _logger.LogDebug("✅ Ordenando por: {SortBy} DESC", request.SortBy);
                }
                else
                {
                    // Ordenação ASCENDENTE
                    switch (request.SortBy)
                    {
{{orderByAscCases}}
                        default:
                            queryable = queryable.OrderBy(e => e.{{info.PrimaryKeyProperty}});
                            break;
                    }
                    
                    _logger.LogDebug("✅ Ordenando por: {SortBy} ASC", request.SortBy);
                }
            }
            else
            {
                // Ordenação padrão (ASC por PK)
                queryable = queryable.OrderBy(e => e.{{info.PrimaryKeyProperty}});
            }

            // Aplica paginação
            var items = await queryable
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

    /// <summary>
    /// v4.0: Gera casos do switch para ordenação ASC ou DESC.
    /// </summary>
    private static string GenerateOrderByCases(EntityInfo info, bool ascending)
    {
        var cases = new List<string>();
        var orderMethod = ascending ? "OrderBy" : "OrderByDescending";

        // Propriedades (exceto PK que é o default)
        foreach (var prop in info.Properties.Where(p => !p.IsNavigation && p.Name != info.PrimaryKeyProperty))
        {
            cases.Add($@"                        case ""{prop.Name}"":
                            queryable = queryable.{orderMethod}(e => e.{prop.Name});
                            break;");
        }

        return string.Join("\n", cases);
    }

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