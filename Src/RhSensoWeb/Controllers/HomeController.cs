using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoWeb.Extensions;
using RhSensoWeb.Models;

namespace RhSensoWeb.Controllers;

/// <summary>
/// Controller principal da aplicação
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Dashboard principal
    /// </summary>
    public IActionResult Index()
    {
        var userSession = User.GetUserSession();
        
        _logger.LogInformation("Dashboard acessado por usuário {Usuario}", userSession.CdUsuario);
        
        ViewBag.UserName = userSession.DcUsuario;
        ViewBag.UserEmail = userSession.EmailUsuario;
        ViewBag.CompanyCode = userSession.CdEmpresa;
        ViewBag.BranchCode = userSession.CdFilial;
        
        return View();
    }

    /// <summary>
    /// Página de privacidade
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Página de erro
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Endpoint para obter informações do usuário (AJAX)
    /// </summary>
    [HttpGet]
    public IActionResult GetUserInfo()
    {
        try
        {
            var userSession = User.GetUserSession();
            
            return Json(new
            {
                success = true,
                data = new
                {
                    cdUsuario = userSession.CdUsuario,
                    dcUsuario = userSession.DcUsuario,
                    emailUsuario = userSession.EmailUsuario,
                    tpUsuario = userSession.TpUsuario,
                    cdEmpresa = userSession.CdEmpresa,
                    cdFilial = userSession.CdFilial,
                    loginTime = userSession.LoginTime,
                    lastActivity = userSession.LastActivity,
                    permissions = userSession.Permissions.Select(p => p.PermissionKey).ToList(),
                    groups = userSession.Groups.Select(g => g.CdGrUser).ToList()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário");
            return Json(new { success = false, message = "Erro interno" });
        }
    }
}
