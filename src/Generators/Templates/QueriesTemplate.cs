// =============================================================================
// RHSENSOERP GENERATOR v4.6 - QUERIES TEMPLATE
// =============================================================================
// v4.6: ADICIONADO - Includes automáticos para navegações
// v4.0: Ordenação dinâmica ASC/DESC com SortBy/Desc
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Linq;
using System.Collections.Generic;

namespace RhSensoERP.Generators.Templates;

public static class QueriesTemplate
{
    /// <summary>
    /// Gera a Query GetById com validação de tenant e Includes.
    /// </summary>
    public static string GenerateGetByIdQuery(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var tenantValidation = GenerateTenantValidationForQuery(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        // ✅ v4.6 NOVO: Includes para navegações
        var includesCode = GenerateIncludesForGetById(info);
        var includesUsing = includesCode.Contains("Include") ? "\nusing Microsoft.EntityFrameworkCore;" : "";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;{{includesUsing}}
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

{{includesCode}}

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
    /// Gera a Query GetPaged com filtro de tenant, ordenação dinâmica e Includes.
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

        var tenantFilter = GenerateTenantFilterForPaged(info);
        var currentUserUsing = info.IsLegacyTable ? "" : "\nusing RhSensoERP.Shared.Core.Abstractions;";
        var currentUserField = info.IsLegacyTable ? "" : "\n    private readonly ICurrentUser _currentUser;";
        var currentUserParam = info.IsLegacyTable ? "" : ",\n        ICurrentUser currentUser";
        var currentUserAssign = info.IsLegacyTable ? "" : "\n        _currentUser = currentUser;";

        var orderByAscCases = GenerateOrderByCases(info, ascending: true);
        var orderByDescCases = GenerateOrderByCases(info, ascending: false);

        // ✅ v4.6 NOVO: Includes para navegações
        var includesForPaged = GenerateIncludesForPaged(info);

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
/// v4.6: Inclui navegações automaticamente.
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

{{tenantFilter}}{{includesForPaged}}

            // Aplica filtro de busca
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
{{searchFilter}}
            }

            // Conta total
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Ordenação dinâmica
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                if (request.Desc)
                {
                    switch (request.SortBy)
                    {
{{orderByDescCases}}
                        default:
                            queryable = queryable.OrderByDescending(e => e.{{info.PrimaryKeyProperty}});
                            break;
                    }
                }
                else
                {
                    switch (request.SortBy)
                    {
{{orderByAscCases}}
                        default:
                            queryable = queryable.OrderBy(e => e.{{info.PrimaryKeyProperty}});
                            break;
                    }
                }
            }
            else
            {
                queryable = queryable.OrderBy(e => e.{{info.PrimaryKeyProperty}});
            }

            // Paginação
            var items = await queryable
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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
    // ✅ v4.6 NOVO: INCLUDES AUTOMÁTICOS
    // =========================================================================

    /// <summary>
    /// Gera código para buscar entidade por ID com Includes.
    /// </summary>
    private static string GenerateIncludesForGetById(EntityInfo info)
    {
        var navWithDisplay = info.Navigations
            .Where(n => n.HasNavigationDisplay &&
                       n.RelationshipType == NavigationRelationshipType.ManyToOne)
            .ToList();

        if (!navWithDisplay.Any())
        {
            // Sem navegações - usa método direto
            return $"            var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);";
        }

        // Com navegações - usa Query() + Includes
        var includes = navWithDisplay
            .Select(n => $"                .Include(x => x.{n.Name})")
            .ToList();

        var includesCode = string.Join("\n", includes);

        return $@"            var entity = await _repository.Query()
{includesCode}
                .FirstOrDefaultAsync(x => x.{info.PrimaryKeyProperty} == query.Id, cancellationToken);";
    }

    /// <summary>
    /// Gera Includes para query paginada.
    /// v3.6.3: ADICIONA .AsNoTracking() DEPOIS dos includes
    /// </summary>
    private static string GenerateIncludesForPaged(EntityInfo info)
    {
        var navWithDisplay = info.Navigations
            .Where(n => n.HasNavigationDisplay &&
                       n.RelationshipType == NavigationRelationshipType.ManyToOne)
            .ToList();

        if (!navWithDisplay.Any())
            return "";

        var includes = navWithDisplay
            .Select(n => $"            queryable = queryable.Include(x => x.{n.Name});")
            .ToList();

        var includesCode = string.Join("\n", includes);

        // ✅ v3.6.3: ADICIONA AsNoTracking DEPOIS dos includes
        return $@"
            // ✅ Carrega navegações
{includesCode}
            queryable = queryable.AsNoTracking();
";
    }

    private static string GenerateOrderByCases(EntityInfo info, bool ascending)
    {
        var cases = new List<string>();
        var orderMethod = ascending ? "OrderBy" : "OrderByDescending";

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

        return $@"            // ✅ Valida tenant
            var tenantId = _currentUser.TenantId;
            if (entity.TenantId != tenantId)
            {{
                _logger.LogWarning(
                    ""Acesso cross-tenant negado: Usuário {{UserId}} tentou acessar {{Id}}"",
                    _currentUser.UserId,
                    query.Id);
                
                return Result<{info.EntityName}Dto>.Failure(
                    Error.Forbidden(""{info.EntityName}.Forbidden"", ""Acesso negado""));
            }}";
    }

    private static string GenerateTenantFilterForPaged(EntityInfo info)
    {
        if (info.IsLegacyTable)
        {
            return @"            var queryable = _repository.Query();";
        }

        return $@"            var tenantId = _currentUser.TenantId;
            if (tenantId == Guid.Empty)
            {{
                return Result<PagedResult<{info.EntityName}Dto>>.Failure(
                    Error.Unauthorized(""User.TenantNotFound"", ""TenantId não encontrado""));
            }}
            
            var queryable = _repository.Query()
                .Where(e => e.TenantId == tenantId);";
    }
}