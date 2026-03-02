// ========================================
// LOGIN - RHSENSOERP
// ========================================

(function () {
    'use strict';

    // Elementos do DOM
    const loginForm = document.getElementById('loginForm');
    const btnLogin = document.getElementById('btnLogin');
    const btnLoginText = document.getElementById('btnLoginText');
    const btnLoginSpinner = document.getElementById('btnLoginSpinner');
    const togglePassword = document.getElementById('togglePassword');
    const togglePasswordIcon = document.getElementById('togglePasswordIcon');
    const senhaInput = document.getElementById('senhaInput');

    // ========================================
    // TOGGLE SENHA (MOSTRAR/OCULTAR)
    // ========================================
    if (togglePassword && senhaInput && togglePasswordIcon) {
        togglePassword.addEventListener('click', function () {
            const type = senhaInput.getAttribute('type') === 'password' ? 'text' : 'password';
            senhaInput.setAttribute('type', type);

            // Alterna o ícone
            if (type === 'password') {
                togglePasswordIcon.classList.remove('fa-eye-slash');
                togglePasswordIcon.classList.add('fa-eye');
            } else {
                togglePasswordIcon.classList.remove('fa-eye');
                togglePasswordIcon.classList.add('fa-eye-slash');
            }
        });
    }

    // ========================================
    // SUBMIT DO FORMULÁRIO
    // ========================================
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            // Validação do jQuery Validation já está ativa
            if (!$(loginForm).valid()) {
                return;
            }

            // Desabilita o botão e mostra loading
            btnLogin.disabled = true;
            btnLoginText.textContent = 'Autenticando...';
            btnLoginSpinner.classList.remove('d-none');
        });
    }

    // ========================================
    // FOCUS NO PRIMEIRO CAMPO AO CARREGAR
    // ========================================
    window.addEventListener('DOMContentLoaded', function () {
        const cdUsuarioInput = document.getElementById('CdUsuario');
        if (cdUsuarioInput) {
            cdUsuarioInput.focus();
        }
    });

    // ========================================
    // MENSAGENS DE VALIDAÇÃO CUSTOMIZADAS (PT-BR)
    // ========================================
    if (typeof $.validator !== 'undefined') {
        $.validator.setDefaults({
            highlight: function (element) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function (element) {
                $(element).removeClass('is-invalid').addClass('is-valid');
            },
            errorElement: 'span',
            errorClass: 'text-danger small',
            errorPlacement: function (error, element) {
                error.insertAfter(element.closest('.input-group').length ? element.closest('.input-group') : element);
            }
        });

        // Mensagens em português
        $.validator.messages.required = 'Este campo é obrigatório.';
        $.validator.messages.email = 'Por favor, insira um e-mail válido.';
    }
})();