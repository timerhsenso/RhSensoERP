// =============================================================================
// RHSENSOERP WEB - FAVORITES CONTROLLER
// =============================================================================
// Arquivo: src/Web/Controllers/FavoritesController.cs
// Descrição: API para toggle de favoritos no menu
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Services.Favorites;
using RhSensoERP.Web.Services.Menu;
using System.Security.Claims;

namespace RhSensoERP.Web.Controllers;

[Authorize]
[Route("api/favorites")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesService _favorites;
    private readonly IMenuDiscoveryService _menu;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(
        IFavoritesService favorites,
        IMenuDiscoveryService menu,
        ILogger<FavoritesController> logger)
    {
        _favorites = favorites;
        _menu = menu;
        _logger = logger;
    }

    /// <summary>
    /// Toggle de favorito para uma tela.
    /// POST /api/favorites/toggle
    /// </summary>
    [HttpPost("toggle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle([FromBody] ToggleRequest dto, CancellationToken ct)
    {
        // 1) Valida UserId
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            _logger.LogWarning("[FAVORITES] Toggle falhou - UserId inválido: {UserIdRaw}", userIdRaw);
            return Unauthorized(new { error = "Usuário não autenticado" });
        }

        // 2) Valida ScreenKey
        if (string.IsNullOrWhiteSpace(dto.ScreenKey))
        {
            return BadRequest(new { error = "ScreenKey é obrigatório" });
        }

        _logger.LogDebug(
            "[FAVORITES] Toggle | User: {UserId} | ScreenKey: {ScreenKey}",
            userId, dto.ScreenKey);

        // 3) Monta DTO completo para persistir no banco
        var upsertDto = new FavoriteUpsertDto
        {
            ScreenKey = dto.ScreenKey,
            CdSistema = dto.CdSistema,
            CdFuncao = dto.CdFuncao,
            Area = dto.Area,
            Controller = dto.Controller,
            Action = dto.Action,
            DisplayName = dto.DisplayName,
            Icon = dto.Icon
        };

        // 4) Executa toggle
        var isFavorite = await _favorites.ToggleFavoriteAsync(userId, upsertDto, ct);

        // 5) Invalida cache do menu para refletir mudança
        _menu.InvalidateCache();

        _logger.LogInformation(
            "[FAVORITES] Toggle OK | User: {UserId} | ScreenKey: {ScreenKey} | IsFavorite: {IsFavorite}",
            userId, dto.ScreenKey, isFavorite);

        return Ok(new { isFavorite, screenKey = dto.ScreenKey });
    }

    /// <summary>
    /// Lista favoritos do usuário logado.
    /// GET /api/favorites
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return Unauthorized(new { error = "Usuário não autenticado" });
        }

        var keys = await _favorites.GetFavoriteKeysAsync(userId, ct);
        return Ok(new { favorites = keys.ToList() });
    }

    /// <summary>
    /// DTO de request para toggle.
    /// </summary>
    public class ToggleRequest
    {
        /// <summary>
        /// Chave única da tela (obrigatório).
        /// Formato: "{CdSistema}:{CdFuncao}:{Area}:{Controller}:{Action}"
        /// </summary>
        public string ScreenKey { get; set; } = string.Empty;

        // Dados opcionais para persistência (útil para reconstruir menu de favoritos)
        public string? CdSistema { get; set; }
        public string? CdFuncao { get; set; }
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? DisplayName { get; set; }
        public string? Icon { get; set; }
    }
}