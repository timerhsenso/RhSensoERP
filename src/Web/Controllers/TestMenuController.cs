// =============================================================================
// RHSENSOERP WEB - CONTROLLER DE TESTE PARA MENU
// =============================================================================
// Arquivo: src/Web/Controllers/TestMenuController.cs
// Descrição: Controller temporário para testar o menu dinâmico
// REMOVA APÓS OS TESTES!
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Attributes;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller de teste para validar que o menu dinâmico está funcionando.
/// REMOVA ESTE ARQUIVO APÓS OS TESTES!
/// </summary>
[Authorize]
[MenuItem(
    Module = MenuModule.Seguranca,
    DisplayName = "🧪 Teste Menu",
    Icon = "fas fa-vial",
    Order = 1,
    Description = "Item de teste para validar menu dinâmico"
)]
public class TestMenuController : Controller
{
    public IActionResult Index()
    {
        return Content("✅ Menu funcionando! Este é um controller de teste. Remova após validar.");
    }
}

/// <summary>
/// Outro controller de teste em módulo diferente.
/// </summary>
[Authorize]
[MenuItem(
    Module = MenuModule.GestaoDePessoas,
    DisplayName = "🧪 Teste RHU",
    Icon = "fas fa-flask",
    Order = 1
)]
public class TestRhuController : Controller
{
    public IActionResult Index()
    {
        return Content("✅ Controller de Gestão de Pessoas funcionando!");
    }
}