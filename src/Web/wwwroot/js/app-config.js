// =============================================================================
// APP-CONFIG.JS v1.1 - CONFIGURAÇÃO GLOBAL (SEM DEPENDÊNCIA DO JQUERY)
// =============================================================================
// ⭐ IMPORTANTE: Este arquivo DEVE ser carregado ANTES de todos os outros!
// NÃO usa jQuery para garantir que funciona mesmo antes do jQuery carregar
// =============================================================================

(function () {
    'use strict';

    console.log('📦 [AppConfig] Inicializando módulo de configuração...');

    // =========================================================================
    // CONFIGURAÇÃO PADRÃO (FALLBACK)
    // =========================================================================
    window.AppConfig = {
        apiBaseUrl: 'https://localhost:7193', // ⚠️ FALLBACK - será substituído pelo backend
        version: '1.0.0',
        environment: 'Development',
        isDevelopment: true,
        defaultTimeout: 30000,
        _loaded: false
    };

    // =========================================================================
    // HELPER: Constrói URL completa da API
    // =========================================================================
    window.AppConfig.buildApiUrl = function (endpoint) {
        if (!endpoint) {
            console.warn('⚠️ [AppConfig] Endpoint vazio!');
            return this.apiBaseUrl;
        }

        // Se já for URL completa, retorna como está
        if (endpoint.startsWith('http://') || endpoint.startsWith('https://')) {
            return endpoint;
        }

        // Remove barra inicial se houver
        const cleanEndpoint = endpoint.startsWith('/')
            ? endpoint.substring(1)
            : endpoint;

        // Remove barra final da baseUrl se houver
        const cleanBaseUrl = this.apiBaseUrl.endsWith('/')
            ? this.apiBaseUrl.substring(0, this.apiBaseUrl.length - 1)
            : this.apiBaseUrl;

        const fullUrl = cleanBaseUrl + '/' + cleanEndpoint;

        console.log('🔗 [AppConfig] URL construída:', {
            endpoint: endpoint,
            baseUrl: this.apiBaseUrl,
            fullUrl: fullUrl
        });

        return fullUrl;
    };

    // =========================================================================
    // CARREGA CONFIGURAÇÕES DO BACKEND (VANILLA JS - SEM JQUERY)
    // =========================================================================
    window.AppConfig.load = function () {
        console.log('🔧 [AppConfig] Carregando configuração do backend...');

        return fetch('/Account/GetConfig', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            cache: 'no-cache'
        })
            .then(function (response) {
                if (!response.ok) {
                    throw new Error('HTTP ' + response.status);
                }
                return response.json();
            })
            .then(function (config) {
                // Atualiza configurações
                window.AppConfig.apiBaseUrl = config.apiBaseUrl || window.AppConfig.apiBaseUrl;
                window.AppConfig.version = config.version || window.AppConfig.version;
                window.AppConfig.environment = config.environment || window.AppConfig.environment;
                window.AppConfig.isDevelopment = config.isDevelopment !== undefined
                    ? config.isDevelopment
                    : window.AppConfig.isDevelopment;
                window.AppConfig.defaultTimeout = config.defaultTimeout || window.AppConfig.defaultTimeout;
                window.AppConfig._loaded = true;

                console.log('✅ [AppConfig] Configuração carregada do backend:', {
                    apiBaseUrl: window.AppConfig.apiBaseUrl,
                    environment: window.AppConfig.environment,
                    version: window.AppConfig.version
                });

                return window.AppConfig;
            })
            .catch(function (error) {
                console.warn('⚠️ [AppConfig] Erro ao carregar do backend:', error.message);
                console.warn('💡 [AppConfig] Usando configuração FALLBACK:', {
                    apiBaseUrl: window.AppConfig.apiBaseUrl
                });

                window.AppConfig._loaded = true;
                return window.AppConfig;
            });
    };

    // =========================================================================
    // AUTO-CARREGA quando DOM estiver pronto (VANILLA JS)
    // =========================================================================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            if (!window.AppConfig._loaded) {
                window.AppConfig.load();
            }
        });
    } else {
        // DOM já está pronto
        if (!window.AppConfig._loaded) {
            window.AppConfig.load();
        }
    }

    console.log('✅ [AppConfig] Módulo inicializado - aguardando carregamento do DOM');
})();