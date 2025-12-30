// =============================================================================
// GERADOR FULL-STACK v3.7 - WEB CONTROLLER TEMPLATE (COM TOGGLE ATIVO)
// Baseado em RhSensoERP.CrudTool v2.0
// v3.7 - ✅ NOVO: Action ToggleAtivo para alternar status Ativo/Desativo
// v3.6 - CORREÇÃO: Usa entity.Module diretamente ao invés de mapear CdSistema
// v3.2 - Organiza Controllers por módulo
// =============================================================================

using GeradorEntidades.Models;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Controller Web que herda de BaseCrudController.
/// v3.7: Adiciona action ToggleAtivo para alternar status de forma dinâmica.
/// v3.6: Usa módulo correto do entity (que vem do manifesto/request).
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

        // ✅ v3.6: USA O MÓDULO CORRETO DO ENTITY (que vem do manifesto/request)
        // Não usa GetMenuModule(CdSistema) que pode estar errado
        var menuModule = entity.Module;

        // Caminho da View organizada por módulo
        var viewPath = $"Views/{modulePath}/{entity.Name}/Index.cshtml";

        // v3.7: Verifica se tem campo "Ativo"
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase));

        var toggleAtivoAction = hasAtivoField ? GenerateToggleAtivoAction(entity, pkType) : "";

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.7
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
        // Verifica permissão de consulta
        if (!await CanViewAsync(CdFuncao, ct))
        {{
            _logger.LogWarning(
                ""Acesso negado: Usuário {{User}} tentou acessar {{Funcao}} sem permissão"",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction(""AccessDenied"", ""Account"");
        }}

        // Busca permissões do usuário para esta função
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

        // View organizada por módulo: Views/Module/Entity/Index.cshtml
        return View(""~/{viewPath}"", viewModel);
    }}

    // =========================================================================
    // ACTION: GET BY ID (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Busca registro por ID via AJAX.
    /// </summary>
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
    // ACTION: CREATE (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
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
    // ACTION: EDIT (POST para compatibilidade com CrudBase.js)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via POST.
    /// CrudBase.js envia para /Edit?id=xxx via POST.
    /// </summary>
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
    // ACTION: UPDATE (PUT padrão REST)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via PUT.
    /// </summary>
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
    // ACTION: DELETE (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Exclui um registro.
    /// CrudBase.js envia DELETE para /Delete?id=xxx
    /// </summary>
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
    // ACTION: DELETE MULTIPLE (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Exclui múltiplos registros.
    /// </summary>
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
    /// v3.7: Gera action ToggleAtivo para alternar status Ativo/Desativo dinamicamente.
    /// </summary>
    private static string GenerateToggleAtivoAction(EntityConfig entity, string pkType)
    {
        return $@"

    // =========================================================================
    // v3.7: ACTION - TOGGLE ATIVO (Alternar Status Ativo/Desativo)
    // =========================================================================

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro via AJAX.
    /// Rate limit implementado no client-side (debounce 500ms).
    /// </summary>
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

            // Chama o serviço para alternar o status
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

    /// <summary>
    /// Request para alternar status Ativo.
    /// </summary>
    public class ToggleAtivoRequest
    {{
        public {pkType} Id {{ get; set; }}
        public bool Ativo {{ get; set; }}
    }}";
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