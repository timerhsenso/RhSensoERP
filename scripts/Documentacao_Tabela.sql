USE [bd_rhu_copenor];
GO

/* =============================================================================
   ███████╗ ██████╗  ██████╗    ██████╗  ██████╗  ██████╗
   ██╔════╝██╔════╝ ██╔════╝    ██╔══██╗██╔═══██╗██╔════╝
   ███████╗██║  ███╗██║         ██║  ██║██║   ██║██║     
   ╚════██║██║   ██║██║         ██║  ██║██║   ██║██║     
   ███████║╚██████╔╝╚██████╗    ██████╔╝╚██████╔╝╚██████╗
   ╚══════╝ ╚═════╝  ╚═════╝    ╚═════╝  ╚═════╝  ╚═════╝
   
   DOCUMENTAÇÃO DO BANCO DE DADOS - EXTENDED PROPERTIES
   =====================================================
   
   Sistema: SGC - Sistema de Gestão Corporativa
   Versão: 3.0
   Data: Dezembro/2025
   
   Este script adiciona Extended Properties (MS_Description) em todas as
   tabelas e colunas do módulo SGC para documentação no SQL Server.
   
   Para consultar a documentação:
   
   SELECT 
       t.name AS Tabela,
       c.name AS Coluna,
       CAST(ep.value AS VARCHAR(500)) AS Descricao
   FROM sys.tables t
   LEFT JOIN sys.columns c ON t.object_id = c.object_id
   LEFT JOIN sys.extended_properties ep 
       ON ep.major_id = t.object_id 
       AND ep.minor_id = ISNULL(c.column_id, 0)
       AND ep.name = 'MS_Description'
   WHERE t.name LIKE 'SGC_%'
   ORDER BY t.name, c.column_id;
   
============================================================================= */

SET NOCOUNT ON;
PRINT '=============================================================================';
PRINT 'SGC - DOCUMENTAÇÃO (EXTENDED PROPERTIES)';
PRINT 'Início: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '=============================================================================';
PRINT '';

-- #############################################################################
-- HELPER: Procedure para adicionar Extended Property de forma segura
-- #############################################################################

IF OBJECT_ID('tempdb..#AddDoc') IS NOT NULL DROP PROCEDURE #AddDoc;
GO

CREATE PROCEDURE #AddDoc
    @TableName  SYSNAME,
    @ColumnName SYSNAME = NULL,
    @Description NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verifica se a tabela existe
    IF OBJECT_ID('dbo.' + @TableName, 'U') IS NULL
    BEGIN
        PRINT '  [!!] Tabela não encontrada: ' + @TableName;
        RETURN;
    END
    
    IF @ColumnName IS NULL
    BEGIN
        -- Documentação da TABELA
        IF EXISTS (SELECT 1 FROM sys.extended_properties ep
                   WHERE ep.major_id = OBJECT_ID('dbo.' + @TableName)
                     AND ep.minor_id = 0
                     AND ep.name = 'MS_Description')
        BEGIN
            -- Atualiza se já existe
            EXEC sys.sp_updateextendedproperty
                @name  = N'MS_Description',
                @value = @Description,
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE',  @level1name = @TableName;
        END
        ELSE
        BEGIN
            -- Adiciona se não existe
            EXEC sys.sp_addextendedproperty
                @name  = N'MS_Description',
                @value = @Description,
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE',  @level1name = @TableName;
        END
    END
    ELSE
    BEGIN
        -- Verifica se a coluna existe
        IF NOT EXISTS (SELECT 1 FROM sys.columns 
                       WHERE object_id = OBJECT_ID('dbo.' + @TableName) 
                         AND name = @ColumnName)
        BEGIN
            PRINT '  [!!] Coluna não encontrada: ' + @TableName + '.' + @ColumnName;
            RETURN;
        END
        
        -- Documentação da COLUNA
        IF EXISTS (SELECT 1 FROM sys.extended_properties ep
                   INNER JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
                   WHERE c.object_id = OBJECT_ID('dbo.' + @TableName)
                     AND c.name = @ColumnName
                     AND ep.name = 'MS_Description')
        BEGIN
            -- Atualiza se já existe
            EXEC sys.sp_updateextendedproperty
                @name  = N'MS_Description',
                @value = @Description,
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE',  @level1name = @TableName,
                @level2type = N'COLUMN', @level2name = @ColumnName;
        END
        ELSE
        BEGIN
            -- Adiciona se não existe
            EXEC sys.sp_addextendedproperty
                @name  = N'MS_Description',
                @value = @Description,
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE',  @level1name = @TableName,
                @level2type = N'COLUMN', @level2name = @ColumnName;
        END
    END
END
GO


-- #############################################################################
-- PARTE 1: TABELAS AUXILIARES (LOOKUPS)
-- #############################################################################

PRINT '>>> PARTE 1: TABELAS AUXILIARES (LOOKUPS)';
PRINT '';

-- =============================================================================
-- SGC_TipoFornecedor
-- =============================================================================
PRINT '  Documentando SGC_TipoFornecedor...';

EXEC #AddDoc 'SGC_TipoFornecedor', NULL, 
    N'Tabela auxiliar (lookup) com classificação de empresas fornecedoras por tipo de serviço prestado. Utilizada para categorização em relatórios e filtros de busca.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Id', 
    N'Identificador único do tipo de fornecedor. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'IdSaas', 
    N'Identificador do tenant para ambiente multi-empresa (SaaS). NULL indica dado global compartilhado entre todos os tenants.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Codigo', 
    N'Código único alfanumérico do tipo (ex: MANUTENCAO, LIMPEZA, TI). Usado em integrações e referências no código-fonte. Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Descricao', 
    N'Descrição legível do tipo de fornecedor para exibição em telas e relatórios. Máx. 100 caracteres.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Icone', 
    N'Classe CSS ou nome do ícone para exibição na interface (ex: fa-wrench, bi-building). Compatível com FontAwesome/Bootstrap Icons.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'CorHex', 
    N'Código hexadecimal da cor para identificação visual em dashboards e badges (ex: #28A745 para verde). Formato: #RRGGBB.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Ordem', 
    N'Ordem de exibição em listas, dropdowns e relatórios. Valores menores aparecem primeiro. Default: 0.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Ativo', 
    N'Indica se o registro está ativo (1) ou inativo/excluído logicamente (0). Registros inativos não aparecem em seleções.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Aud_CreatedAt', 
    N'Data e hora UTC de criação do registro. Preenchido automaticamente pelo sistema. Não deve ser alterado.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Aud_UpdatedAt', 
    N'Data e hora UTC da última atualização do registro. NULL se nunca foi alterado após criação.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Aud_IdUsuarioCadastro', 
    N'ID do usuário (FK → tuse1.id) que criou o registro. NULL para registros de seed/migração.';

EXEC #AddDoc 'SGC_TipoFornecedor', 'Aud_IdUsuarioAtualizacao', 
    N'ID do usuário (FK → tuse1.id) que realizou a última atualização. NULL se nunca foi alterado.';
GO


-- =============================================================================
-- SGC_TipoVeiculo
-- =============================================================================
PRINT '  Documentando SGC_TipoVeiculo...';

