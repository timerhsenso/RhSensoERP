/**
 * =============================================================================
 * GRID CONFIG MODULE v1.5 FIXED
 * Configuração avançada de colunas, filtros e exportação
 * =============================================================================
 * CHANGELOG v1.5 FIXED:
 * - 🔧 REMOVIDO: Scroll na lista de colunas
 * - ✅ CORRIGIDO: Todas as colunas visíveis sem container com altura fixa
 * - 📐 Layout: Lista completa sem overflow
 * =============================================================================
 * CHANGELOG v1.4:
 * - 🔧 CORREÇÃO: Mostra TODOS os campos (incluindo Auditoria) para o usuário escolher
 * - Campos audit e PK vêm desmarcados por padrão (exceto se JSON mandar diferente)
 * =============================================================================
 */

const GridConfig = {
    // =========================================================================
    // CAMPOS DE AUDITORIA (nunca aparecem na grid)
    // =========================================================================
    AUDIT_FIELDS: [
        // Multi-tenancy
        'idsaas', 'id_saas',
        // Data de criação
        'datacriacao', 'dtcriacao', 'createdat', 'dtinclusao', 'datainclusao',
        'dt_criacao', 'data_criacao', 'dt_inclusao', 'data_inclusao',
        // Data de alteração
        'dataalteracao', 'dtalteracao', 'updatedat', 'modifiedat', 'dtatualizacao',
        'dt_alteracao', 'data_alteracao', 'dt_atualizacao', 'data_atualizacao',
        // Usuário de criação
        'usuariocriacao', 'criadopor', 'createdby', 'idusuariocriacao',
        'usuario_criacao', 'criado_por', 'created_by', 'id_usuario_criacao',
        // Usuário de alteração
        'usuarioalteracao', 'alteradopor', 'updatedby', 'idusuarioalteracao',
        'usuario_alteracao', 'alterado_por', 'updated_by', 'id_usuario_alteracao',
        // Outros campos de sistema
        'rowversion', 'timestamp', 'version'
    ],

    // =========================================================================
    // CONFIGURAÇÃO PADRÃO
    // =========================================================================
    config: {
        serverSide: false,
        pageSize: 10,
        exportFormats: ['excel', 'pdf', 'csv'],
        bulkActions: false,
        columns: [],
        filters: [],
        _entityName: null,
        _auditFieldsCount: 0,
        _configVersion: 1.5 // ✅ v1.5 - Sem scroll
    },

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================
    init() {
        console.log('📊 Grid Config v1.5 FIXED initialized (sem scroll)');

        const saved = localStorage.getItem('gridConfig');
        if (saved) {
            try {
                const parsed = JSON.parse(saved);
                // Cache bust if version is different
                if (parsed._configVersion !== this.config._configVersion) {
                    console.log('⚠️ Old config detected. Clearing cache to apply new column rules.');
                    localStorage.removeItem('gridConfig');
                } else {
                    this.config = { ...this.config, ...parsed };
                }
            } catch (e) { }
        }
    },

    save() {
        localStorage.setItem('gridConfig', JSON.stringify(this.config));
    },

    // =========================================================================
    // VERIFICA SE É CAMPO DE AUDITORIA
    // =========================================================================
    isAuditField(fieldName) {
        if (!fieldName) return false;
        const normalized = fieldName.toLowerCase().replace(/[_\-\s]/g, '');
        return this.AUDIT_FIELDS.some(audit => normalized === audit.replace(/[_\-\s]/g, ''));
    },

    // =========================================================================
    // OBTER PROPRIEDADES PARA GRID
    // v1.4: Retorna TODAS as colunas (inclusive Auditoria) para o usuário decidir
    // =========================================================================
    getGridProperties(entity) {
        if (!entity || !entity.properties) return [];
        return entity.properties;
    },

    // =========================================================================
    // RENDERIZAÇÃO
    // =========================================================================
    render() {
        const entity = Store.get('entity');
        if (!entity) return;

        this.renderGeneralOptions();
        this.renderColumnConfig();
        this.renderFilterConfig();
    },

    // =========================================================================
    // OPÇÕES GERAIS
    // =========================================================================
    renderGeneralOptions() {
        const container = document.getElementById('gridGeneralOptions');
        if (!container) return;

        container.innerHTML = `
            <div class="grid-options-panel">
                <h4>⚙️ Opções Gerais</h4>
                <div class="options-row">
                    <div class="option-item">
                        <label>
                            <input type="checkbox" id="serverSide" 
                                   ${this.config.serverSide ? 'checked' : ''}
                                   onchange="GridConfig.updateOption('serverSide', this.checked)">
                            Paginação Server-Side
                        </label>
                        <small>Recomendado para grandes volumes</small>
                    </div>
                    <div class="option-item">
                        <label>Registros por Página:</label>
                        <select onchange="GridConfig.updateOption('pageSize', parseInt(this.value))">
                            <option value="10" ${this.config.pageSize === 10 ? 'selected' : ''}>10</option>
                            <option value="25" ${this.config.pageSize === 25 ? 'selected' : ''}>25</option>
                            <option value="50" ${this.config.pageSize === 50 ? 'selected' : ''}>50</option>
                            <option value="100" ${this.config.pageSize === 100 ? 'selected' : ''}>100</option>
                        </select>
                    </div>
                    <div class="option-item">
                        <label>
                            <input type="checkbox" id="bulkActions" 
                                   ${this.config.bulkActions ? 'checked' : ''}
                                   onchange="GridConfig.updateOption('bulkActions', this.checked)">
                            Ações em Lote
                        </label>
                        <small>Checkbox para seleção múltipla</small>
                    </div>
                </div>
                <div class="options-row">
                    <div class="option-item">
                        <label>Formatos de Exportação:</label>
                        <div class="export-options">
                            <label>
                                <input type="checkbox" value="excel" 
                                       ${this.config.exportFormats.includes('excel') ? 'checked' : ''}
                                       onchange="GridConfig.toggleExport('excel', this.checked)">
                                Excel
                            </label>
                            <label>
                                <input type="checkbox" value="pdf" 
                                       ${this.config.exportFormats.includes('pdf') ? 'checked' : ''}
                                       onchange="GridConfig.toggleExport('pdf', this.checked)">
                                PDF
                            </label>
                            <label>
                                <input type="checkbox" value="csv" 
                                       ${this.config.exportFormats.includes('csv') ? 'checked' : ''}
                                       onchange="GridConfig.toggleExport('csv', this.checked)">
                                CSV
                            </label>
                            <label>
                                <input type="checkbox" value="print" 
                                       ${this.config.exportFormats.includes('print') ? 'checked' : ''}
                                       onchange="GridConfig.toggleExport('print', this.checked)">
                                Imprimir
                            </label>
                        </div>
                    </div>
                </div>
            </div>
        `;
    },

    // =========================================================================
    // CONFIGURAÇÃO DE COLUNAS - v1.5 FIXED: SEM SCROLL
    // =========================================================================
    renderColumnConfig() {
        const entity = Store.get('entity');
        const container = document.getElementById('columnConfig');
        if (!container || !entity) return;

        // Verifica se a entidade mudou
        const currentEntityName = entity.entityName;
        const savedEntityName = this.config._entityName;

        if (this.config.columns.length === 0 || savedEntityName !== currentEntityName) {
            this.config._entityName = currentEntityName;

            // v1.4: Pega TODAS as propriedades
            const gridProps = this.getGridProperties(entity);

            // Mapeia colunas
            this.config.columns = gridProps.map(prop => {
                const isAudit = this.isAuditField(prop.name);
                const isPk = prop.isPrimaryKey || prop.IsPrimaryKey;

                // Visibilidade Default:
                // 1. JSON (list.show) vence
                // 2. PK e Audit escondidos por padrão
                let isVisible = true;
                if (prop.list && prop.list.show !== undefined) {
                    isVisible = prop.list.show;
                } else {
                    if (isPk || isAudit) isVisible = false;
                }

                return {
                    name: prop.name,
                    Name: prop.name, // ✅ PascalCase para backend
                    visible: isVisible,
                    Visible: isVisible, // ✅ PascalCase para backend
                    sortable: prop.list?.sortable ?? true,
                    Sortable: prop.list?.sortable ?? true,
                    searchable: prop.list?.filterable ?? ((prop.type || '').toLowerCase() === 'string'),
                    format: prop.list?.format || this.getDefaultFormat(prop.type),
                    Format: prop.list?.format || this.getDefaultFormat(prop.type),
                    width: prop.list?.width || '',
                    Width: prop.list?.width || '',
                    align: prop.list?.align || this.getDefaultAlign(prop.type),
                    Align: prop.list?.align || this.getDefaultAlign(prop.type),
                    headerText: prop.displayName || prop.name,
                    Title: prop.displayName || prop.name, // ✅ PascalCase
                    Order: this.config.columns.length,
                    isAudit: isAudit // Flag para UI
                };
            });
            this.save();
        }

        const visibleCount = this.config.columns.filter(c => c.visible).length;
        const totalColumns = this.config.columns.length;

        container.innerHTML = `
            <h4>📋 Configuração de Colunas</h4>
            <p class="text-muted">Arraste para reordenar. Configure cada coluna individualmente.</p>
            
            <!-- Info sobre campos ocultos por padrão -->
            ${this.config.columns.some(c => c.isAudit) ? `
                <div style="font-size: 12px; color: #666; padding: 8px; background: #e0f2fe; border-radius: 4px; margin-bottom: 15px; border: 1px solid #bae6fd;">
                    ℹ️ Campos de auditoria/sistema foram incluídos mas estão desmarcados por padrão.
                </div>
            ` : ''}
            
            <!-- Botões de ação -->
            <div style="margin-bottom: 15px; display: flex; gap: 10px;">
                <button class="btn btn-small btn-primary" onclick="GridConfig.selectAllColumns(true)"
                        ${visibleCount === totalColumns ? 'disabled' : ''}>
                    ✅ Selecionar Todas
                </button>
                <button class="btn btn-small btn-secondary" onclick="GridConfig.selectAllColumns(false)"
                        ${visibleCount === 0 ? 'disabled' : ''}>
                    ❌ Desmarcar Todas
                </button>
                <span style="margin-left: auto; color: #666; font-size: 12px;">
                    ${visibleCount} de ${totalColumns} visíveis
                </span>
            </div>
            
            <!-- ✅ v1.5 FIXED: DIV SEM SCROLL - Altura automática -->
            <div class="column-list-no-scroll" id="columnList">
                ${this.config.columns.map((col, idx) => this.renderColumnItem(col, idx)).join('')}
            </div>
        `;

        this.setupColumnReorder();
    },

    // =========================================================================
    // SELECIONAR/DESMARCAR TODAS AS COLUNAS
    // =========================================================================
    selectAllColumns(visible) {
        this.config.columns.forEach(col => {
            col.visible = visible;
            col.Visible = visible; // ✅ PascalCase
        });
        this.save();
        this.renderColumnConfig();

        if (typeof App !== 'undefined' && App.showToast) {
            App.showToast(visible ? '✅ Todas as colunas selecionadas' : '❌ Todas as colunas desmarcadas', 'success');
        }
    },

    // =========================================================================
    // RENDERIZA ITEM DE COLUNA
    // =========================================================================
    renderColumnItem(col, idx) {
        return `
            <div class="column-item" data-index="${idx}" draggable="true">
                <div class="column-drag-handle">☰</div>
                <div class="column-checkbox">
                    <input type="checkbox" ${col.visible ? 'checked' : ''} 
                           onchange="GridConfig.updateColumn(${idx}, 'visible', this.checked)"
                           title="Visível na Grid">
                </div>
                <div class="column-name">
                    <strong>${Utils.escapeHtml(col.name)}</strong>
                    ${col.isAudit ? '<span class="badge badge-warning" style="font-size: 9px; margin-left: 5px;">AUDIT</span>' : ''}
                </div>
                <div class="column-options">
                    <input type="text" value="${Utils.escapeAttr(col.headerText)}" 
                           placeholder="Título" class="col-header-input"
                           onchange="GridConfig.updateColumn(${idx}, 'headerText', this.value)">
                    
                    <select onchange="GridConfig.updateColumn(${idx}, 'format', this.value)" title="Formato">
                        <option value="" ${!col.format ? 'selected' : ''}>Padrão</option>
                        <option value="date" ${col.format === 'date' ? 'selected' : ''}>Data</option>
                        <option value="datetime" ${col.format === 'datetime' ? 'selected' : ''}>Data/Hora</option>
                        <option value="currency" ${col.format === 'currency' ? 'selected' : ''}>Moeda (R$)</option>
                        <option value="number" ${col.format === 'number' ? 'selected' : ''}>Número</option>
                        <option value="percent" ${col.format === 'percent' ? 'selected' : ''}>Percentual</option>
                        <option value="boolean" ${col.format === 'boolean' ? 'selected' : ''}>Sim/Não</option>
                        <option value="status" ${col.format === 'status' ? 'selected' : ''}>Status (Badge)</option>
                    </select>

                    <select onchange="GridConfig.updateColumn(${idx}, 'align', this.value)" title="Alinhamento">
                        <option value="left" ${col.align === 'left' ? 'selected' : ''}>Esquerda</option>
                        <option value="center" ${col.align === 'center' ? 'selected' : ''}>Centro</option>
                        <option value="right" ${col.align === 'right' ? 'selected' : ''}>Direita</option>
                    </select>

                    <input type="text" value="${col.width || ''}" placeholder="Largura" 
                           class="col-width-input" title="Ex: 100px, 15%"
                           onchange="GridConfig.updateColumn(${idx}, 'width', this.value)">

                    <label title="Ordenável">
                        <input type="checkbox" ${col.sortable ? 'checked' : ''} 
                               onchange="GridConfig.updateColumn(${idx}, 'sortable', this.checked)">
                        Sort
                    </label>
                </div>
            </div>
        `;
    },

    // =========================================================================
    // REORDENAÇÃO DE COLUNAS
    // =========================================================================
    setupColumnReorder() {
        const list = document.getElementById('columnList');
        if (!list) return;

        let dragging = null;

        list.querySelectorAll('.column-item').forEach(item => {
            item.addEventListener('dragstart', () => {
                dragging = item;
                item.classList.add('dragging');
            });

            item.addEventListener('dragend', () => {
                dragging = null;
                item.classList.remove('dragging');
            });

            item.addEventListener('dragover', (e) => {
                e.preventDefault();
                if (dragging && dragging !== item) {
                    const rect = item.getBoundingClientRect();
                    const midpoint = rect.top + rect.height / 2;
                    if (e.clientY < midpoint) {
                        list.insertBefore(dragging, item);
                    } else {
                        list.insertBefore(dragging, item.nextSibling);
                    }
                }
            });

            item.addEventListener('drop', (e) => {
                e.preventDefault();
                this.updateColumnOrder();
            });
        });
    },

    updateColumnOrder() {
        const newOrder = [];
        document.querySelectorAll('.column-item').forEach(item => {
            const idx = parseInt(item.dataset.index);
            newOrder.push(this.config.columns[idx]);
        });
        this.config.columns = newOrder;

        // Atualizar Order (índice)
        this.config.columns.forEach((col, idx) => {
            col.Order = idx;
        });

        this.save();
        this.renderColumnConfig();
    },

    // =========================================================================
    // CONFIGURAÇÃO DE FILTROS
    // =========================================================================
    renderFilterConfig() {
        const entity = Store.get('entity');
        const container = document.getElementById('filterConfig');
        if (!container || !entity) return;

        // Filtra propriedades (exclui auditoria)
        const gridProps = this.getGridProperties(entity);

        container.innerHTML = `
            <h4>🔍 Filtros Avançados</h4>
            <p class="text-muted">Configure filtros personalizados para a listagem.</p>
            
            <div class="filter-list" id="filterList">
                ${this.config.filters.map((filter, idx) => this.renderFilterItem(filter, idx, gridProps)).join('')}
            </div>
            
            <button class="btn btn-secondary btn-small" onclick="GridConfig.addFilter()">
                + Adicionar Filtro
            </button>
        `;
    },

    renderFilterItem(filter, idx, properties) {
        return `
            <div class="filter-item">
                <button class="filter-remove" onclick="GridConfig.removeFilter(${idx})">×</button>
                
                <div class="filter-row">
                    <div class="filter-field">
                        <label>Campo:</label>
                        <select onchange="GridConfig.updateFilter(${idx}, 'field', this.value)">
                            <option value="">Selecione...</option>
                            ${properties.map(p => `
                                <option value="${p.name}" ${filter.field === p.name ? 'selected' : ''}>
                                    ${p.name}
                                </option>
                            `).join('')}
                        </select>
                    </div>
                    
                    <div class="filter-field">
                        <label>Tipo:</label>
                        <select onchange="GridConfig.updateFilter(${idx}, 'type', this.value)">
                            <option value="text" ${filter.type === 'text' ? 'selected' : ''}>Texto</option>
                            <option value="select" ${filter.type === 'select' ? 'selected' : ''}>Select</option>
                            <option value="dateRange" ${filter.type === 'dateRange' ? 'selected' : ''}>Range de Data</option>
                            <option value="numberRange" ${filter.type === 'numberRange' ? 'selected' : ''}>Range Numérico</option>
                            <option value="boolean" ${filter.type === 'boolean' ? 'selected' : ''}>Sim/Não</option>
                        </select>
                    </div>
                    
                    <div class="filter-field">
                        <label>Label:</label>
                        <input type="text" value="${Utils.escapeAttr(filter.label || '')}" 
                               placeholder="Label do filtro"
                               onchange="GridConfig.updateFilter(${idx}, 'label', this.value)">
                    </div>
                </div>
                
                ${filter.type === 'select' ? `
                <div class="filter-row">
                    <div class="filter-field wide">
                        <label>Endpoint (API):</label>
                        <input type="text" value="${Utils.escapeAttr(filter.endpoint || '')}" 
                               placeholder="/api/opcoes"
                               onchange="GridConfig.updateFilter(${idx}, 'endpoint', this.value)">
                    </div>
                </div>
                ` : ''}
            </div>
        `;
    },

    addFilter() {
        this.config.filters.push({
            field: '',
            type: 'text',
            label: '',
            endpoint: ''
        });
        this.save();
        this.renderFilterConfig();
    },

    removeFilter(idx) {
        this.config.filters.splice(idx, 1);
        this.save();
        this.renderFilterConfig();
    },

    updateFilter(idx, property, value) {
        this.config.filters[idx][property] = value;
        this.save();

        if (property === 'type') {
            this.renderFilterConfig();
        }
    },

    // =========================================================================
    // UTILITÁRIOS
    // =========================================================================
    updateOption(key, value) {
        this.config[key] = value;
        this.save();
    },

    toggleExport(format, enabled) {
        if (enabled && !this.config.exportFormats.includes(format)) {
            this.config.exportFormats.push(format);
        } else if (!enabled) {
            this.config.exportFormats = this.config.exportFormats.filter(f => f !== format);
        }
        this.save();
    },

    updateColumn(idx, property, value) {
        this.config.columns[idx][property] = value;

        // ✅ Atualizar também a versão PascalCase
        const pascalProp = property.charAt(0).toUpperCase() + property.slice(1);
        this.config.columns[idx][pascalProp] = value;

        this.save();

        // Re-renderiza para atualizar contagem
        if (property === 'visible') {
            this.renderColumnConfig();
        }
    },

    getDefaultFormat(type) {
        const formatMap = {
            'datetime': 'datetime',
            'DateTime': 'datetime',
            'date': 'date',
            'DateOnly': 'date',
            'decimal': 'currency',
            'bool': 'boolean',
            'boolean': 'boolean'
        };
        return formatMap[type?.toLowerCase()] || '';
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

    getVisibleColumns() {
        return this.config.columns.filter(c => c.visible);
    },

    getSearchableColumns() {
        return this.config.columns.filter(c => c.searchable);
    }
};

// Registra módulo
if (typeof App !== 'undefined') {
    App.registerModule('GridConfig', GridConfig);
}
window.GridConfig = GridConfig;

console.log('✅ GridConfig v1.5 FIXED carregado (sem scroll na lista de colunas)');