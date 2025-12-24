/**
 * ============================================================================
 * CAPRESPONSAVEISCONTRATO - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/controleacessoportaria/capresponsaveiscontrato/capresponsaveiscontrato.js
 * Módulo: ControleAcessoPortaria
 * Versão: 3.7 (Geração automática de colunas)
 * Gerado por: GeradorFullStack v3.7
 * Data: 2025-12-24 01:15:50
 * 
 * Implementação específica do CRUD de CapResponsaveisContrato.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class CapResponsaveisContratoCrud extends CrudBase {
    constructor(config) {
        super(config);
        
        // =====================================================================
        // Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = null;
        this.isPkTexto = false;
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
            console.log('✏️ [CapResponsaveisContrato] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [CapResponsaveisContrato] Campo PK desabilitado (edição)');
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
     * Customização antes de submeter.
     * Converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        // Converte campos inteiros
        ['idContrato', 'idFuncionarioLegado'].forEach(field => {
            if (formData[field] !== undefined && formData[field] !== '') {
                formData[field] = parseInt(formData[field], 10);
            }
        });

        // Converte checkboxes para 0/1
        ['Ativo'].forEach(field => {
            const key = field.charAt(0).toLowerCase() + field.slice(1);
            const checkbox = document.getElementById(field);
            if (checkbox) {
                formData[key] = checkbox.checked ? 1 : 0;
            } else if (formData[key] === true || formData[key] === 'true' || formData[key] === 'on') {
                formData[key] = 1;
            } else if (formData[key] === false || formData[key] === 'false' || formData[key] === '' || formData[key] === undefined) {
                formData[key] = 0;
            }
        });


        console.log('📤 [CapResponsaveisContrato] Dados a enviar:', formData);
        return formData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [CapResponsaveisContrato] Registro salvo:', data);
    }

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {
        const id = row[this.config.idField] || row.id || row.Id || row.id || row.Id || '';
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

    console.log('🔐 [CapResponsaveisContrato] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['id'] || row['Id'] || row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {
            console.warn('⚠️ [CapResponsaveisContrato] ID vazio para row:', row);
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
                    const id = getCleanId(row, 'id');
                    return `<input type="checkbox" class="dt-checkboxes form-check-input" data-id="${id}">`;
                }
                return '';
            }
        },
        // Id
        {
            data: 'id',
            name: 'Id',
            title: 'Id',
            orderable: true,
            className: 'text-left'
        },
        // IdContrato
        {
            data: 'idContrato',
            name: 'IdContrato',
            title: 'IdContrato',
            orderable: true,
            className: 'text-left'
        },
        // IdFuncionarioLegado
        {
            data: 'idFuncionarioLegado',
            name: 'IdFuncionarioLegado',
            title: 'IdFuncionarioLegado',
            orderable: true,
            className: 'text-left'
        },
        // TipoResponsabilidade
        {
            data: 'tipoResponsabilidade',
            name: 'TipoResponsabilidade',
            title: 'TipoResponsabilidade',
            orderable: true,
            className: 'text-left'
        },
        // DataInicio
        {
            data: 'dataInicio',
            name: 'DataInicio',
            title: 'DataInicio',
            orderable: true,
            className: 'text-left',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
            }
        },
        // DataFim
        {
            data: 'dataFim',
            name: 'DataFim',
            title: 'DataFim',
            orderable: true,
            className: 'text-left',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
            }
        },
        // Ativo
        {
            data: 'ativo',
            name: 'Ativo',
            title: 'Ativo',
            orderable: true,
            className: 'text-left',
            render: function (data) {
                const isTrue = data === true || data === 1 || data === '1';
                return isTrue
                    ? '<span class="badge bg-success"><i class="fas fa-check"></i></span>'
                    : '<span class="badge bg-secondary"><i class="fas fa-times"></i></span>';
            }
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
                const id = getCleanId(row, 'id');

                console.log('🔧 [CapResponsaveisContrato] Renderizando ações | ID:', id, '| Row:', row);

                let actions = '<div class="btn-group btn-group-sm" role="group">';

                // Botão Editar (somente se tiver permissão)
                if (window.crudPermissions.canEdit) {
                    actions += `<button type="button" class="btn btn-outline-primary btn-edit" 
                                data-id="${id}" title="Editar">
                                <i class="fas fa-edit"></i>
                            </button>`;
                }

                // Botão Excluir (somente se tiver permissão)
                if (window.crudPermissions.canDelete) {
                    actions += `<button type="button" class="btn btn-outline-danger btn-delete" 
                                data-id="${id}" title="Excluir">
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

    window.capresponsaveiscontratoCrud = new CapResponsaveisContratoCrud({
        controllerName: 'CapResponsaveisContrato',
        entityName: 'CapResponsaveisContrato',
        idField: 'id',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {
            order: [[1, 'asc']]
        }
    });

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================

    // CrudBase inicializa automaticamente no construtor
    console.log('✅ [CapResponsaveisContrato] CRUD inicializado com sucesso');
});
