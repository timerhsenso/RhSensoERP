/**
 * ============================================================================
 * CRUD BASE - JavaScript Reutilizável para Operações CRUD
 * ============================================================================
 * Arquivo: wwwroot/js/crud-base.js
 * Versão: 3.1 (Corrigido - Trim automático nos IDs + Debug aprimorado)
 * 
 * Classe base para implementação de CRUDs com DataTables.
 * Fornece funcionalidades reutilizáveis como:
 * - Inicialização e configuração do DataTables
 * - Operações CRUD (Create, Read, Update, Delete)
 * - Exportação (Excel, PDF, CSV, Print)
 * - Seleção múltipla e exclusão em lote
 * - Validação de formulários
 * - Feedback visual com SweetAlert2
 * 
 * CORREÇÕES v3.1:
 * - Trim automático nos IDs capturados dos botões de ação
 * - Validação de ID vazio antes de chamar endpoints
 * - Debug melhorado para identificar problemas
 * 
 * ============================================================================
 */

class CrudBase {
    /**
     * Construtor da classe CrudBase.
     * @param {Object} config - Configurações do CRUD
     */
    constructor(config) {
        // Configurações obrigatórias
        this.controllerName = config.controllerName;
        this.entityName = config.entityName || 'Registro';
        this.entityNamePlural = config.entityNamePlural || 'Registros';
        this.idField = config.idField || 'id';
        this.tableSelector = config.tableSelector || '#tableCrud';
        this.columns = config.columns || [];

        // Configurações opcionais
        this.modalSelector = config.modalSelector || '#modalCrud';
        this.formSelector = config.formSelector || '#formCrud';
        this.permissions = config.permissions || {
            canCreate: true,
            canEdit: true,
            canDelete: true,
            canView: true
        };
        this.exportConfig = config.exportConfig || {
            enabled: true,
            excel: true,
            pdf: true,
            csv: true,
            print: true,
            filename: this.entityNamePlural
        };

        // Estado interno
        this.dataTable = null;
        this.isEditMode = false;
        this.currentId = null;
        this.selectedIds = [];

        // Inicialização
        this.init();
    }

    /**
     * Inicializa o CRUD.
     */
    init() {
        this.initDataTable();
        this.bindEvents();
        this.initValidation();
    }

    /**
     * Inicializa o DataTables.
     */
    initDataTable() {
        const self = this;
        const token = $('input[name="__RequestVerificationToken"]').val();

        // Configuração dos botões de exportação
        const buttons = [];
        if (this.exportConfig.enabled) {
            if (this.exportConfig.excel) {
                buttons.push({
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel"></i> Excel',
                    className: 'btn btn-success btn-sm',
                    exportOptions: {
                        columns: ':not(.no-export)'
                    },
                    filename: this.exportConfig.filename
                });
            }
            if (this.exportConfig.pdf) {
                buttons.push({
                    extend: 'pdf',
                    text: '<i class="fas fa-file-pdf"></i> PDF',
                    className: 'btn btn-danger btn-sm',
                    exportOptions: {
                        columns: ':not(.no-export)'
                    },
                    filename: this.exportConfig.filename
                });
            }
            if (this.exportConfig.csv) {
                buttons.push({
                    extend: 'csv',
                    text: '<i class="fas fa-file-csv"></i> CSV',
                    className: 'btn btn-secondary btn-sm',
                    exportOptions: {
                        columns: ':not(.no-export)'
                    },
                    filename: this.exportConfig.filename
                });
            }
            if (this.exportConfig.print) {
                buttons.push({
                    extend: 'print',
                    text: '<i class="fas fa-print"></i> Imprimir',
                    className: 'btn btn-info btn-sm',
                    exportOptions: {
                        columns: ':not(.no-export)'
                    }
                });
            }
        }

        // Inicializa DataTable
        this.dataTable = $(this.tableSelector).DataTable({
            processing: true,
            serverSide: true,
            responsive: true,
            ajax: {
                url: `/${this.controllerName}/List`,
                type: 'POST',
                contentType: 'application/json',
                headers: {
                    'RequestVerificationToken': token
                },
                data: function (d) {
                    return JSON.stringify(d);
                },
                dataSrc: function (json) {
                    return json.data || [];
                },
                error: function (xhr, error, thrown) {
                    console.error('Erro ao carregar dados:', error, thrown);
                    self.showError('Erro ao carregar dados. Verifique sua conexão.');
                }
            },
            columns: this.columns,
            order: [[1, 'asc']],
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            dom: '<"row"<"col-md-6"l><"col-md-6"<"d-flex justify-content-end"B>>>rt<"row"<"col-md-6"i><"col-md-6"p>>',
            buttons: buttons,
            language: {
                processing: '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Carregando...</span></div>',
                lengthMenu: 'Mostrar _MENU_ registros',
                zeroRecords: 'Nenhum registro encontrado',
                info: 'Mostrando de _START_ até _END_ de _TOTAL_ registros',
                infoEmpty: 'Mostrando 0 até 0 de 0 registros',
                infoFiltered: '(filtrado de _MAX_ registros no total)',
                search: 'Pesquisar:',
                paginate: {
                    first: 'Primeiro',
                    last: 'Último',
                    next: 'Próximo',
                    previous: 'Anterior'
                },
                select: {
                    rows: {
                        _: 'Selecionado %d linhas',
                        0: '',
                        1: 'Selecionado 1 linha'
                    }
                }
            },
            drawCallback: function () {
                // Reinicializa tooltips após cada draw
                $('[data-bs-toggle="tooltip"]').tooltip();
                self.updateSelectedCount();
            }
        });

        // Evento de seleção de linha
        $(this.tableSelector).on('change', '.dt-checkboxes', function () {
            self.updateSelectedCount();
        });
    }

