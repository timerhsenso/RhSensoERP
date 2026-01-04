/**
 * ============================================================================
 * CAPCOLABORADORESFORNECEDOR - JavaScript com Checkbox e Toggle Ativo
 * ============================================================================
 * Arquivo: wwwroot/js/controleacessoportaria/capcolaboradoresfornecedor/capcolaboradoresfornecedor.js
 * Módulo: ControleAcessoPortaria
 * Versão: 4.5 (COLUNAS DE NAVEGAÇÃO CORRIGIDAS)
 * Gerado por: GeradorFullStack v4.5
 * Data: 2026-01-04 21:50:00
 * 
 * Changelog v4.5:
 *   ✅ CORRIGIDO: Adicionadas colunas tipoSanguineoDescricao e ufSigla
 *   ✅ CORRIGIDO: Todas as propriedades de navegação agora aparecem na grid
 * 
 * Changelog v4.4:
 *   ✅ CORRIGIDO: Select2 agora usa data-select2-url (não data-endpoint)
 *   ✅ CORRIGIDO: Validação de resposta da API
 *   ✅ CORRIGIDO: Pré-carregamento de labels funcional
 *   ✅ NOVO: Re-inicialização do Select2 quando modal é aberto
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
 * Implementação específica do CRUD de CapColaboradoresFornecedor.
 * Estende a classe CrudBase com customizações necessárias.
 * ============================================================================
 */

