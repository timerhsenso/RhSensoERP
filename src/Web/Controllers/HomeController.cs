using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Models;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller principal.
/// </summary>
[Authorize] // ✅ Protege todas as actions deste controller
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Página inicial (requer autenticação).
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Usuário autenticado acessando página inicial: {User}", User.Identity?.Name);
        return View();
    }

    /// <summary>
    /// Página de privacidade.
    /// </summary>
    public IActionResult Privacy()
    {
        _logger.LogInformation("Acessando página de privacidade");
        return View();
    }

    /// <summary>
    /// Página de erro.
    /// </summary>
    [AllowAnonymous] // Permite acesso sem autenticação
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Erro na aplicação");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}