    /**
     * ⭐ MÉTODO AUXILIAR: Obtém ID do botão com trim e validação
     * @param {jQuery} $button - Elemento jQuery do botão
     * @returns {string|null} ID limpo ou null se inválido
     */
    getIdFromButton($button) {
        // Tenta obter via data() primeiro (cache do jQuery)
        let id = $button.data('id');

        // Se não encontrou ou é undefined, tenta via attr()
        if (id === undefined || id === null) {
            id = $button.attr('data-id');
        }

        // Converte para string e faz trim
        if (id !== undefined && id !== null) {
            id = String(id).trim();
        }

        // Log para debug
        console.log('🔍 [CRUD-BASE] ID capturado:', {
            dataId: $button.data('id'),
            attrId: $button.attr('data-id'),
            finalId: id,
            isEmpty: !id || id === ''
        });

        // Retorna null se vazio
        if (!id || id === '') {
            console.warn('⚠️ [CRUD-BASE] ID vazio ou inválido!');
            return null;
        }

        return id;
    }

    /**
     * Vincula eventos aos elementos da página.
     */
    bindEvents() {
        const self = this;

        // Botão Novo
        $('#btnCreate').on('click', function () {
            self.openCreateModal();
        });

        // Botão Atualizar
        $('#btnRefresh').on('click', function () {
            self.refresh();
        });

        // Botão Excluir Selecionados
        $('#btnDeleteSelected').on('click', function () {
            self.deleteSelected();
        });

        // Busca
        $('#searchBox').on('keyup', function () {
            self.dataTable.search($(this).val()).draw();
        });

        // ⭐ CORREÇÃO: Botões de ação na tabela com validação de ID
        $(this.tableSelector).on('click', '.btn-view', function () {
            const id = self.getIdFromButton($(this));
            if (id) {
                self.view(id);
            } else {
                self.showError('Não foi possível identificar o registro. Atualize a página.');
            }
        });

        $(this.tableSelector).on('click', '.btn-edit', function () {
            const id = self.getIdFromButton($(this));
            if (id) {
                self.edit(id);
            } else {
                self.showError('Não foi possível identificar o registro. Atualize a página.');
            }
        });

        $(this.tableSelector).on('click', '.btn-delete', function () {
            const id = self.getIdFromButton($(this));
            if (id) {
                self.delete(id);
            } else {
                self.showError('Não foi possível identificar o registro. Atualize a página.');
            }
        });

        // Submit do formulário
        $(this.formSelector).on('submit', function (e) {
            e.preventDefault();
            if ($(this).valid()) {
                self.save();
            }
        });

        // Selecionar/Deselecionar todos
        $(this.tableSelector).on('click', 'thead .dt-checkboxes', function () {
            const checked = $(this).prop('checked');
            $(self.tableSelector + ' tbody .dt-checkboxes').prop('checked', checked);
            self.updateSelectedCount();
        });
    }

