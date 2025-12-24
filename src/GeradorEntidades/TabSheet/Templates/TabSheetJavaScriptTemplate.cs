// =============================================================================
// TABSHEET GENERATOR - TEMPLATE DE JAVASCRIPT
// Versão: 1.0.0
// Autor: RhSensoERP Team
// Data: 2024
// 
// Gera JavaScript para interação com TabSheets (DataTables, AJAX, Modais).
// =============================================================================

using System.Text;
using GeradorEntidades.Models;
using GeradorEntidades.TabSheet.Models;

namespace GeradorEntidades.TabSheet.Templates;

/// <summary>
/// Template para geração de JavaScript dos TabSheets.
/// </summary>
public static class TabSheetJavaScriptTemplate
{
    /// <summary>
    /// Gera o arquivo JavaScript principal do TabSheet.
    /// </summary>
    public static GeneratedTabSheetFile Generate(
        TabSheetConfiguration config,
        Dictionary<string, TabelaInfo> detailTables)
    {
        var sb = new StringBuilder();
        var entityName = config.MasterTable.EntityName;
        var entityNameLower = config.MasterTable.EntityNameCamel;
        var options = config.GenerationOptions;

        // Cabeçalho
        sb.AppendLine("// =============================================================================");
        sb.AppendLine($"// TABSHEET: {config.Title}");
        sb.AppendLine($"// Entidade: {entityName}");
        sb.AppendLine("// Gerado por: TabSheet Generator");
        sb.AppendLine($"// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();

        // Módulo principal
        sb.AppendLine($"const TabSheet{entityName} = (function() {{");
        sb.AppendLine("    'use strict';");
        sb.AppendLine();

        // Variáveis privadas
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // VARIÁVEIS PRIVADAS");
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine();
        sb.AppendLine("    let _config = {");
        sb.AppendLine("        masterId: null,");
        sb.AppendLine("        isEdit: false,");
        sb.AppendLine("        apiBaseUrl: ''");
        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("    const _tables = {};");
        sb.AppendLine("    let _tabsLoaded = {};");
        sb.AppendLine();

        // Inicialização
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // INICIALIZAÇÃO");
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine();
        sb.AppendLine("    function init(config) {");
        sb.AppendLine("        _config = { ..._config, ...config };");
        sb.AppendLine("        console.log('TabSheet inicializado:', _config);");
        sb.AppendLine();
        sb.AppendLine("        // Bind eventos do formulário mestre");
        sb.AppendLine("        bindMasterFormEvents();");
        sb.AppendLine();
        sb.AppendLine("        // Se estiver em modo edição, inicializar tabs");
        sb.AppendLine("        if (_config.isEdit && _config.masterId) {");
        sb.AppendLine("            initializeTabs();");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Eventos do formulário mestre
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // FORMULÁRIO MESTRE");
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine();
        sb.AppendLine("    function bindMasterFormEvents() {");
        sb.AppendLine($"        const form = document.getElementById('form{entityName}');");
        sb.AppendLine("        if (form) {");
        sb.AppendLine("            form.addEventListener('submit', function(e) {");
        sb.AppendLine("                e.preventDefault();");
        sb.AppendLine("                saveMaster();");
        sb.AppendLine("            });");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    async function saveMaster() {");
        sb.AppendLine($"        const form = document.getElementById('form{entityName}');");
        sb.AppendLine("        if (!form.checkValidity()) {");
        sb.AppendLine("            form.classList.add('was-validated');");
        sb.AppendLine("            return;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        const formData = new FormData(form);");
        sb.AppendLine("        const data = Object.fromEntries(formData.entries());");
        sb.AppendLine();
        sb.AppendLine("        try {");
        sb.AppendLine("            showLoading();");
        sb.AppendLine("            const url = _config.isEdit");
        sb.AppendLine("                ? `${_config.apiBaseUrl}/${_config.masterId}`");
        sb.AppendLine("                : _config.apiBaseUrl;");
        sb.AppendLine("            const method = _config.isEdit ? 'PUT' : 'POST';");
        sb.AppendLine();
        sb.AppendLine("            const response = await fetch(url, {");
        sb.AppendLine("                method: method,");
        sb.AppendLine("                headers: { 'Content-Type': 'application/json' },");
        sb.AppendLine("                body: JSON.stringify(data)");
        sb.AppendLine("            });");
        sb.AppendLine();
        sb.AppendLine("            if (response.ok) {");
        sb.AppendLine("                const result = await response.json();");
        sb.AppendLine("                showSuccess('Registro salvo com sucesso!');");
        sb.AppendLine();
        sb.AppendLine("                // Se era novo, redirecionar para edição");
        sb.AppendLine("                if (!_config.isEdit && result.id) {");
        sb.AppendLine("                    window.location.href = `${_config.apiBaseUrl.replace('/api/', '/')}/edit/${result.id}`;");
        sb.AppendLine("                }");
        sb.AppendLine("            } else {");
        sb.AppendLine("                const error = await response.text();");
        sb.AppendLine("                showError('Erro ao salvar: ' + error);");
        sb.AppendLine("            }");
        sb.AppendLine("        } catch (err) {");
        sb.AppendLine("            showError('Erro de conexão: ' + err.message);");
        sb.AppendLine("        } finally {");
        sb.AppendLine("            hideLoading();");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Inicialização das Tabs
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // TABS");
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine();
        sb.AppendLine("    function initializeTabs() {");
        sb.AppendLine("        // Carregar primeira tab automaticamente");

        if (config.Tabs.Count > 0)
        {
            var firstTab = config.Tabs.First();
            sb.AppendLine($"        loadTab('{firstTab.TabId}');");
        }

        sb.AppendLine();
        sb.AppendLine("        // Bind evento de troca de tab");
        sb.AppendLine("        $('a[data-toggle=\"pill\"]').on('shown.bs.tab', function(e) {");
        sb.AppendLine("            const tabId = $(e.target).attr('href').replace('#', '');");
        sb.AppendLine("            loadTab(tabId);");
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    function loadTab(tabId) {");
        sb.AppendLine("        if (_tabsLoaded[tabId]) {");
        sb.AppendLine("            console.log('Tab já carregada:', tabId);");
        sb.AppendLine("            return;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        console.log('Carregando tab:', tabId);");
        sb.AppendLine();
        sb.AppendLine("        switch(tabId) {");

        // Switch para cada tab
        foreach (var tab in config.Tabs)
        {
            sb.AppendLine($"            case '{tab.TabId}':");
            sb.AppendLine($"                load{tab.EntityName}Tab();");
            sb.AppendLine("                break;");
        }

        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        _tabsLoaded[tabId] = true;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Gerar código para cada tab
        foreach (var tab in config.Tabs)
        {
            if (!detailTables.TryGetValue(tab.TableName, out var detailTable))
                continue;

            GenerateTabCode(sb, config, tab, detailTable);
        }

        // Funções utilitárias
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // UTILITÁRIOS");
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine();
        sb.AppendLine("    function showLoading() {");
        sb.AppendLine("        // Implementar overlay de loading");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    function hideLoading() {");
        sb.AppendLine("        // Remover overlay de loading");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    function showSuccess(message) {");
        sb.AppendLine("        toastr.success(message);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    function showError(message) {");
        sb.AppendLine("        toastr.error(message);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    function showConfirm(message) {");
        sb.AppendLine("        return Swal.fire({");
        sb.AppendLine("            title: 'Confirmação',");
        sb.AppendLine("            text: message,");
        sb.AppendLine("            icon: 'warning',");
        sb.AppendLine("            showCancelButton: true,");
        sb.AppendLine("            confirmButtonColor: '#3085d6',");
        sb.AppendLine("            cancelButtonColor: '#d33',");
        sb.AppendLine("            confirmButtonText: 'Sim',");
        sb.AppendLine("            cancelButtonText: 'Não'");
        sb.AppendLine("        }).then((result) => result.isConfirmed);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // API pública
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // API PÚBLICA");
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine();
        sb.AppendLine("    return {");
        sb.AppendLine("        init: init,");
        sb.AppendLine("        saveMaster: saveMaster,");
        sb.AppendLine("        loadTab: loadTab,");

        // Expor funções das tabs
        foreach (var tab in config.Tabs)
        {
            sb.AppendLine($"        reload{tab.EntityName}: function() {{ if (_tables['{tab.EntityName}']) _tables['{tab.EntityName}'].ajax.reload(); }},");
        }

        sb.AppendLine("        getTables: function() { return _tables; }");
        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("})();");

        return new GeneratedTabSheetFile
        {
            FileName = $"{entityNameLower}-tabsheet.js",
            RelativePath = $"Web/wwwroot/{options.ModuleRoute}/js/",
            Content = sb.ToString(),
            FileType = TabSheetFileType.JavaScript,
            EntityName = entityName,
            IsMasterFile = true
        };
    }

    /// <summary>
    /// Gera código JavaScript para uma tab específica.
    /// </summary>
    private static void GenerateTabCode(
        StringBuilder sb,
        TabSheetConfiguration config,
        TabDefinition tab,
        TabelaInfo tabela)
    {
        var entityName = tab.EntityName;
        var entityNameLower = entityName.ToLower();
        var masterPk = config.MasterTable.PrimaryKey;

        sb.AppendLine($"    // -------------------------------------------------------------------------");
        sb.AppendLine($"    // TAB: {tab.Title} ({entityName})");
        sb.AppendLine($"    // -------------------------------------------------------------------------");
        sb.AppendLine();

        // Função de carregamento
        sb.AppendLine($"    function load{entityName}Tab() {{");
        sb.AppendLine($"        const container = document.getElementById('container-{tab.TabId}');");
        sb.AppendLine("        if (!container) return;");
        sb.AppendLine();
        sb.AppendLine("        // Carregar partial via AJAX");
        sb.AppendLine($"        fetch(`${{_config.apiBaseUrl.replace('/api/', '/')}}/partial/{entityNameLower}`)");
        sb.AppendLine("            .then(response => response.text())");
        sb.AppendLine("            .then(html => {");
        sb.AppendLine("                container.innerHTML = html;");
        sb.AppendLine($"                init{entityName}DataTable();");
        sb.AppendLine($"                bind{entityName}Events();");
        sb.AppendLine($"                loadBadge{entityName}();");
        sb.AppendLine("            })");
        sb.AppendLine("            .catch(err => {");
        sb.AppendLine("                container.innerHTML = `<div class=\"alert alert-danger\">Erro ao carregar: ${err.message}</div>`;");
        sb.AppendLine("            });");
        sb.AppendLine("    }");
        sb.AppendLine();

        // DataTable
        sb.AppendLine($"    function init{entityName}DataTable() {{");
        sb.AppendLine($"        if (_tables['{entityName}']) {{");
        sb.AppendLine($"            _tables['{entityName}'].ajax.reload();");
        sb.AppendLine("            return;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine($"        _tables['{entityName}'] = $('#table{entityName}').DataTable({{");
        sb.AppendLine("            processing: true,");
        sb.AppendLine("            serverSide: false,");
        sb.AppendLine("            ajax: {");
        sb.AppendLine($"                url: `${{_config.apiBaseUrl}}/${{_config.masterId}}/{entityNameLower}`,");
        sb.AppendLine("                dataSrc: ''");
        sb.AppendLine("            },");
        sb.AppendLine("            columns: [");

        // Colunas do DataTable
        var columns = tabela.Colunas
            .Where(c => !c.IsPrimaryKey &&
                       !c.Nome.Equals(tab.ForeignKey, StringComparison.OrdinalIgnoreCase) &&
                       !c.IsBinary)
            .Take(6)
            .ToList();

        foreach (var col in columns)
        {
            var render = GetColumnRender(col);
            sb.AppendLine($"                {{ data: '{col.NomePascalCase.ToCamelCase()}'{render} }},");
        }

        // Coluna de ações
        sb.AppendLine("                {");
        sb.AppendLine("                    data: null,");
        sb.AppendLine("                    orderable: false,");
        sb.AppendLine("                    render: function(data, type, row) {");
        sb.AppendLine("                        let html = '<div class=\"btn-group btn-group-sm\">';");

        if (tab.AllowEdit)
        {
            sb.AppendLine($"                        html += `<button class=\"btn btn-info btn-edit-{entityNameLower}\" data-id=\"${{row.{(tab.PrimaryKey ?? "id").ToCamelCase()}}}\" title=\"Editar\"><i class=\"fas fa-edit\"></i></button>`;");
        }

        if (tab.AllowDelete)
        {
            sb.AppendLine($"                        html += `<button class=\"btn btn-danger btn-delete-{entityNameLower}\" data-id=\"${{row.{(tab.PrimaryKey ?? "id").ToCamelCase()}}}\" title=\"Excluir\"><i class=\"fas fa-trash\"></i></button>`;");
        }

        sb.AppendLine("                        html += '</div>';");
        sb.AppendLine("                        return html;");
        sb.AppendLine("                    }");
        sb.AppendLine("                }");
        sb.AppendLine("            ],");
        sb.AppendLine("            language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/pt-BR.json' },");
        sb.AppendLine("            responsive: true,");
        sb.AppendLine("            pageLength: 10");
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Eventos
        sb.AppendLine($"    function bind{entityName}Events() {{");

        // Botão Novo
        if (tab.AllowCreate)
        {
            sb.AppendLine($"        $(document).on('click', '#btnNovo{entityName}', function() {{");
            sb.AppendLine($"            $('#form{entityName}')[0].reset();");
            sb.AppendLine($"            $('#{entityNameLower}_{tab.PrimaryKey ?? "id"}').val('');");
            sb.AppendLine($"            $('#{entityNameLower}_{tab.ForeignKey}').val(_config.masterId);");
            sb.AppendLine($"            $('#modalTitle{entityName}').text('Novo {tab.Title}');");
            sb.AppendLine($"            $('#modal{entityName}').modal('show');");
            sb.AppendLine("        });");
            sb.AppendLine();
        }

        // Botão Editar
        if (tab.AllowEdit)
        {
            sb.AppendLine($"        $(document).on('click', '.btn-edit-{entityNameLower}', async function() {{");
            sb.AppendLine("            const id = $(this).data('id');");
            sb.AppendLine($"            const response = await fetch(`${{_config.apiBaseUrl}}/${{_config.masterId}}/{entityNameLower}/${{id}}`);");
            sb.AppendLine("            if (response.ok) {");
            sb.AppendLine("                const data = await response.json();");
            sb.AppendLine($"                populateForm('form{entityName}', data);");
            sb.AppendLine($"                $('#modalTitle{entityName}').text('Editar {tab.Title}');");
            sb.AppendLine($"                $('#modal{entityName}').modal('show');");
            sb.AppendLine("            }");
            sb.AppendLine("        });");
            sb.AppendLine();
        }

        // Botão Excluir
        if (tab.AllowDelete)
        {
            sb.AppendLine($"        $(document).on('click', '.btn-delete-{entityNameLower}', async function() {{");
            sb.AppendLine("            const id = $(this).data('id');");
            sb.AppendLine("            const confirmed = await showConfirm('Deseja realmente excluir este registro?');");
            sb.AppendLine("            if (confirmed) {");
            sb.AppendLine($"                const response = await fetch(`${{_config.apiBaseUrl}}/${{_config.masterId}}/{entityNameLower}/${{id}}`, {{ method: 'DELETE' }});");
            sb.AppendLine("                if (response.ok) {");
            sb.AppendLine("                    showSuccess('Registro excluído!');");
            sb.AppendLine($"                    _tables['{entityName}'].ajax.reload();");
            sb.AppendLine($"                    loadBadge{entityName}();");
            sb.AppendLine("                } else {");
            sb.AppendLine("                    showError('Erro ao excluir.');");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        });");
            sb.AppendLine();
        }

        // Botão Salvar
        sb.AppendLine($"        $(document).on('click', '#btnSalvar{entityName}', async function() {{");
        sb.AppendLine($"            const form = document.getElementById('form{entityName}');");
        sb.AppendLine("            if (!form.checkValidity()) {");
        sb.AppendLine("                form.classList.add('was-validated');");
        sb.AppendLine("                return;");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            const formData = new FormData(form);");
        sb.AppendLine("            const data = Object.fromEntries(formData.entries());");
        sb.AppendLine($"            const id = data['{tab.PrimaryKey ?? "Id"}'];");
        sb.AppendLine("            const isEdit = id && id !== '';");
        sb.AppendLine();
        sb.AppendLine($"            const url = isEdit");
        sb.AppendLine($"                ? `${{_config.apiBaseUrl}}/${{_config.masterId}}/{entityNameLower}/${{id}}`");
        sb.AppendLine($"                : `${{_config.apiBaseUrl}}/${{_config.masterId}}/{entityNameLower}`;");
        sb.AppendLine("            const method = isEdit ? 'PUT' : 'POST';");
        sb.AppendLine();
        sb.AppendLine("            const response = await fetch(url, {");
        sb.AppendLine("                method: method,");
        sb.AppendLine("                headers: { 'Content-Type': 'application/json' },");
        sb.AppendLine("                body: JSON.stringify(data)");
        sb.AppendLine("            });");
        sb.AppendLine();
        sb.AppendLine("            if (response.ok) {");
        sb.AppendLine("                showSuccess('Registro salvo!');");
        sb.AppendLine($"                $('#modal{entityName}').modal('hide');");
        sb.AppendLine($"                _tables['{entityName}'].ajax.reload();");
        sb.AppendLine($"                loadBadge{entityName}();");
        sb.AppendLine("            } else {");
        sb.AppendLine("                showError('Erro ao salvar.');");
        sb.AppendLine("            }");
        sb.AppendLine("        });");
        sb.AppendLine();

        // Botão Recarregar
        sb.AppendLine($"        $(document).on('click', '#btnRecarregar{entityName}', function() {{");
        sb.AppendLine($"            _tables['{entityName}'].ajax.reload();");
        sb.AppendLine($"            loadBadge{entityName}();");
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Função para carregar badge
        if (tab.ShowBadge)
        {
            sb.AppendLine($"    async function loadBadge{entityName}() {{");
            sb.AppendLine($"        const response = await fetch(`${{_config.apiBaseUrl}}/${{_config.masterId}}/{entityNameLower}/count`);");
            sb.AppendLine("        if (response.ok) {");
            sb.AppendLine("            const count = await response.json();");
            sb.AppendLine($"            $('#badge-{tab.TabId}').text(count);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        // Função auxiliar para popular form
        sb.AppendLine("    function populateForm(formId, data) {");
        sb.AppendLine("        const form = document.getElementById(formId);");
        sb.AppendLine("        if (!form) return;");
        sb.AppendLine("        Object.keys(data).forEach(key => {");
        sb.AppendLine("            const field = form.querySelector(`[name=\"${key}\"]`) || form.querySelector(`#${key.toLowerCase()}`);");
        sb.AppendLine("            if (field) {");
        sb.AppendLine("                if (field.type === 'checkbox') {");
        sb.AppendLine("                    field.checked = data[key];");
        sb.AppendLine("                } else {");
        sb.AppendLine("                    field.value = data[key] ?? '';");
        sb.AppendLine("                }");
        sb.AppendLine("            }");
        sb.AppendLine("        });");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    /// <summary>
    /// Obtém o render customizado para a coluna no DataTable.
    /// </summary>
    private static string GetColumnRender(ColunaInfo coluna)
    {
        if (coluna.IsData)
        {
            return ", render: function(data) { return data ? new Date(data).toLocaleDateString('pt-BR') : ''; }";
        }

        if (coluna.IsBool)
        {
            return ", render: function(data) { return data ? '<span class=\"badge badge-success\">Sim</span>' : '<span class=\"badge badge-secondary\">Não</span>'; }";
        }

        if (coluna.TipoCSharp == "decimal" || coluna.TipoCSharp == "double" || coluna.TipoCSharp == "float")
        {
            return ", render: function(data) { return data != null ? parseFloat(data).toLocaleString('pt-BR', { minimumFractionDigits: 2 }) : ''; }";
        }

        return "";
    }
}

/// <summary>
/// Extensão para converter para camelCase.
/// </summary>
internal static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return char.ToLower(str[0]) + str[1..];
    }
}
