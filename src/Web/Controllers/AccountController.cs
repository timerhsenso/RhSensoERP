// =============================================================================
// RHSENSOERP WEB - ACCOUNT CONTROLLER
// =============================================================================
// CORREÇÃO v2.1: Claims ajustadas para multi-tenancy
// - ✅ Adicionados claims: tenantId e IdSaas
// =============================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Extensions;
using RhSensoERP.Web.Models.Account;
using RhSensoERP.Web.Services;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly IUserPermissionsCacheService _permissionsCache;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthApiService authApiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<AccountController> logger)
    {
        _authApiService = authApiService;
        _permissionsCache = permissionsCache;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var authResponse = await _authApiService.LoginAsync(model, ct);
            if (authResponse == null || authResponse.User == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View(model);
            }

            // ============================================================================
            // BUSCA E ARMAZENA AS PERMISSÕES NO CACHE
            // ============================================================================
            var userPermissions = await _authApiService.GetUserPermissionsAsync(authResponse.User.CdUsuario, null, ct);
            if (userPermissions != null)
            {
                var tokenLifetime = authResponse.ExpiresAt - DateTime.UtcNow;
                _permissionsCache.Set(authResponse.User.CdUsuario, userPermissions, tokenLifetime);
                _logger.LogInformation("Permissões do usuário {CdUsuario} armazenadas no cache.", authResponse.User.CdUsuario);
            }
            else
            {
                _logger.LogWarning("Não foi possível obter as permissões para o usuário {CdUsuario}.", authResponse.User.CdUsuario);
            }

            // =================================================================
            // CLAIMS DO USUÁRIO
            // =================================================================
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authResponse.User.Id.ToString()),
                new(ClaimTypes.Name, authResponse.User.CdUsuario),
                new("dcusuario", authResponse.User.DcUsuario),
                new("cdusuario", authResponse.User.CdUsuario),
                
                // ✅ NOVO: TenantId para multi-tenancy
                new("tenantId", authResponse.User.TenantId?.ToString() ?? Guid.Empty.ToString()),
                new("IdSaas", authResponse.User.TenantId?.ToString() ?? Guid.Empty.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : authResponse.ExpiresAt,
                AllowRefresh = true
            };

            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = authResponse.AccessToken },
                new AuthenticationToken { Name = "refresh_token", Value = authResponse.RefreshToken },
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            _logger.LogInformation("Login bem-sucedido: {CdUsuario}", model.CdUsuario);
            return RedirectToLocal(model.ReturnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login: {CdUsuario}", model.CdUsuario);
            ModelState.AddModelError(string.Empty, "Erro ao processar login. Tente novamente.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var cdUsuario = User.GetCdUsuario();
        if (!string.IsNullOrEmpty(cdUsuario))
        {
            _permissionsCache.Remove(cdUsuario);
            _logger.LogInformation("Permissões do usuário {CdUsuario} removidas do cache.", cdUsuario);
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Usuário deslogado com sucesso.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var cdUsuario = User.GetCdUsuario();
        _logger.LogInformation("Usuário {CdUsuario} acessando Dashboard", cdUsuario);

        var viewModel = new DashboardViewModel
        {
            UserInfo = new UserInfoViewModel
            {
                CdUsuario = cdUsuario ?? string.Empty,
                DcUsuario = User.FindFirst("dcusuario")?.Value ?? User.Identity?.Name ?? string.Empty,
                Id = Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty
            }
        };

        if (!string.IsNullOrEmpty(cdUsuario))
        {
            try
            {
                var permissions = await _permissionsCache.GetOrFetchAsync(cdUsuario, ct);

                if (permissions != null)
                {
                    viewModel.Permissions = permissions;
                    _logger.LogDebug("Dashboard carregado com {Funcoes} funções e {Botoes} botões",
                        permissions.Funcoes.Count,
                        permissions.Botoes.Count);
                }
                else
                {
                    viewModel.HasPermissionsError = true;
                    viewModel.ErrorMessage = "Não foi possível carregar as permissões. Faça login novamente.";
                    _logger.LogWarning("Permissões não encontradas para {CdUsuario}", cdUsuario);
                }
            }
            catch (Exception ex)
            {
                viewModel.HasPermissionsError = true;
                viewModel.ErrorMessage = "Erro ao carregar permissões. Tente novamente.";
                _logger.LogError(ex, "Erro ao obter permissões para {CdUsuario}", cdUsuario);
            }
        }
        else
        {
            viewModel.HasPermissionsError = true;
            viewModel.ErrorMessage = "Usuário não identificado.";
        }

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}