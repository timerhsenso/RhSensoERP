/**
 * RhSensoWeb - Application JavaScript
 * Funções globais e utilitários para a aplicação
 */

// Namespace da aplicação
window.RhSensoWeb = window.RhSensoWeb || {};

(function($, app) {
    'use strict';

    // ========================================
    // Configurações Globais
    // ========================================
    
    app.config = {
        // URLs da API
        apiBaseUrl: '/api/',
        
        // Configurações do DataTables
        dataTable: {
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
            },
            pageLength: 25,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
            responsive: true,
            processing: true,
            serverSide: false,
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel"></i> Excel',
                    className: 'btn btn-success btn-sm'
                },
                {
                    extend: 'pdf',
                    text: '<i class="fas fa-file-pdf"></i> PDF',
                    className: 'btn btn-danger btn-sm'
                },
                {
                    extend: 'print',
                    text: '<i class="fas fa-print"></i> Imprimir',
                    className: 'btn btn-info btn-sm'
                }
            ]
        },
        
        // Configurações do Toastr
        toastr: {
            closeButton: true,
            debug: false,
            newestOnTop: true,
            progressBar: true,
            positionClass: "toast-top-right",
            preventDuplicates: false,
            onclick: null,
            showDuration: "300",
            hideDuration: "1000",
            timeOut: "5000",
            extendedTimeOut: "1000",
            showEasing: "swing",
            hideEasing: "linear",
            showMethod: "fadeIn",
            hideMethod: "fadeOut"
        }
    };

    // ========================================
    // Utilitários Gerais
    // ========================================
    
    app.utils = {
        
        /**
         * Formatar CPF
         */
        formatCpf: function(cpf) {
            if (!cpf) return '';
            cpf = cpf.replace(/\D/g, '');
            return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
        },
        
        /**
         * Formatar CNPJ
         */
        formatCnpj: function(cnpj) {
            if (!cnpj) return '';
            cnpj = cnpj.replace(/\D/g, '');
            return cnpj.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
        },
        
        /**
         * Formatar telefone
         */
        formatPhone: function(phone) {
            if (!phone) return '';
            phone = phone.replace(/\D/g, '');
            if (phone.length === 11) {
                return phone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
            } else if (phone.length === 10) {
                return phone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
            }
            return phone;
        },
        
        /**
         * Formatar data brasileira
         */
        formatDate: function(date) {
            if (!date) return '';
            const d = new Date(date);
            return d.toLocaleDateString('pt-BR');
        },
        
        /**
         * Formatar data e hora brasileira
         */
        formatDateTime: function(date) {
            if (!date) return '';
            const d = new Date(date);
            return d.toLocaleString('pt-BR');
        },
        
        /**
         * Formatar moeda brasileira
         */
        formatCurrency: function(value) {
            if (value === null || value === undefined) return 'R$ 0,00';
            return new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL'
            }).format(value);
        },
        
        /**
         * Debounce function
         */
        debounce: function(func, wait, immediate) {
            let timeout;
            return function executedFunction() {
                const context = this;
                const args = arguments;
                const later = function() {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                const callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        },
        
        /**
         * Gerar ID único
         */
        generateId: function() {
            return 'id_' + Math.random().toString(36).substr(2, 9);
        }
    };

    // ========================================
    // Gerenciamento de Loading
    // ========================================
    
    app.loading = {
        
        /**
         * Mostrar loading em elemento
         */
        show: function(element) {
            const $element = $(element);
            $element.addClass('loading');
            
            if (!$element.find('.loading-overlay').length) {
                $element.append('<div class="loading-overlay"><i class="fas fa-spinner fa-spin"></i></div>');
            }
        },
        
        /**
         * Esconder loading de elemento
         */
        hide: function(element) {
            const $element = $(element);
            $element.removeClass('loading');
            $element.find('.loading-overlay').remove();
        },
        
        /**
         * Mostrar loading global
         */
        showGlobal: function() {
            if (!$('#global-loading').length) {
                $('body').append(`
                    <div id="global-loading" class="global-loading">
                        <div class="loading-content">
                            <i class="fas fa-spinner fa-spin fa-3x text-primary"></i>
                            <p class="mt-3">Carregando...</p>
                        </div>
                    </div>
                `);
            }
            $('#global-loading').fadeIn();
        },
        
        /**
         * Esconder loading global
         */
        hideGlobal: function() {
            $('#global-loading').fadeOut();
        }
    };

    // ========================================
    // Gerenciamento de Modais
    // ========================================
    
    app.modal = {
        
        /**
         * Confirmar ação
         */
        confirm: function(message, callback, title = 'Confirmação') {
            const modalId = app.utils.generateId();
            const modalHtml = `
                <div class="modal fade" id="${modalId}" tabindex="-1" role="dialog">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">${title}</h5>
                                <button type="button" class="close" data-dismiss="modal">
                                    <span>&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <p>${message}</p>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                                <button type="button" class="btn btn-primary confirm-btn">Confirmar</button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            $('body').append(modalHtml);
            const $modal = $('#' + modalId);
            
            $modal.find('.confirm-btn').on('click', function() {
                callback();
                $modal.modal('hide');
            });
            
            $modal.on('hidden.bs.modal', function() {
                $modal.remove();
            });
            
            $modal.modal('show');
        },
        
        /**
         * Mostrar alerta
         */
        alert: function(message, type = 'info', title = 'Informação') {
            const modalId = app.utils.generateId();
            const iconClass = {
                'success': 'fas fa-check-circle text-success',
                'error': 'fas fa-exclamation-triangle text-danger',
                'warning': 'fas fa-exclamation-circle text-warning',
                'info': 'fas fa-info-circle text-info'
            }[type] || 'fas fa-info-circle text-info';
            
            const modalHtml = `
                <div class="modal fade" id="${modalId}" tabindex="-1" role="dialog">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">
                                    <i class="${iconClass} mr-2"></i>${title}
                                </h5>
                                <button type="button" class="close" data-dismiss="modal">
                                    <span>&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <p>${message}</p>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            $('body').append(modalHtml);
            const $modal = $('#' + modalId);
            
            $modal.on('hidden.bs.modal', function() {
                $modal.remove();
            });
            
            $modal.modal('show');
        }
    };

    // ========================================
    // Gerenciamento de DataTables
    // ========================================
    
    app.dataTable = {
        
        /**
         * Inicializar DataTable padrão
         */
        init: function(selector, options = {}) {
            const defaultOptions = $.extend(true, {}, app.config.dataTable, options);
            return $(selector).DataTable(defaultOptions);
        },
        
        /**
         * Inicializar DataTable com AJAX
         */
        initAjax: function(selector, ajaxUrl, columns, options = {}) {
            const defaultOptions = $.extend(true, {}, app.config.dataTable, {
                serverSide: true,
                ajax: {
                    url: ajaxUrl,
                    type: 'GET',
                    error: function(xhr, error, thrown) {
                        console.error('Erro no DataTable:', error);
                        toastr.error('Erro ao carregar dados da tabela');
                    }
                },
                columns: columns
            }, options);
            
            return $(selector).DataTable(defaultOptions);
        }
    };

    // ========================================
    // Gerenciamento de Formulários
    // ========================================
    
    app.form = {
        
        /**
         * Serializar formulário para objeto
         */
        serialize: function(form) {
            const formData = new FormData(form);
            const object = {};
            
            formData.forEach((value, key) => {
                if (object[key]) {
                    if (!Array.isArray(object[key])) {
                        object[key] = [object[key]];
                    }
                    object[key].push(value);
                } else {
                    object[key] = value;
                }
            });
            
            return object;
        },
        
        /**
         * Validar formulário
         */
        validate: function(form) {
            const $form = $(form);
            let isValid = true;
            
            $form.find('[required]').each(function() {
                const $field = $(this);
                if (!$field.val().trim()) {
                    $field.addClass('is-invalid');
                    isValid = false;
                } else {
                    $field.removeClass('is-invalid').addClass('is-valid');
                }
            });
            
            return isValid;
        },
        
        /**
         * Limpar validação do formulário
         */
        clearValidation: function(form) {
            $(form).find('.is-invalid, .is-valid').removeClass('is-invalid is-valid');
            $(form).find('.invalid-feedback').remove();
        }
    };

    // ========================================
    // Inicialização da Aplicação
    // ========================================
    
    app.init = function() {
        
        // Configurar Toastr
        toastr.options = app.config.toastr;
        
        // Configurar CSRF Token para AJAX
        $.ajaxSetup({
            beforeSend: function(xhr, settings) {
                if (!(/^http:.*/.test(settings.url) || /^https:.*/.test(settings.url))) {
                    const token = $('input[name="__RequestVerificationToken"]').val();
                    if (token) {
                        xhr.setRequestHeader("X-CSRF-TOKEN", token);
                    }
                }
            }
        });
        
        // Interceptar erros AJAX globais
        $(document).ajaxError(function(event, xhr, settings, thrownError) {
            if (xhr.status === 401) {
                toastr.error('Sua sessão expirou. Redirecionando para login...');
                setTimeout(() => {
                    window.location.href = '/Auth/Login';
                }, 2000);
            } else if (xhr.status === 403) {
                toastr.error('Você não tem permissão para realizar esta ação');
            } else if (xhr.status >= 500) {
                toastr.error('Erro interno do servidor. Tente novamente.');
            }
        });
        
        // Inicializar tooltips
        $('[data-toggle="tooltip"]').tooltip();
        
        // Inicializar popovers
        $('[data-toggle="popover"]').popover();
        
        // Auto-hide alerts após 5 segundos
        setTimeout(function() {
            $('.alert:not(.alert-permanent)').fadeOut();
        }, 5000);
        
        // Confirmar exclusões
        $(document).on('click', '[data-confirm]', function(e) {
            e.preventDefault();
            const message = $(this).data('confirm') || 'Tem certeza que deseja continuar?';
            const href = $(this).attr('href') || $(this).data('href');
            
            app.modal.confirm(message, function() {
                if (href) {
                    window.location.href = href;
                }
            });
        });
        
        // Máscaras de input
        $('[data-mask="cpf"]').on('input', function() {
            this.value = app.utils.formatCpf(this.value);
        });
        
        $('[data-mask="cnpj"]').on('input', function() {
            this.value = app.utils.formatCnpj(this.value);
        });
        
        $('[data-mask="phone"]').on('input', function() {
            this.value = app.utils.formatPhone(this.value);
        });
        
        console.log('RhSensoWeb inicializado com sucesso!');
    };

    // ========================================
    // Inicialização quando DOM estiver pronto
    // ========================================
    
    $(document).ready(function() {
        app.init();
    });

})(jQuery, window.RhSensoWeb);
