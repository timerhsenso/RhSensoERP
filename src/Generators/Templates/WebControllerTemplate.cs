// =============================================================================
// RHSENSOERP GENERATOR v3.0 - WEB CONTROLLER TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Web Controller (MVC).
/// NOVO no v3.0!
/// </summary>
public static class WebControllerTemplate
{
    /// <summary>
    /// Gera o Web Controller completo com verificação de permissões.
    /// </summary>
    public static string GenerateController(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RhSensoERP.Web.Controllers.Base;
using {{info.WebModelsNamespace}};
using RhSensoERP.Web.Services.Permissions;
using {{info.WebServicesNamespace}};

namespace {{info.WebControllerNamespace}};

/// <summary>
/// Controller MVC para gerenciamento de {{info.DisplayName}}.
/// Herda toda a funcionalidade CRUD do BaseCrudController com verificação de permissões.
/// </summary>
[Authorize]
public class {{info.PluralName}}Controller : BaseCrudController<{{info.EntityName}}Dto, Create{{info.EntityName}}Dto, Update{{info.EntityName}}Dto, {{pkType}}>
{
    // =========================================================================
    // CONFIGURAÇÃO DE PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código da função/tela no sistema de permissões.
    /// Este código deve corresponder ao cadastrado na tabela tfunc1 do banco legado.
    /// </summary>
    private const string CdFuncao = "{{info.CdFuncao}}";

    /// <summary>
    /// Código do sistema ao qual esta função pertence.
    /// </summary>
    private const string CdSistema = "{{info.CdSistema}}";

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public {{info.PluralName}}Controller(
        I{{info.EntityName}}ApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<{{info.PluralName}}Controller> logger)
        : base(apiService, permissionsCache, logger)
    {
    }

    // =========================================================================
    // ACTION: INDEX (Página Principal)
    // =========================================================================

    /// <summary>
    /// Página principal (Index) com verificação de permissão de consulta.
    /// Valida se o usuário tem permissão de CONSULTAR (C) esta função.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Verifica a permissão de consulta ANTES de renderizar a página
        if (!await CanViewAsync(CdFuncao, ct))
        {
            _logger.LogWarning(
                "⛔ Acesso negado: Usuário {User} tentou acessar {Funcao} sem permissão de consulta",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction("AccessDenied", "Account");
        }

        // Busca as permissões específicas do usuário para esta função
        var permissions = await GetUserPermissionsAsync(CdFuncao, ct);

        var viewModel = new {{info.PluralName}}ListViewModel
        {
            UserPermissions = permissions
        };

        _logger.LogInformation(
            "✅ Usuário {User} acessou {Funcao} | Permissões: I={CanCreate}, A={CanEdit}, E={CanDelete}, C={CanView}",
            User.Identity?.Name,
            CdFuncao,
            viewModel.CanCreate,
            viewModel.CanEdit,
            viewModel.CanDelete,
            viewModel.CanView);

        return View(viewModel);
    }

    // =========================================================================
    // ACTION: CREATE (Incluir)
    // =========================================================================

    /// <summary>
    /// Cria um novo registro.
    /// Valida se o usuário tem permissão de INCLUIR (I) nesta função.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] Create{{info.EntityName}}Dto dto)
    {
        if (!await CanCreateAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de inclusão negada: Usuário {User} não tem permissão 'I' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para criar registros nesta tela.");
        }

        _logger.LogInformation(
            "➕ Usuário {User} está criando um novo registro em {Funcao}",
            User.Identity?.Name,
            CdFuncao);

        return await base.Create(dto);
    }

    // =========================================================================
    // ACTION: EDIT (Alterar via POST - compatibilidade com CrudBase.js)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via POST.
    /// Esta action é necessária para compatibilidade com o CrudBase.js que faz POST para /Edit.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] {{pkType}} id, [FromBody] Update{{info.EntityName}}Dto dto)
    {
        {{GenerateIdValidation(info)}}

        if (!await CanEditAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de alteração negada: Usuário {User} não tem permissão 'A' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        _logger.LogInformation(
            "✏️ Usuário {User} está alterando registro {Id} em {Funcao} (via Edit POST)",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: UPDATE (Alterar via PUT - padrão REST)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via PUT (padrão REST).
    /// </summary>
    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update({{pkType}} id, [FromBody] Update{{info.EntityName}}Dto dto)
    {
        if (!await CanEditAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de alteração negada: Usuário {User} não tem permissão 'A' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        _logger.LogInformation(
            "✏️ Usuário {User} está alterando registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: DELETE (Excluir)
    // =========================================================================

    /// <summary>
    /// Exclui um registro.
    /// </summary>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete({{pkType}} id)
    {
        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de exclusão negada: Usuário {User} não tem permissão 'E' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "🗑️ Usuário {User} está excluindo registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Delete(id);
    }

    // =========================================================================
    // ACTION: DELETE MULTIPLE (Excluir Múltiplos)
    // =========================================================================

    /// <summary>
    /// Exclui múltiplos registros de uma vez.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<{{pkType}}>? ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return JsonError("Nenhum registro selecionado para exclusão.");
        }

        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de exclusão múltipla negada: Usuário {User} não tem permissão 'E' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "🗑️ Usuário {User} está excluindo {Count} registros em {Funcao}",
            User.Identity?.Name,
            ids.Count,
            CdFuncao);

        return await base.DeleteMultiple(ids);
    }
}
""";
    }

    /// <summary>
    /// Gera a validação de ID baseada no tipo.
    /// </summary>
    private static string GenerateIdValidation(EntityInfo info)
    {
        return info.PrimaryKeyType switch
        {
            "string" => """
        if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("⛔ Tentativa de edição sem ID informado");
                    return JsonError("ID do registro não informado.");
                }
""",
            "Guid" => """
        if (id == Guid.Empty)
                {
                    _logger.LogWarning("⛔ Tentativa de edição sem ID informado");
                    return JsonError("ID do registro não informado.");
                }
""",
            _ => """
        if (id == default)
                {
                    _logger.LogWarning("⛔ Tentativa de edição sem ID informado");
                    return JsonError("ID do registro não informado.");
                }
"""
        };
    }
}