    /**
     * Inicializa validação do formulário.
     */
    initValidation() {
        $(this.formSelector).validate({
            errorClass: 'is-invalid',
            validClass: 'is-valid',
            errorElement: 'div',
            errorPlacement: function (error, element) {
                error.addClass('invalid-feedback');
                element.closest('.mb-3').append(error);
            },
            highlight: function (element) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function (element) {
                $(element).removeClass('is-invalid').addClass('is-valid');
            }
        });
    }

    /**
     * Abre modal para criar novo registro.
     */
    openCreateModal() {
        this.isEditMode = false;
        this.currentId = null;
        this.clearForm();
        this.enablePrimaryKeyFields(true);
        $('#modalTitle').text(`Novo ${this.entityName}`);
        $(this.modalSelector).modal('show');
    }

    /**
     * Abre modal para editar registro existente.
     * @param {string|number} id - ID do registro
     */
    async edit(id) {
        const self = this;

        // ⭐ VALIDAÇÃO: ID não pode ser vazio
        if (!id || String(id).trim() === '') {
            this.showError('ID do registro não foi informado.');
            return;
        }

        this.isEditMode = true;
        this.currentId = String(id).trim();

        try {
            this.showLoading();

            const response = await $.ajax({
                url: `/${this.controllerName}/GetById`,
                type: 'GET',
                data: { id: this.currentId }
            });

            this.hideLoading();

            if (response.success && response.data) {
                this.clearForm();
                this.populateForm(response.data);
                this.enablePrimaryKeyFields(false);
                $('#modalTitle').text(`Editar ${this.entityName}`);
                $(this.modalSelector).modal('show');
            } else {
                this.showError(response.message || 'Erro ao carregar registro.');
            }
        } catch (error) {
            this.hideLoading();
            console.error('Erro ao carregar registro:', error);
            this.showError('Erro ao carregar registro. Verifique sua conexão.');
        }
    }

    /**
     * Visualiza um registro (somente leitura).
     * @param {string|number} id - ID do registro
     */
    async view(id) {
        // ⭐ VALIDAÇÃO: ID não pode ser vazio
        if (!id || String(id).trim() === '') {
            this.showError('ID do registro não foi informado.');
            return;
        }

        const cleanId = String(id).trim();

        try {
            this.showLoading();

            const response = await $.ajax({
                url: `/${this.controllerName}/GetById`,
                type: 'GET',
                data: { id: cleanId }
            });

            this.hideLoading();

            if (response.success && response.data) {
                this.showViewModal(response.data);
            } else {
                this.showError(response.message || 'Erro ao carregar registro.');
            }
        } catch (error) {
            this.hideLoading();
            console.error('Erro ao visualizar registro:', error);
            this.showError('Erro ao carregar registro. Verifique sua conexão.');
        }
    }

    /**
     * Exibe modal de visualização.
     * @param {Object} data - Dados do registro
     */
    showViewModal(data) {
        let html = '<div class="table-responsive"><table class="table table-bordered">';

        for (const [key, value] of Object.entries(data)) {
            // Ignora campos técnicos
            if (key.startsWith('_') || key === 'id') continue;

            let displayValue = value;

            // Formata valores booleanos
            if (typeof value === 'boolean') {
                displayValue = value
                    ? '<span class="badge bg-success">Sim</span>'
                    : '<span class="badge bg-danger">Não</span>';
            }
            // Formata datas
            else if (value && typeof value === 'string' && value.match(/^\d{4}-\d{2}-\d{2}/)) {
                const date = new Date(value);
                displayValue = date.toLocaleDateString('pt-BR');
            }
            // Formata null/undefined
            else if (value === null || value === undefined) {
                displayValue = '<span class="text-muted">-</span>';
            }

            html += `<tr><th style="width: 30%">${this.formatFieldName(key)}</th><td>${displayValue}</td></tr>`;
        }

        html += '</table></div>';

        $('#viewContent').html(html);
        $('#modalView').modal('show');
    }