EXEC #AddDoc 'SGC_TipoVeiculo', NULL, 
    N'Tabela auxiliar com tipos de veículos e regras de negócio para controle de portaria. Define se o veículo exige checklist NR-20 (inflamáveis) ou pesagem obrigatória na balança.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Id', 
    N'Identificador único do tipo de veículo. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'IdSaas', 
    N'Identificador do tenant para ambiente multi-empresa (SaaS). NULL indica dado global.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Codigo', 
    N'Código único do tipo (ex: CARRO, MOTO, CAMINHAO_G, TANQUE, CARRETA). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Descricao', 
    N'Descrição do tipo de veículo para exibição (ex: Caminhão Grande 8t a 16t). Máx. 100 caracteres.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'ExigeChecklistNR20', 
    N'Indica se veículos deste tipo exigem preenchimento do checklist NR-20 (Inflamáveis e Combustíveis) antes de entrar na planta. Obrigatório para caminhões tanque.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'ExigePesagem', 
    N'Indica se veículos deste tipo devem obrigatoriamente passar pela balança na entrada e saída para controle de carga.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Icone', 
    N'Classe CSS do ícone para exibição (ex: fa-truck, fa-car). Opcional.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Ordem', 
    N'Ordem de exibição em listas e dropdowns.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Ativo', 
    N'Indica se o tipo está ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Aud_CreatedAt', 
    N'Data e hora UTC de criação do registro.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Aud_UpdatedAt', 
    N'Data e hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Aud_IdUsuarioCadastro', 
    N'ID do usuário que criou o registro (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoVeiculo', 'Aud_IdUsuarioAtualizacao', 
    N'ID do usuário que atualizou o registro (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TipoPessoa
-- =============================================================================
PRINT '  Documentando SGC_TipoPessoa...';

EXEC #AddDoc 'SGC_TipoPessoa', NULL, 
    N'Tabela auxiliar com tipos de pessoa para controle de acesso na portaria. Define regras como exigência de documento, foto, vínculo com empresa e permissão para veículo próprio.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Id', 
    N'Identificador único do tipo de pessoa. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoPessoa', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Codigo', 
    N'Código único do tipo (ex: VISITANTE, PRESTADOR, MOTORISTA, CANDIDATO, AUTORIDADE). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Descricao', 
    N'Descrição do tipo de pessoa para exibição em telas.';

EXEC #AddDoc 'SGC_TipoPessoa', 'ExigeDocumento', 
    N'Indica se é obrigatório apresentar documento de identificação (RG/CPF) no cadastro e acesso.';

EXEC #AddDoc 'SGC_TipoPessoa', 'ExigeFoto', 
    N'Indica se é obrigatório capturar foto no momento do cadastro. Recomendado para prestadores recorrentes.';

EXEC #AddDoc 'SGC_TipoPessoa', 'ExigeEmpresa', 
    N'Indica se é obrigatório informar empresa de origem/vínculo. Obrigatório para prestadores e motoristas.';

EXEC #AddDoc 'SGC_TipoPessoa', 'PermiteVeiculoProprio', 
    N'Indica se este tipo de pessoa pode entrar com veículo próprio na planta. Consultores geralmente podem, motoristas de carga não.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Icone', 
    N'Classe CSS do ícone (ex: fa-user, fa-id-card).';

EXEC #AddDoc 'SGC_TipoPessoa', 'Ordem', 
    N'Ordem de exibição em listas.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoPessoa', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoPessoa', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoPessoa', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TipoAso
-- =============================================================================
PRINT '  Documentando SGC_TipoAso...';

EXEC #AddDoc 'SGC_TipoAso', NULL, 
    N'Tabela auxiliar com tipos de Atestado de Saúde Ocupacional conforme NR-07. Define sigla e validade padrão para cada tipo de exame médico ocupacional.';

EXEC #AddDoc 'SGC_TipoAso', 'Id', 
    N'Identificador único do tipo de ASO. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoAso', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoAso', 'Codigo', 
    N'Código único do tipo (ex: ADMISSIONAL, PERIODICO, RETORNO, MUDANCA, DEMISSIONAL). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoAso', 'Descricao', 
    N'Descrição completa do tipo de ASO (ex: ASO Admissional, ASO Periódico).';

EXEC #AddDoc 'SGC_TipoAso', 'Sigla', 
    N'Sigla curta para exibição em listagens compactas (ex: ADM, PER, RET, MUD, DEM). Máx. 10 caracteres.';

EXEC #AddDoc 'SGC_TipoAso', 'ValidadeEmMesesPadrao', 
    N'Validade padrão em meses para este tipo de ASO. Ex: 12 meses para Admissional/Periódico. NULL para Demissional (não tem validade).';

EXEC #AddDoc 'SGC_TipoAso', 'Ordem', 
    N'Ordem de exibição.';

EXEC #AddDoc 'SGC_TipoAso', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoAso', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TipoAso', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoAso', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoAso', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_MotivoAcesso
-- =============================================================================
PRINT '  Documentando SGC_MotivoAcesso...';

EXEC #AddDoc 'SGC_MotivoAcesso', NULL, 
    N'Tabela auxiliar com motivos de acesso à empresa para controle de portaria. Define regras como exigência de destino, responsável interno ou nota fiscal.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Id', 
    N'Identificador único do motivo de acesso. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Codigo', 
    N'Código único do motivo (ex: ENTREGA, COLETA, SERVICO, REUNIAO, VISITA, AUDITORIA, EMERGENCIA). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Descricao', 
    N'Descrição do motivo para exibição (ex: Entrega de Mercadoria, Prestação de Serviço).';

EXEC #AddDoc 'SGC_MotivoAcesso', 'ExigeDestino', 
    N'Indica se é obrigatório informar o setor de destino dentro da planta. TRUE para maioria, FALSE para emergências.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'ExigeResponsavel', 
    N'Indica se é obrigatório informar um funcionário responsável/contato interno. Obrigatório para reuniões e visitas técnicas.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'ExigeNotaFiscal', 
    N'Indica se é obrigatório informar número de nota fiscal. Obrigatório para entregas e coletas de material.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Icone', 
    N'Classe CSS do ícone (ex: fa-box, fa-handshake).';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Ordem', 
    N'Ordem de exibição.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_MotivoAcesso', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_StatusAcesso
-- =============================================================================
PRINT '  Documentando SGC_StatusAcesso...';

EXEC #AddDoc 'SGC_StatusAcesso', NULL, 
    N'Tabela auxiliar com status do ciclo de vida de um registro de acesso. Controla o workflow desde a solicitação até a finalização (saída da planta).';

EXEC #AddDoc 'SGC_StatusAcesso', 'Id', 
    N'Identificador único do status. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_StatusAcesso', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_StatusAcesso', 'Codigo', 
    N'Código único do status (ex: PENDENTE, AUTORIZADO, REJEITADO, EM_PLANTA, FINALIZADO, CANCELADO). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_StatusAcesso', 'Descricao', 
    N'Descrição do status para exibição (ex: Aguardando Autorização, Em Andamento na planta).';

EXEC #AddDoc 'SGC_StatusAcesso', 'CorHex', 
    N'Código hexadecimal da cor para badges e indicadores visuais. Ex: #FFC107 (amarelo) para Pendente, #28A745 (verde) para Autorizado, #DC3545 (vermelho) para Rejeitado.';

EXEC #AddDoc 'SGC_StatusAcesso', 'EhFinal', 
    N'Indica se é um status final (encerra o fluxo). TRUE para: REJEITADO, FINALIZADO, CANCELADO. FALSE para status intermediários.';

EXEC #AddDoc 'SGC_StatusAcesso', 'Ordem', 
    N'Ordem de exibição, geralmente segue o fluxo natural do processo.';

EXEC #AddDoc 'SGC_StatusAcesso', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_StatusAcesso', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_StatusAcesso', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_StatusAcesso', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_StatusAcesso', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TipoChecklist
-- =============================================================================
PRINT '  Documentando SGC_TipoChecklist...';

