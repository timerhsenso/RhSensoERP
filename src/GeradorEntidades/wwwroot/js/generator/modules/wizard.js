/**
 * WIZARD MODULE
 * Gerencia navegação entre etapas
 */

const Wizard = {
    totalSteps: 5,

    init() {
        console.log('📍 Wizard module initialized');
        
        // Subscribe para mudanças de step
        Store.subscribe((key, value) => {
            if (key === 'currentStep' || key === 'reset') {
                this.render();
            }
        });
    },

    // Vai para uma etapa específica
    goToStep(step) {
        if (step < 0 || step >= this.totalSteps) {
            console.warn('Invalid step:', step);
            return;
        }

        Store.set('currentStep', step);
        this.render();

        // Executa ações específicas da etapa
        this.onStepEnter(step);
    },

    // Avança para próxima etapa
    next() {
        const current = Store.get('currentStep');
        if (current < this.totalSteps - 1) {
            this.goToStep(current + 1);
        }
    },

    // Volta para etapa anterior
    previous() {
        const current = Store.get('currentStep');
        if (current > 0) {
            this.goToStep(current - 1);
        }
    },

    // Valida etapa atual e avança
    validateAndNext(step) {
        const validations = {
            0: () => this.validateJsonImport(),
            1: () => this.validateSchemaComparison(),
            2: () => this.validateGridConfig(),
            3: () => this.validateFormDesigner()
        };

        const validator = validations[step];
        if (validator && !validator()) {
            return false;
        }

        this.next();
        return true;
    },

    // Validação da etapa 0: Import JSON
    validateJsonImport() {
        const jsonInput = document.getElementById('jsonInput')?.value;
        
        if (!jsonInput || !jsonInput.trim()) {
            App.showError('jsonError', 'Por favor, insira o JSON da entidade');
            return false;
        }

        try {
            const entity = JSON.parse(jsonInput);
            
            if (!entity.entityName) {
                throw new Error('JSON deve ter "entityName"');
            }
            
            if (!entity.properties || !Array.isArray(entity.properties)) {
                throw new Error('JSON deve ter array "properties"');
            }

            if (entity.properties.length === 0) {
                throw new Error('Array "properties" não pode estar vazio');
            }

            // Valida cada propriedade
            entity.properties.forEach((prop, idx) => {
                if (!prop.name) {
                    throw new Error(`Propriedade ${idx + 1} deve ter "name"`);
                }
                if (!prop.type) {
                    throw new Error(`Propriedade "${prop.name}" deve ter "type"`);
                }
            });

            // Verifica se a entidade mudou - se sim, limpa dados dependentes
            const previousEntity = Store.get('entity');
            const entityChanged = !previousEntity || previousEntity.entityName !== entity.entityName;
            
            if (entityChanged) {
                // Salva estado para undo antes de limpar
                App.saveUndoState('mudança de entidade');
                
                console.log('🔄 Entidade mudou, limpando configurações anteriores...');
                
                // Limpa formFields
                Store.set('formFields', []);
                Store.set('fieldCounter', 0);
                
                // Limpa dbSchema
                Store.set('dbSchema', null);
                
                // Limpa GridConfig
                if (App.modules.GridConfig) {
                    App.modules.GridConfig.config.columns = [];
                    App.modules.GridConfig.config.filters = [];
                    App.modules.GridConfig.config._entityName = null;
                    App.modules.GridConfig.save();
                }
                
                // Limpa SchemaValidator
                if (App.modules.SchemaValidator) {
                    App.modules.SchemaValidator.dbSchema = null;
                    App.modules.SchemaValidator.comparisonResult = null;
                }
            }

            // Salva no estado
            Store.set('entity', entity);
            
            App.hideElement('jsonError');
            App.showSuccess('jsonSuccess', 
                `JSON válido! Entidade: <strong>${Utils.escapeHtml(entity.entityName)}</strong> ` +
                `com ${entity.properties.length} propriedades.` +
                (entityChanged ? ' <em>(configurações anteriores foram limpas)</em>' : '')
            );

            return true;

        } catch (e) {
            App.showError('jsonError', 'Erro: ' + e.message);
            App.hideElement('jsonSuccess');
            return false;
        }
    },

    // Validação da etapa 1: Comparação de Schema
    validateSchemaComparison() {
        // Comparação é opcional, sempre permite avançar
        return true;
    },

    // Validação da etapa 2: Configuração do Grid
    validateGridConfig() {
        // Valida se tem pelo menos uma coluna visível
        const gridConfig = App.modules.GridConfig?.config;
        if (gridConfig) {
            const visibleColumns = gridConfig.columns.filter(c => c.visible);
            if (visibleColumns.length === 0) {
                alert('⚠️ Selecione pelo menos uma coluna para exibir na grid!');
                return false;
            }
        }
        return true;
    },

    // Validação da etapa 3: Form Designer
    validateFormDesigner() {
        const formFields = Store.get('formFields');
        
        if (!formFields || formFields.length === 0) {
            alert('⚠️ Arraste pelo menos um campo para o formulário!');
            return false;
        }

        return true;
    },

    // Ações ao entrar em uma etapa
    onStepEnter(step) {
        switch (step) {
            case 0:
                // Renderiza manifest manager
                if (App.modules.ManifestManager) {
                    App.modules.ManifestManager.render();
                }
                break;
            case 1:
                // Renderiza schema validator
                if (App.modules.SchemaValidator) {
                    App.modules.SchemaValidator.render();
                }
                break;
            case 2:
                // Renderiza configuração do grid
                if (App.modules.GridConfig) {
                    App.modules.GridConfig.render();
                }
                break;
            case 3:
                // Renderiza form designer
                if (App.modules.FormDesigner) {
                    App.modules.FormDesigner.render();
                }
                break;
            case 4:
                // Gera código
                if (App.modules.CodeGenerator) {
                    App.modules.CodeGenerator.generateAll();
                }
                break;
        }
    },

    // Renderiza o wizard
    render() {
        const currentStep = Store.get('currentStep');

        // Atualiza indicadores de step
        document.querySelectorAll('.step').forEach((el, idx) => {
            el.classList.remove('active', 'completed');
            if (idx === currentStep) {
                el.classList.add('active');
            } else if (idx < currentStep) {
                el.classList.add('completed');
            }
        });

        // Atualiza painéis de conteúdo
        document.querySelectorAll('.step-content').forEach(el => {
            el.classList.remove('active');
        });

        const currentContent = document.querySelector(`.step-content[data-step="${currentStep}"]`);
        if (currentContent) {
            currentContent.classList.add('active');
        }
    }
};

// Registra módulo
App.registerModule('Wizard', Wizard);

// Expõe globalmente para onclick handlers
window.validateAndNext = (step) => Wizard.validateAndNext(step);
window.goToStep = (step) => Wizard.goToStep(step);
