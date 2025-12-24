// =============================================================================
// RHSENSOERP WEB - SCREEN CONTROLLER
// =============================================================================
// Arquivo: src/Web/Controllers/ScreenController.cs
// Descrição: Resolve URLs mascaradas (tokens) para rotas reais
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Security;
using RhSensoERP.Web.Services.Menu;

namespace RhSensoERP.Web.Controllers;

[Authorize]
public sealed class ScreenController : Controller
{
    private readonly IScreenLinkService _links;
    private readonly IMenuDiscoveryService _menu;
    private readonly ILogger<ScreenController> _logger;

    public ScreenController(
        IScreenLinkService links,
        IMenuDiscoveryService menu,
        ILogger<ScreenController> logger)
    {
        _links = links;
        _menu = menu;
        _logger = logger;
    }

    /// <summary>
    /// URL mascarada: /s/{token}
    /// O token é uma ScreenKey criptografada via DataProtection.
    /// Permite compartilhar links entre usuários sem expor rotas internas.
    /// </summary>
    [HttpGet("/s/{token}")]
    public async Task<IActionResult> Go(string token, CancellationToken ct)
    {
        string screenKey;

        try
        {
            screenKey = _links.Unprotect(token);
            _logger.LogDebug("[SCREEN] Token descriptografado | ScreenKey: {ScreenKey}", screenKey);
        }
        catch (Exception ex)
        {
            // Token inválido / expirado / de outro deploy com chaves diferentes
            _logger.LogWarning(ex, "[SCREEN] Falha ao descriptografar token: {Token}", token);
            return RedirectToAction("AccessDenied", "Account");
        }

        // Resolve destino a partir do menu descoberto (controllers com [MenuItem])
        var target = await _menu.ResolveByScreenKeyAsync(screenKey, ct);

        if (target is null)
        {
            _logger.LogWarning("[SCREEN] ScreenKey não encontrada: {ScreenKey}", screenKey);
            return RedirectToAction("AccessDenied", "Account");
        }

        _logger.LogDebug(
            "[SCREEN] Redirecionando | ScreenKey: {ScreenKey} | Area: {Area} | Controller: {Controller} | Action: {Action}",
            screenKey, target.Area, target.Controller, target.Action);

        // Redireciona para rota REAL (não mascarada)
        return RedirectToAction(
            actionName: target.Action,
            controllerName: target.Controller,
            routeValues: string.IsNullOrWhiteSpace(target.Area)
                ? null
                : new { area = target.Area }
        );
    }
}