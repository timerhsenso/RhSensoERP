// =============================================================================
// RHSENSOERP WEB - FAVORITES SERVICE INTERFACE
// =============================================================================
// Arquivo: src/Web/Services/Favorites/IFavoritesService.cs
// Descrição: Interface para gerenciamento de favoritos do menu
// =============================================================================

namespace RhSensoERP.Web.Services.Favorites;

public interface IFavoritesService
{
    /// <summary>
    /// Obtém todas as ScreenKeys favoritadas pelo usuário.
    /// </summary>
    Task<HashSet<string>> GetFavoriteKeysAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Toggle de favorito: adiciona se não existe, remove se existe.
    /// Retorna true se ficou favoritado, false se foi removido.
    /// </summary>
    Task<bool> ToggleFavoriteAsync(Guid userId, FavoriteUpsertDto dto, CancellationToken ct = default);
}

/// <summary>
/// DTO para inserção/atualização de favorito.
/// </summary>
public sealed class FavoriteUpsertDto
{
    /// <summary>
    /// Chave única da tela (obrigatório).
    /// Formato: "{CdSistema}:{CdFuncao}:{Area}:{Controller}:{Action}"
    /// </summary>
    public required string ScreenKey { get; init; }

    /// <summary>
    /// Código do sistema (ex: "RHU", "SEG").
    /// </summary>
    public string? CdSistema { get; init; }

    /// <summary>
    /// Código da função de permissão.
    /// </summary>
    public string? CdFuncao { get; init; }

    /// <summary>
    /// Área MVC (ex: "RHU", "SEG").
    /// </summary>
    public string? Area { get; init; }

    /// <summary>
    /// Nome do controller.
    /// </summary>
    public string? Controller { get; init; }

    /// <summary>
    /// Nome da action.
    /// </summary>
    public string? Action { get; init; }

    /// <summary>
    /// Nome de exibição no menu de favoritos.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Ícone FontAwesome.
    /// </summary>
    public string? Icon { get; init; }
}