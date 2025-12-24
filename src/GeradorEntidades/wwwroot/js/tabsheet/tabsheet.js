/**
 * ============================================================================
 * TABSHEET GENERATOR v2.0 - JavaScript
 * ============================================================================
 * Gerenciador completo do wizard de geração de telas Mestre/Detalhe
 * Inclui: DTOs, Services, Controllers, Views, JavaScript
 * 
 * Versão: 2.0
 * Data: 2024
 * ============================================================================
 */

class TabSheetWizard {
    constructor() {
        this.currentStep = 1;
        this.totalSteps = 4;
        this.config = {
            id: '',
            title: '',
            description: '',
            module: 'GestaoDePessoas',
            masterTable: null,
            tabs: [],
            options: this.getDefaultOptions()
        };
        this.tableMetadata = {};
        this.relatedTables = [];

        this.init();
    }

    /**
     * Opções padrão de geração
     */
    getDefaultOptions() {
        return {
            // Backend
            generateMasterEntity: true,
            generateDetailEntities: true,
            generateDTOs: true,           // ← NOVO
            generateServices: true,        // ← NOVO
            generateNavigations: true,
            generateController: true,

            // Frontend
            generateMasterView: true,
            generateTabPartials: true,
            generateJavaScript: true,
            useDataTables: true,

            // Configurações
            moduleRoute: '',
            icon: 'fas fa-table',
            cdFuncao: '',
            cdSistema: 'RHU'
        };
    }

    /**
     * Inicialização
     */
    init() {
        this.bindEvents();
        this.loadTables();
        console.log('✅ TabSheet Wizard v2.0 inicializado');
    }

    /**
     * Bind de eventos
     */
    bindEvents() {
        // Navegação do wizard
        document.getElementById('btnNext')?.addEventListener('click', () => this.nextStep());
        document.getElementById('btnPrev')?.addEventListener('click', () => this.prevStep());
        document.getElementById('btnGenerate')?.addEventListener('click', () => this.generate());

        // Seleção de tabela mestre
        document.getElementById('tsMasterTable')?.addEventListener('change', (e) => {
            this.onMasterTableChange(e.target.value);
        });

        // Busca de tabelas
        document.getElementById('tsTableSearch')?.addEventListener('input', (e) => {
            this.filterTables(e.target.value);
        });

        // Campos básicos
        document.getElementById('tsConfigId')?.addEventListener('input', (e) => {
            this.config.id = e.target.value;
            this.updateSummary();
        });

        document.getElementById('tsTitle')?.addEventListener('input', (e) => {
            this.config.title = e.target.value;
            this.updateSummary();
        });

        document.getElementById('tsModule')?.addEventListener('change', (e) => {
            this.config.module = e.target.value;
        });

        // Opções de geração - Backend
        this.bindOptionCheckbox('optMasterEntity', 'generateMasterEntity');
        this.bindOptionCheckbox('optDetailEntities', 'generateDetailEntities');
        this.bindOptionCheckbox('optDTOs', 'generateDTOs');           // ← NOVO
        this.bindOptionCheckbox('optServices', 'generateServices');    // ← NOVO
        this.bindOptionCheckbox('optNavigations', 'generateNavigations');
        this.bindOptionCheckbox('optController', 'generateController');

        // Opções de geração - Frontend
        this.bindOptionCheckbox('optMasterView', 'generateMasterView');
        this.bindOptionCheckbox('optTabPartials', 'generateTabPartials');
        this.bindOptionCheckbox('optJavaScript', 'generateJavaScript');
        this.bindOptionCheckbox('optDataTables', 'useDataTables');

        // Configurações adicionais
        document.getElementById('tsModuleRoute')?.addEventListener('input', (e) => {
            this.config.options.moduleRoute = e.target.value;
        });

        document.getElementById('tsIcon')?.addEventListener('input', (e) => {
            this.config.options.icon = e.target.value;
        });

        document.getElementById('tsCdFuncao')?.addEventListener('input', (e) => {
            this.config.options.cdFuncao = e.target.value;
        });

        document.getElementById('tsCdSistema')?.addEventListener('input', (e) => {
            this.config.options.cdSistema = e.target.value;
        });

        // Steps clicáveis
        document.querySelectorAll('.ts-step').forEach((step, index) => {
            step.addEventListener('click', () => {
                if (index + 1 <= this.currentStep) {
                    this.goToStep(index + 1);
                }
            });
        });
    }

