/**
 * =============================================================================
 * TABSHEET GENERATOR v2.0 - JAVASCRIPT COMPLETO
 * Gerador de telas Mestre/Detalhe com CRUD completo
 * =============================================================================
 */

const TabSheetV2 = (function () {
    'use strict';

    // =========================================================================
    // ESTADO DA APLICAÇÃO
    // =========================================================================
    const state = {
        currentStep: 1,
        totalSteps: 4,
        masterTable: null,
        masterColumns: [],
        masterForeignKeys: [],
        availableTables: [],
        selectedTabs: [],
        isLoading: false
    };

    // Cache de elementos DOM
    let $modal, $loading, $steps, $stepContents, $btnPrev, $btnNext, $btnPreview, $btnGenerate;

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================
    function init() {
        cacheElements();
        bindEvents();
        console.log('✅ TabSheetV2 inicializado');
    }

    function cacheElements() {
        $modal = $('#modalTabSheet');
        $loading = $('#tsLoading');
        $steps = $('.ts-step');
        $stepContents = $('.ts-step-content');
        $btnPrev = $('#btnPrevStep');
        $btnNext = $('#btnNextStep');
        $btnPreview = $('#btnPreview');
        $btnGenerate = $('#btnGenerate');
    }

    function bindEvents() {
        // Navegação do Wizard
        $btnPrev.on('click', prevStep);
        $btnNext.on('click', nextStep);
        $btnGenerate.on('click', generateCode);
        $btnPreview.on('click', showPreview);

        // Clique nos steps
        $steps.on('click', function () {
            const step = parseInt($(this).data('step'));
            if (step < state.currentStep) {
                goToStep(step);
            }
        });

        // Check all para listagem mestre
        $('#checkAllMasterListagem').on('change', function () {
            const checked = $(this).is(':checked');
            $('#masterListagemBody .listagem-check').prop('checked', checked);
            updateCounts();
        });

        // Check all para formulário mestre
        $('#checkAllMasterForm').on('change', function () {
            const checked = $(this).is(':checked');
            $('#masterFormularioBody .formulario-check').prop('checked', checked);
            updateCounts();
        });

        // Busca de tabelas detalhe
        $('#tsSearchDetail').on('input', function () {
            const term = $(this).val().toLowerCase();
            filterAvailableTables(term);
        });

        // Atualização de contadores
        $modal.on('change', '.listagem-check, .formulario-check', updateCounts);

        // Modal events
        $modal.on('hidden.bs.modal', resetState);
    }

    // =========================================================================
    // ABERTURA DO MODAL
    // =========================================================================
    function openModal(tableName) {
        if (!tableName) {
            showToast('Atenção', 'Selecione uma tabela primeiro', 'warning');
            return;
        }

        // Reset e inicialização
        resetState();
        state.masterTable = tableName;

        // Mostrar modal
        const modal = new bootstrap.Modal($modal[0]);
        modal.show();

        // Carregar dados
        loadMasterTableData(tableName);
    }

    // =========================================================================
    // CARREGAMENTO DE DADOS
    // =========================================================================
    async function loadMasterTableData(tableName) {
        showLoading(true);

        try {
            // Buscar metadados da tabela mestre
            const response = await fetch(`/api/tabsheet/metadata/${encodeURIComponent(tableName)}`);

            if (!response.ok) {
                throw new Error('Erro ao carregar metadados');
            }

            const data = await response.json();

            // Armazenar dados
            state.masterColumns = data.columns || [];
            state.masterForeignKeys = data.foreignKeys || [];
            state.availableTables = data.relatedTables || [];

            // Preencher UI
            populateStep1(tableName, data);
            populateMasterListagem(state.masterColumns);
            populateMasterFormulario(state.masterColumns);
            populateMasterFKs(state.masterForeignKeys);
            populateAvailableTables(state.availableTables);

            showLoading(false);

        } catch (error) {
            console.error('Erro ao carregar dados:', error);
            showLoading(false);

            // Fallback: usar dados mockados para demonstração
            loadMockData(tableName);
        }
    }

    function loadMockData(tableName) {
        // Dados de exemplo para demonstração
        const mockColumns = [
            { name: 'id', type: 'int', isPrimaryKey: true, isIdentity: true, isNullable: false },
            { name: 'codigo', type: 'varchar(20)', isPrimaryKey: false, isIdentity: false, isNullable: false },
            { name: 'nome', type: 'varchar(100)', isPrimaryKey: false, isIdentity: false, isNullable: false },
            { name: 'descricao', type: 'varchar(500)', isPrimaryKey: false, isIdentity: false, isNullable: true },
            { name: 'data_cadastro', type: 'datetime', isPrimaryKey: false, isIdentity: false, isNullable: false },
            { name: 'ativo', type: 'bit', isPrimaryKey: false, isIdentity: false, isNullable: false },
            { name: 'valor', type: 'decimal(18,2)', isPrimaryKey: false, isIdentity: false, isNullable: true },
            { name: 'id_categoria', type: 'int', isPrimaryKey: false, isIdentity: false, isNullable: true, isForeignKey: true }
        ];

        const mockFKs = [
            { column: 'id_categoria', referencedTable: 'categorias', referencedColumn: 'id' }
        ];

        const mockRelated = [
            { tableName: 'itens_' + tableName, fkColumn: 'id_' + tableName, columnCount: 8 },
            { tableName: 'historico_' + tableName, fkColumn: 'id_' + tableName, columnCount: 5 },
            { tableName: 'anexos_' + tableName, fkColumn: 'id_' + tableName, columnCount: 6 }
        ];

        state.masterColumns = mockColumns;
        state.masterForeignKeys = mockFKs;
        state.availableTables = mockRelated;

        populateStep1(tableName, {
            columns: mockColumns,
            primaryKey: 'id',
            schema: 'dbo'
        });
        populateMasterListagem(mockColumns);
        populateMasterFormulario(mockColumns);
        populateMasterFKs(mockFKs);
        populateAvailableTables(mockRelated);
    }

    // =========================================================================
    // POPULAÇÃO DO STEP 1 - IDENTIFICAÇÃO
    // =========================================================================
    function populateStep1(tableName, data) {
        // Gerar ID e título automáticos
        const pascalName = toPascalCase(tableName);
        $('#tsConfigId').val(pascalName + '_TabSheet');
        $('#tsTitle').val('Cadastro de ' + humanize(tableName));

        // Info da tabela mestre
        $('#tsTableName, #tsMasterTableName').text(tableName);
        $('#tsMasterSchema').text(data.schema || 'dbo');
        $('#tsMasterPK').text(data.primaryKey || findPrimaryKey(state.masterColumns));
        $('#tsMasterColCount').text(state.masterColumns.length);
        $('#tsMasterFKCount').text(state.masterForeignKeys.length);
    }

    // =========================================================================
    // POPULAÇÃO DO STEP 2 - TABELA MESTRE
    // =========================================================================
    function populateMasterListagem(columns) {
        const $tbody = $('#masterListagemBody');
        $tbody.empty();

        columns.forEach((col, index) => {
            const badges = getBadges(col);
            const format = inferFormat(col.type);
            const align = inferAlign(col.type);

            const $row = $(`
                <tr data-col="${col.name}" data-order="${index}">
                    <td>
                        <input type="checkbox" class="form-check-input listagem-check" 
                               ${!col.isIdentity ? 'checked' : ''} />
                    </td>
                    <td>
                        <code class="ts-code">${col.name}</code>
                        <span class="ts-col-badges">${badges}</span>
                    </td>
                    <td>
                        <input type="text" class="form-control form-control-sm ts-input-sm listagem-title" 
                               value="${humanize(col.name)}" />
                    </td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm listagem-format">
                            <option value="text" ${format === 'text' ? 'selected' : ''}>Texto</option>
                            <option value="date" ${format === 'date' ? 'selected' : ''}>Data</option>
                            <option value="datetime" ${format === 'datetime' ? 'selected' : ''}>Data/Hora</option>
                            <option value="currency" ${format === 'currency' ? 'selected' : ''}>Moeda</option>
                            <option value="number" ${format === 'number' ? 'selected' : ''}>Número</option>
                            <option value="boolean" ${format === 'boolean' ? 'selected' : ''}>Sim/Não</option>
                        </select>
                    </td>
                    <td>
                        <input type="text" class="form-control form-control-sm ts-input-sm listagem-width" 
                               placeholder="auto" value="${col.isPrimaryKey ? '80px' : ''}" />
                    </td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm listagem-align">
                            <option value="left" ${align === 'left' ? 'selected' : ''}>Esquerda</option>
                            <option value="center" ${align === 'center' ? 'selected' : ''}>Centro</option>
                            <option value="right" ${align === 'right' ? 'selected' : ''}>Direita</option>
                        </select>
                    </td>
                    <td class="text-center">
                        <input type="checkbox" class="form-check-input listagem-sortable" checked />
                    </td>
                </tr>
            `);

            // Drag handle para reordenação (futuro)
            $tbody.append($row);
        });

        // Tornar tabela ordenável
        if (typeof Sortable !== 'undefined') {
            new Sortable($tbody[0], {
                animation: 150,
                handle: 'td:first-child',
                ghostClass: 'bg-primary-subtle'
            });
        }

        updateCounts();
    }

    function populateMasterFormulario(columns) {
        const $tbody = $('#masterFormularioBody');
        $tbody.empty();

        columns.forEach((col, index) => {
            const badges = getBadges(col);
            const inputType = inferInputType(col.type, col.name);
            const colSize = inferColSize(col.type, col.name);
            const isRequired = !col.isNullable && !col.isIdentity;
            const isDisabled = col.isIdentity || col.isPrimaryKey;

            const $row = $(`
                <tr data-col="${col.name}" data-order="${index}">
                    <td>
                        <input type="checkbox" class="form-check-input formulario-check" 
                               ${!col.isIdentity ? 'checked' : ''} />
                    </td>
                    <td>
                        <code class="ts-code">${col.name}</code>
                        <span class="ts-col-badges">${badges}</span>
                    </td>
                    <td>
                        <input type="text" class="form-control form-control-sm ts-input-sm formulario-label" 
                               value="${humanize(col.name)}" />
                    </td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm formulario-type">
                            <option value="text" ${inputType === 'text' ? 'selected' : ''}>Texto</option>
                            <option value="number" ${inputType === 'number' ? 'selected' : ''}>Número</option>
                            <option value="date" ${inputType === 'date' ? 'selected' : ''}>Data</option>
                            <option value="datetime-local" ${inputType === 'datetime-local' ? 'selected' : ''}>Data/Hora</option>
                            <option value="checkbox" ${inputType === 'checkbox' ? 'selected' : ''}>Checkbox</option>
                            <option value="select" ${inputType === 'select' ? 'selected' : ''}>Select</option>
                            <option value="textarea" ${inputType === 'textarea' ? 'selected' : ''}>Textarea</option>
                            <option value="email" ${inputType === 'email' ? 'selected' : ''}>E-mail</option>
                            <option value="password" ${inputType === 'password' ? 'selected' : ''}>Senha</option>
                        </select>
                    </td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm formulario-colsize">
                            <option value="2" ${colSize === 2 ? 'selected' : ''}>2</option>
                            <option value="3" ${colSize === 3 ? 'selected' : ''}>3</option>
                            <option value="4" ${colSize === 4 ? 'selected' : ''}>4</option>
                            <option value="6" ${colSize === 6 ? 'selected' : ''}>6</option>
                            <option value="8" ${colSize === 8 ? 'selected' : ''}>8</option>
                            <option value="12" ${colSize === 12 ? 'selected' : ''}>12</option>
                        </select>
                    </td>
                    <td class="text-center">
                        <input type="checkbox" class="form-check-input formulario-required" 
                               ${isRequired ? 'checked' : ''} />
                    </td>
                    <td class="text-center">
                        <input type="checkbox" class="form-check-input formulario-disabled" 
                               ${isDisabled ? 'checked' : ''} />
                    </td>
                </tr>
            `);

            $tbody.append($row);
        });

        updateCounts();
    }

    function populateMasterFKs(foreignKeys) {
        const $container = $('#masterFKsContainer');
        $container.empty();

        if (foreignKeys.length === 0) {
            $container.html(`
                <div class="col-12">
                    <div class="ts-empty-state py-4">
                        <i class="fas fa-unlink"></i>
                        <h6>Nenhum relacionamento encontrado</h6>
                        <p class="mb-0">Esta tabela não possui foreign keys</p>
                    </div>
                </div>
            `);
            return;
        }

        foreignKeys.forEach(fk => {
            const $card = $(`
                <div class="col-md-6 col-lg-4">
                    <div class="ts-fk-card">
                        <div class="fk-header">
                            <span class="fk-name">
                                <i class="fas fa-link me-1"></i>${fk.column}
                            </span>
                            <span class="badge ts-badge-fk">FK</span>
                        </div>
                        <div class="fk-detail">
                            <i class="fas fa-arrow-right me-1"></i>
                            ${fk.referencedTable}.${fk.referencedColumn}
                        </div>
                        <div class="mt-2">
                            <div class="form-check form-check-inline">
                                <input type="checkbox" class="form-check-input fk-generate-nav" 
                                       id="fk-nav-${fk.column}" checked />
                                <label class="form-check-label small" for="fk-nav-${fk.column}">
                                    Navigation Property
                                </label>
                            </div>
                            <div class="form-check form-check-inline">
                                <input type="checkbox" class="form-check-input fk-generate-lookup" 
                                       id="fk-lookup-${fk.column}" checked />
                                <label class="form-check-label small" for="fk-lookup-${fk.column}">
                                    Lookup
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            `);

            $container.append($card);
        });

        $('#masterFKsCount').text(foreignKeys.length);
    }

    // =========================================================================
    // POPULAÇÃO DO STEP 3 - TABELAS DETALHE
    // =========================================================================
    function populateAvailableTables(tables) {
        const $list = $('#tsAvailableList');
        $list.empty();

        if (tables.length === 0) {
            $list.html(`
                <div class="ts-empty-state py-4">
                    <i class="fas fa-database"></i>
                    <h6>Nenhuma tabela relacionada</h6>
                    <p class="mb-0">Não foram encontradas tabelas com FK para a tabela mestre</p>
                </div>
            `);
            $('#tsAvailableCount').text(0);
            return;
        }

        tables.forEach(table => {
            const $item = $(`
                <div class="ts-available-item" data-table="${table.tableName}">
                    <div class="ts-available-info">
                        <div class="ts-available-name">${humanize(table.tableName)}</div>
                        <div class="ts-available-meta">
                            <span><i class="fas fa-key"></i> ${table.fkColumn}</span>
                            <span><i class="fas fa-columns"></i> ${table.columnCount} cols</span>
                        </div>
                    </div>
                    <button class="btn btn-sm btn-primary ts-btn-add" 
                            onclick="TabSheetV2.addDetailTab('${table.tableName}')">
                        <i class="fas fa-plus"></i>
                    </button>
                </div>
            `);

            $list.append($item);
        });

        $('#tsAvailableCount').text(tables.length);
    }

    function filterAvailableTables(term) {
        $('.ts-available-item').each(function () {
            const tableName = $(this).data('table').toLowerCase();
            $(this).toggle(tableName.includes(term));
        });
    }

    // =========================================================================
    // GERENCIAMENTO DE ABAS DETALHE
    // =========================================================================
    function addDetailTab(tableName) {
        // Verificar se já foi adicionada
        if (state.selectedTabs.find(t => t.tableName === tableName)) {
            showToast('Atenção', 'Esta tabela já foi adicionada', 'warning');
            return;
        }

        // Encontrar dados da tabela
        const tableData = state.availableTables.find(t => t.tableName === tableName);
        if (!tableData) return;

        // Adicionar ao estado
        const tabConfig = {
            tableName: tableName,
            title: humanize(tableName),
            icon: 'fas fa-list',
            order: state.selectedTabs.length + 1,
            allowCreate: true,
            allowEdit: true,
            allowDelete: true,
            columns: [], // Será preenchido ao carregar
            fkColumn: tableData.fkColumn
        };

        state.selectedTabs.push(tabConfig);

        // Atualizar UI
        renderDetailTabs();
        markTableAsAdded(tableName);
        loadDetailTableColumns(tableName);

        // Esconder mensagem vazia
        $('#tsNoTabsMessage').hide();

        // Atualizar contador
        $('#tsSelectedCount').text(state.selectedTabs.length);
    }

    function removeDetailTab(tableName, event) {
        event.stopPropagation();

        // Remover do estado
        state.selectedTabs = state.selectedTabs.filter(t => t.tableName !== tableName);

        // Remover da UI
        $(`#tsDetailTabs li[data-table="${tableName}"]`).remove();
        $(`#tsDetailTabsContent .tab-pane[data-table="${tableName}"]`).remove();

        // Desmarcar da lista disponível
        $(`.ts-available-item[data-table="${tableName}"]`).removeClass('added');

        // Ativar outra aba se necessário
        if (state.selectedTabs.length > 0) {
            const firstTab = $(`#tsDetailTabs .nav-link`).first();
            firstTab.tab('show');
        } else {
            $('#tsNoTabsMessage').show();
        }

        $('#tsSelectedCount').text(state.selectedTabs.length);
    }

    function renderDetailTabs() {
        const $tabs = $('#tsDetailTabs');
        const $content = $('#tsDetailTabsContent');

        // Limpar existentes (manter mensagem vazia)
        $tabs.find('li').remove();
        $content.find('.tab-pane').remove();

        state.selectedTabs.forEach((tab, index) => {
            // Criar aba
            const isActive = index === state.selectedTabs.length - 1;
            const $tabLi = $(`
                <li class="nav-item" data-table="${tab.tableName}">
                    <button class="nav-link ${isActive ? 'active' : ''}" 
                            data-bs-toggle="tab" 
                            data-bs-target="#detail-${tab.tableName}">
                        <i class="${tab.icon} me-1"></i>
                        <span>${tab.title}</span>
                        <span class="ts-tab-close" onclick="TabSheetV2.removeDetailTab('${tab.tableName}', event)">
                            <i class="fas fa-times"></i>
                        </span>
                    </button>
                </li>
            `);

            // Criar conteúdo
            const $tabContent = $(`
                <div class="tab-pane fade ${isActive ? 'show active' : ''}" 
                     id="detail-${tab.tableName}" 
                     data-table="${tab.tableName}">
                    <div class="ts-detail-config p-4">
                        <!-- Header da aba -->
                        <div class="ts-detail-header mb-4">
                            <div class="row g-3 align-items-end">
                                <div class="col-md-3">
                                    <label class="ts-label">Título da Aba</label>
                                    <input type="text" class="form-control ts-input detail-title" 
                                           value="${tab.title}" />
                                </div>
                                <div class="col-md-2">
                                    <label class="ts-label">Ícone</label>
                                    <input type="text" class="form-control ts-input detail-icon" 
                                           value="${tab.icon}" />
                                </div>
                                <div class="col-md-2">
                                    <label class="ts-label">Ordem</label>
                                    <input type="number" class="form-control ts-input detail-order" 
                                           value="${tab.order}" />
                                </div>
                                <div class="col-md-5">
                                    <div class="ts-permissions">
                                        <label class="ts-label">Permissões</label>
                                        <div class="d-flex gap-3">
                                            <div class="form-check">
                                                <input type="checkbox" class="form-check-input detail-allow-create" 
                                                       ${tab.allowCreate ? 'checked' : ''} />
                                                <label class="form-check-label">Criar</label>
                                            </div>
                                            <div class="form-check">
                                                <input type="checkbox" class="form-check-input detail-allow-edit" 
                                                       ${tab.allowEdit ? 'checked' : ''} />
                                                <label class="form-check-label">Editar</label>
                                            </div>
                                            <div class="form-check">
                                                <input type="checkbox" class="form-check-input detail-allow-delete" 
                                                       ${tab.allowDelete ? 'checked' : ''} />
                                                <label class="form-check-label">Excluir</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Sub-tabs para Listagem e Formulário -->
                        <ul class="nav nav-pills ts-sub-tabs mb-3" role="tablist">
                            <li class="nav-item">
                                <button class="nav-link active" data-bs-toggle="pill" 
                                        data-bs-target="#detail-${tab.tableName}-listagem">
                                    <i class="fas fa-list me-2"></i>Colunas da Listagem
                                </button>
                            </li>
                            <li class="nav-item">
                                <button class="nav-link" data-bs-toggle="pill" 
                                        data-bs-target="#detail-${tab.tableName}-formulario">
                                    <i class="fas fa-edit me-2"></i>Campos do Formulário
                                </button>
                            </li>
                        </ul>

                        <div class="tab-content">
                            <div class="tab-pane fade show active" id="detail-${tab.tableName}-listagem">
                                <div class="ts-table-container" style="max-height: 280px;">
                                    <table class="table ts-config-table">
                                        <thead>
                                            <tr>
                                                <th style="width: 50px;"><input type="checkbox" class="form-check-input" checked /></th>
                                                <th>Coluna</th>
                                                <th>Título</th>
                                                <th style="width: 120px;">Formato</th>
                                                <th style="width: 100px;">Largura</th>
                                                <th style="width: 100px;">Alinhamento</th>
                                            </tr>
                                        </thead>
                                        <tbody class="detail-listagem-body"></tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="tab-pane fade" id="detail-${tab.tableName}-formulario">
                                <div class="ts-table-container" style="max-height: 280px;">
                                    <table class="table ts-config-table">
                                        <thead>
                                            <tr>
                                                <th style="width: 50px;"><input type="checkbox" class="form-check-input" checked /></th>
                                                <th>Campo</th>
                                                <th>Label</th>
                                                <th style="width: 140px;">Tipo</th>
                                                <th style="width: 100px;">Tamanho</th>
                                                <th style="width: 80px;" class="text-center">Obrig.</th>
                                            </tr>
                                        </thead>
                                        <tbody class="detail-formulario-body"></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `);

            $tabs.append($tabLi);
            $content.append($tabContent);
        });
    }

    async function loadDetailTableColumns(tableName) {
        try {
            const response = await fetch(`/api/tabsheet/columns/${encodeURIComponent(tableName)}`);

            if (!response.ok) {
                throw new Error('Erro ao carregar colunas');
            }

            const columns = await response.json();
            populateDetailColumns(tableName, columns);

        } catch (error) {
            console.warn('Usando dados mock para:', tableName);
            // Mock para demonstração
            const mockColumns = [
                { name: 'id', type: 'int', isPrimaryKey: true, isIdentity: true, isNullable: false },
                { name: 'id_' + state.masterTable, type: 'int', isPrimaryKey: false, isForeignKey: true, isNullable: false },
                { name: 'descricao', type: 'varchar(200)', isPrimaryKey: false, isNullable: true },
                { name: 'quantidade', type: 'int', isPrimaryKey: false, isNullable: true },
                { name: 'valor', type: 'decimal(18,2)', isPrimaryKey: false, isNullable: true },
                { name: 'data', type: 'datetime', isPrimaryKey: false, isNullable: true }
            ];
            populateDetailColumns(tableName, mockColumns);
        }
    }

    function populateDetailColumns(tableName, columns) {
        const $listagem = $(`#detail-${tableName}-listagem .detail-listagem-body`);
        const $formulario = $(`#detail-${tableName}-formulario .detail-formulario-body`);

        $listagem.empty();
        $formulario.empty();

        columns.forEach((col, index) => {
            const badges = getBadges(col);
            const format = inferFormat(col.type);
            const align = inferAlign(col.type);
            const inputType = inferInputType(col.type, col.name);
            const colSize = inferColSize(col.type, col.name);

            // Linha da listagem
            const $listagemRow = $(`
                <tr data-col="${col.name}">
                    <td><input type="checkbox" class="form-check-input" ${!col.isIdentity ? 'checked' : ''} /></td>
                    <td>
                        <code class="ts-code">${col.name}</code>
                        <span class="ts-col-badges">${badges}</span>
                    </td>
                    <td><input type="text" class="form-control form-control-sm ts-input-sm" value="${humanize(col.name)}" /></td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm">
                            <option value="text" ${format === 'text' ? 'selected' : ''}>Texto</option>
                            <option value="date" ${format === 'date' ? 'selected' : ''}>Data</option>
                            <option value="datetime" ${format === 'datetime' ? 'selected' : ''}>Data/Hora</option>
                            <option value="currency" ${format === 'currency' ? 'selected' : ''}>Moeda</option>
                            <option value="number" ${format === 'number' ? 'selected' : ''}>Número</option>
                            <option value="boolean" ${format === 'boolean' ? 'selected' : ''}>Sim/Não</option>
                        </select>
                    </td>
                    <td><input type="text" class="form-control form-control-sm ts-input-sm" placeholder="auto" /></td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm">
                            <option value="left" ${align === 'left' ? 'selected' : ''}>Esquerda</option>
                            <option value="center" ${align === 'center' ? 'selected' : ''}>Centro</option>
                            <option value="right" ${align === 'right' ? 'selected' : ''}>Direita</option>
                        </select>
                    </td>
                </tr>
            `);

            // Linha do formulário
            const $formularioRow = $(`
                <tr data-col="${col.name}">
                    <td><input type="checkbox" class="form-check-input" ${!col.isIdentity ? 'checked' : ''} /></td>
                    <td>
                        <code class="ts-code">${col.name}</code>
                        <span class="ts-col-badges">${badges}</span>
                    </td>
                    <td><input type="text" class="form-control form-control-sm ts-input-sm" value="${humanize(col.name)}" /></td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm">
                            <option value="text" ${inputType === 'text' ? 'selected' : ''}>Texto</option>
                            <option value="number" ${inputType === 'number' ? 'selected' : ''}>Número</option>
                            <option value="date" ${inputType === 'date' ? 'selected' : ''}>Data</option>
                            <option value="datetime-local" ${inputType === 'datetime-local' ? 'selected' : ''}>Data/Hora</option>
                            <option value="checkbox" ${inputType === 'checkbox' ? 'selected' : ''}>Checkbox</option>
                            <option value="select" ${inputType === 'select' ? 'selected' : ''}>Select</option>
                            <option value="textarea" ${inputType === 'textarea' ? 'selected' : ''}>Textarea</option>
                        </select>
                    </td>
                    <td>
                        <select class="form-select form-select-sm ts-input-sm">
                            <option value="3" ${colSize === 3 ? 'selected' : ''}>3</option>
                            <option value="4" ${colSize === 4 ? 'selected' : ''}>4</option>
                            <option value="6" ${colSize === 6 ? 'selected' : ''}>6</option>
                            <option value="12" ${colSize === 12 ? 'selected' : ''}>12</option>
                        </select>
                    </td>
                    <td class="text-center">
                        <input type="checkbox" class="form-check-input" ${!col.isNullable && !col.isIdentity ? 'checked' : ''} />
                    </td>
                </tr>
            `);

            $listagem.append($listagemRow);
            $formulario.append($formularioRow);
        });

        // Atualizar estado
        const tabIndex = state.selectedTabs.findIndex(t => t.tableName === tableName);
        if (tabIndex >= 0) {
            state.selectedTabs[tabIndex].columns = columns;
        }
    }

    function markTableAsAdded(tableName) {
        $(`.ts-available-item[data-table="${tableName}"]`).addClass('added');
    }

    // =========================================================================
    // NAVEGAÇÃO DO WIZARD
    // =========================================================================
    function goToStep(step) {
        if (step < 1 || step > state.totalSteps) return;

        // Validar passo atual antes de avançar
        if (step > state.currentStep && !validateCurrentStep()) {
            return;
        }

        // Atualizar estado
        const previousStep = state.currentStep;
        state.currentStep = step;

        // Atualizar UI dos steps
        $steps.each(function () {
            const stepNum = parseInt($(this).data('step'));
            $(this).removeClass('active completed');

            if (stepNum < step) {
                $(this).addClass('completed');
            } else if (stepNum === step) {
                $(this).addClass('active');
            }
        });

        // Atualizar conteúdo
        $stepContents.removeClass('active');
        $(`.ts-step-content[data-step="${step}"]`).addClass('active');

        // Atualizar botões
        updateNavigationButtons();

        // Ações específicas por step
        if (step === 4) {
            updateSummary();
        }
    }

    function nextStep() {
        goToStep(state.currentStep + 1);
    }

    function prevStep() {
        goToStep(state.currentStep - 1);
    }

    function validateCurrentStep() {
        switch (state.currentStep) {
            case 1:
                const configId = $('#tsConfigId').val().trim();
                const title = $('#tsTitle').val().trim();

                if (!configId) {
                    showToast('Atenção', 'Informe o ID da configuração', 'warning');
                    $('#tsConfigId').focus();
                    return false;
                }

                if (!title) {
                    showToast('Atenção', 'Informe o título da tela', 'warning');
                    $('#tsTitle').focus();
                    return false;
                }
                return true;

            case 2:
                const listagemChecked = $('#masterListagemBody .listagem-check:checked').length;
                const formularioChecked = $('#masterFormularioBody .formulario-check:checked').length;

                if (listagemChecked === 0) {
                    showToast('Atenção', 'Selecione ao menos uma coluna para a listagem', 'warning');
                    return false;
                }

                if (formularioChecked === 0) {
                    showToast('Atenção', 'Selecione ao menos um campo para o formulário', 'warning');
                    return false;
                }
                return true;

            case 3:
                // Abas são opcionais
                return true;

            default:
                return true;
        }
    }

    function updateNavigationButtons() {
        // Botão Anterior
        if (state.currentStep === 1) {
            $btnPrev.hide();
        } else {
            $btnPrev.show();
        }

        // Botões Próximo/Gerar
        if (state.currentStep === state.totalSteps) {
            $btnNext.hide();
            $btnPreview.show();
            $btnGenerate.show();
        } else {
            $btnNext.show();
            $btnPreview.hide();
            $btnGenerate.hide();
        }
    }

    // =========================================================================
    // RESUMO E GERAÇÃO
    // =========================================================================
    function updateSummary() {
        // Tabela mestre
        $('#summaryMasterTable').text(state.masterTable);
        $('#summaryMasterListCols').text($('#masterListagemBody .listagem-check:checked').length);
        $('#summaryMasterFormCols').text($('#masterFormularioBody .formulario-check:checked').length);

        // Tabelas detalhe
        const $detailContainer = $('#summaryDetailTabs');
        $detailContainer.empty();

        if (state.selectedTabs.length === 0) {
            $detailContainer.html('<p class="text-muted mb-0">Nenhuma aba selecionada</p>');
        } else {
            state.selectedTabs.forEach(tab => {
                $detailContainer.append(`
                    <div class="ts-summary-item">
                        <span><i class="${tab.icon} me-1"></i>${tab.title}</span>
                        <strong>${tab.tableName}</strong>
                    </div>
                `);
            });
        }

        // Arquivos a gerar
        updateFilesList();
    }

    function updateFilesList() {
        const $container = $('#summaryFiles');
        $container.empty();

        const masterName = toPascalCase(state.masterTable);
        const files = [];

        // Backend
        if ($('#optMasterEntity').is(':checked')) {
            files.push({ icon: 'fa-file-code', name: `${masterName}.cs`, type: 'Entity' });
        }
        if ($('#optDetailEntities').is(':checked')) {
            state.selectedTabs.forEach(tab => {
                files.push({ icon: 'fa-file-code', name: `${toPascalCase(tab.tableName)}.cs`, type: 'Entity' });
            });
        }
        if ($('#optController').is(':checked')) {
            files.push({ icon: 'fa-file-code', name: `${masterName}Controller.cs`, type: 'Controller' });
        }

        // Frontend
        if ($('#optMasterView').is(':checked')) {
            files.push({ icon: 'fa-file-alt', name: `Index.cshtml`, type: 'View' });
        }
        if ($('#optTabPartials').is(':checked')) {
            state.selectedTabs.forEach(tab => {
                files.push({ icon: 'fa-file-alt', name: `_Tab${toPascalCase(tab.tableName)}.cshtml`, type: 'Partial' });
            });
        }
        if ($('#optJavaScript').is(':checked')) {
            files.push({ icon: 'fa-file-code', name: `${masterName.toLowerCase()}.js`, type: 'JS' });
        }

        files.forEach(file => {
            $container.append(`
                <div class="ts-file-item">
                    <i class="fas ${file.icon}"></i>
                    <span>${file.name}</span>
                    <span class="badge bg-secondary ms-auto">${file.type}</span>
                </div>
            `);
        });
    }

    async function generateCode() {
        const config = buildConfiguration();

        showLoading(true);
        $btnGenerate.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Gerando...');

        try {
            const response = await fetch('/api/tabsheet/generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(config)
            });

            if (!response.ok) {
                throw new Error('Erro na geração');
            }

            // Download do ZIP
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `${config.id}.zip`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();

            showToast('Sucesso', 'Código gerado com sucesso!', 'success');

            // Fechar modal
            bootstrap.Modal.getInstance($modal[0]).hide();

        } catch (error) {
            console.error('Erro na geração:', error);
            showToast('Erro', 'Falha ao gerar código. Verifique o console.', 'error');
        } finally {
            showLoading(false);
            $btnGenerate.prop('disabled', false).html('<i class="fas fa-download me-2"></i>Gerar e Baixar ZIP');
        }
    }

    function buildConfiguration() {
        // Coletar dados da listagem mestre
        const masterListagem = [];
        $('#masterListagemBody tr').each(function () {
            if ($(this).find('.listagem-check').is(':checked')) {
                masterListagem.push({
                    column: $(this).data('col'),
                    title: $(this).find('.listagem-title').val(),
                    format: $(this).find('.listagem-format').val(),
                    width: $(this).find('.listagem-width').val(),
                    align: $(this).find('.listagem-align').val(),
                    sortable: $(this).find('.listagem-sortable').is(':checked')
                });
            }
        });

        // Coletar dados do formulário mestre
        const masterFormulario = [];
        $('#masterFormularioBody tr').each(function () {
            if ($(this).find('.formulario-check').is(':checked')) {
                masterFormulario.push({
                    column: $(this).data('col'),
                    label: $(this).find('.formulario-label').val(),
                    type: $(this).find('.formulario-type').val(),
                    colSize: parseInt($(this).find('.formulario-colsize').val()),
                    required: $(this).find('.formulario-required').is(':checked'),
                    disabled: $(this).find('.formulario-disabled').is(':checked')
                });
            }
        });

        // Coletar dados das abas
        const tabs = state.selectedTabs.map(tab => {
            const $pane = $(`#detail-${tab.tableName}`);

            // Colunas da listagem
            const listagem = [];
            $pane.find('.detail-listagem-body tr').each(function () {
                if ($(this).find('input[type="checkbox"]').first().is(':checked')) {
                    listagem.push({
                        column: $(this).data('col'),
                        title: $(this).find('input[type="text"]').first().val(),
                        format: $(this).find('select').eq(0).val(),
                        width: $(this).find('input[type="text"]').eq(1).val(),
                        align: $(this).find('select').eq(1).val()
                    });
                }
            });

            // Campos do formulário
            const formulario = [];
            $pane.find('.detail-formulario-body tr').each(function () {
                if ($(this).find('input[type="checkbox"]').first().is(':checked')) {
                    formulario.push({
                        column: $(this).data('col'),
                        label: $(this).find('input[type="text"]').first().val(),
                        type: $(this).find('select').eq(0).val(),
                        colSize: parseInt($(this).find('select').eq(1).val()),
                        required: $(this).find('input[type="checkbox"]').last().is(':checked')
                    });
                }
            });

            return {
                tableName: tab.tableName,
                title: $pane.find('.detail-title').val(),
                icon: $pane.find('.detail-icon').val(),
                order: parseInt($pane.find('.detail-order').val()),
                fkColumn: tab.fkColumn,
                allowCreate: $pane.find('.detail-allow-create').is(':checked'),
                allowEdit: $pane.find('.detail-allow-edit').is(':checked'),
                allowDelete: $pane.find('.detail-allow-delete').is(':checked'),
                listagem: listagem,
                formulario: formulario
            };
        });

        return {
            id: $('#tsConfigId').val(),
            title: $('#tsTitle').val(),
            description: $('#tsDescription').val(),
            module: $('#tsModule').val(),
            masterTable: {
                tableName: state.masterTable,
                primaryKey: findPrimaryKey(state.masterColumns),
                listagem: masterListagem,
                formulario: masterFormulario,
                foreignKeys: state.masterForeignKeys
            },
            tabs: tabs,
            options: {
                generateMasterEntity: $('#optMasterEntity').is(':checked'),
                generateDetailEntities: $('#optDetailEntities').is(':checked'),
                generateNavigations: $('#optNavigations').is(':checked'),
                generateController: $('#optController').is(':checked'),
                generateMasterView: $('#optMasterView').is(':checked'),
                generateTabPartials: $('#optTabPartials').is(':checked'),
                generateJavaScript: $('#optJavaScript').is(':checked'),
                useDataTables: $('#optDataTables').is(':checked'),
                moduleRoute: $('#tsModuleRoute').val(),
                icon: $('#tsIcon').val()
            }
        };
    }

    function showPreview() {
        const config = buildConfiguration();
        console.log('📋 Configuração gerada:', config);

        // Mostrar JSON em modal ou console
        const jsonStr = JSON.stringify(config, null, 2);

        // Criar modal de preview
        const $previewModal = $(`
            <div class="modal fade" id="modalPreview" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title"><i class="fas fa-code me-2"></i>Preview da Configuração</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <pre style="max-height: 500px; overflow: auto;"><code class="language-json">${escapeHtml(jsonStr)}</code></pre>
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
                            <button class="btn btn-primary" onclick="navigator.clipboard.writeText(\`${jsonStr.replace(/`/g, '\\`').replace(/\\/g, '\\\\')}\`); showToast('Copiado', 'JSON copiado para a área de transferência', 'success');">
                                <i class="fas fa-copy me-2"></i>Copiar JSON
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `);

        // Remover modal existente
        $('#modalPreview').remove();
        $('body').append($previewModal);

        // Highlight
        if (typeof hljs !== 'undefined') {
            $previewModal.find('pre code').each(function () {
                hljs.highlightElement(this);
            });
        }

        new bootstrap.Modal($previewModal[0]).show();
    }

    // =========================================================================
    // UTILITÁRIOS
    // =========================================================================
    function showLoading(show) {
        state.isLoading = show;
        if (show) {
            $loading.addClass('active');
        } else {
            $loading.removeClass('active');
        }
    }

    function resetState() {
        state.currentStep = 1;
        state.masterTable = null;
        state.masterColumns = [];
        state.masterForeignKeys = [];
        state.availableTables = [];
        state.selectedTabs = [];

        // Reset UI
        goToStep(1);
        $('#masterListagemBody, #masterFormularioBody, #masterFKsContainer').empty();
        $('#tsAvailableList').empty();
        $('#tsDetailTabs li, #tsDetailTabsContent .tab-pane').remove();
        $('#tsNoTabsMessage').show();

        // Reset contadores
        $('#tsSelectedCount, #tsAvailableCount, #masterListagemCount, #masterFormularioCount').text('0');
    }

    function updateCounts() {
        const listagemCount = $('#masterListagemBody .listagem-check:checked').length;
        const formularioCount = $('#masterFormularioBody .formulario-check:checked').length;

        $('#masterListagemCount').text(listagemCount);
        $('#masterFormularioCount').text(formularioCount);
    }

    function selectAllMasterListagem(checked) {
        $('#masterListagemBody .listagem-check').prop('checked', checked);
        $('#checkAllMasterListagem').prop('checked', checked);
        updateCounts();
    }

    function selectAllMasterForm(checked) {
        $('#masterFormularioBody .formulario-check').prop('checked', checked);
        $('#checkAllMasterForm').prop('checked', checked);
        updateCounts();
    }

    // =========================================================================
    // HELPERS
    // =========================================================================
    function toPascalCase(str) {
        return str
            .replace(/[-_](.)/g, (_, c) => c.toUpperCase())
            .replace(/^(.)/, (_, c) => c.toUpperCase());
    }

    function humanize(str) {
        return str
            .replace(/[-_]/g, ' ')
            .replace(/([A-Z])/g, ' $1')
            .replace(/^./, s => s.toUpperCase())
            .trim();
    }

    function getBadges(col) {
        let badges = '';
        if (col.isPrimaryKey) {
            badges += '<span class="badge ts-badge-pk">PK</span>';
        }
        if (col.isForeignKey) {
            badges += '<span class="badge ts-badge-fk">FK</span>';
        }
        if (col.isIdentity) {
            badges += '<span class="badge ts-badge-identity">ID</span>';
        }
        return badges;
    }

    function inferFormat(type) {
        type = (type || '').toLowerCase();

        // MELHORADO: Detectar automaticamente o formato correto

        // Tipos de data/hora
        if (type.includes('datetime2') || type.includes('datetime')) return 'datetime';
        if (type.includes('datetimeoffset')) return 'datetime';
        if (type.includes('smalldatetime')) return 'datetime';
        if (type.includes('date') && type.includes('time')) return 'datetime';
        if (type.includes('date')) return 'date';
        if (type.includes('time')) return 'text';

        // Tipos monetarios
        if (type.includes('money') || type.includes('smallmoney')) return 'currency';

        // Tipos numericos
        if (type.includes('decimal') || type.includes('numeric')) return 'number';
        if (type.includes('int') || type.includes('bigint') || type.includes('smallint') || type.includes('tinyint')) return 'number';
        if (type.includes('float') || type.includes('real')) return 'number';

        // Tipos booleanos
        if (type.includes('bit')) return 'boolean';

        // Padrao: texto
        return 'text';
    }

    function inferAlign(type) {
        type = (type || '').toLowerCase();

        // MELHORADO: Detectar automaticamente o alinhamento correto

        // Alinhar a direita: numeros e moeda
        if (type.includes('int') || type.includes('bigint') || type.includes('smallint') || type.includes('tinyint')) return 'right';
        if (type.includes('decimal') || type.includes('numeric') || type.includes('money') || type.includes('smallmoney')) return 'right';
        if (type.includes('float') || type.includes('real')) return 'right';

        // Alinhar no centro: booleanos e datas
        if (type.includes('bit')) return 'center';
        if (type.includes('date') || type.includes('datetime') || type.includes('time')) return 'center';

        // Padrao: alinhar a esquerda
        return 'left';
    }

    function inferInputType(type, name) {
        type = (type || '').toLowerCase();
        name = (name || '').toLowerCase();

        // MELHORADO: Detectar automaticamente o tipo correto

        // Campos especiais por nome
        if (name.includes('email')) return 'email';
        if (name.includes('senha') || name.includes('password')) return 'password';

        // Tipos booleanos
        if (type.includes('bit')) return 'checkbox';

        // Tipos de data/hora
        if (type.includes('datetime2') || type.includes('datetime')) return 'datetime-local';
        if (type.includes('datetimeoffset')) return 'datetime-local';
        if (type.includes('smalldatetime')) return 'datetime-local';
        if (type.includes('date')) return 'date';
        if (type.includes('time')) return 'time';

        // Tipos numericos
        if (type.includes('int') || type.includes('bigint') || type.includes('smallint') || type.includes('tinyint')) return 'number';
        if (type.includes('decimal') || type.includes('numeric') || type.includes('money') || type.includes('float') || type.includes('real')) return 'number';

        // Tipos de texto grande
        if (type.includes('text') && !type.includes('ntext')) return 'textarea';
        if (type.includes('ntext')) return 'textarea';
        if (type.includes('varchar') && type.includes('max')) return 'textarea';
        if (type.includes('nvarchar') && type.includes('max')) return 'textarea';

        // Padrao: texto
        return 'text';
    }

    function inferColSize(type, name) {
        type = (type || '').toLowerCase();
        name = (name || '').toLowerCase();

        // MELHORADO: Detectar automaticamente o tamanho da coluna

        // Campos muito grandes (12 colunas)
        if (type.includes('text') && !type.includes('ntext')) return 12;
        if (type.includes('ntext')) return 12;
        if (type.includes('varchar') && type.includes('max')) return 12;
        if (type.includes('nvarchar') && type.includes('max')) return 12;
        if (name.includes('descricao') || name.includes('observacao') || name.includes('comentario')) return 12;

        // Campos pequenos (2-3 colunas)
        if (type.includes('bit')) return 2;
        if (type.includes('date') && !type.includes('time')) return 3;
        if (type.includes('time')) return 3;
        if (type.includes('datetime') || type.includes('datetimeoffset')) return 4;
        if (type.includes('int') || type.includes('bigint') || type.includes('smallint') || type.includes('tinyint')) return 3;
        if (type.includes('decimal') || type.includes('numeric') || type.includes('money')) return 3;
        if (type.includes('float') || type.includes('real')) return 3;

        // Campos pequenos por nome
        if (name.includes('codigo') || name.includes('code') || name.includes('sigla')) return 3;
        if (name.includes('cep') || name.includes('telefone') || name.includes('phone')) return 4;

        // Padrao: 6 colunas
        return 6;
    }

    function findPrimaryKey(columns) {
        const pk = columns.find(c => c.isPrimaryKey);
        return pk ? pk.name : 'id';
    }

    function escapeHtml(str) {
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function showToast(title, message, type) {
        if (typeof window.showToast === 'function') {
            window.showToast(title, message, type);
        } else {
            console.log(`[${type.toUpperCase()}] ${title}: ${message}`);
            alert(`${title}: ${message}`);
        }
    }

    // =========================================================================
    // API PÚBLICA
    // =========================================================================
    return {
        init,
        openModal,
        addDetailTab,
        removeDetailTab,
        selectAllMasterListagem,
        selectAllMasterForm,
        generateCode,
        showPreview
    };

})();

// Inicialização quando o DOM estiver pronto
$(document).ready(function () {
    TabSheetV2.init();
});

// Compatibilidade com versão anterior
window.TabSheetConfig = {
    openModal: TabSheetV2.openModal
};
