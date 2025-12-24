/**
 * =============================================================================
 * TABSHEET GENERATOR v2.0 - COMPONENTES EXTRAS
 * Icon Picker + Toast Notifications + Utilitários
 * =============================================================================
 */

// ============================================================================
// TOAST NOTIFICATION SYSTEM
// ============================================================================

const Toast = (function() {
    'use strict';

    let container = null;

    function init() {
        if (container) return;

        container = document.createElement('div');
        container.className = 'ts-toast-container';
        container.innerHTML = '';
        document.body.appendChild(container);

        // Adicionar estilos se não existirem
        if (!document.getElementById('ts-toast-styles')) {
            const styles = document.createElement('style');
            styles.id = 'ts-toast-styles';
            styles.textContent = `
                .ts-toast-container {
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 99999;
                    display: flex;
                    flex-direction: column;
                    gap: 10px;
                    max-width: 380px;
                }

                .ts-toast {
                    display: flex;
                    align-items: flex-start;
                    gap: 12px;
                    padding: 16px 20px;
                    background: #1e293b;
                    border-radius: 12px;
                    box-shadow: 0 10px 40px rgba(0,0,0,0.3);
                    color: #f1f5f9;
                    animation: tsToastIn 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
                    overflow: hidden;
                    position: relative;
                }

                .ts-toast.hiding {
                    animation: tsToastOut 0.3s ease forwards;
                }

                .ts-toast::before {
                    content: '';
                    position: absolute;
                    left: 0;
                    top: 0;
                    bottom: 0;
                    width: 4px;
                }

                .ts-toast.success::before { background: linear-gradient(180deg, #10b981, #059669); }
                .ts-toast.error::before { background: linear-gradient(180deg, #ef4444, #dc2626); }
                .ts-toast.warning::before { background: linear-gradient(180deg, #f59e0b, #d97706); }
                .ts-toast.info::before { background: linear-gradient(180deg, #06b6d4, #0891b2); }

                .ts-toast-icon {
                    width: 24px;
                    height: 24px;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    flex-shrink: 0;
                    font-size: 12px;
                }

                .ts-toast.success .ts-toast-icon { background: rgba(16, 185, 129, 0.2); color: #10b981; }
                .ts-toast.error .ts-toast-icon { background: rgba(239, 68, 68, 0.2); color: #ef4444; }
                .ts-toast.warning .ts-toast-icon { background: rgba(245, 158, 11, 0.2); color: #f59e0b; }
                .ts-toast.info .ts-toast-icon { background: rgba(6, 182, 212, 0.2); color: #06b6d4; }

                .ts-toast-content {
                    flex: 1;
                    min-width: 0;
                }

                .ts-toast-title {
                    font-weight: 600;
                    font-size: 14px;
                    margin-bottom: 4px;
                }

                .ts-toast-message {
                    font-size: 13px;
                    color: #94a3b8;
                    line-height: 1.4;
                }

                .ts-toast-close {
                    background: none;
                    border: none;
                    color: #64748b;
                    cursor: pointer;
                    padding: 4px;
                    border-radius: 6px;
                    transition: all 0.2s;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }

                .ts-toast-close:hover {
                    background: rgba(255,255,255,0.1);
                    color: #f1f5f9;
                }

                .ts-toast-progress {
                    position: absolute;
                    bottom: 0;
                    left: 0;
                    height: 3px;
                    background: rgba(255,255,255,0.3);
                    animation: tsToastProgress var(--duration) linear forwards;
                }

                @keyframes tsToastIn {
                    from {
                        opacity: 0;
                        transform: translateX(100px) scale(0.8);
                    }
                    to {
                        opacity: 1;
                        transform: translateX(0) scale(1);
                    }
                }

                @keyframes tsToastOut {
                    to {
                        opacity: 0;
                        transform: translateX(100px) scale(0.8);
                    }
                }

                @keyframes tsToastProgress {
                    from { width: 100%; }
                    to { width: 0%; }
                }
            `;
            document.head.appendChild(styles);
        }
    }

    function show(title, message, type = 'info', duration = 4000) {
        init();

        const icons = {
            success: '<i class="fas fa-check"></i>',
            error: '<i class="fas fa-times"></i>',
            warning: '<i class="fas fa-exclamation"></i>',
            info: '<i class="fas fa-info"></i>'
        };

        const toast = document.createElement('div');
        toast.className = `ts-toast ${type}`;
        toast.style.setProperty('--duration', `${duration}ms`);
        toast.innerHTML = `
            <div class="ts-toast-icon">${icons[type]}</div>
            <div class="ts-toast-content">
                <div class="ts-toast-title">${title}</div>
                <div class="ts-toast-message">${message}</div>
            </div>
            <button class="ts-toast-close"><i class="fas fa-times"></i></button>
            <div class="ts-toast-progress"></div>
        `;

        container.appendChild(toast);

        // Close button
        toast.querySelector('.ts-toast-close').addEventListener('click', () => hide(toast));

        // Auto hide
        if (duration > 0) {
            setTimeout(() => hide(toast), duration);
        }

        return toast;
    }

    function hide(toast) {
        toast.classList.add('hiding');
        setTimeout(() => toast.remove(), 300);
    }

    function success(title, message, duration) {
        return show(title, message, 'success', duration);
    }

    function error(title, message, duration) {
        return show(title, message, 'error', duration);
    }

    function warning(title, message, duration) {
        return show(title, message, 'warning', duration);
    }

    function info(title, message, duration) {
        return show(title, message, 'info', duration);
    }

    return { show, success, error, warning, info };
})();

