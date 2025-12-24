-- ============================================================================
-- SGT v3.0 - SISTEMA DE GESTÃO DE TERCEIROS E PORTARIA
-- Integração com Tabelas Legadas
-- Script de Criação do Banco de Dados com Prefixos de Módulo
-- ============================================================================
-- Compatível com: SQL Server 2019+
-- Versão: 3.0 - Com Integração a Tabelas Legadas
-- Data: Dezembro de 2025
-- 
-- TABELAS LEGADAS (NÃO ALTERADAS):
--   - temp1 (Empresas)
--   - test1 (Filiais)
--   - tcus1 (Centros de Custo)
--   - func1 (Funcionários)
--   - tuse1 (Usuários)
-- ============================================================================

-- ============================================================================
-- 1. TABELAS BAS - PLATAFORMA E GOVERNANÇA
-- ============================================================================

-- Tabela de Unidades Federativas
CREATE TABLE bas_ufs (
    id INT PRIMARY KEY IDENTITY(1,1),
    sigla NVARCHAR(2) UNIQUE NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100)
);

-- Tabela de Tipos Sanguíneos
CREATE TABLE bas_tipos_sanguineo (
    id INT PRIMARY KEY IDENTITY(1,1),
    tipo NVARCHAR(10) UNIQUE NOT NULL,
    descricao NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100)
);

-- Tabela de Parentescos
CREATE TABLE bas_parentescos (
    id INT PRIMARY KEY IDENTITY(1,1),
    nome NVARCHAR(50) UNIQUE NOT NULL,
    descricao NVARCHAR(200),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100)
);

-- Tabela de Feriados Corporativos
CREATE TABLE bas_feriados (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    data DATE NOT NULL,
    descricao NVARCHAR(255) NOT NULL,
    tipo NVARCHAR(50),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    INDEX ix_bas_feriados_data (data)
);

-- Tabela de Numerações Sequenciais
CREATE TABLE bas_numeracoes (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    tipo NVARCHAR(100) NOT NULL,
    proximo_numero BIGINT DEFAULT 1,
    prefixo NVARCHAR(20),
    sufixo NVARCHAR(20),
    formato NVARCHAR(100),
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    UNIQUE (id_saas, tipo)
);

-- Tabela de Configurações do Sistema
CREATE TABLE bas_configuracoes (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    chave NVARCHAR(100) NOT NULL UNIQUE,
    valor NVARCHAR(MAX) NOT NULL,
    descricao NVARCHAR(500),
    tipo_dado NVARCHAR(50),
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100)
);

-- Tabela de Auditoria de Alterações
CREATE TABLE bas_auditoria_alteracoes (
    id BIGINT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    tabela_afetada NVARCHAR(100) NOT NULL,
    id_registro INT NOT NULL,
    tipo_operacao NVARCHAR(20),
    valor_anterior NVARCHAR(MAX),
    valor_novo NVARCHAR(MAX),
    campo_alterado NVARCHAR(100),
    data_alteracao DATETIME DEFAULT GETDATE(),
    usuario_alteracao NVARCHAR(100),
    endereco_ip NVARCHAR(50),
    
    INDEX ix_bas_auditoria_tabela (tabela_afetada),
    INDEX ix_bas_auditoria_data (data_alteracao)
);

-- ============================================================================
-- 2. TABELAS CAP - CADASTRO DE PESSOAS E FORNECEDORES
-- ============================================================================

-- NOTA: As tabelas legadas NÃO são alteradas:
-- - temp1 (Empresas próprias)
-- - test1 (Filiais)
-- - tcus1 (Centros de Custo)
-- - func1 (Funcionários próprios)
-- - tuse1 (Usuários)

-- Tabela de Fornecedores (Empresas Terceirizadas) - SEPARADA
CREATE TABLE cap_fornecedores (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    razao_social NVARCHAR(255) NOT NULL,
    nome_fantasia NVARCHAR(255),
    cnpj NVARCHAR(18) UNIQUE,
    cpf NVARCHAR(14),
    email NVARCHAR(100),
    telefone NVARCHAR(20),
    endereco NVARCHAR(500),
    numero NVARCHAR(20),
    complemento NVARCHAR(200),
    bairro NVARCHAR(100),
    cidade NVARCHAR(100),
    id_uf INT,
    cep NVARCHAR(10),
    contato NVARCHAR(100),
    contato_telefone NVARCHAR(20),
    contato_email NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_fornecedores_id_uf FOREIGN KEY (id_uf) REFERENCES bas_ufs(id),
    INDEX ix_cap_fornecedores_cnpj (cnpj),
    INDEX ix_cap_fornecedores_razao_social (razao_social)
);

-- Tabela de Colaboradores de Fornecedores (Funcionários Terceirizados) - SEPARADA
CREATE TABLE cap_colaboradores_fornecedor (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_fornecedor INT NOT NULL,
    nome NVARCHAR(255) NOT NULL,
    cpf NVARCHAR(14) UNIQUE NOT NULL,
    rg NVARCHAR(20),
    email NVARCHAR(100),
    telefone NVARCHAR(20),
    data_nascimento DATE,
    genero NVARCHAR(20),
    estado_civil NVARCHAR(50),
    id_tipo_sanguineo INT,
    endereco NVARCHAR(500),
    numero NVARCHAR(20),
    complemento NVARCHAR(200),
    bairro NVARCHAR(100),
    cidade NVARCHAR(100),
    id_uf INT,
    cep NVARCHAR(10),
    data_admissao DATE NOT NULL,
    data_demissao DATE,
    cargo NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_colab_id_fornecedor FOREIGN KEY (id_fornecedor) REFERENCES cap_fornecedores(id),
    CONSTRAINT fk_cap_colab_id_tipo_sanguineo FOREIGN KEY (id_tipo_sanguineo) REFERENCES bas_tipos_sanguineo(id),
    CONSTRAINT fk_cap_colab_id_uf FOREIGN KEY (id_uf) REFERENCES bas_ufs(id),
    INDEX ix_cap_colab_cpf (cpf),
    INDEX ix_cap_colab_nome (nome),
    INDEX ix_cap_colab_id_fornecedor (id_fornecedor)
);

-- Tabela de Visitantes
CREATE TABLE cap_visitantes (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(255) NOT NULL,
    cpf NVARCHAR(14) UNIQUE,
    rg NVARCHAR(20),
    email NVARCHAR(100),
    telefone NVARCHAR(20),
    empresa NVARCHAR(255),
    id_funcionario_responsavel INT,
    requer_responsavel BIT DEFAULT 0,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    INDEX ix_cap_visitantes_cpf (cpf),
    INDEX ix_cap_visitantes_nome (nome)
);

-- Tabela de Bloqueios de Pessoas
CREATE TABLE cap_bloqueios_pessoa (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_visitante INT,
    motivo NVARCHAR(500) NOT NULL,
    data_bloqueio DATETIME DEFAULT GETDATE(),
    data_desbloqueio DATETIME,
    usuario_bloqueio NVARCHAR(100),
    usuario_desbloqueio NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_bloqueios_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    CONSTRAINT fk_cap_bloqueios_id_visitante FOREIGN KEY (id_visitante) REFERENCES cap_visitantes(id),
    INDEX ix_cap_bloqueios_ativo (ativo),
    INDEX ix_cap_bloqueios_data (data_bloqueio)
);

-- Tabela de Histórico de Bloqueios
CREATE TABLE cap_historico_bloqueios (
    id BIGINT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_bloqueio INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_visitante INT,
    motivo NVARCHAR(500),
    data_bloqueio DATETIME,
    data_desbloqueio DATETIME,
    usuario_bloqueio NVARCHAR(100),
    usuario_desbloqueio NVARCHAR(100),
    data_registro DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT fk_cap_hist_bloqueios_id_bloqueio FOREIGN KEY (id_bloqueio) REFERENCES cap_bloqueios_pessoa(id),
    CONSTRAINT fk_cap_hist_bloqueios_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    CONSTRAINT fk_cap_hist_bloqueios_id_visitante FOREIGN KEY (id_visitante) REFERENCES cap_visitantes(id),
    INDEX ix_cap_hist_bloqueios_data (data_registro)
);