    /**
     * Bind de checkbox de opção
     */
    bindOptionCheckbox(elementId, optionKey) {
        const el = document.getElementById(elementId);
        if (el) {
            el.addEventListener('change', (e) => {
                this.config.options[optionKey] = e.target.checked;
                this.updateSummary();
                console.log(`📦 Opção ${optionKey}: ${e.target.checked}`);
            });
        }
    }

    /**
     * Carrega lista de tabelas do banco
     */
    async loadTables() {
        try {
            const response = await fetch('/api/tabsheet/tables');
            if (!response.ok) throw new Error('Erro ao carregar tabelas');

            const tables = await response.json();
            this.populateTableSelect(tables);
            console.log(`📊 ${tables.length} tabelas carregadas`);
        } catch (error) {
            console.error('❌ Erro ao carregar tabelas:', error);
            this.showError('Erro ao carregar lista de tabelas');
        }
    }

    /**
     * Popula select de tabelas
     */
    populateTableSelect(tables) {
        const select = document.getElementById('tsMasterTable');
        if (!select) return;

        select.innerHTML = '<option value="">Selecione uma tabela...</option>';

        tables.forEach(table => {
            const option = document.createElement('option');
            option.value = table.nomeTabela;
            option.textContent = `${table.nomeTabela} (${table.colunas?.length || 0} colunas)`;
            select.appendChild(option);
        });
    }

    /**
     * Filtro de tabelas na busca
     */
    filterTables(searchText) {
        const select = document.getElementById('tsMasterTable');
        if (!select) return;

        const options = select.querySelectorAll('option');
        const search = searchText.toLowerCase();

        options.forEach(option => {
            if (option.value === '') return;
            const match = option.textContent.toLowerCase().includes(search);
            option.style.display = match ? '' : 'none';
        });
    }

    /**
     * Quando a tabela mestre é alterada
     */
    async onMasterTableChange(tableName) {
        if (!tableName) {
            this.config.masterTable = null;
            this.tableMetadata = {};
            this.relatedTables = [];
            this.updateMasterConfig();
            return;
        }

        try {
            this.showLoading('Carregando metadados...');

            const response = await fetch(`/api/tabsheet/metadata/${tableName}`);
            if (!response.ok) throw new Error('Erro ao carregar metadados');

            const metadata = await response.json();
            this.tableMetadata = metadata;
            this.relatedTables = metadata.relatedTables || [];

            this.config.masterTable = {
                tableName: tableName,
                primaryKey: metadata.primaryKey,
                listagem: [],
                formulario: [],
                foreignKeys: metadata.foreignKeys
            };

            // Auto-preencher ID e título se vazios
            if (!this.config.id) {
                document.getElementById('tsConfigId').value = this.toPascalCase(tableName);
                this.config.id = this.toPascalCase(tableName);
            }
            if (!this.config.title) {
                document.getElementById('tsTitle').value = this.formatDisplayName(tableName);
                this.config.title = this.formatDisplayName(tableName);
            }

            this.updateMasterConfig();
            this.updateRelatedTables();
            this.hideLoading();

            console.log(`✅ Metadados carregados para ${tableName}:`, metadata);
        } catch (error) {
            console.error('❌ Erro ao carregar metadados:', error);
            this.hideLoading();
            this.showError('Erro ao carregar metadados da tabela');
        }
    }

    /**
     * Atualiza configuração da tabela mestre
     */
    updateMasterConfig() {
        const container = document.getElementById('masterColumnConfig');
        if (!container) return;

        if (!this.tableMetadata.columns) {
            container.innerHTML = '<p class="text-muted">Selecione uma tabela para configurar as colunas</p>';
            return;
        }

        let html = `
            <div class="mb-4">
                <h6 class="mb-3"><i class="fas fa-list me-2"></i>Colunas para Listagem</h6>
                <div class="ts-column-list" id="masterListColumns">
                    ${this.renderColumnCheckboxes(this.tableMetadata.columns, 'list')}
                </div>
            </div>
            <div class="mb-4">
                <h6 class="mb-3"><i class="fas fa-edit me-2"></i>Campos do Formulário</h6>
                <div class="ts-column-list" id="masterFormColumns">
                    ${this.renderColumnCheckboxes(this.tableMetadata.columns, 'form')}
                </div>
            </div>
        `;

        container.innerHTML = html;
        this.bindColumnCheckboxes();
    }

