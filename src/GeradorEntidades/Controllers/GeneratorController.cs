// =============================================================================
// GENERATOR CONTROLLER - Serve a página do Wizard
// v2.0 - ADICIONADO: Generator2 (gerador aprimorado)
// =============================================================================

using Microsoft.AspNetCore.Mvc;

namespace GeradorEntidades.Controllers;

/// <summary>
/// Controller que serve as páginas do Wizard de geração.
/// </summary>
public class GeneratorController : Controller
{
    private readonly ILogger<GeneratorController> _logger;

    public GeneratorController(ILogger<GeneratorController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Página principal do Wizard (v1 - original).
    /// Acesse via: https://localhost:xxxx/Generator
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Acessando Wizard de Geração v1");
        return View();
    }

    /// <summary>
    /// ✅ NOVO: Gerador v2.0 (interface aprimorada com carregamento correto do JSON).
    /// Acesse via: https://localhost:xxxx/Generator/Generator2
    /// </summary>
    [HttpGet("Generator2")]
    public IActionResult Generator2()
    {
        _logger.LogInformation("Acessando Gerador v2.0");
        return View();
    }
}