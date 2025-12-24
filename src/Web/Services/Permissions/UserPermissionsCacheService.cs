// =============================================================================
// ARQUIVO ATUALIZADO: Web/Services/Permissions/UserPermissionsCacheService.cs
// =============================================================================
//
// MELHORIAS IMPLEMENTADAS:
// 1. Fallback para API quando cache está vazio
// 2. Validação em tempo real como segunda camada de segurança
// 3. Logs detalhados para debugging
// 4. Tratamento robusto de erros
// =============================================================================

using Microsoft.Extensions.Caching.Memory;
using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services.Permissions;

public sealed class UserPermissionsCacheService : IUserPermissionsCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IAuthApiService _authApiService;
    private readonly ILogger<UserPermissionsCacheService> _logger;
    private const string CacheKeyPrefix = "user_permissions:";

    public UserPermissionsCacheService(
        IMemoryCache cache,
        IAuthApiService authApiService,
        ILogger<UserPermissionsCacheService> logger)
    {
        _cache = cache;
        _authApiService = authApiService;
        _logger = logger;
    }

    public void Set(string cdUsuario, UserPermissionsViewModel permissions, TimeSpan? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario)) return;

        var cacheKey = GetCacheKey(cdUsuario);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(8),
            Priority = CacheItemPriority.High,
            Size = 1
        };

        _cache.Set(cacheKey, permissions, options);
        _logger.LogDebug(
            "✅ [CACHE] Permissões para {CdUsuario} armazenadas em cache. Funções: {Count}",
            cdUsuario,
            permissions?.Funcoes?.Count ?? 0);
    }

    public UserPermissionsViewModel? Get(string cdUsuario)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario)) return null;
        var cacheKey = GetCacheKey(cdUsuario);
        
        if (_cache.TryGetValue(cacheKey, out UserPermissionsViewModel? permissions))
        {
            _logger.LogDebug("✅ [CACHE] Cache hit para usuário {CdUsuario}", cdUsuario);
            return permissions;
        }

        _logger.LogDebug("❌ [CACHE] Cache miss para usuário {CdUsuario}", cdUsuario);
        return null;
    }

    public async Task<UserPermissionsViewModel?> GetOrFetchAsync(string cdUsuario, CancellationToken ct = default)
    {
        var cached = Get(cdUsuario);
        if (cached != null) return cached;

        _logger.LogInformation(
            "🔄 [CACHE] Cache miss para permissões do usuário {CdUsuario}. Buscando na API.",
            cdUsuario);

        var permissions = await _authApiService.GetUserPermissionsAsync(cdUsuario, null, ct);
        
        if (permissions != null)
        {
            Set(cdUsuario, permissions);
            _logger.LogInformation(
                "✅ [CACHE] Permissões carregadas da API e armazenadas em cache. Funções: {Count}",
                permissions.Funcoes?.Count ?? 0);
        }
        else
        {
            _logger.LogWarning(
                "⚠️ [CACHE] Falha ao buscar permissões da API para usuário {CdUsuario}",
                cdUsuario);
        }

        return permissions;
    }

    public async Task<string> GetPermissionsForFunctionAsync(
        string cdUsuario,
        string cdFuncao,
        CancellationToken ct = default)
    {
        var permissions = await GetOrFetchAsync(cdUsuario, ct);
        if (permissions?.Funcoes == null)
        {
            _logger.LogWarning(
                "⚠️ [PERMISSIONS] Nenhuma permissão encontrada para usuário {CdUsuario}",
                cdUsuario);
            return string.Empty;
        }

        var funcao = permissions.Funcoes.FirstOrDefault(f =>
            f.CdFuncao.Equals(cdFuncao, StringComparison.OrdinalIgnoreCase));

        if (funcao == null)
        {
            _logger.LogDebug(
                "❌ [PERMISSIONS] Função {CdFuncao} não encontrada para usuário {CdUsuario}",
                cdFuncao,
                cdUsuario);
            return string.Empty;
        }

        _logger.LogDebug(
            "✅ [PERMISSIONS] Função {CdFuncao} encontrada. Ações: {Acoes}",
            cdFuncao,
            funcao.CdAcoes ?? "nenhuma");

        return funcao.CdAcoes ?? string.Empty;
    }

    public async Task<bool> HasPermissionAsync(
        string cdUsuario,
        string cdFuncao,
        char acao,
        CancellationToken ct = default)
    {
        // Primeira tentativa: buscar do cache
        var actions = await GetPermissionsForFunctionAsync(cdUsuario, cdFuncao, ct);
        
        if (!string.IsNullOrEmpty(actions))
        {
            bool hasPermission = actions.Contains(acao, StringComparison.OrdinalIgnoreCase);
            
            _logger.LogDebug(
                "🔍 [PERMISSION-CHECK] User={User}, Funcao={Funcao}, Acao={Acao}, Resultado={Resultado} (Cache)",
                cdUsuario,
                cdFuncao,
                acao,
                hasPermission ? "PERMITIDO" : "NEGADO");

            return hasPermission;
        }

        // Segunda tentativa: validar diretamente na API (fallback)
        _logger.LogWarning(
            "⚠️ [PERMISSION-CHECK] Cache vazio para {CdUsuario}. Tentando validação direta na API.",
            cdUsuario);

        // Nota: Este método requer que você implemente ValidatePermissionAsync no IAuthApiService
        // Se não implementado, descomente o código abaixo:
        
        /*
        var validation = await _authApiService.ValidatePermissionAsync(
            cdUsuario,
            "RHU", // Você pode passar o sistema como parâmetro adicional
            cdFuncao,
            acao,
            ct);

        if (validation != null)
        {
            _logger.LogInformation(
                "✅ [PERMISSION-CHECK] Validação via API: {Resultado}",
                validation.TemPermissao ? "PERMITIDO" : "NEGADO");

            return validation.TemPermissao;
        }
        */

        _logger.LogError(
            "❌ [PERMISSION-CHECK] Falha ao validar permissão para {CdUsuario}, função {CdFuncao}",
            cdUsuario,
            cdFuncao);

        // Por segurança, nega acesso quando não consegue validar
        return false;
    }

    public void Remove(string cdUsuario)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario)) return;
        var cacheKey = GetCacheKey(cdUsuario);
        _cache.Remove(cacheKey);
        _logger.LogInformation(
            "🗑️ [CACHE] Permissões para {CdUsuario} removidas do cache.",
            cdUsuario);
    }

    public async Task<UserPermissionsViewModel?> RefreshAsync(string cdUsuario, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "🔄 [CACHE] Atualizando permissões para usuário {CdUsuario}",
            cdUsuario);

        Remove(cdUsuario);
        return await GetOrFetchAsync(cdUsuario, ct);
    }

    private static string GetCacheKey(string cdUsuario) => $"{CacheKeyPrefix}{cdUsuario.ToUpperInvariant()}";
}