    /**
     * Renderiza checkboxes das colunas
     */
    renderColumnCheckboxes(columns, type) {
        return columns.map((col, index) => {
            const isPk = col.isPrimaryKey;
            const isFk = col.isForeignKey;
            const checked = type === 'list' ? !isPk : !isPk && !col.isIdentity;
            const icon = isPk ? 'fa-key text-warning' : isFk ? 'fa-link text-info' : 'fa-columns';

            return `
                <div class="ts-column-item">
                    <div class="form-check">
                        <input type="checkbox" class="form-check-input" 
                               id="${type}_${col.name}" 
                               data-column="${col.name}"
                               data-type="${type}"
                               ${checked ? 'checked' : ''}>
                        <label class="form-check-label" for="${type}_${col.name}">
                            <i class="fas ${icon} me-1"></i>
                            <strong>${col.name}</strong>
                            <small class="text-muted ms-2">${col.type}${col.maxLength ? `(${col.maxLength})` : ''}</small>
                        </label>
                    </div>
                </div>
            `;
        }).join('');
    }

    /**
     * Bind de checkboxes das colunas
     */
    bindColumnCheckboxes() {
        document.querySelectorAll('#masterListColumns input[type="checkbox"]').forEach(cb => {
            cb.addEventListener('change', () => this.updateColumnConfig('list'));
        });

        document.querySelectorAll('#masterFormColumns input[type="checkbox"]').forEach(cb => {
            cb.addEventListener('change', () => this.updateColumnConfig('form'));
        });

        // Trigger inicial
        this.updateColumnConfig('list');
        this.updateColumnConfig('form');
    }

    /**
     * Atualiza configuração de colunas
     */
    updateColumnConfig(type) {
        const container = document.getElementById(type === 'list' ? 'masterListColumns' : 'masterFormColumns');
        if (!container) return;

        const selected = [];
        container.querySelectorAll('input[type="checkbox"]:checked').forEach((cb, index) => {
            const colName = cb.dataset.column;
            const colMeta = this.tableMetadata.columns.find(c => c.name === colName);

            if (type === 'list') {
                selected.push({
                    column: colName,
                    title: this.formatDisplayName(colName),
                    format: this.getDefaultFormat(colMeta),
                    sortable: true,
                    align: 'left'
                });
            } else {
                selected.push({
                    column: colName,
                    label: this.formatDisplayName(colName),
                    type: this.getDefaultInputType(colMeta),
                    colSize: 6,
                    required: !colMeta?.isNullable
                });
            }
        });

        if (type === 'list') {
            this.config.masterTable.listagem = selected;
        } else {
            this.config.masterTable.formulario = selected;
        }

        this.updateSummary();
    }

    /**
     * Atualiza lista de tabelas relacionadas
     */
    updateRelatedTables() {
        const container = document.getElementById('relatedTablesList');
        if (!container) return;

        if (!this.relatedTables.length) {
            container.innerHTML = '<p class="text-muted">Nenhuma tabela relacionada encontrada</p>';
            return;
        }

        let html = '';
        this.relatedTables.forEach(table => {
            const isAdded = this.config.tabs.some(t => t.tableName === table.tableName);

            html += `
                <div class="ts-available-item ${isAdded ? 'added' : ''}" data-table="${table.tableName}">
                    <div class="ts-available-info">
                        <span class="ts-available-name">${table.tableName}</span>
                        <div class="ts-available-meta">
                            <span><i class="fas fa-link"></i>${table.fkColumn}</span>
                            <span><i class="fas fa-columns"></i>${table.columnCount} colunas</span>
                        </div>
                    </div>
                    <button class="btn btn-sm btn-primary ts-btn-add" 
                            onclick="tabSheetWizard.addTab('${table.tableName}', '${table.fkColumn}')"
                            ${isAdded ? 'disabled' : ''}>
                        <i class="fas fa-plus"></i>
                    </button>
                </div>
            `;
        });

        container.innerHTML = html;
    }

