// =============================================================================
// RHSENSOERP WEB - MENU DISCOVERY SERVICE (COM GUID DINÂMICO + FAVORITOS)
// =============================================================================
// Arquivo: src/Web/Services/Menu/MenuDiscoveryService.cs
// Descrição:
//   - Descobre automaticamente Controllers marcados com [MenuItem]
//   - Filtra por permissão 'C' (Consulta)
//   - Gera URL dinâmica /go/{guid} por execução (GUID muda a cada deploy/start)
//   - Favoritos persistem por ScreenKey (não dependem do GUID)
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RhSensoERP.Web.Attributes;
using RhSensoERP.Web.Routing;
using RhSensoERP.Web.Services.Favorites;
using RhSensoERP.Web.Services.Permissions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Claims;

namespace RhSensoERP.Web.Services.Menu;

#region === INTERFACES ===

public interface IMenuDiscoveryService
{
    Task<IReadOnlyList<MenuModuleViewModel>> GetMenuAsync(string? username = null, CancellationToken ct = default);
    Task<MenuModuleViewModel?> GetModuleMenuAsync(MenuModule module, string? username = null, CancellationToken ct = default);

    /// <summary>
    /// Resolve uma ScreenKey para os dados de rota (Area, Controller, Action).
    /// Usado pelo ScreenController para redirecionamento.
    /// </summary>
    Task<ScreenTarget?> ResolveByScreenKeyAsync(string screenKey, CancellationToken ct = default);

    void InvalidateCache();
}

#endregion

#region === SERVICE IMPLEMENTATION ===

public class MenuDiscoveryService : IMenuDiscoveryService
{
    private readonly IUserPermissionsCacheService _permissionsService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MenuDiscoveryService> _logger;

    // GUID dinâmico por execução
    private readonly IScreenRouteRegistry _routeRegistry;

    // Favoritos no banco
    private readonly IFavoritesService _favoritesService;

    // =========================================================================
    // CACHE ESTÁTICO
    // =========================================================================

    private static readonly Lazy<IReadOnlyList<MenuItemInfo>> _allMenuItems = new(DiscoverMenuItems);

    private static readonly ConcurrentDictionary<string, CachedMenuEntry> _userMenuCache = new();

    // Cache de ScreenKey -> Dados de rota (para resolução rápida)
    private static readonly ConcurrentDictionary<string, ScreenTarget> _screenKeyToTarget = new();

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public MenuDiscoveryService(
        IUserPermissionsCacheService permissionsService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MenuDiscoveryService> logger,
        IScreenRouteRegistry routeRegistry,
        IFavoritesService favoritesService)
    {
        _permissionsService = permissionsService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _routeRegistry = routeRegistry;
        _favoritesService = favoritesService;
    }

    // =========================================================================
    // DESCOBERTA DE CONTROLLERS (EXECUTADO UMA VEZ)
    // =========================================================================

