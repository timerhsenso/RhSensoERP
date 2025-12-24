// =============================================================================
// GENERATOR CONTROLLER - Serve a página do Wizard
// =============================================================================

using Microsoft.AspNetCore.Mvc;

namespace GeradorEntidades.Controllers;

/// <summary>
/// Controller que serve a página do Wizard de geração.
/// </summary>
public class GeneratorController : Controller
{
    private readonly ILogger<GeneratorController> _logger;

    public GeneratorController(ILogger<GeneratorController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Página principal do Wizard.
    /// Acesse via: https://localhost:xxxx/Generator
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Acessando Wizard de Geração");
        return View();
    }
}