-- Tabela de Contratos com Fornecedores
CREATE TABLE cap_contratos_fornecedor (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_fornecedor INT NOT NULL,
    numero_contrato NVARCHAR(100) UNIQUE NOT NULL,
    data_inicio DATE NOT NULL,
    data_fim DATE,
    valor DECIMAL(15,2),
    descricao NVARCHAR(500),
    status NVARCHAR(50),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_contratos_id_fornecedor FOREIGN KEY (id_fornecedor) REFERENCES cap_fornecedores(id),
    INDEX ix_cap_contratos_data_fim (data_fim),
    INDEX ix_cap_contratos_status (status)
);

-- Tabela de Responsáveis por Contrato
CREATE TABLE cap_responsaveis_contrato (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_contrato INT NOT NULL,
    id_funcionario_legado INT,
    tipo_responsabilidade NVARCHAR(100),
    data_inicio DATE,
    data_fim DATE,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_resp_contrato_id_contrato FOREIGN KEY (id_contrato) REFERENCES cap_contratos_fornecedor(id),
    INDEX ix_cap_resp_contrato_id_contrato (id_contrato)
);

-- ============================================================================
-- 3. TABELAS GTC - CONTROLE DE ACESSO E PORTARIA
-- ============================================================================

-- Tabela de Áreas de Risco
CREATE TABLE gtc_areas (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    nivel_risco NVARCHAR(50),
    requer_checklist BIT DEFAULT 0,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    INDEX ix_gtc_areas_nome (nome)
);

-- Tabela de Portarias Físicas
CREATE TABLE gtc_portarias (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),
    endereco NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    INDEX ix_gtc_portarias_nome (nome)
);

-- Tabela de Permissões de Acesso por Área
CREATE TABLE gtc_permissoes_area (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_area INT NOT NULL,
    permitido BIT DEFAULT 1,
    data_inicio DATE,
    data_fim DATE,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_permissoes_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_gtc_permissoes_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_gtc_permissoes_id_area (id_area)
);

-- Tabela de Agendamentos de Acesso
CREATE TABLE gtc_agendamentos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_visitante INT,
    id_area INT,
    data_agendamento DATE NOT NULL,
    hora_entrada_prevista TIME,
    hora_saida_prevista TIME,
    motivo NVARCHAR(500),
    status NVARCHAR(50),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_agendamentos_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_gtc_agendamentos_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    CONSTRAINT fk_gtc_agendamentos_id_visitante FOREIGN KEY (id_visitante) REFERENCES cap_visitantes(id),
    INDEX ix_gtc_agendamentos_data (data_agendamento),
    INDEX ix_gtc_agendamentos_status (status)
);

-- Tabela de Autorizações Formais de Acesso
CREATE TABLE gtc_autorizacoes (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_agendamento INT NOT NULL,
    numero_autorizacao NVARCHAR(100) UNIQUE NOT NULL,
    status NVARCHAR(50),
    usuario_autorizador NVARCHAR(100),
    data_autorizacao DATETIME,
    motivo_recusa NVARCHAR(500),
    observacoes NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_autorizacoes_id_agendamento FOREIGN KEY (id_agendamento) REFERENCES gtc_agendamentos(id),
    INDEX ix_gtc_autorizacoes_status (status),
    INDEX ix_gtc_autorizacoes_numero (numero_autorizacao)
);

-- Tabela de Acessos na Portaria
CREATE TABLE gtc_acessos_portaria (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    numero_protocolo NVARCHAR(100) UNIQUE NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_visitante INT,
    id_area INT,
    id_portaria INT,
    id_autorizacao INT,
    data_entrada DATETIME NOT NULL,
    data_saida DATETIME,
    hora_saida_prevista TIME,
    motivo NVARCHAR(500),
    setor NVARCHAR(100),
    contato_interno NVARCHAR(100),
    autorizado BIT DEFAULT 1,
    motivo_bloqueio NVARCHAR(500),
    observacoes NVARCHAR(500),
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_acessos_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_gtc_acessos_id_portaria FOREIGN KEY (id_portaria) REFERENCES gtc_portarias(id),
    CONSTRAINT fk_gtc_acessos_id_autorizacao FOREIGN KEY (id_autorizacao) REFERENCES gtc_autorizacoes(id),
    CONSTRAINT fk_gtc_acessos_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    CONSTRAINT fk_gtc_acessos_id_visitante FOREIGN KEY (id_visitante) REFERENCES cap_visitantes(id),
    INDEX ix_gtc_acessos_data_entrada (data_entrada),
    INDEX ix_gtc_acessos_autorizado (autorizado),
    INDEX ix_gtc_acessos_numero_protocolo (numero_protocolo)
);

-- Tabela de Tipos de Veículos
CREATE TABLE gtc_tipos_veiculo (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    INDEX ix_gtc_tipos_veiculo_nome (nome)
);

-- Tabela de Veículos
CREATE TABLE gtc_veiculos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_fornecedor INT,
    id_tipo_veiculo INT NOT NULL,
    placa NVARCHAR(10) UNIQUE NOT NULL,
    marca NVARCHAR(100),
    modelo NVARCHAR(100),
    cor NVARCHAR(50),
    ano INT,
    renavam NVARCHAR(20),
    proprietario NVARCHAR(255),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_veiculos_id_fornecedor FOREIGN KEY (id_fornecedor) REFERENCES cap_fornecedores(id),
    CONSTRAINT fk_gtc_veiculos_id_tipo_veiculo FOREIGN KEY (id_tipo_veiculo) REFERENCES gtc_tipos_veiculo(id),
    INDEX ix_gtc_veiculos_placa (placa)
);

-- Tabela de CRLV
CREATE TABLE gtc_crlv (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_veiculo INT NOT NULL,
    numero_crlv NVARCHAR(50) UNIQUE,
    data_emissao DATE,
    data_vencimento DATE,
    status NVARCHAR(50),
    observacoes NVARCHAR(500),
    caminho_arquivo NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_crlv_id_veiculo FOREIGN KEY (id_veiculo) REFERENCES gtc_veiculos(id),
    INDEX ix_gtc_crlv_data_vencimento (data_vencimento)
);

-- Tabela de Acessos de Veículos
CREATE TABLE gtc_acessos_veiculos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    numero_protocolo NVARCHAR(100) UNIQUE NOT NULL,
    id_veiculo INT NOT NULL,
    data_entrada DATETIME NOT NULL,
    data_saida DATETIME,
    motivo NVARCHAR(500),
    autorizado BIT DEFAULT 1,
    motivo_bloqueio NVARCHAR(500),
    observacoes NVARCHAR(500),
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_acessos_veic_id_veiculo FOREIGN KEY (id_veiculo) REFERENCES gtc_veiculos(id),
    INDEX ix_gtc_acessos_veic_data_entrada (data_entrada),
    INDEX ix_gtc_acessos_veic_id_veiculo (id_veiculo)
);

-- Tabela de Crachás
CREATE TABLE gtc_crachas (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    numero_cracha NVARCHAR(50) UNIQUE NOT NULL,
    data_emissao DATE NOT NULL,
    data_vencimento DATE,
    status NVARCHAR(50),
    observacoes NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_crachas_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_gtc_crachas_numero (numero_cracha),
    INDEX ix_gtc_crachas_status (status)
);

-- Tabela de Tipos de Checklist
CREATE TABLE gtc_tipos_checklist (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    periodicidade NVARCHAR(50),
    obrigatorio BIT DEFAULT 0,
    aplicavel_a NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    INDEX ix_gtc_tipos_checklist_nome (nome)
);

-- Tabela de Itens do Checklist
CREATE TABLE gtc_itens_checklist (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_tipo_checklist INT NOT NULL,
    ordem INT,
    descricao NVARCHAR(500) NOT NULL,
    tipo_resposta NVARCHAR(50),
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_itens_checklist_id_tipo FOREIGN KEY (id_tipo_checklist) REFERENCES gtc_tipos_checklist(id),
    INDEX ix_gtc_itens_checklist_id_tipo (id_tipo_checklist)
);