EXEC #AddDoc 'SGC_TipoChecklist', NULL, 
    N'Tabela auxiliar com tipos de checklist aplicáveis em acessos à empresa. Referencia normas regulamentadoras como NR-20, IBAMA, SASSMAQ.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Id', 
    N'Identificador único do tipo de checklist. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoChecklist', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Codigo', 
    N'Código único do tipo (ex: NR20_VEICULO, IBAMA, SASSMAQ, INTEGRACAO). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Descricao', 
    N'Descrição do tipo de checklist (ex: Checklist NR-20 Inflamáveis, Checklist Integração Visitante).';

EXEC #AddDoc 'SGC_TipoChecklist', 'NormaReferencia', 
    N'Norma regulamentadora de referência (ex: NR-20, NR-33, IBAMA, SASSMAQ). NULL se não houver norma específica.';

EXEC #AddDoc 'SGC_TipoChecklist', 'AplicavelA', 
    N'Define a quem o checklist se aplica. Valores: VEICULO (veículos), PESSOA (pessoas), AMBOS (veículos e pessoas).';

EXEC #AddDoc 'SGC_TipoChecklist', 'Obrigatorio', 
    N'Indica se o preenchimento do checklist é obrigatório para o tipo de acesso correspondente.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Ordem', 
    N'Ordem de exibição.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoChecklist', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoChecklist', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoChecklist', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TipoParentesco
-- =============================================================================
PRINT '  Documentando SGC_TipoParentesco...';

EXEC #AddDoc 'SGC_TipoParentesco', NULL, 
    N'Tabela auxiliar com graus de parentesco para contatos de emergência. Padroniza a informação para relatórios e comunicação em casos de acidentes.';

EXEC #AddDoc 'SGC_TipoParentesco', 'Id', 
    N'Identificador único do tipo de parentesco. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoParentesco', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoParentesco', 'Codigo', 
    N'Código único (ex: CONJUGE, PAI, MAE, FILHO, IRMAO, AMIGO, VIZINHO, OUTRO). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoParentesco', 'Descricao', 
    N'Descrição do parentesco (ex: Cônjuge/Companheiro(a), Pai, Mãe, Filho(a)).';

EXEC #AddDoc 'SGC_TipoParentesco', 'Ordem', 
    N'Ordem de exibição. Parentescos mais próximos primeiro.';

EXEC #AddDoc 'SGC_TipoParentesco', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoParentesco', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TipoParentesco', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoParentesco', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoParentesco', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TipoSanguineo
-- =============================================================================
PRINT '  Documentando SGC_TipoSanguineo...';

EXEC #AddDoc 'SGC_TipoSanguineo', NULL, 
    N'Tabela auxiliar com tipos sanguíneos e informações de compatibilidade para doação/recepção. CRÍTICO: Informação essencial para atendimento de emergências médicas em ambiente industrial.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'Id', 
    N'Identificador único do tipo sanguíneo. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'Codigo', 
    N'Código do tipo sanguíneo (ex: A+, A-, B+, B-, AB+, AB-, O+, O-). Máx. 5 caracteres.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'Descricao', 
    N'Descrição por extenso (ex: A Positivo, O Negativo).';

EXEC #AddDoc 'SGC_TipoSanguineo', 'PodeDoarPara', 
    N'Lista de tipos sanguíneos que podem receber doação deste tipo (ex: A+ pode doar para A+ e AB+). Informativo.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'PodeReceberDe', 
    N'Lista de tipos sanguíneos que este tipo pode receber (ex: A+ pode receber de A+, A-, O+, O-). Informativo.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'Ordem', 
    N'Ordem de exibição.';

EXEC #AddDoc 'SGC_TipoSanguineo', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoSanguineo', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação. Tabela de domínio com auditoria simplificada.';
GO


-- =============================================================================
-- SGC_TipoDocumentoTerceiro
-- =============================================================================
PRINT '  Documentando SGC_TipoDocumentoTerceiro...';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', NULL, 
    N'Tabela auxiliar (catálogo) de tipos de documentos controlados para terceiros. Define validade padrão e se é obrigatório para liberação de acesso à planta.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Id', 
    N'Identificador único do tipo de documento. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Codigo', 
    N'Código único do tipo (ex: ASO, CNH, NR06, NR10, NR20, NR33, NR35, INTEGRACAO, MOPP). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Descricao', 
    N'Descrição completa do documento (ex: Atestado de Saúde Ocupacional, Treinamento NR-35 Trabalho em Altura).';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'ValidadeEmMeses', 
    N'Validade padrão em meses. Ex: 12 para ASO, 24 para NR-35, 60 para CNH. NULL se não tiver validade.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'EhObrigatorio', 
    N'Indica se o documento é obrigatório para liberação de acesso. Ex: ASO e Integração são obrigatórios; NRs específicas dependem da atividade.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'UsadoEmPortaria', 
    N'Indica se o documento é verificado no controle de acesso/portaria.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'UsadoEmTreinamento', 
    N'Indica se o documento está relacionado a certificações de treinamento.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoDocumentoTerceiro', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TipoTreinamento
-- =============================================================================
PRINT '  Documentando SGC_TipoTreinamento...';

EXEC #AddDoc 'SGC_TipoTreinamento', NULL, 
    N'Tabela auxiliar (catálogo) de tipos de treinamento com carga horária e validade padrão. Base para controle de capacitação de funcionários e terceiros conforme NRs.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Id', 
    N'Identificador único do tipo de treinamento. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Codigo', 
    N'Código único do treinamento (ex: NR06, NR10, NR20, NR33, NR35, INTEGRACAO, CIPA, BRIGADA). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Descricao', 
    N'Descrição completa (ex: NR-10 - Segurança em Instalações Elétricas, NR-35 - Trabalho em Altura).';

EXEC #AddDoc 'SGC_TipoTreinamento', 'CargaHorariaHoras', 
    N'Carga horária mínima em horas conforme norma. Ex: 40h para NR-10, 8h para NR-35. Decimal para permitir meias-horas.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'ValidadeEmMeses', 
    N'Validade em meses conforme norma. Ex: 24 meses para NR-10, 12 meses para NR-33. NULL se não tiver validade.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TipoTreinamento', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO

PRINT '';


-- #############################################################################
-- PARTE 2: TABELAS DE CADASTRO
-- #############################################################################

PRINT '>>> PARTE 2: TABELAS DE CADASTRO';
PRINT '';

-- =============================================================================
-- SGC_FornecedorEmpresa
-- =============================================================================
PRINT '  Documentando SGC_FornecedorEmpresa...';