    /**
     * Formata nome do campo para exibição.
     * @param {string} fieldName - Nome do campo
     * @returns {string} Nome formatado
     */
    formatFieldName(fieldName) {
        // Remove prefixos comuns
        let name = fieldName.replace(/^(cd|dc|dt|nr|fl|id)/i, '');

        // Adiciona espaços antes de maiúsculas
        name = name.replace(/([A-Z])/g, ' $1').trim();

        // Capitaliza primeira letra
        return name.charAt(0).toUpperCase() + name.slice(1);
    }

    /**
     * Salva o registro (cria ou atualiza).
     */
    async save() {
        const self = this;
        const token = $('input[name="__RequestVerificationToken"]').val();

        // Coleta dados do formulário
        const formData = this.getFormData();

        // Hook para customização antes de enviar
        const processedData = this.beforeSubmit(formData, this.isEditMode);
        if (processedData === false) return; // Cancela se retornar false

        const dataToSend = processedData || formData;

        try {
            this.showLoading();

            const url = this.isEditMode
                ? `/${this.controllerName}/Edit?id=${encodeURIComponent(this.currentId)}`
                : `/${this.controllerName}/Create`;

            const response = await $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json',
                headers: {
                    'RequestVerificationToken': token
                },
                data: JSON.stringify(dataToSend)
            });

            this.hideLoading();

