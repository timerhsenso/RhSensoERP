// =============================================================================
// RHSENSOERP.WEB - GENERIC CRUD JAVASCRIPT
// =============================================================================
// Arquivo: wwwroot/js/generic-crud.js
// Descrição: JavaScript genérico para CRUD dinâmico baseado em metadados
// Versão: 2.0 - Corrigido para ambiente com autenticação Cookie
// =============================================================================

/**
 * Módulo GenericCrud - Gerencia DataTable e Forms dinâmicos baseados em metadados.
 */
const GenericCrud = (function () {
    'use strict';

    // =========================================================================
    // CONFIGURAÇÕES PRIVADAS
    // =========================================================================
    let _metadata = null;
    let _dataTable = null;
    let _apiBaseUrl = '';
    let _options = {
        tableSelector: '#dataTable',
        formSelector: '#entityForm',
        modalSelector: '#entityModal',
        deleteModalSelector: '#deleteModal',
        onDataLoaded: null,
        onSaveSuccess: null,
        onDeleteSuccess: null,
        onError: null
    };

    // =========================================================================
    // FUNÇÕES PÚBLICAS
    // =========================================================================

    /**
     * Inicializa o módulo GenericCrud.
     * @param {string} entityName - Nome da entidade (ex: "Banco", "Sistema")
     * @param {object} options - Opções de configuração
     */
    async function init(entityName, options) {
        options = options || {};
        _options = Object.assign({}, _options, options);

        try {
            console.log('[GenericCrud] Iniciando para entidade: ' + entityName);

            // Carrega metadados via proxy do Web (usando fetch com credentials)
            _metadata = await loadMetadata(entityName);

            if (!_metadata) {
                console.error('[GenericCrud] Metadados não encontrados para: ' + entityName);
                return false;
            }

            // Define URL base da API (usa o que vier nos metadados)
            _apiBaseUrl = _metadata.endpoints ? _metadata.endpoints.baseUrl : '';

            console.log('[GenericCrud] Metadados carregados:', _metadata.displayName);
            console.log('[GenericCrud] Endpoint base:', _apiBaseUrl);
            console.log('[GenericCrud] Propriedades:', _metadata.properties ? _metadata.properties.length : 0);

            return true;
        } catch (error) {
            console.error('[GenericCrud] Erro ao inicializar:', error);
            if (typeof _options.onError === 'function') {
                _options.onError(error);
            }
            return false;
        }
    }

    /**
     * Carrega metadados da API (via proxy do MetadataService no Web).
     * @param {string} entityName - Nome da entidade
     * @returns {object} Metadados da entidade
     */
    async function loadMetadata(entityName) {
        try {
            // Chama o endpoint do Web que faz proxy para a API
            var url = '/Metadata/GetEntityMetadata?entityName=' + encodeURIComponent(entityName);
            console.log('[GenericCrud] Buscando metadados de: ' + url);

            var response = await fetch(url, {
                method: 'GET',
                credentials: 'same-origin', // Envia cookies de autenticação
                headers: {
                    'Accept': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error('HTTP ' + response.status + ': ' + response.statusText);
            }

            var data = await response.json();
            console.log('[GenericCrud] Metadados recebidos:', data);
            return data;
        } catch (error) {
            console.error('[GenericCrud] Erro ao carregar metadados:', error);
            return null;
        }
    }

    /**
     * Retorna os metadados carregados.
     * @returns {object} Metadados da entidade
     */
    function getMetadata() {
        return _metadata;
    }

    // =========================================================================
    // DATATABLE
    // =========================================================================

    /**
     * Inicializa o DataTable com base nos metadados.
     * @param {object} customOptions - Opções customizadas do DataTable
     */
    function initDataTable(customOptions) {
        customOptions = customOptions || {};

        if (!_metadata) {
            console.error('[GenericCrud] Metadados não carregados. Chame init() primeiro.');
            return null;
        }

        console.log('[GenericCrud] Inicializando DataTable...');

        // Gera colunas dinamicamente
        var columns = generateColumns();
        console.log('[GenericCrud] Colunas geradas:', columns.length);

        // URL para buscar dados (via proxy do Web)
        var dataUrl = '/Metadata/GetEntityData?entityName=' + encodeURIComponent(_metadata.entityName);

        // Configurações padrão do DataTable
        var defaultOptions = {
            processing: true,
            serverSide: false,
            ajax: {
                url: dataUrl,
                type: 'GET',
                dataSrc: function (json) {
                    console.log('[GenericCrud] Dados recebidos:', json);
                    // Suporta tanto { data: [...] } quanto [...] diretamente
                    if (json && json.data) {
                        return json.data;
                    }
                    if (Array.isArray(json)) {
                        return json;
                    }
                    return [];
                },
                error: function (xhr, error, thrown) {
                    console.error('[GenericCrud] Erro ao carregar dados:', error, thrown);
                    console.error('[GenericCrud] XHR:', xhr);
                    handleError(xhr, { message: 'Erro ao carregar dados: ' + (thrown || error) });
                }
            },
            columns: columns,
            order: getDefaultOrder(),
            pageLength: (_metadata.uiConfig && _metadata.uiConfig.pageSize) || 10,
            lengthMenu: getLengthMenu(),
            language: getDataTableLanguage(),
            responsive: true,
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                '<"row"<"col-sm-12"tr>>' +
                '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            drawCallback: function () {
                console.log('[GenericCrud] DataTable drawCallback');
                if (typeof _options.onDataLoaded === 'function') {
                    _options.onDataLoaded(_dataTable);
                }
            },
            initComplete: function () {
                console.log('[GenericCrud] DataTable inicializado com sucesso');
            }
        };

        // Mescla opções customizadas
        var finalOptions = Object.assign({}, defaultOptions, customOptions);

        // Destrói DataTable existente se houver
        if (_dataTable) {
            _dataTable.destroy();
        }

        // Inicializa DataTable
        _dataTable = $(_options.tableSelector).DataTable(finalOptions);

        return _dataTable;
    }

    /**
     * Retorna a ordem padrão para o DataTable.
     */
    function getDefaultOrder() {
        if (!_metadata || !_metadata.uiConfig) {
            return [[0, 'asc']];
        }

        var sortField = _metadata.uiConfig.defaultSortField || '';
        var sortDir = (_metadata.uiConfig.defaultSortDirection || 'asc').toLowerCase();

        // Encontra o índice da coluna
        var colIndex = 0;
        if (sortField && _metadata.properties) {
            var listProps = _metadata.properties
                .filter(function (p) { return p.list && p.list.show; })
                .sort(function (a, b) { return (a.list.order || 0) - (b.list.order || 0); });

            for (var i = 0; i < listProps.length; i++) {
                if (listProps[i].name.toLowerCase() === sortField.toLowerCase()) {
                    colIndex = i;
                    break;
                }
            }
        }

        return [[colIndex, sortDir]];
    }

    /**
     * Retorna o menu de tamanho de página.
     */
    function getLengthMenu() {
        if (_metadata && _metadata.uiConfig && _metadata.uiConfig.pageSizeOptions) {
            return _metadata.uiConfig.pageSizeOptions.map(function (n) { return [n, n]; });
        }
        return [[10, 25, 50, 100], [10, 25, 50, 100]];
    }

    /**
     * Gera as colunas do DataTable baseado nos metadados.
     */
    function generateColumns() {
        var columns = [];

        if (!_metadata || !_metadata.properties) {
            console.warn('[GenericCrud] Sem propriedades nos metadados');
            return columns;
        }

        // Filtra propriedades que devem aparecer na lista
        var listProperties = _metadata.properties
            .filter(function (p) { return p.list && p.list.show; })
            .sort(function (a, b) { return (a.list.order || 0) - (b.list.order || 0); });

        console.log('[GenericCrud] Propriedades para lista:', listProperties.length);

        listProperties.forEach(function (prop) {
            var column = {
                data: toCamelCase(prop.name),
                name: prop.name,
                title: prop.displayName || prop.name,
                orderable: prop.list.sortable !== false,
                searchable: prop.list.filterable !== false,
                className: prop.list.cssClass || '',
                render: getColumnRenderer(prop)
            };

            if (prop.list.width) {
                column.width = prop.list.width;
            }

            columns.push(column);
        });

        // Coluna de ações
        var actions = _metadata.actions || {};
        if (actions.canEdit || actions.canDelete || actions.canView) {
            columns.push({
                data: null,
                name: 'actions',
                title: 'Ações',
                orderable: false,
                searchable: false,
                className: 'text-center',
                width: '120px',
                render: renderActionsColumn
            });
        }

        return columns;
    }

    /**
     * Retorna o renderer apropriado para o tipo de coluna.
     */
    function getColumnRenderer(prop) {
        var format = (prop.list && prop.list.format) || '';
        var align = (prop.list && prop.list.align) || 'left';

        return function (data, type, row) {
            if (type !== 'display') return data;

            if (data === null || data === undefined || data === '') {
                return '<span class="text-muted">-</span>';
            }

            var rendered = data;

            switch (format.toLowerCase()) {
                case 'boolean':
                    var boolVal = data === true || data === 'true' || data === 1 || data === '1' || data === 'S' || data === 's';
                    rendered = boolVal
                        ? '<span class="badge bg-success"><i class="fas fa-check"></i> Sim</span>'
                        : '<span class="badge bg-secondary"><i class="fas fa-times"></i> Não</span>';
                    break;
                case 'date':
                    rendered = formatDate(data);
                    break;
                case 'datetime':
                    rendered = formatDateTime(data);
                    break;
                case 'currency':
                    rendered = formatCurrency(data);
                    break;
                case 'percent':
                    rendered = formatPercent(data);
                    break;
                default:
                    rendered = escapeHtml(String(data));
            }

            if (align === 'center') {
                rendered = '<div class="text-center">' + rendered + '</div>';
            } else if (align === 'right') {
                rendered = '<div class="text-end">' + rendered + '</div>';
            }

            return rendered;
        };
    }

    /**
     * Renderiza a coluna de ações.
     */
    function renderActionsColumn(data, type, row) {
        if (type !== 'display') return null;

        var actions = _metadata.actions || {};
        var pk = _metadata.primaryKey;
        var pkName = pk ? toCamelCase(pk.propertyName) : 'id';
        var id = row[pkName];

        var html = '<div class="btn-group btn-group-sm" role="group">';

        if (actions.canView) {
            html += '<button type="button" class="btn btn-info btn-view" data-id="' + escapeHtml(id) + '" title="Visualizar">' +
                '<i class="fas fa-eye"></i></button>';
        }

        if (actions.canEdit) {
            html += '<button type="button" class="btn btn-warning btn-edit" data-id="' + escapeHtml(id) + '" title="Editar">' +
                '<i class="fas fa-edit"></i></button>';
        }

        if (actions.canDelete) {
            html += '<button type="button" class="btn btn-danger btn-delete" data-id="' + escapeHtml(id) + '" title="Excluir">' +
                '<i class="fas fa-trash"></i></button>';
        }

        html += '</div>';
        return html;
    }

    /**
     * Recarrega os dados do DataTable.
     */
    function reloadData() {
        if (_dataTable) {
            _dataTable.ajax.reload(null, false);
        }
    }

    // =========================================================================
    // FORMULÁRIO
    // =========================================================================

    /**
     * Gera o HTML do formulário baseado nos metadados.
     * @param {string} mode - Modo: 'create', 'edit', 'view'
     */
    function generateFormHtml(mode) {
        mode = mode || 'create';

        if (!_metadata || !_metadata.properties) {
            return '<p class="text-danger">Metadados não carregados.</p>';
        }

        // Filtra propriedades que devem aparecer no formulário
        var formProperties = _metadata.properties.filter(function (p) {
            if (!p.form || !p.form.show) return false;
            if (mode === 'create' && p.form.showOnCreate === false) return false;
            if (mode === 'edit' && p.form.showOnEdit === false) return false;
            return true;
        });

        // Agrupa por grupo
        var groups = groupBy(formProperties, function (p) {
            return (p.form && p.form.group) || '_default';
        });

        var html = '<form id="entityForm" novalidate>';

        // Para cada grupo
        Object.keys(groups).forEach(function (groupName) {
            var groupProps = groups[groupName].sort(function (a, b) {
                return (a.form.order || 0) - (b.form.order || 0);
            });

            if (groupName !== '_default') {
                html += '<h6 class="border-bottom pb-2 mb-3">' + escapeHtml(groupName) + '</h6>';
            }

            html += '<div class="row">';

            groupProps.forEach(function (prop) {
                html += generateFieldHtml(prop, mode);
            });

            html += '</div>';
        });

        html += '</form>';
        return html;
    }

    /**
     * Gera o HTML de um campo de formulário.
     */
    function generateFieldHtml(prop, mode) {
        var form = prop.form || {};
        var colSize = form.colSize || 6;
        var inputType = (form.inputType || 'text').toLowerCase();
        var fieldName = toCamelCase(prop.name);
        var isReadonly = mode === 'view' || form.readonly === true;
        var isRequired = prop.isRequired && mode !== 'view';
        var pk = _metadata.primaryKey;
        var isPrimaryKey = pk && prop.name === pk.propertyName;

        var html = '<div class="col-md-' + colSize + ' mb-3">';
        html += '<label for="' + fieldName + '" class="form-label">';
        html += escapeHtml(prop.displayName || prop.name);
        if (isRequired) {
            html += ' <span class="text-danger">*</span>';
        }
        html += '</label>';

        // Wrapper para input com ícone
        if (form.icon) {
            html += '<div class="input-group">';
            html += '<span class="input-group-text"><i class="' + escapeHtml(form.icon) + '"></i></span>';
        }

        var commonAttrs = 'id="' + fieldName + '" name="' + fieldName + '" class="form-control"';
        if (isReadonly) commonAttrs += ' readonly disabled';
        if (isRequired) commonAttrs += ' required';
        if (prop.maxLength) commonAttrs += ' maxlength="' + prop.maxLength + '"';
        if (form.placeholder) commonAttrs += ' placeholder="' + escapeHtml(form.placeholder) + '"';

        switch (inputType) {
            case 'textarea':
                html += '<textarea ' + commonAttrs + ' rows="3"></textarea>';
                break;
            case 'checkbox':
                html = '<div class="col-md-' + colSize + ' mb-3">';
                html += '<div class="form-check">';
                html += '<input type="checkbox" class="form-check-input" id="' + fieldName + '" name="' + fieldName + '"';
                if (isReadonly) html += ' disabled';
                html += '>';
                html += '<label class="form-check-label" for="' + fieldName + '">' + escapeHtml(prop.displayName || prop.name) + '</label>';
                html += '</div></div>';
                return html;
            case 'select':
                html += '<select ' + commonAttrs + '>';
                html += '<option value="">Selecione...</option>';
                if (form.options) {
                    form.options.forEach(function (opt) {
                        html += '<option value="' + escapeHtml(opt.value) + '">' + escapeHtml(opt.text) + '</option>';
                    });
                }
                html += '</select>';
                break;
            case 'date':
                html += '<input type="date" ' + commonAttrs + '>';
                break;
            case 'datetime':
                html += '<input type="datetime-local" ' + commonAttrs + '>';
                break;
            case 'number':
                html += '<input type="number" ' + commonAttrs + '>';
                break;
            case 'email':
                html += '<input type="email" ' + commonAttrs + '>';
                break;
            case 'tel':
                html += '<input type="tel" ' + commonAttrs + '>';
                break;
            case 'password':
                html += '<input type="password" ' + commonAttrs + ' autocomplete="new-password">';
                break;
            default:
                html += '<input type="text" ' + commonAttrs + '>';
        }

        if (form.icon) {
            html += '</div>';
        }

        if (form.helpText) {
            html += '<div class="form-text">' + escapeHtml(form.helpText) + '</div>';
        }

        html += '</div>';
        return html;
    }

    /**
     * Popula o formulário com dados.
     */
    function populateForm(data) {
        if (!data || !_metadata) return;

        _metadata.properties.forEach(function (prop) {
            var fieldName = toCamelCase(prop.name);
            var element = document.getElementById(fieldName);
            if (!element) return;

            var value = data[fieldName];
            if (value === null || value === undefined) {
                value = '';
            }

            var inputType = (prop.form && prop.form.inputType || '').toLowerCase();

            if (element.type === 'checkbox') {
                element.checked = value === true || value === 'true' || value === 1 || value === '1' || value === 'S';
            } else if (inputType === 'date' && value) {
                element.value = formatDateForInput(value);
            } else if (inputType === 'datetime' && value) {
                element.value = formatDateTimeForInput(value);
            } else {
                element.value = value;
            }
        });
    }

    /**
     * Obtém os dados do formulário.
     */
    function getFormData() {
        var data = {};

        if (!_metadata) return data;

        _metadata.properties.forEach(function (prop) {
            var fieldName = toCamelCase(prop.name);
            var element = document.getElementById(fieldName);
            if (!element) return;

            if (element.type === 'checkbox') {
                data[fieldName] = element.checked;
            } else {
                var value = element.value;

                // Converte tipos
                if (prop.clrType === 'Int32' || prop.clrType === 'Int64') {
                    data[fieldName] = value ? parseInt(value, 10) : null;
                } else if (prop.clrType === 'Decimal' || prop.clrType === 'Double') {
                    data[fieldName] = value ? parseFloat(value) : null;
                } else if (prop.clrType === 'Boolean') {
                    data[fieldName] = value === 'true' || value === '1';
                } else {
                    data[fieldName] = value || null;
                }
            }
        });

        return data;
    }

    /**
     * Valida o formulário.
     */
    function validateForm() {
        var form = document.getElementById('entityForm');
        if (!form) return false;

        form.classList.add('was-validated');
        return form.checkValidity();
    }

    /**
     * Reseta o formulário.
     */
    function resetForm() {
        var form = document.getElementById('entityForm');
        if (form) {
            form.reset();
            form.classList.remove('was-validated');
        }
    }

    // =========================================================================
    // OPERAÇÕES CRUD (via proxy do Web)
    // =========================================================================

    async function getById(id) {
        try {
            var url = '/Metadata/GetEntityById?entityName=' + encodeURIComponent(_metadata.entityName) +
                '&id=' + encodeURIComponent(id);
            var response = await fetch(url, {
                method: 'GET',
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error('HTTP ' + response.status);
            }

            return await response.json();
        } catch (error) {
            console.error('[GenericCrud] Erro ao buscar por ID:', error);
            handleError(null, error);
            return null;
        }
    }

    async function create(data) {
        try {
            var url = '/Metadata/CreateEntity?entityName=' + encodeURIComponent(_metadata.entityName);
            var response = await fetch(url, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                var errorData = await response.json().catch(function () { return {}; });
                throw new Error(errorData.message || 'HTTP ' + response.status);
            }

            var result = await response.json();

            if (typeof _options.onSaveSuccess === 'function') {
                _options.onSaveSuccess('create', result);
            }

            return result;
        } catch (error) {
            console.error('[GenericCrud] Erro ao criar:', error);
            handleError(null, error);
            throw error;
        }
    }

    async function update(id, data) {
        try {
            var url = '/Metadata/UpdateEntity?entityName=' + encodeURIComponent(_metadata.entityName) +
                '&id=' + encodeURIComponent(id);
            var response = await fetch(url, {
                method: 'PUT',
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                var errorData = await response.json().catch(function () { return {}; });
                throw new Error(errorData.message || 'HTTP ' + response.status);
            }

            var result = await response.json();

            if (typeof _options.onSaveSuccess === 'function') {
                _options.onSaveSuccess('update', result);
            }

            return result;
        } catch (error) {
            console.error('[GenericCrud] Erro ao atualizar:', error);
            handleError(null, error);
            throw error;
        }
    }

    async function remove(id) {
        try {
            var url = '/Metadata/DeleteEntity?entityName=' + encodeURIComponent(_metadata.entityName) +
                '&id=' + encodeURIComponent(id);
            var response = await fetch(url, {
                method: 'DELETE',
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                var errorData = await response.json().catch(function () { return {}; });
                throw new Error(errorData.message || 'HTTP ' + response.status);
            }

            if (typeof _options.onDeleteSuccess === 'function') {
                _options.onDeleteSuccess(id);
            }

            return true;
        } catch (error) {
            console.error('[GenericCrud] Erro ao excluir:', error);
            handleError(null, error);
            throw error;
        }
    }

    // =========================================================================
    // FUNÇÕES AUXILIARES
    // =========================================================================

    function handleError(xhr, error) {
        if (typeof _options.onError === 'function') {
            _options.onError(error);
        } else {
            var message = error && error.message ? error.message : 'Ocorreu um erro ao processar a requisição.';
            if (typeof toastr !== 'undefined') {
                toastr.error(message);
            } else {
                console.error('[GenericCrud] Erro:', message);
            }
        }
    }

    function toCamelCase(str) {
        if (!str) return '';
        return str.charAt(0).toLowerCase() + str.slice(1);
    }

    function escapeHtml(str) {
        if (str === null || str === undefined) return '';
        var div = document.createElement('div');
        div.textContent = String(str);
        return div.innerHTML;
    }

    function groupBy(array, keyFn) {
        return array.reduce(function (result, item) {
            var key = keyFn(item);
            (result[key] = result[key] || []).push(item);
            return result;
        }, {});
    }

    // =========================================================================
    // FORMATADORES
    // =========================================================================

    function formatDate(value) {
        if (!value) return '';
        try {
            var date = new Date(value);
            return date.toLocaleDateString('pt-BR');
        } catch (e) {
            return value;
        }
    }

    function formatDateTime(value) {
        if (!value) return '';
        try {
            var date = new Date(value);
            return date.toLocaleString('pt-BR');
        } catch (e) {
            return value;
        }
    }

    function formatDateForInput(value) {
        if (!value) return '';
        try {
            var date = new Date(value);
            return date.toISOString().split('T')[0];
        } catch (e) {
            return '';
        }
    }

    function formatDateTimeForInput(value) {
        if (!value) return '';
        try {
            var date = new Date(value);
            return date.toISOString().slice(0, 16);
        } catch (e) {
            return '';
        }
    }

    function formatCurrency(value) {
        if (value === null || value === undefined) return '';
        try {
            return new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL'
            }).format(value);
        } catch (e) {
            return value;
        }
    }

    function formatPercent(value) {
        if (value === null || value === undefined) return '';
        try {
            return new Intl.NumberFormat('pt-BR', {
                style: 'percent',
                minimumFractionDigits: 2
            }).format(value / 100);
        } catch (e) {
            return value;
        }
    }

    // =========================================================================
    // CONFIGURAÇÃO DE IDIOMA DO DATATABLE
    // =========================================================================

    function getDataTableLanguage() {
        return {
            processing: '<i class="fas fa-spinner fa-spin"></i> Processando...',
            search: 'Buscar:',
            lengthMenu: 'Mostrar _MENU_ registros',
            info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
            infoEmpty: 'Nenhum registro encontrado',
            infoFiltered: '(filtrado de _MAX_ registros)',
            loadingRecords: 'Carregando...',
            zeroRecords: 'Nenhum registro encontrado',
            emptyTable: 'Nenhum dado disponível na tabela',
            paginate: {
                first: '<i class="fas fa-angle-double-left"></i>',
                previous: '<i class="fas fa-angle-left"></i>',
                next: '<i class="fas fa-angle-right"></i>',
                last: '<i class="fas fa-angle-double-right"></i>'
            },
            aria: {
                sortAscending: ': ordenar coluna ascendente',
                sortDescending: ': ordenar coluna descendente'
            }
        };
    }

    // =========================================================================
    // API PÚBLICA
    // =========================================================================

    return {
        init: init,
        loadMetadata: loadMetadata,
        getMetadata: getMetadata,
        initDataTable: initDataTable,
        reloadData: reloadData,
        generateFormHtml: generateFormHtml,
        populateForm: populateForm,
        getFormData: getFormData,
        validateForm: validateForm,
        resetForm: resetForm,
        getById: getById,
        create: create,
        update: update,
        remove: remove,
        toCamelCase: toCamelCase,
        escapeHtml: escapeHtml,
        formatDate: formatDate,
        formatDateTime: formatDateTime,
        formatCurrency: formatCurrency
    };
})();

// Exporta para uso global
window.GenericCrud = GenericCrud;