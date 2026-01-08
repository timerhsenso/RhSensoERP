/**
 * =============================================================================
 * GRID CONFIG MODULE v1.9 CORRIGIDO
 * Configuração avançada de colunas, filtros e exportação
 * =============================================================================
 * CHANGELOG v1.9 CORRIGIDO:
 * - ✅ TODOS os confirm() substituídos por Swal.fire()
 * - ✅ TODOS os alert() substituídos por Swal.fire()
 * - ✅ Campo "Largura" corrigido: 10px → 100px
 * - 🎨 Modais SweetAlert2 em: removeColumn, reloadAllColumns, removeSelectedColumns
 * 
 * CHANGELOG v1.8 FINAL:
 * - ✅ NOVO: Clicar na linha toggle checkbox (marcar/desmarcar)
 * - ✅ NOVO: Campos de auditoria bloqueados (checkbox disabled + sem botão 🗑️)
 * - 🎨 Visual diferenciado para campos de auditoria (opacity + cursor not-allowed)
 * - 🔧 handleColumnClick() - Toggle checkbox ao clicar na linha
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
        _configVersion: 1.9 // ✅ v1.9 - Todos os modais SweetAlert2
    },

    // =========================================================================
    // INICIALIZAÇÃO
    // =========================================================================
    init() {
        console.log('📊 Grid Config v1.9 CORRIGIDO initialized (todos os modais SweetAlert2)');

        const saved = localStorage.getItem('gridConfig');
        if (saved) {
            try {
                const parsed = JSON.parse(saved);
                // Cache bust if version is different
                if (parsed._configVersion !== this.config._configVersion) {
                    console.log('⚠️ Old config detected. Clearing cache to apply new features.');
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
    // CONFIGURAÇÃO DE COLUNAS
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

            // Pega TODAS as propriedades
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
                    Name: prop.name,
                    visible: isVisible,
                    Visible: isVisible,
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
                    Title: prop.displayName || prop.name,
                    Order: this.config.columns.length,
                    isAudit: isAudit
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
            <div style="margin-bottom: 15px; display: flex; gap: 10px; flex-wrap: wrap;">
                <button class="btn btn-small btn-primary" onclick="GridConfig.selectAllColumns(true)"
                        ${visibleCount === totalColumns ? 'disabled' : ''}>
                    ✅ Selecionar Todas
                </button>
                <button class="btn btn-small btn-secondary" onclick="GridConfig.selectAllColumns(false)"
                        ${visibleCount === 0 ? 'disabled' : ''}>
                    ❌ Desmarcar Todas
                </button>
                <button class="btn btn-small btn-delete-selected" onclick="GridConfig.removeSelectedColumns()" 
                        title="Excluir todas as colunas marcadas da lista"
                        ${visibleCount === 0 ? 'disabled' : ''}>
                    🗑️ Excluir Selecionados
                </button>
                <button class="btn btn-small btn-reload" onclick="GridConfig.reloadAllColumns()" 
                        title="Recarregar todas as colunas da entidade">
                    🔄 Recarregar Todas
                </button>
                <span style="margin-left: auto; color: #666; font-size: 12px; align-self: center;">
                    ${visibleCount} de ${totalColumns} marcadas
                </span>
            </div>
            
            <!-- Lista de colunas SEM SCROLL -->
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
            col.Visible = visible;
        });
        this.save();
        this.renderColumnConfig();

        if (typeof App !== 'undefined' && App.showToast) {
            App.showToast(visible ? '✅ Todas as colunas selecionadas' : '❌ Todas as colunas desmarcadas', 'success');
        }
    },

    // =========================================================================
    // ✅ v1.9: REMOVER COLUNA DA LISTA (SweetAlert2)
    // =========================================================================
    removeColumn(idx) {
        const col = this.config.columns[idx];
        const columnName = col.name;

        Swal.fire({
            title: 'Remover Coluna?',
            html: `Deseja remover a coluna <strong>"${columnName}"</strong> da lista?<br><small class="text-muted">(Não afetará a geração, apenas limpa a interface)</small>`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-check me-2"></i>Sim, Remover',
            cancelButtonText: '<i class="fas fa-times me-2"></i>Cancelar',
            confirmButtonColor: '#ef4444',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                this.config.columns.splice(idx, 1);
                this.save();
                this.renderColumnConfig();

                Swal.fire({
                    title: 'Removido!',
                    text: `Coluna "${columnName}" removida da lista.`,
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        });
    },

    // =========================================================================
    // ✅ v1.9: RECARREGAR TODAS AS COLUNAS (SweetAlert2)
    // =========================================================================
    reloadAllColumns() {
        Swal.fire({
            title: 'Recarregar Colunas?',
            html: `
                <p>Deseja recarregar todas as colunas da entidade?</p>
                <small class="text-muted">⚠️ Isso irá <strong>resetar</strong> suas configurações de colunas.</small>
            `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-sync me-2"></i>Sim, Recarregar',
            cancelButtonText: '<i class="fas fa-times me-2"></i>Cancelar',
            confirmButtonColor: '#0099cc',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                const entity = Store.get('entity');

                if (!entity) {
                    Swal.fire({
                        title: 'Erro!',
                        text: 'Nenhuma entidade carregada no Store!',
                        icon: 'error',
                        confirmButtonColor: '#ef4444'
                    });
                    return;
                }

                // Forçar reload limpando o array
                this.config.columns = [];
                this.config._entityName = null;
                this.save();

                // Re-renderizar (irá recriar as colunas)
                this.renderColumnConfig();

                Swal.fire({
                    title: 'Recarregado!',
                    text: 'Todas as colunas foram recarregadas com sucesso!',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        });
    },

    // =========================================================================
    // ✅ v1.9: EXCLUIR TODAS AS COLUNAS SELECIONADAS (SweetAlert2)
    // =========================================================================
    removeSelectedColumns() {
        // Contar quantas colunas estão marcadas
        const selectedColumns = this.config.columns.filter(c => c.visible === true || c.Visible === true);
        const selectedCount = selectedColumns.length;

        if (selectedCount === 0) {
            Swal.fire({
                title: 'Atenção!',
                text: 'Nenhuma coluna marcada para excluir!',
                icon: 'warning',
                confirmButtonText: 'OK',
                confirmButtonColor: '#0099cc'
            });
            return;
        }

        // Listar nomes das colunas que serão excluídas
        const previewNames = selectedCount > 5
            ? selectedColumns.slice(0, 5).map(c => c.name).join(', ') + ` e mais ${selectedCount - 5}...`
            : selectedColumns.map(c => c.name).join(', ');

        // Confirmar com o usuário
        Swal.fire({
            title: 'Excluir Selecionados?',
            html: `
                <p>Deseja excluir <strong>${selectedCount} coluna${selectedCount > 1 ? 's' : ''} marcada${selectedCount > 1 ? 's' : ''}</strong>?</p>
                <div style="background: #f8f9fa; padding: 12px; border-radius: 8px; margin: 16px 0; max-height: 150px; overflow-y: auto;">
                    <strong style="color: #0099cc;">Colunas:</strong><br>
                    <small style="color: #666;">${previewNames}</small>
                </div>
                <small class="text-muted">⚠️ Não afetará a geração, apenas limpa a interface.</small>
            `,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: '<i class="fas fa-trash me-2"></i>Sim, Excluir Todas',
            cancelButtonText: '<i class="fas fa-times me-2"></i>Cancelar',
            confirmButtonColor: '#ef4444',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                // Remover todas as colunas marcadas (manter apenas as desmarcadas)
                this.config.columns = this.config.columns.filter(c => c.visible !== true && c.Visible !== true);

                this.save();
                this.renderColumnConfig();

                Swal.fire({
                    title: 'Removidas!',
                    text: `${selectedCount} coluna${selectedCount > 1 ? 's' : ''} removida${selectedCount > 1 ? 's' : ''} da lista.`,
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        });
    },

    // =========================================================================
    // RENDERIZA ITEM DE COLUNA
    // =========================================================================
    renderColumnItem(col, idx) {
        // ✅ v1.8: Campos de auditoria são bloqueados (não pode marcar nem excluir)
        const isAuditLocked = col.isAudit === true;
        const disabledAttr = isAuditLocked ? 'disabled' : '';
        const auditClass = isAuditLocked ? 'column-item-audit' : '';

        return `
            <div class="column-item ${auditClass}" data-index="${idx}" draggable="true"
                 onclick="GridConfig.handleColumnClick(event, ${idx}, ${isAuditLocked})">
                <!-- Drag Handle -->
                <div class="column-drag-handle" title="Arrastar para reordenar">☰</div>
                
                <!-- Checkbox -->
                <div class="column-checkbox">
                    <input type="checkbox" ${col.visible ? 'checked' : ''} 
                           ${disabledAttr}
                           onchange="GridConfig.updateColumn(${idx}, 'visible', this.checked)"
                           onclick="event.stopPropagation()"
                           title="${isAuditLocked ? 'Campo de auditoria (bloqueado)' : 'Visível na Grid'}">
                </div>
                
                <!-- Nome da Coluna -->
                <div class="column-name">
                    <strong>${Utils.escapeHtml(col.name)}</strong>
                    ${col.isAudit ? '<span class="badge badge-warning" style="font-size: 9px; margin-left: 5px;">AUDIT</span>' : ''}
                </div>
                
                <!-- Opções -->
                <div class="column-options" onclick="event.stopPropagation()">
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
                           style="width: 100px !important; max-width: 100px !important; min-width: 80px !important;"
                           onchange="GridConfig.updateColumn(${idx}, 'width', this.value)">

                    <label title="Ordenável">
                        <input type="checkbox" ${col.sortable ? 'checked' : ''} 
                               onchange="GridConfig.updateColumn(${idx}, 'sortable', this.checked)">
                        Sort
                    </label>
                </div>
                
                <!-- Botão de Excluir (APENAS se NÃO for auditoria) -->
                ${!isAuditLocked ? `
                <button class="btn-delete-column" onclick="GridConfig.removeColumn(${idx}); event.stopPropagation();" 
                        title="Excluir esta coluna da lista">
                    🗑️
                </button>
                ` : ''}
            </div>
        `;
    },

    // =========================================================================
    // CLICK NA LINHA PARA TOGGLE CHECKBOX
    // =========================================================================
    handleColumnClick(event, idx, isAuditLocked) {
        // Não fazer nada se for campo de auditoria
        if (isAuditLocked) {
            return;
        }

        // Não fazer nada se clicou em um elemento interativo
        const target = event.target;
        const isInteractive = target.tagName === 'INPUT' ||
            target.tagName === 'SELECT' ||
            target.tagName === 'BUTTON' ||
            target.closest('.column-options') ||
            target.closest('.btn-delete-column') ||
            target.closest('.column-drag-handle');

        if (isInteractive) {
            return;
        }

        // Toggle do checkbox
        const col = this.config.columns[idx];
        const newValue = !col.visible;

        this.updateColumn(idx, 'visible', newValue);

        // Re-renderizar para atualizar o checkbox visualmente
        this.renderColumnConfig();
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

        // Atualizar também a versão PascalCase
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

console.log('✅ GridConfig v1.9 CORRIGIDO carregado (todos os modais SweetAlert2 + campo Largura 100px)');