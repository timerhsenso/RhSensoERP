// =============================================================================
// RHSENSOERP WEB - MENU DIAGNOSTIC CONTROLLER
// =============================================================================
// Arquivo: src/Web/Controllers/MenuDiagnosticController.cs
// Descrição: Endpoint para diagnosticar problemas com o menu dinâmico
// REMOVA APÓS OS TESTES OU PROTEJA EM PRODUÇÃO!
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Services.Menu;
using System.Security.Claims;
using System.Text.Json;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller de diagnóstico para o menu dinâmico.
/// Acesse: /MenuDiagnostic/Status
/// </summary>
[Authorize]
[Route("[controller]")]
public class MenuDiagnosticController : Controller
{
    private readonly IMenuDiscoveryService _menuService;
    private readonly ILogger<MenuDiagnosticController> _logger;

    public MenuDiagnosticController(
        IMenuDiscoveryService menuService,
        ILogger<MenuDiagnosticController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna status completo do menu para diagnóstico.
    /// GET /MenuDiagnostic/Status
    /// </summary>
    [HttpGet("Status")]
    public async Task<IActionResult> Status(CancellationToken ct)
    {
        var diagnostic = new
        {
            Timestamp = DateTime.UtcNow,

            // Informações do usuário
            User = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Name = User.Identity?.Name,
                CdUsuario = User.FindFirstValue("cdusuario"),
                NameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            },

            // Status do menu
            Menu = await GetMenuStatusAsync(ct)
        };

        _logger.LogInformation(
            "[DIAGNOSTIC] Status solicitado | User: {User} | IsAuth: {IsAuth}",
            diagnostic.User.Name,
            diagnostic.User.IsAuthenticated);

        return Json(diagnostic, new JsonSerializerOptions { WriteIndented = true });
    }

    private async Task<object> GetMenuStatusAsync(CancellationToken ct)
    {
        try
        {
            var menu = await _menuService.GetMenuAsync(null, ct);

            return new
            {
                Success = true,
                TotalModules = menu.Count,
                TotalItems = menu.Sum(m => m.Items.Count),
                Modules = menu.Select(m => new
                {
                    m.DisplayName,
                    m.Icon,
                    m.CdSistema,
                    ItemCount = m.Items.Count,
                    Items = m.Items.Select(i => new
                    {
                        i.DisplayName,
                        i.Controller,
                        i.Action,
                        i.Area,
                        i.ScreenKey,
                        i.HasPermission,
                        i.IsFavorite,
                        i.Url
                    })
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[DIAGNOSTIC] Erro ao obter menu");

            return new
            {
                Success = false,
                Error = ex.Message,
                StackTrace = ex.StackTrace
            };
        }
    }

    /// <summary>
    /// Força invalidação do cache do menu.
    /// POST /MenuDiagnostic/InvalidateCache
    /// </summary>
    [HttpPost("InvalidateCache")]
    public IActionResult InvalidateCache()
    {
        _menuService.InvalidateCache();
        return Ok(new { message = "Cache invalidado com sucesso" });
    }
}