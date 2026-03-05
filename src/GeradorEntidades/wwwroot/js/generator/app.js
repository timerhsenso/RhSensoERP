/**
 * =============================================================================
 * RAZOR PAGE GENERATOR v2.1
 * Aplicação Principal e State Management
 * =============================================================================
 * CHANGELOG v2.1:
 * - Reset mais robusto (limpa todos os dados corretamente)
 * - Melhoria no sistema de Undo
 * - Função forceReset para casos extremos
 * =============================================================================
 */

// ===========================================
// STATE MANAGEMENT
// ===========================================
const Store = {
    state: {
        currentStep: 0,
        entity: null,
        dbSchema: null,
        displayColumns: [],
        searchColumns: [],
        formFields: [],
        selectedField: null,
        fieldCounter: 0
    },

    listeners: [],

    getState() {
        return this.state;
    },

    get(key) {
        return this.state[key];
    },

    set(key, value) {
        this.state[key] = value;
        this.persist();
        this.notify(key, value);
    },

    setMultiple(updates) {
        Object.keys(updates).forEach(key => {
            this.state[key] = updates[key];
        });
        this.persist();
        this.notify('multiple', updates);
    },

    // Reset COMPLETO do estado
    reset() {
        console.log('🔄 Store.reset() - Limpando estado...');

        this.state = {
            currentStep: 0,
            entity: null,
            dbSchema: null,
            displayColumns: [],
            searchColumns: [],
            formFields: [],
            selectedField: null,
            fieldCounter: 0
        };

        this.persist();
        this.notify('reset', null);

        console.log('✅ Estado resetado');
    },

    subscribe(listener) {
        this.listeners.push(listener);
        return () => {
            this.listeners = this.listeners.filter(l => l !== listener);
        };
    },

    notify(key, value) {
        this.listeners.forEach(listener => {
            try {
                listener(key, value, this.state);
            } catch (e) {
                console.error('Error in store listener:', e);
            }
        });
    },

    persist() {
        try {
            localStorage.setItem('razorGeneratorState', JSON.stringify(this.state));
        } catch (e) {
            console.warn('Could not persist state:', e);
        }
    },

    restore() {
        try {
            const saved = localStorage.getItem('razorGeneratorState');
            if (saved) {
                const parsed = JSON.parse(saved);
                // Merge com valores default para garantir que todas as propriedades existam
                this.state = {
                    currentStep: 0,
                    entity: null,
                    dbSchema: null,
                    displayColumns: [],
                    searchColumns: [],
                    formFields: [],
                    selectedField: null,
                    fieldCounter: 0,
                    ...parsed
                };
                return true;
            }
        } catch (e) {
            console.warn('Could not restore state:', e);
        }
        return false;
    },

    // Limpa completamente o localStorage
    clearAll() {
        console.log('🗑️ Limpando todo o localStorage...');
        localStorage.removeItem('razorGeneratorState');
        localStorage.removeItem('gridConfig');
        localStorage.removeItem('formLayoutConfig');
        localStorage.removeItem('lastManifestUrl');
        this.reset();
    }
};

