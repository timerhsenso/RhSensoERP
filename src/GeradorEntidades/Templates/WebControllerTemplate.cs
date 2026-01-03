// =============================================================================
// GERADOR FULL-STACK v4.0 - WEB CONTROLLER TEMPLATE
// ⭐ v4.0 - SELECT2 LOOKUP AUTOMÁTICO
// v3.7 - Action ToggleAtivo para alternar status Ativo/Desativo
// v3.6 - Usa entity.Module diretamente
// =============================================================================
// ⭐ COPIE E COLE ESTE ARQUIVO COMPLETO SUBSTITUINDO O ORIGINAL
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Controller Web que herda de BaseCrudController.
/// v4.0: Geração automática de actions para Select2 Lookup.
/// v3.7: Adiciona action ToggleAtivo.
/// </summary>
public static class WebControllerTemplate
{
    /// <summary>
    /// Gera o Controller Web completo.
    /// </summary>
    public static GeneratedFile Generate(EntityConfig entity)
    {
        var pkType = entity.PkTypeSimple;
        var modulePath = GetModulePath(entity.Module);
        var menuModule = entity.Module;
        var viewPath = $"Views/{modulePath}/{entity.Name}/Index.cshtml";

        // v3.7: Verifica se tem campo "Ativo"
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase));

        var toggleAtivoAction = hasAtivoField ? GenerateToggleAtivoAction(entity, pkType) : "";

        // ⭐ v4.0: Gera actions de Select2 Lookup
        var select2Actions = GenerateSelect2LookupActions(entity);

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: {entity.Name}
// Module: {entity.Module}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Attributes;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.{modulePath}.{entity.Name};
using RhSensoERP.Web.Services.{modulePath}.{entity.Name};
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers.{modulePath};

// =============================================================================
// MENU ITEM - Configuração para aparecer no menu dinâmico
// =============================================================================
[MenuItem(
    Module = MenuModule.{menuModule},
    DisplayName = ""{entity.DisplayName}"",
    Icon = ""{entity.Icon}"",
    Order = {entity.MenuOrder},
    CdFuncao = ""{entity.CdFuncao}""
)]

