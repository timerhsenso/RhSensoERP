// =============================================================================
// API CLIENT v4.6 FIXED v2 - RESPEITA CONFIGURAÇÕES DO USUÁRIO
// =============================================================================
// 
// ✅ CORRIGIDO v2: Filtro de colunas agora usa === true ao invés de !== false
// ✅ CORRIGIDO v2: Apenas colunas MARCADAS são incluídas (visible === true)
// ✅ TESTADO: Funciona 100% com GridConfig e FormDesigner
// 
// MUDANÇAS v4.6 FIXED v2:
// - collectGridColumns() SEMPRE pega APENAS colunas com visible === true
// - Linha ~407: filter(c => c.visible === true) ao invés de filter(c => c.visible !== false)
// - Log detalhado mostrando: total de colunas, marcadas, e nomes das incluídas
// 
// MUDANÇAS v4.6 FIXED v1:
// - collectGridColumns() SEMPRE pega do GridConfig.config.columns (se configurado)
// - collectFormFields() SEMPRE pega do Store.formFields (se configurado)
// - Auto-geração APENAS se usuário não configurou NADA
// - Log detalhado para debug
// 
// ✅ v4.7: CORRIGIDO moduleName - agora envia chave correta e prioriza JSON do manifesto
// ✅ v4.7: CORRIGIDO displayName - pré-leitura do entity.displayName
// 
// =============================================================================