class CapColaboradoresFornecedorCrud extends CrudBase {
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
            console.log('✏️ [CapColaboradoresFornecedor] Campo PK habilitado para edição (criação)');
        } else {
            // Edição: campo readonly
            $pkField.prop('readonly', true)
                .addClass('bg-light');
            console.log('🔒 [CapColaboradoresFornecedor] Campo PK desabilitado (edição)');
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

        // ⭐ v4.4: Pré-carrega Labels do Select2
        this.loadSelect2Labels();
    }

    /**
     * ⭐ v4.4 CORRIGIDO: Carrega labels dos campos Select2 Ajax (Recupera texto do ID selecionado)
     * Agora usa data-select2-url em vez de data-endpoint
     */
    loadSelect2Labels() {
        $('.select2-ajax').each(function () {
            const $select = $(this);
            const val = $select.val();
            const endpoint = $select.data('select2-url');  // ✅ v4.4 CORRIGIDO: data-select2-url
            const valueField = $select.data('value-field') || 'id';
            const textField = $select.data('text-field') || 'nome';

            if (val && endpoint && val !== '0') {
                // ⭐ v4.4: Endpoint para buscar um item por ID
                const detailEndpoint = endpoint.replace(/\/$/, '') + '/' + val;

                $.ajax({
                    url: detailEndpoint,
                    type: 'GET',
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (response) {
                        if (response) {
                            // ⭐ v4.4: Suporta Datawrapper (Result<T>) e resposta direta
                            const data = response.data || response;

                            const id = data[valueField];
                            const text = data[textField];

                            if (id && text) {
                                // ⭐ v4.4: Validação adicional
                                if ($select.find("option[value='" + id + "']").length === 0) {
                                    const newOption = new Option(text, id, true, true);
                                    $select.append(newOption).trigger('change');
                                } else {
                                    $select.val(id).trigger('change');
                                }
                            } else {
                                console.warn(`[Select2] Campos obrigatórios não encontrados:`, { valueField, textField, data });
                            }
                        }
                    },
                    error: function (xhr) {
                        console.warn(`[Select2] Falha ao carregar label de ${detailEndpoint}:`, xhr);
                    }
                });
            }
        });
    }

    /**
     * ⭐ v3.9 CORRIGIDO: Retorna objeto em PascalCase
     * Remove campos de auditoria, converte tipos e valida campos obrigatórios.
     */
    beforeSubmit(formData, isEdit) {
        console.log('📥 [CapColaboradoresFornecedor] Dados ANTES:', JSON.parse(JSON.stringify(formData)));

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
        cleanData.Nome = formData.nome || formData.Nome || '';
        cleanData.Cpf = formData.cpf || formData.Cpf || '';
        cleanData.Rg = formData.rg || formData.Rg || '';
        cleanData.Email = formData.email || formData.Email || '';
        cleanData.Telefone = formData.telefone || formData.Telefone || '';
        cleanData.Genero = formData.genero || formData.Genero || '';
        cleanData.EstadoCivil = formData.estadoCivil || formData.EstadoCivil || '';
        cleanData.Endereco = formData.endereco || formData.Endereco || '';
        cleanData.Numero = formData.numero || formData.Numero || '';
        cleanData.Complemento = formData.complemento || formData.Complemento || '';
        cleanData.Bairro = formData.bairro || formData.Bairro || '';
        cleanData.Cidade = formData.cidade || formData.Cidade || '';
        cleanData.Cep = formData.cep || formData.Cep || '';
        cleanData.Cargo = formData.cargo || formData.Cargo || '';


        // Integer required fields - PascalCase
        cleanData.IdFornecedor = parseInt(formData.idFornecedor || formData.IdFornecedor || 0, 10);
        cleanData.IdTipoSanguineo = parseInt(formData.idTipoSanguineo || formData.IdTipoSanguineo || 0, 10);
        cleanData.IdUf = parseInt(formData.idUf || formData.IdUf || 0, 10);


        // Boolean fields - PascalCase
        cleanData.Ativo = formData.ativo === true || formData.Ativo === 'true' || false;

        console.log('📤 [CapColaboradoresFornecedor] Dados DEPOIS (PascalCase):', JSON.parse(JSON.stringify(cleanData)));
        return cleanData;
    }

    /**
     * Customização após submeter.
     */
    afterSubmit(data, isEdit) {
        console.log('✅ [CapColaboradoresFornecedor] Registro salvo:', data);

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

    console.log('🔐 [CapColaboradoresFornecedor] Permissões ativas:', window.crudPermissions);

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
            console.warn('⚠️ [CapColaboradoresFornecedor] ID vazio para row:', row);
        }

        return id;
    }

    // =========================================================================
    // ✅ v4.5: CONFIGURAÇÃO DAS COLUNAS (CORRIGIDO COM TODAS AS NAVEGAÇÕES)
    // =========================================================================

    const columns = [
        // =====================================================================
        // COLUNA DE SELEÇÃO (CHECKBOX)
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

        // Nome
        {
            data: 'nome',
            name: 'Nome',
            title: 'Nome',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },

        // ✅ v4.5: Fornecedor (Navegação)
        {
            data: 'fornecedorRazaoSocial',
            name: 'fornecedorRazaoSocial',
            title: 'Fornecedor',
            orderable: false,
            searchable: false,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },

        // ✅ v4.5: Tipo Sanguíneo (Navegação)
        {
            data: 'tipoSanguineoDescricao',
            name: 'tipoSanguineoDescricao',
            title: 'Tipo Sanguíneo',
            orderable: false,
            searchable: false,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },

        // ✅ v4.5: UF (Navegação)
        {
            data: 'ufSigla',
            name: 'ufSigla',
            title: 'UF',
            orderable: false,
            searchable: false,
            width: '60px',
            className: 'text-center',
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
            }
        },

        // Telefone
        {
            data: 'telefone',
            name: 'Telefone',
            title: 'Telefone',
            orderable: true,
            render: function (data, type, row) {
                return data !== undefined && data !== null ? data : '';
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

    const crud = new CapColaboradoresFornecedorCrud({
        controllerName: 'CapColaboradoresFornecedor',
        entityName: 'CapColaboradoresFornecedor',
        entityNamePlural: 'CapColaboradoresFornecedors',
        idField: 'id',
        tableSelector: '#tableCrud',
        columns: columns,
        permissions: window.crudPermissions,
        exportConfig: {
            enabled: true,
            excel: true,
            pdf: true,
            csv: true,
            print: true,
            filename: 'CapColaboradoresFornecedor'
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

        console.log(`🔄 [CapColaboradoresFornecedor] Toggle Ativo - ID: ${id}, Novo valor: ${newValue}`);

        // Previne múltiplos cliques (Rate Limit - Debounce 500ms)
        clearTimeout(toggleDebounceTimer);

        // Desabilita temporariamente
        $toggle.prop('disabled', true);

        toggleDebounceTimer = setTimeout(function () {
            $.ajax({
                url: `/CapColaboradoresFornecedor/ToggleAtivo`,
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
                        console.log(`✅ [CapColaboradoresFornecedor] Toggle Ativo atualizado - ID: ${id}`);
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
                        console.error(`❌ [CapColaboradoresFornecedor] Erro ao atualizar Toggle Ativo:`, response);

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
                    console.error(`❌ [CapColaboradoresFornecedor] Erro AJAX Toggle Ativo:`, xhr);

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


    // =========================================================================
    // ⭐ v4.4: INICIALIZAÇÃO DO SELECT2 (AJAX) - CORRIGIDO
    // =========================================================================
    initSelect2();

    function initSelect2() {
        $('.select2-ajax').each(function () {
            const $select = $(this);
            const endpoint = $select.data('select2-url');  // ✅ v4.4 CORRIGIDO: data-select2-url
            const valueField = $select.data('value-field') || 'id';
            const textField = $select.data('text-field') || 'nome';
            const placeholder = $select.attr('placeholder') || 'Selecione...';

            if (!endpoint) {
                console.error('[Select2] Endpoint não configurado para campo:', $select.attr('id'));
                return;
            }

            $select.select2({
                theme: 'bootstrap-5',
                placeholder: placeholder,
                allowClear: true,
                dropdownParent: $('#modalCrud'), // Importante para funcionar dentro do modal bootstrap
                width: '100%',
                ajax: {
                    url: endpoint,
                    dataType: 'json',
                    delay: 250, // Debounce
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    data: function (params) {
                        return {
                            search: params.term, // Termo de busca
                            page: params.page || 1,
                            pageSize: 20
                        };
                    },
                    processResults: function (data) {
                        // ⭐ v4.4 CORRIGIDO: Mapeia o retorno da API para o formato do Select2
                        // Suporta: { items: [] }, { data: [] }, { results: [] } ou []
                        const items = data.items || data.data || data.results || data || [];

                        // ⭐ v4.4: Validação de resposta
                        if (!Array.isArray(items)) {
                            console.error('[Select2] Resposta não é um array:', data);
                            return { results: [] };
                        }

                        console.log('[Select2] Dados recebidos:', data);
                        console.log('[Select2] Itens extraídos:', items);
                        console.log('[Select2] Config:', { valueField, textField });

                        return {
                            results: items.map(function (item) {
                                const id = item[valueField];
                                const text = item[textField];

                                // ⭐ v4.4: Validação de campos obrigatórios
                                if (!id || !text) {
                                    console.warn('[Select2] Item sem campos obrigatórios:', item, { valueField, textField });
                                }

                                return {
                                    id: id || '',
                                    text: text || 'Sem descrição',
                                    originalItem: item // Guarda item original se precisar
                                };
                            })
                        };
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.error('[Select2] Erro na requisição:', textStatus, errorThrown);
                        console.error('Endpoint:', endpoint);
                    },
                    cache: true
                },
                language: {
                    noResults: function () { return "Nenhum resultado encontrado"; },
                    searching: function () { return "Buscando..."; },
                    inputTooShort: function () { return "Digite para buscar..."; }
                }
            });
        });
    }

    // =========================================================================
    // ⭐ v4.4: RE-INICIALIZAÇÃO DO SELECT2 QUANDO MODAL É ABERTO
    // =========================================================================
    $(document).on('shown.bs.modal', '#modalCrud', function () {
        console.log('[Select2] Modal aberto - reinicializando Select2');
        initSelect2();
    });

    console.log('✅ [CapColaboradoresFornecedor] JavaScript v4.5 inicializado com sucesso!');
});