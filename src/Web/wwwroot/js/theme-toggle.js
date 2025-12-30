/**
 * ============================================================================
 * RHSENSOERP - THEME TOGGLE (DARK MODE)
 * ============================================================================
 * Arquivo: wwwroot/js/theme-toggle.js
 * Versão: 1.0
 * Descrição: Controla o tema (light/dark) com persistência no localStorage
 * ============================================================================
 */

(function () {
    'use strict';

    const THEME_KEY = 'rhsensoerp-theme';
    const THEME_LIGHT = 'light';
    const THEME_DARK = 'dark';

    /**
     * Classe para gerenciar o tema
     */
    class ThemeManager {
        constructor() {
            this.currentTheme = this.getStoredTheme() || this.getPreferredTheme();
            this.init();
        }

        /**
         * Inicializa o gerenciador de temas
         */
        init() {
            // Aplica o tema salvo ou preferido
            this.applyTheme(this.currentTheme);

            // Listener para mudanças de preferência do sistema
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
                if (!this.getStoredTheme()) {
                    this.applyTheme(e.matches ? THEME_DARK : THEME_LIGHT);
                }
            });

            console.log('✅ [ThemeManager] Inicializado | Tema:', this.currentTheme);
        }

        /**
         * Obtém o tema salvo no localStorage
         */
        getStoredTheme() {
            return localStorage.getItem(THEME_KEY);
        }

        /**
         * Salva o tema no localStorage
         */
        setStoredTheme(theme) {
            localStorage.setItem(THEME_KEY, theme);
        }

        /**
         * Obtém a preferência de tema do sistema
         */
        getPreferredTheme() {
            return window.matchMedia('(prefers-color-scheme: dark)').matches
                ? THEME_DARK
                : THEME_LIGHT;
        }

        /**
         * Aplica o tema
         */
        applyTheme(theme) {
            document.documentElement.setAttribute('data-theme', theme);
            this.currentTheme = theme;
            this.updateToggleButton();

            // Dispara evento customizado
            window.dispatchEvent(new CustomEvent('themeChanged', {
                detail: { theme: theme }
            }));

            console.log('🎨 [ThemeManager] Tema aplicado:', theme);
        }

        /**
         * Alterna entre light e dark
         */
        toggleTheme() {
            const newTheme = this.currentTheme === THEME_LIGHT ? THEME_DARK : THEME_LIGHT;
            this.setStoredTheme(newTheme);
            this.applyTheme(newTheme);

            // Animação suave
            document.body.style.transition = 'background-color 0.3s ease, color 0.3s ease';
            setTimeout(() => {
                document.body.style.transition = '';
            }, 300);
        }

        /**
         * Atualiza o botão de toggle
         */
        updateToggleButton() {
            const toggleBtn = document.getElementById('theme-toggle');
            if (!toggleBtn) return;

            const icon = toggleBtn.querySelector('i');
            const text = toggleBtn.querySelector('.theme-text');

            if (this.currentTheme === THEME_DARK) {
                icon.className = 'fas fa-sun';
                if (text) text.textContent = 'Light Mode';
                toggleBtn.setAttribute('title', 'Mudar para modo claro');
            } else {
                icon.className = 'fas fa-moon';
                if (text) text.textContent = 'Dark Mode';
                toggleBtn.setAttribute('title', 'Mudar para modo escuro');
            }
        }

        /**
         * Retorna o tema atual
         */
        getCurrentTheme() {
            return this.currentTheme;
        }
    }

    // Instancia o gerenciador globalmente
    window.themeManager = new ThemeManager();

    // Aguarda o DOM carregar para vincular eventos
    document.addEventListener('DOMContentLoaded', () => {
        const toggleBtn = document.getElementById('theme-toggle');

        if (toggleBtn) {
            toggleBtn.addEventListener('click', (e) => {
                e.preventDefault();
                window.themeManager.toggleTheme();
            });
        }

        // Atualiza o botão inicialmente
        window.themeManager.updateToggleButton();
    });

})();