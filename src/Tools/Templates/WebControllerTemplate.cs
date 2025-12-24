// =============================================================================
// RHSENSOERP CRUD TOOL - WEB CONTROLLER TEMPLATE
// Versão: 2.0 - Compatível com BaseCrudController existente
// =============================================================================
using RhSensoERP.CrudTool.Models;

namespace RhSensoERP.CrudTool.Templates;

/// <summary>
/// Gera Controller Web que herda de BaseCrudController.
/// Adiciona apenas a lógica de permissões específica da entidade.
/// </summary>
public static class WebControllerTemplate
{
    /// <summary>
    /// Gera o Controller Web completo.
    /// </summary>
    public static string Generate(EntityConfig entity)
    {
        var pkType = entity.PkTypeSimple;

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.{entity.PluralName};
using RhSensoERP.Web.Services.{entity.PluralName};
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de {entity.DisplayName}.
/// Herda toda a funcionalidade CRUD de BaseCrudController.
/// </summary>
[Authorize]
public class {entity.PluralName}Controller 
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

    public {entity.PluralName}Controller(
        I{entity.Name}ApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<{entity.PluralName}Controller> logger)
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

        var viewModel = new {entity.PluralName}ListViewModel
        {{
            UserPermissions = permissions
        }};

        _logger.LogDebug(
            ""Usuário {{User}} acessou {{Funcao}} | Permissões: {{Permissions}}"",
            User.Identity?.Name,
            CdFuncao,
            permissions);

        return View(viewModel);
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
}}
";
    }
}
