/**
 * ============================================================================
 * TEST ENTITY COMPLETE - JavaScript com Controle de Permissões
 * ============================================================================
 * Arquivo: wwwroot/js/gestaodepessoas/testentitycomplete/testentitycomplete.js
 * Módulo: GestaoDePessoas
 * Versão: 3.9 (PascalCase para model binding)
 * Gerado por: GeradorFullStack v3.9
 * Data: 2025-12-28 14:22:46
 * 
 * Implementação específica do CRUD de Test Entity Complete.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class TestEntityCompleteCrud extends CrudBase {
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
            console.log('✏️ [TestEntityComplete] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [TestEntityComplete] Campo PK desabilitado (edição)');
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
        console.log('📥 [TestEntityComplete] Dados ANTES:', JSON.parse(JSON.stringify(formData)));

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
        cleanData.Cnpj = formData.cnpj || formData.Cnpj || '';
        cleanData.Codigo = formData.codigo || formData.Codigo || '';
        cleanData.Cpf = formData.cpf || formData.Cpf || '';
        cleanData.DataComOffset = formData.dataComOffset || formData.DataComOffset || '';
        cleanData.DeletedBy = formData.deletedBy || formData.DeletedBy || '';
        cleanData.Descricao = formData.descricao || formData.Descricao || '';
        cleanData.DuracaoTimeSpan = formData.duracaoTimeSpan || formData.DuracaoTimeSpan || '';
        cleanData.Email = formData.email || formData.Email || '';
        cleanData.JsonConfig = formData.jsonConfig || formData.JsonConfig || '';
        cleanData.JsonData = formData.jsonData || formData.JsonData || '';
        cleanData.JsonMetadata = formData.jsonMetadata || formData.JsonMetadata || '';
        cleanData.Nome = formData.nome || formData.Nome || '';
        cleanData.ObservacoesMax = formData.observacoesMax || formData.ObservacoesMax || '';
        cleanData.Prioridade = formData.prioridade || formData.Prioridade || '';
        cleanData.Status = formData.status || formData.Status || '';
        cleanData.StatusNullable = formData.statusNullable || formData.StatusNullable || '';
        cleanData.Telefone = formData.telefone || formData.Telefone || '';
        cleanData.TextoPlano = formData.textoPlano || formData.TextoPlano || '';
        cleanData.Url = formData.url || formData.Url || '';
        cleanData.XmlContent = formData.xmlContent || formData.XmlContent || '';
        cleanData.XmlSettings = formData.xmlSettings || formData.XmlSettings || '';

        // Integer nullable fields - PascalCase
        if (formData.byteNullable !== undefined && formData.byteNullable !== null && formData.byteNullable !== '') {
            const val = parseInt(formData.byteNullable, 10);
            cleanData.ByteNullable = isNaN(val) ? null : val;
        } else if (formData.ByteNullable !== undefined && formData.ByteNullable !== null && formData.ByteNullable !== '') {
            const val = parseInt(formData.ByteNullable, 10);
            cleanData.ByteNullable = isNaN(val) ? null : val;
        } else {
            cleanData.ByteNullable = null;
        }

        if (formData.categoryId !== undefined && formData.categoryId !== null && formData.categoryId !== '') {
            const val = parseInt(formData.categoryId, 10);
            cleanData.CategoryId = isNaN(val) ? null : val;
        } else if (formData.CategoryId !== undefined && formData.CategoryId !== null && formData.CategoryId !== '') {
            const val = parseInt(formData.CategoryId, 10);
            cleanData.CategoryId = isNaN(val) ? null : val;
        } else {
            cleanData.CategoryId = null;
        }

        if (formData.deletedByUserId !== undefined && formData.deletedByUserId !== null && formData.deletedByUserId !== '') {
            const val = parseInt(formData.deletedByUserId, 10);
            cleanData.DeletedByUserId = isNaN(val) ? null : val;
        } else if (formData.DeletedByUserId !== undefined && formData.DeletedByUserId !== null && formData.DeletedByUserId !== '') {
            const val = parseInt(formData.DeletedByUserId, 10);
            cleanData.DeletedByUserId = isNaN(val) ? null : val;
        } else {
            cleanData.DeletedByUserId = null;
        }

        if (formData.idEmpresa !== undefined && formData.idEmpresa !== null && formData.idEmpresa !== '') {
            const val = parseInt(formData.idEmpresa, 10);
            cleanData.IdEmpresa = isNaN(val) ? null : val;
        } else if (formData.IdEmpresa !== undefined && formData.IdEmpresa !== null && formData.IdEmpresa !== '') {
            const val = parseInt(formData.IdEmpresa, 10);
            cleanData.IdEmpresa = isNaN(val) ? null : val;
        } else {
            cleanData.IdEmpresa = null;
        }

        if (formData.idFilial !== undefined && formData.idFilial !== null && formData.idFilial !== '') {
            const val = parseInt(formData.idFilial, 10);
            cleanData.IdFilial = isNaN(val) ? null : val;
        } else if (formData.IdFilial !== undefined && formData.IdFilial !== null && formData.IdFilial !== '') {
            const val = parseInt(formData.IdFilial, 10);
            cleanData.IdFilial = isNaN(val) ? null : val;
        } else {
            cleanData.IdFilial = null;
        }

        if (formData.intNullable !== undefined && formData.intNullable !== null && formData.intNullable !== '') {
            const val = parseInt(formData.intNullable, 10);
            cleanData.IntNullable = isNaN(val) ? null : val;
        } else if (formData.IntNullable !== undefined && formData.IntNullable !== null && formData.IntNullable !== '') {
            const val = parseInt(formData.IntNullable, 10);
            cleanData.IntNullable = isNaN(val) ? null : val;
        } else {
            cleanData.IntNullable = null;
        }

        if (formData.longNullable !== undefined && formData.longNullable !== null && formData.longNullable !== '') {
            const val = parseInt(formData.longNullable, 10);
            cleanData.LongNullable = isNaN(val) ? null : val;
        } else if (formData.LongNullable !== undefined && formData.LongNullable !== null && formData.LongNullable !== '') {
            const val = parseInt(formData.LongNullable, 10);
            cleanData.LongNullable = isNaN(val) ? null : val;
        } else {
            cleanData.LongNullable = null;
        }

        if (formData.ownerId !== undefined && formData.ownerId !== null && formData.ownerId !== '') {
            const val = parseInt(formData.ownerId, 10);
            cleanData.OwnerId = isNaN(val) ? null : val;
        } else if (formData.OwnerId !== undefined && formData.OwnerId !== null && formData.OwnerId !== '') {
            const val = parseInt(formData.OwnerId, 10);
            cleanData.OwnerId = isNaN(val) ? null : val;
        } else {
            cleanData.OwnerId = null;
        }

        if (formData.parentId !== undefined && formData.parentId !== null && formData.parentId !== '') {
            const val = parseInt(formData.parentId, 10);
            cleanData.ParentId = isNaN(val) ? null : val;
        } else if (formData.ParentId !== undefined && formData.ParentId !== null && formData.ParentId !== '') {
            const val = parseInt(formData.ParentId, 10);
            cleanData.ParentId = isNaN(val) ? null : val;
        } else {
            cleanData.ParentId = null;
        }

        if (formData.shortNullable !== undefined && formData.shortNullable !== null && formData.shortNullable !== '') {
            const val = parseInt(formData.shortNullable, 10);
            cleanData.ShortNullable = isNaN(val) ? null : val;
        } else if (formData.ShortNullable !== undefined && formData.ShortNullable !== null && formData.ShortNullable !== '') {
            const val = parseInt(formData.ShortNullable, 10);
            cleanData.ShortNullable = isNaN(val) ? null : val;
        } else {
            cleanData.ShortNullable = null;
        }

        // Integer required fields - PascalCase
        cleanData.ByteField = parseInt(formData.byteField || formData.ByteField || 0, 10);

        cleanData.IntField = parseInt(formData.intField || formData.IntField || 0, 10);

        cleanData.LongField = parseInt(formData.longField || formData.LongField || 0, 10);

        cleanData.QuantidadeComRange = parseInt(formData.quantidadeComRange || formData.QuantidadeComRange || 0, 10);

        cleanData.ShortField = parseInt(formData.shortField || formData.ShortField || 0, 10);

        // Decimal fields - PascalCase
        cleanData.DecimalMoeda = parseFloat((formData.decimalMoeda || formData.DecimalMoeda || '0').toString().replace(',', '.'));

        if (formData.decimalNullable !== undefined && formData.decimalNullable !== null && formData.decimalNullable !== '') {
            cleanData.DecimalNullable = parseFloat((formData.decimalNullable || '0').toString().replace(',', '.'));
        } else {
            cleanData.DecimalNullable = null;
        }

        cleanData.DecimalPadrao = parseFloat((formData.decimalPadrao || formData.DecimalPadrao || '0').toString().replace(',', '.'));

        cleanData.DecimalPercentual = parseFloat((formData.decimalPercentual || formData.DecimalPercentual || '0').toString().replace(',', '.'));

        cleanData.DecimalPreciso = parseFloat((formData.decimalPreciso || formData.DecimalPreciso || '0').toString().replace(',', '.'));

        cleanData.DoubleField = parseFloat((formData.doubleField || formData.DoubleField || '0').toString().replace(',', '.'));

        if (formData.doubleNullable !== undefined && formData.doubleNullable !== null && formData.doubleNullable !== '') {
            cleanData.DoubleNullable = parseFloat((formData.doubleNullable || '0').toString().replace(',', '.'));
        } else {
            cleanData.DoubleNullable = null;
        }

        cleanData.FloatField = parseFloat((formData.floatField || formData.FloatField || '0').toString().replace(',', '.'));

        if (formData.floatNullable !== undefined && formData.floatNullable !== null && formData.floatNullable !== '') {
            cleanData.FloatNullable = parseFloat((formData.floatNullable || '0').toString().replace(',', '.'));
        } else {
            cleanData.FloatNullable = null;
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

        const checkboxBloqueado = document.getElementById('Bloqueado');
        if (checkboxBloqueado) {
            cleanData.Bloqueado = checkboxBloqueado.checked;
        } else {
            cleanData.Bloqueado = formData.bloqueado === true || 
                                    formData.Bloqueado === true || 
                                    formData.bloqueado === 'true' || 
                                    formData.bloqueado === 1;
        }

        const checkboxBoolNullable = document.getElementById('BoolNullable');
        if (checkboxBoolNullable) {
            cleanData.BoolNullable = checkboxBoolNullable.checked;
        } else {
            cleanData.BoolNullable = formData.boolNullable === true || 
                                    formData.BoolNullable === true || 
                                    formData.boolNullable === 'true' || 
                                    formData.boolNullable === 1;
        }

        const checkboxIsDeleted = document.getElementById('IsDeleted');
        if (checkboxIsDeleted) {
            cleanData.IsDeleted = checkboxIsDeleted.checked;
        } else {
            cleanData.IsDeleted = formData.isDeleted === true || 
                                    formData.IsDeleted === true || 
                                    formData.isDeleted === 'true' || 
                                    formData.isDeleted === 1;
        }

        // DateTime fields - PascalCase
        cleanData.DataAntiga = (formData.dataAntiga || formData.DataAntiga) || null;

        cleanData.DataHoraAltaPrecisao = (formData.dataHoraAltaPrecisao || formData.DataHoraAltaPrecisao) || null;

        cleanData.DataHoraNullable = (formData.dataHoraNullable || formData.DataHoraNullable) || null;

        cleanData.DataHoraPadrao = formData.dataHoraPadrao || formData.DataHoraPadrao || new Date().toISOString();

        cleanData.DataHoraSemPrecisao = (formData.dataHoraSemPrecisao || formData.DataHoraSemPrecisao) || null;

        cleanData.DataSmallDateTime = (formData.dataSmallDateTime || formData.DataSmallDateTime) || null;

        cleanData.DeletedAtUtc = (formData.deletedAtUtc || formData.DeletedAtUtc) || null;

        // Guid nullable fields - PascalCase
        cleanData.ExternalId = (formData.externalId || formData.ExternalId) || null;

        cleanData.GuidNullable = (formData.guidNullable || formData.GuidNullable) || null;


        console.log('📤 [TestEntityComplete] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [TestEntityComplete] Registro salvo:', data);
        
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

    console.log('🔐 [TestEntityComplete] Permissões ativas:', window.crudPermissions);

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
            console.warn('⚠️ [TestEntityComplete] ID vazio para row:', row);
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
        // IdEmpresa
        {
            data: 'idEmpresa',
            name: 'IdEmpresa',
            title: 'IdEmpresa',
            orderable: true,
            className: 'text-left'
        },
        // IdFilial
        {
            data: 'idFilial',
            name: 'IdFilial',
            title: 'IdFilial',
            orderable: true,
            className: 'text-left'
        },
        // Codigo
        {
            data: 'codigo',
            name: 'Codigo',
            title: 'Codigo',
            orderable: true,
            className: 'text-left'
        },
        // Nome
        {
            data: 'nome',
            name: 'Nome',
            title: 'Nome',
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

    window.testentitycompleteCrud = new TestEntityCompleteCrud({
        controllerName: 'TestEntityComplete',
        entityName: 'TestEntityComplete',
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
    console.log('✅ [TestEntityComplete] CRUD inicializado com sucesso (v3.9 - PascalCase)');
});
