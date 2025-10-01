using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoWeb.Extensions;
using RhSensoWeb.Models.Auth;
using RhSensoWeb.Services.Interfaces;

namespace RhSensoWeb.Controllers;

/// <summary>
/// Controller para autenticação e autorização
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthApiService authApiService, ILogger<AuthController> logger)
    {
        _authApiService = authApiService;
        _logger = logger;
    }

    /// <summary>
    /// Exibe a tela de login
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // Se já está autenticado, redirecionar
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    /// <summary>
    /// Processa o login do usuário
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        model.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            _logger.LogInformation("Tentativa de login para usuário {Usuario}", model.CdUsuario);

            // Preparar requisição para API
            var loginRequest = new LoginRequestDto
            {
                CdUsuario = model.CdUsuario,
                Senha = model.Senha,
                Dominio = model.Dominio
            };

            // Chamar API de login
            var response = await _authApiService.LoginAsync(loginRequest);

            if (response.Success && response.Data != null)
            {
                // Criar sessão do usuário
                var userSession = new UserSessionModel
                {
                    CdUsuario = response.Data.UserData.CdUsuario,
                    DcUsuario = response.Data.UserData.DcUsuario,
                    EmailUsuario = response.Data.UserData.EmailUsuario,
                    TpUsuario = response.Data.UserData.TpUsuario,
                    FlAtivo = response.Data.UserData.FlAtivo,
                    CdEmpresa = response.Data.UserData.CdEmpresa,
                    CdFilial = response.Data.UserData.CdFilial,
                    IdSaas = response.Data.UserData.IdSaas,
                    AccessToken = response.Data.AccessToken,
                    Groups = response.Data.Groups,
                    Permissions = response.Data.Permissions,
                    LoginTime = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow
                };

                // Criar claims do usuário
                var claims = userSession.ToClaims();
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Configurar propriedades do cookie
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe 
                        ? DateTimeOffset.UtcNow.AddDays(30) 
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                // Fazer login (criar cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);

                _logger.LogInformation("Login realizado com sucesso para usuário {Usuario}", model.CdUsuario);

                // Redirecionar para página solicitada ou dashboard
                return RedirectToLocal(returnUrl);
            }
            else
            {
                _logger.LogWarning("Login falhou para usuário {Usuario}: {Message}", 
                    model.CdUsuario, response.Message);

                model.ErrorMessage = response.Message ?? "Usuário ou senha inválidos";
                ModelState.AddModelError(string.Empty, model.ErrorMessage);
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante login do usuário {Usuario}", model.CdUsuario);
            
            model.ErrorMessage = "Erro interno do sistema. Tente novamente.";
            ModelState.AddModelError(string.Empty, model.ErrorMessage);
            return View(model);
        }
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = User.GetUserId();
            var token = User.GetAccessToken();

            _logger.LogInformation("Logout iniciado para usuário {Usuario}", userId);

            // Chamar API de logout (opcional)
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    await _authApiService.LogoutAsync(token);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erro ao chamar logout da API para usuário {Usuario}", userId);
                    // Continuar com logout local mesmo se API falhar
                }
            }

            // Fazer logout local (remover cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Logout realizado com sucesso para usuário {Usuario}", userId);

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return RedirectToAction(nameof(Login));
        }
    }

    /// <summary>
    /// Página de acesso negado
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// Endpoint para verificar se usuário está autenticado (AJAX)
    /// </summary>
    [HttpGet]
    public IActionResult CheckAuth()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userSession = User.GetUserSession();
            return Json(new
            {
                authenticated = true,
                user = new
                {
                    cdUsuario = userSession.CdUsuario,
                    dcUsuario = userSession.DcUsuario,
                    emailUsuario = userSession.EmailUsuario,
                    tpUsuario = userSession.TpUsuario
                }
            });
        }

        return Json(new { authenticated = false });
    }

    /// <summary>
    /// Endpoint para renovar sessão (AJAX)
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RefreshSession()
    {
        try
        {
            var token = User.GetAccessToken();
            
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Token não encontrado" });
            }

            // Validar token atual
            var isValid = await _authApiService.ValidateTokenAsync(token);
            
            if (!isValid)
            {
                return Json(new { success = false, message = "Sessão expirada" });
            }

            // Atualizar última atividade
            var claims = User.Claims.ToList();
            var lastActivityClaim = claims.FirstOrDefault(c => c.Type == "LastActivity");
            
            if (lastActivityClaim != null)
            {
                claims.Remove(lastActivityClaim);
            }
            
            claims.Add(new Claim("LastActivity", DateTime.UtcNow.ToString("O")));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);

            return Json(new { success = true, message = "Sessão renovada" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar sessão");
            return Json(new { success = false, message = "Erro interno" });
        }
    }

    /// <summary>
    /// Redireciona para URL local ou dashboard
    /// </summary>
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
