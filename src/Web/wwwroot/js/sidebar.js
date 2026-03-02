/**
 * ============================================================================
 * RHSENSOERP - SIDEBAR MANAGER
 * ============================================================================
 * Arquivo: wwwroot/js/sidebar.js
 * Versão: 1.0
 * Descrição: Controla o comportamento da sidebar vertical:
 *            - Colapsar / Expandir no desktop
 *            - Abrir / Fechar em mobile
 *            - Persistência do estado no localStorage
 *            - Submenus accordion
 *            - Tooltips no modo colapsado
 * ============================================================================
 */

(function () {
    'use strict';

    // =========================================================================
    // CONSTANTES
    // =========================================================================
    const SIDEBAR_STATE_KEY = 'rhsensoerp-sidebar-collapsed';
    const SIDEBAR_ID        = 'appSidebar';
    const OVERLAY_ID        = 'sidebarOverlay';
    const TOGGLE_DESKTOP_ID = 'sidebarCollapseBtn';
    const TOGGLE_MOBILE_ID  = 'sidebarMobileToggle';

    // =========================================================================
    // CLASSE PRINCIPAL
    // =========================================================================
    class SidebarManager {

        constructor() {
            this.sidebar        = document.getElementById(SIDEBAR_ID);
            this.overlay        = document.getElementById(OVERLAY_ID);
            this.desktopToggle  = document.getElementById(TOGGLE_DESKTOP_ID);
            this.mobileToggle   = document.getElementById(TOGGLE_MOBILE_ID);
            this.isCollapsed    = false;
            this.isMobileOpen   = false;

            if (!this.sidebar) {
                console.warn('[SidebarManager] Sidebar não encontrada (#' + SIDEBAR_ID + ')');
                return;
            }

            this._init();
        }

        // =====================================================================
        // INICIALIZAÇÃO
        // =====================================================================
        _init() {
            // Restaura estado salvo (apenas desktop)
            if (this._isDesktop()) {
                this.isCollapsed = localStorage.getItem(SIDEBAR_STATE_KEY) === 'true';
                if (this.isCollapsed) {
                    this.sidebar.classList.add('collapsed');
                }
            }

            // Vincula eventos
            this._bindEvents();

            // Inicializa submenus (mantém aberto o módulo ativo)
            this._initSubmenus();

            console.log('[SidebarManager] Inicializado | Colapsado:', this.isCollapsed);
        }

        // =====================================================================
        // BIND DE EVENTOS
        // =====================================================================
        _bindEvents() {
            // Toggle desktop (botão de seta na sidebar)
            if (this.desktopToggle) {
                this.desktopToggle.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.toggleCollapse();
                });
            }

            // Toggle mobile (botão hambúrguer na topbar)
            if (this.mobileToggle) {
                this.mobileToggle.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.toggleMobile();
                });
            }

            // Fechar ao clicar no overlay (mobile)
            if (this.overlay) {
                this.overlay.addEventListener('click', () => {
                    this.closeMobile();
                });
            }

            // Fechar com ESC (mobile)
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Escape' && this.isMobileOpen) {
                    this.closeMobile();
                }
            });

            // Ajuste ao redimensionar janela
            window.addEventListener('resize', this._debounce(() => {
                this._handleResize();
            }, 150));
        }

        // =====================================================================
        // COLAPSAR / EXPANDIR (DESKTOP)
        // =====================================================================
        toggleCollapse() {
            this.isCollapsed = !this.isCollapsed;
            this.sidebar.classList.toggle('collapsed', this.isCollapsed);

            // Persiste estado
            localStorage.setItem(SIDEBAR_STATE_KEY, String(this.isCollapsed));

            // Dispara evento customizado para que outros componentes possam reagir
            window.dispatchEvent(new CustomEvent('sidebarToggled', {
                detail: { collapsed: this.isCollapsed }
            }));

            console.log('[SidebarManager] Sidebar', this.isCollapsed ? 'colapsada' : 'expandida');
        }

        collapse() {
            if (!this.isCollapsed) this.toggleCollapse();
        }

        expand() {
            if (this.isCollapsed) this.toggleCollapse();
        }

        // =====================================================================
        // ABRIR / FECHAR (MOBILE)
        // =====================================================================
        toggleMobile() {
            if (this.isMobileOpen) {
                this.closeMobile();
            } else {
                this.openMobile();
            }
        }

        openMobile() {
            this.isMobileOpen = true;
            this.sidebar.classList.add('mobile-open');
            if (this.overlay) this.overlay.classList.add('active');
            document.body.style.overflow = 'hidden';
        }

        closeMobile() {
            this.isMobileOpen = false;
            this.sidebar.classList.remove('mobile-open');
            if (this.overlay) this.overlay.classList.remove('active');
            document.body.style.overflow = '';
        }

        // =====================================================================
        // SUBMENUS (ACCORDION)
        // =====================================================================
        _initSubmenus() {
            const submenuToggles = this.sidebar.querySelectorAll('[data-sidebar-submenu]');

            submenuToggles.forEach(toggle => {
                const targetId = toggle.getAttribute('data-sidebar-submenu');
                const submenu  = document.getElementById(targetId);

                if (!submenu) return;

                // Verifica se algum item do submenu está ativo
                const hasActiveChild = submenu.querySelector('.active') !== null;

                if (hasActiveChild) {
                    // Mantém aberto se tiver item ativo
                    toggle.setAttribute('aria-expanded', 'true');
                    submenu.classList.add('show');
                } else {
                    toggle.setAttribute('aria-expanded', 'false');
                }

                toggle.addEventListener('click', (e) => {
                    e.preventDefault();

                    // Se sidebar colapsada no desktop, não abre submenu
                    if (this.isCollapsed && this._isDesktop()) return;

                    const isExpanded = toggle.getAttribute('aria-expanded') === 'true';

                    // Fecha outros submenus abertos (accordion)
                    submenuToggles.forEach(otherToggle => {
                        if (otherToggle !== toggle) {
                            const otherId = otherToggle.getAttribute('data-sidebar-submenu');
                            const otherSubmenu = document.getElementById(otherId);
                            if (otherSubmenu && otherSubmenu.classList.contains('show')) {
                                otherToggle.setAttribute('aria-expanded', 'false');
                                // Usa Bootstrap collapse se disponível
                                if (typeof bootstrap !== 'undefined' && bootstrap.Collapse) {
                                    const bsCollapse = bootstrap.Collapse.getInstance(otherSubmenu);
                                    if (bsCollapse) bsCollapse.hide();
                                } else {
                                    otherSubmenu.classList.remove('show');
                                }
                            }
                        }
                    });

                    // Abre/fecha este submenu
                    toggle.setAttribute('aria-expanded', String(!isExpanded));
                    if (typeof bootstrap !== 'undefined' && bootstrap.Collapse) {
                        const bsCollapse = bootstrap.Collapse.getOrCreateInstance(submenu, { toggle: false });
                        isExpanded ? bsCollapse.hide() : bsCollapse.show();
                    } else {
                        submenu.classList.toggle('show', !isExpanded);
                    }
                });
            });
        }

        // =====================================================================
        // RESIZE HANDLER
        // =====================================================================
        _handleResize() {
            if (this._isDesktop()) {
                // Fecha mobile ao voltar para desktop
                if (this.isMobileOpen) {
                    this.closeMobile();
                }
            }
        }

        // =====================================================================
        // UTILITÁRIOS
        // =====================================================================
        _isDesktop() {
            return window.innerWidth >= 992;
        }

        _debounce(fn, delay) {
            let timer;
            return function (...args) {
                clearTimeout(timer);
                timer = setTimeout(() => fn.apply(this, args), delay);
            };
        }
    }

    // =========================================================================
    // INICIALIZAÇÃO QUANDO DOM ESTIVER PRONTO
    // =========================================================================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            window.sidebarManager = new SidebarManager();
        });
    } else {
        window.sidebarManager = new SidebarManager();
    }

})();