-- Tabela de Regras de Checklist por Área
CREATE TABLE gtc_regras_checklist_area (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_area INT NOT NULL,
    id_tipo_checklist INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_regras_checklist_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_gtc_regras_checklist_id_tipo FOREIGN KEY (id_tipo_checklist) REFERENCES gtc_tipos_checklist(id),
    UNIQUE (id_area, id_tipo_checklist)
);

-- Tabela de Checklists Preenchidos
CREATE TABLE gtc_checklists (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_tipo_checklist INT NOT NULL,
    id_funcionario_responsavel INT,
    id_colaborador_responsavel INT,
    data_preenchimento DATE NOT NULL,
    status NVARCHAR(50),
    observacoes NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_checklists_id_tipo FOREIGN KEY (id_tipo_checklist) REFERENCES gtc_tipos_checklist(id),
    CONSTRAINT fk_gtc_checklists_id_colab FOREIGN KEY (id_colaborador_responsavel) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_gtc_checklists_data (data_preenchimento),
    INDEX ix_gtc_checklists_id_tipo (id_tipo_checklist)
);

-- Tabela de Respostas do Checklist
CREATE TABLE gtc_respostas_checklist (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_checklist INT NOT NULL,
    id_item_checklist INT NOT NULL,
    resposta NVARCHAR(MAX),
    data_resposta DATETIME,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_respostas_id_checklist FOREIGN KEY (id_checklist) REFERENCES gtc_checklists(id),
    CONSTRAINT fk_gtc_respostas_id_item FOREIGN KEY (id_item_checklist) REFERENCES gtc_itens_checklist(id),
    INDEX ix_gtc_respostas_id_checklist (id_checklist)
);

-- ============================================================================
-- 4. TABELAS MSO - EQUIPAMENTOS DE PROTEÇÃO INDIVIDUAL
-- ============================================================================

-- Tabela de Grupos de EPI
CREATE TABLE mso_grupos_epi (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    INDEX ix_mso_grupos_epi_nome (nome)
);

-- Tabela de EPIs
CREATE TABLE mso_epis (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_grupo_epi INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    codigo_epi NVARCHAR(50),
    norma NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_epis_id_grupo FOREIGN KEY (id_grupo_epi) REFERENCES mso_grupos_epi(id),
    INDEX ix_mso_epis_nome (nome)
);

-- Tabela de Entregas de EPI
CREATE TABLE mso_entregas_epi (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_epi INT NOT NULL,
    data_entrega DATE NOT NULL,
    data_devolucao DATE,
    quantidade INT DEFAULT 1,
    observacoes NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_entregas_id_epi FOREIGN KEY (id_epi) REFERENCES mso_epis(id),
    CONSTRAINT fk_mso_entregas_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_mso_entregas_data (data_entrega)
);

-- Tabela de Regras de EPI por Área
CREATE TABLE mso_regras_epi_area (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_area INT NOT NULL,
    id_epi INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_regras_area_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_mso_regras_area_id_epi FOREIGN KEY (id_epi) REFERENCES mso_epis(id),
    UNIQUE (id_area, id_epi)
);

-- Tabela de Regras de EPI por Tipo de Veículo
CREATE TABLE mso_regras_epi_veiculo (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_tipo_veiculo INT NOT NULL,
    id_epi INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_regras_veic_id_tipo FOREIGN KEY (id_tipo_veiculo) REFERENCES gtc_tipos_veiculo(id),
    CONSTRAINT fk_mso_regras_veic_id_epi FOREIGN KEY (id_epi) REFERENCES mso_epis(id),
    UNIQUE (id_tipo_veiculo, id_epi)
);

-- ============================================================================
-- 5. TABELAS TRE - TREINAMENTOS E CAPACITAÇÕES
-- ============================================================================

-- Tabela de Tipos de Treinamento
CREATE TABLE tre_tipos_treinamento (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    codigo_nr NVARCHAR(20),
    dias_prazo_validade INT,
    obrigatorio BIT DEFAULT 0,
    aplicavel_a NVARCHAR(100),
    carga_horaria INT,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    INDEX ix_tre_tipos_nome (nome),
    INDEX ix_tre_tipos_codigo_nr (codigo_nr)
);

-- Tabela de Cursos Obrigatórios por Cargo (Legado)
CREATE TABLE tre_cursos_por_cargo_legado (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_cargo_legado INT NOT NULL,
    id_tipo_treinamento INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    dias_prazo_validade INT,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_tre_cursos_cargo_id_tipo FOREIGN KEY (id_tipo_treinamento) REFERENCES tre_tipos_treinamento(id),
    UNIQUE (id_cargo_legado, id_tipo_treinamento)
);

-- Tabela de Regras de Treinamento por Área
CREATE TABLE tre_regras_treinamento_area (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_area INT NOT NULL,
    id_tipo_treinamento INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_tre_regras_area_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_tre_regras_area_id_tipo FOREIGN KEY (id_tipo_treinamento) REFERENCES tre_tipos_treinamento(id),
    UNIQUE (id_area, id_tipo_treinamento)
);

-- Tabela de Treinamentos Realizados
CREATE TABLE tre_treinamentos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_tipo_treinamento INT NOT NULL,
    data_realizacao DATE NOT NULL,
    data_vencimento DATE,
    instrutor NVARCHAR(255),
    local_realizacao NVARCHAR(255),
    resultado NVARCHAR(50),
    nota DECIMAL(5,2),
    observacoes NVARCHAR(500),
    caminho_arquivo NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_tre_treinamentos_id_tipo FOREIGN KEY (id_tipo_treinamento) REFERENCES tre_tipos_treinamento(id),
    CONSTRAINT fk_tre_treinamentos_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_tre_treinamentos_data_vencimento (data_vencimento),
    INDEX ix_tre_treinamentos_resultado (resultado)
);

-- ============================================================================
-- 6. TABELAS DOC - GESTÃO DOCUMENTAL
-- ============================================================================

-- Tabela de Tipos de Documentos
CREATE TABLE doc_tipos_documento (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    dias_prazo_validade INT,
    obrigatorio BIT DEFAULT 0,
    aplicavel_a NVARCHAR(100),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    INDEX ix_doc_tipos_nome (nome)
);

-- Tabela de Documentos
CREATE TABLE doc_documentos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,
    id_colaborador_fornecedor INT,
    id_fornecedor INT,
    id_tipo_documento INT NOT NULL,
    numero_documento NVARCHAR(100),
    data_emissao DATE,
    data_vencimento DATE,
    status_verificacao NVARCHAR(50),
    observacoes NVARCHAR(500),
    caminho_arquivo NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_doc_documentos_id_tipo FOREIGN KEY (id_tipo_documento) REFERENCES doc_tipos_documento(id),
    CONSTRAINT fk_doc_documentos_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    CONSTRAINT fk_doc_documentos_id_fornecedor FOREIGN KEY (id_fornecedor) REFERENCES cap_fornecedores(id),
    INDEX ix_doc_documentos_data_vencimento (data_vencimento)
);

-- Tabela de Documentos Obrigatórios por Contrato
CREATE TABLE doc_documentos_contrato (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_contrato INT NOT NULL,
    id_tipo_documento INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_doc_doc_contrato_id_contrato FOREIGN KEY (id_contrato) REFERENCES cap_contratos_fornecedor(id),
    CONSTRAINT fk_doc_doc_contrato_id_tipo FOREIGN KEY (id_tipo_documento) REFERENCES doc_tipos_documento(id),
    UNIQUE (id_contrato, id_tipo_documento)
);

-- Tabela de Regras de Documentos por Área
CREATE TABLE doc_regras_documento_area (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_area INT NOT NULL,
    id_tipo_documento INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_doc_regras_area_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_doc_regras_area_id_tipo FOREIGN KEY (id_tipo_documento) REFERENCES doc_tipos_documento(id),
    UNIQUE (id_area, id_tipo_documento)
);

