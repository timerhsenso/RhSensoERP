/**
 * SCHEMA VALIDATOR MODULE v2.0
 * Compara estrutura da Entidade com Schema do Banco via API
 * 
 * Endpoints utilizados:
 * - GET /api/schema/table/{tableName}?database={db}
 * - GET /api/schema/tables?filter={filter}
 */

const SchemaValidator = {
    // Estado
    dbSchema: null,
    comparisonResult: null,
    isLoading: false,

    init() {
        console.log('🔍 Schema Validator v2.0 initialized');
    },

    // =========================================
    // RENDER DA ETAPA 2
    // =========================================
    render() {
        const container = document.getElementById('schemaValidatorContent');
        if (!container) {
            console.warn('SchemaValidator: container #schemaValidatorContent não encontrado');
            return;
        }

        const entity = Store.get('entity');
        const tableName = entity?.tableName || 'Não definido';
        const lastDatabase = this.getLastDatabase();

        // Renderiza o conteúdo
        container.innerHTML = this.getTemplate(entity, tableName, lastDatabase);
        
        console.log('✅ SchemaValidator renderizado');
    },

    getTemplate(entity, tableName, lastDatabase) {
        const baseUrl = ManifestManager?.baseUrl || localStorage.getItem('lastManifestUrl') || 'https://localhost:7193';
        
        return `
            <!-- Info da Entidade -->
            <div class="schema-info-card">
                <div class="schema-info-row">
                    <span class="schema-info-label">📋 Entidade:</span>
                    <span class="schema-info-value">${Utils.escapeHtml(entity?.entityName || 'Não carregada')}</span>
                </div>
                <div class="schema-info-row">
                    <span class="schema-info-label">🗄️ Tabela:</span>
                    <span class="schema-info-value">${Utils.escapeHtml(tableName)}</span>
                    <span class="schema-info-hint">(detectada do JSON)</span>
                </div>
            </div>

            <!-- Carregar do Banco -->
            <div class="schema-loader-section">
                <h4>🔌 Carregar Schema do Banco</h4>
                
                <div class="schema-loader-row">
                    <div class="schema-input-group">
                        <label>Banco de dados:</label>
                        <input type="text" 
                               id="schemaDatabaseInput" 
                               class="schema-database-input"
                               placeholder="(vazio = usa DefaultConnection)"
                               value="${Utils.escapeHtml(lastDatabase)}">
                        <span class="schema-input-hint">Ex: bd_rhu_copenor</span>
                    </div>
                    
                    <div class="schema-input-group">
                        <label>Nome da tabela:</label>
                        <input type="text" 
                               id="schemaTableInput" 
                               class="schema-table-input"
                               placeholder="TB_FUNCIONARIOS"
                               value="${Utils.escapeHtml(tableName !== 'Não definido' ? tableName : '')}">
                    </div>
                </div>

                <div class="schema-loader-actions">
                    <button class="btn btn-primary" onclick="SchemaValidator.loadFromDatabase()" id="btnLoadSchema">
                        🔄 Carregar Schema do Banco
                    </button>
                    <button class="btn btn-secondary btn-small" onclick="SchemaValidator.toggleManualInput()">
                        📋 Colar JSON Manual
                    </button>
                </div>

                <!-- Input Manual (hidden by default) -->
                <div id="manualSchemaInput" class="manual-schema-section" style="display: none;">
                    <label>JSON do Schema (formato do SchemaController):</label>
                    <textarea id="dbSchemaInput" class="schema-textarea" placeholder='{
  "tableName": "TB_EXEMPLO",
  "columns": [
    { "name": "Id", "type": "int", "sqlType": "int", "isNullable": false, "isPrimaryKey": true }
  ]
}'></textarea>
                    <button class="btn btn-secondary" onclick="SchemaValidator.compareFromManual()">
                        🔄 Comparar com JSON
                    </button>
                </div>

                <!-- Loading -->
                <div id="schemaLoading" class="schema-loading" style="display: none;">
                    <div class="spinner"></div>
                    <span>Carregando schema do banco...</span>
                </div>

                <!-- Status -->
                <div id="schemaStatus" class="schema-status" style="display: none;"></div>
            </div>

            <!-- Resultado da Comparação -->
            <div id="comparisonResult" class="schema-comparison" style="display: none;">
                <h4>📊 Resultado da Comparação</h4>
                
                <!-- Resumo -->
                <div id="comparisonSummary" class="comparison-summary"></div>
                
                <!-- Tabela de Comparação -->
                <div class="comparison-table-wrapper">
                    <table class="comparison-table">
                        <thead>
                            <tr>
                                <th>Status</th>
                                <th>Propriedade (Entidade)</th>
                                <th>Tipo C#</th>
                                <th>Coluna (Banco)</th>
                                <th>Tipo SQL</th>
                                <th>Detalhes</th>
                            </tr>
                        </thead>
                        <tbody id="comparisonBody"></tbody>
                    </table>
                </div>

                <!-- Colunas extras no banco -->
                <div id="extraColumnsSection" style="display: none;">
                    <h5>⚠️ Colunas no banco que não existem na entidade:</h5>
                    <div id="extraColumnsList" class="extra-columns-list"></div>
                    <button class="btn btn-warning btn-small" onclick="SchemaValidator.addMissingColumns()">
                        ➕ Adicionar todas à entidade
                    </button>
                </div>
            </div>
        `;
    },

    // =========================================
    // CARREGAR DO BANCO VIA API
    // =========================================
    async loadFromDatabase() {
        const tableInput = document.getElementById('schemaTableInput');
        const dbInput = document.getElementById('schemaDatabaseInput');
        const tableName = tableInput?.value?.trim();
        const database = dbInput?.value?.trim();

        if (!tableName) {
            this.showStatus('⚠️ Informe o nome da tabela', 'warning');
            tableInput?.focus();
            return;
        }

        // Salva último banco usado
        if (database) {
            localStorage.setItem('lastSchemaDatabase', database);
        }

        const baseUrl = ManifestManager?.baseUrl || localStorage.getItem('lastManifestUrl') || 'https://localhost:7193';
        let url = `${baseUrl}/api/schema/table/${encodeURIComponent(tableName)}`;
        
        if (database) {
            url += `?database=${encodeURIComponent(database)}`;
        }

        this.setLoading(true);
        this.showStatus('', 'info');

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || `HTTP ${response.status}`);
            }

            const schema = await response.json();
            this.dbSchema = schema;
            Store.set('dbSchema', schema);

            this.showStatus(`✅ Schema carregado: ${schema.columnCount} colunas`, 'success');
            this.compare();

        } catch (error) {
            console.error('Erro ao carregar schema:', error);
            this.showStatus(`❌ Erro: ${error.message}`, 'error');
        } finally {
            this.setLoading(false);
        }
    },

    // =========================================
    // COMPARAÇÃO
    // =========================================
    compare() {
        const entity = Store.get('entity');
        const dbSchema = this.dbSchema || Store.get('dbSchema');

        if (!entity) {
            this.showStatus('⚠️ Nenhuma entidade carregada. Volte à Etapa 1.', 'warning');
            return;
        }

        if (!dbSchema || !dbSchema.columns) {
            this.showStatus('⚠️ Schema do banco não carregado.', 'warning');
            return;
        }

        // Mapeia colunas do banco por nome (case insensitive)
        const dbColumnsMap = new Map();
        dbSchema.columns.forEach(col => {
            dbColumnsMap.set(col.name.toLowerCase(), col);
        });

        // Mapeia propriedades da entidade por nome
        const entityPropsMap = new Map();
        entity.properties.forEach(prop => {
            entityPropsMap.set(prop.name.toLowerCase(), prop);
        });

        const results = [];
        let okCount = 0;
        let warningCount = 0;
        let errorCount = 0;

        // Compara propriedades da entidade com banco
        entity.properties.forEach(prop => {
            const dbCol = dbColumnsMap.get(prop.name.toLowerCase());
            const result = this.compareProperty(prop, dbCol);
            results.push(result);

            if (result.status === 'ok') okCount++;
            else if (result.status === 'warning') warningCount++;
            else errorCount++;
        });

        // Encontra colunas do banco que não existem na entidade
        const extraColumns = [];
        dbSchema.columns.forEach(col => {
            if (!entityPropsMap.has(col.name.toLowerCase())) {
                extraColumns.push(col);
            }
        });

        this.comparisonResult = { results, extraColumns, okCount, warningCount, errorCount };
        this.renderComparison();
    },

    compareProperty(prop, dbCol) {
        if (!dbCol) {
            return {
                status: 'error',
                statusIcon: '❌',
                statusText: 'Não encontrada',
                prop: prop,
                dbCol: null,
                details: 'Coluna não existe no banco'
            };
        }

        // Verifica compatibilidade de tipos
        const typeCheck = this.checkTypeCompatibility(prop.type, dbCol.type, dbCol.sqlType);

        if (typeCheck.compatible) {
            // Verifica nullable
            const nullableMatch = this.checkNullable(prop, dbCol);
            
            if (!nullableMatch.match) {
                return {
                    status: 'warning',
                    statusIcon: '⚠️',
                    statusText: 'Nullable diferente',
                    prop: prop,
                    dbCol: dbCol,
                    details: nullableMatch.details
                };
            }

            // Verifica maxLength para strings
            if (prop.type === 'string' && prop.maxLength && dbCol.maxLength) {
                if (prop.maxLength > dbCol.maxLength) {
                    return {
                        status: 'warning',
                        statusIcon: '⚠️',
                        statusText: 'MaxLength excede',
                        prop: prop,
                        dbCol: dbCol,
                        details: `Entidade: ${prop.maxLength}, Banco: ${dbCol.maxLength}`
                    };
                }
            }

            return {
                status: 'ok',
                statusIcon: '✅',
                statusText: 'OK',
                prop: prop,
                dbCol: dbCol,
                details: typeCheck.details || ''
            };
        }

        return {
            status: 'warning',
            statusIcon: '⚠️',
            statusText: 'Tipo diferente',
            prop: prop,
            dbCol: dbCol,
            details: typeCheck.details
        };
    },

    checkTypeCompatibility(entityType, dbCSharpType, dbSqlType) {
        const normalizedEntity = entityType?.toLowerCase().replace('?', '') || '';
        const normalizedDb = dbCSharpType?.toLowerCase().replace('?', '') || '';
        const normalizedSql = dbSqlType?.toLowerCase() || '';

        // Mapeamento de tipos compatíveis
        const compatibilityMap = {
            'string': ['string'],
            'int': ['int', 'short', 'byte', 'long'],
            'long': ['long', 'int'],
            'short': ['short', 'int', 'byte'],
            'byte': ['byte', 'short', 'int'],
            'decimal': ['decimal', 'double', 'float'],
            'double': ['double', 'decimal', 'float'],
            'float': ['float', 'double', 'decimal'],
            'bool': ['bool'],
            'datetime': ['datetime', 'dateonly', 'datetimeoffset'],
            'dateonly': ['dateonly', 'datetime'],
            'timeonly': ['timeonly', 'datetime'],
            'guid': ['guid'],
            'byte[]': ['byte[]']
        };

        const compatibleTypes = compatibilityMap[normalizedEntity] || [];
        
        if (compatibleTypes.includes(normalizedDb)) {
            return { compatible: true, details: '' };
        }

        // Se não é exatamente compatível, verifica se é conversível
        if (normalizedEntity === normalizedDb) {
            return { compatible: true, details: '' };
        }

        return { 
            compatible: false, 
            details: `Entidade: ${entityType} → Banco: ${dbCSharpType} (${dbSqlType})`
        };
    },

    checkNullable(prop, dbCol) {
        const propNullable = prop.nullable ?? prop.isNullable ?? true;
        const dbNullable = dbCol.isNullable ?? true;

        // Se entidade diz NOT NULL mas banco permite NULL = OK (banco mais permissivo)
        // Se entidade diz NULL mas banco diz NOT NULL = Warning
        if (!propNullable && dbNullable) {
            return { match: true, details: '' };
        }

        if (propNullable && !dbNullable) {
            return { 
                match: false, 
                details: 'Entidade permite NULL, banco não permite'
            };
        }

        return { match: true, details: '' };
    },

    // =========================================
    // RENDER DO RESULTADO
    // =========================================
    renderComparison() {
        const resultDiv = document.getElementById('comparisonResult');
        const summaryDiv = document.getElementById('comparisonSummary');
        const bodyDiv = document.getElementById('comparisonBody');
        const extraSection = document.getElementById('extraColumnsSection');
        const extraList = document.getElementById('extraColumnsList');

        if (!resultDiv || !this.comparisonResult) return;

        const { results, extraColumns, okCount, warningCount, errorCount } = this.comparisonResult;

        // Resumo
        summaryDiv.innerHTML = `
            <div class="summary-item summary-ok">
                <span class="summary-count">${okCount}</span>
                <span class="summary-label">✅ OK</span>
            </div>
            <div class="summary-item summary-warning">
                <span class="summary-count">${warningCount}</span>
                <span class="summary-label">⚠️ Avisos</span>
            </div>
            <div class="summary-item summary-error">
                <span class="summary-count">${errorCount}</span>
                <span class="summary-label">❌ Erros</span>
            </div>
            <div class="summary-item summary-extra">
                <span class="summary-count">${extraColumns.length}</span>
                <span class="summary-label">📋 Extras no banco</span>
            </div>
        `;

        // Tabela de comparação
        bodyDiv.innerHTML = results.map(r => `
            <tr class="comparison-row comparison-${r.status}">
                <td class="status-cell">
                    <span class="status-badge status-${r.status}">${r.statusIcon} ${r.statusText}</span>
                </td>
                <td class="prop-cell">
                    <strong>${Utils.escapeHtml(r.prop.name)}</strong>
                    ${r.prop.isPrimaryKey ? '<span class="badge badge-pk">PK</span>' : ''}
                    ${r.prop.isForeignKey ? '<span class="badge badge-fk">FK</span>' : ''}
                </td>
                <td class="type-cell">${Utils.escapeHtml(r.prop.type)}</td>
                <td class="col-cell">
                    ${r.dbCol ? Utils.escapeHtml(r.dbCol.name) : '<span class="not-found">—</span>'}
                </td>
                <td class="type-cell">
                    ${r.dbCol ? Utils.escapeHtml(r.dbCol.sqlType) : '—'}
                </td>
                <td class="details-cell">
                    ${r.details ? `<span class="details-text">${Utils.escapeHtml(r.details)}</span>` : ''}
                </td>
            </tr>
        `).join('');

        // Colunas extras no banco
        if (extraColumns.length > 0) {
            extraSection.style.display = 'block';
            extraList.innerHTML = extraColumns.map(col => `
                <div class="extra-column-item">
                    <span class="extra-col-name">${Utils.escapeHtml(col.name)}</span>
                    <span class="extra-col-type">${Utils.escapeHtml(col.type)} (${Utils.escapeHtml(col.sqlType)})</span>
                    ${col.isPrimaryKey ? '<span class="badge badge-pk">PK</span>' : ''}
                    ${col.isForeignKey ? '<span class="badge badge-fk">FK</span>' : ''}
                    ${col.isNullable ? '<span class="badge badge-null">NULL</span>' : ''}
                    <button class="btn btn-xs btn-outline" onclick="SchemaValidator.addSingleColumn('${Utils.escapeHtml(col.name)}')">
                        ➕
                    </button>
                </div>
            `).join('');
        } else {
            extraSection.style.display = 'none';
        }

        resultDiv.style.display = 'block';
    },

    // =========================================
    // ADICIONAR COLUNAS FALTANTES
    // =========================================
    addSingleColumn(columnName) {
        const dbSchema = this.dbSchema || Store.get('dbSchema');
        const entity = Store.get('entity');
        
        if (!dbSchema || !entity) return;

        const column = dbSchema.columns.find(c => c.name === columnName);
        if (!column) return;

        const newProp = this.columnToProperty(column);
        entity.properties.push(newProp);
        Store.set('entity', entity);

        // Atualiza textarea da Etapa 1
        const jsonInput = document.getElementById('jsonInput');
        if (jsonInput) {
            jsonInput.value = JSON.stringify(entity, null, 2);
        }

        this.showStatus(`✅ Coluna "${columnName}" adicionada à entidade`, 'success');
        this.compare(); // Refaz comparação
    },

    addMissingColumns() {
        const { extraColumns } = this.comparisonResult || {};
        if (!extraColumns || extraColumns.length === 0) return;

        const entity = Store.get('entity');
        if (!entity) return;

        extraColumns.forEach(col => {
            const newProp = this.columnToProperty(col);
            entity.properties.push(newProp);
        });

        Store.set('entity', entity);

        // Atualiza textarea da Etapa 1
        const jsonInput = document.getElementById('jsonInput');
        if (jsonInput) {
            jsonInput.value = JSON.stringify(entity, null, 2);
        }

        this.showStatus(`✅ ${extraColumns.length} colunas adicionadas à entidade`, 'success');
        this.compare(); // Refaz comparação
    },

    columnToProperty(column) {
        return {
            name: column.name,
            type: column.type.replace('?', ''), // Remove nullable do tipo
            nullable: column.isNullable,
            maxLength: column.maxLength || null,
            isPrimaryKey: column.isPrimaryKey || false,
            isForeignKey: column.isForeignKey || false,
            foreignKeyEntity: column.foreignKeyTable || null
        };
    },

    // =========================================
    // INPUT MANUAL
    // =========================================
    toggleManualInput() {
        const manualSection = document.getElementById('manualSchemaInput');
        if (!manualSection) return;

        const isHidden = manualSection.style.display === 'none';
        manualSection.style.display = isHidden ? 'block' : 'none';
    },

    compareFromManual() {
        const dbInput = document.getElementById('dbSchemaInput')?.value;

        if (!dbInput || !dbInput.trim()) {
            this.showStatus('⚠️ Cole o JSON do schema', 'warning');
            return;
        }

        try {
            const dbSchema = JSON.parse(dbInput);
            
            // Normaliza formato (pode vir do SchemaController ou formato antigo)
            if (!dbSchema.columns && dbSchema.Columns) {
                dbSchema.columns = dbSchema.Columns;
            }

            this.dbSchema = dbSchema;
            Store.set('dbSchema', dbSchema);

            this.showStatus(`✅ Schema carregado: ${dbSchema.columns?.length || 0} colunas`, 'success');
            this.compare();

        } catch (e) {
            this.showStatus(`❌ Erro no JSON: ${e.message}`, 'error');
        }
    },

    // =========================================
    // HELPERS
    // =========================================
    getLastDatabase() {
        return localStorage.getItem('lastSchemaDatabase') || '';
    },

    setLoading(loading) {
        this.isLoading = loading;
        const loadingDiv = document.getElementById('schemaLoading');
        const btnLoad = document.getElementById('btnLoadSchema');
        
        if (loadingDiv) {
            loadingDiv.style.display = loading ? 'flex' : 'none';
        }
        if (btnLoad) {
            btnLoad.disabled = loading;
            btnLoad.textContent = loading ? '⏳ Carregando...' : '🔄 Carregar Schema do Banco';
        }
    },

    showStatus(message, type = 'info') {
        const status = document.getElementById('schemaStatus');
        if (!status) return;

        if (!message) {
            status.style.display = 'none';
            return;
        }

        const colors = {
            'info': '#2196F3',
            'success': '#4CAF50',
            'warning': '#FF9800',
            'error': '#F44336'
        };

        status.innerHTML = `<span style="color: ${colors[type]}">${message}</span>`;
        status.style.display = 'block';

        if (type === 'success') {
            setTimeout(() => { status.style.display = 'none'; }, 5000);
        }
    }
};

// Registra módulo
App.registerModule('SchemaValidator', SchemaValidator);

// Mantém compatibilidade com função global antiga
window.compareSchemas = () => SchemaValidator.compare();
