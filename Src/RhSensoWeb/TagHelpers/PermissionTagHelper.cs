using Microsoft.AspNetCore.Razor.TagHelpers;
using RhSensoWeb.Extensions;

namespace RhSensoWeb.TagHelpers;

/// <summary>
/// TagHelper para controle de permissões em elementos HTML
/// </summary>
[HtmlTargetElement("*", Attributes = "permission")]
[HtmlTargetElement("*", Attributes = "permission-any")]
[HtmlTargetElement("*", Attributes = "permission-all")]
public class PermissionTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionTagHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Permissão única necessária
    /// </summary>
    [HtmlAttributeName("permission")]
    public string? Permission { get; set; }

    /// <summary>
    /// Lista de permissões (qualquer uma é suficiente)
    /// </summary>
    [HtmlAttributeName("permission-any")]
    public string? PermissionAny { get; set; }

    /// <summary>
    /// Lista de permissões (todas são necessárias)
    /// </summary>
    [HtmlAttributeName("permission-all")]
    public string? PermissionAll { get; set; }

    /// <summary>
    /// Se true, inverte a lógica (mostra quando NÃO tem permissão)
    /// </summary>
    [HtmlAttributeName("permission-negate")]
    public bool Negate { get; set; } = false;

    /// <summary>
    /// Ordem de execução do TagHelper
    /// </summary>
    public override int Order => -1000;

    /// <summary>
    /// Processa o elemento baseado nas permissões
    /// </summary>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            // Usuário não autenticado - remover elemento
            output.SuppressOutput();
            return;
        }

        var user = httpContext.User;
        bool hasPermission = false;

        // Verificar permissão única
        if (!string.IsNullOrEmpty(Permission))
        {
            hasPermission = user.HasPermission(Permission);
        }
        // Verificar qualquer permissão da lista
        else if (!string.IsNullOrEmpty(PermissionAny))
        {
            var permissions = PermissionAny.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim())
                                          .ToArray();
            hasPermission = user.HasAnyPermission(permissions);
        }
        // Verificar todas as permissões da lista
        else if (!string.IsNullOrEmpty(PermissionAll))
        {
            var permissions = PermissionAll.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim())
                                          .ToArray();
            hasPermission = user.HasAllPermissions(permissions);
        }
        else
        {
            // Nenhuma permissão especificada - permitir
            hasPermission = true;
        }

        // Aplicar negação se necessário
        if (Negate)
        {
            hasPermission = !hasPermission;
        }

        // Remover elemento se não tem permissão
        if (!hasPermission)
        {
            output.SuppressOutput();
        }
        else
        {
            // Remover atributos de permissão do output final
            output.Attributes.RemoveAll("permission");
            output.Attributes.RemoveAll("permission-any");
            output.Attributes.RemoveAll("permission-all");
            output.Attributes.RemoveAll("permission-negate");
        }
    }
}

/// <summary>
/// TagHelper específico para botões com permissões IAEC
/// </summary>
[HtmlTargetElement("button", Attributes = "iaec-permission")]
[HtmlTargetElement("a", Attributes = "iaec-permission")]
public class IaecPermissionTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IaecPermissionTagHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Permissão base (ex: "SEG.SEG_USUARIOS")
    /// </summary>
    [HtmlAttributeName("iaec-permission")]
    public string? BasePermission { get; set; }

    /// <summary>
    /// Tipo de operação IAEC (I, A, E, C)
    /// </summary>
    [HtmlAttributeName("iaec-operation")]
    public string? Operation { get; set; }

    /// <summary>
    /// Ordem de execução do TagHelper
    /// </summary>
    public override int Order => -1000;

    /// <summary>
    /// Processa o elemento baseado na permissão IAEC
    /// </summary>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrEmpty(BasePermission) || string.IsNullOrEmpty(Operation))
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            output.SuppressOutput();
            return;
        }

        var fullPermission = $"{BasePermission}.{Operation.ToUpper()}";
        var hasPermission = httpContext.User.HasPermission(fullPermission);

        if (!hasPermission)
        {
            output.SuppressOutput();
        }
        else
        {
            // Remover atributos do output final
            output.Attributes.RemoveAll("iaec-permission");
            output.Attributes.RemoveAll("iaec-operation");
        }
    }
}

/// <summary>
/// TagHelper para mostrar/ocultar baseado no tipo de usuário
/// </summary>
[HtmlTargetElement("*", Attributes = "user-type")]
[HtmlTargetElement("*", Attributes = "user-type-any")]
public class UserTypeTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserTypeTagHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Tipo de usuário necessário
    /// </summary>
    [HtmlAttributeName("user-type")]
    public string? UserType { get; set; }

    /// <summary>
    /// Lista de tipos de usuário (qualquer um é suficiente)
    /// </summary>
    [HtmlAttributeName("user-type-any")]
    public string? UserTypeAny { get; set; }

    /// <summary>
    /// Ordem de execução do TagHelper
    /// </summary>
    public override int Order => -1000;

    /// <summary>
    /// Processa o elemento baseado no tipo de usuário
    /// </summary>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            output.SuppressOutput();
            return;
        }

        var currentUserType = httpContext.User.GetUserType();
        bool hasAccess = false;

        if (!string.IsNullOrEmpty(UserType))
        {
            hasAccess = string.Equals(currentUserType, UserType, StringComparison.OrdinalIgnoreCase);
        }
        else if (!string.IsNullOrEmpty(UserTypeAny))
        {
            var allowedTypes = UserTypeAny.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(t => t.Trim())
                                         .ToArray();
            hasAccess = allowedTypes.Any(t => string.Equals(currentUserType, t, StringComparison.OrdinalIgnoreCase));
        }

        if (!hasAccess)
        {
            output.SuppressOutput();
        }
        else
        {
            // Remover atributos do output final
            output.Attributes.RemoveAll("user-type");
            output.Attributes.RemoveAll("user-type-any");
        }
    }
}