-- ============================================================================
-- 7. TABELAS CAP - OCORRÊNCIAS E EVIDÊNCIAS
-- ============================================================================

-- Tabela de Tipos de Ocorrência
CREATE TABLE cap_tipos_ocorrencia (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    severidade NVARCHAR(50),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    INDEX ix_cap_tipos_ocorrencia_nome (nome)
);

-- Tabela de Ocorrências
CREATE TABLE cap_ocorrencias (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_tipo_ocorrencia INT NOT NULL,
    id_acesso_portaria INT,
    id_funcionario_responsavel INT,
    id_colaborador_responsavel INT,
    descricao NVARCHAR(500) NOT NULL,
    data_ocorrencia DATETIME NOT NULL,
    local NVARCHAR(500),
    observacoes NVARCHAR(500),
    status NVARCHAR(50),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_ocorrencias_id_tipo FOREIGN KEY (id_tipo_ocorrencia) REFERENCES cap_tipos_ocorrencia(id),
    CONSTRAINT fk_cap_ocorrencias_id_acesso FOREIGN KEY (id_acesso_portaria) REFERENCES gtc_acessos_portaria(id),
    CONSTRAINT fk_cap_ocorrencias_id_colab FOREIGN KEY (id_colaborador_responsavel) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_cap_ocorrencias_data (data_ocorrencia),
    INDEX ix_cap_ocorrencias_status (status)
);

-- Tabela de Anexos de Ocorrência
CREATE TABLE cap_anexos_ocorrencia (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_ocorrencia INT NOT NULL,
    nome_arquivo NVARCHAR(255) NOT NULL,
    caminho_arquivo NVARCHAR(500) NOT NULL,
    tipo_arquivo NVARCHAR(50),
    tamanho_arquivo BIGINT,
    data_upload DATETIME DEFAULT GETDATE(),
    usuario_upload NVARCHAR(100),
    
    CONSTRAINT fk_cap_anexos_id_ocorrencia FOREIGN KEY (id_ocorrencia) REFERENCES cap_ocorrencias(id),
    INDEX ix_cap_anexos_id_ocorrencia (id_ocorrencia)
);

-- ============================================================================
-- 8. TABELAS INT - INTEGRAÇÕES INDUSTRIAIS
-- ============================================================================

-- Tabela de Tipos de Dispositivos
CREATE TABLE int_tipos_dispositivo (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    INDEX ix_int_tipos_dispositivo_nome (nome)
);

-- Tabela de Dispositivos Físicos
CREATE TABLE int_dispositivos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_portaria INT NOT NULL,
    id_tipo_dispositivo INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    endereco NVARCHAR(500),
    endereco_ip NVARCHAR(50),
    porta INT,
    chave_api NVARCHAR(255),
    status NVARCHAR(50),
    ultima_verificacao DATETIME,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_int_dispositivos_id_portaria FOREIGN KEY (id_portaria) REFERENCES gtc_portarias(id),
    CONSTRAINT fk_int_dispositivos_id_tipo FOREIGN KEY (id_tipo_dispositivo) REFERENCES int_tipos_dispositivo(id),
    INDEX ix_int_dispositivos_status (status)
);

-- Tabela de Log de Comunicação com Dispositivos
CREATE TABLE int_log_dispositivos (
    id BIGINT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_dispositivo INT NOT NULL,
    tipo_operacao NVARCHAR(50),
    comando NVARCHAR(500),
    resposta NVARCHAR(MAX),
    status_http INT,
    tempo_resposta INT,
    data_log DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT fk_int_log_id_dispositivo FOREIGN KEY (id_dispositivo) REFERENCES int_dispositivos(id),
    INDEX ix_int_log_data (data_log),
    INDEX ix_int_log_id_dispositivo (id_dispositivo)
);

-- Tabela de Eventos de Dispositivos
CREATE TABLE int_eventos_dispositivos (
    id BIGINT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_dispositivo INT NOT NULL,
    id_acesso_portaria INT,
    tipo_evento NVARCHAR(100),
    dados NVARCHAR(MAX),
    data_evento DATETIME NOT NULL,
    data_registro DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT fk_int_eventos_id_dispositivo FOREIGN KEY (id_dispositivo) REFERENCES int_dispositivos(id),
    CONSTRAINT fk_int_eventos_id_acesso FOREIGN KEY (id_acesso_portaria) REFERENCES gtc_acessos_portaria(id),
    INDEX ix_int_eventos_data (data_evento),
    INDEX ix_int_eventos_id_dispositivo (id_dispositivo)
);

-- ============================================================================
-- 9. TABELAS DE MOTOR DE VALIDAÇÃO
-- ============================================================================

-- Tabela de Motivos de Bloqueio Padronizados
CREATE TABLE bas_motivos_bloqueio (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    codigo NVARCHAR(50) UNIQUE NOT NULL,
    descricao NVARCHAR(500) NOT NULL,
    categoria NVARCHAR(100),
    severidade NVARCHAR(50),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    INDEX ix_bas_motivos_codigo (codigo)
);

-- Tabela de Histórico de Validações de Acesso
CREATE TABLE bas_historico_validacoes (
    id BIGINT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_acesso_portaria INT NOT NULL,
    id_motivo_bloqueio INT,
    validacao NVARCHAR(50),
    resultado NVARCHAR(50),
    detalhes NVARCHAR(MAX),
    data_validacao DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT fk_bas_hist_val_id_acesso FOREIGN KEY (id_acesso_portaria) REFERENCES gtc_acessos_portaria(id),
    CONSTRAINT fk_bas_hist_val_id_motivo FOREIGN KEY (id_motivo_bloqueio) REFERENCES bas_motivos_bloqueio(id),
    INDEX ix_bas_hist_val_id_acesso (id_acesso_portaria),
    INDEX ix_bas_hist_val_data (data_validacao)
);

-- ============================================================================
-- 10. ÍNDICES ADICIONAIS PARA PERFORMANCE
-- ============================================================================

CREATE INDEX ix_cap_colab_id_fornecedor_ativo ON cap_colaboradores_fornecedor(id_fornecedor, ativo);
CREATE INDEX ix_doc_documentos_id_colab_data_venc ON doc_documentos(id_colaborador_fornecedor, data_vencimento);
CREATE INDEX ix_tre_treinamentos_id_colab_data_venc ON tre_treinamentos(id_colaborador_fornecedor, data_vencimento);
CREATE INDEX ix_gtc_acessos_id_colab_data_entrada ON gtc_acessos_portaria(id_colaborador_fornecedor, data_entrada);

-- ============================================================================
-- 11. VIEWS PARA RELATÓRIOS GERENCIAIS
-- ============================================================================

-- View: Documentos Vencidos ou Próximos a Vencer (Fornecedores e Colaboradores)
CREATE VIEW vw_documentos_vencidos AS
SELECT 
    d.id,
    d.id_saas,
    c.nome AS nome_colaborador,
    c.cpf,
    f.razao_social AS fornecedor,
    td.nome AS tipo_documento,
    d.data_vencimento,
    DATEDIFF(DAY, GETDATE(), d.data_vencimento) AS dias_para_vencer,
    CASE 
        WHEN d.data_vencimento < GETDATE() THEN 'Vencido'
        WHEN DATEDIFF(DAY, GETDATE(), d.data_vencimento) <= 30 THEN 'Próximo a Vencer'
        ELSE 'Vigente'
    END AS status
FROM doc_documentos d
LEFT JOIN cap_colaboradores_fornecedor c ON d.id_colaborador_fornecedor = c.id
LEFT JOIN cap_fornecedores f ON d.id_fornecedor = f.id
INNER JOIN doc_tipos_documento td ON d.id_tipo_documento = td.id
WHERE d.ativo = 1;

