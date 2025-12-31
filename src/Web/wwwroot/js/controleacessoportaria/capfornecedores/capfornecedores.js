/**
 * ============================================================================
 * CAPFORNECEDORES - JavaScript com Checkbox e Toggle Ativo
 * ============================================================================
 * Arquivo: wwwroot/js/controleacessoportaria/capfornecedores/capfornecedores.js
 * Módulo: ControleAcessoPortaria
 * Versão: 4.3 (FINAL - 100% FUNCIONAL)
 * Gerado por: GeradorFullStack v4.3
 * Data: 2025-12-30 21:33:31
 * 
 * Changelog v4.3:
 *   ✅ CRÍTICO: Geração automática inteligente de colunas (não depende de Grid)
 *   ✅ CRÍTICO: Resolve 100% do erro "aDataSort" do DataTables
 *   ✅ CRÍTICO: Heurísticas: Form.Show, tipos comuns, ordem alfabética
 * 
 * Changelog v4.2:
 *   ✅ CORRIGIDO: dataTableColumns → columns (compatível com CrudBase)
 *   ✅ CORRIGIDO: Parâmetros obrigatórios do CrudBase adicionados
 *   ✅ CORRIGIDO: idField em lowercase, classes CSS corretas
 * 
 * Changelog v4.1:
 *   ✅ Checkbox "Selecionar Todos" no header da DataTable
 *   ✅ Toggle Switch dinâmico para campo Ativo (rate limit 500ms)
 *   ✅ Exclusão múltipla com contador
 * 
 * Implementação específica do CRUD de CapFornecedores.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class CapFornecedoresCrud extends CrudBase {
    constructor(config) {
        super(config);
        
        // =====================================================================
        // Identifica campos de PK de texto
        // =====================================================================
        this.pkTextoField = null;
        this.isPkTexto = false;
        
        // =====================================================================
        // v4.1: Debounce para Toggle Ativo
        // =====================================================================
        this.toggleDebounceTimer = null;
    }

    /**
     * Habilita/desabilita campos de chave primária.
     * PKs de texto são editáveis apenas na criação.
     */
    enablePrimaryKeyFields(enable) {
        if (!this.isPkTexto) return;
        
        const $pkField = $('#' + this.pkTextoField);
        if ($pkField.length === 0) return;
        
        if (enable) {
            // Criação: campo editável
            $pkField.prop('readonly', false)
                    .prop('disabled', false)
                    .removeClass('bg-light');
            console.log('✏️ [CapFornecedores] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                    .addClass('bg-light');
            console.log('🔒 [CapFornecedores] Campo PK desabilitado (edição)');
        }
    }

    /**
     * Override: Abre modal para NOVO registro.
     * Habilita PK de texto na criação.
     */
    openCreateModal() {
        super.openCreateModal();
        
        // Habilita PK de texto para digitação
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(true);
        }
    }

    /**
     * Override: Abre modal para EDIÇÃO.
     * Desabilita PK de texto na edição.
     */
    async openEditModal(id) {
        await super.openEditModal(id);
        
        // Desabilita PK de texto (não pode alterar chave)
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(false);
        }
    }

    /**
     * ⭐ v3.9 CORRIGIDO: Retorna objeto em PascalCase
     * Remove campos de auditoria, converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        console.log('📥 [CapFornecedores] Dados ANTES:', JSON.parse(JSON.stringify(formData)));

        // =====================================================================
        // ⭐ CRÍTICO: Remove campos de auditoria (backend preenche automaticamente)
        // =====================================================================
        delete formData.createdAtUtc;
        delete formData.updatedAtUtc;
        delete formData.createdByUserId;
        delete formData.updatedByUserId;
        delete formData.tenantId;
        delete formData.id;
        delete formData.CreatedAtUtc;
        delete formData.UpdatedAtUtc;
        delete formData.CreatedByUserId;
        delete formData.UpdatedByUserId;
        delete formData.TenantId;
        delete formData.Id;
        delete formData.dataCriacao;
        delete formData.dataAtualizacao;
        delete formData.usuarioCriacao;
        delete formData.usuarioAtualizacao;
        delete formData.createdAt;
        delete formData.updatedAt;
        delete formData.createdBy;
        delete formData.updatedBy;

        // =====================================================================
        // ⭐ v3.9: CRIA OBJETO LIMPO EM PASCALCASE (model binding ASP.NET Core)
        // =====================================================================
        const cleanData = {};

        // String fields - PascalCase
        cleanData.Bairro = formData.bairro || formData.Bairro || '';
        cleanData.Cep = formData.cep || formData.Cep || '';
        cleanData.Cidade = formData.cidade || formData.Cidade || '';
        cleanData.Cnpj = formData.cnpj || formData.Cnpj || '';
        cleanData.Complemento = formData.complemento || formData.Complemento || '';
        cleanData.Contato = formData.contato || formData.Contato || '';
        cleanData.ContatoEmail = formData.contatoEmail || formData.ContatoEmail || '';
        cleanData.ContatoTelefone = formData.contatoTelefone || formData.ContatoTelefone || '';
        cleanData.Cpf = formData.cpf || formData.Cpf || '';
        cleanData.Email = formData.email || formData.Email || '';
        cleanData.Endereco = formData.endereco || formData.Endereco || '';
        cleanData.NomeFantasia = formData.nomeFantasia || formData.NomeFantasia || '';
        cleanData.Numero = formData.numero || formData.Numero || '';
        cleanData.RazaoSocial = formData.razaoSocial || formData.RazaoSocial || '';
        cleanData.Telefone = formData.telefone || formData.Telefone || '';

        // Integer nullable fields - PascalCase
        if (formData.idUf !== undefined && formData.idUf !== null && formData.idUf !== '') {
            const val = parseInt(formData.idUf, 10);
            cleanData.IdUf = isNaN(val) ? null : val;
        } else if (formData.IdUf !== undefined && formData.IdUf !== null && formData.IdUf !== '') {
            const val = parseInt(formData.IdUf, 10);
            cleanData.IdUf = isNaN(val) ? null : val;
        } else {
            cleanData.IdUf = null;
        }

        // ⭐ Boolean fields - PascalCase - Pega direto do DOM (checkbox)
        const checkboxAtivo = document.getElementById('Ativo');
        if (checkboxAtivo) {
            cleanData.Ativo = checkboxAtivo.checked;
        } else {
            cleanData.Ativo = formData.ativo === true || 
                                    formData.Ativo === true || 
                                    formData.ativo === 'true' || 
                                    formData.ativo === 1;
        }


        console.log('📤 [CapFornecedores] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [CapFornecedores] Registro salvo:', data);
        
        // Atualiza a grid automaticamente
        if (this.dataTable) {
            this.dataTable.ajax.reload(null, false); // Mantém paginação
        }
    }

    /**
     * Override do método getRowId para extrair ID corretamente.
     */
    getRowId(row) {
        const id = row[this.config.idField] || row.id || row.Id || '';
        return typeof id === 'string' ? id.trim() : id;
    }
}

