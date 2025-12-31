/**
 * ============================================================================
 * TABELA DE SISTEMAS - JavaScript com Checkbox e Toggle Ativo
 * ============================================================================
 * Arquivo: wwwroot/js/seguranca/tsistema/tsistema.js
 * Módulo: Seguranca
 * Versão: 4.3 (FINAL - 100% FUNCIONAL)
 * Gerado por: GeradorFullStack v4.3
 * Data: 2025-12-30 17:48:37
 * 
 * Changelog v4.3:
 *   ✅ CRÍTICO: Geração automática inteligente de colunas (não depende de Grid)
 *   ✅ CRÍTICO: Resolve 100% do erro "aDataSort" do DataTables
 *   ✅ CRÍTICO: Heurísticas: Form.Show, tipos comuns, ordem alfabética
 * 
 * Changelog v4.2:
 *   ✅ CORRIGIDO: dataTableColumns → columns (compatível com CrudBase)
 *   ✅ CORRIGIDO: Parâmetros obrigatórios do CrudBase adicionados
 *   ✅ CORRIGIDO: idField em lowercase, classes CSS corretas
 * 
 * Changelog v4.1:
 *   ✅ Checkbox "Selecionar Todos" no header da DataTable
 *   ✅ Toggle Switch dinâmico para campo Ativo (rate limit 500ms)
 *   ✅ Exclusão múltipla com contador
 * 
 * Implementação específica do CRUD de Tabela de Sistemas.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class TsistemaCrud extends CrudBase {
    constructor(config) {
        super(config);
        
        // =====================================================================
        // Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = 'CdsiStema';
        this.isPkTexto = true;
        
        // =====================================================================
        // v4.1: Debounce para Toggle Ativo
        // =====================================================================
        this.toggleDebounceTimer = null;
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * PKs de texto são editáveis apenas na criação.
     */
    enablePrimaryKeyFields(enable) {
        if (!this.isPkTexto) return;
        
        const $pkField = $('#' + this.pkTextoField);
        if ($pkField.length === 0) return;
        
        if (enable) {
            // Criação: campo editável
            $pkField.prop('readonly', false)
                    .prop('disabled', false)
                    .removeClass('bg-light');
            console.log('✏️ [Tsistema] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [Tsistema] Campo PK desabilitado (edição)');
        }
    }

    /**
     * Override: Abre modal para NOVO registro.
     * Habilita PK de texto na criação.
     */
    openCreateModal() {
        super.openCreateModal();
        
        // Habilita PK de texto para digitação
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(true);
        }
    }

    /**
     * Override: Abre modal para EDIÇÃO.
     * Desabilita PK de texto na edição.
     */
    async openEditModal(id) {
        await super.openEditModal(id);
        
        // Desabilita PK de texto (não pode alterar chave)
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(false);
        }
    }

    /**
     * ⭐ v3.9 CORRIGIDO: Retorna objeto em PascalCase
     * Remove campos de auditoria, converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        console.log('📥 [Tsistema] Dados ANTES:', JSON.parse(JSON.stringify(formData)));

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

        // =====================================================================
        // ⭐ v3.9: CRIA OBJETO LIMPO EM PASCALCASE (model binding ASP.NET Core)
        // =====================================================================
        const cleanData = {};

        // String fields - PascalCase
        cleanData.CdsiStema = formData.cdsiStema || formData.CdsiStema || '';
        cleanData.DcsiStema = formData.dcsiStema || formData.DcsiStema || '';


        console.log('📤 [Tsistema] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [Tsistema] Registro salvo:', data);
        
        // Atualiza a grid automaticamente
        if (this.dataTable) {
            this.dataTable.ajax.reload(null, false); // Mantém paginação
        }
    }

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {
        const id = row[this.config.idField] || row.id || row.Id || '';
        return typeof id === 'string' ? id.trim() : id;
    }
}

// Inicialização quando o documento estiver pronto
$(document).ready(function () {

    // =========================================================================
    // VERIFICAÇÃO DE PERMISSÕES
    // =========================================================================

    // Verifica se as permissões foram injetadas pela View
    if (typeof window.crudPermissions === 'undefined') {
        console.error('❌ Permissões não foram carregadas! Usando valores padrão.');
        window.crudPermissions = {
            canCreate: false,
            canEdit: false,
            canDelete: false,
            canView: true
        };
    }

    console.log('🔐 [Tsistema] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {
            console.warn('⚠️ [Tsistema] ID vazio para row:', row);
        }

        return id;
    }

    // =========================================================================
    // ✅ v4.2: CONFIGURAÇÃO DAS COLUNAS (CORRIGIDO)
    // =========================================================================

    const columns = [
        // =====================================================================
        // v4.1: COLUNA DE SELEÇÃO (CHECKBOX)
        // =====================================================================
        {
            data: null,
            name: 'Select',
            title: '<input type="checkbox" id="selectAll" class="form-check-input" />',
            orderable: false,
            searchable: false,
            width: '30px',
            className: 'text-center no-export',
            render: function (data, type, row) {
                const id = getCleanId(row, 'cdsiStema');
                return `<input type="checkbox" class="form-check-input row-select dt-checkboxes" value="${id}" data-id="${id}" />`;
            }
        },
        // CdsiStema
        {
            data: 'cdsiStema',
            name: 'CdsiStema',
            title: 'CdsiStema',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // DcsiStema
        {
            data: 'dcsiStema',
            name: 'DcsiStema',
            title: 'DcsiStema',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Ações
        {
            data: null,
            name: 'Actions',
            title: 'Ações',
            orderable: false,
            searchable: false,
            width: '100px',
            className: 'text-center no-export',
            render: function (data, type, row) {
                const id = getCleanId(row, 'cdsiStema');
                let actions = '';
                
                if (window.crudPermissions.canEdit) {
                    actions += `<button class="btn btn-sm btn-primary btn-edit" data-id="${id}" title="Editar">
                                    <i class="fas fa-edit"></i>
                                </button> `;
                }
                
                if (window.crudPermissions.canDelete) {
                    actions += `<button class="btn btn-sm btn-danger btn-delete" data-id="${id}" title="Excluir">
                                    <i class="fas fa-trash"></i>
                                </button>`;
                }
                
                return actions || '<span class="text-muted">Sem ações</span>';
            }
        }

    ];

    // =========================================================================
    // ✅ v4.2: INSTANCIA O CRUD (CORRIGIDO: TODOS OS PARÂMETROS)
    // =========================================================================

    const crud = new TsistemaCrud({
        controllerName: 'Tsistema',
        entityName: 'Tabela de Sistemas',
        entityNamePlural: 'Tabela de Sistemass',
        idField: 'cdsiStema',
        tableSelector: '#tableCrud',
        columns: columns,  // ✅ CORRIGIDO: era "dataTableColumns"
        permissions: window.crudPermissions,
        exportConfig: {
            enabled: true,
            excel: true,
            pdf: true,
            csv: true,
            print: true,
            filename: 'Tsistema'
        }
    });

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX "SELECIONAR TODOS"
    // =========================================================================

    $('#tableCrud').on('click', '#selectAll', function () {
        const isChecked = $(this).prop('checked');
        $('.row-select').prop('checked', isChecked);
        crud.updateSelectedCount();
        console.log(`${isChecked ? '✅' : '❌'} Selecionou todos os registros`);
    });

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX INDIVIDUAL
    // =========================================================================

    $(document).on('change', '.row-select', function () {
        const totalCheckboxes = $('.row-select').length;
        const checkedCheckboxes = $('.row-select:checked').length;
        
        // Atualiza estado do "Selecionar Todos"
        $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
        
        crud.updateSelectedCount();
    });



    console.log('✅ [Tsistema] JavaScript inicializado com sucesso!');
});
