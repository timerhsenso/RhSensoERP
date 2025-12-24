/**
 * ============================================================================
 * TSISTEMA - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/tsistemas/tsistema.js
 * Versão: 3.1 (Suporte a PKs de texto)
 * Gerado por: GeradorFullStack v3.1
 * Data: 2025-12-02 02:25:04
 * 
 * Implementação específica do CRUD de Tsistema.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class TsistemaCrud extends CrudBase {
    constructor(config) {
        super(config);
        
        // =====================================================================
        // CORREÇÃO v3.1: Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = 'CdsiStema';
        this.isPkTexto = true;
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * CORREÇÃO v3.1: PKs de texto são editáveis apenas na criação.
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
     * CORREÇÃO v3.1: Habilita PK de texto na criação.
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
     * CORREÇÃO v3.1: Desabilita PK de texto na edição.
     */
    async openEditModal(id) {
        await super.openEditModal(id);
        
        // Desabilita PK de texto (não pode alterar chave)
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(false);
        }
    }

    /**
     * Customização antes de submeter.
     * Converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        // Garante que PK de texto seja string trimada
        if (formData.cdsiStema) {
            formData.cdsiStema = String(formData.cdsiStema).trim();
        }


        console.log('📤 [Tsistema] Dados a enviar:', formData);
        return formData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [Tsistema] Registro salvo:', data);
    }

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {
        const id = row[this.config.idField] || row.cdsiStema || row.CdsiStema || row.id || row.Id || '';
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
                 row['cdsiStema'] || row['CdsiStema'] || row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {
            console.warn('⚠️ [Tsistema] ID vazio para row:', row);
        }

        return id;
    }

    // =========================================================================
    // CONFIGURAÇÃO DAS COLUNAS DO DATATABLES
    // =========================================================================

    const columns = [
        // Coluna de seleção (checkbox)
        {
            data: null,
            orderable: false,
            searchable: false,
            className: 'dt-checkboxes-cell',
            width: '40px',
            render: function (data, type, row) {
                // Só mostra checkbox se pode excluir
                if (window.crudPermissions.canDelete) {
                    const id = getCleanId(row, 'cdsiStema');
                    return `<input type="checkbox" class="dt-checkboxes form-check-input" data-id="${id}">`;
                }
                return '';
            }
        },
        // Cdsi Stema
        {
            data: 'cdsiStema',
            name: 'CdsiStema',
            title: 'Cdsi Stema',
            orderable: true,
            className: 'text-left'
        },
        // Dcsi Stema
        {
            data: 'dcsiStema',
            name: 'DcsiStema',
            title: 'Dcsi Stema',
            orderable: true,
            className: 'text-left'
        },
        // Coluna de ações
        {
            data: null,
            orderable: false,
            searchable: false,
            className: 'text-end no-export',
            title: 'Ações',
            width: '130px',
            render: function (data, type, row) {
                const id = getCleanId(row, 'cdsiStema');

                console.log('🔧 [Tsistema] Renderizando ações | ID:', id, '| Row:', row);

                let actions = '<div class="btn-group btn-group-sm" role="group">';

                // Botão Visualizar
                if (window.crudPermissions.canView) {
                    actions += `<button type="button" class="btn btn-info btn-view" 
                        data-id="${id}" 
                        data-bs-toggle="tooltip" 
                        title="Visualizar">
                        <i class="fas fa-eye"></i>
                    </button>`;
                }

                // Botão Editar
                if (window.crudPermissions.canEdit) {
                    actions += `<button type="button" class="btn btn-warning btn-edit" 
                        data-id="${id}" 
                        data-bs-toggle="tooltip" 
                        title="Editar">
                        <i class="fas fa-edit"></i>
                    </button>`;
                }

                // Botão Excluir
                if (window.crudPermissions.canDelete) {
                    actions += `<button type="button" class="btn btn-danger btn-delete" 
                        data-id="${id}" 
                        data-bs-toggle="tooltip" 
                        title="Excluir">
                        <i class="fas fa-trash"></i>
                    </button>`;
                }

                actions += '</div>';
                return actions;
            }
        }
    ];

    // =========================================================================
    // INICIALIZAÇÃO DO CRUD
    // =========================================================================

    window.tsistemaCrud = new TsistemaCrud({
        controllerName: 'Tsistemas',
        entityName: 'Tsistema',
        idField: 'cdsiStema',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {
            order: [[1, 'asc']]
        }
    });

    // =========================================================================
    // CONTROLE DE TOOLBAR BASEADO EM PERMISSÕES
    // =========================================================================

    // Desabilita botão "Novo" se não pode criar
    if (!window.crudPermissions.canCreate) {
        $('#btnNew').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para criar registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [Tsistema] Botão "Novo" desabilitado (sem permissão de criação)');
    }

    // Desabilita botão "Excluir Selecionados" se não pode excluir
    if (!window.crudPermissions.canDelete) {
        $('#btnDeleteSelected').prop('disabled', true)
            .addClass('disabled')
            .attr('title', 'Você não tem permissão para excluir registros')
            .css('cursor', 'not-allowed');

        console.log('🔒 [Tsistema] Botão "Excluir Selecionados" desabilitado (sem permissão de exclusão)');
    }

    // =========================================================================
    // LOG DE INICIALIZAÇÃO
    // =========================================================================

    console.log('✅ CRUD de Tsistema v3.1 inicializado com permissões:', {
        criar: window.crudPermissions.canCreate,
        editar: window.crudPermissions.canEdit,
        excluir: window.crudPermissions.canDelete,
        visualizar: window.crudPermissions.canView,
        pkTexto: true
    });
});
