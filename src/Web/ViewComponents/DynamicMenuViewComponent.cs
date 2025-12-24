// =============================================================================
// RHSENSOERP WEB - DYNAMIC MENU VIEW COMPONENT
// =============================================================================
// Arquivo: src/Web/Views/Shared/Components/DynamicMenu/DynamicMenuViewComponent.cs
// Descrição: ViewComponent para renderizar o menu dinâmico
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Services.Menu;

namespace RhSensoERP.Web.ViewComponents;

public class DynamicMenuViewComponent : ViewComponent
{
    private readonly IMenuDiscoveryService _menuService;
    private readonly ILogger<DynamicMenuViewComponent> _logger;

    public DynamicMenuViewComponent(
        IMenuDiscoveryService menuService,
        ILogger<DynamicMenuViewComponent> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var username = HttpContext.User?.Identity?.Name;

            _logger.LogDebug(
                "[DynamicMenu] Invocando | User: {User} | IsAuth: {IsAuth}",
                username,
                HttpContext.User?.Identity?.IsAuthenticated);

            var menu = await _menuService.GetMenuAsync(username);

            _logger.LogDebug(
                "[DynamicMenu] Menu obtido | Modules: {Modules} | Items: {Items}",
                menu.Count,
                menu.Sum(m => m.Items.Count));

            return View(menu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[DynamicMenu] Erro ao obter menu");
            return View(Array.Empty<MenuModuleViewModel>());
        }
    }
}