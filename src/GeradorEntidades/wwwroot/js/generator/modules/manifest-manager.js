/**
 * =============================================================================
 * MANIFEST MODULE v2.2 - CORREÇÃO CAMPOS DE NAVEGAÇÃO
 * Integração com Manifest Backend (Módulos → Entidades)
 * =============================================================================
 * Arquivo: wwwroot/js/generator/modules/manifest-manager.js
 * Projeto: GeradorEntidades / RhSensoERP
 * =============================================================================
 * CHANGELOG:
 * v2.2 - 🔧 CORREÇÃO: Preserva campos isReadOnly, list, form, filter, lookup do JSON v4.3
 *      - Campos de navegação (isReadOnly: true) agora aparecem na grid
 * v2.1 - Corrigido normalizeEntity para incluir route, cdFuncao, cdSistema, etc
 *      - Adicionado registro do módulo no final (App.registerModule)
 *      - Adicionado preview da rota da API
 * v2.0 - Versão inicial com seleção de módulos
 * =============================================================================
 */

const ManifestManager = {
    // =========================================================================
    // TEMPLATES PRÉ-CONFIGURADOS
    // =========================================================================
    templates: {
        'crud-basico': {
            name: 'CRUD Básico',
            description: 'Listagem + Create + Edit + Delete padrão',
            icon: '📝',
            config: {
                grid: { serverSide: false, pageSize: 10, exportFormats: ['excel'], bulkActions: false },
                form: { columns: 2, useTabs: false }
            }
        },
        'crud-avancado': {
            name: 'CRUD Avançado',
            description: 'Com filtros, exportação e ações em lote',
            icon: '🚀',
            config: {
                grid: { serverSide: true, pageSize: 25, exportFormats: ['excel', 'pdf', 'csv'], bulkActions: true },
                form: { columns: 2, useTabs: false }
            }
        },
        'cadastro-completo': {
            name: 'Cadastro Completo',
            description: 'Formulário com abas e validações completas',
            icon: '📋',
            config: {
                grid: { serverSide: true, pageSize: 25, exportFormats: ['excel', 'pdf'], bulkActions: true },
                form: { columns: 2, useTabs: true, tabs: ['Dados Gerais', 'Informações Adicionais', 'Observações'] }
            }
        },
        'relatorio': {
            name: 'Relatório/Consulta',
            description: 'Foco em filtros e exportação, sem edição',
            icon: '📊',
            config: {
                grid: { serverSide: true, pageSize: 50, exportFormats: ['excel', 'pdf', 'csv', 'print'], bulkActions: false },
                form: { columns: 1, useTabs: false },
                readOnly: true
            }
        }
    },

    // =========================================================================
    // ESTADO
    // =========================================================================
    baseUrl: '',
    modules: [],
    entities: [],
    selectedModule: null,
    selectedEntity: null,

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================
    init() {
        console.log('📡 Manifest Manager v2.2 initialized');
    },

    render() {
        console.log('📡 ManifestManager.render() chamado');
        this.renderManifestLoader();
        this.renderTemplates();
        this.renderProjectExport();
    },

    // =========================================================================
    // MANIFEST LOADER (Módulos → Entidades)
    // =========================================================================
    renderManifestLoader() {
        const container = document.getElementById('manifestLoader');
        if (!container) {
            console.warn('⚠️ Elemento #manifestLoader não encontrado');
            return;
        }

        console.log('📡 Renderizando ManifestLoader...');

        container.innerHTML = `
            <div class="manifest-section" style="background: #f8f9fa; border-radius: 8px; padding: 20px; margin-bottom: 20px;">
                <h4 style="margin-top: 0; color: #333;">📡 Carregar do Manifest (Backend)</h4>
                <p style="color: #666; margin-bottom: 15px;">Carregue a entidade diretamente da API do backend</p>
                
                <!-- URL Base -->
                <div class="manifest-row" style="display: flex; align-items: center; gap: 10px; margin-bottom: 15px;">
                    <label style="min-width: 120px; font-weight: 500;">URL Base da API:</label>
                    <div style="display: flex; gap: 10px; flex: 1;">
                        <input type="text" id="manifestUrl" 
                               style="flex: 1; padding: 8px 12px; border: 1px solid #ddd; border-radius: 4px;"
                               placeholder="https://localhost:7193"
                               value="${this.getLastManifestUrl()}">
                        <button class="btn btn-primary" onclick="ManifestManager.loadModules()" 
                                style="padding: 8px 16px; background: #4CAF50; color: white; border: none; border-radius: 4px; cursor: pointer;">
                            🔄 Carregar Módulos
                        </button>
                    </div>
                </div>

                <!-- Seleção de Módulo -->
                <div id="moduleSelector" class="manifest-row" style="display: none; align-items: center; gap: 10px; margin-bottom: 15px;">
                    <label style="min-width: 120px; font-weight: 500;">📦 Módulo:</label>
                    <div style="display: flex; gap: 10px; flex: 1; align-items: center;">
                        <select id="moduleSelect" onchange="ManifestManager.onModuleChange(this.value)"
                                style="flex: 1; padding: 8px 12px; border: 1px solid #ddd; border-radius: 4px;">
                            <option value="">Selecione um módulo...</option>
                        </select>
                        <span id="moduleCount" style="background: #2196F3; color: white; padding: 4px 8px; border-radius: 4px; font-size: 12px;"></span>
                    </div>
                </div>

                <!-- Seleção de Entidade -->
                <div id="entitySelector" class="manifest-row" style="display: none; align-items: center; gap: 10px; margin-bottom: 15px;">
                    <label style="min-width: 120px; font-weight: 500;">📋 Entidade:</label>
                    <div style="display: flex; gap: 10px; flex: 1; align-items: center;">
                        <select id="entitySelect" onchange="ManifestManager.onEntityChange(this.value)"
                                style="flex: 1; padding: 8px 12px; border: 1px solid #ddd; border-radius: 4px;">
                            <option value="">Selecione uma entidade...</option>
                        </select>
                        <span id="entityCount" style="background: #2196F3; color: white; padding: 4px 8px; border-radius: 4px; font-size: 12px;"></span>
                    </div>
                </div>

                <!-- Preview da Entidade -->
                <div id="entityPreview" style="display: none; background: white; border: 1px solid #ddd; border-radius: 8px; padding: 15px; margin-top: 15px;">
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                        <span id="previewEntityName" style="font-size: 18px; font-weight: bold; color: #333;"></span>
                        <span id="previewTableName" style="background: #607D8B; color: white; padding: 4px 8px; border-radius: 4px; font-size: 12px;"></span>
                    </div>
                    <div id="previewRoute" style="margin-bottom: 10px;"></div>
                    <div id="previewMeta" style="margin-bottom: 10px; font-size: 13px; color: #666;"></div>
                    <div id="previewProperties" style="max-height: 200px; overflow-y: auto; border-top: 1px solid #eee; padding-top: 10px;"></div>
                    <button class="btn btn-success" onclick="ManifestManager.useSelectedEntity()" 
                            style="margin-top: 15px; padding: 10px 20px; background: #4CAF50; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 14px;">
                        ✅ Usar Esta Entidade
                    </button>
                </div>

                <!-- Status -->
                <div id="manifestStatus" style="display: none; margin-top: 15px; padding: 10px; border-radius: 4px;"></div>
            </div>
        `;
    },

    getLastManifestUrl() {
        return localStorage.getItem('lastManifestUrl') || 'https://localhost:7193';
    },

    // =========================================================================
    // CARREGAR MÓDULOS
    // =========================================================================
    async loadModules() {
        const urlInput = document.getElementById('manifestUrl');
        this.baseUrl = urlInput?.value?.trim().replace(/\/$/, '') || '';

        if (!this.baseUrl) {
            this.showStatus('⚠️ Informe a URL base da API', 'warning');
            return;
        }

        localStorage.setItem('lastManifestUrl', this.baseUrl);
        this.showStatus('🔄 Carregando módulos...', 'info');

        try {
            const response = await fetch(`${this.baseUrl}/api/manifest/modules`, {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const data = await response.json();

            // Normaliza resposta (pode ser array direto ou { modules: [...] })
            this.modules = Array.isArray(data) ? data : (data.modules || data.Modules || []);

            this.populateModuleSelect();
            this.showStatus(`✅ ${this.modules.length} módulos encontrados`, 'success');

        } catch (error) {
            console.error('Erro ao carregar módulos:', error);
            this.showStatus(`❌ Erro: ${error.message}. Verifique CORS e se a API está rodando.`, 'error');
        }
    },

    populateModuleSelect() {
        const select = document.getElementById('moduleSelect');
        const container = document.getElementById('moduleSelector');
        const countBadge = document.getElementById('moduleCount');

        if (!select || !container) return;

        select.innerHTML = '<option value="">Selecione um módulo...</option>';

        this.modules.forEach(mod => {
            const name = mod.moduleName || mod.ModuleName || mod.name || mod.Name || mod;
            const entityCount = mod.entityCount || mod.EntityCount || mod.entities?.length || '';
            select.innerHTML += `<option value="${name}">${name}${entityCount ? ` (${entityCount})` : ''}</option>`;
        });

        container.style.display = 'flex';
        countBadge.textContent = `${this.modules.length} módulos`;

        document.getElementById('entitySelector').style.display = 'none';
        document.getElementById('entityPreview').style.display = 'none';
    },

    // =========================================================================
    // CARREGAR ENTIDADES DO MÓDULO
    // =========================================================================
    async onModuleChange(moduleName) {
        if (!moduleName) {
            document.getElementById('entitySelector').style.display = 'none';
            document.getElementById('entityPreview').style.display = 'none';
            return;
        }

        this.selectedModule = moduleName;
        this.showStatus(`🔄 Carregando entidades de ${moduleName}...`, 'info');

        try {
            const response = await fetch(`${this.baseUrl}/api/manifest/modules/${moduleName}`, {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const data = await response.json();

            this.entities = Array.isArray(data) ? data : (data.entities || data.Entities || []);

            this.populateEntitySelect();
            this.showStatus(`✅ ${this.entities.length} entidades em ${moduleName}`, 'success');

        } catch (error) {
            console.error('Erro ao carregar entidades:', error);
            this.showStatus(`❌ Erro: ${error.message}`, 'error');
        }
    },

    populateEntitySelect() {
        const select = document.getElementById('entitySelect');
        const container = document.getElementById('entitySelector');
        const countBadge = document.getElementById('entityCount');

        if (!select || !container) return;

        select.innerHTML = '<option value="">Selecione uma entidade...</option>';

        this.entities.forEach((entity, idx) => {
            const name = entity.entityName || entity.EntityName || entity.name || entity.Name;
            const tableName = entity.tableName || entity.TableName || '';
            select.innerHTML += `<option value="${idx}">${name}${tableName ? ` [${tableName}]` : ''}</option>`;
        });

        container.style.display = 'flex';
        countBadge.textContent = `${this.entities.length} entidades`;

        document.getElementById('entityPreview').style.display = 'none';
    },

    // =========================================================================
    // SELECIONAR ENTIDADE
    // =========================================================================
    async onEntityChange(index) {
        if (index === '' || index === undefined) {
            document.getElementById('entityPreview').style.display = 'none';
            return;
        }

        const entity = this.entities[parseInt(index)];
        if (!entity) return;

        const entityName = entity.entityName || entity.EntityName || entity.name || entity.Name;

        // Se não tem properties, busca detalhes
        if (!entity.properties && !entity.Properties) {
            this.showStatus(`🔄 Carregando detalhes de ${entityName}...`, 'info');

            try {
                const response = await fetch(`${this.baseUrl}/api/manifest/entities/${entityName}`, {
                    method: 'GET',
                    headers: { 'Accept': 'application/json' }
                });

                if (!response.ok) throw new Error(`HTTP ${response.status}`);

                this.selectedEntity = await response.json();

            } catch (error) {
                console.error('Erro ao carregar detalhes:', error);
                this.showStatus(`❌ Erro: ${error.message}`, 'error');
                return;
            }
        } else {
            this.selectedEntity = entity;
        }

        this.renderEntityPreview();
        this.showStatus('', 'info');
    },

    renderEntityPreview() {
        const container = document.getElementById('entityPreview');
        const nameEl = document.getElementById('previewEntityName');
        const tableEl = document.getElementById('previewTableName');
        const routeEl = document.getElementById('previewRoute');
        const metaEl = document.getElementById('previewMeta');
        const propsEl = document.getElementById('previewProperties');

        if (!container || !this.selectedEntity) return;

        const entity = this.selectedEntity;
        const name = entity.entityName || entity.EntityName || entity.name || entity.Name;
        const table = entity.tableName || entity.TableName || entity.table || '';
        const route = entity.route || entity.Route || entity.apiRoute || entity.ApiRoute || '';
        const cdFuncao = entity.cdFuncao || entity.CdFuncao || '';
        const cdSistema = entity.cdSistema || entity.CdSistema || '';
        const props = entity.properties || entity.Properties || entity.columns || entity.Columns || [];

        nameEl.textContent = name;
        tableEl.textContent = table;

        // Mostra a rota da API
        if (routeEl) {
            routeEl.innerHTML = route
                ? `<code style="background: #e8f5e9; color: #2e7d32; padding: 4px 8px; border-radius: 4px;">🛤️ ${route}</code>`
                : '<span style="color: #ff9800;">⚠️ Sem rota definida</span>';
        }

        // Mostra metadados
        if (metaEl) {
            metaEl.innerHTML = `
                ${cdFuncao ? `<span style="margin-right: 15px;"><strong>CdFuncao:</strong> <code>${cdFuncao}</code></span>` : ''}
                ${cdSistema ? `<span><strong>CdSistema:</strong> <code>${cdSistema}</code></span>` : ''}
            `;
        }

        propsEl.innerHTML = props.slice(0, 10).map(prop => {
            const propName = prop.name || prop.Name;
            const propType = prop.type || prop.Type || prop.clrType || prop.ClrType || 'string';
            const nullable = prop.nullable ?? prop.Nullable ?? prop.isNullable ?? true;
            const isPK = prop.isPrimaryKey || prop.IsPrimaryKey;
            const isFK = prop.isForeignKey || prop.IsForeignKey;

            return `
                <div style="display: flex; gap: 10px; padding: 5px 0; border-bottom: 1px solid #f0f0f0;">
                    <span style="font-weight: 500; min-width: 150px;">${propName}</span>
                    <span style="color: #1976D2; min-width: 80px;">${propType}</span>
                    ${isPK ? '<span style="background: #FFC107; color: #333; padding: 2px 6px; border-radius: 3px; font-size: 11px;">PK</span>' : ''}
                    ${isFK ? '<span style="background: #9C27B0; color: white; padding: 2px 6px; border-radius: 3px; font-size: 11px;">FK</span>' : ''}
                    ${!nullable ? '<span style="background: #F44336; color: white; padding: 2px 6px; border-radius: 3px; font-size: 11px;">*</span>' : ''}
                </div>
            `;
        }).join('');

        if (props.length > 10) {
            propsEl.innerHTML += `<div style="color: #999; padding: 10px 0; text-align: center;">... e mais ${props.length - 10} propriedades</div>`;
        }

        container.style.display = 'block';
    },

    // =========================================================================
    // USAR ENTIDADE SELECIONADA
    // =========================================================================
    useSelectedEntity() {
        if (!this.selectedEntity) {
            this.showStatus('⚠️ Selecione uma entidade primeiro', 'warning');
            return;
        }

        // v2.2: Normaliza preservando TODOS os campos do JSON v4.3
        const normalized = this.normalizeEntity(this.selectedEntity);

        // Debug: mostra o que foi normalizado
        console.log('🔍 Entidade original:', this.selectedEntity);
        console.log('🔍 Entidade normalizada:', normalized);
        console.log('🛤️ Route:', normalized.route);
        console.log('📋 CdFuncao:', normalized.cdFuncao);
        console.log('🏢 CdSistema:', normalized.cdSistema);
        console.log('📊 Total de propriedades:', normalized.properties.length);

        const jsonInput = document.getElementById('jsonInput');
        if (jsonInput) {
            jsonInput.value = JSON.stringify(normalized, null, 2);
        }

        // Preenche campos de configuração automaticamente
        if (normalized.cdFuncao) {
            const cdFuncaoInput = document.getElementById('cdFuncao');
            if (cdFuncaoInput) cdFuncaoInput.value = normalized.cdFuncao;
        }

        if (normalized.cdSistema) {
            const cdSistemaSelect = document.getElementById('cdSistema');
            if (cdSistemaSelect) {
                // Tenta encontrar a opção correspondente
                const option = Array.from(cdSistemaSelect.options).find(o => o.value === normalized.cdSistema);
                if (option) cdSistemaSelect.value = normalized.cdSistema;
            }
        }

        if (normalized.displayName) {
            const displayNameInput = document.getElementById('displayName');
            if (displayNameInput) displayNameInput.value = normalized.displayName;
        }

        if (normalized.icon) {
            const iconInput = document.getElementById('iconClass');
            if (iconInput) iconInput.value = normalized.icon;
        }

        Store.set('entity', normalized);

        const name = normalized.entityName;
        this.showStatus(`✅ Entidade "${name}" carregada! Avançando...`, 'success');

        setTimeout(() => {
            if (typeof Wizard !== 'undefined') {
                Wizard.goToStep(1);
            }
        }, 800);
    },

    // =========================================================================
    // v2.2 🔧 CORRIGIDO: PRESERVA TODOS OS CAMPOS DO JSON v4.3
    // =========================================================================
    normalizeEntity(entity) {
        const props = entity.properties || entity.Properties || entity.columns || entity.Columns || [];

        return {
            // Identificação
            entityName: entity.entityName || entity.EntityName || entity.name || entity.Name,
            fullName: entity.fullName || entity.FullName || '',
            displayName: entity.displayName || entity.DisplayName || entity.entityName || entity.name || '',
            tableName: entity.tableName || entity.TableName || entity.table || entity.Table || '',
            schema: entity.schema || entity.Schema || 'dbo',

            // Módulo
            moduleName: entity.moduleName || entity.ModuleName || '',
            moduleDisplayName: entity.moduleDisplayName || entity.ModuleDisplayName || '',

            // Rota da API
            route: entity.route || entity.Route || entity.apiRoute || entity.ApiRoute || '',

            // Permissões
            cdSistema: entity.cdSistema || entity.CdSistema || 'RHU',
            cdFuncao: entity.cdFuncao || entity.CdFuncao || '',

            // Configurações
            icon: entity.icon || entity.Icon || 'fas fa-table',
            requiresAuth: entity.requiresAuth ?? entity.RequiresAuth ?? true,
            supportsBatchDelete: entity.supportsBatchDelete ?? entity.SupportsBatchDelete ?? true,
            usePluralRoute: entity.usePluralRoute ?? entity.UsePluralRoute ?? false,

            // Primary Key
            primaryKeyName: entity.primaryKeyName || entity.PrimaryKeyName || 'Id',
            primaryKeyType: entity.primaryKeyType || entity.PrimaryKeyType || 'int',
            primaryKeyIsIdentity: entity.primaryKeyIsIdentity ?? entity.PrimaryKeyIsIdentity ?? true,

            // 🔧 PROPRIEDADES - PRESERVA TODOS OS CAMPOS DO v4.3
            properties: props.map(prop => {
                const normalized = {
                    // Campos básicos
                    name: prop.name || prop.Name,
                    type: this.normalizeType(prop.type || prop.Type || prop.clrType || prop.ClrType),
                    displayName: prop.displayName || prop.DisplayName || prop.name || prop.Name,
                    columnName: prop.columnName || prop.ColumnName || prop.name || prop.Name,
                    inputType: prop.inputType || prop.InputType || 'text',
                    isNullable: prop.isNullable ?? prop.IsNullable ?? prop.nullable ?? prop.Nullable ?? true,
                    isPrimaryKey: prop.isPrimaryKey || prop.IsPrimaryKey || false,
                    isRequired: prop.isRequired ?? prop.IsRequired ?? !(prop.isNullable ?? true),
                    isIdentity: prop.isIdentity || prop.IsIdentity || false,
                    isForeignKey: prop.isForeignKey || prop.IsForeignKey || false,
                    foreignKeyEntity: prop.foreignKeyEntity || prop.ForeignKeyEntity || null,
                    maxLength: prop.maxLength || prop.MaxLength || null,

                    // 🔧 CAMPOS CRÍTICOS DO v4.3 (eram ignorados antes!)
                    isReadOnly: prop.isReadOnly ?? prop.IsReadOnly ?? false,
                    excludeFromDto: prop.excludeFromDto ?? prop.ExcludeFromDto ?? false,

                    // 🔧 CONFIGURAÇÕES COMPLETAS DO v4.3
                    list: prop.list || prop.List || null,
                    form: prop.form || prop.Form || null,
                    filter: prop.filter || prop.Filter || null,
                    lookup: prop.lookup || prop.Lookup || null
                };

                // Se não tem configurações v4.3, cria defaults básicos
                if (!normalized.list) {
                    normalized.list = {
                        show: !normalized.isPrimaryKey,
                        order: 0,
                        sortable: true,
                        filterable: normalized.type.toLowerCase() === 'string',
                        format: this.getDefaultFormat(normalized.type),
                        align: this.getDefaultAlign(normalized.type)
                    };
                }

                if (!normalized.form) {
                    normalized.form = {
                        show: !normalized.isPrimaryKey,
                        showOnCreate: !normalized.isPrimaryKey,
                        showOnEdit: !normalized.isPrimaryKey,
                        inputType: normalized.inputType,
                        disabled: normalized.isReadOnly,
                        colSize: 6
                    };
                }

                if (!normalized.filter) {
                    normalized.filter = {
                        show: normalized.type.toLowerCase() === 'string',
                        filterType: this.getFilterType(normalized.type),
                        defaultOperator: normalized.type.toLowerCase() === 'string' ? 'contains' : 'equals'
                    };
                }

                return normalized;
            }),

            // Navegações (se existir)
            navigations: (entity.navigations || entity.Navigations || []).map(nav => ({
                name: nav.name || nav.Name,
                targetEntity: nav.targetEntity || nav.TargetEntity,
                relationType: nav.relationType || nav.RelationType || 'OneToMany',
                foreignKeyProperty: nav.foreignKeyProperty || nav.ForeignKeyProperty || '',
                isCollection: nav.isCollection ?? nav.IsCollection ?? true
            }))
        };
    },

    // =========================================================================
    // 🔧 HELPERS ADICIONADOS/ATUALIZADOS
    // =========================================================================
    normalizeType(type) {
        if (!type) return 'string';

        const typeStr = type.toString().toLowerCase();

        if (typeStr.includes('int32') || typeStr.includes('int64') || typeStr === 'int' || typeStr === 'long') return 'int';
        if (typeStr.includes('decimal') || typeStr.includes('double') || typeStr.includes('float')) return 'decimal';
        if (typeStr.includes('bool')) return 'bool';
        if (typeStr.includes('datetime')) return 'DateTime';
        if (typeStr.includes('dateonly')) return 'DateOnly';
        if (typeStr.includes('date')) return 'date';
        if (typeStr.includes('guid')) return 'Guid';
        if (typeStr.includes('byte[]')) return 'byte[]';

        return 'string';
    },

    getDefaultFormat(type) {
        const formatMap = {
            'datetime': 'datetime',
            'DateTime': 'datetime',
            'date': 'date',
            'DateOnly': 'date',
            'decimal': 'number',
            'bool': 'boolean',
            'boolean': 'boolean'
        };
        return formatMap[type] || 'text';
    },

    getDefaultAlign(type) {
        const alignMap = {
            'int': 'right',
            'long': 'right',
            'decimal': 'right',
            'float': 'right',
            'double': 'right'
        };
        return alignMap[type?.toLowerCase()] || 'left';
    },

    getFilterType(type) {
        const typeStr = type?.toLowerCase() || '';
        if (typeStr.includes('int') || typeStr.includes('decimal')) return 'number';
        if (typeStr.includes('date')) return 'date';
        if (typeStr.includes('bool')) return 'boolean';
        return 'text';
    },

    // =========================================================================
    // TEMPLATES
    // =========================================================================
    renderTemplates() {
        const container = document.getElementById('templateSelector');
        if (!container) return;

        container.innerHTML = `
            <div class="templates-section" style="background: #f8f9fa; border-radius: 8px; padding: 20px; margin-bottom: 20px;">
                <h4 style="margin-top: 0;">📦 Templates Pré-configurados</h4>
                <p style="color: #666;">Selecione um template para aplicar configurações automáticas</p>
                
                <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 15px; margin-top: 15px;">
                    ${Object.entries(this.templates).map(([key, tpl]) => `
                        <div class="template-card" data-template="${key}" onclick="ManifestManager.applyTemplate('${key}')"
                             style="background: white; border: 2px solid #ddd; border-radius: 8px; padding: 15px; cursor: pointer; transition: all 0.2s;">
                            <div style="font-size: 24px; text-align: center;">${tpl.icon}</div>
                            <div style="font-weight: bold; text-align: center; margin: 10px 0;">${tpl.name}</div>
                            <div style="font-size: 12px; color: #666; text-align: center;">${tpl.description}</div>
                        </div>
                    `).join('')}
                </div>
            </div>
        `;
    },

    applyTemplate(templateKey) {
        const template = this.templates[templateKey];
        if (!template) return;

        if (template.config.grid && App.modules.GridConfig) {
            Object.assign(App.modules.GridConfig.config, template.config.grid);
            App.modules.GridConfig.save();
        }

        if (template.config.form && App.modules.FormDesigner) {
            Object.assign(App.modules.FormDesigner.layoutConfig, template.config.form);
            App.modules.FormDesigner.saveLayoutConfig();
        }

        document.querySelectorAll('.template-card').forEach(card => {
            card.style.borderColor = '#ddd';
            card.style.background = 'white';
        });
        const selected = document.querySelector(`[data-template="${templateKey}"]`);
        if (selected) {
            selected.style.borderColor = '#4CAF50';
            selected.style.background = '#e8f5e9';
        }

        this.showStatus(`✅ Template "${template.name}" aplicado!`, 'success');
    },

    // =========================================================================
    // PROJECT EXPORT/IMPORT
    // =========================================================================
    renderProjectExport() {
        const container = document.getElementById('projectExport');
        if (!container) return;

        container.innerHTML = `
            <div class="export-section" style="background: #f8f9fa; border-radius: 8px; padding: 20px;">
                <h4 style="margin-top: 0;">💾 Salvar/Carregar Projeto</h4>
                <p style="color: #666;">Exporte ou importe a configuração completa do projeto</p>
                
                <div style="display: flex; gap: 10px; margin-top: 15px;">
                    <button class="btn btn-secondary" onclick="ManifestManager.exportProject()"
                            style="padding: 10px 20px; background: #607D8B; color: white; border: none; border-radius: 4px; cursor: pointer;">
                        📤 Exportar Configuração
                    </button>
                    <label style="padding: 10px 20px; background: #607D8B; color: white; border: none; border-radius: 4px; cursor: pointer;">
                        📥 Importar Configuração
                        <input type="file" id="importProjectFile" accept=".json" 
                               onchange="ManifestManager.importProject(this)" style="display: none;">
                    </label>
                </div>
            </div>
        `;
    },

    exportProject() {
        const project = {
            version: '2.2',
            exportedAt: new Date().toISOString(),
            entity: Store.get('entity'),
            gridConfig: App.modules.GridConfig?.config,
            formConfig: {
                layoutConfig: App.modules.FormDesigner?.layoutConfig,
                formFields: Store.get('formFields')
            }
        };

        const blob = new Blob([JSON.stringify(project, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = `projeto-${project.entity?.entityName || 'sem-nome'}.json`;
        a.click();

        URL.revokeObjectURL(url);
        this.showStatus('✅ Projeto exportado!', 'success');
    },

    importProject(input) {
        const file = input.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = (e) => {
            try {
                const project = JSON.parse(e.target.result);

                if (project.entity) {
                    Store.set('entity', project.entity);
                    const jsonInput = document.getElementById('jsonInput');
                    if (jsonInput) {
                        jsonInput.value = JSON.stringify(project.entity, null, 2);
                    }
                }

                if (project.gridConfig && App.modules.GridConfig) {
                    App.modules.GridConfig.config = project.gridConfig;
                    App.modules.GridConfig.save();
                }

                if (project.formConfig) {
                    if (App.modules.FormDesigner && project.formConfig.layoutConfig) {
                        App.modules.FormDesigner.layoutConfig = project.formConfig.layoutConfig;
                        App.modules.FormDesigner.saveLayoutConfig();
                    }
                    if (project.formConfig.formFields) {
                        Store.set('formFields', project.formConfig.formFields);
                    }
                }

                this.showStatus(`✅ Projeto "${project.entity?.entityName}" importado!`, 'success');

            } catch (error) {
                this.showStatus(`❌ Erro ao importar: ${error.message}`, 'error');
            }
        };

        reader.readAsText(file);
        input.value = '';
    },

    // =========================================================================
    // UTILITÁRIOS
    // =========================================================================
    showStatus(message, type = 'info') {
        const status = document.getElementById('manifestStatus');
        if (!status) return;

        if (!message) {
            status.style.display = 'none';
            return;
        }

        const colors = {
            'info': { bg: '#e3f2fd', text: '#1976D2' },
            'success': { bg: '#e8f5e9', text: '#2e7d32' },
            'warning': { bg: '#fff3e0', text: '#ef6c00' },
            'error': { bg: '#ffebee', text: '#c62828' }
        };

        const color = colors[type] || colors.info;

        status.innerHTML = `<span>${message}</span>`;
        status.style.display = 'block';
        status.style.background = color.bg;
        status.style.color = color.text;
        status.style.padding = '10px 15px';
        status.style.borderRadius = '4px';

        if (type === 'success' || type === 'info') {
            setTimeout(() => { status.style.display = 'none'; }, 5000);
        }
    }
};

// =============================================================================
// ⭐ REGISTRO DO MÓDULO (CRÍTICO!)
// =============================================================================
App.registerModule('ManifestManager', ManifestManager);
window.ManifestManager = ManifestManager;

console.log('✅ ManifestManager v2.2 carregado e registrado');