EXEC #AddDoc 'SGC_FornecedorEmpresa', NULL, 
    N'Cadastro de empresas fornecedoras (CNPJ) que prestam serviços ou fornecem materiais. Armazena dados cadastrais, contato principal com validação de e-mail, contato secundário de backup e endereço completo.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Id', 
    N'Identificador único da empresa fornecedora. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'IdSaas', 
    N'Identificador do tenant para ambiente multi-empresa (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'CdEmpresaContratante', 
    N'Código da empresa contratante no sistema legado. Vínculo com tabela de empresas do ERP.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'CdFilialContratante', 
    N'Código da filial contratante no sistema legado. Junto com CdEmpresaContratante identifica a unidade.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'IdTipoFornecedor', 
    N'FK → SGC_TipoFornecedor.Id. Classificação do fornecedor (Manutenção, Limpeza, TI, etc.).';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'RazaoSocial', 
    N'Razão social da empresa fornecedora conforme CNPJ. Campo obrigatório. Máx. 150 caracteres.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'NomeFantasia', 
    N'Nome fantasia da empresa. Opcional. Máx. 100 caracteres.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Cnpj', 
    N'CNPJ da empresa fornecedora. Somente números, 14 dígitos. Campo obrigatório e único.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'InscricaoEstadual', 
    N'Inscrição Estadual da empresa. Somente números. Opcional.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'InscricaoMunicipal', 
    N'Inscrição Municipal da empresa. Somente números. Opcional.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'NomeContatoPrincipal', 
    N'Nome da pessoa de contato principal na empresa fornecedora.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'EmailPrincipal', 
    N'E-mail principal de contato da empresa. Utilizado para comunicações oficiais e verificação.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'EmailVerificado', 
    N'Indica se o e-mail principal foi verificado/autenticado. 0=Não verificado, 1=Verificado. Verificação via token enviado por e-mail.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'EmailDataVerificacao', 
    N'Data e hora UTC em que o e-mail foi verificado com sucesso. NULL se ainda não verificado.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'EmailTokenVerificacao', 
    N'Token único (GUID ou hash) enviado por e-mail para validação. Limpo após verificação bem-sucedida.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'EmailTokenExpiracao', 
    N'Data e hora UTC de expiração do token de verificação. Padrão: 24 horas após geração.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'TelefonePrincipal', 
    N'Telefone fixo principal com DDD. Formato livre, máx. 20 caracteres.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'CelularPrincipal', 
    N'Celular principal com DDD. Formato: somente números (ex: 11999998888). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'CelularPrincipalWhatsApp', 
    N'Indica se o celular principal possui WhatsApp ativo. 0=Não, 1=Sim. Usado para integração com API WhatsApp Business.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'ContatoSecundarioNome', 
    N'Nome do contato secundário/backup da empresa. Acionado quando contato principal não responde.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'ContatoSecundarioEmail', 
    N'E-mail do contato secundário.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'ContatoSecundarioCelular', 
    N'Celular do contato secundário com DDD.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'ContatoSecundarioCelularWhatsApp', 
    N'Indica se o celular secundário possui WhatsApp.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Logradouro', 
    N'Endereço: logradouro (rua, avenida, etc.).';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Numero', 
    N'Endereço: número ou S/N.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Complemento', 
    N'Endereço: complemento (sala, andar, bloco).';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Bairro', 
    N'Endereço: bairro.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Cidade', 
    N'Endereço: cidade/município.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Uf', 
    N'Endereço: UF (sigla do estado, 2 caracteres).';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Cep', 
    N'Endereço: CEP (somente números, 8 dígitos).';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Ativo', 
    N'Indica se a empresa está ativa no sistema. 0=Inativa (não aparece em seleções), 1=Ativa.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação do registro.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou o registro (FK → tuse1.id).';

EXEC #AddDoc 'SGC_FornecedorEmpresa', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que realizou a última atualização (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_FornecedorColaborador
-- =============================================================================
PRINT '  Documentando SGC_FornecedorColaborador...';

EXEC #AddDoc 'SGC_FornecedorColaborador', NULL, 
    N'Cadastro de colaboradores (pessoas físicas) de empresas terceiras/fornecedoras. Inclui dados pessoais, contato com validação de e-mail, contato de emergência obrigatório e informações médicas críticas para ambiente industrial (tipo sanguíneo, alergias, doenças crônicas).';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Id', 
    N'Identificador único do colaborador terceiro. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'IdSaas', 
    N'Identificador do tenant (SaaS). NULL para dados globais.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'IdFornecedorEmpresa', 
    N'FK → SGC_FornecedorEmpresa.Id. Empresa à qual o colaborador está vinculado. Campo obrigatório.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'NomeCompleto', 
    N'Nome completo do colaborador conforme documento. Campo obrigatório. Máx. 150 caracteres.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'NomeCracha', 
    N'Nome para exibição em crachá, geralmente primeiro nome e sobrenome. Máx. 60 caracteres.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Cpf', 
    N'CPF do colaborador. Somente números, 11 dígitos. Campo obrigatório e único por empresa.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Rg', 
    N'Número do RG/Identidade. Formato livre. Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'OrgaoRg', 
    N'Órgão emissor do RG (ex: SSP, DETRAN, IFP). Máx. 20 caracteres.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'UfRg', 
    N'UF de emissão do RG. 2 caracteres.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'DataNascimento', 
    N'Data de nascimento do colaborador. Formato DATE.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Sexo', 
    N'Sexo do colaborador. Valores: M=Masculino, F=Feminino, O=Outro/Não informado.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Funcao', 
    N'Função/cargo do colaborador na empresa fornecedora (ex: Eletricista, Soldador, Motorista). Máx. 80 caracteres.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'MatriculaFornecedor', 
    N'Matrícula ou código do colaborador na empresa fornecedora. Para referência cruzada.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Email', 
    N'E-mail pessoal ou corporativo do colaborador.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'EmailVerificado', 
    N'Indica se o e-mail foi verificado. 0=Não, 1=Sim.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'EmailDataVerificacao', 
    N'Data/hora UTC da verificação do e-mail.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'EmailTokenVerificacao', 
    N'Token único para verificação de e-mail.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'EmailTokenExpiracao', 
    N'Data/hora de expiração do token de verificação.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Telefone', 
    N'Telefone fixo com DDD.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Celular', 
    N'Celular com DDD. Somente números.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'CelularWhatsApp', 
    N'Indica se o celular possui WhatsApp. 0=Não, 1=Sim.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'ContatoEmergenciaNome', 
    N'CRÍTICO: Nome completo do contato de emergência. Pessoa a ser acionada em caso de acidente. Recomendado obrigatório.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'ContatoEmergenciaParentesco', 
    N'Grau de parentesco com o contato de emergência (ex: Cônjuge, Pai, Mãe, Filho, Irmão).';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'ContatoEmergenciaCelular', 
    N'Celular do contato de emergência com DDD. CRÍTICO para acidentes.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'ContatoEmergenciaCelularWhatsApp', 
    N'Indica se o celular de emergência possui WhatsApp.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'ContatoEmergenciaTelefoneFixo', 
    N'Telefone fixo alternativo do contato de emergência.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'TipoSanguineo', 
    N'CRÍTICO: Tipo sanguíneo do colaborador (A+, A-, B+, B-, AB+, AB-, O+, O-). Essencial para atendimento de emergência médica.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'PossuiAlergia', 
    N'CRÍTICO: Indica se o colaborador possui alguma alergia conhecida. 0=Não, 1=Sim.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'DescricaoAlergia', 
    N'Descrição detalhada das alergias (medicamentos, alimentos, substâncias químicas, látex, etc.). Informar equipe de resgate.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'PossuiDoencaCronica', 
    N'CRÍTICO: Indica se possui doença crônica. 0=Não, 1=Sim. Ex: diabetes, hipertensão, epilepsia, cardiopatia.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'DescricaoDoencaCronica', 
    N'Descrição das doenças crônicas. Informação vital para equipe médica em emergências.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'MedicacaoUsoContinuo', 
    N'Lista de medicações de uso contínuo do colaborador. Relevante para interação medicamentosa em emergências.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'PodeAcessarPlanta', 
    N'Indica se o colaborador está liberado para acesso à planta industrial. 0=Bloqueado, 1=Liberado. Bloqueio pode ser por documentação vencida.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'DataValidadeCadastro', 
    N'Data limite de validade da liberação de acesso. Após esta data, requer renovação de documentação.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Ativo', 
    N'Status do colaborador. 0=Inativo (desligado da empresa ou bloqueado), 1=Ativo.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_FornecedorColaborador', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_Visitante
