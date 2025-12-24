/**
 * =============================================================================
 * FORM DESIGNER MODULE v2.1
 * Designer visual de formulários com drag & drop, tabs e layout
 * =============================================================================
 * CHANGELOG v2.1:
 * - Adicionado botão "Adicionar Todos" para campos editáveis
 * - Exclusão automática de campos de auditoria (IdSaas, DtCriacao, etc.)
 * - Corrigido bug de duplicação ao arrastar campos
 * - Botão "Limpar Formulário" mais confiável
 * =============================================================================
 */

const FormDesigner = {
    // =========================================================================
    // CAMPOS DE AUDITORIA (nunca aparecem no form nem na grid)
    // =========================================================================
    AUDIT_FIELDS: [
        // Multi-tenancy
        'idsaas', 'id_saas',
        // Data de criação
        'datacriacao', 'dtcriacao', 'createdat', 'dtinclusao', 'datainclusao',
        'dt_criacao', 'data_criacao', 'dt_inclusao', 'data_inclusao',
        // Data de alteração
        'dataalteracao', 'dtalteracao', 'updatedat', 'modifiedat', 'dtaicalizacao',
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
    // CONFIGURAÇÃO DE LAYOUT
    // =========================================================================
    layoutConfig: {
        columns: 2,
        useTabs: false,
        tabs: ['Dados Gerais']
    },

    activeTabIndex: 0,

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================
    init() {
        console.log('🎨 Form Designer v2.1 initialized');

        const savedLayout = localStorage.getItem('formLayoutConfig');
        if (savedLayout) {
            try {
                this.layoutConfig = JSON.parse(savedLayout);
            } catch (e) { }
        }
    },

    saveLayoutConfig() {
        localStorage.setItem('formLayoutConfig', JSON.stringify(this.layoutConfig));
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
    // FILTRA PROPRIEDADES EDITÁVEIS (remove PK, Identity, Auditoria)
    // =========================================================================
    getEditableProperties(entity) {
        if (!entity || !entity.properties) return [];

        return entity.properties.filter(prop => {
            // Exclui PK
            if (prop.isPrimaryKey || prop.IsPrimaryKey) return false;

            // Exclui Identity (geralmente é a PK)
            if (prop.isIdentity || prop.IsIdentity) return false;

            // Exclui campos de auditoria
            if (this.isAuditField(prop.name)) return false;

            return true;
        });
    },

    // =========================================================================
    // RENDERIZAÇÃO
    // =========================================================================
    render() {
        this.renderLayoutOptions();
        this.renderPalette();
        this.renderCanvas();
        this.setupDragDrop();
    },

    // =========================================================================
    // OPÇÕES DE LAYOUT
    // =========================================================================
    renderLayoutOptions() {
        const container = document.getElementById('layoutOptions');
        if (!container) return;

        container.innerHTML = `
            <div class="layout-config">
                <div class="layout-row">
                    <div class="layout-item">
                        <label>Colunas:</label>
                        <select id="layoutColumns" onchange="FormDesigner.setColumns(this.value)">
                            <option value="1" ${this.layoutConfig.columns === 1 ? 'selected' : ''}>1 Coluna</option>
                            <option value="2" ${this.layoutConfig.columns === 2 ? 'selected' : ''}>2 Colunas</option>
                            <option value="3" ${this.layoutConfig.columns === 3 ? 'selected' : ''}>3 Colunas</option>
                            <option value="4" ${this.layoutConfig.columns === 4 ? 'selected' : ''}>4 Colunas</option>
                        </select>
                    </div>
                    <div class="layout-item">
                        <label>
                            <input type="checkbox" id="useTabs" 
                                   ${this.layoutConfig.useTabs ? 'checked' : ''} 
                                   onchange="FormDesigner.toggleTabs(this.checked)">
                            Usar Abas
                        </label>
                    </div>
                </div>
                <div id="tabsConfig" class="tabs-config" style="display: ${this.layoutConfig.useTabs ? 'block' : 'none'}">
                    <label>Abas:</label>
                    <div id="tabsList" class="tabs-list">
                        ${this.layoutConfig.tabs.map((tab, idx) => `
                            <div class="tab-item">
                                <input type="text" value="${Utils.escapeAttr(tab)}" 
                                       onchange="FormDesigner.updateTab(${idx}, this.value)">
                                ${idx > 0 ? `<button onclick="FormDesigner.removeTab(${idx})">×</button>` : ''}
                            </div>
                        `).join('')}
                    </div>
                    <button class="btn btn-small btn-secondary" onclick="FormDesigner.addTab()">+ Adicionar Aba</button>
                </div>
            </div>
        `;
    },

    setColumns(cols) {
        this.layoutConfig.columns = parseInt(cols);
        this.saveLayoutConfig();
        this.updateFieldSizes();
        this.renderCanvas();
    },

    updateFieldSizes() {
        const colSizeMap = { 1: 'col-md-12', 2: 'col-md-6', 3: 'col-md-4', 4: 'col-md-3' };
        const defaultSize = colSizeMap[this.layoutConfig.columns] || 'col-md-6';

        const formFields = Store.get('formFields') || [];
        formFields.forEach(field => {
            if (!field.colSize || field.colSize === 'col-md-6' || field.colSize === 'col-md-12' ||
                field.colSize === 'col-md-4' || field.colSize === 'col-md-3') {
                field.colSize = defaultSize;
            }
        });
        Store.set('formFields', formFields);
    },

    toggleTabs(enabled) {
        this.layoutConfig.useTabs = enabled;
        if (enabled && this.layoutConfig.tabs.length === 0) {
            this.layoutConfig.tabs = ['Dados Gerais'];
        }
        this.saveLayoutConfig();

        const tabsConfig = document.getElementById('tabsConfig');
        if (tabsConfig) {
            tabsConfig.style.display = enabled ? 'block' : 'none';
        }
        this.renderCanvas();
    },

    addTab() {
        const newTabName = `Aba ${this.layoutConfig.tabs.length + 1}`;
        this.layoutConfig.tabs.push(newTabName);
        this.saveLayoutConfig();
        this.renderLayoutOptions();
    },

    removeTab(index) {
        if (this.layoutConfig.tabs.length <= 1) return;

        const removedTab = this.layoutConfig.tabs[index];
        this.layoutConfig.tabs.splice(index, 1);

        const formFields = Store.get('formFields') || [];
        formFields.forEach(field => {
            if (field.tab === removedTab) {
                field.tab = this.layoutConfig.tabs[0];
            }
        });
        Store.set('formFields', formFields);

        this.saveLayoutConfig();
        this.renderLayoutOptions();
        this.renderCanvas();
    },

    updateTab(index, value) {
        const oldName = this.layoutConfig.tabs[index];
        this.layoutConfig.tabs[index] = value;

        const formFields = Store.get('formFields') || [];
        formFields.forEach(field => {
            if (field.tab === oldName) {
                field.tab = value;
            }
        });
        Store.set('formFields', formFields);

        this.saveLayoutConfig();
    },

    // =========================================================================
    // PALETA DE CAMPOS (com exclusão de auditoria)
    // =========================================================================
    renderPalette() {
        const entity = Store.get('entity');
        const palette = document.getElementById('fieldsPalette');

        if (!palette || !entity) return;

        // Filtra campos editáveis (exclui PK, Identity, Auditoria)
        const editableProps = this.getEditableProperties(entity);
        const formFields = Store.get('formFields') || [];
        const addedFieldNames = formFields.map(f => f.name.toLowerCase());

        // Conta campos
        const totalProps = entity.properties?.length || 0;
        const auditCount = totalProps - editableProps.length;
        const availableCount = editableProps.filter(p => !addedFieldNames.includes(p.name.toLowerCase())).length;

        palette.innerHTML = `
            <!-- Botões de Ação -->
            <div class="palette-actions" style="margin-bottom: 15px; padding: 10px; background: #f8f9fa; border-radius: 8px;">
                <button class="btn btn-success btn-small" onclick="FormDesigner.addAllFields()" 
                        style="width: 100%; margin-bottom: 8px;"
                        ${availableCount === 0 ? 'disabled' : ''}>
                    ✨ Adicionar Todos (${availableCount})
                </button>
                <button class="btn btn-danger btn-small" onclick="FormDesigner.clearAllFields()" 
                        style="width: 100%;"
                        ${formFields.length === 0 ? 'disabled' : ''}>
                    🗑️ Limpar Formulário
                </button>
            </div>
            
            <!-- Info sobre campos excluídos -->
            ${auditCount > 0 ? `
                <div class="palette-info" style="font-size: 11px; color: #666; padding: 8px; background: #fff3cd; border-radius: 4px; margin-bottom: 10px;">
                    ℹ️ ${auditCount} campo(s) de auditoria ocultados automaticamente
                </div>
            ` : ''}
            
            <!-- Lista de campos disponíveis -->
            ${editableProps.map(prop => {
            const isAdded = addedFieldNames.includes(prop.name.toLowerCase());
            return `
                    <div class="draggable-field ${isAdded ? 'field-added' : ''}" 
                         draggable="${isAdded ? 'false' : 'true'}" 
                         data-field='${Utils.escapeAttr(JSON.stringify(prop))}'
                         style="${isAdded ? 'opacity: 0.5; cursor: not-allowed;' : ''}">
                        <strong>${Utils.escapeHtml(prop.name)}</strong>
                        ${isAdded ? '<span style="color: #28a745; float: right;">✓</span>' : ''}
                        <br>
                        <small>${Utils.escapeHtml(prop.type)}${prop.nullable || prop.isNullable ? ' (nullable)' : ''}</small>
                    </div>
                `;
        }).join('')}
        `;

        // Setup drag events na paleta
        palette.querySelectorAll('.draggable-field:not(.field-added)').forEach(field => {
            field.addEventListener('dragstart', (e) => {
                e.dataTransfer.setData('field', field.dataset.field);
                field.classList.add('dragging');
            });

            field.addEventListener('dragend', () => {
                field.classList.remove('dragging');
            });
        });
    },

    // =========================================================================
    // ADICIONAR TODOS OS CAMPOS
    // =========================================================================
    addAllFields() {
        const entity = Store.get('entity');
        if (!entity) return;

        App.saveUndoState('adicionar todos os campos');

        const editableProps = this.getEditableProperties(entity);
        const formFields = Store.get('formFields') || [];
        const addedFieldNames = formFields.map(f => f.name.toLowerCase());
        let fieldCounter = Store.get('fieldCounter') || 0;

        const colSizeMap = { 1: 'col-md-12', 2: 'col-md-6', 3: 'col-md-4', 4: 'col-md-3' };
        const defaultColSize = colSizeMap[this.layoutConfig.columns] || 'col-md-6';
        const defaultTab = this.layoutConfig.useTabs ? this.layoutConfig.tabs[0] : '';

        let addedCount = 0;

        editableProps.forEach(prop => {
            if (addedFieldNames.includes(prop.name.toLowerCase())) return;

            const fieldConfig = {
                id: `field_${fieldCounter++}`,
                ...prop,
                inputType: Utils.getDefaultInputType(prop.type),
                label: prop.displayName || prop.name,
                placeholder: '',
                group: 'Dados Gerais',
                tab: defaultTab,
                required: !(prop.nullable ?? prop.isNullable ?? true),
                colSize: defaultColSize,
                validations: [],
                mask: '',
                helpText: '',
                endpoint: '',
                valueField: 'id',
                textField: 'nome',
                cascadeTo: '',
                dependsOn: '',
                filterParam: ''
            };

            formFields.push(fieldConfig);
            addedCount++;
        });

        Store.setMultiple({ formFields, fieldCounter });

        this.renderPalette();
        this.renderCanvas();

        App.showToast(`✅ ${addedCount} campo(s) adicionado(s)!`, 'success');
    },

    // =========================================================================
    // LIMPAR TODOS OS CAMPOS
    // =========================================================================
    clearAllFields() {
        const formFields = Store.get('formFields') || [];

        if (formFields.length === 0) {
            App.showToast('ℹ️ Formulário já está vazio', 'info');
            return;
        }

        if (!confirm(`⚠️ Tem certeza que deseja remover todos os ${formFields.length} campos do formulário?`)) {
            return;
        }

        App.saveUndoState('limpar formulário');

        // Limpa completamente
        Store.set('formFields', []);
        Store.set('fieldCounter', 0);
        Store.set('selectedField', null);

        this.clearConfigPanel();
        this.renderPalette();
        this.renderCanvas();

        App.showToast('🗑️ Formulário limpo!', 'success');
    },

    // =========================================================================
    // RENDERIZA CANVAS
    // =========================================================================
    renderCanvas() {
        const canvas = document.getElementById('formCanvas');
        const formFields = Store.get('formFields') || [];

        if (!canvas) return;

        if (this.layoutConfig.useTabs) {
            canvas.innerHTML = this.renderTabsCanvas(formFields);
        } else if (formFields.length === 0) {
            canvas.innerHTML = `
                <div class="canvas-empty">
                    <h3>Arraste os campos aqui</h3>
                    <p>Arraste campos da paleta à esquerda para montar seu formulário</p>
                    <p style="margin-top: 10px;">
                        <button class="btn btn-primary btn-small" onclick="FormDesigner.addAllFields()">
                            ✨ Ou clique aqui para adicionar todos
                        </button>
                    </p>
                </div>
            `;
        } else {
            canvas.innerHTML = this.renderSimpleCanvas(formFields);
        }

        this.attachCanvasEvents();
        this.setupSortable();
        this.setupDragDrop();
    },

    renderSimpleCanvas(formFields) {
        const groups = this.groupFieldsByGroup(formFields);
        let html = '';

        Object.keys(groups).forEach(groupName => {
            html += `
                <div class="canvas-group">
                    <div class="canvas-group-header">
                        <span>📁 ${Utils.escapeHtml(groupName)}</span>
                    </div>
                    <div class="canvas-row" data-group="${Utils.escapeAttr(groupName)}">
                        ${groups[groupName].map(field => this.createFieldElement(field)).join('')}
                    </div>
                </div>
            `;
        });

        return html;
    },

    renderTabsCanvas(formFields) {
        if (this.activeTabIndex >= this.layoutConfig.tabs.length) {
            this.activeTabIndex = 0;
        }

        let html = `
            <div class="canvas-tabs">
                ${this.layoutConfig.tabs.map((tab, idx) => `
                    <button class="canvas-tab ${idx === this.activeTabIndex ? 'active' : ''}" 
                            onclick="FormDesigner.switchCanvasTab(${idx})">${Utils.escapeHtml(tab)}</button>
                `).join('')}
            </div>
        `;

        this.layoutConfig.tabs.forEach((tab, idx) => {
            const tabFields = formFields.filter(f => (f.tab || this.layoutConfig.tabs[0]) === tab);
            const groups = this.groupFieldsByGroup(tabFields);

            html += `
                <div class="canvas-tab-content ${idx === this.activeTabIndex ? 'active' : ''}" 
                     data-tab="${idx}" 
                     data-tab-name="${Utils.escapeAttr(tab)}">
                    ${Object.keys(groups).length === 0 ? `
                        <div class="canvas-empty-tab">
                            <p>Arraste campos para esta aba</p>
                        </div>
                    ` : ''}
                    ${Object.keys(groups).map(groupName => `
                        <div class="canvas-group">
                            <div class="canvas-group-header">
                                <span>📁 ${Utils.escapeHtml(groupName)}</span>
                            </div>
                            <div class="canvas-row" data-group="${Utils.escapeAttr(groupName)}" data-tab="${Utils.escapeAttr(tab)}">
                                ${groups[groupName].map(field => this.createFieldElement(field)).join('')}
                            </div>
                        </div>
                    `).join('')}
                </div>
            `;
        });

        return html;
    },

    groupFieldsByGroup(fields) {
        const groups = {};
        fields.forEach(field => {
            const groupName = field.group || 'Dados Gerais';
            if (!groups[groupName]) groups[groupName] = [];
            groups[groupName].push(field);
        });
        return groups;
    },

    switchCanvasTab(idx) {
        this.activeTabIndex = idx;

        document.querySelectorAll('.canvas-tab').forEach(t => t.classList.remove('active'));
        document.querySelectorAll('.canvas-tab-content').forEach(c => c.classList.remove('active'));

        document.querySelectorAll('.canvas-tab')[idx]?.classList.add('active');
        document.querySelector(`.canvas-tab-content[data-tab="${idx}"]`)?.classList.add('active');
    },

    attachCanvasEvents() {
        document.querySelectorAll('.dropped-field').forEach(el => {
            const fieldId = el.dataset.fieldId;
            const formFields = Store.get('formFields') || [];
            const field = formFields.find(f => f.id === fieldId);

            if (field) {
                el.addEventListener('click', (e) => {
                    if (!e.target.classList.contains('field-remove')) {
                        this.selectField(field);
                    }
                });
            }
        });
    },

    setupSortable() {
        document.querySelectorAll('.canvas-row').forEach(row => {
            const fields = row.querySelectorAll('.dropped-field');

            fields.forEach(field => {
                field.setAttribute('draggable', 'true');

                field.addEventListener('dragstart', (e) => {
                    e.dataTransfer.setData('reorder', field.dataset.fieldId);
                    field.classList.add('dragging');
                });

                field.addEventListener('dragend', () => {
                    field.classList.remove('dragging');
                });

                field.addEventListener('dragover', (e) => {
                    e.preventDefault();
                    const dragging = document.querySelector('.dropped-field.dragging');
                    if (dragging && dragging !== field) {
                        const rect = field.getBoundingClientRect();
                        const midpoint = rect.left + rect.width / 2;
                        if (e.clientX < midpoint) {
                            row.insertBefore(dragging, field);
                        } else {
                            row.insertBefore(dragging, field.nextSibling);
                        }
                    }
                });
            });

            row.addEventListener('drop', (e) => {
                e.preventDefault();
                this.updateFieldOrder();
            });
        });
    },

    updateFieldOrder() {
        const newOrder = [];
        document.querySelectorAll('.dropped-field').forEach(el => {
            const fieldId = el.dataset.fieldId;
            const formFields = Store.get('formFields') || [];
            const field = formFields.find(f => f.id === fieldId);
            if (field) newOrder.push(field);
        });
        Store.set('formFields', newOrder);
    },

    setupDragDrop() {
        const canvas = document.getElementById('formCanvas');
        if (!canvas) return;

        if (this.layoutConfig.useTabs) {
            document.querySelectorAll('.canvas-tab-content').forEach((tabContent, idx) => {
                tabContent.addEventListener('dragover', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    tabContent.classList.add('drag-over');
                });

                tabContent.addEventListener('dragleave', (e) => {
                    if (!tabContent.contains(e.relatedTarget)) {
                        tabContent.classList.remove('drag-over');
                    }
                });

                tabContent.addEventListener('drop', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    tabContent.classList.remove('drag-over');

                    const fieldData = e.dataTransfer.getData('field');
                    if (fieldData) {
                        try {
                            const prop = JSON.parse(fieldData);
                            const tabName = this.layoutConfig.tabs[idx];
                            this.addField(prop, tabName);
                        } catch (err) {
                            console.error('Error parsing dropped field:', err);
                        }
                    }
                });
            });
        } else {
            canvas.addEventListener('dragover', (e) => {
                e.preventDefault();
                canvas.classList.add('drag-over');
            });

            canvas.addEventListener('dragleave', () => {
                canvas.classList.remove('drag-over');
            });

            canvas.addEventListener('drop', (e) => {
                e.preventDefault();
                canvas.classList.remove('drag-over');

                const fieldData = e.dataTransfer.getData('field');
                if (fieldData) {
                    try {
                        const prop = JSON.parse(fieldData);
                        this.addField(prop);
                    } catch (err) {
                        console.error('Error parsing dropped field:', err);
                    }
                }
            });
        }
    },

    // =========================================================================
    // ADICIONA CAMPO (com verificação robusta de duplicação)
    // =========================================================================
    addField(propData, targetTab = null) {
        // FORÇA atualização do Store para garantir dados frescos
        const formFields = [...(Store.get('formFields') || [])];
        let fieldCounter = Store.get('fieldCounter') || 0;

        // Verifica duplicação de forma case-insensitive
        const propNameLower = propData.name.toLowerCase();
        const exists = formFields.some(f => f.name.toLowerCase() === propNameLower);

        if (exists) {
            App.showToast(`⚠️ Campo "${propData.name}" já foi adicionado!`, 'warning');
            return;
        }

        const colSizeMap = { 1: 'col-md-12', 2: 'col-md-6', 3: 'col-md-4', 4: 'col-md-3' };
        const defaultColSize = colSizeMap[this.layoutConfig.columns] || 'col-md-6';

        let tabName = '';
        if (this.layoutConfig.useTabs) {
            tabName = targetTab || this.layoutConfig.tabs[this.activeTabIndex] || this.layoutConfig.tabs[0];
        }

        const fieldConfig = {
            id: `field_${Date.now()}_${fieldCounter++}`,
            ...propData,
            inputType: Utils.getDefaultInputType(propData.type),
            label: propData.displayName || propData.name,
            placeholder: '',
            group: 'Dados Gerais',
            tab: tabName,
            required: !(propData.nullable ?? propData.isNullable ?? true),
            colSize: defaultColSize,
            validations: [],
            mask: '',
            helpText: '',
            endpoint: '',
            valueField: 'id',
            textField: 'nome',
            cascadeTo: '',
            dependsOn: '',
            filterParam: ''
        };

        formFields.push(fieldConfig);

        Store.setMultiple({
            formFields,
            fieldCounter
        });

        this.renderPalette();
        this.renderCanvas();
        this.selectField(fieldConfig);
    },

    // =========================================================================
    // CRIA ELEMENTO HTML DO CAMPO
    // =========================================================================
    createFieldElement(field) {
        const selectedField = Store.get('selectedField');
        const isSelected = selectedField?.id === field.id;
        const colClass = field.colSize || 'col-md-6';

        const inputPreview = this.renderInputPreview(field);

        return `
            <div class="dropped-field ${isSelected ? 'selected' : ''} ${colClass}" 
                 data-field-id="${field.id}" draggable="true">
                <button class="field-remove" onclick="FormDesigner.removeField('${field.id}')">×</button>
                <div class="field-preview">
                    <label class="field-preview-label">
                        ${Utils.escapeHtml(field.label || field.name)}
                        ${field.required ? '<span class="required-mark">*</span>' : ''}
                    </label>
                    ${inputPreview}
                    ${field.helpText ? `<small class="field-help">${Utils.escapeHtml(field.helpText)}</small>` : ''}
                </div>
                <div class="field-meta">
                    <span class="badge badge-primary">${field.inputType}</span>
                    <span class="badge badge-secondary">${colClass}</span>
                    ${field.mask ? `<span class="badge badge-warning">${field.mask}</span>` : ''}
                </div>
            </div>
        `;
    },

    renderInputPreview(field) {
        const placeholder = field.placeholder || `Digite ${field.label || field.name}...`;

        switch (field.inputType) {
            case 'textarea':
                return `<div class="preview-textarea">${Utils.escapeHtml(placeholder)}</div>`;

            case 'select':
                return `
                    <div class="preview-select">
                        <span>Selecione...</span>
                        <span class="preview-select-arrow">▼</span>
                    </div>
                `;

            case 'checkbox':
                return `
                    <div class="preview-checkbox">
                        <input type="checkbox" disabled>
                        <span>${Utils.escapeHtml(field.label || field.name)}</span>
                    </div>
                `;

            case 'date':
            case 'datetime-local':
            case 'time':
                return `<div class="preview-input preview-date">📅 ${field.inputType}</div>`;

            case 'number':
                return `<div class="preview-input">123</div>`;

            case 'email':
                return `<div class="preview-input">email@exemplo.com</div>`;

            case 'hidden':
                return `<div class="preview-hidden">[campo oculto]</div>`;

            default:
                return `<div class="preview-input">${Utils.escapeHtml(placeholder)}</div>`;
        }
    },

    // =========================================================================
    // REMOVE CAMPO
    // =========================================================================
    removeField(fieldId) {
        App.saveUndoState('remoção de campo');

        let formFields = Store.get('formFields') || [];
        formFields = formFields.filter(f => f.id !== fieldId);

        const selectedField = Store.get('selectedField');
        if (selectedField?.id === fieldId) {
            Store.set('selectedField', null);
            this.clearConfigPanel();
        }

        Store.set('formFields', formFields);
        this.renderPalette();
        this.renderCanvas();
    },

    // =========================================================================
    // SELEÇÃO DE CAMPO
    // =========================================================================
    selectField(field) {
        Store.set('selectedField', field);

        document.querySelectorAll('.dropped-field').forEach(el => {
            el.classList.remove('selected');
        });

        const fieldEl = document.querySelector(`.dropped-field[data-field-id="${field.id}"]`);
        if (fieldEl) {
            fieldEl.classList.add('selected');
        }

        this.renderConfigPanel(field);
    },

    clearConfigPanel() {
        const panel = document.getElementById('fieldConfigPanel');
        if (panel) {
            panel.innerHTML = `
                <p style="color: #999; text-align: center; margin-top: 50px;">
                    Selecione um campo no canvas
                </p>
            `;
        }
    },

    // =========================================================================
    // PAINEL DE CONFIGURAÇÃO
    // =========================================================================
    renderConfigPanel(field) {
        const panel = document.getElementById('fieldConfigPanel');
        if (!panel) return;

        const formFields = Store.get('formFields') || [];
        const selectFields = formFields.filter(f => f.inputType === 'select' && f.id !== field.id);

        let html = `
            <!-- Informações Básicas -->
            <div class="config-section">
                <h4>📝 Informações Básicas</h4>
                <div class="config-row">
                    <label>Nome do Campo:</label>
                    <input type="text" value="${Utils.escapeAttr(field.name)}" disabled 
                           style="background: #f5f5f5;">
                </div>
                <div class="config-row">
                    <label>Label:</label>
                    <input type="text" value="${Utils.escapeAttr(field.label)}" 
                           onchange="FormDesigner.updateField('label', this.value)">
                </div>
                <div class="config-row">
                    <label>Tipo de Input:</label>
                    <select onchange="FormDesigner.updateField('inputType', this.value)">
                        ${this.renderInputTypeOptions(field.inputType)}
                    </select>
                </div>
                <div class="config-row">
                    <label>Placeholder:</label>
                    <input type="text" value="${Utils.escapeAttr(field.placeholder || '')}" 
                           onchange="FormDesigner.updateField('placeholder', this.value)">
                </div>
                <div class="config-row">
                    <label>Texto de Ajuda:</label>
                    <input type="text" value="${Utils.escapeAttr(field.helpText || '')}" 
                           onchange="FormDesigner.updateField('helpText', this.value)">
                </div>
            </div>

            <!-- Layout -->
            <div class="config-section">
                <h4>📐 Layout</h4>
                <div class="config-row">
                    <label>Tamanho da Coluna:</label>
                    <select onchange="FormDesigner.updateField('colSize', this.value)">
                        <option value="col-md-2" ${field.colSize === 'col-md-2' ? 'selected' : ''}>2 cols (16%)</option>
                        <option value="col-md-3" ${field.colSize === 'col-md-3' ? 'selected' : ''}>3 cols (25%)</option>
                        <option value="col-md-4" ${field.colSize === 'col-md-4' ? 'selected' : ''}>4 cols (33%)</option>
                        <option value="col-md-6" ${field.colSize === 'col-md-6' ? 'selected' : ''}>6 cols (50%)</option>
                        <option value="col-md-8" ${field.colSize === 'col-md-8' ? 'selected' : ''}>8 cols (66%)</option>
                        <option value="col-md-12" ${field.colSize === 'col-md-12' ? 'selected' : ''}>12 cols (100%)</option>
                    </select>
                </div>
                <div class="config-row">
                    <label>Grupo:</label>
                    <input type="text" value="${Utils.escapeAttr(field.group || 'Dados Gerais')}" 
                           onchange="FormDesigner.updateField('group', this.value)">
                </div>
            </div>

            <!-- Comportamento -->
            <div class="config-section">
                <h4>⚙️ Comportamento</h4>
                <div class="config-row checkbox-row">
                    <label>
                        <input type="checkbox" ${field.required ? 'checked' : ''} 
                               onchange="FormDesigner.updateField('required', this.checked)">
                        Campo Obrigatório
                    </label>
                </div>
                <div class="config-row">
                    <label>Máscara:</label>
                    <select onchange="FormDesigner.updateField('mask', this.value)">
                        <option value="" ${!field.mask ? 'selected' : ''}>Nenhuma</option>
                        <option value="cpf" ${field.mask === 'cpf' ? 'selected' : ''}>CPF (000.000.000-00)</option>
                        <option value="cnpj" ${field.mask === 'cnpj' ? 'selected' : ''}>CNPJ (00.000.000/0000-00)</option>
                        <option value="phone" ${field.mask === 'phone' ? 'selected' : ''}>Telefone (00) 00000-0000</option>
                        <option value="cep" ${field.mask === 'cep' ? 'selected' : ''}>CEP (00000-000)</option>
                        <option value="date" ${field.mask === 'date' ? 'selected' : ''}>Data (00/00/0000)</option>
                        <option value="money" ${field.mask === 'money' ? 'selected' : ''}>Moeda (R$ 0.000,00)</option>
                    </select>
                </div>
            </div>

            <!-- Configuração de Select -->
            <div id="selectConfig" class="config-section" style="display: ${field.inputType === 'select' ? 'block' : 'none'}">
                <h4>🔗 Configuração do Select</h4>
                <div class="config-row">
                    <label>Endpoint (API):</label>
                    <input type="text" value="${Utils.escapeAttr(field.endpoint || '')}" 
                           placeholder="/api/opcoes"
                           onchange="FormDesigner.updateField('endpoint', this.value)">
                </div>
                <div class="config-row">
                    <label>Campo de Valor (ID):</label>
                    <input type="text" value="${Utils.escapeAttr(field.valueField || 'id')}" 
                           onchange="FormDesigner.updateField('valueField', this.value)">
                </div>
                <div class="config-row">
                    <label>Campo de Texto:</label>
                    <input type="text" value="${Utils.escapeAttr(field.textField || 'nome')}" 
                           onchange="FormDesigner.updateField('textField', this.value)">
                </div>
                
                <!-- Cascade (opcional) -->
                <div class="config-row checkbox-row">
                    <label>
                        <input type="checkbox" ${field.cascadeTo ? 'checked' : ''} 
                               onchange="FormDesigner.toggleCascade(this.checked)">
                        Cascata para outro campo
                    </label>
                </div>
                <div id="cascadeOptions" style="display: ${field.cascadeTo ? 'block' : 'none'}">
                    <div class="config-row">
                        <label>Campo dependente:</label>
                        <select onchange="FormDesigner.updateField('cascadeTo', this.value)">
                            <option value="">Selecione...</option>
                            ${selectFields.map(f => `
                                <option value="${f.name}" ${field.cascadeTo === f.name ? 'selected' : ''}>
                                    ${f.label || f.name}
                                </option>
                            `).join('')}
                        </select>
                    </div>
                    <div class="config-row">
                        <label>Parâmetro de filtro:</label>
                        <input type="text" value="${Utils.escapeAttr(field.filterParam || '')}" 
                               placeholder="idPai"
                               onchange="FormDesigner.updateField('filterParam', this.value)">
                    </div>
                </div>
            </div>

            <!-- Validações -->
            <div class="config-section">
                <h4>✅ Regras de Validação</h4>
                <div id="validationsList">
                    ${this.renderValidations(field.validations || [])}
                </div>
                <button class="add-validation-btn" onclick="FormDesigner.addValidation()">
                    + Adicionar Validação
                </button>
            </div>
        `;

        panel.innerHTML = html;
    },

    renderInputTypeOptions(selected) {
        const options = [
            { value: 'text', label: 'Text' },
            { value: 'number', label: 'Number' },
            { value: 'email', label: 'Email' },
            { value: 'tel', label: 'Telefone' },
            { value: 'date', label: 'Date' },
            { value: 'datetime-local', label: 'DateTime' },
            { value: 'time', label: 'Time' },
            { value: 'checkbox', label: 'Checkbox' },
            { value: 'select', label: 'Select (Dropdown)' },
            { value: 'textarea', label: 'TextArea' },
            { value: 'hidden', label: 'Hidden' }
        ];

        return options.map(opt =>
            `<option value="${opt.value}" ${selected === opt.value ? 'selected' : ''}>${opt.label}</option>`
        ).join('');
    },

    renderValidations(validations) {
        if (!validations || validations.length === 0) {
            return '<p style="color: #999; font-size: 0.85rem;">Nenhuma validação configurada</p>';
        }

        return validations.map((val, idx) => `
            <div class="validation-rule">
                <button class="validation-rule-remove" onclick="FormDesigner.removeValidation(${idx})">×</button>
                
                <div style="margin-bottom: 8px;">
                    <label style="font-size: 0.85rem;">Tipo:</label>
                    <select onchange="FormDesigner.updateValidation(${idx}, 'type', this.value)" 
                            style="width: 100%; padding: 6px; font-size: 0.85rem;">
                        <option value="range" ${val.type === 'range' ? 'selected' : ''}>Range (Min/Max)</option>
                        <option value="email" ${val.type === 'email' ? 'selected' : ''}>Email</option>
                        <option value="cpf" ${val.type === 'cpf' ? 'selected' : ''}>CPF</option>
                        <option value="cnpj" ${val.type === 'cnpj' ? 'selected' : ''}>CNPJ</option>
                        <option value="phone" ${val.type === 'phone' ? 'selected' : ''}>Telefone</option>
                        <option value="cep" ${val.type === 'cep' ? 'selected' : ''}>CEP</option>
                        <option value="regex" ${val.type === 'regex' ? 'selected' : ''}>Regex</option>
                    </select>
                </div>

                ${val.type === 'range' ? `
                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 8px; margin-bottom: 8px;">
                        <div>
                            <label style="font-size: 0.8rem;">Mínimo:</label>
                            <input type="number" value="${val.min || ''}" 
                                   onchange="FormDesigner.updateValidation(${idx}, 'min', this.value)"
                                   style="width: 100%; padding: 6px; font-size: 0.85rem;">
                        </div>
                        <div>
                            <label style="font-size: 0.8rem;">Máximo:</label>
                            <input type="number" value="${val.max || ''}" 
                                   onchange="FormDesigner.updateValidation(${idx}, 'max', this.value)"
                                   style="width: 100%; padding: 6px; font-size: 0.85rem;">
                        </div>
                    </div>
                ` : ''}

                ${val.type === 'regex' ? `
                    <div style="margin-bottom: 8px;">
                        <label style="font-size: 0.8rem;">Expressão Regular:</label>
                        <input type="text" value="${Utils.escapeAttr(val.pattern || '')}" 
                               placeholder="^[A-Z0-9]+$"
                               onchange="FormDesigner.updateValidation(${idx}, 'pattern', this.value)"
                               style="width: 100%; padding: 6px; font-size: 0.85rem;">
                    </div>
                ` : ''}

                <div>
                    <label style="font-size: 0.8rem;">Mensagem de Erro:</label>
                    <input type="text" value="${Utils.escapeAttr(val.message || '')}" 
                           placeholder="Digite a mensagem de erro"
                           onchange="FormDesigner.updateValidation(${idx}, 'message', this.value)"
                           style="width: 100%; padding: 6px; font-size: 0.85rem;">
                </div>
            </div>
        `).join('');
    },

    updateField(property, value) {
        const selectedField = Store.get('selectedField');
        if (!selectedField) return;

        selectedField[property] = value;

        const formFields = Store.get('formFields') || [];
        const idx = formFields.findIndex(f => f.id === selectedField.id);
        if (idx !== -1) {
            formFields[idx] = selectedField;
            Store.set('formFields', formFields);
        }

        if (property === 'inputType') {
            const selectConfig = document.getElementById('selectConfig');
            if (selectConfig) {
                selectConfig.style.display = value === 'select' ? 'block' : 'none';
            }
        }

        this.renderCanvas();
    },

    toggleCascade(enabled) {
        const cascadeOptions = document.getElementById('cascadeOptions');
        if (cascadeOptions) {
            cascadeOptions.style.display = enabled ? 'block' : 'none';
        }

        if (!enabled) {
            this.updateField('cascadeTo', '');
            this.updateField('filterParam', '');
        }
    },

    addValidation() {
        const selectedField = Store.get('selectedField');
        if (!selectedField) return;

        if (!selectedField.validations) {
            selectedField.validations = [];
        }

        selectedField.validations.push({
            type: 'range',
            min: '',
            max: '',
            pattern: '',
            message: ''
        });

        const formFields = Store.get('formFields') || [];
        const idx = formFields.findIndex(f => f.id === selectedField.id);
        if (idx !== -1) {
            formFields[idx] = selectedField;
            Store.set('formFields', formFields);
        }

        this.renderConfigPanel(selectedField);
    },

    removeValidation(index) {
        const selectedField = Store.get('selectedField');
        if (!selectedField || !selectedField.validations) return;

        selectedField.validations.splice(index, 1);

        const formFields = Store.get('formFields') || [];
        const idx = formFields.findIndex(f => f.id === selectedField.id);
        if (idx !== -1) {
            formFields[idx] = selectedField;
            Store.set('formFields', formFields);
        }

        this.renderConfigPanel(selectedField);
    },

    updateValidation(index, property, value) {
        const selectedField = Store.get('selectedField');
        if (!selectedField || !selectedField.validations || !selectedField.validations[index]) return;

        selectedField.validations[index][property] = value;

        if (property === 'type') {
            const formFields = Store.get('formFields') || [];
            const idx = formFields.findIndex(f => f.id === selectedField.id);
            if (idx !== -1) {
                formFields[idx] = selectedField;
                Store.set('formFields', formFields);
            }
            this.renderConfigPanel(selectedField);
        }
    }
};

// Registra módulo
App.registerModule('FormDesigner', FormDesigner);
window.FormDesigner = FormDesigner;

console.log('✅ FormDesigner v2.1 carregado');