// Global function
window.showToast = function(title, message, type, duration) {
    return Toast.show(title, message, type, duration);
};


// ============================================================================
// ICON PICKER COMPONENT
// ============================================================================

const IconPicker = (function() {
    'use strict';

    // Font Awesome icons mais usados
    const icons = [
        // Interface
        'fas fa-home', 'fas fa-cog', 'fas fa-cogs', 'fas fa-sliders-h', 'fas fa-bars',
        'fas fa-ellipsis-h', 'fas fa-ellipsis-v', 'fas fa-grip-horizontal', 'fas fa-grip-vertical',
        
        // Actions
        'fas fa-plus', 'fas fa-minus', 'fas fa-times', 'fas fa-check', 'fas fa-edit',
        'fas fa-trash', 'fas fa-save', 'fas fa-download', 'fas fa-upload', 'fas fa-sync',
        'fas fa-redo', 'fas fa-undo', 'fas fa-search', 'fas fa-filter', 'fas fa-sort',
        
        // Navigation
        'fas fa-arrow-left', 'fas fa-arrow-right', 'fas fa-arrow-up', 'fas fa-arrow-down',
        'fas fa-chevron-left', 'fas fa-chevron-right', 'fas fa-chevron-up', 'fas fa-chevron-down',
        'fas fa-angle-double-left', 'fas fa-angle-double-right',
        
        // Data
        'fas fa-table', 'fas fa-list', 'fas fa-list-alt', 'fas fa-th', 'fas fa-th-large',
        'fas fa-th-list', 'fas fa-database', 'fas fa-server', 'fas fa-hdd', 'fas fa-layer-group',
        
        // Files
        'fas fa-file', 'fas fa-file-alt', 'fas fa-file-pdf', 'fas fa-file-excel', 'fas fa-file-word',
        'fas fa-file-code', 'fas fa-file-archive', 'fas fa-file-image', 'fas fa-folder', 'fas fa-folder-open',
        
        // Users
        'fas fa-user', 'fas fa-users', 'fas fa-user-plus', 'fas fa-user-minus', 'fas fa-user-edit',
        'fas fa-user-cog', 'fas fa-user-shield', 'fas fa-user-tie', 'fas fa-id-card', 'fas fa-id-badge',
        
        // Business
        'fas fa-briefcase', 'fas fa-building', 'fas fa-industry', 'fas fa-store', 'fas fa-warehouse',
        'fas fa-chart-bar', 'fas fa-chart-line', 'fas fa-chart-pie', 'fas fa-chart-area', 'fas fa-calculator',
        
        // Finance
        'fas fa-dollar-sign', 'fas fa-euro-sign', 'fas fa-coins', 'fas fa-wallet', 'fas fa-credit-card',
        'fas fa-money-bill', 'fas fa-money-check', 'fas fa-receipt', 'fas fa-file-invoice', 'fas fa-file-invoice-dollar',
        
        // Time
        'fas fa-clock', 'fas fa-calendar', 'fas fa-calendar-alt', 'fas fa-calendar-check', 'fas fa-calendar-plus',
        'fas fa-history', 'fas fa-hourglass', 'fas fa-stopwatch', 'fas fa-bell', 'fas fa-alarm-clock',
        
        // Status
        'fas fa-check-circle', 'fas fa-times-circle', 'fas fa-exclamation-circle', 'fas fa-info-circle',
        'fas fa-question-circle', 'fas fa-ban', 'fas fa-lock', 'fas fa-unlock', 'fas fa-eye', 'fas fa-eye-slash',
        
        // Communication
        'fas fa-envelope', 'fas fa-phone', 'fas fa-fax', 'fas fa-comments', 'fas fa-comment',
        'fas fa-paper-plane', 'fas fa-inbox', 'fas fa-share', 'fas fa-reply', 'fas fa-forward',
        
        // Objects
        'fas fa-book', 'fas fa-bookmark', 'fas fa-clipboard', 'fas fa-clipboard-list', 'fas fa-clipboard-check',
        'fas fa-tasks', 'fas fa-project-diagram', 'fas fa-sitemap', 'fas fa-network-wired', 'fas fa-cubes',
        
        // Medical/Health
        'fas fa-heart', 'fas fa-heartbeat', 'fas fa-stethoscope', 'fas fa-medkit', 'fas fa-hospital',
        'fas fa-ambulance', 'fas fa-pills', 'fas fa-syringe', 'fas fa-user-md', 'fas fa-notes-medical',
        
        // Education
        'fas fa-graduation-cap', 'fas fa-university', 'fas fa-school', 'fas fa-chalkboard', 'fas fa-chalkboard-teacher',
        'fas fa-book-open', 'fas fa-book-reader', 'fas fa-award', 'fas fa-certificate', 'fas fa-medal',
        
        // Security
        'fas fa-shield-alt', 'fas fa-key', 'fas fa-fingerprint', 'fas fa-user-lock', 'fas fa-lock-open',
        
        // Misc
        'fas fa-star', 'fas fa-flag', 'fas fa-tag', 'fas fa-tags', 'fas fa-thumbtack',
        'fas fa-paperclip', 'fas fa-link', 'fas fa-unlink', 'fas fa-external-link-alt', 'fas fa-qrcode'
    ];

    let modal = null;
    let currentInput = null;
    let callback = null;

    function init() {
        if (modal) return;

        modal = document.createElement('div');
        modal.className = 'ts-icon-picker-modal';
        modal.innerHTML = `
            <div class="ts-icon-picker-backdrop"></div>
            <div class="ts-icon-picker-dialog">
                <div class="ts-icon-picker-header">
                    <h5><i class="fas fa-icons me-2"></i>Selecionar Ícone</h5>
                    <button class="ts-icon-picker-close"><i class="fas fa-times"></i></button>
                </div>
                <div class="ts-icon-picker-search">
                    <i class="fas fa-search"></i>
                    <input type="text" placeholder="Buscar ícone..." />
                </div>
                <div class="ts-icon-picker-grid"></div>
                <div class="ts-icon-picker-footer">
                    <div class="ts-icon-picker-preview">
                        <i class="fas fa-question"></i>
                        <span>Nenhum selecionado</span>
                    </div>
                    <div class="ts-icon-picker-actions">
                        <button class="ts-btn ts-btn-secondary ts-icon-picker-cancel">Cancelar</button>
                        <button class="ts-btn ts-btn-primary ts-icon-picker-confirm">Confirmar</button>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Populate grid
        const grid = modal.querySelector('.ts-icon-picker-grid');
        icons.forEach(icon => {
            const item = document.createElement('div');
            item.className = 'ts-icon-picker-item';
            item.dataset.icon = icon;
            item.innerHTML = `<i class="${icon}"></i>`;
            item.title = icon.replace('fas fa-', '').replace(/-/g, ' ');
            grid.appendChild(item);
        });

        // Events
        modal.querySelector('.ts-icon-picker-backdrop').addEventListener('click', close);
        modal.querySelector('.ts-icon-picker-close').addEventListener('click', close);
        modal.querySelector('.ts-icon-picker-cancel').addEventListener('click', close);
        modal.querySelector('.ts-icon-picker-confirm').addEventListener('click', confirm);

        // Search
        modal.querySelector('.ts-icon-picker-search input').addEventListener('input', (e) => {
            const term = e.target.value.toLowerCase();
            modal.querySelectorAll('.ts-icon-picker-item').forEach(item => {
                const iconName = item.dataset.icon.toLowerCase();
                item.style.display = iconName.includes(term) ? '' : 'none';
            });
        });

        // Item selection
        grid.addEventListener('click', (e) => {
            const item = e.target.closest('.ts-icon-picker-item');
            if (!item) return;

            modal.querySelectorAll('.ts-icon-picker-item').forEach(i => i.classList.remove('selected'));
            item.classList.add('selected');

            const icon = item.dataset.icon;
            const preview = modal.querySelector('.ts-icon-picker-preview');
            preview.querySelector('i').className = icon;
            preview.querySelector('span').textContent = icon;
        });

        // Styles
        if (!document.getElementById('ts-icon-picker-styles')) {
            const styles = document.createElement('style');
            styles.id = 'ts-icon-picker-styles';
            styles.textContent = `
                .ts-icon-picker-modal {
                    display: none;
                    position: fixed;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    z-index: 99999;
                }

                .ts-icon-picker-modal.active {
                    display: block;
                }

                .ts-icon-picker-backdrop {
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: rgba(0, 0, 0, 0.5);
                    backdrop-filter: blur(4px);
                    animation: tsFadeIn 0.2s ease;
                }

                .ts-icon-picker-dialog {
                    position: absolute;
                    top: 50%;
                    left: 50%;
                    transform: translate(-50%, -50%);
                    width: 90%;
                    max-width: 600px;
                    max-height: 80vh;
                    background: var(--ts-bg-primary, #fff);
                    border-radius: 16px;
                    box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
                    display: flex;
                    flex-direction: column;
                    animation: tsScaleIn 0.3s ease;
                    overflow: hidden;
                }

                .ts-icon-picker-header {
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    padding: 1rem 1.25rem;
                    background: linear-gradient(135deg, #6366f1, #8b5cf6);
                    color: white;
                }

                .ts-icon-picker-header h5 {
                    margin: 0;
                    font-size: 1rem;
                }

                .ts-icon-picker-close {
                    background: rgba(255,255,255,0.1);
                    border: none;
                    color: white;
                    width: 32px;
                    height: 32px;
                    border-radius: 50%;
                    cursor: pointer;
                    transition: all 0.2s;
                }

                .ts-icon-picker-close:hover {
                    background: rgba(255,255,255,0.2);
                    transform: rotate(90deg);
                }

                .ts-icon-picker-search {
                    padding: 1rem;
                    position: relative;
                    border-bottom: 1px solid var(--ts-border, #e2e8f0);
                }

                .ts-icon-picker-search input {
                    width: 100%;
                    padding: 0.625rem 1rem 0.625rem 2.5rem;
                    border: 2px solid var(--ts-border, #e2e8f0);
                    border-radius: 9999px;
                    font-size: 0.875rem;
                    background: var(--ts-bg-secondary, #f8fafc);
                    transition: all 0.2s;
                }

                .ts-icon-picker-search input:focus {
                    outline: none;
                    border-color: #6366f1;
                    box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.2);
                }

                .ts-icon-picker-search i {
                    position: absolute;
                    left: 1.75rem;
                    top: 50%;
                    transform: translateY(-50%);
                    color: var(--ts-text-muted, #94a3b8);
                }

                .ts-icon-picker-grid {
                    flex: 1;
                    overflow-y: auto;
                    padding: 1rem;
                    display: grid;
                    grid-template-columns: repeat(auto-fill, minmax(48px, 1fr));
                    gap: 8px;
                }

                .ts-icon-picker-item {
                    aspect-ratio: 1;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 1.25rem;
                    border-radius: 10px;
                    cursor: pointer;
                    transition: all 0.2s;
                    background: var(--ts-bg-secondary, #f8fafc);
                    color: var(--ts-text-secondary, #64748b);
                    border: 2px solid transparent;
                }

                .ts-icon-picker-item:hover {
                    background: var(--ts-bg-tertiary, #f1f5f9);
                    transform: scale(1.1);
                    color: #6366f1;
                }

                .ts-icon-picker-item.selected {
                    background: linear-gradient(135deg, #6366f1, #8b5cf6);
                    color: white;
                    transform: scale(1.1);
                    box-shadow: 0 4px 12px rgba(99, 102, 241, 0.4);
                }

                .ts-icon-picker-footer {
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    padding: 1rem;
                    border-top: 1px solid var(--ts-border, #e2e8f0);
                    background: var(--ts-bg-secondary, #f8fafc);
                }

                .ts-icon-picker-preview {
                    display: flex;
                    align-items: center;
                    gap: 0.75rem;
                }

                .ts-icon-picker-preview i {
                    width: 40px;
                    height: 40px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background: linear-gradient(135deg, #6366f1, #8b5cf6);
                    color: white;
                    border-radius: 10px;
                    font-size: 1.125rem;
                }

                .ts-icon-picker-preview span {
                    font-family: 'Fira Code', monospace;
                    font-size: 0.8125rem;
                    color: var(--ts-text-secondary, #64748b);
                }

                .ts-icon-picker-actions {
                    display: flex;
                    gap: 0.5rem;
                }
            `;
            document.head.appendChild(styles);
        }
    }

    function open(inputElement, onSelect) {
        init();
        currentInput = inputElement;
        callback = onSelect;

        // Pre-select current value
        const currentValue = inputElement ? inputElement.value : '';
        modal.querySelectorAll('.ts-icon-picker-item').forEach(item => {
            item.classList.toggle('selected', item.dataset.icon === currentValue);
        });

        if (currentValue) {
            const preview = modal.querySelector('.ts-icon-picker-preview');
            preview.querySelector('i').className = currentValue;
            preview.querySelector('span').textContent = currentValue;
        }

        modal.classList.add('active');
        modal.querySelector('.ts-icon-picker-search input').focus();
    }

    function close() {
        modal.classList.remove('active');
        currentInput = null;
        callback = null;
    }

    function confirm() {
        const selected = modal.querySelector('.ts-icon-picker-item.selected');
        if (selected) {
            const icon = selected.dataset.icon;
            if (currentInput) {
                currentInput.value = icon;
            }
            if (callback) {
                callback(icon);
            }
        }
        close();
    }

    return { open, close };
})();

// Global function
window.openIconPicker = function(input, callback) {
    IconPicker.open(input, callback);
};


// ============================================================================
// UTILITIES
// ============================================================================

const TSUtils = {
    // Debounce function
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
    },

    // Throttle function
    throttle(func, limit) {
        let inThrottle;
        return function(...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    },

    // Format bytes
    formatBytes(bytes, decimals = 2) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const dm = decimals < 0 ? 0 : decimals;
        const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
    },

    // Copy to clipboard
    async copyToClipboard(text) {
        try {
            await navigator.clipboard.writeText(text);
            Toast.success('Copiado!', 'Texto copiado para a área de transferência');
            return true;
        } catch (err) {
            Toast.error('Erro', 'Não foi possível copiar');
            return false;
        }
    },

    // Generate unique ID
    generateId(prefix = 'ts') {
        return `${prefix}_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    },

    // Slugify
    slugify(text) {
        return text
            .toString()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .toLowerCase()
            .trim()
            .replace(/\s+/g, '-')
            .replace(/[^\w-]+/g, '')
            .replace(/--+/g, '-');
    },

    // Deep clone
    deepClone(obj) {
        return JSON.parse(JSON.stringify(obj));
    },

    // Check if mobile
    isMobile() {
        return window.innerWidth < 768;
    },

    // Animate element
    animate(element, animationClass, duration = 500) {
        return new Promise(resolve => {
            element.classList.add(animationClass);
            setTimeout(() => {
                element.classList.remove(animationClass);
                resolve();
            }, duration);
        });
    },

    // Smooth scroll to element
    scrollTo(element, offset = 0) {
        const top = element.getBoundingClientRect().top + window.pageYOffset - offset;
        window.scrollTo({ top, behavior: 'smooth' });
    },

    // Parse query string
    parseQueryString(queryString) {
        const params = {};
        const searchParams = new URLSearchParams(queryString);
        for (const [key, value] of searchParams) {
            params[key] = value;
        }
        return params;
    },

    // Serialize form
    serializeForm(form) {
        const formData = new FormData(form);
        const data = {};
        for (const [key, value] of formData) {
            if (data[key]) {
                if (!Array.isArray(data[key])) {
                    data[key] = [data[key]];
                }
                data[key].push(value);
            } else {
                data[key] = value;
            }
        }
        return data;
    }
};

// Export to window
window.TSUtils = TSUtils;


// ============================================================================
// CONFIRMATION MODAL
// ============================================================================

const Confirm = (function() {
    'use strict';

    let modal = null;
    let resolveCallback = null;

    function init() {
        if (modal) return;

        modal = document.createElement('div');
        modal.className = 'ts-confirm-modal';
        modal.innerHTML = `
            <div class="ts-confirm-backdrop"></div>
            <div class="ts-confirm-dialog">
                <div class="ts-confirm-icon"></div>
                <div class="ts-confirm-title"></div>
                <div class="ts-confirm-message"></div>
                <div class="ts-confirm-actions">
                    <button class="ts-btn ts-btn-secondary ts-confirm-cancel">Cancelar</button>
                    <button class="ts-btn ts-confirm-ok">Confirmar</button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        modal.querySelector('.ts-confirm-backdrop').addEventListener('click', () => close(false));
        modal.querySelector('.ts-confirm-cancel').addEventListener('click', () => close(false));
        modal.querySelector('.ts-confirm-ok').addEventListener('click', () => close(true));

        // Styles
        if (!document.getElementById('ts-confirm-styles')) {
            const styles = document.createElement('style');
            styles.id = 'ts-confirm-styles';
            styles.textContent = `
                .ts-confirm-modal {
                    display: none;
                    position: fixed;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    z-index: 99999;
                }

                .ts-confirm-modal.active {
                    display: block;
                }

                .ts-confirm-backdrop {
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: rgba(0, 0, 0, 0.5);
                    backdrop-filter: blur(4px);
                    animation: tsFadeIn 0.2s ease;
                }

                .ts-confirm-dialog {
                    position: absolute;
                    top: 50%;
                    left: 50%;
                    transform: translate(-50%, -50%);
                    background: var(--ts-bg-primary, #fff);
                    border-radius: 16px;
                    padding: 2rem;
                    width: 90%;
                    max-width: 400px;
                    text-align: center;
                    box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
                    animation: tsBounceIn 0.4s ease;
                }

                .ts-confirm-icon {
                    width: 64px;
                    height: 64px;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin: 0 auto 1rem;
                    font-size: 2rem;
                }

                .ts-confirm-icon.danger {
                    background: rgba(239, 68, 68, 0.1);
                    color: #ef4444;
                }

                .ts-confirm-icon.warning {
                    background: rgba(245, 158, 11, 0.1);
                    color: #f59e0b;
                }

                .ts-confirm-icon.info {
                    background: rgba(6, 182, 212, 0.1);
                    color: #06b6d4;
                }

                .ts-confirm-title {
                    font-size: 1.25rem;
                    font-weight: 600;
                    color: var(--ts-text-primary, #1e293b);
                    margin-bottom: 0.5rem;
                }

                .ts-confirm-message {
                    color: var(--ts-text-secondary, #64748b);
                    font-size: 0.9375rem;
                    margin-bottom: 1.5rem;
                    line-height: 1.5;
                }

                .ts-confirm-actions {
                    display: flex;
                    gap: 0.75rem;
                    justify-content: center;
                }

                .ts-confirm-actions .ts-btn {
                    min-width: 120px;
                }

                .ts-confirm-modal.danger .ts-confirm-ok {
                    background: linear-gradient(135deg, #ef4444, #dc2626);
                    color: white;
                }

                .ts-confirm-modal.warning .ts-confirm-ok {
                    background: linear-gradient(135deg, #f59e0b, #d97706);
                    color: white;
                }
            `;
            document.head.appendChild(styles);
        }
    }

    function show(options = {}) {
        init();

        const {
            title = 'Confirmar ação',
            message = 'Deseja continuar?',
            type = 'info',
            confirmText = 'Confirmar',
            cancelText = 'Cancelar'
        } = options;

        const icons = {
            danger: '<i class="fas fa-exclamation-triangle"></i>',
            warning: '<i class="fas fa-exclamation-circle"></i>',
            info: '<i class="fas fa-question-circle"></i>'
        };

        modal.className = `ts-confirm-modal active ${type}`;
        modal.querySelector('.ts-confirm-icon').className = `ts-confirm-icon ${type}`;
        modal.querySelector('.ts-confirm-icon').innerHTML = icons[type];
        modal.querySelector('.ts-confirm-title').textContent = title;
        modal.querySelector('.ts-confirm-message').textContent = message;
        modal.querySelector('.ts-confirm-ok').textContent = confirmText;
        modal.querySelector('.ts-confirm-cancel').textContent = cancelText;

        return new Promise(resolve => {
            resolveCallback = resolve;
        });
    }

    function close(result) {
        modal.classList.remove('active');
        if (resolveCallback) {
            resolveCallback(result);
            resolveCallback = null;
        }
    }

    function danger(title, message) {
        return show({ title, message, type: 'danger', confirmText: 'Excluir' });
    }

    function warning(title, message) {
        return show({ title, message, type: 'warning' });
    }

    return { show, danger, warning };
})();

window.Confirm = Confirm;