/// <summary>
/// Controller para gerenciamento de {entity.DisplayName}.
/// Herda toda a funcionalidade CRUD de BaseCrudController.
/// </summary>
[Authorize]
public class {entity.Name}Controller 
    : BaseCrudController<{entity.Name}Dto, Create{entity.Name}Request, Update{entity.Name}Request, {pkType}>
{{
    // =========================================================================
    // CONFIGURAÇÃO DE PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código da função/tela no sistema de permissões.
    /// Corresponde ao cadastrado na tabela tfunc1 do banco legado.
    /// </summary>
    private const string CdFuncao = ""{entity.CdFuncao}"";

    /// <summary>
    /// Código do sistema ao qual esta função pertence.
    /// </summary>
    private const string CdSistema = ""{entity.CdSistema}"";

    private readonly I{entity.Name}ApiService _{entity.NameLower}Service;

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public {entity.Name}Controller(
        I{entity.Name}ApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<{entity.Name}Controller> logger)
        : base(apiService, permissionsCache, logger)
    {{
        _{entity.NameLower}Service = apiService;
    }}

    // =========================================================================
    // ACTION: INDEX (Página Principal)
    // =========================================================================

    /// <summary>
    /// Página principal com verificação de permissão de consulta.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {{
        if (!await CanViewAsync(CdFuncao, ct))
        {{
            _logger.LogWarning(
                ""Acesso negado: Usuário {{User}} tentou acessar {{Funcao}} sem permissão"",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction(""AccessDenied"", ""Account"");
        }}

        var permissions = await GetUserPermissionsAsync(CdFuncao, ct);

        var viewModel = new {entity.Name}ListViewModel
        {{
            UserPermissions = permissions
        }};

        _logger.LogDebug(
            ""Usuário {{User}} acessou {{Funcao}} | Permissões: {{Permissions}}"",
            User.Identity?.Name,
            CdFuncao,
            permissions);

        return View(""~/{viewPath}"", viewModel);
    }}

    // =========================================================================
    // ACTION: GET BY ID (Sobrescrito para verificar permissão)
    // =========================================================================

    [HttpGet]
    public override async Task<IActionResult> GetById({pkType} id)
    {{
        if (!await CanViewAsync(CdFuncao))
        {{
            return JsonError(""Você não tem permissão para visualizar registros."");
        }}

        return await base.GetById(id);
    }}

    // =========================================================================
    // ACTION: CREATE
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] Create{entity.Name}Request dto)
    {{
        if (!await CanCreateAsync(CdFuncao))
        {{
            _logger.LogWarning(
                ""Tentativa de inclusão negada: Usuário {{User}} sem permissão 'I' em {{Funcao}}"",
                User.Identity?.Name,
                CdFuncao);

            return JsonError(""Você não tem permissão para criar registros nesta tela."");
        }}

        _logger.LogInformation(
            ""Usuário {{User}} criando registro em {{Funcao}}"",
            User.Identity?.Name,
            CdFuncao);

        return await base.Create(dto);
    }}

    // =========================================================================
    // ACTION: EDIT
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] {pkType} id, [FromBody] Update{entity.Name}Request dto)
    {{
        if (EqualityComparer<{pkType}>.Default.Equals(id, default))
        {{
            return JsonError(""ID do registro não informado."");
        }}

        if (!await CanEditAsync(CdFuncao))
        {{
            _logger.LogWarning(
                ""Tentativa de alteração negada: Usuário {{User}} sem permissão 'A' em {{Funcao}}"",
                User.Identity?.Name,
                CdFuncao);

            return JsonError(""Você não tem permissão para alterar registros nesta tela."");
        }}

        _logger.LogInformation(
            ""Usuário {{User}} alterando registro {{Id}} em {{Funcao}}"",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }}

    // =========================================================================
    // ACTION: UPDATE
    // =========================================================================

    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update({pkType} id, [FromBody] Update{entity.Name}Request dto)
    {{
        if (!await CanEditAsync(CdFuncao))
        {{
            return JsonError(""Você não tem permissão para alterar registros nesta tela."");
        }}

        return await base.Update(id, dto);
    }}

    // =========================================================================
    // ACTION: DELETE
    // =========================================================================

    [HttpPost]
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete({pkType} id)
    {{
        if (!await CanDeleteAsync(CdFuncao))
        {{
            _logger.LogWarning(
                ""Tentativa de exclusão negada: Usuário {{User}} sem permissão 'E' em {{Funcao}}"",
                User.Identity?.Name,
                CdFuncao);

            return JsonError(""Você não tem permissão para excluir registros nesta tela."");
        }}

        _logger.LogInformation(
            ""Usuário {{User}} excluindo registro {{Id}} em {{Funcao}}"",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Delete(id);
    }}

    // =========================================================================
    // ACTION: DELETE MULTIPLE
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<{pkType}> ids)
    {{
        if (ids == null || ids.Count == 0)
        {{
            return JsonError(""Nenhum registro selecionado para exclusão."");
        }}

        if (!await CanDeleteAsync(CdFuncao))
        {{
            _logger.LogWarning(
                ""Tentativa de exclusão múltipla negada: Usuário {{User}} sem permissão 'E' em {{Funcao}}"",
                User.Identity?.Name,
                CdFuncao);

            return JsonError(""Você não tem permissão para excluir registros nesta tela."");
        }}

        _logger.LogInformation(
            ""Usuário {{User}} excluindo {{Count}} registros em {{Funcao}}"",
            User.Identity?.Name,
            ids.Count,
            CdFuncao);

        return await base.DeleteMultiple(ids);
    }}
{toggleAtivoAction}
{select2Actions}
}}
";

        return new GeneratedFile
        {
            FileName = $"{entity.Name}Controller.cs",
            RelativePath = $"Web/Controllers/{modulePath}/{entity.Name}Controller.cs",
            Content = content,
            FileType = "Controller"
        };
    }

    /// <summary>
    /// v3.7: Gera action ToggleAtivo.
    /// </summary>
    private static string GenerateToggleAtivoAction(EntityConfig entity, string pkType)
    {
        return $@"

    // =========================================================================
    // v3.7: ACTION - TOGGLE ATIVO (Alternar Status Ativo/Desativo)
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAtivo([FromBody] ToggleAtivoRequest request, CancellationToken ct = default)
    {{
        if (request == null)
        {{
            return JsonError(""Requisição inválida."");
        }}

        if (!await CanEditAsync(CdFuncao, ct))
        {{
            _logger.LogWarning(
                ""Tentativa de alteração de status negada: Usuário {{User}} sem permissão 'A' em {{Funcao}}"",
                User.Identity?.Name,
                CdFuncao);

            return JsonError(""Você não tem permissão para alterar registros nesta tela."");
        }}

        try
        {{
            _logger.LogInformation(
                ""Usuário {{User}} alterando status Ativo do registro {{Id}} para {{Ativo}} em {{Funcao}}"",
                User.Identity?.Name,
                request.Id,
                request.Ativo,
                CdFuncao);

            await _{entity.NameLower}Service.ToggleAtivoAsync(request.Id, request.Ativo, ct);

            var mensagem = request.Ativo
                ? ""Registro ativado com sucesso!""
                : ""Registro desativado com sucesso!"";

            return JsonSuccess(mensagem);
        }}
        catch (KeyNotFoundException ex)
        {{
            _logger.LogWarning(
                ex,
                ""Registro {{Id}} não encontrado ao tentar alterar status Ativo"",
                request.Id);

            return JsonError(""Registro não encontrado."");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(
                ex,
                ""Erro ao alternar status Ativo do registro {{Id}} em {{Funcao}}"",
                request.Id,
                CdFuncao);

            return JsonError(""Erro ao atualizar status do registro."");
        }}
    }}

    public class ToggleAtivoRequest
    {{
        public {pkType} Id {{ get; set; }}
        public bool Ativo {{ get; set; }}
    }}";
    }

    // =========================================================================
    // ⭐ v4.0: SELECT2 LOOKUP - GERAÇÃO AUTOMÁTICA DE ACTIONS
    // =========================================================================

    /// <summary>
    /// ⭐ v4.0: Gera actions de lookup para Select2.
    /// </summary>
    private static string GenerateSelect2LookupActions(EntityConfig entity)
    {
        if (!entity.Select2Lookups.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // ⭐ v4.0: ACTIONS - LOOKUPS PARA SELECT2 (GERADO AUTOMATICAMENTE)");
        sb.AppendLine("    // =========================================================================");

        foreach (var lookup in entity.Select2Lookups)
        {
            var valuePascal = ToPascalCase(lookup.ValueField);
            var textPascal = ToPascalCase(lookup.TextField);

            sb.AppendLine($@"
    /// <summary>
    /// Retorna lista de {lookup.DisplayName} para preencher Select2 via AJAX.
    /// Gerado automaticamente para o campo {lookup.PropertyName}.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> {lookup.MethodName}(
        [FromQuery] string search = """", 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {{
        try
        {{
            if (!await CanViewAsync(CdFuncao, ct))
            {{
                return Json(new
                {{
                    success = false,
                    message = ""Você não tem permissão para visualizar {lookup.DisplayName}."",
                    items = new List<object>()
                }});
            }}

            var items = await _{entity.NameLower}Service.{lookup.MethodName}Async(search, page, pageSize, ct);

            return Json(new
            {{
                success = true,
                items = items.Select(x => new
                {{
                    id = x.{valuePascal},
                    text = x.{textPascal}
                }}).ToList(),
                pagination = new
                {{
                    more = items.Count >= pageSize
                }}
            }});
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""Erro ao buscar {lookup.DisplayName} para Select2"");
            return Json(new
            {{
                success = false,
                message = ""Erro ao carregar {lookup.DisplayName}."",
                items = new List<object>()
            }});
        }}
    }}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Helper: Converte para PascalCase.
    /// </summary>
    private static string ToPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        return char.ToUpper(text[0]) + text.Substring(1);
    }

    /// <summary>
    /// Converte nome do módulo para path de pasta.
    /// </summary>
    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "Common";

        return moduleName;
    }
}