-- =============================================================================
PRINT '  Documentando SGC_Visitante...';

EXEC #AddDoc 'SGC_Visitante', NULL, 
    N'Cadastro de visitantes (pessoas não recorrentes) para controle de portaria. Inclui dados pessoais, contato com validação simplificada de e-mail e contato de emergência básico.';

EXEC #AddDoc 'SGC_Visitante', 'Id', 
    N'Identificador único do visitante. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_Visitante', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_Visitante', 'IdTipoPessoa', 
    N'FK → SGC_TipoPessoa.Id. Tipo de visitante (Visitante comum, Candidato, Familiar, Autoridade, etc.).';

EXEC #AddDoc 'SGC_Visitante', 'NomeCompleto', 
    N'Nome completo do visitante. Campo obrigatório.';

EXEC #AddDoc 'SGC_Visitante', 'Cpf', 
    N'CPF do visitante. Somente números. Opcional dependendo do tipo de pessoa.';

EXEC #AddDoc 'SGC_Visitante', 'Rg', 
    N'Número do RG/Identidade.';

EXEC #AddDoc 'SGC_Visitante', 'OrgaoRg', 
    N'Órgão emissor do RG.';

EXEC #AddDoc 'SGC_Visitante', 'Email', 
    N'E-mail do visitante.';

EXEC #AddDoc 'SGC_Visitante', 'EmailVerificado', 
    N'Indica se o e-mail foi verificado. Verificação simplificada (sem token).';

EXEC #AddDoc 'SGC_Visitante', 'EmailDataVerificacao', 
    N'Data/hora da verificação do e-mail.';

EXEC #AddDoc 'SGC_Visitante', 'Telefone', 
    N'Telefone fixo com DDD.';

EXEC #AddDoc 'SGC_Visitante', 'Celular', 
    N'Celular com DDD.';

EXEC #AddDoc 'SGC_Visitante', 'CelularWhatsApp', 
    N'Indica se o celular possui WhatsApp.';

EXEC #AddDoc 'SGC_Visitante', 'ContatoEmergenciaNome', 
    N'Nome do contato de emergência.';

EXEC #AddDoc 'SGC_Visitante', 'ContatoEmergenciaCelular', 
    N'Celular do contato de emergência.';

EXEC #AddDoc 'SGC_Visitante', 'ContatoEmergenciaCelularWhatsApp', 
    N'Indica se o celular de emergência possui WhatsApp.';

EXEC #AddDoc 'SGC_Visitante', 'EmpresaOrigem', 
    N'Nome da empresa de origem do visitante. Informativo.';

EXEC #AddDoc 'SGC_Visitante', 'CargoFuncao', 
    N'Cargo ou função do visitante na empresa de origem.';

EXEC #AddDoc 'SGC_Visitante', 'FotoPath', 
    N'Caminho do arquivo de foto do visitante. Capturada na portaria.';

EXEC #AddDoc 'SGC_Visitante', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_Visitante', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_Visitante', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_Visitante', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_Visitante', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_Instrutor
-- =============================================================================
PRINT '  Documentando SGC_Instrutor...';

EXEC #AddDoc 'SGC_Instrutor', NULL, 
    N'Cadastro de instrutores para ministrar treinamentos. Pode ser funcionário interno ou profissional externo. Armazena qualificações e especialidades.';

EXEC #AddDoc 'SGC_Instrutor', 'Id', 
    N'Identificador único do instrutor. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_Instrutor', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_Instrutor', 'CdEmpresa', 
    N'Código da empresa no sistema legado.';

EXEC #AddDoc 'SGC_Instrutor', 'CdFilial', 
    N'Código da filial no sistema legado.';

EXEC #AddDoc 'SGC_Instrutor', 'NomeCompleto', 
    N'Nome completo do instrutor.';

EXEC #AddDoc 'SGC_Instrutor', 'Cpf', 
    N'CPF do instrutor.';

EXEC #AddDoc 'SGC_Instrutor', 'Email', 
    N'E-mail do instrutor.';

EXEC #AddDoc 'SGC_Instrutor', 'EmailVerificado', 
    N'Indica se o e-mail foi verificado.';

EXEC #AddDoc 'SGC_Instrutor', 'EmailDataVerificacao', 
    N'Data/hora da verificação do e-mail.';

EXEC #AddDoc 'SGC_Instrutor', 'Telefone', 
    N'Telefone fixo.';

EXEC #AddDoc 'SGC_Instrutor', 'Celular', 
    N'Celular com DDD.';

EXEC #AddDoc 'SGC_Instrutor', 'CelularWhatsApp', 
    N'Indica se o celular possui WhatsApp.';

EXEC #AddDoc 'SGC_Instrutor', 'EhInterno', 
    N'Indica se é funcionário interno (1) ou instrutor externo/terceiro (0).';

EXEC #AddDoc 'SGC_Instrutor', 'IdFuncionario', 
    N'FK → func1.id. ID do funcionário se for instrutor interno. NULL se externo.';

EXEC #AddDoc 'SGC_Instrutor', 'EmpresaExterna', 
    N'Nome da empresa externa se for instrutor terceiro.';

EXEC #AddDoc 'SGC_Instrutor', 'Especialidades', 
    N'Lista de especialidades/treinamentos que o instrutor está habilitado a ministrar. Texto livre.';

EXEC #AddDoc 'SGC_Instrutor', 'RegistroProfissional', 
    N'Registro profissional (CREA, CRM, etc.) se aplicável.';

EXEC #AddDoc 'SGC_Instrutor', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_Instrutor', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_Instrutor', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_Instrutor', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_Instrutor', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_LocalTreinamento
-- =============================================================================
PRINT '  Documentando SGC_LocalTreinamento...';

EXEC #AddDoc 'SGC_LocalTreinamento', NULL, 
    N'Cadastro de locais (salas/auditórios) para realização de treinamentos. Define capacidade e recursos disponíveis. Pode ser interno ou externo.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Id', 
    N'Identificador único do local. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_LocalTreinamento', 'CdEmpresa', 
    N'Código da empresa.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'CdFilial', 
    N'Código da filial.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Codigo', 
    N'Código único do local (ex: SALA01, AUDIT_A, CAMPO_PRATICO).';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Nome', 
    N'Nome descritivo do local (ex: Sala de Treinamento 01, Auditório Principal).';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Descricao', 
    N'Descrição adicional ou observações sobre o local.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Capacidade', 
    N'Capacidade máxima de pessoas do local.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'TemProjetor', 
    N'Indica se possui projetor/datashow disponível.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'TemArCondicionado', 
    N'Indica se possui ar condicionado.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'EhExterno', 
    N'Indica se é local externo à empresa (alugado, parceiro, etc.).';

EXEC #AddDoc 'SGC_LocalTreinamento', 'EnderecoExterno', 
    N'Endereço completo se for local externo.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_LocalTreinamento', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_Veiculo
-- =============================================================================
PRINT '  Documentando SGC_Veiculo...';

EXEC #AddDoc 'SGC_Veiculo', NULL, 
    N'Cadastro de veículos para controle de portaria e pesagem. Pode ser vinculado a empresa fornecedora, funcionário ou terceiro avulso. Armazena dados do veículo e proprietário.';

EXEC #AddDoc 'SGC_Veiculo', 'Id', 
    N'Identificador único do veículo. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_Veiculo', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_Veiculo', 'IdTipoVeiculo', 
    N'FK → SGC_TipoVeiculo.Id. Tipo do veículo (define regras de checklist e pesagem).';

