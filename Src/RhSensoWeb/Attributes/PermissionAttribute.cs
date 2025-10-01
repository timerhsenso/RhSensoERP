using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RhSensoWeb.Extensions;

namespace RhSensoWeb.Attributes;

/// <summary>
/// Atributo para controle de permissões em actions
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _permission;
    private readonly string[]? _permissions;
    private readonly bool _requireAll;

    /// <summary>
    /// Construtor para permissão única
    /// </summary>
    /// <param name="permission">Permissão necessária</param>
    public PermissionAttribute(string permission)
    {
        _permission = permission;
        _requireAll = false;
    }

    /// <summary>
    /// Construtor para múltiplas permissões
    /// </summary>
    /// <param name="permissions">Lista de permissões</param>
    /// <param name="requireAll">Se true, todas as permissões são necessárias. Se false, qualquer uma é suficiente</param>
    public PermissionAttribute(string[] permissions, bool requireAll = false)
    {
        _permissions = permissions;
        _requireAll = requireAll;
        _permission = string.Empty;
    }

    /// <summary>
    /// Executa a verificação de autorização
    /// </summary>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verificar se o usuário está autenticado
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var user = context.HttpContext.User;
        bool hasPermission = false;

        // Verificar permissão única
        if (!string.IsNullOrEmpty(_permission))
        {
            hasPermission = user.HasPermission(_permission);
        }
        // Verificar múltiplas permissões
        else if (_permissions != null && _permissions.Length > 0)
        {
            if (_requireAll)
            {
                hasPermission = user.HasAllPermissions(_permissions);
            }
            else
            {
                hasPermission = user.HasAnyPermission(_permissions);
            }
        }
        else
        {
            // Nenhuma permissão especificada - permitir
            hasPermission = true;
        }

        // Se não tem permissão, redirecionar para acesso negado
        if (!hasPermission)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
        }
    }
}

/// <summary>
/// Atributo para permissões IAEC (Incluir, Alterar, Excluir, Consultar)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IaecPermissionAttribute : PermissionAttribute
{
    /// <summary>
    /// Construtor para permissão IAEC
    /// </summary>
    /// <param name="basePermission">Permissão base (ex: "SEG.SEG_USUARIOS")</param>
    /// <param name="operation">Operação IAEC (I, A, E, C)</param>
    public IaecPermissionAttribute(string basePermission, string operation) 
        : base($"{basePermission}.{operation.ToUpper()}")
    {
    }
}

/// <summary>
/// Atributo para verificar tipo de usuário
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UserTypeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedTypes;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="allowedTypes">Tipos de usuário permitidos</param>
    public UserTypeAttribute(params string[] allowedTypes)
    {
        _allowedTypes = allowedTypes;
    }

    /// <summary>
    /// Executa a verificação de autorização
    /// </summary>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verificar se o usuário está autenticado
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var userType = context.HttpContext.User.GetUserType();
        
        if (!_allowedTypes.Contains(userType, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
        }
    }
}

/// <summary>
/// Atributo para verificar se é administrador
/// </summary>
public class AdminOnlyAttribute : UserTypeAttribute
{
    public AdminOnlyAttribute() : base("ADMIN")
    {
    }
}
