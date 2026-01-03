/**
 * ============================================================================
 * CAPCOLABORADORESFORNECEDOR - v6.1 CORRIGIDO
 * ============================================================================
 */

class CapColaboradoresFornecedorCrud extends CrudBase {
    constructor(config) {
        super(config);
        this.pkTextoField = null;
        this.isPkTexto = false;
        this.toggleDebounceTimer = null;
        this.jwtToken = null; // ✅ Cache do token
        this.tokenPromise = null; // ✅ Promise para evitar múltiplas chamadas
    }

    /**
     * ⭐ Busca token JWT do backend (com cache e debounce)
     */
    async getAuthToken() {
        // Se já temos token em cache, retorna
        if (this.jwtToken) {
            return this.jwtToken;
        }

        // Se já está buscando, aguarda a promise existente
        if (this.tokenPromise) {
            return this.tokenPromise;
        }

        // Cria nova promise para buscar token
        this.tokenPromise = $.ajax({
            url: '/Account/GetToken',
            type: 'GET',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        }).then(response => {
            if (response && response.token) {
                this.jwtToken = response.token;
                console.log('✅ [AUTH] Token JWT obtido com sucesso');
                return this.jwtToken;
            }
            console.error('❌ [AUTH] Resposta inválida do endpoint GetToken:', response);
            return null;
        }).catch(error => {
            console.error('❌ [AUTH] Erro ao buscar token JWT:', error);
            return null;
        }).finally(() => {
            // Limpa a promise após conclusão
            this.tokenPromise = null;
        });

        return this.tokenPromise;
    }

    enablePrimaryKeyFields(enable) {
        if (!this.isPkTexto) return;
        const $pkField = $('#' + this.pkTextoField);
        if ($pkField.length === 0) return;

        if (enable) {
            $pkField.prop('readonly', false).prop('disabled', false).removeClass('bg-light');
        } else {
            $pkField.prop('readonly', true).addClass('bg-light');
        }
    }

    openCreateModal() {
        super.openCreateModal();
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(true);
        }
        this.loadSelect2Labels();
    }

    async openEditModal(id) {
        await super.openEditModal(id);
        if (this.isPkTexto) {
            this.enablePrimaryKeyFields(false);
        }
        this.loadSelect2Labels();
    }

    /**
     * ⭐ Carrega labels dos campos Select2 (VERSÃO CORRIGIDA)
     */
    async loadSelect2Labels() {
        const self = this;
        const authToken = await this.getAuthToken();

        if (!authToken) {
            console.warn('⚠️ [Select2] Token não disponível - labels não serão carregados');
            return;
        }

        $('.select2-ajax').each(function () {
            const $select = $(this);
            const val = $select.val();

            const lookupEndpoint = $select.data('select2-url');
            const valueField = $select.data('value-field') || 'id';
            const textField = $select.data('text-field') || 'nome';

            if (!val || !lookupEndpoint || val === '0') return;

            const baseEndpoint = String(lookupEndpoint).replace(/\/lookup\/?$/i, '');
            const detailEndpoint = baseEndpoint.replace(/\/$/, '') + '/' + val;

            $.ajax({
                url: detailEndpoint,
                type: 'GET',
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (!response) return;

                    const data = (response.value ?? response.data ?? response);
                    const id = data?.[valueField];
                    const text = data?.[textField];

                    if (id && text) {
                        if ($select.find(`option[value='${id}']`).length === 0) {
                            const newOption = new Option(text, id, true, true);
                            $select.append(newOption).trigger('change');
                        } else {
                            $select.val(id).trigger('change');
                        }
                    }
                },
                error: function (xhr) {
                    console.warn('[Select2] Erro ao carregar label:', xhr.status, xhr.statusText);
                }
            });
        });
    }

    beforeSubmit(formData, isEdit) {
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

        const cleanData = {};
        cleanData.Nome = formData.nome || formData.Nome || '';
        cleanData.Cpf = formData.cpf || formData.Cpf || '';
        cleanData.IdFornecedor = parseInt(formData.idFornecedor || formData.IdFornecedor || 0, 10);
        cleanData.Ativo = formData.ativo === true || formData.Ativo === true ||
            formData.ativo === 'true' || formData.Ativo === 'true' || false;

        return cleanData;
    }

    afterSubmit(data, isEdit) {
        if (this.dataTable) {
            this.dataTable.ajax.reload(null, false);
        }
    }

    getRowId(row) {
        const id = row[this.config.idField] || row.id || row.Id || '';
        return typeof id === 'string' ? id.trim() : id;
    }
}

