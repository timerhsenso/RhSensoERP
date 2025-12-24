// ============================================================================
// ARQUIVO ALTERADO - SUBSTITUIR: src/Identity/Application/Services/AuthService.cs
// ============================================================================

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Common;

// Alias para evitar confusão com o namespace BCrypt.Net
using BCryptNet = BCrypt.Net.BCrypt;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço de autenticação responsável por:
/// - Realizar login (Legacy / SaaS / ADWin)
/// - Renovar tokens (refresh token)
/// - Realizar logout (revogar tokens)
/// - Validar senha conforme estratégia de autenticação
/// 
/// Observação sobre logs:
/// - Logs detalhados de DEBUG só são emitidos quando o ambiente é "Development"
///   (ASPNETCORE_ENVIRONMENT == Development).
/// - Logs de Warning e Error continuam sendo registrados em qualquer ambiente.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IdentityDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITenantContext _tenantContext; // ✅ FASE 1
    private readonly IPermissaoService _permissaoService; // ✅ FASE 2
    private readonly IActiveDirectoryService _activeDirectoryService; // ✅ FASE 3
    private readonly ILogger<AuthService> _logger;
    private readonly AuthSettings _authSettings;
    private readonly SecurityPolicySettings _securityPolicy;

    public AuthService(
        IdentityDbContext db,
        IJwtService jwtService,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        ITenantContext tenantContext, // ✅ FASE 1
        IPermissaoService permissaoService, // ✅ FASE 2
        IActiveDirectoryService activeDirectoryService, // ✅ FASE 3
        ILogger<AuthService> logger,
        IOptions<AuthSettings> authSettings,
        IOptions<SecurityPolicySettings> securityPolicy)
    {
        _db = db;
        _jwtService = jwtService;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _tenantContext = tenantContext;
        _permissaoService = permissaoService;
        _activeDirectoryService = activeDirectoryService;
        _logger = logger;
        _authSettings = authSettings.Value;
        _securityPolicy = securityPolicy.Value;

        EnsureDefaultStrategiesExist();
    }

    /// <summary>
    /// Verifica se o ambiente atual é Development.
    /// Apenas neste ambiente logs de DEBUG mais verbosos serão emitidos.
    /// </summary>
    private static bool IsDevelopmentEnvironment()
        => string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            "Development",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Garante que estratégias padrão existem caso o appsettings não as defina.
    /// Também valida e ajusta a estratégia padrão (DefaultStrategy).
    /// </summary>
    private void EnsureDefaultStrategiesExist()
    {
        if (_authSettings.Strategies.Count == 0)
        {
            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: AuthSettings.Strategies vazio. Criando configurações padrão.");

            _authSettings.Strategies["Legacy"] = new StrategyConfig
            {
                Enabled = true,
                UseBCrypt = false,
                SyncWithUserSecurity = true,
                RequireEmailConfirmation = false,
                Require2FA = false
            };

            _authSettings.Strategies["SaaS"] = new StrategyConfig
            {
                Enabled = true,
                UseBCrypt = true,
                SyncWithUserSecurity = false,
                RequireEmailConfirmation = true,
                Require2FA = false
            };

            _authSettings.Strategies["ADWin"] = new StrategyConfig
            {
                Enabled = false,
                UseBCrypt = false,
                SyncWithUserSecurity = false,
                RequireEmailConfirmation = false,
                Require2FA = false
            };

            _logger.LogInformation(
                "✅ INICIALIZAÇÃO: Estratégias padrão criadas: {Strategies}",
                string.Join(", ", _authSettings.Strategies.Keys));
        }
        else
        {
            _logger.LogInformation(
                "✅ INICIALIZAÇÃO: Estratégias carregadas do appsettings: {Strategies}",
                string.Join(", ", _authSettings.Strategies.Keys));
        }

        if (string.IsNullOrWhiteSpace(_authSettings.DefaultStrategy))
        {
            _authSettings.DefaultStrategy = "Legacy";
            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy vazio. Definido como '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);
        }

        if (!_authSettings.Strategies.ContainsKey(_authSettings.DefaultStrategy))
        {
            var firstEnabled = _authSettings.Strategies
                .FirstOrDefault(s => s.Value.Enabled).Key ?? "Legacy";

            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy '{DefaultStrategy}' não encontrada. Usando '{Fallback}'",
                _authSettings.DefaultStrategy,
                firstEnabled);

            _authSettings.DefaultStrategy = firstEnabled;
        }
    }

    /// <summary>
    /// Autentica um usuário com credenciais e retorna tokens JWT.
    /// 
    /// Fluxo resumido:
    /// 1. Resolve o tenant (quando houver multi-tenant).
    /// 2. Determina o AuthMode (Legacy, SaaS ou ADWin).
    /// 3. Localiza o usuário (por cdusuario / email conforme AuthMode).
    /// 4. Carrega ou cria o registro de UserSecurity.
    /// 5. Verifica lockout (conta bloqueada).
    /// 6. Valida a senha conforme estratégia.
    /// 7. Aplica políticas adicionais (e-mail confirmado, 2FA).
    /// 8. Registra auditoria de login e atualiza UserSecurity.
    /// 9. Carrega permissões (grupos / funções / botões).
    /// 10. Gera tokens JWT + refresh token e retorna AuthResponse.
    /// </summary>
    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        try
        {
            _logger.LogInformation("🚀 AuthService.LoginAsync INICIADO para {LoginIdentifier}", request.LoginIdentifier);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] LoginAsync - Request: {@Request}, IpAddress: {Ip}, UserAgent: {UserAgent}",
                    request, ipAddress, userAgent);
            }

            // ====================================================================
            // ETAPA 1: RESOLVER TENANT
            // ====================================================================
            var tenantId = _tenantContext.TenantId;
            Guid? tenantGuid = null;

            if (!string.IsNullOrEmpty(tenantId) && Guid.TryParse(tenantId, out var parsedTenantId))
            {
                tenantGuid = parsedTenantId;
                _logger.LogInformation("✅ ETAPA 1: Tenant resolvido - TenantId: {TenantId}", tenantId);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Tenant GUID parseado com sucesso: {TenantGuid}", tenantGuid);
                }
            }
            else
            {
                _logger.LogWarning("⚠️ ETAPA 1: Tenant não resolvido. Usando configuração global.");

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] TenantId não informado ou inválido. Valor recebido: {TenantId}", tenantId);
                }
            }

            // ====================================================================
            // ETAPA 2: DETERMINAR AUTHMODE
            // ====================================================================
            var authMode = await DeterminarAuthModeAsync(tenantGuid, ct);
            _logger.LogInformation("✅ ETAPA 2: AuthMode determinado: '{AuthMode}'", authMode);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] AuthMode final usado no login: {AuthMode}", authMode);
            }

            // ====================================================================
            // ETAPA 3: LOCALIZAR USUÁRIO
            // ====================================================================
            var usuario = await LocalizarUsuarioAsync(request.LoginIdentifier, authMode, ct);

            if (usuario == null)
            {
                _logger.LogWarning("❌ ETAPA 3: Usuário '{LoginIdentifier}' NÃO ENCONTRADO", request.LoginIdentifier);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Nenhum registro retornado na busca de usuário para identificador {LoginIdentifier}",
                        request.LoginIdentifier);
                }

                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            _logger.LogInformation("✅ ETAPA 3: Usuário encontrado - CdUsuario: {CdUsuario}, FlAtivo: {FlAtivo}",
                usuario.CdUsuario, usuario.FlAtivo);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Dados básicos do usuário: {@Usuario}", new
                {
                    usuario.Id,
                    usuario.CdUsuario,
                    usuario.Email_Usuario,
                    usuario.FlAtivo,
                    usuario.NoMatric,
                    usuario.CdEmpresa,
                    usuario.CdFilial,
                    usuario.TenantId
                });
            }

            // Verificar se usuário está ativo
            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("❌ LOGIN: Usuário {CdUsuario} INATIVO", usuario.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // ====================================================================
            // ETAPA 4: CARREGAR SEGURANÇA MODERNA
            // ====================================================================
            _logger.LogInformation("🔍 ETAPA 4: Buscando UserSecurity para IdUsuario={IdUsuario}", usuario.Id);

            var userSecurity = await GetOrCreateUserSecurityAsync(usuario, ct);

            _logger.LogInformation("✅ ETAPA 4: UserSecurity obtido. Id={Id}, LockoutEnd={LockoutEnd}",
                userSecurity.Id, userSecurity.LockoutEnd);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] UserSecurity carregado/criado: {@UserSecurity}", new
                {
                    userSecurity.Id,
                    userSecurity.IdUsuario,
                    userSecurity.IdSaaS,
                    userSecurity.AccessFailedCount,
                    userSecurity.LockoutEnd,
                    userSecurity.EmailConfirmed,
                    userSecurity.TwoFactorEnabled,
                    userSecurity.MustChangePassword
                });
            }

            // ====================================================================
            // ETAPA 5: VERIFICAR LOCKOUT
            // ====================================================================
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                var remainingMinutes = (userSecurity.LockoutEnd.Value - _dateTimeProvider.UtcNow).TotalMinutes;
                _logger.LogWarning("🔒 ETAPA 5: Conta BLOQUEADA até {LockoutEnd}", userSecurity.LockoutEnd);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Conta bloqueada. Minutos restantes: {Minutes}", remainingMinutes);
                }

                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Account locked", ct);

                return Result<AuthResponse>.Failure(
                    "ACCOUNT_LOCKED",
                    $"Conta bloqueada. Tente novamente em {Math.Ceiling(remainingMinutes)} minutos.");
            }

            _logger.LogInformation("✅ ETAPA 5: Lockout verificado - Conta não está bloqueada");

            // Obter configuração da estratégia
            if (!_authSettings.Strategies.TryGetValue(authMode, out var strategyConfig))
            {
                _logger.LogError(
                    "❌ LOGIN: Estratégia '{AuthMode}' não encontrada. Disponíveis: {Available}",
                    authMode,
                    string.Join(", ", _authSettings.Strategies.Keys));

                return Result<AuthResponse>.Failure(
                    "INVALID_AUTH_STRATEGY",
                    "Modo de autenticação inválido. Contate o administrador.");
            }

            if (!strategyConfig.Enabled)
            {
                _logger.LogWarning("⚠️ LOGIN: Estratégia '{AuthMode}' está DESABILITADA", authMode);
                return Result<AuthResponse>.Failure(
                    "AUTH_STRATEGY_DISABLED",
                    "O modo de autenticação está desabilitado.");
            }

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] StrategyConfig utilizada: {@StrategyConfig}", strategyConfig);
            }

            // ====================================================================
            // ETAPA 6: VALIDAR CREDENCIAIS
            // ====================================================================
            _logger.LogInformation("🔐 ETAPA 6: Validando senha com estratégia '{AuthMode}'", authMode);

            var isValidPassword = ValidatePassword(usuario, userSecurity, request.Senha, authMode);

            if (!isValidPassword)
            {
                _logger.LogWarning("❌ ETAPA 6: Senha INVÁLIDA para {CdUsuario}", usuario.CdUsuario);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Falha de senha - incrementando AccessFailedCount (atual: {Count})",
                        userSecurity.AccessFailedCount);
                }

                userSecurity.IncrementAccessFailedCount();

                if (userSecurity.AccessFailedCount >= _securityPolicy.MaxFailedAccessAttempts)
                {
                    var lockoutEnd = _dateTimeProvider.UtcNow.AddMinutes(_securityPolicy.LockoutDurationMinutes);
                    userSecurity.LockUntil(lockoutEnd, $"Max failed attempts ({_securityPolicy.MaxFailedAccessAttempts})");

                    _logger.LogWarning(
                        "🔒 LOGIN: Conta {CdUsuario} BLOQUEADA até {LockoutEnd} após {Attempts} tentativas",
                        usuario.CdUsuario,
                        lockoutEnd,
                        userSecurity.AccessFailedCount);

                    if (isDevelopment)
                    {
                        _logger.LogDebug("🔧 [DEV] Lockout aplicado - LockoutEnd: {LockoutEnd}", lockoutEnd);
                    }
                }

                await UpdateUserSecurityInDatabaseAsync(userSecurity, ct);
                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Invalid password", ct);

                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            _logger.LogInformation("✅ ETAPA 6: Credenciais VÁLIDAS");

            // Validações de segurança adicionais
            if (strategyConfig.RequireEmailConfirmation && !userSecurity.EmailConfirmed)
            {
                _logger.LogWarning("⚠️ LOGIN: E-mail não confirmado para {CdUsuario}", usuario.CdUsuario);
                return Result<AuthResponse>.Failure(
                    "EMAIL_NOT_CONFIRMED",
                    "E-mail não confirmado. Verifique sua caixa de entrada.");
            }

            if (strategyConfig.Require2FA && !userSecurity.TwoFactorEnabled)
            {
                _logger.LogWarning("⚠️ LOGIN: 2FA obrigatório mas não configurado para {CdUsuario}", usuario.CdUsuario);
                return Result<AuthResponse>.Failure(
                    "2FA_REQUIRED",
                    "Autenticação de dois fatores obrigatória. Configure 2FA antes de fazer login.");
            }

            // ====================================================================
            // ETAPA 7: REGISTRAR AUDITORIA
            // ETAPA 8: RESET/INCREMENTO DE TENTATIVAS
            // ====================================================================
            _logger.LogInformation("✅ ETAPA 7-8: Resetando tentativas e registrando auditoria");

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Resetando AccessFailedCount e registrando login bem-sucedido");
            }

            userSecurity.ResetAccessFailedCount();
            userSecurity.RegisterSuccessfulLogin(ipAddress);

            await UpdateUserSecurityInDatabaseAsync(userSecurity, ct);
            await RegisterSuccessfulLoginAsync(userSecurity, ipAddress, userAgent, ct);

            // ====================================================================
            // ETAPA 9: CARREGAR PERMISSÕES
            // ====================================================================
            _logger.LogInformation("🔑 ETAPA 9: Carregando permissões do usuário");

            UserPermissionsDto? permissions = null;

            try
            {
                permissions = await _permissaoService.CarregarPermissoesAsync(
                    usuario.CdUsuario,
                    cdSistema: null, // null = carregar de todos os sistemas
                    ct);

                _logger.LogInformation(
                    "✅ ETAPA 9: Permissões carregadas - Grupos: {Grupos}, Funções: {Funcoes}, Botões: {Botoes}",
                    permissions.Grupos.Count,
                    permissions.Funcoes.Count,
                    permissions.Botoes.Count);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Estrutura de permissões carregadas: {@Permissions}", permissions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "⚠️ ETAPA 9: Erro ao carregar permissões. Login continuará sem permissões.");

                permissions = null;
            }

            // ====================================================================
            // ETAPA 10: CRIAR SESSÃO / TOKEN / CLAIMS
            // ====================================================================
            _logger.LogInformation("✅ ETAPA 10: Gerando tokens JWT");

            var accessToken = _jwtService.GenerateAccessToken(usuario, userSecurity, permissions);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                request.DeviceId,
                request.DeviceName,
                null, // expirationDays
                ct);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Tokens gerados - AccessToken (tamanho): {Length}, RefreshToken (inicio): {Start}",
                    accessToken?.Length,
                    !string.IsNullOrEmpty(refreshToken) && refreshToken.Length > 10
                        ? refreshToken[..10] + "..."
                        : refreshToken);
            }

            // Mapear informações do usuário
            var userInfo = new UserInfoDto
            {
                Id = usuario.Id,
                CdUsuario = usuario.CdUsuario,
                DcUsuario = usuario.DcUsuario,
                Email = usuario.Email_Usuario,
                NoMatric = usuario.NoMatric,
                CdEmpresa = usuario.CdEmpresa,
                CdFilial = usuario.CdFilial,
                TenantId = usuario.TenantId,
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900, // 15 minutos em segundos
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("✅ LOGIN: Tokens gerados com sucesso para {CdUsuario}", usuario.CdUsuario);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] AuthResponse final: {@Response}", response);
            }

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar login: {LoginIdentifier}", request.LoginIdentifier);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Exceção detalhada no LoginAsync: {Exception}", ex);
            }

            return Result<AuthResponse>.Failure("LOGIN_ERROR", "Erro ao processar login. Tente novamente.");
        }
    }

    // ========================================================================
    // MÉTODOS AUXILIARES - NOVOS (FASE 1)
    // ========================================================================

    /// <summary>
    /// Determina o AuthMode consultando a tabela SEG_SecurityPolicy.
    /// Ordem de prioridade:
    /// 1. SecurityPolicy por tenant (IdSaaS = tenantId).
    /// 2. SecurityPolicy global (IdSaaS = null).
    /// 3. DefaultStrategy definido em appsettings.
    /// </summary>
    private async Task<string> DeterminarAuthModeAsync(Guid? tenantId, CancellationToken ct)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        try
        {
            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] DeterminarAuthModeAsync - TenantId: {TenantId}", tenantId);
            }

            // Consultar política de segurança do tenant
            SecurityPolicy? securityPolicy = null;

            if (tenantId.HasValue)
            {
                securityPolicy = await _db.Set<SecurityPolicy>()
                    .AsNoTracking()
                    .Where(sp => sp.IdSaaS == tenantId && sp.IsActive)
                    .FirstOrDefaultAsync(ct);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] SecurityPolicy por tenant retornada: {Encontrada}",
                        securityPolicy != null);
                }
            }

            // Se não encontrou por tenant, buscar política global (IdSaaS = null)
            if (securityPolicy == null)
            {
                securityPolicy = await _db.Set<SecurityPolicy>()
                    .AsNoTracking()
                    .Where(sp => sp.IdSaaS == null && sp.IsActive)
                    .FirstOrDefaultAsync(ct);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] SecurityPolicy global retornada: {Encontrada}",
                        securityPolicy != null);
                }
            }

            // Se encontrou política e tem AuthMode definido, usar
            if (securityPolicy != null && !string.IsNullOrWhiteSpace(securityPolicy.AuthMode))
            {
                _logger.LogInformation(
                    "✅ AuthMode obtido do banco: '{AuthMode}' (Tenant: {TenantId})",
                    securityPolicy.AuthMode,
                    tenantId?.ToString() ?? "Global");

                return securityPolicy.AuthMode;
            }

            // Fallback: usar configuração padrão do appsettings
            _logger.LogInformation(
                "⚠️ AuthMode não encontrado no banco. Usando DefaultStrategy: '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);

            return _authSettings.DefaultStrategy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao determinar AuthMode. Usando DefaultStrategy: '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Exceção detalhada em DeterminarAuthModeAsync: {Exception}", ex);
            }

            return _authSettings.DefaultStrategy;
        }
    }

    /// <summary>
    /// Localiza usuário por email ou cdusuario conforme o AuthMode.
    /// - SaaS: busca SOMENTE por email (Email_Usuario).
    /// - Legacy: busca por cdusuario OU email.
    /// - ADWin: busca por cdusuario (deve corresponder ao AD).
    /// </summary>
    private async Task<Usuario?> LocalizarUsuarioAsync(
        string loginIdentifier,
        string authMode,
        CancellationToken ct)
    {
        var isDevelopment = IsDevelopmentEnvironment();
        Usuario? usuario = null;

        if (isDevelopment)
        {
            _logger.LogDebug("🔧 [DEV] LocalizarUsuarioAsync - Identifier: {Identifier}, AuthMode: {AuthMode}",
                loginIdentifier, authMode);
        }

        switch (authMode)
        {
            case "SaaS":
                // SaaS: buscar SOMENTE por email
                _logger.LogDebug("🔍 Buscando usuário por EMAIL (modo SaaS): {Email}", loginIdentifier);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email_Usuario == loginIdentifier, ct);
                break;

            case "Legacy":
                // Legacy: buscar por cdusuario OU email
                _logger.LogDebug("🔍 Buscando usuário por CDUSUARIO ou EMAIL (modo Legacy): {Identifier}", loginIdentifier);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.CdUsuario == loginIdentifier ||
                        u.Email_Usuario == loginIdentifier, ct);
                break;

            case "ADWin":
                // ADWin: buscar por cdusuario (deve corresponder ao AD)
                _logger.LogDebug("🔍 Buscando usuário por CDUSUARIO (modo ADWin): {CdUsuario}", loginIdentifier);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.CdUsuario == loginIdentifier, ct);
                break;

            default:
                _logger.LogWarning("⚠️ AuthMode desconhecido: '{AuthMode}'. Usando busca padrão por cdusuario.", authMode);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.CdUsuario == loginIdentifier, ct);
                break;
        }

        if (isDevelopment)
        {
            _logger.LogDebug("🔧 [DEV] Resultado LocalizarUsuarioAsync - Encontrado: {Encontrado}",
                usuario != null);
        }

        return usuario;
    }

    // ========================================================================
    // MÉTODOS AUXILIARES - MANTIDOS DO CÓDIGO ORIGINAL
    // ========================================================================

    /// <summary>
    /// Renova tokens JWT usando um refresh token válido.
    /// - Valida o refresh token.
    /// - Obtém UserSecurity e Usuário.
    /// - Verifica se usuário está ativo e não bloqueado.
    /// - Revoga o refresh token antigo.
    /// - Carrega permissões e gera novos tokens.
    /// </summary>
    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken ct = default)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        try
        {
            _logger.LogInformation("🔄 REFRESH: Validando refresh token");

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] RefreshTokenAsync - Request: {@Request}, IpAddress: {Ip}",
                    request, ipAddress);
            }

            // Validar refresh token
            var isValid = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, Guid.Empty, ct);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Resultado ValidateRefreshTokenAsync: {IsValid}", isValid);
            }

            if (!isValid)
            {
                _logger.LogWarning("❌ REFRESH: Token inválido ou expirado");
                return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Refresh token inválido ou expirado.");
            }

            // Buscar UserSecurity pelo token
            var userSecurity = await _db.Set<UserSecurity>()
                .FirstOrDefaultAsync(us => us.RefreshTokens.Any(rt => rt.TokenHash == request.RefreshToken), ct);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] UserSecurity para refresh token encontrado: {Encontrado}",
                    userSecurity != null);
            }

            // Buscar usuário associado ao UserSecurity
            if (userSecurity == null)
            {
                _logger.LogWarning("❌ REFRESH: UserSecurity não encontrado");
                return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Refresh token inválido.");
            }

            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userSecurity.IdUsuario, ct);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Usuário associado ao UserSecurity encontrado: {Encontrado}",
                    usuario != null);
            }

            if (usuario == null)
            {
                _logger.LogWarning("❌ REFRESH: Usuário não encontrado para UserSecurity {Id}", userSecurity.Id);
                return Result<AuthResponse>.Failure("USER_NOT_FOUND", "Usuário não encontrado.");
            }

            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("❌ REFRESH: Usuário {CdUsuario} INATIVO", usuario.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // Verificar lockout
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                _logger.LogWarning("🔒 REFRESH: Conta BLOQUEADA até {LockoutEnd}", userSecurity.LockoutEnd);
                return Result<AuthResponse>.Failure("ACCOUNT_LOCKED", "Conta bloqueada.");
            }

            // Revogar refresh token antigo
            await _jwtService.RevokeRefreshTokenAsync(
                request.RefreshToken,
                "Replaced by new token",
                ipAddress,
                ct);

            // Carregar permissões para o novo token
            UserPermissionsDto? permissions = null;
            try
            {
                permissions = await _permissaoService.CarregarPermissoesAsync(usuario.CdUsuario, null, ct);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Permissões carregadas no refresh: {@Permissions}", permissions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Erro ao carregar permissões no refresh token");
            }

            var newAccessToken = _jwtService.GenerateAccessToken(usuario, userSecurity, permissions);
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                null,
                null,
                null,
                ct);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Novos tokens gerados no refresh - AccessTokenLength: {Length}, RefreshTokenStart: {Start}",
                    newAccessToken?.Length,
                    !string.IsNullOrEmpty(newRefreshToken) && newRefreshToken.Length > 10
                        ? newRefreshToken[..10] + "..."
                        : newRefreshToken);
            }

            // Mapear informações do usuário
            var userInfo = new UserInfoDto
            {
                Id = usuario.Id,
                CdUsuario = usuario.CdUsuario,
                DcUsuario = usuario.DcUsuario,
                Email = usuario.Email_Usuario,
                NoMatric = usuario.NoMatric,
                CdEmpresa = usuario.CdEmpresa,
                CdFilial = usuario.CdFilial,
                TenantId = usuario.TenantId,
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900,
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("✅ REFRESH: Tokens renovados com sucesso para {CdUsuario}", usuario.CdUsuario);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] AuthResponse (refresh) final: {@Response}", response);
            }

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar refresh token");

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Exceção detalhada em RefreshTokenAsync: {Exception}", ex);
            }

            return Result<AuthResponse>.Failure("REFRESH_ERROR", "Erro ao processar refresh token.");
        }
    }

    /// <summary>
    /// Realiza logout revogando refresh tokens do usuário.
    /// - Se RevokeAllTokens = true: revoga todos os tokens do usuário.
    /// - Se for informado apenas um refresh token: revoga somente aquele token.
    /// </summary>
    public async Task<Result<bool>> LogoutAsync(
        string userId,
        LogoutRequest request,
        CancellationToken ct = default)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        try
        {
            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                return Result<bool>.Failure("INVALID_USER_ID", "ID de usuário inválido.");
            }

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] LogoutAsync - UserId: {UserId}, Request: {@Request}",
                    userId, request);
            }

            if (request.RevokeAllTokens)
            {
                _logger.LogInformation("🔓 LOGOUT: Revogando TODOS os tokens do usuário {UserId}", userId);

                var userSecurity = await _db.Set<UserSecurity>()
                    .FirstOrDefaultAsync(us => us.IdUsuario == userIdGuid, ct);

                if (userSecurity != null)
                {
                    await _jwtService.RevokeAllUserTokensAsync(
                        userSecurity.Id,
                        "unknown", // ipAddress não disponível no contexto
                        "User logout - all tokens",
                        ct);

                    // Regenerar security stamp para invalidar tokens JWT existentes
                    userSecurity.RegenerateSecurityStamp();
                    await _db.SaveChangesAsync(ct);

                    _logger.LogInformation("🔓 Todos os tokens do usuário foram revogados");

                    if (isDevelopment)
                    {
                        _logger.LogDebug("🔧 [DEV] SecurityStamp regenerado para UserSecurityId: {Id}", userSecurity.Id);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogInformation("🔓 LOGOUT: Revogando token específico para usuário {UserId}", userId);

                if (isDevelopment)
                {
                    _logger.LogDebug("🔧 [DEV] Revogando refresh token específico: {TokenStart}",
                        request.RefreshToken.Length > 10
                            ? request.RefreshToken[..10] + "..."
                            : request.RefreshToken);
                }

                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "User logout", "N/A", ct);
            }

            _logger.LogInformation("✅ Logout realizado com sucesso: {UserId}", userId);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar logout: {UserId}", userId);

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Exceção detalhada em LogoutAsync: {Exception}", ex);
            }

            return Result<bool>.Failure("LOGOUT_ERROR", "Erro ao processar logout.");
        }
    }

    /// <summary>
    /// Valida senha do usuário de acordo com a estratégia especificada.
    /// Suporta:
    /// - Legacy: texto plano (SenhaUser) ou BCrypt (PasswordHash em Usuario).
    /// - SaaS: BCrypt em UserSecurity.PasswordHash.
    /// - ADWin: autenticação via Active Directory com possíveis fallbacks (SaaS/Legacy).
    /// 
    /// Logs de DEBUG mais detalhados são emitidos apenas em ambiente Development.
    /// </summary>
    private bool ValidatePassword(
    Usuario usuario,
    UserSecurity userSecurity,
    string senha,
    string strategy)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        // Logs detalhados apenas em DEV
        if (isDevelopment)
        {
            _logger.LogDebug("🔐 [DEV] ValidatePassword INICIADO");
            _logger.LogDebug("🔐 [DEV] Strategy: {Strategy}, User: {Usuario}", strategy, usuario.CdUsuario);
            _logger.LogDebug("🔐 [DEV] Campos senha - PasswordHash: {PasswordHash}, SenhaUser: {SenhaUser}",
                !string.IsNullOrEmpty(usuario.PasswordHash) ? "PRESENTE" : "AUSENTE",
                !string.IsNullOrEmpty(usuario.SenhaUser) ? "PRESENTE" : "AUSENTE");
        }

        if (!_authSettings.Strategies.TryGetValue(strategy, out var strategyConfig))
        {
            _logger.LogError(
                "Estratégia '{Strategy}' não encontrada em ValidatePassword. Usando Legacy como fallback.",
                strategy);
            strategy = "Legacy";
            strategyConfig = _authSettings.Strategies[strategy];
        }

        switch (strategy)
        {
            case "Legacy":
            case "Legado": // ✅ SUPORTE PARA AMBAS AS FORMAS
                if (isDevelopment)
                {
                    _logger.LogDebug("🔐 [DEV] Modo Legacy - Iniciando validação");
                }

                // 1) Se já existe PasswordHash no usuário → SEMPRE usa BCrypt
                if (!string.IsNullOrWhiteSpace(usuario.PasswordHash))
                {
                    if (isDevelopment)
                    {
                        _logger.LogDebug("🔐 [DEV] Usando PasswordHash (BCrypt)");
                        _logger.LogDebug("🔐 [DEV] Hash armazenado (primeiros 20 chars): {Hash}...",
                            usuario.PasswordHash.Length > 20 ? usuario.PasswordHash.Substring(0, 20) : usuario.PasswordHash);
                    }

                    try
                    {
                        var result = BCryptNet.Verify(senha, usuario.PasswordHash);

                        if (isDevelopment)
                        {
                            _logger.LogDebug("🔐 [DEV] Resultado BCrypt: {Resultado}", result ? "VÁLIDO" : "INVÁLIDO");
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Erro ao validar PasswordHash com BCrypt para {User}", usuario.CdUsuario);
                        return false;
                    }
                }

                // 2) Se ainda está no modo legado (SenhaUser)
                if (!string.IsNullOrWhiteSpace(usuario.SenhaUser))
                {
                    // ✅ CORREÇÃO: Remover espaços em branco do campo CHAR
                    var senhaArmazenada = usuario.SenhaUser.Trim();
                    var senhaFornecida = senha.Trim();

                    if (isDevelopment)
                    {
                        _logger.LogDebug("🔐 [DEV] Usando SenhaUser (verificando múltiplos formatos)");
                        _logger.LogDebug("🔐 [DEV] Senha fornecida (length): {Length}", senhaFornecida.Length);
                        _logger.LogDebug("🔐 [DEV] Senha armazenada (length): {Length}", senhaArmazenada.Length);
                        _logger.LogDebug("🔐 [DEV] Senha armazenada (primeiros 10 chars): {Senha}...",
                            senhaArmazenada.Length > 10 ? senhaArmazenada.Substring(0, 10) : senhaArmazenada);
                    }

                    // 2.1) Tentar comparação direta (texto plano)
                    if (ConstantTimeEquals(senhaFornecida, senhaArmazenada))
                    {
                        if (isDevelopment)
                        {
                            _logger.LogDebug("✅ [DEV] Senha VÁLIDA - Comparação direta (texto plano)");
                        }
                        _logger.LogInformation("✅ LOGIN: Senha validada (texto plano) para {User}", usuario.CdUsuario);
                        return true;
                    }

                    // 2.2) Tentar MD5 (hexadecimal lowercase) - Comum em sistemas legados
                    try
                    {
                        var md5Hash = ComputeMd5Hash(senhaFornecida);

                        if (isDevelopment)
                        {
                            _logger.LogDebug("🔐 [DEV] MD5 da senha fornecida: {MD5}", md5Hash);
                        }

                        if (ConstantTimeEquals(md5Hash, senhaArmazenada))
                        {
                            if (isDevelopment)
                            {
                                _logger.LogDebug("✅ [DEV] Senha VÁLIDA - MD5 (lowercase hex)");
                            }
                            _logger.LogInformation("✅ LOGIN: Senha validada (MD5) para {User}", usuario.CdUsuario);
                            return true;
                        }

                        // 2.3) Tentar MD5 (hexadecimal UPPERCASE)
                        var md5HashUpper = md5Hash.ToUpperInvariant();

                        if (ConstantTimeEquals(md5HashUpper, senhaArmazenada))
                        {
                            if (isDevelopment)
                            {
                                _logger.LogDebug("✅ [DEV] Senha VÁLIDA - MD5 (uppercase hex)");
                            }
                            _logger.LogInformation("✅ LOGIN: Senha validada (MD5 uppercase) para {User}", usuario.CdUsuario);
                            return true;
                        }

                        // 2.4) Tentar MD5 em Base64
                        var md5Base64 = Convert.ToBase64String(
                            MD5.HashData(Encoding.UTF8.GetBytes(senhaFornecida))
                        );

                        if (isDevelopment)
                        {
                            _logger.LogDebug("🔐 [DEV] MD5-Base64 da senha fornecida: {MD5}", md5Base64);
                        }

                        if (ConstantTimeEquals(md5Base64, senhaArmazenada))
                        {
                            if (isDevelopment)
                            {
                                _logger.LogDebug("✅ [DEV] Senha VÁLIDA - MD5 (Base64)");
                            }
                            _logger.LogInformation("✅ LOGIN: Senha validada (MD5 Base64) para {User}", usuario.CdUsuario);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Erro ao tentar validar MD5 para {User}", usuario.CdUsuario);
                    }

                    // ❌ Nenhum formato funcionou
                    if (isDevelopment)
                    {
                        _logger.LogDebug("❌ [DEV] Senha INVÁLIDA - Nenhum formato correspondeu");
                        _logger.LogDebug("🔐 [DEV] Formatos testados: texto plano, MD5 (hex lower), MD5 (hex upper), MD5 (base64)");
                    }

                    _logger.LogWarning("❌ LOGIN: Senha inválida para {User} - Nenhum formato correspondeu", usuario.CdUsuario);
                    return false;
                }

                // ❌ Nenhum campo de senha disponível
                if (isDevelopment)
                {
                    _logger.LogDebug("❌ [DEV] NENHUM método de senha disponível");
                }

                _logger.LogWarning("❌ LOGIN: Usuário {User} não possui senha cadastrada", usuario.CdUsuario);
                return false;

            case "SaaS":
                if (isDevelopment)
                {
                    _logger.LogDebug("🔐 [DEV] Modo SaaS - Validando com UserSecurity.PasswordHash");
                }

                if (userSecurity == null || string.IsNullOrWhiteSpace(userSecurity.PasswordHash))
                {
                    if (isDevelopment)
                    {
                        _logger.LogDebug("❌ [DEV] UserSecurity ou PasswordHash não encontrado");
                    }
                    return false;
                }

                if (isDevelopment)
                {
                    _logger.LogDebug("🔐 [DEV] Hash UserSecurity (primeiros 20 chars): {Hash}...",
                        userSecurity.PasswordHash.Length > 20 ? userSecurity.PasswordHash.Substring(0, 20) : userSecurity.PasswordHash);
                }

                try
                {
                    var resultSaaS = BCryptNet.Verify(senha, userSecurity.PasswordHash);

                    if (isDevelopment)
                    {
                        _logger.LogDebug("🔐 [DEV] Resultado SaaS BCrypt: {Resultado}", resultSaaS ? "VÁLIDO" : "INVÁLIDO");
                    }

                    return resultSaaS;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao validar senha SaaS com BCrypt para {User}", usuario.CdUsuario);
                    return false;
                }

            case "ADWin":
                // Autenticação Active Directory
                _logger.LogInformation("🔐 ADWIN: Iniciando autenticação Active Directory");

                // Verificar se AD está disponível
                if (!_activeDirectoryService.IsAvailable())
                {
                    _logger.LogWarning("⚠️ ADWIN: AD não está disponível, tentando fallback para senha local");

                    // Fallback 1: Tentar SaaS
                    if (userSecurity != null && !string.IsNullOrWhiteSpace(userSecurity.PasswordHash))
                    {
                        _logger.LogInformation("🔄 ADWIN: Tentando fallback para SAAS");
                        return ValidatePassword(usuario, userSecurity, senha, "SaaS");
                    }

                    // Fallback 2: Tentar Legacy
                    if (!string.IsNullOrWhiteSpace(usuario.PasswordHash))
                    {
                        _logger.LogInformation("🔄 ADWIN: Tentando fallback para LEGACY (PasswordHash)");
                        try
                        {
                            return BCryptNet.Verify(senha, usuario.PasswordHash);
                        }
                        catch
                        {
                            // Continua para próximo fallback
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(usuario.SenhaUser))
                    {
                        _logger.LogInformation("🔄 ADWIN: Tentando fallback para LEGACY (SenhaUser)");
                        return ValidatePassword(usuario, userSecurity, senha, "Legacy");
                    }

                    _logger.LogError("❌ ADWIN: AD indisponível e sem fallback configurado");
                    return false;
                }

                // Autenticar no AD usando cdusuario
                try
                {
                    var isAdValid = _activeDirectoryService.AuthenticateAsync(
                        usuario.CdUsuario,
                        senha,
                        domain: null,
                        CancellationToken.None).GetAwaiter().GetResult();

                    if (isAdValid)
                    {
                        _logger.LogInformation("✅ ADWIN: Autenticação AD bem-sucedida para {CdUsuario}", usuario.CdUsuario);
                    }
                    else
                    {
                        _logger.LogWarning("❌ ADWIN: Autenticação AD falhou para {CdUsuario}", usuario.CdUsuario);
                    }

                    return isAdValid;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ ADWIN: Erro ao autenticar no AD");
                    return false;
                }

            default:
                _logger.LogError("❌ Estratégia desconhecida: {Strategy}", strategy);
                return false;
        }
    }

    /// <summary>
    /// Calcula hash MD5 de uma string (para compatibilidade com sistemas legados).
    /// </summary>
    private static string ComputeMd5Hash(string input)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }


    /// <summary>
    /// Método público de validação de senha para compatibilidade com outros componentes.
    /// Busca o usuário e seu UserSecurity e delega para ValidatePassword.
    /// </summary>
    public async Task<bool> ValidatePasswordAsync(
        string cdUsuario,
        string senha,
        string strategy,
        CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CdUsuario == cdUsuario, ct);

        if (usuario == null)
        {
            return false;
        }

        var userSecurity = await _db.Set<UserSecurity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

        if (userSecurity == null)
        {
            return false;
        }

        return ValidatePassword(usuario, userSecurity, senha, strategy);
    }

    /// <summary>
    /// Busca ou cria UserSecurity para usuário legado (migração automática).
    /// Caso não exista, cria um registro com hash gerado a partir da senha atual
    /// (ou senha temporária se SenhaUser estiver nulo).
    /// </summary>
    private async Task<UserSecurity> GetOrCreateUserSecurityAsync(Usuario usuario, CancellationToken ct)
    {
        var userSecurity = await _db.Set<UserSecurity>()
            .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

        if (userSecurity == null)
        {
            var passwordHash = !string.IsNullOrWhiteSpace(usuario.PasswordHash)
                ? usuario.PasswordHash
                : BCryptNet.HashPassword(usuario.SenhaUser ?? "ChangeMe@123");

            userSecurity = new UserSecurity(
                usuario.Id,
                usuario.TenantId,
                passwordHash,
                string.Empty);

            userSecurity.ConfirmEmail();

            _db.Set<UserSecurity>().Add(userSecurity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "UserSecurity criado automaticamente para usuário legado: {CdUsuario}",
                usuario.CdUsuario);
        }

        return userSecurity;
    }

    /// <summary>
    /// Atualiza UserSecurity no banco (lockout e tentativas de login).
    /// Usa ExecuteSqlRawAsync com parâmetros SqlParameter explícitos para máxima compatibilidade.
    /// </summary>
    private async Task UpdateUserSecurityInDatabaseAsync(UserSecurity userSecurity, CancellationToken ct)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        var parameters = new[]
        {
            new SqlParameter("@AccessFailedCount", userSecurity.AccessFailedCount),
            new SqlParameter("@LockoutEnd", userSecurity.LockoutEnd.HasValue ? userSecurity.LockoutEnd.Value : DBNull.Value),
            new SqlParameter("@UpdatedAt", _dateTimeProvider.UtcNow),
            new SqlParameter("@Id", userSecurity.Id),
            new SqlParameter("@ConcurrencyStamp", userSecurity.ConcurrencyStamp)
        };

        if (isDevelopment)
        {
            _logger.LogDebug("🔧 [DEV] UpdateUserSecurityInDatabaseAsync - Params: {@Params}", new
            {
                userSecurity.Id,
                userSecurity.AccessFailedCount,
                userSecurity.LockoutEnd,
                userSecurity.ConcurrencyStamp
            });
        }

        await _db.Database.ExecuteSqlRawAsync(
            @"UPDATE dbo.SEG_UserSecurity
              SET AccessFailedCount = @AccessFailedCount,
                  LockoutEnd = @LockoutEnd,
                  UpdatedAt = @UpdatedAt
              WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp;",
            parameters,
            ct);
    }

    /// <summary>
    /// Registra tentativa de login bem-sucedida no audit log.
    /// Não inclui [Id] no INSERT - SQL Server gera automaticamente via IDENTITY (bigint).
    /// </summary>
    private async Task RegisterSuccessfulLoginAsync(
        UserSecurity userSecurity,
        string ipAddress,
        string? userAgent,
        CancellationToken ct)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        try
        {
            var parameters = new[]
            {
                new SqlParameter("@IdUserSecurity", userSecurity.Id),
                new SqlParameter("@IdSaaS", userSecurity.IdSaaS.HasValue ? userSecurity.IdSaaS.Value : DBNull.Value),
                new SqlParameter("@IpAddress", !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : DBNull.Value),
                new SqlParameter("@UserAgent", !string.IsNullOrWhiteSpace(userAgent) ? userAgent : DBNull.Value),
                new SqlParameter("@LoginAttemptAt", _dateTimeProvider.UtcNow)
            };

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] RegisterSuccessfulLoginAsync - Params: {@Params}", new
                {
                    userSecurity.Id,
                    userSecurity.IdSaaS,
                    ipAddress,
                    userAgent
                });
            }

            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO [dbo].[SEG_LoginAuditLog] 
                      ([IdUserSecurity], [IdSaaS], [IsSuccess], [IpAddress], [UserAgent], [LoginAttemptAt])
                  VALUES 
                      (@IdUserSecurity, @IdSaaS, 1, @IpAddress, @UserAgent, @LoginAttemptAt);",
                parameters,
                ct);

            _logger.LogInformation("✅ LOGIN: Audit log registrado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ AUDIT: Erro ao registrar login no audit log (não crítico)");

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Exceção detalhada em RegisterSuccessfulLoginAsync: {Exception}", ex);
            }
        }
    }

    /// <summary>
    /// Registra tentativa de login falhada no audit log.
    /// Não inclui [Id] no INSERT - SQL Server gera automaticamente via IDENTITY (bigint).
    /// </summary>
    private async Task RegisterFailedLoginAsync(
        UserSecurity userSecurity,
        string ipAddress,
        string? userAgent,
        string? reason,
        CancellationToken ct)
    {
        var isDevelopment = IsDevelopmentEnvironment();

        try
        {
            var parameters = new[]
            {
                new SqlParameter("@IdUserSecurity", userSecurity.Id),
                new SqlParameter("@IdSaaS", userSecurity.IdSaaS.HasValue ? userSecurity.IdSaaS.Value : DBNull.Value),
                new SqlParameter("@IpAddress", !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : DBNull.Value),
                new SqlParameter("@UserAgent", !string.IsNullOrWhiteSpace(userAgent) ? userAgent : DBNull.Value),
                new SqlParameter("@FailureReason", !string.IsNullOrWhiteSpace(reason) ? reason : DBNull.Value),
                new SqlParameter("@LoginAttemptAt", _dateTimeProvider.UtcNow)
            };

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] RegisterFailedLoginAsync - Params: {@Params}", new
                {
                    userSecurity.Id,
                    userSecurity.IdSaaS,
                    ipAddress,
                    userAgent,
                    reason
                });
            }

            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO [dbo].[SEG_LoginAuditLog] 
                      ([IdUserSecurity], [IdSaaS], [IsSuccess], [IpAddress], [UserAgent], [FailureReason], [LoginAttemptAt])
                  VALUES 
                      (@IdUserSecurity, @IdSaaS, 0, @IpAddress, @UserAgent, @FailureReason, @LoginAttemptAt);",
                parameters,
                ct);

            _logger.LogInformation("✅ LOGIN: Falha registrada no audit log");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ AUDIT: Erro ao registrar falha no audit log (não crítico)");

            if (isDevelopment)
            {
                _logger.LogDebug("🔧 [DEV] Exceção detalhada em RegisterFailedLoginAsync: {Exception}", ex);
            }
        }
    }

    /// <summary>
    /// Comparação em tempo constante para strings (usada apenas como fallback legado).
    /// Evita vazamento de informação de timing em comparações de senhas.
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a is null || b is null)
        {
            return false;
        }

        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);

        if (aBytes.Length != bBytes.Length)
        {
            // Comparação "dummy" para consumir tempo similar e não vazar timing pelo tamanho
            CryptographicOperations.FixedTimeEquals(aBytes, aBytes);
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
