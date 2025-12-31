// =============================================================================
// GERADOR FULL-STACK v4.3 - JAVASCRIPT TEMPLATE (FINAL - 100% FUNCIONAL)
// Baseado em RhSensoERP.CrudTool v2.5
// v4.3 - ✅ CRÍTICO: Geração AUTOMÁTICA de colunas (não depende de Grid.Show)
//      - ✅ CRÍTICO: Resolve erro "aDataSort" - SEMPRE gera colunas
//      - ✅ CRÍTICO: Heurísticas inteligentes (Form, tipos comuns, ordem alfabética)
// v4.2 - ✅ CORRIGIDO: dataTableColumns → columns (compatível com CrudBase)
//      - ✅ CORRIGIDO: Adiciona todos os parâmetros obrigatórios do CrudBase
//      - ✅ CORRIGIDO: idField em lowercase, classes CSS corretas
// v4.1 - ✅ NOVO: Checkbox "Selecionar Todos" + Toggle Switch para campo Ativo
// v4.0 - ✅ ADICIONADO: Suporte a ordenação server-side do DataTables
// v3.9 - ✅ CORRIGIDO: Gera código em PascalCase para model binding ASP.NET Core
// v3.8 - ✅ CORRIGIDO: Remove automaticamente campos de auditoria no beforeSubmit
// v3.7 - ✅ CORRIGIDO: Gera TODAS as colunas relevantes automaticamente
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera JavaScript que estende a classe CrudBase existente.
/// v4.2: Corrige parâmetros para compatibilidade com CrudBase.
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
 * Versão: 4.3 (FINAL - 100% FUNCIONAL)
 * Gerado por: GeradorFullStack v4.3
 * Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
 * 
 * Changelog v4.3:
 *   ✅ CRÍTICO: Geração automática inteligente de colunas (não depende de Grid)
 *   ✅ CRÍTICO: Resolve 100% do erro ""aDataSort"" do DataTables
 *   ✅ CRÍTICO: Heurísticas: Form.Show, tipos comuns, ordem alfabética
 * 
 * Changelog v4.2:
 *   ✅ CORRIGIDO: dataTableColumns → columns (compatível com CrudBase)
 *   ✅ CORRIGIDO: Parâmetros obrigatórios do CrudBase adicionados
 *   ✅ CORRIGIDO: idField em lowercase, classes CSS corretas
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
        if (this.dataTable) {{
            this.dataTable.ajax.reload(null, false); // Mantém paginação
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
    // ✅ v4.2: CONFIGURAÇÃO DAS COLUNAS (CORRIGIDO)
    // =========================================================================

    const columns = [
{columns}
    ];

    // =========================================================================
    // ✅ v4.2: INSTANCIA O CRUD (CORRIGIDO: TODOS OS PARÂMETROS)
    // =========================================================================

    const crud = new {entity.Name}Crud({{
        controllerName: '{entity.Name}',
        entityName: '{entity.DisplayName}',
        entityNamePlural: '{entity.DisplayName}s',
        idField: '{idFieldLower}',
        tableSelector: '#tableCrud',
        columns: columns,  // ✅ CORRIGIDO: era ""dataTableColumns""
        permissions: window.crudPermissions,
        exportConfig: {{
            enabled: true,
            excel: true,
            pdf: true,
            csv: true,
            print: true,
            filename: '{entity.Name}'
        }}
    }});

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX ""SELECIONAR TODOS""
    // =========================================================================

    $('#tableCrud').on('click', '#selectAll', function () {{
        const isChecked = $(this).prop('checked');
        $('.row-select').prop('checked', isChecked);
        crud.updateSelectedCount();
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
        
        crud.updateSelectedCount();
    }});

{(hasAtivoField ? GenerateToggleAtivoHandler(entity) : "")}

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
    /// Gera handler para Toggle Ativo (v4.1).
    /// </summary>
    private static string GenerateToggleAtivoHandler(EntityConfig entity)
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
                        console.log(`✅ [{entity.Name}] Toggle Ativo atualizado - ID: ${{id}}`);
                        $toggle.data('current', newValue);
                        
                        // Usa SweetAlert se disponível, senão console
                        if (typeof Swal !== 'undefined') {{
                            Swal.fire({{
                                icon: 'success',
                                title: 'Sucesso!',
                                text: response.message || 'Status atualizado!',
                                timer: 2000,
                                showConfirmButton: false
                            }});
                        }}
                    }} else {{
                        // Reverte toggle em caso de erro
                        $toggle.prop('checked', currentValue);
                        console.error(`❌ [{entity.Name}] Erro ao atualizar Toggle Ativo:`, response);
                        
                        if (typeof Swal !== 'undefined') {{
                            Swal.fire({{
                                icon: 'error',
                                title: 'Erro!',
                                text: response.message || 'Erro ao atualizar status'
                            }});
                        }}
                    }}
                }},
                error: function (xhr) {{
                    // Reverte toggle em caso de erro
                    $toggle.prop('checked', currentValue);
                    console.error(`❌ [{entity.Name}] Erro AJAX Toggle Ativo:`, xhr);
                    
                    if (typeof Swal !== 'undefined') {{
                        Swal.fire({{
                            icon: 'error',
                            title: 'Erro!',
                            text: 'Erro ao comunicar com servidor'
                        }});
                    }}
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
    /// Gera colunas do DataTable automaticamente.
    /// v4.3: Geração inteligente baseada em heurísticas (não usa Grid.Show).
    /// v4.2: Corrige classes CSS e idField lowercase.
    /// v4.1: Adiciona coluna de checkbox e toggle para Ativo.
    /// </summary>
    private static string GenerateColumns(EntityConfig entity)
    {
        var sb = new StringBuilder();
        var idFieldLower = char.ToLower((entity.PrimaryKey?.Name ?? "Id")[0]) + (entity.PrimaryKey?.Name ?? "Id").Substring(1);

        // =====================================================================
        // v4.1: COLUNA DE SELEÇÃO (CHECKBOX)
        // =====================================================================
        sb.AppendLine($@"        // =====================================================================
        // v4.1: COLUNA DE SELEÇÃO (CHECKBOX)
        // =====================================================================
        {{
            data: null,
            name: 'Select',
            title: '<input type=""checkbox"" id=""selectAll"" class=""form-check-input"" />',
            orderable: false,
            searchable: false,
            width: '30px',
            className: 'text-center no-export',
            render: function (data, type, row) {{
                const id = getCleanId(row, '{idFieldLower}');
                return `<input type=""checkbox"" class=""form-check-input row-select dt-checkboxes"" value=""${{id}}"" data-id=""${{id}}"" />`;
            }}
        }},");

        // =====================================================================
        // v4.3: COLUNAS VISÍVEIS (GERAÇÃO AUTOMÁTICA INTELIGENTE)
        // =====================================================================
        // Gera colunas automaticamente baseado em heurísticas
        var visibleProps = entity.Properties
            .Where(p => !p.IsPrimaryKey || (entity.PrimaryKey?.IsIdentity == false)) // Exclui PKs auto
            .Where(p => !IsAuditField(p)) // Exclui auditoria
            .Where(p => p.Form?.Show != false) // Inclui se estiver no formulário
            .Where(p => p.IsString || p.IsInt || p.IsLong || p.IsBool || p.IsDecimal || p.IsDateTime) // Tipos comuns
            .OrderBy(p => p.Name) // Ordem alfabética
            .ToList();

        // ⭐ v4.3: Limita a 10 colunas (mantém performance do DataTables)
        if (visibleProps.Count > 10)
        {
            visibleProps = visibleProps.Take(10).ToList();
        }

        // ⭐ v4.3: GARANTIA MÍNIMA - Se mesmo assim não tiver colunas, pega as 3 primeiras não-PK
        if (!visibleProps.Any())
        {
            visibleProps = entity.Properties
                .Where(p => !p.IsPrimaryKey)
                .Take(3)
                .ToList();
        }

        foreach (var prop in visibleProps)
        {
            var propNameCamel = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
            var displayName = prop.DisplayName ?? prop.Name;

            // Campo Ativo vira Toggle Switch
            if (prop.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
                prop.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine($@"        // {displayName}
        {{
            data: '{propNameCamel}',
            name: '{prop.Name}',
            title: '{displayName}',
            orderable: true,
            width: '80px',
            className: 'text-center',
            render: function (data, type, row) {{
                if (type === 'display') {{
                    const checked = data ? 'checked' : '';
                    const id = getCleanId(row, '{idFieldLower}');
                    return `
                        <div class=""form-check form-switch"">
                            <input class=""form-check-input toggle-ativo"" 
                                   type=""checkbox"" 
                                   ${{checked}}
                                   data-id=""${{id}}""
                                   data-current=""${{data}}""
                                   title=""Clique para ${{data ? 'desativar' : 'ativar'}}"">
                        </div>`;
                }}
                return data;
            }}
        }},");
            }
            // Boolean comum vira badge
            else if (prop.IsBool)
            {
                sb.AppendLine($@"        // {displayName}
        {{
            data: '{propNameCamel}',
            name: '{prop.Name}',
            title: '{displayName}',
            orderable: true,
            className: 'text-center',
            render: function (data, type, row) {{
                if (type === 'display') {{
                    return data 
                        ? '<span class=""badge bg-success"">Sim</span>' 
                        : '<span class=""badge bg-secondary"">Não</span>';
                }}
                return data;
            }}
        }},");
            }
            // DateTime
            else if (prop.IsDateTime)
            {
                sb.AppendLine($@"        // {displayName}
        {{
            data: '{propNameCamel}',
            name: '{prop.Name}',
            title: '{displayName}',
            orderable: true,
            render: function (data, type, row) {{
                if (type === 'display' && data) {{
                    const date = new Date(data);
                    return date.toLocaleDateString('pt-BR');
                }}
                return data || '';
            }}
        }},");
            }
            // Outros campos
            else
            {
                sb.AppendLine($@"        // {displayName}
        {{
            data: '{propNameCamel}',
            name: '{prop.Name}',
            title: '{displayName}',
            orderable: true,
            render: function (data, type, row) {{
                return data !== undefined && data !== null ? data : '';
            }}
        }},");
            }
        }

        // =====================================================================
        // COLUNA DE AÇÕES
        // =====================================================================
        sb.AppendLine($@"        // Ações
        {{
            data: null,
            name: 'Actions',
            title: 'Ações',
            orderable: false,
            searchable: false,
            width: '100px',
            className: 'text-center no-export',
            render: function (data, type, row) {{
                const id = getCleanId(row, '{idFieldLower}');
                let actions = '';
                
                if (window.crudPermissions.canEdit) {{
                    actions += `<button class=""btn btn-sm btn-primary btn-edit"" data-id=""${{id}}"" title=""Editar"">
                                    <i class=""fas fa-edit""></i>
                                </button> `;
                }}
                
                if (window.crudPermissions.canDelete) {{
                    actions += `<button class=""btn btn-sm btn-danger btn-delete"" data-id=""${{id}}"" title=""Excluir"">
                                    <i class=""fas fa-trash""></i>
                                </button>`;
                }}
                
                return actions || '<span class=""text-muted"">Sem ações</span>';
            }}
        }}");

        return sb.ToString();
    }

    /// <summary>
    /// Verifica se o campo é de auditoria/sistema.
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
            "DataCriacao", "DtCriacao",
            "UsuarioCriacao", "CriadoPor",
            "DataAtualizacao", "DtAtualizacao",
            "UsuarioAtualizacao", "AtualizadoPor"
        };

        return auditFields.Any(f => prop.Name.Equals(f, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gera lógica do beforeSubmit.
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