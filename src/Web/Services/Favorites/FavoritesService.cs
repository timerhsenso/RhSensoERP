// =============================================================================
// RHSENSOERP WEB - FAVORITES SERVICE (SESSION/MEMORY)
// =============================================================================
// Arquivo: src/Web/Services/Favorites/FavoritesService.cs
// Descrição: Gerenciamento de favoritos usando Session (sem acesso a banco)
// 
// NOTA: Esta implementação usa Session para armazenar favoritos.
// Os favoritos persistem enquanto a sessão do usuário estiver ativa.
// Para persistência permanente, implemente chamadas à API de backend.
// =============================================================================

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace RhSensoERP.Web.Services.Favorites;

public sealed class FavoritesService : IFavoritesService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<FavoritesService> _logger;

    private const string SessionKey = "UserFavorites";

    public FavoritesService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<FavoritesService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public Task<HashSet<string>> GetFavoriteKeysAsync(Guid userId, CancellationToken ct = default)
    {
        var favorites = GetFavoritesFromSession(userId);

        _logger.LogDebug(
            "[FAVORITES] GetFavoriteKeys | User: {UserId} | Count: {Count}",
            userId, favorites.Count);

        return Task.FromResult(favorites);
    }

    public Task<bool> ToggleFavoriteAsync(Guid userId, FavoriteUpsertDto dto, CancellationToken ct = default)
    {
        var favorites = GetFavoritesFromSession(userId);
        bool isFavorite;

        if (favorites.Contains(dto.ScreenKey))
        {
            // Já existe - remover
            favorites.Remove(dto.ScreenKey);
            isFavorite = false;

            _logger.LogInformation(
                "[FAVORITES] Removido | User: {UserId} | ScreenKey: {ScreenKey}",
                userId, dto.ScreenKey);
        }
        else
        {
            // Não existe - adicionar
            favorites.Add(dto.ScreenKey);
            isFavorite = true;

            _logger.LogInformation(
                "[FAVORITES] Adicionado | User: {UserId} | ScreenKey: {ScreenKey}",
                userId, dto.ScreenKey);
        }

        SaveFavoritesToSession(userId, favorites);

        return Task.FromResult(isFavorite);
    }

    private HashSet<string> GetFavoritesFromSession(Guid userId)
    {
        if (Session == null)
        {
            _logger.LogWarning("[FAVORITES] Session não disponível");
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var key = $"{SessionKey}:{userId}";
        var json = Session.GetString(key);

        if (string.IsNullOrEmpty(json))
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(json);
            return new HashSet<string>(list ?? [], StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FAVORITES] Erro ao deserializar favoritos da sessão");
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private void SaveFavoritesToSession(Guid userId, HashSet<string> favorites)
    {
        if (Session == null)
        {
            _logger.LogWarning("[FAVORITES] Session não disponível para salvar");
            return;
        }

        var key = $"{SessionKey}:{userId}";
        var json = JsonSerializer.Serialize(favorites.ToList());

        Session.SetString(key, json);

        _logger.LogDebug(
            "[FAVORITES] Salvos na sessão | User: {UserId} | Count: {Count}",
            userId, favorites.Count);
    }
}