// ===========================================
// UTILIDADES
// ===========================================
const Utils = {
    escapeHtml(text) {
        if (text === null || text === undefined) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    },

    escapeAttr(text) {
        if (text === null || text === undefined) return '';
        return String(text)
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    },

    escapeScriptTag(text) {
        return text.replace(/<\/script>/gi, '<\\/script>');
    },

    generateId(prefix = 'field') {
        return `${prefix}_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    },

    capitalize(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    },

    toCamelCase(str) {
        return str.charAt(0).toLowerCase() + str.slice(1);
    },

    toPascalCase(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    },

    toKebabCase(str) {
        return str.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
    },

    downloadFile(content, filename, mimeType = 'text/plain') {
        const blob = new Blob([content], { type: mimeType });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },

    getDefaultInputType(type) {
        // ✅ v4.4: Mapa completo - PascalCase (C#/JSON manifesto) + lowercase
        const typeMap = {
            // lowercase
            'string': 'text',
            'int': 'number',
            'long': 'number',
            'short': 'number',
            'decimal': 'number',
            'float': 'number',
            'double': 'number',
            'datetime': 'datetime-local',
            'date': 'date',
            'time': 'time',
            'bool': 'checkbox',
            'boolean': 'checkbox',
            'guid': 'text',
            'email': 'email',
            // ✅ PascalCase (como vem do JSON do manifesto C#)
            'DateTime': 'datetime-local',
            'DateOnly': 'date',
            'TimeOnly': 'time',
            'Boolean': 'checkbox',
            'Guid': 'text',
            // ✅ Nullable (C# T?)
            'int?': 'number',
            'long?': 'number',
            'short?': 'number',
            'decimal?': 'number',
            'float?': 'number',
            'double?': 'number',
            'bool?': 'checkbox',
            'Guid?': 'text',
            'DateTime?': 'datetime-local',
            'DateOnly?': 'date',
            'TimeOnly?': 'time'
        };
        // Tenta match exato primeiro, depois lowercase
        return typeMap[type] || typeMap[type?.toLowerCase()] || 'text';
    },

    mapToCSharpType(type) {
        const typeMap = {
            'string': 'string',
            'int': 'int',
            'long': 'long',
            'decimal': 'decimal',
            'float': 'float',
            'double': 'double',
            'datetime': 'DateTime',
            'date': 'DateTime',
            'time': 'TimeSpan',
            'bool': 'bool',
            'boolean': 'bool',
            'guid': 'Guid'
        };
        return typeMap[type?.toLowerCase()] || 'string';
    },

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
};

// ===========================================
// APLICAÇÃO
// ===========================================
const App = {
    modules: {},

    init() {
        console.log('🚀 Razor Page Generator v2.1 iniciando...');

        // Tenta restaurar estado
        const restored = Store.restore();
        if (restored) {
            console.log('📦 Estado restaurado do localStorage');
        }

        // Inicializa módulos
        this.initModules();

        // Setup de eventos globais
        this.setupGlobalEvents();

        // Renderiza estado inicial
        this.render();

        console.log('✅ Aplicação inicializada');
    },

    registerModule(name, module) {
        this.modules[name] = module;
        if (typeof module.init === 'function') {
            module.init();
        }
    },

    initModules() {
        Object.values(this.modules).forEach(module => {
            if (typeof module.init === 'function') {
                module.init();
            }
        });
    },

    setupGlobalEvents() {
        // Navegação pelas etapas clicando nos steps
        document.querySelectorAll('.step').forEach(step => {
            step.addEventListener('click', () => {
                const stepNum = parseInt(step.dataset.step);
                const currentStep = Store.get('currentStep');
                if (stepNum <= currentStep) {
                    if (this.modules.Wizard) {
                        this.modules.Wizard.goToStep(stepNum);
                    }
                }
            });
        });

        // Atalhos de teclado
        document.addEventListener('keydown', (e) => {
            // Ctrl+S para salvar/persistir
            if (e.ctrlKey && e.key === 's') {
                e.preventDefault();
                Store.persist();
                this.showToast('💾 Estado salvo', 'success');
            }

            // Ctrl+Z para undo
            if (e.ctrlKey && e.key === 'z' && !e.shiftKey) {
                e.preventDefault();
                this.undo();
            }

            // Escape para fechar modais/popups
            if (e.key === 'Escape') {
                const propEditor = document.getElementById('fieldPropertyEditor');
                if (propEditor && propEditor.style.display !== 'none') {
                    propEditor.style.display = 'none';
                }
            }
        });

        console.log('⌨️ Atalhos: Ctrl+S (salvar), Ctrl+Z (desfazer), Esc (fechar)');
    },

    render() {
        const currentStep = Store.get('currentStep');

        document.querySelectorAll('.step').forEach((el, idx) => {
            el.classList.remove('active', 'completed');
            if (idx === currentStep) {
                el.classList.add('active');
            } else if (idx < currentStep) {
                el.classList.add('completed');
            }
        });

        document.querySelectorAll('.step-content').forEach(el => {
            el.classList.remove('active');
        });

        const currentContent = document.querySelector(`.step-content[data-step="${currentStep}"]`);
        if (currentContent) {
            currentContent.classList.add('active');
        }
    },

    showError(elementId, message) {
        const el = document.getElementById(elementId);
        if (el) {
            el.textContent = '❌ ' + message;
            el.style.display = 'block';
        }
    },

    showSuccess(elementId, message) {
        const el = document.getElementById(elementId);
        if (el) {
            el.innerHTML = '✅ ' + message;
            el.style.display = 'block';
        }
    },

    hideElement(elementId) {
        const el = document.getElementById(elementId);
        if (el) {
            el.style.display = 'none';
        }
    },

    // =========================================
    // RESET COMPLETO
    // =========================================
    confirmReset() {
        const entity = Store.get('entity');
        const formFields = Store.get('formFields') || [];

        if (!entity && formFields.length === 0) {
            this.showToast('ℹ️ Nada para limpar', 'info');
            return;
        }

        const message = `⚠️ Tem certeza que deseja limpar tudo?\n\n` +
            `Isso irá remover:\n` +
            `• Entidade: ${entity?.entityName || 'Nenhuma'}\n` +
            `• Campos do formulário: ${formFields.length}\n` +
            `• Todas as configurações\n\n` +
            `Esta ação não pode ser desfeita.`;

        if (confirm(message)) {
            this.saveUndoState('reset');
            this.forceReset();
        }
    },

    // Reset forçado - limpa TUDO
    forceReset() {
        console.log('🔄 Executando reset forçado...');

        // Limpa localStorage
        localStorage.removeItem('gridConfig');
        localStorage.removeItem('formLayoutConfig');
        localStorage.removeItem('razorGeneratorState');

        // Reseta módulos
        if (this.modules.GridConfig) {
            this.modules.GridConfig.config = {
                serverSide: false,
                pageSize: 10,
                exportFormats: ['excel', 'pdf', 'csv'],
                bulkActions: false,
                columns: [],
                filters: [],
                _entityName: null
            };
        }

        if (this.modules.FormDesigner) {
            this.modules.FormDesigner.layoutConfig = {
                columns: 2,
                useTabs: false,
                tabs: ['Dados Gerais']
            };
            this.modules.FormDesigner.activeTabIndex = 0;
        }

        // Reseta Store
        Store.reset();

        // Recarrega a página para garantir estado limpo
        location.reload();
    },

    // =========================================
    // SISTEMA DE UNDO
    // =========================================
    undoStack: [],
    maxUndoStates: 5,

    saveUndoState(action = 'change') {
        const state = JSON.stringify(Store.getState());
        this.undoStack.push({ action, state, timestamp: Date.now() });

        if (this.undoStack.length > this.maxUndoStates) {
            this.undoStack.shift();
        }

        console.log(`💾 Undo state saved (${action})`);
    },

    undo() {
        if (this.undoStack.length === 0) {
            this.showToast('ℹ️ Nada para desfazer', 'info');
            return;
        }

        const lastState = this.undoStack.pop();
        try {
            const state = JSON.parse(lastState.state);
            Object.keys(state).forEach(key => {
                Store.state[key] = state[key];
            });
            Store.persist();

            this.render();
            if (this.modules.Wizard) {
                this.modules.Wizard.onStepEnter(Store.get('currentStep'));
            }

            this.showToast(`↩️ Desfeito: ${lastState.action}`, 'success');
        } catch (e) {
            console.error('Erro ao desfazer:', e);
            this.showToast('❌ Erro ao desfazer', 'error');
        }
    },

    // =========================================
    // TOAST NOTIFICATIONS
    // =========================================
    showToast(message, type = 'info', duration = 3000) {
        const existingToast = document.querySelector('.app-toast');
        if (existingToast) {
            existingToast.remove();
        }

        const colors = {
            'info': '#2196F3',
            'success': '#4CAF50',
            'warning': '#FF9800',
            'error': '#F44336'
        };

        const toast = document.createElement('div');
        toast.className = 'app-toast';
        toast.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            padding: 15px 25px;
            background: ${colors[type]};
            color: white;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            z-index: 10000;
            animation: slideIn 0.3s ease;
            font-weight: 500;
            max-width: 400px;
        `;
        toast.textContent = message;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => toast.remove(), 300);
        }, duration);
    },

    // =========================================
    // VALIDAÇÃO EM TEMPO REAL DO JSON
    // =========================================
    setupJsonValidation() {
        const jsonInput = document.getElementById('jsonInput');
        if (!jsonInput) return;

        let timeout;
        jsonInput.addEventListener('input', () => {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                this.validateJsonRealtime(jsonInput.value);
            }, 500);
        });
    },

    validateJsonRealtime(value) {
        const errorEl = document.getElementById('jsonError');
        const successEl = document.getElementById('jsonSuccess');

        if (!value || !value.trim()) {
            this.hideElement('jsonError');
            this.hideElement('jsonSuccess');
            return;
        }

        try {
            const entity = JSON.parse(value);

            const issues = [];

            if (!entity.entityName) {
                issues.push('Falta "entityName"');
            }
            if (!entity.properties || !Array.isArray(entity.properties)) {
                issues.push('Falta array "properties"');
            } else if (entity.properties.length === 0) {
                issues.push('"properties" está vazio');
            } else {
                entity.properties.forEach((prop, idx) => {
                    if (!prop.name) issues.push(`Propriedade ${idx + 1}: falta "name"`);
                    if (!prop.type) issues.push(`Propriedade "${prop.name || idx + 1}": falta "type"`);
                });
            }

            if (issues.length > 0) {
                if (errorEl) {
                    errorEl.innerHTML = '⚠️ Avisos:<br>• ' + issues.join('<br>• ');
                    errorEl.style.display = 'block';
                    errorEl.style.background = '#fff3cd';
                    errorEl.style.color = '#856404';
                }
                this.hideElement('jsonSuccess');
            } else {
                if (successEl) {
                    successEl.innerHTML = `✅ JSON válido! <strong>${Utils.escapeHtml(entity.entityName)}</strong> com ${entity.properties.length} propriedades`;
                    successEl.style.display = 'block';
                }
                this.hideElement('jsonError');
            }
        } catch (e) {
            if (errorEl) {
                errorEl.innerHTML = `❌ JSON inválido: ${e.message}`;
                errorEl.style.display = 'block';
                errorEl.style.background = '#f8d7da';
                errorEl.style.color = '#721c24';
            }
            this.hideElement('jsonSuccess');
        }
    }
};

// Expõe globalmente
window.Store = Store;
window.Utils = Utils;
window.App = App;

// Auto-inicializa quando DOM estiver pronto
document.addEventListener('DOMContentLoaded', () => {
    App.init();

    App.setupJsonValidation();

    // Adiciona CSS para animações de toast
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        @keyframes slideOut {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }
    `;
    document.head.appendChild(style);
});

console.log('✅ App v2.1 carregado');