    private static IReadOnlyList<MenuItemInfo> DiscoverMenuItems()
    {
        var items = new List<MenuItemInfo>();

        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();

        foreach (var assembly in assemblies)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).Cast<Type>().ToArray();
            }

            var controllers = types
                .Where(t => t is not null
                            && t.IsClass
                            && !t.IsAbstract
                            && typeof(Controller).IsAssignableFrom(t)
                            && t.GetCustomAttribute<MenuItemAttribute>() != null)
                .ToList();

            if (controllers.Count == 0)
                continue;

            foreach (var controller in controllers)
            {
                var attr = controller.GetCustomAttribute<MenuItemAttribute>()!;

                if (attr.Hidden)
                    continue;

                var controllerName = controller.Name;
                if (controllerName.EndsWith("Controller", StringComparison.Ordinal))
                    controllerName = controllerName[..^10];

                // Área: prioridade = MenuItem.Area, depois [Area] no controller
                var areaAttr = controller.GetCustomAttribute<AreaAttribute>();
                var areaName = attr.Area ?? areaAttr?.RouteValue;

                items.Add(new MenuItemInfo
                {
                    ControllerName = controllerName,
                    ControllerType = controller,
                    Module = attr.Module,
                    DisplayName = attr.DisplayName ?? FormatDisplayName(controllerName),
                    Icon = attr.Icon,
                    Order = attr.Order,
                    CdFuncao = attr.CdFuncao,
                    CdSistema = attr.CdSistema,
                    ComingSoon = attr.ComingSoon,
                    Badge = attr.Badge,
                    BadgeColor = attr.BadgeColor,
                    Description = attr.Description,
                    Action = string.IsNullOrWhiteSpace(attr.Action) ? "Index" : attr.Action,
                    RouteValues = attr.RouteValues,
                    Area = areaName
                });
            }
        }

        return items
            .OrderBy(i => i.Module)
            .ThenBy(i => i.Order)
            .ThenBy(i => i.DisplayName)
            .ToList()
            .AsReadOnly();
    }

    private static string FormatDisplayName(string controllerName)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            controllerName,
            "([a-z])([A-Z])",
            "$1 $2");
    }

    // =========================================================================
    // OBTENÇÃO DO MENU (COM PERMISSÃO + GUID + FAVORITOS)
    // =========================================================================

    public async Task<IReadOnlyList<MenuModuleViewModel>> GetMenuAsync(
        string? username = null,
        CancellationToken ct = default)
    {
        // 1) Obtém cdUsuario do claim (usado para permissões e cache)
        var cdUsuario = _httpContextAccessor.HttpContext?.User?.FindFirstValue("cdusuario");

        // username é usado apenas como chave de cache alternativa
        username ??= cdUsuario ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        _logger.LogDebug(
            "[MENU] Iniciando GetMenuAsync | CdUsuario: '{CdUsuario}' | Username: '{Username}' | IsAuth: {IsAuth}",
            cdUsuario,
            username,
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated);

        if (string.IsNullOrWhiteSpace(cdUsuario) && string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("[MENU] CdUsuario/Username vazio. Menu retornado vazio.");
            return Array.Empty<MenuModuleViewModel>();
        }

        // Usa cdUsuario para permissões, fallback para username
        var userKey = cdUsuario ?? username!;

        // 2) cache por usuário
        if (_userMenuCache.TryGetValue(userKey, out var cached))
        {
            var age = DateTimeOffset.UtcNow - cached.CreatedAt;
            if (age <= CacheDuration)
            {
                _logger.LogDebug(
                    "[MENU] Cache HIT '{User}' | Age: {Age:mm\\:ss} | Modules: {Modules} | Items: {Items}",
                    userKey,
                    age,
                    cached.Menu.Count,
                    cached.Menu.Sum(m => m.Items.Count));
                return cached.Menu;
            }

            _userMenuCache.TryRemove(userKey, out _);
            _logger.LogDebug("[MENU] Cache expirado '{User}'. Recarregando...", userKey);
        }

        // 3) favoritos do usuário (por ScreenKey)
        var favoriteKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var userIdRaw = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdRaw, out var userId))
        {
            favoriteKeys = await _favoritesService.GetFavoriteKeysAsync(userId, ct);
        }

        // 4) todos itens descobertos
        var allItems = _allMenuItems.Value;

        _logger.LogDebug("[MENU] Total itens descobertos: {Count}", allItems.Count);

        var modules = new List<MenuModuleViewModel>();

        foreach (var group in allItems.GroupBy(i => i.Module).OrderBy(g => GetModuleOrder(g.Key)))
        {
            var moduleInfo = GetModuleInfo(group.Key);
            var moduleItems = new List<MenuItemViewModel>();

            _logger.LogDebug(
                "[MENU] Processando módulo: {Module} ({DisplayName}) | Itens: {Count}",
                group.Key, moduleInfo.DisplayName, group.Count());

            foreach (var item in group.OrderBy(i => i.Order).ThenBy(i => i.DisplayName))
            {
                // 4.1) permissão (consulta)
                var hasPermission = true;

                if (!string.IsNullOrWhiteSpace(item.CdFuncao))
                {
                    hasPermission = await _permissionsService.HasPermissionAsync(
                        userKey,
                        item.CdFuncao!,
                        'C',
                        ct);

                    _logger.LogDebug(
                        "[MENU] Permissão 'C' | Item: {Item} | CdFuncao: {CdFuncao} | User: {User} | Result: {Res}",
                        item.DisplayName, item.CdFuncao, userKey, hasPermission ? "OK" : "NEGADO");
                }

                // sem permissão e não é coming soon => oculta
                if (!hasPermission && !item.ComingSoon)
                {
                    _logger.LogDebug("[MENU] Oculto (sem permissão): {Item}", item.DisplayName);
                    continue;
                }

                // 4.2) ScreenKey (estável) + GUID dinâmico
                // ScreenKey NÃO depende do GUID. É isso que mantém favoritos funcionando.
                var cdSistema = !string.IsNullOrWhiteSpace(item.CdSistema) ? item.CdSistema! : moduleInfo.CdSistema;
                var cdFuncao = item.CdFuncao ?? item.ControllerName;

                var screenKey = $"{cdSistema}:{cdFuncao}:{item.Area}:{item.ControllerName}:{item.Action}";

                var routeValues = new RouteValueDictionary
                {
                    ["controller"] = item.ControllerName,
                    ["action"] = item.Action ?? "Index"
                };

                if (!string.IsNullOrWhiteSpace(item.Area))
                    routeValues["area"] = item.Area;

                // GUID gerado por execução
                var routeGuid = _routeRegistry.GetOrCreateGuid(screenKey, routeValues);

                // Mapeia ScreenKey para dados de rota (para ResolveByScreenKeyAsync)
                _screenKeyToTarget[screenKey] = new ScreenTarget
                {
                    Area = item.Area,
                    Controller = item.ControllerName,
                    Action = item.Action ?? "Index"
                };

                moduleItems.Add(new MenuItemViewModel
                {
                    Area = item.Area,
                    Controller = item.ControllerName,
                    Action = item.Action ?? "Index",
                    DisplayName = item.DisplayName,
                    Icon = item.Icon,
                    CdFuncao = item.CdFuncao,
                    CdSistema = cdSistema,
                    ComingSoon = item.ComingSoon,
                    Badge = item.ComingSoon ? "Em breve" : item.Badge,
                    BadgeColor = item.ComingSoon ? "secondary" : item.BadgeColor,
                    Description = item.Description,
                    HasPermission = hasPermission && !item.ComingSoon,

                    ScreenKey = screenKey,
                    RouteGuid = routeGuid,
                    IsFavorite = favoriteKeys.Contains(screenKey)
                });

                _logger.LogDebug("[MENU] Adicionado: {Item} | Url: {Url}", item.DisplayName, $"/go/{routeGuid}");
            }

            if (moduleItems.Count > 0)
            {
                modules.Add(new MenuModuleViewModel
                {
                    Module = group.Key,
                    DisplayName = moduleInfo.DisplayName,
                    Icon = moduleInfo.Icon,
                    Order = moduleInfo.Order,
                    CdSistema = moduleInfo.CdSistema,
                    Items = moduleItems.AsReadOnly()
                });
            }
        }

        var entry = new CachedMenuEntry
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Menu = modules.AsReadOnly()
        };

        _userMenuCache[userKey] = entry;

        _logger.LogInformation(
            "[MENU] Menu montado '{User}' | Modules: {Modules} | Items: {Items} | TTL: {TTL}min",
            userKey,
            entry.Menu.Count,
            entry.Menu.Sum(m => m.Items.Count),
            CacheDuration.TotalMinutes);

        return entry.Menu;
    }

    public async Task<MenuModuleViewModel?> GetModuleMenuAsync(
        MenuModule module,
        string? username = null,
        CancellationToken ct = default)
    {
        var menu = await GetMenuAsync(username, ct);
        return menu.FirstOrDefault(m => m.Module == module);
    }

    /// <summary>
    /// Resolve uma ScreenKey para os dados de rota.
    /// Primeiro tenta o cache local. Se não encontrar, força uma descoberta de menu.
    /// </summary>
    public async Task<ScreenTarget?> ResolveByScreenKeyAsync(string screenKey, CancellationToken ct = default)
    {
        // 1) Tenta cache estático
        if (_screenKeyToTarget.TryGetValue(screenKey, out var target))
            return target;

        // 2) Se não encontrou, pode ser que o menu ainda não foi carregado.
        // Força carregamento de menu para popular o cache.
        var cdUsuario = _httpContextAccessor.HttpContext?.User?.FindFirstValue("cdusuario");
        var username = cdUsuario ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        if (!string.IsNullOrWhiteSpace(username))
        {
            await GetMenuAsync(username, ct);

            // Tenta novamente após carregamento
            if (_screenKeyToTarget.TryGetValue(screenKey, out target))
                return target;
        }

        // 3) Fallback: tenta parsear ScreenKey diretamente
        // Formato: "{CdSistema}:{CdFuncao}:{Area}:{Controller}:{Action}"
        var parts = screenKey.Split(':');
        if (parts.Length >= 5)
        {
            return new ScreenTarget
            {
                Area = string.IsNullOrWhiteSpace(parts[2]) ? null : parts[2],
                Controller = parts[3],
                Action = parts[4]
            };
        }

        _logger.LogWarning("[MENU] ScreenKey não resolvida: {ScreenKey}", screenKey);
        return null;
    }

    public void InvalidateCache()
    {
        var count = _userMenuCache.Count;
        _userMenuCache.Clear();
        _logger.LogInformation("[MENU] Cache invalidado | Removidos: {Count}", count);
    }

    // =========================================================================
    // Helpers módulo
    // =========================================================================

    private static (string DisplayName, string Icon, int Order, string CdSistema) GetModuleInfo(MenuModule module)
    {
        var field = typeof(MenuModule).GetField(module.ToString());
        var attr = field?.GetCustomAttribute<MenuModuleInfoAttribute>();

        return attr != null
            ? (attr.DisplayName, attr.Icon, attr.Order, attr.CdSistema)
            : (module.ToString(), "fas fa-folder", 99, "OUT");
    }

    private static int GetModuleOrder(MenuModule module) => GetModuleInfo(module).Order;

    // =========================================================================
    // Estruturas internas
    // =========================================================================

    internal class MenuItemInfo
    {
        public string ControllerName { get; set; } = string.Empty;
        public Type ControllerType { get; set; } = null!;
        public MenuModule Module { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Icon { get; set; } = "fas fa-circle";
        public int Order { get; set; }
        public string? CdFuncao { get; set; }
        public string? CdSistema { get; set; }
        public bool ComingSoon { get; set; }
        public string? Badge { get; set; }
        public string BadgeColor { get; set; } = "info";
        public string? Description { get; set; }
        public string Action { get; set; } = "Index";
        public string? RouteValues { get; set; }
        public string? Area { get; set; }
    }

    internal sealed class CachedMenuEntry
    {
        public DateTimeOffset CreatedAt { get; init; }
        public IReadOnlyList<MenuModuleViewModel> Menu { get; init; } = Array.Empty<MenuModuleViewModel>();
    }
}