-- View: Treinamentos Vencidos ou Próximos a Vencer (Fornecedores)
CREATE VIEW vw_treinamentos_vencidos AS
SELECT 
    t.id,
    t.id_saas,
    c.nome AS nome_colaborador,
    c.cpf,
    f.razao_social AS fornecedor,
    tt.nome AS tipo_treinamento,
    t.data_vencimento,
    DATEDIFF(DAY, GETDATE(), t.data_vencimento) AS dias_para_vencer,
    CASE 
        WHEN t.data_vencimento < GETDATE() THEN 'Vencido'
        WHEN DATEDIFF(DAY, GETDATE(), t.data_vencimento) <= 30 THEN 'Próximo a Vencer'
        ELSE 'Vigente'
    END AS status
FROM tre_treinamentos t
INNER JOIN cap_colaboradores_fornecedor c ON t.id_colaborador_fornecedor = c.id
INNER JOIN cap_fornecedores f ON c.id_fornecedor = f.id
INNER JOIN tre_tipos_treinamento tt ON t.id_tipo_treinamento = tt.id
WHERE t.ativo = 1 AND t.resultado = 'Aprovado';

-- View: Conformidade por Colaborador de Fornecedor
CREATE VIEW vw_conformidade_colaborador_fornecedor AS
SELECT 
    c.id,
    c.id_saas,
    c.nome,
    c.cpf,
    f.razao_social AS fornecedor,
    COUNT(DISTINCT d.id) AS total_documentos,
    COUNT(DISTINCT CASE WHEN d.data_vencimento < GETDATE() THEN d.id END) AS documentos_vencidos,
    COUNT(DISTINCT t.id) AS total_treinamentos,
    COUNT(DISTINCT CASE WHEN t.data_vencimento < GETDATE() THEN t.id END) AS treinamentos_vencidos,
    COUNT(DISTINCT CASE WHEN bp.ativo = 1 THEN bp.id END) AS bloqueios_ativos
FROM cap_colaboradores_fornecedor c
INNER JOIN cap_fornecedores f ON c.id_fornecedor = f.id
LEFT JOIN doc_documentos d ON c.id = d.id_colaborador_fornecedor AND d.ativo = 1
LEFT JOIN tre_treinamentos t ON c.id = t.id_colaborador_fornecedor AND t.ativo = 1 AND t.resultado = 'Aprovado'
LEFT JOIN cap_bloqueios_pessoa bp ON c.id = bp.id_colaborador_fornecedor
WHERE c.ativo = 1
GROUP BY c.id, c.id_saas, c.nome, c.cpf, f.razao_social;

-- View: Acessos Não Autorizados
CREATE VIEW vw_acessos_nao_autorizados AS
SELECT 
    ap.id,
    ap.id_saas,
    ap.numero_protocolo,
    COALESCE(c.nome, v.nome) AS pessoa,
    COALESCE(c.cpf, v.cpf) AS cpf,
    ar.nome AS area,
    ap.data_entrada,
    ap.motivo_bloqueio,
    ap.usuario_criacao
FROM gtc_acessos_portaria ap
LEFT JOIN cap_colaboradores_fornecedor c ON ap.id_colaborador_fornecedor = c.id
LEFT JOIN cap_visitantes v ON ap.id_visitante = v.id
LEFT JOIN gtc_areas ar ON ap.id_area = ar.id
WHERE ap.autorizado = 0
ORDER BY ap.data_entrada DESC;

-- View: Contratos Vencidos
CREATE VIEW vw_contratos_vencidos AS
SELECT 
    ct.id,
    ct.id_saas,
    f.razao_social,
    ct.numero_contrato,
    ct.data_fim,
    DATEDIFF(DAY, GETDATE(), ct.data_fim) AS dias_para_vencer,
    CASE 
        WHEN ct.data_fim < GETDATE() THEN 'Vencido'
        WHEN DATEDIFF(DAY, GETDATE(), ct.data_fim) <= 30 THEN 'Próximo a Vencer'
        ELSE 'Vigente'
    END AS status
FROM cap_contratos_fornecedor ct
INNER JOIN cap_fornecedores f ON ct.id_fornecedor = f.id
WHERE ct.ativo = 1;

-- View: Pessoas Bloqueadas
CREATE VIEW vw_pessoas_bloqueadas AS
SELECT 
    bp.id,
    bp.id_saas,
    COALESCE(c.nome, v.nome) AS pessoa,
    COALESCE(c.cpf, v.cpf) AS cpf,
    bp.motivo,
    bp.data_bloqueio,
    bp.data_desbloqueio,
    bp.usuario_bloqueio
FROM cap_bloqueios_pessoa bp
LEFT JOIN cap_colaboradores_fornecedor c ON bp.id_colaborador_fornecedor = c.id
LEFT JOIN cap_visitantes v ON bp.id_visitante = v.id
WHERE bp.ativo = 1
ORDER BY bp.data_bloqueio DESC;

-- ============================================================================
-- 12. DADOS INICIAIS DE CONFIGURAÇÃO
-- ============================================================================

INSERT INTO bas_configuracoes (id_saas, chave, valor, descricao, tipo_dado) VALUES
(1, 'ControlGranularAcesso', 'true', 'Ativar controle granular de acesso por áreas', 'boolean'),
(1, 'ExigirVisitanteResponsavel', 'true', 'Visitante deve ter um funcionário responsável', 'boolean'),
(1, 'ExigirChecklist', 'true', 'Obrigar preenchimento de checklist antes de acesso', 'boolean'),
(1, 'DiasPreviaAvisoDocumentoVencido', '30', 'Dias antes do vencimento para avisar', 'int'),
(1, 'DiasPreviaAvisoTreinamentoVencido', '30', 'Dias antes do vencimento de treinamento para avisar', 'int'),
(1, 'DiasPreviaAvisoContratoVencido', '30', 'Dias antes do vencimento de contrato para avisar', 'int'),
(1, 'ExigirEPI', 'true', 'Validar EPI obrigatório antes de acesso', 'boolean'),
(1, 'RegistrarOcorrencias', 'true', 'Registrar ocorrências de desvios', 'boolean');

-- ============================================================================
-- FIM DO SCRIPT
-- ============================================================================
-- ============================================================================
-- SGT v3.0 - ADIÇÕES AO MODELO
-- Tabelas: ASOs/Exames, Escala de Porteiros, Contatos de Emergência, Notificações
-- ============================================================================
-- Data: Dezembro de 2025
-- ============================================================================

-- ============================================================================
-- 1. MÓDULO MSO - EXAMES OCUPACIONAIS (ASOs e outros)
-- ============================================================================

-- Tabela de Tipos de Exame (configurável)
-- Permite cadastrar: ASO Admissional, Periódico, Demissional, Retorno ao Trabalho,
-- Mudança de Função, Audiometria, Espirometria, Acuidade Visual, etc.
CREATE TABLE mso_tipos_exame (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    codigo NVARCHAR(20) NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    categoria NVARCHAR(50),                    -- 'ASO', 'Complementar', 'Laboratorial', 'Imagem'
    dias_prazo_validade INT,                   -- NULL = sem validade
    dias_antecedencia_aviso INT DEFAULT 30,    -- Dias antes para alertar vencimento
    obrigatorio BIT DEFAULT 0,
    aplicavel_a NVARCHAR(100),                 -- 'Todos', 'Proprios', 'Terceirizados'
    requer_aptidao BIT DEFAULT 1,              -- Se exige resultado Apto/Inapto
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    UNIQUE (id_saas, codigo),
    INDEX ix_mso_tipos_exame_nome (nome),
    INDEX ix_mso_tipos_exame_categoria (categoria)
);