    /**
     * Adiciona uma aba (tabela detalhe)
     */
    async addTab(tableName, fkColumn) {
        try {
            this.showLoading('Carregando metadados da tabela...');

            const response = await fetch(`/api/tabsheet/metadata/${tableName}`);
            if (!response.ok) throw new Error('Erro ao carregar metadados');

            const metadata = await response.json();

            const tab = {
                tableName: tableName,
                title: this.formatDisplayName(tableName),
                icon: 'fas fa-list',
                order: this.config.tabs.length,
                fkColumn: fkColumn,
                allowCreate: true,
                allowEdit: true,
                allowDelete: true,
                listagem: [],
                formulario: [],
                metadata: metadata
            };

            this.config.tabs.push(tab);
            this.updateRelatedTables();
            this.updateTabsUI();
            this.updateSummary();
            this.hideLoading();

            console.log(`✅ Aba adicionada: ${tableName}`);
        } catch (error) {
            console.error('❌ Erro ao adicionar aba:', error);
            this.hideLoading();
            this.showError('Erro ao adicionar aba');
        }
    }

    /**
     * Remove uma aba
     */
    removeTab(index) {
        this.config.tabs.splice(index, 1);
        this.updateRelatedTables();
        this.updateTabsUI();
        this.updateSummary();
    }

    /**
     * Atualiza UI das abas
     */
    updateTabsUI() {
        const container = document.getElementById('detailTabsConfig');
        if (!container) return;

        if (!this.config.tabs.length) {
            container.innerHTML = `
                <div class="ts-empty-state">
                    <i class="fas fa-folder-open"></i>
                    <h5>Nenhuma aba selecionada</h5>
                    <p>Clique no botão "+" nas tabelas relacionadas para adicionar abas</p>
                </div>
            `;
            return;
        }

        let tabsHtml = '<ul class="nav nav-tabs ts-detail-tabs" role="tablist">';
        let contentHtml = '<div class="tab-content ts-detail-config">';

        this.config.tabs.forEach((tab, index) => {
            const active = index === 0 ? 'active' : '';

            tabsHtml += `
                <li class="nav-item" role="presentation">
                    <button class="nav-link ${active}" data-bs-toggle="tab" data-bs-target="#tab${index}">
                        <i class="${tab.icon} me-1"></i>${tab.title}
                        <span class="ts-tab-close" onclick="event.stopPropagation(); tabSheetWizard.removeTab(${index})">
                            <i class="fas fa-times"></i>
                        </span>
                    </button>
                </li>
            `;

            contentHtml += `
                <div class="tab-pane fade ${index === 0 ? 'show active' : ''}" id="tab${index}">
                    <div class="p-3">
                        <div class="row g-3 mb-3">
                            <div class="col-md-6">
                                <label class="ts-label">Título da Aba</label>
                                <input type="text" class="form-control ts-input" 
                                       value="${tab.title}" 
                                       onchange="tabSheetWizard.updateTabProperty(${index}, 'title', this.value)">
                            </div>
                            <div class="col-md-6">
                                <label class="ts-label">Ícone</label>
                                <input type="text" class="form-control ts-input" 
                                       value="${tab.icon}"
                                       onchange="tabSheetWizard.updateTabProperty(${index}, 'icon', this.value)">
                            </div>
                        </div>
                        <div class="row g-3 mb-3">
                            <div class="col-12">
                                <label class="ts-label">Permissões</label>
                                <div class="d-flex gap-3">
                                    <div class="form-check">
                                        <input type="checkbox" class="form-check-input" 
                                               id="tabCreate${index}" ${tab.allowCreate ? 'checked' : ''}
                                               onchange="tabSheetWizard.updateTabProperty(${index}, 'allowCreate', this.checked)">
                                        <label class="form-check-label" for="tabCreate${index}">Criar</label>
                                    </div>
                                    <div class="form-check">
                                        <input type="checkbox" class="form-check-input" 
                                               id="tabEdit${index}" ${tab.allowEdit ? 'checked' : ''}
                                               onchange="tabSheetWizard.updateTabProperty(${index}, 'allowEdit', this.checked)">
                                        <label class="form-check-label" for="tabEdit${index}">Editar</label>
                                    </div>
                                    <div class="form-check">
                                        <input type="checkbox" class="form-check-input" 
                                               id="tabDelete${index}" ${tab.allowDelete ? 'checked' : ''}
                                               onchange="tabSheetWizard.updateTabProperty(${index}, 'allowDelete', this.checked)">
                                        <label class="form-check-label" for="tabDelete${index}">Excluir</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="ts-info-box">
                            <i class="fas fa-info-circle me-2"></i>
                            FK: <strong>${tab.fkColumn}</strong> → ${this.config.masterTable?.tableName}
                        </div>
                    </div>
                </div>
            `;
        });

        tabsHtml += '</ul>';
        contentHtml += '</div>';

        container.innerHTML = tabsHtml + contentHtml;
    }