// Inicialização quando o documento estiver pronto
$(document).ready(function () {

    // =========================================================================
    // VERIFICAÇÃO DE PERMISSÕES
    // =========================================================================

    // Verifica se as permissões foram injetadas pela View
    if (typeof window.crudPermissions === 'undefined') {
        console.error('❌ Permissões não foram carregadas! Usando valores padrão.');
        window.crudPermissions = {
            canCreate: false,
            canEdit: false,
            canDelete: false,
            canView: true
        };
    }

    console.log('🔐 [CapFornecedores] Permissões ativas:', window.crudPermissions);

    // =========================================================================
    // FUNÇÃO AUXILIAR: Extrai ID com trim e validação
    // =========================================================================

    function getCleanId(row, fieldName) {
        if (!row) return '';

        // Tenta várias variações do nome do campo
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] || 
                 row['id'] || row['Id'] || '';

        // Converte para string e faz trim
        id = String(id).trim();

        // Log para debug
        if (!id) {
            console.warn('⚠️ [CapFornecedores] ID vazio para row:', row);
        }

        return id;
    }

    // =========================================================================
    // ✅ v4.2: CONFIGURAÇÃO DAS COLUNAS (CORRIGIDO)
    // =========================================================================

    const columns = [
        // =====================================================================
        // v4.1: COLUNA DE SELEÇÃO (CHECKBOX)
        // =====================================================================
        {
            data: null,
            name: 'Select',
            title: '<input type="checkbox" id="selectAll" class="form-check-input" />',
            orderable: false,
            searchable: false,
            width: '30px',
            className: 'text-center no-export',
            render: function (data, type, row) {
                const id = getCleanId(row, 'id');
                return `<input type="checkbox" class="form-check-input row-select dt-checkboxes" value="${id}" data-id="${id}" />`;
            }
        },
        // Ativo
        {
            data: 'ativo',
            name: 'Ativo',
            title: 'Ativo',
            orderable: true,
            width: '80px',
            className: 'text-center',
            render: function (data, type, row) {
                if (type === 'display') {
                    const checked = data ? 'checked' : '';
                    const id = getCleanId(row, 'id');
                    return `
                        <div class="form-check form-switch">
                            <input class="form-check-input toggle-ativo" 
                                   type="checkbox" 
                                   ${checked}
                                   data-id="${id}"
                                   data-current="${data}"
                                   title="Clique para ${data ? 'desativar' : 'ativar'}">
                        </div>`;
                }
                return data;
            }
        },
        // Bairro
        {
            data: 'bairro',
            name: 'Bairro',
            title: 'Bairro',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // CEP
        {
            data: 'cep',
            name: 'Cep',
            title: 'CEP',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Cidade
        {
            data: 'cidade',
            name: 'Cidade',
            title: 'Cidade',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Cnpj
        {
            data: 'cnpj',
            name: 'Cnpj',
            title: 'Cnpj',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Complemento
        {
            data: 'complemento',
            name: 'Complemento',
            title: 'Complemento',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Contato
        {
            data: 'contato',
            name: 'Contato',
            title: 'Contato',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // E-mail Contato
        {
            data: 'contatoEmail',
            name: 'ContatoEmail',
            title: 'E-mail Contato',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Telefone Contato
        {
            data: 'contatoTelefone',
            name: 'ContatoTelefone',
            title: 'Telefone Contato',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // CPF
        {
            data: 'cpf',
            name: 'Cpf',
            title: 'CPF',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },
        // Ações
        {
            data: null,
            name: 'Actions',
            title: 'Ações',
            orderable: false,
            searchable: false,
            width: '100px',
            className: 'text-center no-export',
            render: function (data, type, row) {
                const id = getCleanId(row, 'id');
                let actions = '';
                
                if (window.crudPermissions.canEdit) {
                    actions += `<button class="btn btn-sm btn-primary btn-edit" data-id="${id}" title="Editar">
                                    <i class="fas fa-edit"></i>
                                </button> `;
                }
                
                if (window.crudPermissions.canDelete) {
                    actions += `<button class="btn btn-sm btn-danger btn-delete" data-id="${id}" title="Excluir">
                                    <i class="fas fa-trash"></i>
                                </button>`;
                }
                
                return actions || '<span class="text-muted">Sem ações</span>';
            }
        }

    ];

    // =========================================================================
    // ✅ v4.2: INSTANCIA O CRUD (CORRIGIDO: TODOS OS PARÂMETROS)
    // =========================================================================

    const crud = new CapFornecedoresCrud({
        controllerName: 'CapFornecedores',
        entityName: 'CapFornecedores',
        entityNamePlural: 'CapFornecedoress',
        idField: 'id',
        tableSelector: '#tableCrud',
        columns: columns,  // ✅ CORRIGIDO: era "dataTableColumns"
        permissions: window.crudPermissions,
        exportConfig: {
            enabled: true,
            excel: true,
            pdf: true,
            csv: true,
            print: true,
            filename: 'CapFornecedores'
        }
    });

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX "SELECIONAR TODOS"
    // =========================================================================

    $('#tableCrud').on('click', '#selectAll', function () {
        const isChecked = $(this).prop('checked');
        $('.row-select').prop('checked', isChecked);
        crud.updateSelectedCount();
        console.log(`${isChecked ? '✅' : '❌'} Selecionou todos os registros`);
    });

    // =========================================================================
    // v4.1: HANDLER - CHECKBOX INDIVIDUAL
    // =========================================================================

    $(document).on('change', '.row-select', function () {
        const totalCheckboxes = $('.row-select').length;
        const checkedCheckboxes = $('.row-select:checked').length;
        
        // Atualiza estado do "Selecionar Todos"
        $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
        
        crud.updateSelectedCount();
    });


    // =========================================================================
    // v4.1: HANDLER - TOGGLE SWITCH PARA CAMPO ATIVO (COM RATE LIMIT)
    // =========================================================================

    let toggleDebounceTimer = null;

    $(document).on('change', '.toggle-ativo', function () {
        const $toggle = $(this);
        const id = $toggle.data('id');
        const currentValue = $toggle.data('current');
        const newValue = $toggle.prop('checked');

        console.log(`🔄 [CapFornecedores] Toggle Ativo - ID: ${id}, Novo valor: ${newValue}`);

        // Previne múltiplos cliques (Rate Limit - Debounce 500ms)
        clearTimeout(toggleDebounceTimer);

        // Desabilita temporariamente
        $toggle.prop('disabled', true);

        toggleDebounceTimer = setTimeout(function () {
            $.ajax({
                url: `/CapFornecedores/ToggleAtivo`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                data: JSON.stringify({
                    Id: id,
                    Ativo: newValue
                }),
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        console.log(`✅ [CapFornecedores] Toggle Ativo atualizado - ID: ${id}`);
                        $toggle.data('current', newValue);
                        
                        // Usa SweetAlert se disponível, senão console
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                icon: 'success',
                                title: 'Sucesso!',
                                text: response.message || 'Status atualizado!',
                                timer: 2000,
                                showConfirmButton: false
                            });
                        }
                    } else {
                        // Reverte toggle em caso de erro
                        $toggle.prop('checked', currentValue);
                        console.error(`❌ [CapFornecedores] Erro ao atualizar Toggle Ativo:`, response);
                        
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                icon: 'error',
                                title: 'Erro!',
                                text: response.message || 'Erro ao atualizar status'
                            });
                        }
                    }
                },
                error: function (xhr) {
                    // Reverte toggle em caso de erro
                    $toggle.prop('checked', currentValue);
                    console.error(`❌ [CapFornecedores] Erro AJAX Toggle Ativo:`, xhr);
                    
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            icon: 'error',
                            title: 'Erro!',
                            text: 'Erro ao comunicar com servidor'
                        });
                    }
                },
                complete: function () {
                    // Reabilita toggle
                    $toggle.prop('disabled', false);
                }
            });
        }, 500); // Rate Limit de 500ms
    });


    console.log('✅ [CapFornecedores] JavaScript inicializado com sucesso!');
});