const ApiClient = {
    generatedFiles: [],

    // =========================================================================
    // DOWNLOAD ZIP
    // =========================================================================

    async downloadZip() {
        try {
            const data = this.collectWizardData();

            if (!data.entityName) {
                alert('Nome da entidade é obrigatório');
                return;
            }

            if (!data.cdFuncao) {
                alert('CdFuncao é obrigatório');
                return;
            }

            console.log('📤 Dados:', data);

            const url = '/api/generator/download-zip';
            console.log('🌐 URL:', url);

            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Erro ${response.status}: ${errorText}`);
            }

            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = downloadUrl;
            a.download = `${data.entityName}_Frontend.zip`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(downloadUrl);
            a.remove();

            alert('ZIP baixado com sucesso!');

        } catch (error) {
            console.error('❌ Erro:', error);
            alert('Erro ao baixar ZIP: ' + error.message);
        }
    },

    // =========================================================================
    // GERAR CÓDIGO
    // =========================================================================

    async generate() {
        try {
            const data = this.collectWizardData();

            if (!data.entityName) {
                alert('Nome da entidade é obrigatório');
                return null;
            }

            if (!data.cdFuncao) {
                alert('CdFuncao é obrigatório');
                return null;
            }

            const url = '/api/generator/generate';
            console.log('🌐 URL:', url);

            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Erro ${response.status}: ${errorText}`);
            }

            const result = await response.json();

            if (result.success) {
                this.generatedFiles = result.files || [];
                this.displayGeneratedFiles(result);
                alert(`${result.files.length} arquivo(s) gerado(s)!`);
            } else {
                alert('Erro: ' + result.error);
            }

            return result;

        } catch (error) {
            console.error('❌ Erro:', error);
            alert('Erro ao gerar código: ' + error.message);
            return null;
        }
    },

    // =========================================================================
    // ✅ v4.6 FIXED: COLETA DADOS RESPEITANDO CONFIGURAÇÕES DO USUÁRIO
    // ✅ v4.7: CORRIGIDO moduleName e displayName
    // =========================================================================

    collectWizardData() {
        try {
            const entity = Store.get('entity') || {};

            console.log('═══════════════════════════════════════════════════════');
            console.log('📦 COLETANDO DADOS DO WIZARD v4.7');
            console.log('═══════════════════════════════════════════════════════');
            console.log('📋 Entity:', entity.entityName);

            // =================================================================
            // ApiRoute
            // =================================================================
            let apiRoute = entity.route || entity.apiRoute || entity.Route ||
                entity.ApiRoute || entity.apiroute || '';

            console.log('🛤️  ApiRoute:', apiRoute);

            // =================================================================
            // FormLayout
            // =================================================================
            let formLayout = {
                columns: 2,
                useTabs: false,
                tabs: []
            };

            if (typeof FormDesigner !== 'undefined' && FormDesigner.layoutConfig) {
                formLayout = {
                    columns: FormDesigner.layoutConfig.columns || 2,
                    useTabs: FormDesigner.layoutConfig.useTabs || false,
                    tabs: FormDesigner.layoutConfig.tabs || ['Dados Gerais']
                };
                console.log('📐 FormLayout:', formLayout);
            }

            // =================================================================
            // CdSistema e Módulo
            // ✅ v4.7: PRIORIDADE: entity.moduleName (JSON) > moduloMap > fallback
            // =================================================================

            let cdSistema = 'RHU';

            if (entity.permissions && entity.permissions.cdSistema) {
                cdSistema = entity.permissions.cdSistema;
            } else if (entity.cdSistema) {
                cdSistema = entity.cdSistema;
            } else {
                cdSistema = document.getElementById('cdSistema')?.value || 'RHU';
            }

            console.log('🔑 CdSistema:', cdSistema);

            const moduloMap = {
                'SEG': 'Seguranca',
                'RHU': 'GestaoDePessoas',
                'GTC': 'GestaoDeTerceiros',
                'CAP': 'ControleAcessoPortaria',
                'CPO': 'ControleDePonto',
                'TRE': 'TreinamentoDesenvolvimento',
                'MSO': 'SaudeOcupacional',
                'AVA': 'Avaliacao',
                'ESO': 'eSocial',
                'EPI': 'GestaoDeEpi'
            };

            // ✅ v4.7: PRIORIDADE 1 → moduleName do JSON (já vem correto do manifesto!)
            // PRIORIDADE 2 → lookup por CdSistema (pode não casar com nome real da pasta)
            // PRIORIDADE 3 → fallback
            let modulo = entity.moduleName || entity.modulo || entity.Module || '';

            if (!modulo) {
                modulo = moduloMap[cdSistema] || 'Common';
                console.log('📦 Módulo (via CdSistema lookup):', modulo);
            } else {
                console.log('📦 Módulo (do JSON manifesto):', modulo);
            }

            // =================================================================
            // Auto-gera CdFuncao
            // =================================================================
            let cdFuncao = document.getElementById('cdFuncao')?.value || '';

            if (!cdFuncao && entity.entityName) {
                cdFuncao = `${cdSistema}_FM_${entity.entityName.toUpperCase()}`;
                console.log('🔧 CdFuncao auto-gerado:', cdFuncao);
            }

            // =================================================================
            // DisplayName
            // =================================================================
            let displayName = document.getElementById('displayName')?.value ||
                entity.displayName || entity.DisplayName || entity.entityName || '';

            console.log('📝 DisplayName:', displayName);

            // =================================================================
            // IconClass
            // =================================================================
            const iconMap = {
                'RHU': 'fas fa-users',
                'TRE': 'fas fa-graduation-cap',
                'MSO': 'fas fa-heartbeat',
                'GTC': 'fas fa-hard-hat',
                'CAP': 'fas fa-door-open',
                'SEG': 'fas fa-shield-alt',
                'EPI': 'fas fa-vest',
                'ESO': 'fas fa-file-invoice',
                'AVA': 'fas fa-star',
                'CPO': 'fas fa-clock'
            };

            let iconClass = document.getElementById('iconClass')?.value || iconMap[cdSistema] || 'fas fa-folder';
            console.log('🎨 IconClass:', iconClass);

            // =================================================================
            // ✅ CRÍTICO: COLETAR FORM FIELDS (RESPEITA USUÁRIO)
            // =================================================================
            const formFields = this.collectFormFields(entity);
            console.log('📝 FormFields coletados:', formFields.length);
            if (formFields.length > 0) {
                console.log('   Campos:', formFields.map(f => f.name).join(', '));
            }

            // =================================================================
            // ✅ CRÍTICO: COLETAR GRID COLUMNS (RESPEITA USUÁRIO)
            // =================================================================
            const gridColumns = this.collectGridColumns(entity);
            console.log('📊 GridColumns coletados:', gridColumns.length);
            if (gridColumns.length > 0) {
                console.log('   Colunas:', gridColumns.map(c => c.Name).join(', '));
            }

            // =================================================================
            // OPÇÕES DE GERAÇÃO
            // =================================================================
            const gerarWebController = document.getElementById('optWebController')?.checked ?? true;
            const gerarWebModels = document.getElementById('optWebModels')?.checked ?? true;
            const gerarWebServices = document.getElementById('optWebServices')?.checked ?? true;
            const gerarView = document.getElementById('optView')?.checked ?? true;
            const gerarJavaScript = document.getElementById('optJavaScript')?.checked ?? true;
            const gerarEntidade = document.getElementById('optEntidade')?.checked ?? false;

            // =================================================================
            // MONTA OBJETO FINAL (camelCase para backend converter)
            // ✅ v4.7: Chave corrigida de "module" para "moduleName"
            // ✅ v4.7: Adicionado apiRoute no payload
            // =================================================================
            const data = {
                entityName: entity.entityName || '',
                tableName: entity.tableName || '',
                moduleName: modulo,                // ✅ v4.7: ERA "module" → AGORA "moduleName" (casa com [JsonPropertyName("moduleName")])
                cdSistema: cdSistema,
                cdFuncao: cdFuncao,
                displayName: displayName,
                iconClass: iconClass,
                apiRoute: apiRoute,                // ✅ v4.7: Garante envio do apiRoute

                // ✅ Configurações do Grid e Form
                gridColumns: gridColumns,
                formFields: formFields,
                formLayout: formLayout,

                // Opções de geração
                gerarWebController: gerarWebController,
                gerarWebModels: gerarWebModels,
                gerarWebServices: gerarWebServices,
                gerarView: gerarView,
                gerarJavaScript: gerarJavaScript,
                gerarEntidade: gerarEntidade
            };

            console.log('═══════════════════════════════════════════════════════');
            console.log('✅ DADOS COLETADOS COM SUCESSO v4.7');
            console.log('   - moduleName:', data.moduleName);
            console.log('   - apiRoute:', data.apiRoute);
            console.log('   - cdFuncao:', data.cdFuncao);
            console.log('   - displayName:', data.displayName);
            console.log('   - FormFields:', data.formFields.length);
            console.log('   - GridColumns:', data.gridColumns.length);
            console.log('═══════════════════════════════════════════════════════');

            return data;

        } catch (error) {
            console.error('❌ Erro em collectWizardData:', error);
            alert('Erro ao coletar dados: ' + error.message);
            return {};
        }
    },

    // =========================================================================
    // ✅ v4.6 FIXED: COLETA FORM FIELDS - RESPEITA USUÁRIO PRIMEIRO
    // =========================================================================

    collectFormFields(entity) {
        try {
            // 1️⃣ PRIORIDADE 1: Verifica se usuário arrastou campos no canvas
            const userFields = Store.get('formFields') || [];

            if (userFields.length > 0) {
                console.log('✅ Usando campos configurados pelo usuário (FormDesigner):', userFields.length);

                return userFields.map(field => ({
                    name: field.name || field.Name,
                    label: field.label || field.Label || this._formatDisplayName(field.name),
                    inputType: field.inputType || field.InputType || this._getDefaultInputType(field.type),
                    required: field.required !== undefined ? field.required : !field.nullable,
                    placeholder: field.placeholder || field.Placeholder || '',
                    colSize: field.colSize || field.ColSize || this._getDefaultColSize(field.type, field.maxLength),
                    maxLength: field.maxLength || field.MaxLength || null,
                    validations: field.validations || [],
                    isSelect: field.inputType === 'select' || field.InputType === 'select',
                    selectOptions: field.selectOptions || field.SelectOptions || null,
                    isSelect2Ajax: field.isSelect2Ajax || field.IsSelect2Ajax || false,
                    apiEndpoint: field.apiEndpoint || field.ApiEndpoint || '',
                    displayField: field.displayField || field.DisplayField || 'nome',
                    valueField: field.valueField || field.ValueField || 'id',
                    order: field.order !== undefined ? field.order : 0,
                    tabGroup: field.tabGroup || field.TabGroup || 'Dados Gerais'
                }));
            }

            // 2️⃣ PRIORIDADE 2: Auto-gera APENAS campos editáveis
            console.log('⚠️  Nenhum campo configurado pelo usuário. Auto-gerando campos editáveis...');

            if (!entity || !entity.properties) {
                console.warn('❌ Entity sem properties. Retornando array vazio.');
                return [];
            }

            const properties = entity.properties || [];

            return properties
                .filter(prop => {
                    // Respeita form.showOnCreate do JSON
                    if (prop.form && prop.form.showOnCreate === false) {
                        return false;
                    }

                    const name = (prop.name || '').toLowerCase();

                    // Exclui PK auto-incremento
                    if ((prop.isPrimaryKey || prop.isIdentity) && name === 'id') {
                        return false;
                    }

                    // Exclui navegações (isReadOnly)
                    if (prop.isReadOnly) {
                        return false;
                    }

                    // Exclui auditoria
                    if (this._isAuditField(name)) {
                        return false;
                    }

                    return true;
                })
                .map((prop, idx) => ({
                    name: prop.name,
                    label: prop.displayName || this._formatDisplayName(prop.name),
                    inputType: this._getDefaultInputType(prop.type),
                    required: !prop.nullable,
                    placeholder: prop.description || '',
                    colSize: this._getDefaultColSize(prop.type, prop.maxLength),
                    maxLength: prop.maxLength || null,
                    validations: [],
                    isSelect: prop.isForeignKey || false,
                    selectOptions: null,
                    isSelect2Ajax: prop.isForeignKey || false,
                    apiEndpoint: '',
                    displayField: 'nome',
                    valueField: 'id',
                    order: idx,
                    tabGroup: 'Dados Gerais'
                }));

        } catch (error) {
            console.error('❌ Erro em collectFormFields:', error);
            return [];
        }
    },

    // =========================================================================
    // ✅ v4.6 FIXED: COLETA GRID COLUMNS - RESPEITA USUÁRIO PRIMEIRO
    // =========================================================================

    collectGridColumns(entity) {
        try {
            // 1️⃣ PRIORIDADE 1: Verifica se usuário configurou colunas no GridConfig
            if (typeof GridConfig !== 'undefined' && GridConfig.config && GridConfig.config.columns) {
                // ✅ v4.6 FIXED v2: Filtrar APENAS colunas com visible === true (marcadas pelo usuário)
                const userColumns = GridConfig.config.columns.filter(c => {
                    const isVisible = c.Visible === true || c.visible === true;
                    return isVisible;
                });

                console.log('📊 GridConfig.config.columns total:', GridConfig.config.columns.length);
                console.log('📊 Colunas marcadas (visible=true):', userColumns.length);

                if (userColumns.length > 0) {
                    console.log('✅ Usando colunas configuradas pelo usuário (GridConfig):', userColumns.length);
                    console.log('   Colunas:', userColumns.map(c => c.Name || c.name).join(', '));

                    return userColumns.map(col => ({
                        Name: col.Name || col.name,
                        Title: col.Title || col.title || this._formatDisplayName(col.Name || col.name),
                        Visible: true, // ✅ Sempre true (já filtrado)
                        Order: col.Order !== undefined ? col.Order : (col.order !== undefined ? col.order : 0),
                        Format: col.Format || col.format || '',
                        Width: col.Width || col.width || null,
                        Align: col.Align || col.align || 'left',
                        Sortable: col.Sortable !== undefined ? col.Sortable : (col.sortable !== undefined ? col.sortable : true)
                    }));
                }
            }

            // 2️⃣ PRIORIDADE 2: Auto-gera até 8 colunas principais
            console.log('⚠️  Nenhuma coluna configurada pelo usuário. Auto-gerando até 8 colunas...');

            if (!entity || !entity.properties) {
                console.warn('❌ Entity sem properties. Retornando array vazio.');
                return [];
            }

            const properties = entity.properties || [];

            return properties
                .filter(prop => {
                    // Respeita list.show do JSON v4.3
                    if (prop.list && prop.list.show === false) {
                        return false;
                    }

                    const name = (prop.name || '').toLowerCase();
                    const isAudit = this._isAuditField(name);
                    return !isAudit;
                })
                .slice(0, 8)
                .map((prop, idx) => {
                    const propType = (prop.type || 'string').toLowerCase();

                    return {
                        Name: prop.name,
                        Title: prop.displayName || this._formatDisplayName(prop.name),
                        Visible: true,
                        Order: idx,
                        Format: this._getFormatFromType(propType),
                        Width: null,
                        Align: 'left',
                        Sortable: true
                    };
                });

        } catch (error) {
            console.error('❌ Erro em collectGridColumns:', error);
            return [];
        }
    },

    // =========================================================================
    // HELPERS PRIVADOS (com _)
    // =========================================================================

    _isAuditField(name) {
        if (!name) return false;
        const normalized = name.toLowerCase();

        const auditKeywords = [
            'datacriacao', 'usuariocriacao', 'dataatualizacao', 'usuarioatualizacao',
            'dtcriacao', 'dtatualizacao', 'idsaas', 'id_saas',
            'createdat', 'updatedat', 'createdby', 'updatedby',
            'tenantid', 'rowversion', 'timestamp'
        ];

        return auditKeywords.some(keyword => normalized.includes(keyword));
    },

    _formatDisplayName(name) {
        if (!name) return name;

        try {
            let formatted = name
                .replace(/^cd/i, 'Código ')
                .replace(/^nm/i, '')
                .replace(/^dt/i, 'Data ')
                .replace(/^id/i, '')
                .replace(/^fl/i, '');

            formatted = formatted.replace(/([A-Z])/g, ' $1').trim();

            return formatted.charAt(0).toUpperCase() + formatted.slice(1);
        } catch (error) {
            return name;
        }
    },

    _getDefaultInputType(type) {
        if (!type) return 'text';

        const typeLower = type.toLowerCase();

        if (typeLower.includes('date') && !typeLower.includes('time')) return 'date';
        if (typeLower.includes('datetime')) return 'datetime-local';
        if (typeLower.includes('bool')) return 'checkbox';
        if (typeLower.includes('int') || typeLower.includes('decimal')) return 'number';
        if (typeLower.includes('email')) return 'email';

        return 'text';
    },

    _getDefaultColSize(type, maxLength) {
        const typeLower = (type || '').toLowerCase();

        if (typeLower.includes('bool')) return 2;
        if (typeLower.includes('date')) return 4;
        if (typeLower.includes('int') || typeLower.includes('decimal')) return 3;
        if (maxLength && maxLength < 20) return 3;
        if (maxLength && maxLength > 255) return 12;

        return 6;
    },

    _getFormatFromType(type) {
        if (!type) return 'text';

        const typeLower = type.toLowerCase();

        if (typeLower.includes('date') && !typeLower.includes('time')) return 'date';
        if (typeLower.includes('datetime')) return 'datetime';
        if (typeLower.includes('bool')) return 'boolean';
        if (typeLower.includes('decimal') || typeLower.includes('money')) return 'currency';

        return 'text';
    },

    // =========================================================================
    // EXIBIÇÃO
    // =========================================================================

    displayGeneratedFiles(result) {
        const container = document.getElementById('generationResult');
        const filesList = document.getElementById('generatedFilesList');
        const previewSection = document.getElementById('codePreviewSection');

        if (!container || !filesList) return;

        if (!result.files || result.files.length === 0) {
            filesList.innerHTML = '<p>Nenhum arquivo gerado.</p>';
            container.style.display = 'block';
            return;
        }

        let html = '<div class="generated-files-grid">';
        result.files.forEach((file, idx) => {
            const icon = this.getFileIcon(file.fileType);
            html += `
                <div class="generated-file-item">
                    <span class="file-icon">${icon}</span>
                    <div class="file-info">
                        <strong>${file.fileName}</strong>
                        <small>${file.relativePath}</small>
                    </div>
                    <div class="file-actions">
                        <button class="btn btn-sm btn-outline-primary" onclick="ApiClient.previewFile(${idx})">
                            👁️ Ver
                        </button>
                        <button class="btn btn-sm btn-outline-success" onclick="ApiClient.copyFile(${idx})">
                            📋 Copiar
                        </button>
                    </div>
                </div>
            `;
        });
        html += '</div>';

        filesList.innerHTML = html;
        container.style.display = 'block';

        if (result.files.length > 0 && previewSection) {
            this.previewFile(0);
            previewSection.style.display = 'block';
        }
    },

    getFileIcon(fileType) {
        const icons = {
            'Controller': '🎮',
            'Model': '📦',
            'View': '🖼️',
            'JavaScript': '⚡',
            'Entity': '🛍️',
            'Service': '⚙️'
        };
        return icons[fileType] || '📄';
    },

    previewFile(index) {
        const file = this.generatedFiles[index];
        if (!file) return;

        const tabsHeader = document.getElementById('codeTabsHeader');
        const tabsContent = document.getElementById('codeTabsContent');

        if (tabsHeader) {
            let tabsHtml = '';
            this.generatedFiles.forEach((f, idx) => {
                const active = idx === index ? 'active' : '';
                tabsHtml += `<button class="code-tab ${active}" onclick="ApiClient.previewFile(${idx})">${f.fileName}</button>`;
            });
            tabsHeader.innerHTML = tabsHtml;
        }

        if (tabsContent) {
            const escaped = file.content
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;');
            tabsContent.innerHTML = `<pre class="code-preview"><code>${escaped}</code></pre>`;
        }
    },

    async copyFile(index) {
        const file = this.generatedFiles[index];
        if (!file) return;

        try {
            await navigator.clipboard.writeText(file.content);
            alert(`✅ ${file.fileName} copiado!`);
        } catch (err) {
            console.error('Erro ao copiar:', err);
            const textarea = document.createElement('textarea');
            textarea.value = file.content;
            document.body.appendChild(textarea);
            textarea.select();
            document.execCommand('copy');
            document.body.removeChild(textarea);
            alert(`✅ ${file.fileName} copiado!`);
        }
    }
};

window.ApiClient = ApiClient;

console.log('✅ ApiClient v4.7 - moduleName corrigido + Respeita configurações do usuário (Grid + Form)');