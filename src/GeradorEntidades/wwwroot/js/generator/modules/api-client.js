// =============================================================================
// API CLIENT v3.9 - LÓGICA CORRETA
// =============================================================================
// CHANGELOG v3.9:
// - 🎯 CORREÇÃO: Auto-geração de formFields com lógica CORRETA
//   • Exclui campos com form.showOnCreate === false
//   • Exclui campos isReadOnly
//   • Exclui campos de auditoria
//   • Respeita configurações do JSON v4.3
// 
// v3.7 - Auto-gera CdFuncao, campos e colunas
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
    // ✅ v3.9: COLETA DADOS COM LÓGICA CORRETA
    // =========================================================================

    collectWizardData() {
        try {
            const entity = Store.get('entity') || {};
            const formFields = Store.get('formFields') || [];

            console.log('📋 Entity:', entity);
            console.log('📋 FormFields:', formFields);

            // =================================================================
            // ApiRoute
            // =================================================================
            let apiRoute = entity.route || entity.apiRoute || entity.Route ||
                entity.ApiRoute || entity.apiroute || '';

            console.log('🛤️ ApiRoute:', apiRoute);

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
            // =================================================================
            const cdSistema = document.getElementById('cdSistema')?.value || 'RHU';

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

            const modulo = moduloMap[cdSistema] || 'Common';
            console.log('📦 Módulo:', modulo);

            // =================================================================
            // Auto-gera CdFuncao
            // =================================================================
            let cdFuncao = document.getElementById('cdFuncao')?.value || '';

            if (!cdFuncao && entity.entityName) {
                cdFuncao = `${cdSistema}_FM_${entity.entityName.toUpperCase()}`;
                console.log('🔧 CdFuncao auto-gerado:', cdFuncao);
            }

            // =================================================================
            // Mapeia ícones
            // =================================================================
            const iconMap = {
                'SEG': 'fas fa-shield-alt',
                'RHU': 'fas fa-users',
                'GTC': 'fas fa-hard-hat',
                'CAP': 'fas fa-door-open',
                'CPO': 'fas fa-clock',
                'TRE': 'fas fa-certificate',
                'MSO': 'fas fa-heartbeat',
                'AVA': 'fas fa-chart-line',
                'ESO': 'fas fa-file-alt',
                'EPI': 'fas fa-vest'
            };

            const icon = document.getElementById('iconClass')?.value ||
                iconMap[cdSistema] ||
                'fas fa-table';

            console.log('🎨 Ícone:', icon);

            // =================================================================
            // ✅ v3.9: Auto-preenche FormFields COM LÓGICA CORRETA
            // =================================================================
            let finalFormFields = formFields;

            if (!formFields || formFields.length === 0) {
                console.warn('⚠️ FormFields vazio! Auto-preenchendo...');

                const properties = entity.properties || [];

                finalFormFields = properties
                    .filter(prop => {
                        const name = (prop.name || '').toLowerCase();

                        // 1. Exclui campos de auditoria
                        const isAudit = this._isAuditField(name);
                        if (isAudit) {
                            console.log(`   ❌ Excluído (auditoria): ${prop.name}`);
                            return false;
                        }

                        // 2. Respeita form.showOnCreate do JSON v4.3
                        if (prop.form) {
                            if (prop.form.show === false || prop.form.showOnCreate === false) {
                                console.log(`   ❌ Excluído (form config): ${prop.name}`);
                                return false;
                            }
                        }

                        // 3. Exclui campos ReadOnly (não editáveis)
                        if (prop.isReadOnly) {
                            console.log(`   ❌ Excluído (isReadOnly): ${prop.name}`);
                            return false;
                        }

                        // 4. MANTÉM campos editáveis
                        console.log(`   ✅ Incluído: ${prop.name} (editável)`);
                        return true;
                    })
                    .map((prop, idx) => {
                        const propType = (prop.type || 'string').toLowerCase();

                        return {
                            name: prop.name,
                            label: prop.displayName || this._formatDisplayName(prop.name),
                            type: prop.type || 'string',
                            inputType: this._getDefaultInputType(propType),
                            colSize: this._getDefaultColSize(propType, prop.maxLength),
                            order: idx,
                            tab: null,
                            group: 'Dados Gerais',
                            required: prop.required || false,
                            placeholder: '',
                            helpText: prop.description || '',
                            maxLength: prop.maxLength || null,

                            // Preserva configurações do JSON v4.3
                            isReadOnly: prop.isReadOnly || false,
                            disabled: prop.form?.disabled || prop.isReadOnly || false
                        };
                    });

                console.log('✅ Auto-gerados', finalFormFields.length, 'campos');
            }

            // =================================================================
            // collectGridColumns
            // =================================================================
            const gridColumns = this.collectGridColumns();

            // =================================================================
            // Retorno
            // =================================================================
            return {
                entityName: entity.entityName || entity.name || '',
                tableName: entity.tableName || entity.TableName || '',
                cdFuncao: cdFuncao,
                cdSistema: cdSistema,
                displayName: document.getElementById('displayName')?.value ||
                    entity.displayName || '',
                icon: icon,
                menuOrder: 10,
                apiRoute: apiRoute,
                modulo: modulo,

                gerarEntidade: document.getElementById('optEntidade')?.checked || false,
                gerarWebController: document.getElementById('optWebController')?.checked ?? true,
                gerarWebModels: document.getElementById('optWebModels')?.checked ?? true,
                gerarWebServices: document.getElementById('optWebServices')?.checked ?? true,
                gerarView: document.getElementById('optView')?.checked ?? true,
                gerarJavaScript: document.getElementById('optJavaScript')?.checked ?? true,

                gridColumns: gridColumns,
                formLayout: formLayout,

                formFields: finalFormFields.map((f, idx) => ({
                    name: f.name,
                    label: f.label || f.displayName || f.name,
                    type: f.type || 'string',
                    inputType: f.inputType || 'text',
                    colSize: parseInt(f.colSize, 10) || 6,
                    order: idx,
                    tab: f.tab || null,
                    group: f.group || 'Dados Gerais',
                    required: f.required || false,
                    placeholder: f.placeholder || '',
                    helpText: f.helpText || '',
                    mask: f.mask || null,
                    maxLength: f.maxLength ? parseInt(f.maxLength, 10) : null,
                    disabled: f.disabled || false,
                    isReadOnly: f.isReadOnly || false
                }))
            };

        } catch (error) {
            console.error('❌ Erro em collectWizardData:', error);
            alert('Erro ao coletar dados: ' + error.message);
            throw error;
        }
    },

    // =========================================================================
    // collectGridColumns
    // =========================================================================

    collectGridColumns() {
        try {
            // Tenta pegar do GridConfig
            if (typeof GridConfig !== 'undefined' &&
                GridConfig.config?.columns &&
                GridConfig.config.columns.length > 0) {

                return GridConfig.config.columns.map((c, idx) => ({
                    name: c.name,
                    title: c.title || c.name,
                    visible: c.visible !== false,
                    order: idx,
                    format: c.format || 'text',
                    width: c.width || null,
                    align: c.align || 'left',
                    sortable: c.sortable !== false
                }));
            }

            // ✅ FALLBACK: Auto-gera
            console.warn('⚠️ GridColumns vazio! Auto-preenchendo...');

            const entity = Store.get('entity') || {};
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
                        name: prop.name,
                        title: prop.displayName || this._formatDisplayName(prop.name),
                        visible: true,
                        order: idx,
                        format: this._getFormatFromType(propType),
                        width: null,
                        align: 'left',
                        sortable: true
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

console.log('✅ ApiClient v3.9 carregado - LÓGICA CORRETA (Form: apenas editáveis / Grid: tudo exceto auditoria)');