EXEC #AddDoc 'SGC_Veiculo', 'Placa', 
    N'Placa do veículo. Formato Mercosul (sem hífen): ABC1D23. 7 caracteres. Obrigatório e único.';

EXEC #AddDoc 'SGC_Veiculo', 'PlacaFormatada', 
    N'Placa formatada para exibição (ex: ABC-1D23). Campo calculado/auxiliar.';

EXEC #AddDoc 'SGC_Veiculo', 'Renavam', 
    N'Código RENAVAM do veículo.';

EXEC #AddDoc 'SGC_Veiculo', 'Marca', 
    N'Marca/fabricante (ex: Volkswagen, Mercedes-Benz, Volvo).';

EXEC #AddDoc 'SGC_Veiculo', 'Modelo', 
    N'Modelo do veículo (ex: Constellation 24.280, FH 540).';

EXEC #AddDoc 'SGC_Veiculo', 'AnoFabricacao', 
    N'Ano de fabricação do veículo.';

EXEC #AddDoc 'SGC_Veiculo', 'AnoModelo', 
    N'Ano do modelo do veículo.';

EXEC #AddDoc 'SGC_Veiculo', 'Cor', 
    N'Cor predominante do veículo.';

EXEC #AddDoc 'SGC_Veiculo', 'CapacidadeCarga', 
    N'Capacidade máxima de carga em toneladas. Decimal para precisão.';

EXEC #AddDoc 'SGC_Veiculo', 'TipoCarroceria', 
    N'Tipo de carroceria (Baú, Graneleiro, Tanque, Sider, Prancha, etc.).';

EXEC #AddDoc 'SGC_Veiculo', 'ProprietarioTipo', 
    N'Tipo de proprietário. E=Empresa fornecedora, F=Funcionário interno, T=Terceiro avulso.';

EXEC #AddDoc 'SGC_Veiculo', 'IdFornecedorEmpresa', 
    N'FK → SGC_FornecedorEmpresa.Id. Preenchido se ProprietarioTipo = E.';

EXEC #AddDoc 'SGC_Veiculo', 'IdFuncionario', 
    N'FK → func1.id. Preenchido se ProprietarioTipo = F.';

EXEC #AddDoc 'SGC_Veiculo', 'NomeProprietario', 
    N'Nome do proprietário se ProprietarioTipo = T (terceiro avulso).';

EXEC #AddDoc 'SGC_Veiculo', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_Veiculo', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_Veiculo', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_Veiculo', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_Veiculo', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TreinamentoTurma
-- =============================================================================
PRINT '  Documentando SGC_TreinamentoTurma...';

EXEC #AddDoc 'SGC_TreinamentoTurma', NULL, 
    N'Turmas/sessões de treinamento com data, horário, instrutor e local. Agrega participantes na tabela SGC_TreinamentoParticipante.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Id', 
    N'Identificador único da turma. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'IdTipoTreinamento', 
    N'FK → SGC_TipoTreinamento.Id. Tipo de treinamento sendo ministrado.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'IdInstrutor', 
    N'FK → SGC_Instrutor.Id. Instrutor responsável pela turma.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'IdLocalTreinamento', 
    N'FK → SGC_LocalTreinamento.Id. Local onde será realizado.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'DataInicio', 
    N'Data e hora de início do treinamento.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'DataFim', 
    N'Data e hora de término do treinamento. NULL se ainda não finalizado.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Local', 
    N'Descrição livre do local. Alternativa ao IdLocalTreinamento para locais não cadastrados.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Instrutor', 
    N'Nome do instrutor. Alternativa ao IdInstrutor para instrutores não cadastrados.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Observacao', 
    N'Observações gerais sobre a turma.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Ativo', 
    N'Status ativo (1) ou cancelado/inativo (0).';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TreinamentoTurma', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_TreinamentoParticipante
-- =============================================================================
PRINT '  Documentando SGC_TreinamentoParticipante...';

EXEC #AddDoc 'SGC_TreinamentoParticipante', NULL, 
    N'Participantes de turmas de treinamento. Pode ser funcionário interno ou colaborador terceiro. Registra presença, aprovação e data de validade resultante.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Id', 
    N'Identificador único do participante na turma. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'IdTreinamentoTurma', 
    N'FK → SGC_TreinamentoTurma.Id. Turma de treinamento.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'IdFuncionario', 
    N'FK → func1.id. ID do funcionário interno. NULL se for terceiro. Exclusivo com IdFornecedorColaborador.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'IdFornecedorColaborador', 
    N'FK → SGC_FornecedorColaborador.Id. ID do terceiro. NULL se for funcionário. Exclusivo com IdFuncionario.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Presenca', 
    N'Indica se o participante compareceu ao treinamento. 0=Ausente, 1=Presente. Default 1.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'DataValidadeResultante', 
    N'Data de validade do certificado resultante. Calculada: DataFim + ValidadeEmMeses do tipo de treinamento.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Aprovado', 
    N'Indica se foi aprovado no treinamento (quando há avaliação). NULL=Sem avaliação, 0=Reprovado, 1=Aprovado.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Observacao', 
    N'Observações sobre a participação (justificativa de ausência, nota da avaliação, etc.).';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Ativo', 
    N'Status ativo (1) ou cancelado (0).';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_TreinamentoParticipante', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_PessoaDocumento
-- =============================================================================
PRINT '  Documentando SGC_PessoaDocumento...';

EXEC #AddDoc 'SGC_PessoaDocumento', NULL, 
    N'Documentos de funcionários e terceiros (ASO, CNH, certificados de NRs). Controla data de validade e armazena caminho do arquivo digitalizado. Base para alertas de vencimento.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Id', 
    N'Identificador único do documento. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_PessoaDocumento', 'EhFuncionario', 
    N'Define a qual tipo de pessoa o documento pertence. 1=Funcionário interno (usa IdFuncionario), 0=Terceiro (usa IdFornecedorColaborador).';

EXEC #AddDoc 'SGC_PessoaDocumento', 'IdFuncionario', 
    N'FK → func1.id. ID do funcionário se EhFuncionario=1.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'IdFornecedorColaborador', 
    N'FK → SGC_FornecedorColaborador.Id. ID do terceiro se EhFuncionario=0.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'IdTipoDocumentoTerceiro', 
    N'FK → SGC_TipoDocumentoTerceiro.Id. Tipo de documento (ASO, CNH, NR-10, etc.).';

EXEC #AddDoc 'SGC_PessoaDocumento', 'NumeroDocumento', 
    N'Número ou código do documento (ex: número do certificado, número da CNH).';

EXEC #AddDoc 'SGC_PessoaDocumento', 'DataEmissao', 
    N'Data de emissão/realização do documento/treinamento.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'DataValidade', 
    N'Data de validade do documento. Usado para alertas de vencimento. NULL se não tiver validade.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'ArquivoPath', 
    N'Caminho do arquivo digitalizado (PDF, imagem). Relativo ao storage.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Observacao', 
    N'Observações sobre o documento.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Ativo', 
    N'Status ativo (1) ou inativo/substituído (0).';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_PessoaDocumento', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_ChecklistModelo
-- =============================================================================
PRINT '  Documentando SGC_ChecklistModelo...';

EXEC #AddDoc 'SGC_ChecklistModelo', NULL, 
    N'Modelos de checklist para aplicação em acessos à empresa. Cada modelo contém uma lista de itens/perguntas na tabela SGC_ChecklistItem. Suporta versionamento.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Id', 
    N'Identificador único do modelo. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_ChecklistModelo', 'IdTipoChecklist', 
    N'FK → SGC_TipoChecklist.Id. Tipo de checklist (NR-20, IBAMA, etc.).';