            if (response.success) {
                $(this.modalSelector).modal('hide');
                this.showSuccess(response.message || `${this.entityName} salvo com sucesso!`);
                this.refresh();

                // Hook para customização após salvar
                this.afterSubmit(response.data, this.isEditMode);
            } else {
                this.showError(response.message || 'Erro ao salvar registro.');

                // Exibe erros de validação se houver
                if (response.errors) {
                    this.showValidationErrors(response.errors);
                }
            }
        } catch (error) {
            this.hideLoading();
            console.error('Erro ao salvar:', error);
            this.showError('Erro ao salvar registro. Verifique sua conexão.');
        }
    }

    /**
     * Exclui um registro.
     * @param {string|number} id - ID do registro
     */
    async delete(id) {
        // ⭐ VALIDAÇÃO: ID não pode ser vazio
        if (!id || String(id).trim() === '') {
            this.showError('ID do registro não foi informado.');
            return;
        }

        const cleanId = String(id).trim();
        const self = this;
        const token = $('input[name="__RequestVerificationToken"]').val();

        const result = await Swal.fire({
            title: 'Confirmar Exclusão',
            text: `Deseja realmente excluir este ${this.entityName.toLowerCase()}?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: '<i class="fas fa-trash"></i> Sim, excluir',
            cancelButtonText: '<i class="fas fa-times"></i> Cancelar'
        });

        if (result.isConfirmed) {
            try {
                this.showLoading();

                // ✅ Envia ID como query parameter com trim
                const response = await $.ajax({
                    url: `/${this.controllerName}/Delete?id=${encodeURIComponent(cleanId)}`,
                    type: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });

                this.hideLoading();

                if (response.success) {
                    this.showSuccess(response.message || `${this.entityName} excluído com sucesso!`);
                    this.refresh();
                } else {
                    this.showError(response.message || 'Erro ao excluir registro.');
                }
            } catch (error) {
                this.hideLoading();
                console.error('Erro ao excluir:', error);
                this.showError('Erro ao excluir registro. Verifique sua conexão.');
            }
        }
    }

    /**
     * Exclui registros selecionados.
     * Usa endpoint /DeleteMultiple do Web controller que chama /batch da API.
     */
    async deleteSelected() {
        const ids = this.getSelectedIds();

        if (ids.length === 0) {
            this.showWarning('Selecione pelo menos um registro para excluir.');
            return;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();

        const result = await Swal.fire({
            title: 'Confirmar Exclusão em Lote',
            text: `Deseja realmente excluir ${ids.length} ${ids.length === 1 ? this.entityName.toLowerCase() : this.entityNamePlural.toLowerCase()}?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: `<i class="fas fa-trash"></i> Sim, excluir ${ids.length}`,
            cancelButtonText: '<i class="fas fa-times"></i> Cancelar'
        });

        if (result.isConfirmed) {
            try {
                this.showLoading();

                const response = await $.ajax({
                    url: `/${this.controllerName}/DeleteMultiple`,
                    type: 'POST',
                    contentType: 'application/json',
                    headers: {
                        'RequestVerificationToken': token
                    },
                    data: JSON.stringify(ids)
                });

                this.hideLoading();

                if (response.success) {
                    // Verifica se há detalhes de exclusão parcial
                    if (response.data && response.data.failureCount > 0) {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Exclusão Parcial',
                            html: `<p>Excluídos: <strong>${response.data.successCount}</strong></p>
                                   <p>Falhas: <strong>${response.data.failureCount}</strong></p>`
                        });
                    } else {
                        const count = response.data?.successCount || ids.length;
                        this.showSuccess(`${count} registro(s) excluído(s) com sucesso!`);
                    }
                    this.refresh();
                } else {
                    this.showError(response.message || 'Erro ao excluir registros.');
                }

            } catch (error) {
                this.hideLoading();
                console.error('Erro ao excluir múltiplos:', error);
                this.showError('Erro ao excluir registros. Verifique sua conexão.');
            }
        }
    }

    /**
     * Atualiza a tabela.
     */
    refresh() {
        this.dataTable.ajax.reload(null, false);
    }

    /**
     * Limpa o formulário.
     */
    clearForm() {
        $(this.formSelector)[0].reset();
        $(this.formSelector).find('.is-invalid').removeClass('is-invalid');
        $(this.formSelector).find('.is-valid').removeClass('is-valid');
        $(this.formSelector).find('.invalid-feedback').remove();
        $('#Id').val('');
    }

    /**
     * Coleta dados do formulário.
     * @returns {Object} Dados do formulário
     */
    getFormData() {
        const formData = {};
        const form = $(this.formSelector);

        // Inputs de texto, hidden, etc.
        form.find('input:not([type="checkbox"]):not([type="radio"]), textarea, select').each(function () {
            const name = $(this).attr('name');
            if (name) {
                formData[name] = $(this).val();
            }
        });

        // Checkboxes
        form.find('input[type="checkbox"]').each(function () {
            const name = $(this).attr('name');
            if (name && name !== '__RequestVerificationToken') {
                formData[name] = $(this).is(':checked');
            }
        });

        // Radio buttons
        form.find('input[type="radio"]:checked').each(function () {
            const name = $(this).attr('name');
            if (name) {
                formData[name] = $(this).val();
            }
        });

        return formData;
    }

    /**
     * =========================================================================
     * MÉTODO CORRIGIDO - Popula formulário com dados (case-insensitive)
     * =========================================================================
     * @param {Object} data - Dados para preencher o formulário
     */
    populateForm(data) {
        const form = $(this.formSelector);

        // Cria um mapa de nomes de campos em lowercase para os nomes originais
        const fieldMap = {};
        for (const key in data) {
            fieldMap[key.toLowerCase()] = key;
        }

        // Preenche cada campo do formulário
        form.find('input, textarea, select').each(function () {
            const $field = $(this);
            const fieldName = $field.attr('name') || $field.attr('id');

            if (!fieldName || fieldName === '__RequestVerificationToken') return;

            // Busca o valor usando case-insensitive matching
            const lowerFieldName = fieldName.toLowerCase();
            const actualKey = fieldMap[lowerFieldName];

            if (actualKey && data[actualKey] !== undefined) {
                const value = data[actualKey];

                if ($field.is(':checkbox')) {
                    // Checkbox - define checked baseado no valor booleano
                    $field.prop('checked', value === true || value === 'true' || value === 1);
                } else if ($field.is(':radio')) {
                    // Radio - seleciona o valor correspondente
                    $field.prop('checked', $field.val() === String(value));
                } else if ($field.is('select')) {
                    // Select - define o valor selecionado
                    $field.val(value).trigger('change');
                } else {
                    // Input text, textarea, hidden, etc.
                    // ⭐ CORREÇÃO: Trim em valores string
                    const finalValue = typeof value === 'string' ? value.trim() : value;
                    $field.val(finalValue);
                }
            }
        });

        // Log para debug
        console.log('📝 Formulário populado com dados:', data);
    }

    /**
     * Obtém IDs dos registros selecionados.
     * @returns {Array} Array de IDs selecionados
     */
    getSelectedIds() {
        const ids = [];
        const self = this;

        $(this.tableSelector + ' tbody .dt-checkboxes:checked').each(function () {
            const row = $(this).closest('tr');
            const rowData = self.dataTable.row(row).data();
            if (rowData) {
                // Busca o ID usando case-insensitive
                const idField = self.idField.toLowerCase();
                for (const key in rowData) {
                    if (key.toLowerCase() === idField) {
                        // ⭐ CORREÇÃO: Trim no ID
                        const id = typeof rowData[key] === 'string'
                            ? rowData[key].trim()
                            : rowData[key];
                        if (id && String(id).trim() !== '') {
                            ids.push(id);
                        }
                        break;
                    }
                }
            }
        });

        console.log('✅ [CRUD-BASE] IDs selecionados:', ids);
        return ids;
    }

    /**
     * Atualiza contador de selecionados.
     */
    updateSelectedCount() {
        const count = $(this.tableSelector + ' tbody .dt-checkboxes:checked').length;
        $('#selectedCount').text(count);
        $('#btnDeleteSelected').prop('disabled', count === 0);

        // Atualiza info de seleção
        const infoText = count > 0 ? `Selecionado ${count} linha${count > 1 ? 's' : ''}` : '';
        $(this.tableSelector).closest('.dataTables_wrapper').find('.dataTables_info').append(
            count > 0 ? ` | <strong>${infoText}</strong>` : ''
        );
    }

    /**
     * Exibe erros de validação nos campos.
     * @param {Object} errors - Objeto com erros por campo
     */
    showValidationErrors(errors) {
        for (const [field, messages] of Object.entries(errors)) {
            // Busca o campo usando case-insensitive
            let $field = $(`#${field}`);

            // Se não encontrou, tenta com lowercase
            if ($field.length === 0) {
                $field = $(`#${field.charAt(0).toUpperCase() + field.slice(1)}`);
            }
            if ($field.length === 0) {
                $field = $(`#${field.toLowerCase()}`);
            }

            if ($field.length > 0) {
                $field.addClass('is-invalid');
                const errorHtml = `<div class="invalid-feedback">${messages.join('<br>')}</div>`;
                $field.closest('.mb-3').find('.invalid-feedback').remove();
                $field.closest('.mb-3').append(errorHtml);
            }
        }
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * Deve ser sobrescrito nas classes filhas.
     * @param {boolean} enable - true para habilitar, false para desabilitar
     */
    enablePrimaryKeyFields(enable) {
        // Implementação padrão - sobrescrever nas classes filhas
        $('#Id').prop('readonly', !enable);
    }

    /**
     * Hook executado antes de submeter o formulário.
     * Pode ser sobrescrito nas classes filhas.
     * @param {Object} formData - Dados do formulário
     * @param {boolean} isEdit - true se está editando
     * @returns {Object|false} Dados processados ou false para cancelar
     */
    beforeSubmit(formData, isEdit) {
        return formData;
    }

    /**
     * Hook executado após submeter o formulário com sucesso.
     * Pode ser sobrescrito nas classes filhas.
     * @param {Object} data - Dados retornados pela API
     * @param {boolean} isEdit - true se estava editando
     */
    afterSubmit(data, isEdit) {
        // Implementação padrão vazia
    }

    // =========================================================================
    // MÉTODOS DE FEEDBACK (SweetAlert2)
    // =========================================================================

    showSuccess(message) {
        Swal.fire({
            icon: 'success',
            title: 'Sucesso!',
            text: message,
            timer: 2000,
            showConfirmButton: false
        });
    }

    showError(message) {
        Swal.fire({
            icon: 'error',
            title: 'Erro!',
            text: message
        });
    }

    showWarning(message) {
        Swal.fire({
            icon: 'warning',
            title: 'Atenção!',
            text: message
        });
    }

    showInfo(message) {
        Swal.fire({
            icon: 'info',
            title: 'Informação',
            text: message
        });
    }

    showLoading() {
        Swal.fire({
            title: 'Processando...',
            html: 'Por favor, aguarde.',
            allowOutsideClick: false,
            allowEscapeKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    }

    hideLoading() {
        Swal.close();
    }
}

// Exporta para uso global
window.CrudBase = CrudBase;