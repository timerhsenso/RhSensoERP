// =============================================================================
// GERADOR FULL-STACK v3.9 - JAVASCRIPT TEMPLATE (CORRIGIDO - PASCALCASE)
// Baseado em RhSensoERP.CrudTool v2.5
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

        var content = $@"/**
 * ============================================================================
 * {entity.DisplayName.ToUpper()} - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/{modulePathLower}/{entity.NameLower}/{entity.NameLower}.js
 * Módulo: {entity.Module}
 * Versão: 3.9 (PascalCase para model binding)
 * Gerado por: GeradorFullStack v3.9
 * Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
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
    // CONFIGURAÇÃO DAS COLUNAS DO DATATABLES
    // =========================================================================

    const columns = [
        // Coluna de seleção (checkbox)
        {{
            data: null,
            orderable: false,
            searchable: false,
            className: 'dt-checkboxes-cell',
            width: '40px',
            render: function (data, type, row) {{
                // Só mostra checkbox se pode excluir
                if (window.crudPermissions.canDelete) {{
                    const id = getCleanId(row, '{idFieldLower}');
                    return `<input type=""checkbox"" class=""dt-checkboxes form-check-input"" data-id=""${{id}}"">`;
                }}
                return '';
            }}
        }},
{columns}
        // Coluna de ações
        {{
            data: null,
            orderable: false,
            searchable: false,
            className: 'text-end no-export',
            title: 'Ações',
            width: '130px',
            render: function (data, type, row) {{
                const id = getCleanId(row, '{idFieldLower}');

                let actions = '<div class=""btn-group btn-group-sm"" role=""group"">';

                // Botão Editar (somente se tiver permissão)
                if (window.crudPermissions.canEdit) {{
                    actions += `<button type=""button"" class=""btn btn-outline-primary btn-edit"" 
                                data-id=""${{id}}"" title=""Editar"">
                                <i class=""fas fa-edit""></i>
                            </button>`;
                }}

                // Botão Excluir (somente se tiver permissão)
                if (window.crudPermissions.canDelete) {{
                    actions += `<button type=""button"" class=""btn btn-outline-danger btn-delete"" 
                                data-id=""${{id}}"" title=""Excluir"">
                                <i class=""fas fa-trash""></i>
                            </button>`;
                }}

                actions += '</div>';
                return actions;
            }}
        }}
    ];

    // =========================================================================
    // INICIALIZAÇÃO DO CRUD
    // =========================================================================

    window.{entity.NameLower}Crud = new {entity.Name}Crud({{
        controllerName: '{entity.Name}',
        entityName: '{entity.Name}',
        idField: '{idFieldLower}',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {{
            order: [[1, 'asc']],
            pageLength: 25
        }}
    }});

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================

    // CrudBase inicializa automaticamente no construtor
    console.log('✅ [{entity.Name}] CRUD inicializado com sucesso (v3.9 - PascalCase)');
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
    /// ✅ v3.7: Gera as colunas do DataTables.
    /// Auto-gera se não configuradas pelo usuário.
    /// </summary>
    private static string GenerateColumns(EntityConfig entity)
    {
        var sb = new StringBuilder();

        // Usa colunas configuradas pelo wizard OU auto-gera
        var listProps = entity.Properties.Where(p => p.List?.Show == true).ToList();

        if (!listProps.Any())
        {
            // Auto-gera colunas para propriedades relevantes
            listProps = entity.Properties
                .Where(p => !IsAuditField(p)) // Exclui campos de auditoria
                .Where(p => !p.IsPrimaryKey || p.IsString) // Exclui PKs auto-geradas
                .OrderBy(p => p.Name)
                .Take(10) // Limita a 10 colunas principais
                .ToList();
        }

        foreach (var prop in listProps)
        {
            var align = prop.List?.Align ?? "left";
            var sortable = prop.List?.Sortable ?? true ? "true" : "false";
            var format = prop.List?.Format ?? GetDefaultFormat(prop);
            var width = !string.IsNullOrEmpty(prop.List?.Width) ? $"\n            width: '{prop.List!.Width}'," : "";

            // Nome da propriedade em camelCase para o JSON
            var dataName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);

            string render = format switch
            {
                "date" => $@",
            render: function (data) {{
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR');
            }}",
                "datetime" => $@",
            render: function (data) {{
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', {{ hour: '2-digit', minute: '2-digit' }});
            }}",
                "currency" => $@",
            render: function (data) {{
                if (data == null) return '-';
                return 'R$ ' + parseFloat(data).toLocaleString('pt-BR', {{ minimumFractionDigits: 2 }});
            }}",
                "percentage" => $@",
            render: function (data) {{
                if (data == null) return '-';
                return parseFloat(data).toFixed(2) + '%';
            }}",
                "boolean" => $@",
            render: function (data) {{
                const isTrue = data === true || data === 1 || data === '1';
                return isTrue
                    ? '<span class=""badge bg-success""><i class=""fas fa-check""></i></span>'
                    : '<span class=""badge bg-secondary""><i class=""fas fa-times""></i></span>';
            }}",
                _ => ""
            };

            sb.AppendLine($@"        // {prop.DisplayName}
        {{
            data: '{dataName}',
            name: '{prop.Name}',
            title: '{prop.DisplayName}',{width}
            orderable: {sortable},
            className: 'text-{align}'{render}
        }},");
        }

        var result = sb.ToString().TrimEnd('\r', '\n');
        return result;
    }

    /// <summary>
    /// ✅ v3.7: Determina formato default baseado no tipo da propriedade.
    /// </summary>
    private static string GetDefaultFormat(PropertyConfig prop)
    {
        if (prop.IsDateTime) return "date";
        if (prop.IsDecimal) return "currency";
        if (prop.IsBool) return "boolean";
        return "text";
    }

    /// <summary>
    /// ✅ v3.7: Verifica se é campo de auditoria (não deve aparecer na grid).
    /// </summary>
    private static bool IsAuditField(PropertyConfig prop)
    {
        var auditFields = new[]
        {
            "DataCriacao", "DtCriacao", "CreatedAt", "CreatedDate", "CreatedAtUtc",
            "UsuarioCriacao", "CreatedBy", "CreatedByUser", "CriadoPor", "CreatedByUserId",
            "DataAtualizacao", "DtAtualizacao", "UpdatedAt", "ModifiedAt", "ModifiedDate", "UpdatedAtUtc",
            "UsuarioAtualizacao", "UpdatedBy", "ModifiedBy", "ModifiedByUser", "AtualizadoPor", "UpdatedByUserId",
            "TenantId", "IdSaaS", "IdSaas"
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