    /**
     * Atualiza propriedade de uma aba
     */
    updateTabProperty(index, property, value) {
        if (this.config.tabs[index]) {
            this.config.tabs[index][property] = value;
            this.updateSummary();
        }
    }

    /**
     * Atualiza resumo da geração
     */
    updateSummary() {
        // Tabela mestre
        const summaryMaster = document.getElementById('summaryMasterTable');
        if (summaryMaster) {
            summaryMaster.textContent = this.config.masterTable?.tableName || '-';
        }

        const summaryListCols = document.getElementById('summaryMasterListCols');
        if (summaryListCols) {
            summaryListCols.textContent = this.config.masterTable?.listagem?.length || 0;
        }

        const summaryFormCols = document.getElementById('summaryMasterFormCols');
        if (summaryFormCols) {
            summaryFormCols.textContent = this.config.masterTable?.formulario?.length || 0;
        }

        // Abas detalhe
        const summaryTabs = document.getElementById('summaryDetailTabs');
        if (summaryTabs) {
            if (this.config.tabs.length === 0) {
                summaryTabs.innerHTML = '<p class="text-muted">Nenhuma aba selecionada</p>';
            } else {
                summaryTabs.innerHTML = this.config.tabs.map(tab => `
                    <div class="ts-summary-item">
                        <span><i class="${tab.icon} me-1"></i>${tab.title}</span>
                        <strong>${tab.tableName}</strong>
                    </div>
                `).join('');
            }
        }

        // Lista de arquivos
        const summaryFiles = document.getElementById('summaryFiles');
        if (summaryFiles) {
            summaryFiles.innerHTML = this.generateFileList();
        }
    }

    /**
     * Gera lista de arquivos que serão criados
     */
    generateFileList() {
        const files = [];
        const opts = this.config.options;
        const entityName = this.config.id || 'Entity';
        const plural = entityName + 's';

        // Backend
        if (opts.generateMasterEntity) {
            files.push({ name: `${entityName}.cs`, type: 'Entity', icon: 'fa-cube', color: 'primary' });
        }

        if (opts.generateDTOs) {
            files.push({ name: `${entityName}Dto.cs`, type: 'DTO', icon: 'fa-box', color: 'info' });
            files.push({ name: `Create${entityName}Request.cs`, type: 'DTO', icon: 'fa-box', color: 'info' });
            files.push({ name: `Update${entityName}Request.cs`, type: 'DTO', icon: 'fa-box', color: 'info' });
            files.push({ name: `${plural}ListViewModel.cs`, type: 'ViewModel', icon: 'fa-box', color: 'info' });
        }

        if (opts.generateServices) {
            files.push({ name: `I${entityName}ApiService.cs`, type: 'Interface', icon: 'fa-cog', color: 'success' });
            files.push({ name: `${entityName}ApiService.cs`, type: 'Service', icon: 'fa-cog', color: 'success' });
        }

        if (opts.generateController) {
            files.push({ name: `${plural}Controller.cs`, type: 'Controller', icon: 'fa-server', color: 'warning' });
        }

        // Frontend
        if (opts.generateMasterView) {
            files.push({ name: 'Index.cshtml', type: 'View', icon: 'fa-file-code', color: 'danger' });
        }

        if (opts.generateJavaScript) {
            files.push({ name: `${entityName.toLowerCase()}.js`, type: 'JavaScript', icon: 'fa-js', color: 'warning' });
        }

        // Abas
        if (opts.generateTabPartials && this.config.tabs.length > 0) {
            this.config.tabs.forEach(tab => {
                const tabName = this.toPascalCase(tab.tableName);
                files.push({ name: `_Tab${tabName}.cshtml`, type: 'Partial', icon: 'fa-file-code', color: 'secondary' });

                if (opts.generateDetailEntities) {
                    files.push({ name: `${tabName}Dto.cs`, type: 'DTO', icon: 'fa-box', color: 'info' });
                }
            });
        }

        return files.map(f => `
            <div class="ts-file-item">
                <i class="fas ${f.icon} text-${f.color} me-2"></i>
                <span class="ts-file-name">${f.name}</span>
                <span class="badge bg-${f.color} ms-auto">${f.type}</span>
            </div>
        `).join('');
    }

