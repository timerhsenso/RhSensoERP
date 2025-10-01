using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoWeb.Attributes;
using RhSensoWeb.Extensions;
using RhSensoWeb.Models.SEG;
using RhSensoWeb.Models.Shared;
using RhSensoWeb.Services.Interfaces;

namespace RhSensoWeb.Areas.SEG.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários
/// </summary>
[Area("SEG")]
[Authorize]
public class UsuariosController : Controller
{
    private readonly IUsuarioApiService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioApiService usuarioService,
        ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Lista de usuários
    /// </summary>
    [HttpGet]
    [Permission("SEG.SEG_USUARIOS.C")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Usuários";
        ViewData["Subtitle"] = "Gerenciamento de usuários do sistema";
        ViewData["Icon"] = "fas fa-users";

        return View();
    }

    /// <summary>
    /// Dados para DataTable (AJAX)
    /// </summary>
    [HttpPost]
    [Permission("SEG.SEG_USUARIOS.C")]
    public async Task<IActionResult> GetData([FromBody] DataTablesRequest request)
    {
        try
        {
            // Obter filtros da query string
            var filtros = new UsuarioFiltroDto
            {
                Nome = Request.Query["nome"],
                Email = Request.Query["email"],
                Tipo = Request.Query["tipo"],
                Status = Request.Query["status"].ToString().FirstOrDefault(),
                Empresa = Request.Query["empresa"],
                Filial = Request.Query["filial"],
                Grupo = Request.Query["grupo"]
            };

            var result = await _usuarioService.GetUsuariosAsync(request, filtros);
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados dos usuários");
            
            return Json(new DataTablesResponse<UsuarioListDto>
            {
                Draw = request.Draw,
                RecordsTotal = 0,
                RecordsFiltered = 0,
                Data = new List<UsuarioListDto>(),
                Error = "Erro interno do servidor"
            });
        }
    }

    /// <summary>
    /// Formulário de criação
    /// </summary>
    [HttpGet]
    [Permission("SEG.SEG_USUARIOS.I")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Novo Usuário";
        ViewData["Subtitle"] = "Cadastrar novo usuário no sistema";
        ViewData["Icon"] = "fas fa-user-plus";

        var model = new UsuarioViewModel();
        await CarregarDadosFormulario();
        
        return View(model);
    }

    /// <summary>
    /// Processar criação
    /// </summary>
    [HttpPost]
    [Permission("SEG.SEG_USUARIOS.I")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await CarregarDadosFormulario();
                return View(model);
            }

            var result = await _usuarioService.CreateUsuarioAsync(model);
            