EXEC #AddDoc 'SGC_ChecklistModelo', 'CdEmpresa', 
    N'Código da empresa. Modelo pode ser específico por filial.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'CdFilial', 
    N'Código da filial.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Nome', 
    N'Nome do modelo de checklist.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Descricao', 
    N'Descrição detalhada do propósito do checklist.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Versao', 
    N'Versão do modelo (ex: 1.0, 2.0). Permite rastreabilidade de alterações.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Ativo', 
    N'Status ativo (1) ou inativo/obsoleto (0).';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_ChecklistModelo', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_ChecklistItem
-- =============================================================================
PRINT '  Documentando SGC_ChecklistItem...';

EXEC #AddDoc 'SGC_ChecklistItem', NULL, 
    N'Itens (perguntas/verificações) de um modelo de checklist. Define tipo de resposta esperada (Sim/Não, texto, número) e se item não conforme bloqueia o acesso.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Id', 
    N'Identificador único do item. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_ChecklistItem', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_ChecklistItem', 'IdChecklistModelo', 
    N'FK → SGC_ChecklistModelo.Id. Modelo ao qual o item pertence.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Ordem', 
    N'Ordem de exibição do item no checklist. Deve ser sequencial.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Codigo', 
    N'Código do item para referência (ex: NR20-01, NR20-02).';

EXEC #AddDoc 'SGC_ChecklistItem', 'Pergunta', 
    N'Texto da pergunta/verificação (ex: "Extintor de incêndio dentro da validade?").';

EXEC #AddDoc 'SGC_ChecklistItem', 'TipoResposta', 
    N'Tipo de resposta esperada. Valores: SIM_NAO (Sim/Não), SIM_NAO_NA (Sim/Não/Não Aplicável), TEXTO (livre), NUMERO (numérico), DATA (data).';

EXEC #AddDoc 'SGC_ChecklistItem', 'RespostaEsperada', 
    N'Resposta esperada para conformidade. Ex: "SIM" para perguntas onde Sim é conforme.';

EXEC #AddDoc 'SGC_ChecklistItem', 'EhBloqueante', 
    N'Indica se resposta não conforme bloqueia a liberação de acesso. CRÍTICO para segurança.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Observacao', 
    N'Instrução ou observação para quem preenche o checklist.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Ativo', 
    N'Status ativo (1) ou inativo (0).';

EXEC #AddDoc 'SGC_ChecklistItem', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_ChecklistItem', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_ChecklistItem', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO

PRINT '';


-- #############################################################################
-- PARTE 3: TABELAS TRANSACIONAIS
-- #############################################################################

PRINT '>>> PARTE 3: TABELAS TRANSACIONAIS';
PRINT '';

-- =============================================================================
-- SGC_RegistroAcesso
-- =============================================================================
PRINT '  Documentando SGC_RegistroAcesso...';

EXEC #AddDoc 'SGC_RegistroAcesso', NULL, 
    N'Registro de entrada e saída de pessoas na empresa. Controla protocolo único, status do fluxo, veículo, carga, pesagem e checklist aplicado. Tabela principal de movimentação da portaria.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Id', 
    N'Identificador único do registro de acesso. Chave primária auto-incremento BIGINT para alto volume.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_RegistroAcesso', 'CdEmpresa', 
    N'Código da empresa.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'CdFilial', 
    N'Código da filial/portaria onde ocorreu o acesso.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'NumeroProtocolo', 
    N'Número único do protocolo de acesso. Formato sugerido: ANO+SEQUENCIAL (ex: 2025000001). Campo único.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdStatusAcesso', 
    N'FK → SGC_StatusAcesso.Id. Status atual do acesso (Pendente, Autorizado, Em Planta, Finalizado, etc.).';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdMotivoAcesso', 
    N'FK → SGC_MotivoAcesso.Id. Motivo do acesso (Entrega, Serviço, Reunião, etc.).';

EXEC #AddDoc 'SGC_RegistroAcesso', 'TipoPessoa', 
    N'Tipo de pessoa. F=Funcionário interno, T=Terceiro/Prestador, V=Visitante. Define qual coluna de FK usar.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdFuncionario', 
    N'FK → func1.id. ID do funcionário se TipoPessoa=F.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdFornecedorColaborador', 
    N'FK → SGC_FornecedorColaborador.Id. ID do terceiro se TipoPessoa=T.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdVisitante', 
    N'FK → SGC_Visitante.Id. ID do visitante se TipoPessoa=V.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'SetorDestino', 
    N'Setor/área de destino dentro da empresa.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'PessoaContato', 
    N'Nome do funcionário responsável/contato interno que autorizou ou receberá o visitante.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'PossuiVeiculo', 
    N'Indica se a pessoa está com veículo. 0=Não (pedestre), 1=Sim.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'IdVeiculo', 
    N'FK → SGC_Veiculo.Id. Veículo cadastrado no sistema.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'PlacaVeiculoAvulso', 
    N'Placa do veículo não cadastrado (avulso). Usado para veículos de primeira visita.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'PossuiCarga', 
    N'Indica se o veículo está transportando carga.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'DescricaoCarga', 
    N'Descrição da carga transportada.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'PesoEntrada', 
    N'Peso do veículo na entrada em kg. Registrado pela balança.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'PesoSaida', 
    N'Peso do veículo na saída em kg. Registrado pela balança. Diferença indica carga entregue/coletada.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'NotaFiscal', 
    N'Número da nota fiscal da carga. Obrigatório para entregas e coletas.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'DataHoraEntrada', 
    N'Data e hora de entrada na empresa. Preenchido na liberação.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'DataHoraSaida', 
    N'Data e hora de saída da empresa. NULL enquanto está na planta.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'DataHoraPrevisaoSaida', 
    N'Data e hora prevista para saída. Usado para alertas de permanência prolongada.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'NumeroCrachaVisitante', 
    N'Número do crachá de visitante emprestado. Deve ser devolvido na saída.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Observacao', 
    N'Observações gerais sobre o acesso.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Ativo', 
    N'Status ativo (1) ou cancelado (0).';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou o registro (FK → tuse1.id).';

EXEC #AddDoc 'SGC_RegistroAcesso', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_RegistroAcessoChecklist
-- =============================================================================
PRINT '  Documentando SGC_RegistroAcessoChecklist...';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', NULL, 
    N'Respostas do checklist aplicado em um registro de acesso. Cada registro representa uma resposta a um item do checklist. Histórico de verificações realizadas.';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'Id', 
    N'Identificador único da resposta. Chave primária auto-incremento BIGINT.';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'IdRegistroAcesso', 
    N'FK → SGC_RegistroAcesso.Id. Registro de acesso ao qual a resposta pertence.';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'IdChecklistItem', 
    N'FK → SGC_ChecklistItem.Id. Item do checklist respondido.';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'Resposta', 
    N'Resposta fornecida pelo operador (SIM, NAO, NA, ou valor texto/número).';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'Conforme', 
    N'Indica se a resposta está conforme o esperado. NULL=Não avaliado, 0=Não conforme, 1=Conforme.';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'Observacao', 
    N'Observação adicional sobre o item (justificativa de não conformidade, etc.).';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação (momento do preenchimento).';

EXEC #AddDoc 'SGC_RegistroAcessoChecklist', 'Aud_IdUsuarioCadastro', 
    N'Usuário que preencheu o checklist (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_Autorizacao