-- Tabela de Exames Realizados (ASOs e complementares)
CREATE TABLE mso_exames (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,                 -- FK flexível para func1
    id_colaborador_fornecedor INT,
    id_tipo_exame INT NOT NULL,
    
    -- Dados do Exame
    data_realizacao DATE NOT NULL,
    data_vencimento DATE,
    
    -- Resultado
    resultado NVARCHAR(50),                    -- 'Apto', 'Inapto', 'Apto com Restrições', 'Pendente'
    restricoes NVARCHAR(500),                  -- Detalhamento se "Apto com Restrições"
    observacoes_clinicas NVARCHAR(MAX),
    
    -- Dados do Médico/Clínica
    medico_responsavel NVARCHAR(255),
    crm_medico NVARCHAR(20),
    clinica NVARCHAR(255),
    
    -- Tipo específico de ASO (se categoria = 'ASO')
    tipo_aso NVARCHAR(50),                     -- 'Admissional', 'Periodico', 'Demissional', 'RetornoTrabalho', 'MudancaFuncao'
    
    -- Arquivo
    caminho_arquivo NVARCHAR(500),
    
    -- Controle
    status_verificacao NVARCHAR(50),           -- 'Pendente', 'Verificado', 'Rejeitado'
    usuario_verificacao NVARCHAR(100),
    data_verificacao DATETIME,
    motivo_rejeicao NVARCHAR(500),
    
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_exames_id_tipo FOREIGN KEY (id_tipo_exame) REFERENCES mso_tipos_exame(id),
    CONSTRAINT fk_mso_exames_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    INDEX ix_mso_exames_data_vencimento (data_vencimento),
    INDEX ix_mso_exames_resultado (resultado),
    INDEX ix_mso_exames_id_colab (id_colaborador_fornecedor),
    INDEX ix_mso_exames_tipo_aso (tipo_aso)
);

-- Tabela de Regras de Exames por Área (quais exames são obrigatórios por área)
CREATE TABLE mso_regras_exame_area (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_area INT NOT NULL,
    id_tipo_exame INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_regras_exame_area_id_area FOREIGN KEY (id_area) REFERENCES gtc_areas(id),
    CONSTRAINT fk_mso_regras_exame_area_id_tipo FOREIGN KEY (id_tipo_exame) REFERENCES mso_tipos_exame(id),
    UNIQUE (id_area, id_tipo_exame)
);

-- Tabela de Regras de Exames por Cargo (para funcionários legados)
CREATE TABLE mso_regras_exame_cargo (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_cargo_legado INT NOT NULL,              -- FK flexível para tabela de cargos legada
    id_tipo_exame INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    periodicidade_meses INT,                   -- Periodicidade específica para este cargo
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_regras_exame_cargo_id_tipo FOREIGN KEY (id_tipo_exame) REFERENCES mso_tipos_exame(id),
    UNIQUE (id_cargo_legado, id_tipo_exame)
);

-- Tabela de Regras de Exames por Contrato de Fornecedor
CREATE TABLE mso_regras_exame_contrato (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_contrato INT NOT NULL,
    id_tipo_exame INT NOT NULL,
    obrigatorio BIT DEFAULT 1,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    CONSTRAINT fk_mso_regras_exame_contrato_id_contrato FOREIGN KEY (id_contrato) REFERENCES cap_contratos_fornecedor(id),
    CONSTRAINT fk_mso_regras_exame_contrato_id_tipo FOREIGN KEY (id_tipo_exame) REFERENCES mso_tipos_exame(id),
    UNIQUE (id_contrato, id_tipo_exame)
);

-- ============================================================================
-- 2. MÓDULO GTC - ESCALA DE PORTEIROS
-- ============================================================================

-- Tabela de Turnos
CREATE TABLE gtc_turnos (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    codigo NVARCHAR(20) NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    hora_inicio TIME NOT NULL,
    hora_fim TIME NOT NULL,
    cruza_meia_noite BIT DEFAULT 0,            -- Se o turno passa da meia-noite
    carga_horaria_minutos INT,                 -- Duração em minutos
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    UNIQUE (id_saas, codigo),
    INDEX ix_gtc_turnos_nome (nome)
);

-- Tabela de Equipes de Portaria
CREATE TABLE gtc_equipes_portaria (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_portaria INT NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    cor_identificacao NVARCHAR(20),            -- Para visualização em calendário
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_equipes_id_portaria FOREIGN KEY (id_portaria) REFERENCES gtc_portarias(id),
    INDEX ix_gtc_equipes_nome (nome)
);

-- Tabela de Membros da Equipe
CREATE TABLE gtc_membros_equipe (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_equipe INT NOT NULL,
    id_funcionario_legado INT,                 -- FK flexível para func1
    funcao NVARCHAR(100),                      -- 'Líder', 'Porteiro', 'Vigilante', etc.
    data_inicio DATE NOT NULL,
    data_fim DATE,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_membros_id_equipe FOREIGN KEY (id_equipe) REFERENCES gtc_equipes_portaria(id),
    INDEX ix_gtc_membros_id_equipe (id_equipe)
);

-- Tabela de Escala de Trabalho
CREATE TABLE gtc_escalas_portaria (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_portaria INT NOT NULL,
    id_equipe INT,
    id_turno INT NOT NULL,
    id_funcionario_legado INT,                 -- FK flexível - pode ser individual ou por equipe
    
    -- Período da escala
    data_escala DATE NOT NULL,
    
    -- Status
    status NVARCHAR(50),                       -- 'Programado', 'Confirmado', 'EmAndamento', 'Concluido', 'Falta', 'Troca'
    
    -- Horários reais (para controle de ponto)
    hora_entrada_real TIME,
    hora_saida_real TIME,
    
    -- Observações
    observacoes NVARCHAR(500),
    
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_escalas_id_portaria FOREIGN KEY (id_portaria) REFERENCES gtc_portarias(id),
    CONSTRAINT fk_gtc_escalas_id_equipe FOREIGN KEY (id_equipe) REFERENCES gtc_equipes_portaria(id),
    CONSTRAINT fk_gtc_escalas_id_turno FOREIGN KEY (id_turno) REFERENCES gtc_turnos(id),
    INDEX ix_gtc_escalas_data (data_escala),
    INDEX ix_gtc_escalas_status (status),
    INDEX ix_gtc_escalas_id_portaria_data (id_portaria, data_escala)
);

-- Tabela de Trocas de Escala
CREATE TABLE gtc_trocas_escala (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_escala_original INT NOT NULL,
    id_funcionario_substituto INT,             -- FK flexível para func1
    motivo NVARCHAR(500) NOT NULL,
    status NVARCHAR(50),                       -- 'Solicitada', 'Aprovada', 'Rejeitada', 'Cancelada'
    usuario_aprovador NVARCHAR(100),
    data_aprovacao DATETIME,
    observacoes NVARCHAR(500),
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_gtc_trocas_id_escala FOREIGN KEY (id_escala_original) REFERENCES gtc_escalas_portaria(id),
    INDEX ix_gtc_trocas_status (status)
);

-- ============================================================================
-- 3. MÓDULO CAP - CONTATOS DE EMERGÊNCIA
-- ============================================================================

-- Tabela de Contatos de Emergência
CREATE TABLE cap_contatos_emergencia (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_funcionario_legado INT,                 -- FK flexível para func1
    id_colaborador_fornecedor INT,
    
    -- Dados do Contato
    nome NVARCHAR(255) NOT NULL,
    id_parentesco INT,                         -- FK para bas_parentescos
    telefone_principal NVARCHAR(20) NOT NULL,
    telefone_secundario NVARCHAR(20),
    email NVARCHAR(100),
    
    -- Endereço (opcional)
    endereco NVARCHAR(500),
    cidade NVARCHAR(100),
    id_uf INT,
    cep NVARCHAR(10),
    
    -- Prioridade
    ordem_prioridade INT DEFAULT 1,            -- 1 = primeiro a ser contatado
    
    -- Observações
    observacoes NVARCHAR(500),
    
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    data_atualizacao DATETIME DEFAULT GETDATE(),
    usuario_atualizacao NVARCHAR(100),
    
    CONSTRAINT fk_cap_contatos_emerg_id_colab FOREIGN KEY (id_colaborador_fornecedor) REFERENCES cap_colaboradores_fornecedor(id),
    CONSTRAINT fk_cap_contatos_emerg_id_parentesco FOREIGN KEY (id_parentesco) REFERENCES bas_parentescos(id),
    CONSTRAINT fk_cap_contatos_emerg_id_uf FOREIGN KEY (id_uf) REFERENCES bas_ufs(id),
    INDEX ix_cap_contatos_emerg_id_colab (id_colaborador_fornecedor),
    INDEX ix_cap_contatos_emerg_ordem (ordem_prioridade)
);

