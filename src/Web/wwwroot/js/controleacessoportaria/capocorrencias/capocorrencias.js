/**
 * ============================================================================
 * CAPOCORRENCIAS - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/controleacessoportaria/capocorrencias/capocorrencias.js
 * Módulo: ControleAcessoPortaria
 * Versão: 3.9 (PascalCase para model binding)
 * Gerado por: GeradorFullStack v3.9
 * Data: 2025-12-28 20:36:06
 * 
 * Implementação específica do CRUD de CapOcorrencias.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class CapOcorrenciasCrud extends CrudBase {
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
            console.log('✏️ [CapOcorrencias] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [CapOcorrencias] Campo PK desabilitado (edição)');
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
        console.log('📥 [CapOcorrencias] Dados ANTES:', JSON.parse(JSON.stringify(formData)));

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
        cleanData.Descricao = formData.descricao || formData.Descricao || '';
        cleanData.Local = formData.local || formData.Local || '';
        cleanData.Observacoes = formData.observacoes || formData.Observacoes || '';
        cleanData.Status = formData.status || formData.Status || '';

        // Integer nullable fields - PascalCase
        if (formData.idAcessoPortaria !== undefined && formData.idAcessoPortaria !== null && formData.idAcessoPortaria !== '') {
            const val = parseInt(formData.idAcessoPortaria, 10);
            cleanData.IdAcessoPortaria = isNaN(val) ? null : val;
        } else if (formData.IdAcessoPortaria !== undefined && formData.IdAcessoPortaria !== null && formData.IdAcessoPortaria !== '') {
            const val = parseInt(formData.IdAcessoPortaria, 10);
            cleanData.IdAcessoPortaria = isNaN(val) ? null : val;
        } else {
            cleanData.IdAcessoPortaria = null;
        }

        if (formData.idColaboradorResponsavel !== undefined && formData.idColaboradorResponsavel !== null && formData.idColaboradorResponsavel !== '') {
            const val = parseInt(formData.idColaboradorResponsavel, 10);
            cleanData.IdColaboradorResponsavel = isNaN(val) ? null : val;
        } else if (formData.IdColaboradorResponsavel !== undefined && formData.IdColaboradorResponsavel !== null && formData.IdColaboradorResponsavel !== '') {
            const val = parseInt(formData.IdColaboradorResponsavel, 10);
            cleanData.IdColaboradorResponsavel = isNaN(val) ? null : val;
        } else {
            cleanData.IdColaboradorResponsavel = null;
        }

        if (formData.idFuncionarioResponsavel !== undefined && formData.idFuncionarioResponsavel !== null && formData.idFuncionarioResponsavel !== '') {
            const val = parseInt(formData.idFuncionarioResponsavel, 10);
            cleanData.IdFuncionarioResponsavel = isNaN(val) ? null : val;
        } else if (formData.IdFuncionarioResponsavel !== undefined && formData.IdFuncionarioResponsavel !== null && formData.IdFuncionarioResponsavel !== '') {
            const val = parseInt(formData.IdFuncionarioResponsavel, 10);
            cleanData.IdFuncionarioResponsavel = isNaN(val) ? null : val;
        } else {
            cleanData.IdFuncionarioResponsavel = null;
        }

        // Integer required fields - PascalCase
        cleanData.IdTipoOcorrencia = parseInt(formData.idTipoOcorrencia || formData.IdTipoOcorrencia || 0, 10);

        // ⭐ Boolean fields - PascalCase - Pega direto do DOM (checkbox)
        const checkboxAtivo = document.getElementById('Ativo');
        if (checkboxAtivo) {
            cleanData.Ativo = checkboxAtivo.checked;
        } else {
            cleanData.Ativo = formData.ativo === true || 
                                    formData.Ativo === true || 
                                    formData.ativo === 'true' || 
                                    formData.ativo === 1;
        }

        // DateTime fields - PascalCase
        cleanData.DataOcorrencia = formData.dataOcorrencia || formData.DataOcorrencia || new Date().toISOString();


        console.log('📤 [CapOcorrencias] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [CapOcorrencias] Registro salvo:', data);
        
        // Atualiza a grid automaticamente
        if (this.table) {
            this.table.ajax.reload(null, false); // Mantém paginação
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

    console.log('🔐 [CapOcorrencias] Permissões ativas:', window.crudPermissions);

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
            console.warn('⚠️ [CapOcorrencias] ID vazio para row:', row);
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
        // IdTipoOcorrencia
        {
            data: 'idTipoOcorrencia',
            name: 'IdTipoOcorrencia',
            title: 'IdTipoOcorrencia',
            orderable: true,
            className: 'text-left'
        },
        // IdAcessoPortaria
        {
            data: 'idAcessoPortaria',
            name: 'IdAcessoPortaria',
            title: 'IdAcessoPortaria',
            orderable: true,
            className: 'text-left'
        },
        // DataOcorrencia
        {
            data: 'dataOcorrencia',
            name: 'DataOcorrencia',
            title: 'DataOcorrencia',
            orderable: true,
            className: 'text-left',
            render: function (data) {
                if (!data) return '-';
                const date = new Date(data);
                return date.toLocaleDateString('pt-BR') + ' ' + 
                       date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
            }
        },
        // Status
        {
            data: 'status',
            name: 'Status',
            title: 'Status',
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
                const id = getCleanId(row, 'id');

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

    window.capocorrenciasCrud = new CapOcorrenciasCrud({
        controllerName: 'CapOcorrencias',
        entityName: 'CapOcorrencias',
        idField: 'id',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {
            order: [[1, 'asc']],
            pageLength: 25
        }
    });

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================

    // CrudBase inicializa automaticamente no construtor
    console.log('✅ [CapOcorrencias] CRUD inicializado com sucesso (v3.9 - PascalCase)');
});
