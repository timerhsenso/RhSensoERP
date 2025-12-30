// =============================================================================
// GERADOR FULL-STACK v4.1 - JAVASCRIPT TEMPLATE (COM CHECKBOX E TOGGLE ATIVO)
// Baseado em RhSensoERP.CrudTool v2.5
// v4.1 - ✅ NOVO: Checkbox "Selecionar Todos" + Toggle Switch para campo Ativo
// v4.0 - ✅ ADICIONADO: Suporte a ordenação server-side do DataTables
// v3.9 - ✅ CORRIGIDO: Gera código em PascalCase para model binding ASP.NET Core
// v3.8 - ✅ CORRIGIDO: Remove automaticamente campos de auditoria no beforeSubmit
// v3.7 - ✅ CORRIGIDO: Gera TODAS as colunas relevantes automaticamente
// v3.2 - Organiza JavaScript por módulo/entidade
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera JavaScript que estende a classe CrudBase existente.
/// v4.1: Adiciona checkbox "Selecionar Todos" e Toggle Switch para campo Ativo.
/// v4.0: Adiciona ordenação server-side funcional.
/// v3.9: beforeSubmit retorna objeto em PascalCase para compatibilidade com ASP.NET Core.
/// v3.8: Remove automaticamente campos de auditoria e TenantId no beforeSubmit.
/// v3.7: Auto-gera colunas se o usuário não configurou no Wizard.
/// </summary>
public static class JavaScriptTemplate
{
    /// <summary>
    /// Gera arquivo JavaScript que estende CrudBase.
    /// </summary>
    public static GeneratedFile Generate(EntityConfig entity)
    {
        var modulePath = GetModulePath(entity.Module);
        var modulePathLower = modulePath.ToLowerInvariant();
        var columns = GenerateColumns(entity);
        var beforeSubmitLogic = GenerateBeforeSubmitLogic(entity);
        var idField = entity.PrimaryKey?.Name ?? "Id";
        var idFieldLower = char.ToLower(idField[0]) + idField.Substring(1);

        // Verifica se a PK é de texto (não Identity e não Guid)
        var isPkTexto = entity.PrimaryKey != null && !entity.PrimaryKey.IsIdentity && !entity.PrimaryKey.IsGuid;
        var pkFieldId = entity.PrimaryKey?.Name ?? "Id";

        // v4.1: Verifica se tem campo "Ativo"
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase));

        var content = $@"/**
 * ============================================================================
 * {entity.DisplayName.ToUpper()} - JavaScript com Checkbox e Toggle Ativo
 * ============================================================================
 * Arquivo: wwwroot/js/{modulePathLower}/{entity.NameLower}/{entity.NameLower}.js
 * Módulo: {entity.Module}
 * Versão: 4.1 (COM CHECKBOX E TOGGLE ATIVO)
 * Gerado por: GeradorFullStack v4.1
 * Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
 * 
 * Changelog v4.1:
 *   ✅ Checkbox ""Selecionar Todos"" no header da DataTable
 *   ✅ Toggle Switch dinâmico para campo Ativo (rate limit 500ms)
 *   ✅ Exclusão múltipla com contador
 * 
 * Implementação específica do CRUD de {entity.DisplayName}.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class {entity.Name}Crud extends CrudBase {{
    constructor(config) {{
        super(config);
        
        // =====================================================================
        // Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = {(isPkTexto ? $"'{pkFieldId}'" : "null")};
        this.isPkTexto = {(isPkTexto ? "true" : "false")};
        
        // =====================================================================
        // v4.1: Debounce para Toggle Ativo
        // =====================================================================
        this.toggleDebounceTimer = null;
    }}

    /**
     * Habilita/desabilita campos de chave primária.
     * PKs de texto são editáveis apenas na criação.
     */
    enablePrimaryKeyFields(enable) {{
        if (!this.isPkTexto) return;
        
        const $pkField = $('#' + this.pkTextoField);
        if ($pkField.length === 0) return;
        
        if (enable) {{
            // Criação: campo editável
            $pkField.prop('readonly', false)
                    .prop('disabled', false)
                    .removeClass('bg-light');
            console.log('✏️ [{entity.Name}] Campo PK habilitado para edição (criação)');
        }} else {{
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [{entity.Name}] Campo PK desabilitado (edição)');
        }}
    }}

    /**
     * Override: Abre modal para NOVO registro.
     * Habilita PK de texto na criação.
     */
    openCreateModal() {{
        super.openCreateModal();
        
        // Habilita PK de texto para digitação
        if (this.isPkTexto) {{
            this.enablePrimaryKeyFields(true);
        }}
    }}

    /**
     * Override: Abre modal para EDIÇÃO.
     * Desabilita PK de texto na edição.
     */
    async openEditModal(id) {{
        await super.openEditModal(id);
        
        // Desabilita PK de texto (não pode alterar chave)
        if (this.isPkTexto) {{
            this.enablePrimaryKeyFields(false);
        }}
    }}

    /**
     * ⭐ v3.9 CORRIGIDO: Retorna objeto em PascalCase
     * Remove campos de auditoria, converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {{
        console.log('📥 [{entity.Name}] Dados ANTES:', JSON.parse(JSON.stringify(formData)));
{beforeSubmitLogic}
        console.log('📤 [{entity.Name}] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }}

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {{
        console.log('✅ [{entity.Name}] Registro salvo:', data);
        
        // Atualiza a grid automaticamente
        if (this.table) {{
            this.table.ajax.reload(null, false); // Mantém paginação
        }}
    }}

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {{
        const id = row[this.config.idField] || row.id || row.Id || '';
        return typeof id === 'string' ? id.trim() : id;
    }}
}}

// Inicialização quando o documento estiver pronto
$(document).ready(function () {{

    // =========================================================================
    // VERIFICAÇÃO DE PERMISSÕES
    // =========================================================================

    // Verifica se as permissões foram injetadas pela View
    if (typeof window.crudPermissions === 'undefined') {{
        console.error('❌ Permissões não foram carregadas! Usando valores padrão.');
        window.crudPermissions = {{
            canCreate: false,
            canEdit: false,
            canDelete: false,
            canView: true
        }};
    }}

    console.log('🔐 [{entity.Name}] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {{
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {{
            console.warn('⚠️ [{entity.Name}] ID vazio para row:', row);
        }}

        return id;
    }}

    // =========================================================================
    // ✅ v4.1: CONFIGURAÇÃO DAS COLUNAS COM CHECKBOX E TOGGLE ATIVO
    // =========================================================================

    const columns = [
        // =====================================================================
        // v4.1: COLUNA DE SELEÇÃO (CHECKBOX)
        // =====================================================================
        {{
            data: null,
            name: 'Select',
            title: '<input type=""checkbox"" id=""selectAll"" class=""form-check-input"" />',
            orderable: false,
            searchable: false,
            width: '30px',
            className: 'text-center',
            render: function (data, type, row) {{
                const id = getCleanId(row, '{idField}');
                return `<input type=""checkbox"" class=""form-check-input row-select"" value=""${{id}}"" data-id=""${{id}}"" />`;
            }}
        }},
{columns}
    ];

    // =========================================================================
    // INSTANCIA O CRUD
    // =========================================================================

    const crud = new {entity.Name}Crud({{
        entityName: '{entity.Name}',
        idField: '{idField}',
        baseUrl: '/{entity.Name}',
        dataTableColumns: columns,
        exportOptions: {{
            columns: ':not(:first-child):not(:last-child)' // Exclui checkbox e ações
        }}
    }});

    crud.init();

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX ""SELECIONAR TODOS""
    // =========================================================================

    $('#selectAll').on('click', function () {{
        const isChecked = $(this).prop('checked');
        $('.row-select').prop('checked', isChecked);
        updateSelectedCount();
        console.log(`${{isChecked ? '✅' : '❌'}} Selecionou todos os registros`);
    }});

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX INDIVIDUAL
    // =========================================================================

    $(document).on('change', '.row-select', function () {{
        const totalCheckboxes = $('.row-select').length;
        const checkedCheckboxes = $('.row-select:checked').length;
        
        // Atualiza estado do ""Selecionar Todos""
        $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
        
        updateSelectedCount();
    }});

    // =========================================================================
    // v4.1: ATUALIZA CONTADOR DO BOTÃO ""EXCLUIR SELECIONADOS""
    // =========================================================================

    function updateSelectedCount() {{
        const count = $('.row-select:checked').length;
        const $badge = $('#deleteSelectedBtn .badge');
        
        if ($badge.length) {{
            $badge.text(count);
        }}
        
        $('#deleteSelectedBtn').prop('disabled', count === 0);
    }}

    // =========================================================================
    // v4.1: HANDLER - EXCLUIR SELECIONADOS
    // =========================================================================

    $('#deleteSelectedBtn').on('click', function () {{
        const selectedIds = [];
        $('.row-select:checked').each(function () {{
            const id = $(this).data('id');
            if (id) {{
                selectedIds.push(id);
            }}
        }});

        if (selectedIds.length === 0) {{
            toastr.warning('Nenhum registro selecionado.');
            return;
        }}

        if (!confirm(`Deseja realmente excluir ${{selectedIds.length}} registro(s)?`)) {{
            return;
        }}

        // Usa o método deleteMultiple do CrudBase
        crud.deleteMultiple(selectedIds);
    }});

{(hasAtivoField ? GenerateToggleAtivoHandlers(entity, idField) : "")}

    console.log('✅ [{entity.Name}] JavaScript inicializado com sucesso!');
}});
";

        return new GeneratedFile
        {
            FileName = $"{entity.NameLower}.js",
            RelativePath = $"Web/wwwroot/js/{modulePathLower}/{entity.NameLower}/{entity.NameLower}.js",
            Content = content,
            FileType = "JavaScript"
        };
    }

    #region Helper Methods

    /// <summary>
    /// v4.1: Gera handlers para Toggle Switch do campo Ativo.
    /// </summary>
    private static string GenerateToggleAtivoHandlers(EntityConfig entity, string idField)
    {
        return $@"
    // =========================================================================
    // v4.1: HANDLER - TOGGLE SWITCH PARA CAMPO ATIVO (COM RATE LIMIT)
    // =========================================================================

    let toggleDebounceTimer = null;

    $(document).on('change', '.toggle-ativo', function () {{
        const $toggle = $(this);
        const id = $toggle.data('id');
        const currentValue = $toggle.data('current');
        const newValue = $toggle.prop('checked');

        console.log(`🔄 [{entity.Name}] Toggle Ativo - ID: ${{id}}, Novo valor: ${{newValue}}`);

        // Previne múltiplos cliques (Rate Limit - Debounce 500ms)
        clearTimeout(toggleDebounceTimer);

        // Desabilita temporariamente
        $toggle.prop('disabled', true);

        toggleDebounceTimer = setTimeout(function () {{
            $.ajax({{
                url: `/{entity.Name}/ToggleAtivo`,
                type: 'POST',
                headers: {{
                    'RequestVerificationToken': $('input[name=""__RequestVerificationToken""]').val()
                }},
                data: JSON.stringify({{
                    Id: id,
                    Ativo: newValue
                }}),
                contentType: 'application/json',
                success: function (response) {{
                    if (response.success) {{
                        toastr.success(response.message || 'Status atualizado com sucesso!');
                        $toggle.data('current', newValue);
                        console.log(`✅ [{entity.Name}] Toggle Ativo atualizado - ID: ${{id}}`);
                    }} else {{
                        // Reverte toggle em caso de erro
                        $toggle.prop('checked', currentValue);
                        toastr.error(response.message || 'Erro ao atualizar status');
                        console.error(`❌ [{entity.Name}] Erro ao atualizar Toggle Ativo:`, response);
                    }}
                }},
                error: function (xhr) {{
                    // Reverte toggle em caso de erro
                    $toggle.prop('checked', currentValue);
                    toastr.error('Erro ao comunicar com servidor');
                    console.error(`❌ [{entity.Name}] Erro AJAX Toggle Ativo:`, xhr);
                }},
                complete: function () {{
                    // Reabilita toggle
                    $toggle.prop('disabled', false);
                }}
            }});
        }}, 500); // Rate Limit de 500ms
    }});
";
    }

    /// <summary>
    /// ⭐ v4.1 ATUALIZADO: Gera colunas da DataTable incluindo checkbox e toggle ativo.
    /// </summary>
    private static string GenerateColumns(EntityConfig entity)
    {
        var sb = new StringBuilder();

        // Pega colunas configuradas no Grid
        var gridColumns = entity.Properties
            .Where(p => p.List?.Show == true)
            .OrderBy(p => p.List!.Order)
            .ToList();

        // Se não tem colunas configuradas, gera automaticamente
        if (!gridColumns.Any())
        {
            gridColumns = entity.Properties
                .Where(p => !IsAuditField(p))
                .Take(5)
                .ToList();
        }

        foreach (var prop in gridColumns)
        {
            var columnConfig = prop.List!;
            var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);

            sb.AppendLine($@"        // {prop.DisplayName}");
            sb.AppendLine($@"        {{");
            sb.AppendLine($@"            data: '{propNameCamel}',");
            sb.AppendLine($@"            name: '{prop.Name}',");
            sb.AppendLine($@"            title: '{prop.DisplayName}',");
            sb.AppendLine($@"            orderable: {(columnConfig.Sortable ? "true" : "false")},");

            // v4.1: Toggle Switch para campo Ativo
            if (prop.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
                prop.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase))
            {
                var idField = entity.PrimaryKey?.Name ?? "Id";
                var idFieldCamel = char.ToLower(idField[0]) + idField.Substring(1);

                sb.AppendLine($@"            width: '80px',");
                sb.AppendLine($@"            className: 'text-center',");
                sb.AppendLine($@"            render: function (data, type, row) {{");
                sb.AppendLine($@"                if (type === 'display') {{");
                sb.AppendLine($@"                    const checked = data ? 'checked' : '';");
                sb.AppendLine($@"                    const id = row.{idFieldCamel} || row.{idField} || row.id || row.Id;");
                sb.AppendLine($@"                    return `");
                sb.AppendLine($@"                        <div class=""form-check form-switch"">");
                sb.AppendLine($@"                            <input class=""form-check-input toggle-ativo"" ");
                sb.AppendLine($@"                                   type=""checkbox"" ");
                sb.AppendLine($@"                                   ${{checked}}");
                sb.AppendLine($@"                                   data-id=""${{id}}""");
                sb.AppendLine($@"                                   data-current=""${{data}}""");
                sb.AppendLine($@"                                   title=""Clique para ${{data ? 'desativar' : 'ativar'}}"">");
                sb.AppendLine($@"                        </div>`;");
                sb.AppendLine($@"                }}");
                sb.AppendLine($@"                return data;");
                sb.AppendLine($@"            }}");
            }
            else
            {
                // Renderização padrão para outros campos
                sb.AppendLine($@"            render: function (data, type, row) {{");
                sb.AppendLine($@"                return data !== undefined && data !== null ? data : '';");
                sb.AppendLine($@"            }}");
            }

            sb.AppendLine($@"        }},");
        }

        // Coluna de Ações (sempre presente)
        sb.AppendLine($@"        // Ações");
        sb.AppendLine($@"        {{");
        sb.AppendLine($@"            data: null,");
        sb.AppendLine($@"            name: 'Actions',");
        sb.AppendLine($@"            title: 'Ações',");
        sb.AppendLine($@"            orderable: false,");
        sb.AppendLine($@"            searchable: false,");
        sb.AppendLine($@"            width: '100px',");
        sb.AppendLine($@"            className: 'text-center',");
        sb.AppendLine($@"            render: function (data, type, row) {{");
        sb.AppendLine($@"                const id = getCleanId(row, '{entity.PrimaryKey?.Name ?? "Id"}');");
        sb.AppendLine($@"                let actions = '';");
        sb.AppendLine($@"                ");
        sb.AppendLine($@"                if (window.crudPermissions.canEdit) {{");
        sb.AppendLine($@"                    actions += `<button class=""btn btn-sm btn-primary edit-btn"" data-id=""${{id}}"" title=""Editar"">");
        sb.AppendLine($@"                                    <i class=""fas fa-edit""></i>");
        sb.AppendLine($@"                                </button> `;");
        sb.AppendLine($@"                }}");
        sb.AppendLine($@"                ");
        sb.AppendLine($@"                if (window.crudPermissions.canDelete) {{");
        sb.AppendLine($@"                    actions += `<button class=""btn btn-sm btn-danger delete-btn"" data-id=""${{id}}"" title=""Excluir"">");
        sb.AppendLine($@"                                    <i class=""fas fa-trash""></i>");
        sb.AppendLine($@"                                </button>`;");
        sb.AppendLine($@"                }}");
        sb.AppendLine($@"                ");
        sb.AppendLine($@"                return actions || '<span class=""text-muted"">Sem ações</span>';");
        sb.AppendLine($@"            }}");
        sb.AppendLine($@"        }}");

        return sb.ToString();
    }

    /// <summary>
    /// Verifica se o campo é de auditoria.
    /// </summary>
    private static bool IsAuditField(PropertyConfig prop)
    {
        var auditFields = new[]
        {
            "CreatedAt", "CreatedDate", "CreatedAtUtc",
            "CreatedBy", "CreatedByUser", "CreatedByUserId",
            "UpdatedAt", "ModifiedAt", "ModifiedDate", "UpdatedAtUtc",
            "UpdatedBy", "ModifiedBy", "ModifiedByUser", "UpdatedByUserId",
            "TenantId", "IdSaaS", "IdSaas",
            "RowVersion", "Version", "Timestamp",
            "IsDeleted", "DeletedAt", "DeletedBy", "DeletedByUserId",
            "DataCriacao", "DtCriacao", "UsuarioCriacao", "CriadoPor",
            "DataAtualizacao", "DtAtualizacao", "UsuarioAtualizacao", "AtualizadoPor"
        };

        return auditFields.Any(f => prop.Name.Equals(f, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// ⭐ v3.9 REESCRITO: Gera lógica do beforeSubmit retornando objeto em PascalCase.
    /// Compatível com ASP.NET Core model binding (System.Text.Json e Newtonsoft.Json).
    /// </summary>
    private static string GenerateBeforeSubmitLogic(EntityConfig entity)
    {
        var sb = new StringBuilder();

        // =====================================================================
        // STEP 1: Remove campos de auditoria
        // =====================================================================
        sb.AppendLine($@"
        // =====================================================================
        // ⭐ CRÍTICO: Remove campos de auditoria (backend preenche automaticamente)
        // =====================================================================
        delete formData.createdAtUtc;
        delete formData.updatedAtUtc;
        delete formData.createdByUserId;
        delete formData.updatedByUserId;
        delete formData.tenantId;
        delete formData.id;
        delete formData.CreatedAtUtc;
        delete formData.UpdatedAtUtc;
        delete formData.CreatedByUserId;
        delete formData.UpdatedByUserId;
        delete formData.TenantId;
        delete formData.Id;
        delete formData.dataCriacao;
        delete formData.dataAtualizacao;
        delete formData.usuarioCriacao;
        delete formData.usuarioAtualizacao;
        delete formData.createdAt;
        delete formData.updatedAt;
        delete formData.createdBy;
        delete formData.updatedBy;
");

        // =====================================================================
        // STEP 2: Cria objeto limpo em PascalCase
        // =====================================================================
        sb.AppendLine($@"        // =====================================================================
        // ⭐ v3.9: CRIA OBJETO LIMPO EM PASCALCASE (model binding ASP.NET Core)
        // =====================================================================
        const cleanData = {{}};
");

        // =====================================================================
        // STEP 3: Mapeia campos para PascalCase
        // =====================================================================

        // Pega todos os campos que devem estar no formulário (exceto auditoria e PKs auto-geradas)
        var formProps = entity.Properties
            .Where(p => p.Form?.Show == true)
            .Where(p => !IsAuditField(p))
            .Where(p => !p.IsPrimaryKey || (!p.IsIdentity && !p.IsGuid)) // PKs de texto são incluídas
            .OrderBy(p => p.Name)
            .ToList();

        // String fields
        var stringProps = formProps.Where(p => p.IsString).ToList();
        if (stringProps.Any())
        {
            sb.AppendLine($@"        // String fields - PascalCase");
            foreach (var prop in stringProps)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                sb.AppendLine($@"        cleanData.{prop.Name} = formData.{propNameCamel} || formData.{prop.Name} || '';");
            }
            sb.AppendLine();
        }

        // Integer fields (nullable)
        var intProps = formProps.Where(p => (p.IsInt || p.IsLong) && p.IsNullable).ToList();
        if (intProps.Any())
        {
            sb.AppendLine($@"        // Integer nullable fields - PascalCase");
            foreach (var prop in intProps)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                sb.AppendLine($@"        if (formData.{propNameCamel} !== undefined && formData.{propNameCamel} !== null && formData.{propNameCamel} !== '') {{
            const val = parseInt(formData.{propNameCamel}, 10);
            cleanData.{prop.Name} = isNaN(val) ? null : val;
        }} else if (formData.{prop.Name} !== undefined && formData.{prop.Name} !== null && formData.{prop.Name} !== '') {{
            const val = parseInt(formData.{prop.Name}, 10);
            cleanData.{prop.Name} = isNaN(val) ? null : val;
        }} else {{
            cleanData.{prop.Name} = null;
        }}
");
            }
        }

        // Integer fields (required)
        var intPropsRequired = formProps.Where(p => (p.IsInt || p.IsLong) && !p.IsNullable).ToList();
        if (intPropsRequired.Any())
        {
            sb.AppendLine($@"        // Integer required fields - PascalCase");
            foreach (var prop in intPropsRequired)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                sb.AppendLine($@"        cleanData.{prop.Name} = parseInt(formData.{propNameCamel} || formData.{prop.Name} || 0, 10);
");
            }
        }

        // Decimal fields
        var decimalProps = formProps.Where(p => p.IsDecimal).ToList();
        if (decimalProps.Any())
        {
            sb.AppendLine($@"        // Decimal fields - PascalCase");
            foreach (var prop in decimalProps)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                if (prop.IsNullable)
                {
                    sb.AppendLine($@"        if (formData.{propNameCamel} !== undefined && formData.{propNameCamel} !== null && formData.{propNameCamel} !== '') {{
            cleanData.{prop.Name} = parseFloat((formData.{propNameCamel} || '0').toString().replace(',', '.'));
        }} else {{
            cleanData.{prop.Name} = null;
        }}
");
                }
                else
                {
                    sb.AppendLine($@"        cleanData.{prop.Name} = parseFloat((formData.{propNameCamel} || formData.{prop.Name} || '0').toString().replace(',', '.'));
");
                }
            }
        }

        // Boolean fields - PEGA DO DOM (checkbox)
        var boolProps = formProps.Where(p => p.IsBool).ToList();
        if (boolProps.Any())
        {
            sb.AppendLine($@"        // ⭐ Boolean fields - PascalCase - Pega direto do DOM (checkbox)");
            foreach (var prop in boolProps)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                sb.AppendLine($@"        const checkbox{prop.Name} = document.getElementById('{prop.Name}');
        if (checkbox{prop.Name}) {{
            cleanData.{prop.Name} = checkbox{prop.Name}.checked;
        }} else {{
            cleanData.{prop.Name} = formData.{propNameCamel} === true || 
                                    formData.{prop.Name} === true || 
                                    formData.{propNameCamel} === 'true' || 
                                    formData.{propNameCamel} === 1;
        }}
");
            }
        }

        // DateTime fields
        var dateProps = formProps.Where(p => p.IsDateTime).ToList();
        if (dateProps.Any())
        {
            sb.AppendLine($@"        // DateTime fields - PascalCase");
            foreach (var prop in dateProps)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                if (prop.IsNullable)
                {
                    sb.AppendLine($@"        cleanData.{prop.Name} = (formData.{propNameCamel} || formData.{prop.Name}) || null;
");
                }
                else
                {
                    sb.AppendLine($@"        cleanData.{prop.Name} = formData.{propNameCamel} || formData.{prop.Name} || new Date().toISOString();
");
                }
            }
        }

        // Guid fields (nullable, não PK)
        var guidProps = formProps.Where(p => p.IsGuid && p.IsNullable).ToList();
        if (guidProps.Any())
        {
            sb.AppendLine($@"        // Guid nullable fields - PascalCase");
            foreach (var prop in guidProps)
            {
                var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                sb.AppendLine($@"        cleanData.{prop.Name} = (formData.{propNameCamel} || formData.{prop.Name}) || null;
");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converte nome do módulo para path de pasta.
    /// </summary>
    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "common";

        return moduleName;
    }

    #endregion
}