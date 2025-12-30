/**
 * ============================================================================
 * CAPVISITANTES - JavaScript com Ordenação Server-Side
 * ============================================================================
 * Arquivo: wwwroot/js/controleacessoportaria/capvisitantes/capvisitantes.js
 * Módulo: ControleAcessoPortaria
 * Versão: 4.0 (COM ORDENAÇÃO FUNCIONAL)
 * Gerado por: GeradorFullStack v4.0
 * Data: 2025-12-30 04:08:11
 * 
 * Changelog v4.0:
 *   ✅ Ordenação server-side habilitada por padrão
 *   ✅ Colunas mapeadas com 'name' em PascalCase para backend
 *   ✅ Render functions para compatibilidade PascalCase/camelCase
 * 
 * Implementação específica do CRUD de CapVisitantes.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class CapVisitantesCrud extends CrudBase {
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
            console.log('✏️ [CapVisitantes] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [CapVisitantes] Campo PK desabilitado (edição)');
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
        console.log('📥 [CapVisitantes] Dados ANTES:', JSON.parse(JSON.stringify(formData)));

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
        cleanData.Cpf = formData.cpf || formData.Cpf || '';
        cleanData.Email = formData.email || formData.Email || '';
        cleanData.Empresa = formData.empresa || formData.Empresa || '';
        cleanData.Nome = formData.nome || formData.Nome || '';
        cleanData.Rg = formData.rg || formData.Rg || '';
        cleanData.Telefone = formData.telefone || formData.Telefone || '';

        // Integer nullable fields - PascalCase
        if (formData.idFuncionarioResponsavel !== undefined && formData.idFuncionarioResponsavel !== null && formData.idFuncionarioResponsavel !== '') {
            const val = parseInt(formData.idFuncionarioResponsavel, 10);
            cleanData.IdFuncionarioResponsavel = isNaN(val) ? null : val;
        } else if (formData.IdFuncionarioResponsavel !== undefined && formData.IdFuncionarioResponsavel !== null && formData.IdFuncionarioResponsavel !== '') {
            const val = parseInt(formData.IdFuncionarioResponsavel, 10);
            cleanData.IdFuncionarioResponsavel = isNaN(val) ? null : val;
        } else {
            cleanData.IdFuncionarioResponsavel = null;
        }

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

        const checkboxRequerResponsavel = document.getElementById('RequerResponsavel');
        if (checkboxRequerResponsavel) {
            cleanData.RequerResponsavel = checkboxRequerResponsavel.checked;
        } else {
            cleanData.RequerResponsavel = formData.requerResponsavel === true || 
                                    formData.RequerResponsavel === true || 
                                    formData.requerResponsavel === 'true' || 
                                    formData.requerResponsavel === 1;
        }


        console.log('📤 [CapVisitantes] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [CapVisitantes] Registro salvo:', data);
        
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

    console.log('🔐 [CapVisitantes] Permissões ativas:', window.crudPermissions);

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
            console.warn('⚠️ [CapVisitantes] ID vazio para row:', row);
        }

        return id;
    }

    // =========================================================================
    // ✅ v4.0: CONFIGURAÇÃO DAS COLUNAS COM ORDENAÇÃO
    // =========================================================================

    const columns = [
        // Coluna de seleção (checkbox)
        {
            data: null,
            name: null,                    // ✅ Não ordena
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
        // ✅ Nome - Ordenável
        {
            data: 'nome',
            name: 'Nome',          // ✅ PascalCase para backend
            title: 'Nome',
            orderable: true,         // ✅ CRÍTICO
            searchable: true,
            className: 'text-left',
            render: function (data, type, row) {
                return row.nome || row.Nome || '';
            }
        },
        // ✅ Cpf - Ordenável
        {
            data: 'cpf',
            name: 'Cpf',          // ✅ PascalCase para backend
            title: 'Cpf',
            orderable: true,         // ✅ CRÍTICO
            searchable: true,
            className: 'text-left',
            render: function (data, type, row) {
                return row.cpf || row.Cpf || '';
            }
        },
        // ✅ Email - Ordenável
        {
            data: 'email',
            name: 'Email',          // ✅ PascalCase para backend
            title: 'Email',
            orderable: true,         // ✅ CRÍTICO
            searchable: true,
            className: 'text-left',
            render: function (data, type, row) {
                return row.email || row.Email || '';
            }
        },
        // ✅ Ativo - Ordenável
        {
            data: 'ativo',
            name: 'Ativo',          // ✅ PascalCase para backend
            title: 'Ativo',
            orderable: true,         // ✅ CRÍTICO
            searchable: true,
            className: 'text-left',
            render: function (data, type, row) {
                const valor = row.ativo !== undefined ? row.ativo : row.Ativo;
                const isTrue = valor === true || valor === 1 || valor === '1';
                return isTrue
                    ? '<span class="badge bg-success"><i class="fas fa-check"></i></span>'
                    : '<span class="badge bg-secondary"><i class="fas fa-times"></i></span>';
            }
        },
        // Coluna de ações
        {
            data: null,
            name: null,                    // ✅ Não ordena
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
    // ✅ v4.0: INICIALIZAÇÃO DO CRUD COM ORDENAÇÃO HABILITADA
    // =========================================================================

    window.capvisitantesCrud = new CapVisitantesCrud({
        controllerName: 'CapVisitantes',
        entityName: 'CapVisitantes',
        idField: 'id',
        columns: columns,
        permissions: window.crudPermissions,
        dataTableOptions: {
            // ✅ CRÍTICO: Habilita server-side processing e ordenação
            serverSide: true,
            processing: true,
            ordering: true,
            
            // ✅ Ordenação inicial (primeira coluna de dados)
            order: [[1, 'asc']],
            
            pageLength: 25,
            
            // Idioma PT-BR
            language: {
                processing: "Processando...",
                emptyTable: "Nenhum registro encontrado",
                info: "Mostrando _START_ até _END_ de _TOTAL_ registros",
                infoEmpty: "Mostrando 0 até 0 de 0 registros",
                infoFiltered: "(filtrado de _MAX_ registros)",
                lengthMenu: "Mostrar _MENU_ registros",
                loadingRecords: "Carregando...",
                search: "Buscar:",
                zeroRecords: "Nenhum registro encontrado",
                paginate: {
                    first: "Primeiro",
                    previous: "Anterior",
                    next: "Próximo",
                    last: "Último"
                }
            }
        }
    });

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================

    // CrudBase inicializa automaticamente no construtor
    console.log('✅ [CapVisitantes] CRUD inicializado com ordenação server-side (v4.0)');
});