$(document).ready(function () {

    if (typeof window.crudPermissions === 'undefined') {
        console.error('❌ Permissões não carregadas!');
        window.crudPermissions = {
            canCreate: false,
            canEdit: false,
            canDelete: false,
            canView: true
        };
    }

    function getCleanId(row, fieldName) {
        if (!row) return '';
        let id = row[fieldName] || row[fieldName.toLowerCase()] || row[fieldName.toUpperCase()] ||
            row['id'] || row['Id'] || '';
        return String(id).trim();
    }

    const columns = [
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
        {
            data: 'nome',
            name: 'Nome',
            title: 'Nome',
            orderable: true,
            render: function (data) { return data ?? ''; }
        },
        {
            data: 'cpf',
            name: 'Cpf',
            title: 'Cpf',
            orderable: true,
            render: function (data) { return data ?? ''; }
        },
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

    $('#tableCrud').on('click', '#selectAll', function () {
        const isChecked = $(this).prop('checked');
        $('.row-select').prop('checked', isChecked);
        crud.updateSelectedCount();
    });

    $(document).on('change', '.row-select', function () {
        const totalCheckboxes = $('.row-select').length;
        const checkedCheckboxes = $('.row-select:checked').length;
        $('#selectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
        crud.updateSelectedCount();
    });

    let toggleDebounceTimer = null;

    $(document).on('change', '.toggle-ativo', function () {
        const $toggle = $(this);
        const id = $toggle.data('id');
        const currentValue = $toggle.data('current');
        const newValue = $toggle.prop('checked');

        clearTimeout(toggleDebounceTimer);
        $toggle.prop('disabled', true);

        toggleDebounceTimer = setTimeout(function () {
            $.ajax({
                url: `/CapColaboradoresFornecedor/ToggleAtivo`,
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                data: JSON.stringify({ Id: id, Ativo: newValue }),
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        $toggle.data('current', newValue);
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
                        $toggle.prop('checked', currentValue);
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
                    $toggle.prop('checked', currentValue);
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            icon: 'error',
                            title: 'Erro!',
                            text: 'Erro ao comunicar com servidor'
                        });
                    }
                },
                complete: function () {
                    $toggle.prop('disabled', false);
                }
            });
        }, 500);
    });

    // =========================================================================
    // ⭐ SELECT2 (AJAX) - VERSÃO FINAL CORRIGIDA
    // =========================================================================
    initSelect2();

    async function initSelect2() {
        console.log('[Select2] Inicializando - Buscando token JWT...');

        // ✅ Busca token ANTES de inicializar Select2
        const authToken = await crud.getAuthToken();

        if (!authToken) {
            console.error('❌ [Select2] Token não disponível - Select2 NÃO será inicializado');
            alert('Erro: Token de autenticação não disponível. Por favor, recarregue a página.');
            return;
        }

        console.log('✅ [Select2] Token obtido - Inicializando campos...');

        $('.select2-ajax').each(function () {
            const $select = $(this);

            const lookupEndpoint = $select.data('select2-url');
            const valueField = $select.data('value-field') || 'id';
            const textField = $select.data('text-field') || 'nome';
            const placeholder = $select.attr('placeholder') || 'Selecione...';

            if (!lookupEndpoint) {
                console.error('[Select2] Endpoint não configurado para:', $select.attr('id'));
                return;
            }

            if ($select.hasClass('select2-hidden-accessible')) {
                $select.select2('destroy');
            }

            $select.select2({
                theme: 'bootstrap-5',
                placeholder: placeholder,
                allowClear: true,
                dropdownParent: $('#modalCrud'),
                width: '100%',
                ajax: {
                    url: lookupEndpoint,
                    dataType: 'json',
                    delay: 250,
                    headers: {
                        'Authorization': `Bearer ${authToken}`, // ✅ Token já disponível
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    data: function (params) {
                        return {
                            search: params.term,
                            page: params.page || 1,
                            pageSize: 20
                        };
                    },
                    processResults: function (response, params) {
                        params.page = params.page || 1;
                        const pageSize = 20;

                        const value = response?.value ?? response?.data ?? response;
                        const items =
                            value?.items ??
                            value?.results ??
                            response?.items ??
                            response?.results ??
                            (Array.isArray(value) ? value : []);

                        const totalCount =
                            value?.totalCount ??
                            value?.TotalCount ??
                            response?.totalCount ??
                            response?.TotalCount ??
                            0;

                        if (!Array.isArray(items)) {
                            console.error('[Select2] Resposta inválida:', response);
                            return { results: [] };
                        }

                        const results = items.map(function (item) {
                            return {
                                id: (item?.[valueField] ?? '').toString(),
                                text: (item?.[textField] ?? 'Sem descrição').toString(),
                                originalItem: item
                            };
                        });

                        return {
                            results: results,
                            pagination: {
                                more: totalCount > 0 ?
                                    (params.page * pageSize) < totalCount :
                                    (items.length === pageSize)
                            }
                        };
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        console.error('[Select2] Erro na requisição:', {
                            status: jqXHR.status,
                            statusText: jqXHR.statusText,
                            textStatus: textStatus,
                            error: errorThrown,
                            endpoint: lookupEndpoint
                        });

                        // ✅ NÃO redireciona mais - só loga o erro
                        if (jqXHR.status === 401) {
                            console.error('❌ [AUTH] Erro 401 - Token inválido ou expirado');
                            console.error('💡 Sugestão: Recarregue a página para obter novo token');
                        }
                    },
                    cache: true
                },
                language: {
                    noResults: function () { return "Nenhum resultado encontrado"; },
                    searching: function () { return "Buscando..."; },
                    inputTooShort: function () { return "Digite para buscar..."; },
                    errorLoading: function () { return "Erro ao carregar resultados"; }
                }
            });

            console.log(`✅ [Select2] Campo '${$select.attr('id')}' inicializado com sucesso`);
        });
    }

    // ✅ Re-inicializa quando modal abre
    $(document).on('shown.bs.modal', '#modalCrud', function () {
        console.log('[Select2] Modal aberto - reinicializando Select2');
        initSelect2();
        crud.loadSelect2Labels();
    });

    console.log('✅ [CapColaboradoresFornecedor] JavaScript inicializado com sucesso!');
});