#endregion

#region === VIEWMODELS ===

/// <summary>
/// Representa um módulo do menu (dropdown).
/// </summary>
public class MenuModuleViewModel
{
    public MenuModule Module { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public string CdSistema { get; set; } = string.Empty;
    public IReadOnlyList<MenuItemViewModel> Items { get; set; } = Array.Empty<MenuItemViewModel>();
}

/// <summary>
/// Representa um item de menu (link individual).
/// </summary>
public class MenuItemViewModel
{
    public string? Area { get; set; }
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Index";
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = "fas fa-circle";
    public string? CdFuncao { get; set; }
    public string? CdSistema { get; set; }
    public bool ComingSoon { get; set; }
    public string? Badge { get; set; }
    public string BadgeColor { get; set; } = "info";
    public string? Description { get; set; }
    public bool HasPermission { get; set; }

    // =======================
    // FAVORITOS + GUID DINÂMICO
    // =======================
    public string ScreenKey { get; set; } = string.Empty;
    public Guid? RouteGuid { get; set; }
    public bool IsFavorite { get; set; }

    /// <summary>
    /// URL para navegação. Usa GUID dinâmico se disponível.
    /// </summary>
    public string Url
    {
        get
        {
            if (RouteGuid.HasValue)
                return $"/go/{RouteGuid.Value}";

            var areaPrefix = string.IsNullOrWhiteSpace(Area) ? string.Empty : $"/{Area}";
            return $"{areaPrefix}/{Controller}/{Action}";
        }
    }
}

/// <summary>
/// Dados de destino para redirecionamento por ScreenKey.
/// </summary>
public class ScreenTarget
{
    public string? Area { get; set; }
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Index";
}

#endregion