-- ============================================================================
-- 4. MÓDULO BAS - SISTEMA DE NOTIFICAÇÕES (Opcional)
-- ============================================================================

-- Tabela de Tipos de Notificação
CREATE TABLE bas_tipos_notificacao (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    codigo NVARCHAR(50) NOT NULL,
    nome NVARCHAR(100) NOT NULL,
    descricao NVARCHAR(500),
    template_titulo NVARCHAR(255),             -- Template do título (suporta variáveis)
    template_mensagem NVARCHAR(MAX),           -- Template da mensagem (suporta variáveis)
    categoria NVARCHAR(50),                    -- 'Vencimento', 'Bloqueio', 'Ocorrencia', 'Sistema'
    severidade NVARCHAR(20),                   -- 'Info', 'Aviso', 'Alerta', 'Critico'
    envia_email BIT DEFAULT 0,
    envia_push BIT DEFAULT 0,
    ativo BIT DEFAULT 1,
    data_criacao DATETIME DEFAULT GETDATE(),
    usuario_criacao NVARCHAR(100),
    
    UNIQUE (id_saas, codigo),
    INDEX ix_bas_tipos_notificacao_categoria (categoria)
);

-- Tabela de Notificações Geradas
CREATE TABLE bas_notificacoes (
    id BIGINT PRIMARY KEY IDENTITY(1,1),
    id_saas INT NOT NULL,
    id_tipo_notificacao INT NOT NULL,
    
    -- Destinatário
    id_usuario_destino INT,                    -- FK flexível para tuse1 ou usuário do sistema
    email_destino NVARCHAR(100),
    
    -- Conteúdo
    titulo NVARCHAR(255) NOT NULL,
    mensagem NVARCHAR(MAX) NOT NULL,
    
    -- Referência (qual registro gerou a notificação)
    tabela_referencia NVARCHAR(100),           -- Ex: 'mso_exames', 'doc_documentos'
    id_referencia INT,                         -- ID do registro que gerou
    
    -- Status
    status NVARCHAR(50),                       -- 'Pendente', 'Enviada', 'Lida', 'Arquivada'
    data_leitura DATETIME,
    data_envio_email DATETIME,
    
    -- Agendamento
    data_agendada DATETIME,                    -- Quando deve ser enviada
    
    data_criacao DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT fk_bas_notificacoes_id_tipo FOREIGN KEY (id_tipo_notificacao) REFERENCES bas_tipos_notificacao(id),
    INDEX ix_bas_notificacoes_status (status),
    INDEX ix_bas_notificacoes_data_criacao (data_criacao),
    INDEX ix_bas_notificacoes_id_usuario (id_usuario_destino),
    INDEX ix_bas_notificacoes_referencia (tabela_referencia, id_referencia)
);

-- ============================================================================
-- 5. VIEWS ADICIONAIS
-- ============================================================================

-- View: Exames/ASOs Vencidos ou Próximos a Vencer
CREATE VIEW vw_exames_vencidos AS
SELECT 
    e.id,
    e.id_saas,
    COALESCE(c.nome, 'Funcionário Interno') AS nome_pessoa,
    COALESCE(c.cpf, '') AS cpf,
    f.razao_social AS fornecedor,
    te.nome AS tipo_exame,
    te.categoria,
    e.tipo_aso,
    e.resultado,
    e.data_realizacao,
    e.data_vencimento,
    DATEDIFF(DAY, GETDATE(), e.data_vencimento) AS dias_para_vencer,
    CASE 
        WHEN e.data_vencimento < GETDATE() THEN 'Vencido'
        WHEN DATEDIFF(DAY, GETDATE(), e.data_vencimento) <= 30 THEN 'Próximo a Vencer'
        ELSE 'Vigente'
    END AS status_vencimento,
    CASE 
        WHEN e.id_funcionario_legado IS NOT NULL THEN 'Proprio'
        ELSE 'Terceirizado'
    END AS tipo_pessoa
FROM mso_exames e
LEFT JOIN cap_colaboradores_fornecedor c ON e.id_colaborador_fornecedor = c.id
LEFT JOIN cap_fornecedores f ON c.id_fornecedor = f.id
INNER JOIN mso_tipos_exame te ON e.id_tipo_exame = te.id
WHERE e.ativo = 1 
  AND e.resultado IN ('Apto', 'Apto com Restrições');

-- View: ASOs especificamente (filtro por categoria)
CREATE VIEW vw_asos_vencidos AS
SELECT * FROM vw_exames_vencidos
WHERE categoria = 'ASO';

-- View: Escala do Dia
CREATE VIEW vw_escala_dia AS
SELECT 
    ep.id,
    ep.id_saas,
    p.nome AS portaria,
    t.nome AS turno,
    t.hora_inicio,
    t.hora_fim,
    eq.nome AS equipe,
    ep.data_escala,
    ep.status,
    ep.hora_entrada_real,
    ep.hora_saida_real,
    ep.id_funcionario_legado,
    ep.observacoes
FROM gtc_escalas_portaria ep
INNER JOIN gtc_portarias p ON ep.id_portaria = p.id
INNER JOIN gtc_turnos t ON ep.id_turno = t.id
LEFT JOIN gtc_equipes_portaria eq ON ep.id_equipe = eq.id
WHERE ep.ativo = 1;

-- View: Conformidade Atualizada (incluindo exames)
CREATE VIEW vw_conformidade_completa_colaborador AS
SELECT 
    c.id,
    c.id_saas,
    c.nome,
    c.cpf,
    f.razao_social AS fornecedor,
    -- Documentos
    COUNT(DISTINCT d.id) AS total_documentos,
    COUNT(DISTINCT CASE WHEN d.data_vencimento < GETDATE() THEN d.id END) AS documentos_vencidos,
    -- Treinamentos
    COUNT(DISTINCT t.id) AS total_treinamentos,
    COUNT(DISTINCT CASE WHEN t.data_vencimento < GETDATE() THEN t.id END) AS treinamentos_vencidos,
    -- Exames/ASOs
    COUNT(DISTINCT e.id) AS total_exames,
    COUNT(DISTINCT CASE WHEN e.data_vencimento < GETDATE() THEN e.id END) AS exames_vencidos,
    -- ASOs especificamente
    COUNT(DISTINCT CASE WHEN te.categoria = 'ASO' THEN e.id END) AS total_asos,
    COUNT(DISTINCT CASE WHEN te.categoria = 'ASO' AND e.data_vencimento < GETDATE() THEN e.id END) AS asos_vencidos,
    -- Bloqueios
    COUNT(DISTINCT CASE WHEN bp.ativo = 1 THEN bp.id END) AS bloqueios_ativos,
    -- Status Geral
    CASE 
        WHEN COUNT(DISTINCT CASE WHEN bp.ativo = 1 THEN bp.id END) > 0 THEN 'Bloqueado'
        WHEN COUNT(DISTINCT CASE WHEN d.data_vencimento < GETDATE() THEN d.id END) > 0 
          OR COUNT(DISTINCT CASE WHEN t.data_vencimento < GETDATE() THEN t.id END) > 0
          OR COUNT(DISTINCT CASE WHEN e.data_vencimento < GETDATE() THEN e.id END) > 0 THEN 'Irregular'
        ELSE 'Regular'
    END AS status_conformidade