-- =============================================================================
PRINT '  Documentando SGC_Autorizacao...';

EXEC #AddDoc 'SGC_Autorizacao', NULL, 
    N'Autorizações pré-aprovadas para acesso de terceiros ou visitantes recorrentes. Define período de vigência e restrições de horário/dias da semana.';

EXEC #AddDoc 'SGC_Autorizacao', 'Id', 
    N'Identificador único da autorização. Chave primária auto-incremento.';

EXEC #AddDoc 'SGC_Autorizacao', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_Autorizacao', 'CdEmpresa', 
    N'Código da empresa.';

EXEC #AddDoc 'SGC_Autorizacao', 'CdFilial', 
    N'Código da filial.';

EXEC #AddDoc 'SGC_Autorizacao', 'NumeroAutorizacao', 
    N'Número único da autorização. Para referência em relatórios e comunicação.';

EXEC #AddDoc 'SGC_Autorizacao', 'TipoPessoa', 
    N'Tipo de pessoa autorizada. T=Terceiro/Prestador, V=Visitante.';

EXEC #AddDoc 'SGC_Autorizacao', 'IdFornecedorColaborador', 
    N'FK → SGC_FornecedorColaborador.Id. Terceiro autorizado se TipoPessoa=T.';

EXEC #AddDoc 'SGC_Autorizacao', 'IdVisitante', 
    N'FK → SGC_Visitante.Id. Visitante autorizado se TipoPessoa=V.';

EXEC #AddDoc 'SGC_Autorizacao', 'DataInicio', 
    N'Data de início da vigência da autorização.';

EXEC #AddDoc 'SGC_Autorizacao', 'DataFim', 
    N'Data de fim da vigência da autorização.';

EXEC #AddDoc 'SGC_Autorizacao', 'HoraInicioPermitida', 
    N'Hora de início do período diário permitido (ex: 08:00). NULL = qualquer hora.';

EXEC #AddDoc 'SGC_Autorizacao', 'HoraFimPermitida', 
    N'Hora de fim do período diário permitido (ex: 18:00). NULL = qualquer hora.';

EXEC #AddDoc 'SGC_Autorizacao', 'DiasPermitidos', 
    N'Dias da semana permitidos. Formato: SEG,TER,QUA,QUI,SEX ou 1,2,3,4,5. NULL = todos os dias.';

EXEC #AddDoc 'SGC_Autorizacao', 'IdFuncionarioAprovador', 
    N'FK → func1.id. Funcionário que aprovou a autorização.';

EXEC #AddDoc 'SGC_Autorizacao', 'DataAprovacao', 
    N'Data/hora da aprovação da autorização.';

EXEC #AddDoc 'SGC_Autorizacao', 'MotivoAutorizacao', 
    N'Justificativa/motivo para concessão da autorização.';

EXEC #AddDoc 'SGC_Autorizacao', 'Ativo', 
    N'Status ativo (1) ou cancelado/revogado (0).';

EXEC #AddDoc 'SGC_Autorizacao', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação.';

EXEC #AddDoc 'SGC_Autorizacao', 'Aud_UpdatedAt', 
    N'Data/hora UTC da última atualização.';

EXEC #AddDoc 'SGC_Autorizacao', 'Aud_IdUsuarioCadastro', 
    N'Usuário que criou (FK → tuse1.id).';

EXEC #AddDoc 'SGC_Autorizacao', 'Aud_IdUsuarioAtualizacao', 
    N'Usuário que atualizou (FK → tuse1.id).';
GO


-- =============================================================================
-- SGC_AlertaVencimento
-- =============================================================================
PRINT '  Documentando SGC_AlertaVencimento...';

EXEC #AddDoc 'SGC_AlertaVencimento', NULL, 
    N'Alertas automáticos de vencimento de documentos, treinamentos, ASOs e autorizações. Gerados por job/processo batch. Controla workflow de leitura e resolução.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Id', 
    N'Identificador único do alerta. Chave primária auto-incremento BIGINT para alto volume.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdSaas', 
    N'Identificador do tenant (SaaS).';

EXEC #AddDoc 'SGC_AlertaVencimento', 'CdEmpresa', 
    N'Código da empresa.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'CdFilial', 
    N'Código da filial.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'TipoAlerta', 
    N'Tipo/origem do alerta. Valores: DOCUMENTO, TREINAMENTO, ASO, AUTORIZACAO.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdPessoaDocumento', 
    N'FK → SGC_PessoaDocumento.Id. Se TipoAlerta=DOCUMENTO.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdTreinamentoParticipante', 
    N'FK → SGC_TreinamentoParticipante.Id. Se TipoAlerta=TREINAMENTO.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdAutorizacao', 
    N'FK → SGC_Autorizacao.Id. Se TipoAlerta=AUTORIZACAO.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'TipoPessoa', 
    N'Tipo da pessoa afetada. F=Funcionário, T=Terceiro, V=Visitante.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdFuncionario', 
    N'FK → func1.id. ID do funcionário se TipoPessoa=F.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdFornecedorColaborador', 
    N'FK → SGC_FornecedorColaborador.Id. ID do terceiro se TipoPessoa=T.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'IdVisitante', 
    N'FK → SGC_Visitante.Id. ID do visitante se TipoPessoa=V.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'DataVencimento', 
    N'Data de vencimento do item alertado.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'DiasParaVencimento', 
    N'Dias restantes para o vencimento no momento da criação do alerta. Negativo = já vencido.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Descricao', 
    N'Descrição resumida do alerta (ex: "ASO de João Silva vence em 15 dias").';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Lido', 
    N'Indica se o alerta foi visualizado por algum usuário. 0=Não lido, 1=Lido.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'DataLeitura', 
    N'Data/hora da primeira visualização do alerta.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Aud_IdUsuarioLeitura', 
    N'Usuário que leu o alerta (FK → tuse1.id).';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Resolvido', 
    N'Indica se a situação foi resolvida (documento renovado, treinamento realizado). 0=Pendente, 1=Resolvido.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'DataResolucao', 
    N'Data/hora da resolução.';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Aud_IdUsuarioResolucao', 
    N'Usuário que marcou como resolvido (FK → tuse1.id).';

EXEC #AddDoc 'SGC_AlertaVencimento', 'ObservacaoResolucao', 
    N'Observação sobre a resolução (ex: novo documento cadastrado, treinamento realizado).';

EXEC #AddDoc 'SGC_AlertaVencimento', 'Aud_CreatedAt', 
    N'Data/hora UTC de criação do alerta (geração automática).';
GO


-- #############################################################################
-- LIMPEZA E FINALIZAÇÃO
-- #############################################################################

DROP PROCEDURE #AddDoc;
GO

PRINT '';
PRINT '=============================================================================';
PRINT 'DOCUMENTAÇÃO CONCLUÍDA COM SUCESSO!';
PRINT 'Término: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '=============================================================================';
PRINT '';
PRINT 'Tabelas documentadas: 26';
PRINT '';
PRINT 'Para consultar a documentação, execute:';
PRINT '';
PRINT 'SELECT ';
PRINT '    t.name AS Tabela,';
PRINT '    CAST(ep.value AS VARCHAR(500)) AS Descricao';
PRINT 'FROM sys.tables t';
PRINT 'INNER JOIN sys.extended_properties ep ON ep.major_id = t.object_id AND ep.minor_id = 0';
PRINT 'WHERE t.name LIKE ''SGC_%'' AND ep.name = ''MS_Description''';
PRINT 'ORDER BY t.name;';
PRINT '';
PRINT '=============================================================================';
GO