    // =========================================================================
    // NAVEGAÇÃO DO WIZARD
    // =========================================================================

    nextStep() {
        if (this.validateStep(this.currentStep)) {
            if (this.currentStep < this.totalSteps) {
                this.goToStep(this.currentStep + 1);
            }
        }
    }

    prevStep() {
        if (this.currentStep > 1) {
            this.goToStep(this.currentStep - 1);
        }
    }

    goToStep(step) {
        // Esconde step atual
        document.querySelector(`.ts-step-content[data-step="${this.currentStep}"]`)?.classList.remove('active');
        document.querySelector(`.ts-step[data-step="${this.currentStep}"]`)?.classList.remove('active');

        // Mostra novo step
        this.currentStep = step;
        document.querySelector(`.ts-step-content[data-step="${step}"]`)?.classList.add('active');
        document.querySelector(`.ts-step[data-step="${step}"]`)?.classList.add('active');

        // Atualiza steps completados
        for (let i = 1; i < step; i++) {
            document.querySelector(`.ts-step[data-step="${i}"]`)?.classList.add('completed');
        }

        // Atualiza botões
        this.updateNavigationButtons();

        // Atualiza resumo no último step
        if (step === this.totalSteps) {
            this.updateSummary();
        }
    }

    updateNavigationButtons() {
        const btnPrev = document.getElementById('btnPrev');
        const btnNext = document.getElementById('btnNext');
        const btnGenerate = document.getElementById('btnGenerate');

        if (btnPrev) {
            btnPrev.style.display = this.currentStep > 1 ? '' : 'none';
        }

        if (btnNext) {
            btnNext.style.display = this.currentStep < this.totalSteps ? '' : 'none';
        }

        if (btnGenerate) {
            btnGenerate.style.display = this.currentStep === this.totalSteps ? '' : 'none';
        }
    }

    validateStep(step) {
        switch (step) {
            case 1:
                if (!this.config.id) {
                    this.showError('Informe o ID da configuração');
                    return false;
                }
                if (!this.config.title) {
                    this.showError('Informe o título da tela');
                    return false;
                }
                if (!this.config.masterTable?.tableName) {
                    this.showError('Selecione a tabela mestre');
                    return false;
                }
                return true;

            case 2:
                if (!this.config.masterTable?.listagem?.length) {
                    this.showError('Selecione pelo menos uma coluna para listagem');
                    return false;
                }
                return true;

            default:
                return true;
        }
    }

    // =========================================================================
    // GERAÇÃO
    // =========================================================================

