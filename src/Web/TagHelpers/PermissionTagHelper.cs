// =============================================================================
// RHSENSOERP WEB - PERMISSION TAG HELPER (ATUALIZADO COM CACHE DE PERMISSÕES)
// =============================================================================
using Microsoft.AspNetCore.Razor.TagHelpers;
using RhSensoERP.Web.Extensions;
using RhSensoERP.Web.Services.Permissions; // ✅ Adicionado

namespace RhSensoERP.Web.TagHelpers;

[HtmlTargetElement(Attributes = "permission-function,permission-action")]
public class PermissionTagHelper : TagHelper
{
    private readonly IUserPermissionsCacheService _permissionsCache; // ✅ Injetado
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionTagHelper(IUserPermissionsCacheService permissionsCache, IHttpContextAccessor httpContextAccessor)
    {
        _permissionsCache = permissionsCache;
        _httpContextAccessor = httpContextAccessor;
    }

    [HtmlAttributeName("permission-function")]
    public string? PermissionFunction { get; set; }

    [HtmlAttributeName("permission-action")]
    public char PermissionAction { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(PermissionFunction))
        {
            output.SuppressOutput();
            return;
        }

        var user = _httpContextAccessor.HttpContext?.User;
        var cdUsuario = user?.GetCdUsuario();

        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            output.SuppressOutput();
            return;
        }

        // ✅ Verifica a permissão usando o serviço de cache
        var hasPermission = await _permissionsCache.HasPermissionAsync(cdUsuario, PermissionFunction, PermissionAction);

        if (!hasPermission)
        {
            output.SuppressOutput();
        }

        output.Attributes.RemoveAll("permission-function");
        output.Attributes.RemoveAll("permission-action");
    }
}
