// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TipoParentesco
// Module: Shared
// Data: 2026-03-02 17:59:26
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Attributes;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Shared.TipoParentesco;
using RhSensoERP.Web.Services.Shared.TipoParentesco;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers.Shared;

// =============================================================================
// MENU ITEM - Configuração para aparecer no menu dinâmico
// =============================================================================
[MenuItem(
    Module = MenuModule.TabelasCompartilhadas,
    DisplayName = "Tipo de Parentesco",
    Icon = "fas fa-table",
    Order = 10,
    CdFuncao = "SEG_FM_TSISTEMA"
)]

/// <summary>
/// Controller para gerenciamento de Tipo de Parentesco.
/// Herda toda a funcionalidade CRUD de BaseCrudController.
/// </summary>
[Authorize]
public class TipoParentescoController 
    : BaseCrudController<TipoParentescoDto, CreateTipoParentescoRequest, UpdateTipoParentescoRequest, byte>
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
    private const string CdSistema = "SGT";

    private readonly ITipoParentescoApiService _tipoparentescoService;

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public TipoParentescoController(
        ITipoParentescoApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<TipoParentescoController> logger)
        : base(apiService, permissionsCache, logger)
    {
        _tipoparentescoService = apiService;
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

        var viewModel = new TipoParentescoListViewModel
        {
            UserPermissions = permissions
        };

        _logger.LogDebug(
            "Usuário {User} acessou {Funcao} | Permissões: {Permissions}",
            User.Identity?.Name,
            CdFuncao,
            permissions);

        return View("~/Views/Shared/TipoParentesco/Index.cshtml", viewModel);
    }

    // =========================================================================
    // ACTION: GET BY ID (Sobrescrito para verificar permissão)
    // =========================================================================

    [HttpGet]
    public override async Task<IActionResult> GetById(byte id)
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
    public override async Task<IActionResult> Create([FromBody] CreateTipoParentescoRequest dto)
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
    public async Task<IActionResult> Edit([FromQuery] byte id, [FromBody] UpdateTipoParentescoRequest dto)
    {
        if (EqualityComparer<byte>.Default.Equals(id, default))
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
    public override async Task<IActionResult> Update(byte id, [FromBody] UpdateTipoParentescoRequest dto)
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
    public override async Task<IActionResult> Delete(byte id)
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
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<byte> ids)
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


    // =========================================================================
    // v3.7: ACTION - TOGGLE ATIVO (Alternar Status Ativo/Desativo)
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAtivo([FromBody] ToggleAtivoRequest request, CancellationToken ct = default)
    {
        if (request == null)
        {
            return JsonError("Requisição inválida.");
        }

        if (!await CanEditAsync(CdFuncao, ct))
        {
            _logger.LogWarning(
                "Tentativa de alteração de status negada: Usuário {User} sem permissão 'A' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        try
        {
            _logger.LogInformation(
                "Usuário {User} alterando status Ativo do registro {Id} para {Ativo} em {Funcao}",
                User.Identity?.Name,
                request.Id,
                request.Ativo,
                CdFuncao);

            await _tipoparentescoService.ToggleAtivoAsync(request.Id, request.Ativo, ct);

            var mensagem = request.Ativo
                ? "Registro ativado com sucesso!"
                : "Registro desativado com sucesso!";

            return JsonSuccess(mensagem);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Registro {Id} não encontrado ao tentar alterar status Ativo",
                request.Id);

            return JsonError("Registro não encontrado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao alternar status Ativo do registro {Id} em {Funcao}",
                request.Id,
                CdFuncao);

            return JsonError("Erro ao atualizar status do registro.");
        }
    }

    public class ToggleAtivoRequest
    {
        public byte Id { get; set; }
        public bool Ativo { get; set; }
    }

}