            if (result.Success)
            {
                TempData["Success"] = "Usuário criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result.Message);
            await CarregarDadosFormulario();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário");
            ModelState.AddModelError("", "Erro interno do sistema");
            await CarregarDadosFormulario();
            return View(model);
        }
    }

    /// <summary>
    /// Formulário de edição
    /// </summary>
    [HttpGet]
    [Permission("SEG.SEG_USUARIOS.A")]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Usuário não encontrado";
                return RedirectToAction(nameof(Index));
            }

            var result = await _usuarioService.GetUsuarioByIdAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Usuário não encontrado";
                return RedirectToAction(nameof(Index));
            }

            var model = new UsuarioViewModel
            {
                CdUsuario = result.Data.CdUsuario,
                DcUsuario = result.Data.DcUsuario,
                EmailUsuario = result.Data.EmailUsuario,
                TpUsuario = result.Data.TpUsuario,
                FlAtivo = result.Data.FlAtivo,
                CdEmpresa = result.Data.CdEmpresa,
                CdFilial = result.Data.CdFilial,
                IdSaas = result.Data.IdSaas,
                Observacoes = result.Data.Observacoes,
                GruposSelecionados = result.Data.Grupos
            };

            ViewData["Title"] = "Editar Usuário";
            ViewData["Subtitle"] = $"Editando: {result.Data.DcUsuario}";
            ViewData["Icon"] = "fas fa-user-edit";

            await CarregarDadosFormulario();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar usuário para edição: {Id}", id);
            TempData["Error"] = "Erro interno do sistema";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Processar edição
    /// </summary>
    [HttpPost]
    [Permission("SEG.SEG_USUARIOS.A")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UsuarioViewModel model)
    {
        try
        {
            if (id != model.CdUsuario)
            {
                TempData["Error"] = "Dados inconsistentes";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await CarregarDadosFormulario();
                return View(model);
            }

            var result = await _usuarioService.UpdateUsuarioAsync(id, model);
            
            if (result.Success)
            {
                TempData["Success"] = "Usuário atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result.Message);
            await CarregarDadosFormulario();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: {Id}", id);
            ModelState.AddModelError("", "Erro interno do sistema");
            await CarregarDadosFormulario();
            return View(model);
        }
    }

    /// <summary>
    /// Visualizar detalhes
    /// </summary>
    [HttpGet]
    [Permission("SEG.SEG_USUARIOS.C")]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Usuário não encontrado";
                return RedirectToAction(nameof(Index));
            }

            var result = await _usuarioService.GetUsuarioByIdAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Usuário não encontrado";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = "Detalhes do Usuário";
            ViewData["Subtitle"] = result.Data.DcUsuario;
            ViewData["Icon"] = "fas fa-user";

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar detalhes do usuário: {Id}", id);
            TempData["Error"] = "Erro interno do sistema";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Excluir usuário
    /// </summary>
    [HttpPost]
    [Permission("SEG.SEG_USUARIOS.E")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Usuário não encontrado" });
            }

            var result = await _usuarioService.DeleteUsuarioAsync(id);
            
            if (result.Success)
            {
                return Json(new { success = true, message = "Usuário excluído com sucesso!" });
            }

            return Json(new { success = false, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir usuário: {Id}", id);
            return Json(new { success = false, message = "Erro interno do sistema" });
        }
    }

    /// <summary>
    /// Alterar status do usuário
    /// </summary>
    [HttpPost]
    [Permission("SEG.SEG_USUARIOS.A")]
    public async Task<IActionResult> ToggleStatus(string id, bool ativo)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Usuário não encontrado" });
            }

            var result = await _usuarioService.ToggleUsuarioStatusAsync(id, ativo);
            
            if (result.Success)
            {
                var status = ativo ? "ativado" : "desativado";
                return Json(new { success = true, message = $"Usuário {status} com sucesso!" });
            }

            return Json(new { success = false, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do usuário: {Id}", id);
            return Json(new { success = false, message = "Erro interno do sistema" });
        }
    }

    /// <summary>
    /// Alterar senha do usuário
    /// </summary>
    [HttpPost]
    [Permission("SEG.SEG_USUARIOS.A")]
    public async Task<IActionResult> AlterarSenha(string id, string novaSenha)
    {
        try
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(novaSenha))
            {
                return Json(new { success = false, message = "Dados inválidos" });
            }

            if (novaSenha.Length < 6)
            {
                return Json(new { success = false, message = "A senha deve ter pelo menos 6 caracteres" });
            }

            var result = await _usuarioService.AlterarSenhaAsync(id, novaSenha);
            
            if (result.Success)
            {
                return Json(new { success = true, message = "Senha alterada com sucesso!" });
            }

            return Json(new { success = false, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar senha do usuário: {Id}", id);
            return Json(new { success = false, message = "Erro interno do sistema" });
        }
    }

    /// <summary>
    /// Obter filiais por empresa (AJAX)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetFiliais(string cdEmpresa)
    {
        try
        {
            if (string.IsNullOrEmpty(cdEmpresa))
            {
                return Json(new List<FilialDto>());
            }

            var result = await _usuarioService.GetFiliaisAsync(cdEmpresa);
            
            if (result.Success && result.Data != null)
            {
                return Json(result.Data);
            }

            return Json(new List<FilialDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter filiais da empresa: {CdEmpresa}", cdEmpresa);
            return Json(new List<FilialDto>());
        }
    }

    /// <summary>
    /// Carregar dados para formulários
    /// </summary>
    private async Task CarregarDadosFormulario()
    {
        try
        {
            // Carregar grupos
            var gruposResult = await _usuarioService.GetGruposAsync();
            ViewBag.Grupos = gruposResult.Success && gruposResult.Data != null 
                ? gruposResult.Data 
                : new List<GrupoDto>();

            // Carregar empresas
            var empresasResult = await _usuarioService.GetEmpresasAsync();
            ViewBag.Empresas = empresasResult.Success && empresasResult.Data != null 
                ? empresasResult.Data 
                : new List<EmpresaDto>();

            // Tipos de usuário
            ViewBag.TiposUsuario = new List<object>
            {
                new { Value = "ADMIN", Text = "Administrador" },
                new { Value = "USER", Text = "Usuário" },
                new { Value = "GUEST", Text = "Convidado" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar dados do formulário");
            ViewBag.Grupos = new List<GrupoDto>();
            ViewBag.Empresas = new List<EmpresaDto>();
            ViewBag.TiposUsuario = new List<object>();
        }
    }
}