    async generate() {
        try {
            this.showLoading('Gerando arquivos...');

            // Monta payload
            const payload = {
                id: this.config.id,
                title: this.config.title,
                description: this.config.description,
                module: this.config.module,
                masterTable: {
                    tableName: this.config.masterTable.tableName,
                    primaryKey: this.config.masterTable.primaryKey,
                    listagem: this.config.masterTable.listagem,
                    formulario: this.config.masterTable.formulario,
                    foreignKeys: this.config.masterTable.foreignKeys
                },
                tabs: this.config.tabs.map(t => ({
                    tableName: t.tableName,
                    title: t.title,
                    icon: t.icon,
                    order: t.order,
                    fkColumn: t.fkColumn,
                    allowCreate: t.allowCreate,
                    allowEdit: t.allowEdit,
                    allowDelete: t.allowDelete,
                    listagem: t.listagem,
                    formulario: t.formulario
                })),
                options: {
                    generateMasterEntity: this.config.options.generateMasterEntity,
                    generateDetailEntities: this.config.options.generateDetailEntities,
                    generateDTOs: this.config.options.generateDTOs,
                    generateServices: this.config.options.generateServices,
                    generateNavigations: this.config.options.generateNavigations,
                    generateController: this.config.options.generateController,
                    generateMasterView: this.config.options.generateMasterView,
                    generateTabPartials: this.config.options.generateTabPartials,
                    generateJavaScript: this.config.options.generateJavaScript,
                    useDataTables: this.config.options.useDataTables,
                    moduleRoute: this.config.options.moduleRoute || this.config.module?.toLowerCase(),
                    icon: this.config.options.icon,
                    cdFuncao: this.config.options.cdFuncao,
                    cdSistema: this.config.options.cdSistema
                }
            };

            console.log('📤 Enviando configuração:', payload);

            const response = await fetch('/api/tabsheet/generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || 'Erro ao gerar arquivos');
            }

            // Download do ZIP
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `${this.config.id}.zip`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();

            this.hideLoading();
            this.showSuccess('Arquivos gerados com sucesso!');

            console.log('✅ Geração concluída');

        } catch (error) {
            console.error('❌ Erro na geração:', error);
            this.hideLoading();
            this.showError(error.message || 'Erro ao gerar arquivos');
        }
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    toPascalCase(str) {
        if (!str) return '';
        return str.split(/[_-]/)
            .map(s => s.charAt(0).toUpperCase() + s.slice(1).toLowerCase())
            .join('');
    }

    formatDisplayName(str) {
        if (!str) return '';
        return this.toPascalCase(str)
            .replace(/([a-z])([A-Z])/g, '$1 $2')
            .replace(/^Cd /, 'Código ')
            .replace(/^Dt /, 'Data ')
            .replace(/^Nr /, 'Número ')
            .replace(/^Nm /, 'Nome ')
            .replace(/^Vl /, 'Valor ');
    }

    getDefaultFormat(colMeta) {
        if (!colMeta) return 'text';
        const type = colMeta.type?.toLowerCase() || '';
        if (type.includes('date')) return 'date';
        if (type.includes('decimal') || type.includes('money')) return 'currency';
        if (type.includes('bit')) return 'boolean';
        return 'text';
    }

    getDefaultInputType(colMeta) {
        if (!colMeta) return 'text';
        const type = colMeta.type?.toLowerCase() || '';
        if (type.includes('date')) return type.includes('time') ? 'datetime-local' : 'date';
        if (type.includes('bit')) return 'checkbox';
        if (type.includes('int') || type.includes('decimal') || type.includes('numeric')) return 'number';
        if (colMeta.maxLength && colMeta.maxLength > 255) return 'textarea';
        return 'text';
    }

    showLoading(message) {
        // Implementar loading overlay
        console.log('⏳', message);
    }

    hideLoading() {
        // Esconder loading overlay
    }

    showError(message) {
        // Toast ou alert de erro
        alert('❌ ' + message);
    }

    showSuccess(message) {
        // Toast ou alert de sucesso
        alert('✅ ' + message);
    }
}

// =========================================================================
// INICIALIZAÇÃO GLOBAL
// =========================================================================

let tabSheetWizard;

document.addEventListener('DOMContentLoaded', function () {
    // Inicializa quando o modal é aberto
    const modal = document.getElementById('tabSheetModal');
    if (modal) {
        modal.addEventListener('shown.bs.modal', function () {
            if (!tabSheetWizard) {
                tabSheetWizard = new TabSheetWizard();
            }
        });
    } else {
        // Se não houver modal, inicializa direto
        tabSheetWizard = new TabSheetWizard();
    }
});

// Export para uso externo
window.TabSheetWizard = TabSheetWizard;