FROM cap_colaboradores_fornecedor c
INNER JOIN cap_fornecedores f ON c.id_fornecedor = f.id
LEFT JOIN doc_documentos d ON c.id = d.id_colaborador_fornecedor AND d.ativo = 1
LEFT JOIN tre_treinamentos t ON c.id = t.id_colaborador_fornecedor AND t.ativo = 1 AND t.resultado = 'Aprovado'
LEFT JOIN mso_exames e ON c.id = e.id_colaborador_fornecedor AND e.ativo = 1 AND e.resultado IN ('Apto', 'Apto com Restrições')
LEFT JOIN mso_tipos_exame te ON e.id_tipo_exame = te.id
LEFT JOIN cap_bloqueios_pessoa bp ON c.id = bp.id_colaborador_fornecedor
WHERE c.ativo = 1
GROUP BY c.id, c.id_saas, c.nome, c.cpf, f.razao_social;

-- ============================================================================
-- 6. DADOS INICIAIS
-- ============================================================================

-- Tipos de Exame Padrão
INSERT INTO mso_tipos_exame (id_saas, codigo, nome, descricao, categoria, dias_prazo_validade, dias_antecedencia_aviso, obrigatorio, aplicavel_a, requer_aptidao) VALUES
(1, 'ASO-ADM', 'ASO Admissional', 'Exame admissional obrigatório', 'ASO', NULL, 30, 1, 'Todos', 1),
(1, 'ASO-PER', 'ASO Periódico', 'Exame periódico anual', 'ASO', 365, 30, 1, 'Todos', 1),
(1, 'ASO-DEM', 'ASO Demissional', 'Exame demissional obrigatório', 'ASO', NULL, 30, 1, 'Todos', 1),
(1, 'ASO-RET', 'ASO Retorno ao Trabalho', 'Exame de retorno após afastamento > 30 dias', 'ASO', NULL, 30, 0, 'Todos', 1),
(1, 'ASO-MUD', 'ASO Mudança de Função', 'Exame para mudança de função/risco', 'ASO', NULL, 30, 0, 'Todos', 1),
(1, 'AUDIO', 'Audiometria', 'Exame audiométrico ocupacional', 'Complementar', 365, 30, 0, 'Todos', 0),
(1, 'ESPIRO', 'Espirometria', 'Exame de função pulmonar', 'Complementar', 365, 30, 0, 'Todos', 0),
(1, 'ACUIDADE', 'Acuidade Visual', 'Teste de acuidade visual', 'Complementar', 365, 30, 0, 'Todos', 0),
(1, 'ECG', 'Eletrocardiograma', 'ECG de repouso', 'Complementar', 365, 30, 0, 'Todos', 0),
(1, 'HEMOGRAMA', 'Hemograma Completo', 'Exame laboratorial de sangue', 'Laboratorial', 180, 30, 0, 'Todos', 0),
(1, 'GLICEMIA', 'Glicemia de Jejum', 'Exame de glicose em jejum', 'Laboratorial', 180, 30, 0, 'Todos', 0),
(1, 'TOXICOL', 'Toxicológico', 'Exame toxicológico (motoristas)', 'Laboratorial', 900, 60, 0, 'Todos', 0),
(1, 'RX-TORAX', 'Raio-X de Tórax', 'Radiografia de tórax PA', 'Imagem', 365, 30, 0, 'Todos', 0);

-- Turnos Padrão
INSERT INTO gtc_turnos (id_saas, codigo, nome, descricao, hora_inicio, hora_fim, cruza_meia_noite, carga_horaria_minutos) VALUES
(1, 'TURNO-A', 'Turno A - Manhã', 'Turno da manhã', '06:00', '14:00', 0, 480),
(1, 'TURNO-B', 'Turno B - Tarde', 'Turno da tarde', '14:00', '22:00', 0, 480),
(1, 'TURNO-C', 'Turno C - Noite', 'Turno da noite', '22:00', '06:00', 1, 480),
(1, 'TURNO-ADM', 'Turno Administrativo', 'Horário comercial', '08:00', '18:00', 0, 600),
(1, '12X36-D', '12x36 Diurno', 'Escala 12x36 diurno', '07:00', '19:00', 0, 720),
(1, '12X36-N', '12x36 Noturno', 'Escala 12x36 noturno', '19:00', '07:00', 1, 720);

-- Tipos de Notificação Padrão
INSERT INTO bas_tipos_notificacao (id_saas, codigo, nome, descricao, template_titulo, template_mensagem, categoria, severidade, envia_email) VALUES
(1, 'DOC-VENC-30', 'Documento Vencendo 30 dias', 'Alerta de documento próximo ao vencimento', 'Documento {{tipo_documento}} vencendo em 30 dias', 'O documento {{tipo_documento}} do colaborador {{nome}} vence em {{data_vencimento}}.', 'Vencimento', 'Aviso', 1),
(1, 'DOC-VENCIDO', 'Documento Vencido', 'Alerta de documento vencido', 'Documento {{tipo_documento}} VENCIDO', 'O documento {{tipo_documento}} do colaborador {{nome}} está VENCIDO desde {{data_vencimento}}.', 'Vencimento', 'Critico', 1),
(1, 'TRE-VENC-30', 'Treinamento Vencendo 30 dias', 'Alerta de treinamento próximo ao vencimento', 'Treinamento {{tipo_treinamento}} vencendo em 30 dias', 'O treinamento {{tipo_treinamento}} do colaborador {{nome}} vence em {{data_vencimento}}.', 'Vencimento', 'Aviso', 1),
(1, 'TRE-VENCIDO', 'Treinamento Vencido', 'Alerta de treinamento vencido', 'Treinamento {{tipo_treinamento}} VENCIDO', 'O treinamento {{tipo_treinamento}} do colaborador {{nome}} está VENCIDO desde {{data_vencimento}}.', 'Vencimento', 'Critico', 1),
(1, 'ASO-VENC-30', 'ASO Vencendo 30 dias', 'Alerta de ASO próximo ao vencimento', 'ASO de {{nome}} vencendo em 30 dias', 'O ASO Periódico do colaborador {{nome}} vence em {{data_vencimento}}.', 'Vencimento', 'Alerta', 1),
(1, 'ASO-VENCIDO', 'ASO Vencido', 'Alerta de ASO vencido', 'ASO de {{nome}} VENCIDO', 'O ASO do colaborador {{nome}} está VENCIDO desde {{data_vencimento}}. Acesso bloqueado.', 'Vencimento', 'Critico', 1),
(1, 'CONTRATO-VENC', 'Contrato Vencendo', 'Alerta de contrato próximo ao vencimento', 'Contrato {{numero_contrato}} vencendo', 'O contrato {{numero_contrato}} do fornecedor {{fornecedor}} vence em {{data_vencimento}}.', 'Vencimento', 'Alerta', 1),
(1, 'BLOQUEIO', 'Pessoa Bloqueada', 'Notificação de bloqueio de pessoa', 'Colaborador {{nome}} foi BLOQUEADO', 'O colaborador {{nome}} foi bloqueado. Motivo: {{motivo}}.', 'Bloqueio', 'Alerta', 1),
(1, 'OCORRENCIA', 'Nova Ocorrência', 'Registro de nova ocorrência', 'Nova ocorrência registrada', 'Uma nova ocorrência do tipo {{tipo_ocorrencia}} foi registrada em {{data_ocorrencia}}.', 'Ocorrencia', 'Info', 0);

-- ============================================================================
-- 7. ÍNDICES ADICIONAIS PARA PERFORMANCE
-- ============================================================================

CREATE INDEX ix_mso_exames_id_colab_data_venc ON mso_exames(id_colaborador_fornecedor, data_vencimento);
CREATE INDEX ix_mso_exames_funcionario_legado ON mso_exames(id_funcionario_legado) WHERE id_funcionario_legado IS NOT NULL;
CREATE INDEX ix_gtc_escalas_funcionario_data ON gtc_escalas_portaria(id_funcionario_legado, data_escala);
CREATE INDEX ix_cap_contatos_emerg_funcionario ON cap_contatos_emergencia(id_funcionario_legado) WHERE id_funcionario_legado IS NOT NULL;
CREATE INDEX ix_bas_notificacoes_pendentes ON bas_notificacoes(status, data_agendada) WHERE status = 'Pendente';

-- ============================================================================
-- FIM DO SCRIPT DE ADIÇÕES
-- ============================================================================
