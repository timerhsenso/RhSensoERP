// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 16:27:03
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Attributes;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.TabelasCompartilhadas.TabelaAuxiliar;
using RhSensoERP.Web.Services.TabelasCompartilhadas.TabelaAuxiliar;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers.TabelasCompartilhadas;

// =============================================================================
// MENU ITEM - Configuração para aparecer no menu dinâmico
// =============================================================================
[MenuItem(
    Module = MenuModule.TabelasCompartilhadas,
    DisplayName = "Tabela Auxiliar",
    Icon = "fas fa-table",
    Order = 10,
    CdFuncao = "SEG_FM_TSISTEMA"
)]

/// <summary>
/// Controller para gerenciamento de Tabela Auxiliar.
/// Herda toda a funcionalidade CRUD de BaseCrudController.
/// </summary>
[Authorize]
public class TabelaAuxiliarController 
    : BaseCrudController<TabelaAuxiliarDto, CreateTabelaAuxiliarRequest, UpdateTabelaAuxiliarRequest, string>
{
    // =========================================================================
    // CONFIGURAÇÃO DE PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código da função/tela no sistema de permissões.
    /// Corresponde ao cadastrado na tabela tfunc1 do banco legado.
    /// </summary>
    private const string CdFuncao = "SEG_FM_TSISTEMA";

    /// <summary>
    /// Código do sistema ao qual esta função pertence.
    /// </summary>
    private const string CdSistema = "SEG";

    private readonly ITabelaAuxiliarApiService _tabelaauxiliarService;

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public TabelaAuxiliarController(
        ITabelaAuxiliarApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<TabelaAuxiliarController> logger)
        : base(apiService, permissionsCache, logger)
    {
        _tabelaauxiliarService = apiService;
    }

    // =========================================================================
    // ACTION: INDEX (Página Principal)
    // =========================================================================

    /// <summary>
    /// Página principal com verificação de permissão de consulta.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        if (!await CanViewAsync(CdFuncao, ct))
        {
            _logger.LogWarning(
                "Acesso negado: Usuário {User} tentou acessar {Funcao} sem permissão",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction("AccessDenied", "Account");
        }

        var permissions = await GetUserPermissionsAsync(CdFuncao, ct);

        var viewModel = new TabelaAuxiliarListViewModel
        {
            UserPermissions = permissions
        };

        _logger.LogDebug(
            "Usuário {User} acessou {Funcao} | Permissões: {Permissions}",
            User.Identity?.Name,
            CdFuncao,
            permissions);

        return View("~/Views/TabelasCompartilhadas/TabelaAuxiliar/Index.cshtml", viewModel);
    }

    // =========================================================================
    // ACTION: GET BY ID (Sobrescrito para verificar permissão)
    // =========================================================================

    [HttpGet]
    public override async Task<IActionResult> GetById(string id)
    {
        if (!await CanViewAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para visualizar registros.");
        }

        return await base.GetById(id);
    }

    // =========================================================================
    // ACTION: CREATE
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] CreateTabelaAuxiliarRequest dto)
    {
        if (!await CanCreateAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de inclusão negada: Usuário {User} sem permissão 'I' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para criar registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} criando registro em {Funcao}",
            User.Identity?.Name,
            CdFuncao);

        return await base.Create(dto);
    }

    // =========================================================================
    // ACTION: EDIT
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] string id, [FromBody] UpdateTabelaAuxiliarRequest dto)
    {
        if (EqualityComparer<string>.Default.Equals(id, default))
        {
            return JsonError("ID do registro não informado.");
        }

        if (!await CanEditAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de alteração negada: Usuário {User} sem permissão 'A' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} alterando registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: UPDATE
    // =========================================================================

    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update(string id, [FromBody] UpdateTabelaAuxiliarRequest dto)
    {
        if (!await CanEditAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: DELETE
    // =========================================================================

    [HttpPost]
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(string id)
    {
        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de exclusão negada: Usuário {User} sem permissão 'E' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} excluindo registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Delete(id);
    }

    // =========================================================================
    // ACTION: DELETE MULTIPLE
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<string> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return JsonError("Nenhum registro selecionado para exclusão.");
        }

        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de exclusão múltipla negada: Usuário {User} sem permissão 'E' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} excluindo {Count} registros em {Funcao}",
            User.Identity?.Name,
            ids.Count,
            CdFuncao);

        return await base.DeleteMultiple(ids);
    }


}
