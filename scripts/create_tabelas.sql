
,USE [bd_rhu_copenor];
GO

/* =============================================================================
   ███████╗ ██████╗  ██████╗    ██╗   ██╗██████╗    ██████╗ 
   ██╔════╝██╔════╝ ██╔════╝    ██║   ██║╚════██╗  ██╔═████╗
   ███████╗██║  ███╗██║         ██║   ██║ █████╔╝  ██║██╔██║
   ╚════██║██║   ██║██║         ╚██╗ ██╔╝ ╚═══██╗  ████╔╝██║
   ███████║╚██████╔╝╚██████╗     ╚████╔╝ ██████╔╝  ╚██████╔╝
   ╚══════╝ ╚═════╝  ╚═════╝      ╚═══╝  ╚═════╝    ╚═════╝ 
   
   SISTEMA DE GESTÃO CORPORATIVA - SCRIPT CONSOLIDADO
   ===================================================
   
   Versão: 3.0
   Data: Dezembro/2025
   Autor: RhSensoERP
   Banco: SQL Server 2019+
   
   PADRÃO DE AUDITORIA:
   - Aud_CreatedAt          : Data/hora de criação (UTC)
   - Aud_UpdatedAt          : Data/hora de atualização (UTC)
   - Aud_IdUsuarioCadastro  : FK → tuse1.id (usuário que criou)
   - Aud_IdUsuarioAtualizacao: FK → tuse1.id (usuário que atualizou)
   
   ORDEM DE EXECUÇÃO:
   1. Tabelas Auxiliares (Lookups)
   2. Tabelas de Cadastro
   3. Tabelas Transacionais
   4. Foreign Keys
   5. Extended Properties (Documentação)
   
============================================================================= */

SET NOCOUNT ON;
PRINT '=============================================================================';
PRINT 'SGC - SCRIPT CONSOLIDADO v3.0';
PRINT 'Início: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '=============================================================================';
PRINT '';

-- #############################################################################
-- PARTE 1: TABELAS AUXILIARES (LOOKUPS)
-- #############################################################################

PRINT '>>> PARTE 1: TABELAS AUXILIARES (LOOKUPS)';
PRINT '';

/* =============================================================================
   SGC_TipoFornecedor
   Descrição: Classificação das empresas fornecedoras por tipo de serviço.
              Permite categorizar fornecedores para relatórios e filtros.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoFornecedor', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoFornecedor](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [Icone]                    VARCHAR(50)  NULL,
        [CorHex]                   CHAR(7)      NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoFornecedor_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoFornecedor_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoFornecedor_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoFornecedor] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoFornecedor_Codigo]
    ON [dbo].[SGC_TipoFornecedor]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoFornecedor] ([Codigo], [Descricao], [Ordem])
    VALUES 
        ('MANUTENCAO', 'Manutenção Industrial', 1),
        ('LIMPEZA', 'Limpeza e Conservação', 2),
        ('SEGURANCA', 'Segurança Patrimonial', 3),
        ('TRANSPORTE', 'Transporte e Logística', 4),
        ('ALIMENTACAO', 'Alimentação e Refeições', 5),
        ('TI', 'Tecnologia da Informação', 6),
        ('CONSULTORIA', 'Consultoria e Assessoria', 7),
        ('CONSTRUCAO', 'Construção Civil', 8),
        ('OUTROS', 'Outros Serviços', 99);
    
    PRINT '  [OK] SGC_TipoFornecedor criada';
END
ELSE
    PRINT '  [--] SGC_TipoFornecedor já existe';
GO


/* =============================================================================
   SGC_TipoVeiculo
   Descrição: Classificação de veículos com regras de negócio para portaria.
              Define se o veículo exige checklist NR-20 ou pesagem obrigatória.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoVeiculo', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoVeiculo](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [ExigeChecklistNR20]       BIT          NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_ExigeNR20] DEFAULT (0),
        [ExigePesagem]             BIT          NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_ExigePesagem] DEFAULT (0),
        [Icone]                    VARCHAR(50)  NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoVeiculo] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoVeiculo_Codigo]
    ON [dbo].[SGC_TipoVeiculo]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoVeiculo] ([Codigo], [Descricao], [ExigeChecklistNR20], [ExigePesagem], [Ordem])
    VALUES 
        ('CARRO', 'Carro de Passeio', 0, 0, 1),
        ('MOTO', 'Motocicleta', 0, 0, 2),
        ('PICKUP', 'Pickup/Utilitário', 0, 0, 3),
        ('VAN', 'Van/Furgão', 0, 0, 4),
        ('CAMINHAO_P', 'Caminhão Pequeno (até 3,5t)', 0, 0, 5),
        ('CAMINHAO_M', 'Caminhão Médio (3,5t a 8t)', 0, 1, 6),
        ('CAMINHAO_G', 'Caminhão Grande (8t a 16t)', 1, 1, 7),
        ('CARRETA', 'Carreta/Semi-Reboque', 1, 1, 8),
        ('TANQUE', 'Caminhão Tanque (Inflamáveis)', 1, 1, 9),
        ('BITREM', 'Bitrem/Rodotrem', 1, 1, 10),
        ('BICICLETA', 'Bicicleta', 0, 0, 11),
        ('OUTRO', 'Outro Veículo', 0, 0, 99);
    
    PRINT '  [OK] SGC_TipoVeiculo criada';
END
ELSE
    PRINT '  [--] SGC_TipoVeiculo já existe';
GO


/* =============================================================================
   SGC_TipoPessoa
   Descrição: Classificação de tipos de pessoa para controle de acesso.
              Define regras como exigência de documento, foto ou vínculo com empresa.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoPessoa', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoPessoa](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [ExigeDocumento]           BIT          NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_ExigeDoc] DEFAULT (1),
        [ExigeFoto]                BIT          NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_ExigeFoto] DEFAULT (0),
        [ExigeEmpresa]             BIT          NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_ExigeEmpresa] DEFAULT (0),
        [PermiteVeiculoProprio]    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_VeiculoProprio] DEFAULT (0),
        [Icone]                    VARCHAR(50)  NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoPessoa_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoPessoa] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoPessoa_Codigo]
    ON [dbo].[SGC_TipoPessoa]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoPessoa] ([Codigo], [Descricao], [ExigeDocumento], [ExigeFoto], [ExigeEmpresa], [PermiteVeiculoProprio], [Ordem])
    VALUES 
        ('VISITANTE', 'Visitante', 1, 1, 0, 0, 1),
        ('PRESTADOR', 'Prestador de Serviço', 1, 1, 1, 1, 2),
        ('MOTORISTA', 'Motorista de Carga', 1, 0, 1, 0, 3),
        ('ENTREGADOR', 'Entregador', 1, 0, 1, 0, 4),
        ('CONSULTOR', 'Consultor/Auditor', 1, 1, 1, 1, 5),
        ('CANDIDATO', 'Candidato a Vaga', 1, 0, 0, 0, 6),
        ('FAMILIAR', 'Familiar de Funcionário', 1, 0, 0, 0, 7),
        ('AUTORIDADE', 'Autoridade/Fiscal', 1, 0, 1, 1, 8),
        ('OUTRO', 'Outro', 1, 0, 0, 0, 99);
    
    PRINT '  [OK] SGC_TipoPessoa criada';
END
ELSE
    PRINT '  [--] SGC_TipoPessoa já existe';
GO


/* =============================================================================
   SGC_TipoAso
   Descrição: Tipos de Atestado de Saúde Ocupacional conforme NR-07.
              Define validade padrão para cada tipo de exame.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoAso', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoAso](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [Sigla]                    VARCHAR(10)  NOT NULL,
        [ValidadeEmMesesPadrao]    INT          NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoAso_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoAso_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoAso_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoAso] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoAso_Codigo]
    ON [dbo].[SGC_TipoAso]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoAso] ([Codigo], [Descricao], [Sigla], [ValidadeEmMesesPadrao], [Ordem])
    VALUES 
        ('ADMISSIONAL', 'ASO Admissional', 'ADM', 12, 1),
        ('PERIODICO', 'ASO Periódico', 'PER', 12, 2),
        ('RETORNO', 'ASO Retorno ao Trabalho', 'RET', 12, 3),
        ('MUDANCA', 'ASO Mudança de Risco/Função', 'MUD', 12, 4),
        ('DEMISSIONAL', 'ASO Demissional', 'DEM', NULL, 5);
    
    PRINT '  [OK] SGC_TipoAso criada';
END
ELSE
    PRINT '  [--] SGC_TipoAso já existe';
GO


/* =============================================================================
   SGC_MotivoAcesso
   Descrição: Motivos de acesso à empresa para controle de portaria.
              Define regras como exigência de destino, responsável ou nota fiscal.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_MotivoAcesso', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_MotivoAcesso](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [ExigeDestino]             BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoAcesso_ExigeDestino] DEFAULT (1),
        [ExigeResponsavel]         BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoAcesso_ExigeResponsavel] DEFAULT (0),
        [ExigeNotaFiscal]          BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoAcesso_ExigeNF] DEFAULT (0),
        [Icone]                    VARCHAR(50)  NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_MotivoAcesso_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoAcesso_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_MotivoAcesso_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_MotivoAcesso] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_MotivoAcesso_Codigo]
    ON [dbo].[SGC_MotivoAcesso]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_MotivoAcesso] ([Codigo], [Descricao], [ExigeDestino], [ExigeResponsavel], [ExigeNotaFiscal], [Ordem])
    VALUES 
        ('ENTREGA', 'Entrega de Mercadoria', 1, 0, 1, 1),
        ('COLETA', 'Coleta de Material', 1, 0, 1, 2),
        ('SERVICO', 'Prestação de Serviço', 1, 1, 0, 3),
        ('REUNIAO', 'Reunião/Evento', 1, 1, 0, 4),
        ('VISITA', 'Visita Técnica', 1, 1, 0, 5),
        ('AUDITORIA', 'Auditoria/Fiscalização', 1, 1, 0, 6),
        ('ENTREVISTA', 'Entrevista de Emprego', 1, 1, 0, 7),
        ('MANUTENCAO', 'Manutenção Programada', 1, 1, 0, 8),
        ('EMERGENCIA', 'Emergência', 0, 0, 0, 9),
        ('OUTRO', 'Outro Motivo', 1, 0, 0, 99);
    
    PRINT '  [OK] SGC_MotivoAcesso criada';
END
ELSE
    PRINT '  [--] SGC_MotivoAcesso já existe';
GO


/* =============================================================================
   SGC_StatusAcesso
   Descrição: Status do ciclo de vida de um registro de acesso.
              Controla o workflow desde a solicitação até a finalização.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_StatusAcesso', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_StatusAcesso](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [CorHex]                   CHAR(7)      NULL,
        [EhFinal]                  BIT          NOT NULL CONSTRAINT [DF_SGC_StatusAcesso_EhFinal] DEFAULT (0),
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_StatusAcesso_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_StatusAcesso_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_StatusAcesso_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_StatusAcesso] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_StatusAcesso_Codigo]
    ON [dbo].[SGC_StatusAcesso]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_StatusAcesso] ([Codigo], [Descricao], [CorHex], [EhFinal], [Ordem])
    VALUES 
        ('PENDENTE', 'Aguardando Autorização', '#FFC107', 0, 1),
        ('AUTORIZADO', 'Autorizado', '#28A745', 0, 2),
        ('REJEITADO', 'Rejeitado/Negado', '#DC3545', 1, 3),
        ('EM_PLANTA', 'Em Andamento (na planta)', '#17A2B8', 0, 4),
        ('FINALIZADO', 'Finalizado (saiu)', '#6C757D', 1, 5),
        ('CANCELADO', 'Cancelado', '#343A40', 1, 6);
    
    PRINT '  [OK] SGC_StatusAcesso criada';
END
ELSE
    PRINT '  [--] SGC_StatusAcesso já existe';
GO


/* =============================================================================
   SGC_TipoChecklist
   Descrição: Tipos de checklist aplicáveis em acessos à empresa.
              Referencia normas como NR-20, IBAMA, SASSMAQ.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoChecklist', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoChecklist](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(100) NOT NULL,
        [NormaReferencia]          VARCHAR(50)  NULL,
        [AplicavelA]               VARCHAR(20)  NOT NULL,
        [Obrigatorio]              BIT          NOT NULL CONSTRAINT [DF_SGC_TipoChecklist_Obrigatorio] DEFAULT (0),
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoChecklist_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoChecklist_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoChecklist_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoChecklist] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_TipoChecklist_AplicavelA] CHECK ([AplicavelA] IN ('VEICULO', 'PESSOA', 'AMBOS'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoChecklist_Codigo]
    ON [dbo].[SGC_TipoChecklist]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoChecklist] ([Codigo], [Descricao], [NormaReferencia], [AplicavelA], [Obrigatorio], [Ordem])
    VALUES 
        ('NR20_VEICULO', 'Checklist NR-20 (Inflamáveis)', 'NR-20', 'VEICULO', 1, 1),
        ('IBAMA', 'Checklist Ambiental IBAMA', 'IBAMA', 'VEICULO', 0, 2),
        ('SASSMAQ', 'Checklist SASSMAQ', 'SASSMAQ', 'VEICULO', 0, 3),
        ('SEG_MOTORISTA', 'Checklist Segurança Motorista', NULL, 'PESSOA', 0, 4),
        ('INTEGRACAO', 'Checklist Integração Visitante', NULL, 'PESSOA', 0, 5),
        ('VEICULO_GERAL', 'Inspeção Veicular Geral', NULL, 'VEICULO', 0, 6);
    
    PRINT '  [OK] SGC_TipoChecklist criada';
END
ELSE
    PRINT '  [--] SGC_TipoChecklist já existe';
GO


/* =============================================================================
   SGC_TipoParentesco
   Descrição: Graus de parentesco para contatos de emergência.
              Padroniza a informação para relatórios e comunicação.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoParentesco', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoParentesco](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(50)  NOT NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoParentesco_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoParentesco_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoParentesco_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoParentesco] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoParentesco_Codigo]
    ON [dbo].[SGC_TipoParentesco]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoParentesco] ([Codigo], [Descricao], [Ordem])
    VALUES 
        ('CONJUGE', 'Cônjuge/Companheiro(a)', 1),
        ('PAI', 'Pai', 2),
        ('MAE', 'Mãe', 3),
        ('FILHO', 'Filho(a)', 4),
        ('IRMAO', 'Irmão/Irmã', 5),
        ('AVO', 'Avô/Avó', 6),
        ('TIO', 'Tio/Tia', 7),
        ('PRIMO', 'Primo(a)', 8),
        ('SOGRO', 'Sogro(a)', 9),
        ('CUNHADO', 'Cunhado(a)', 10),
        ('AMIGO', 'Amigo(a)', 11),
        ('VIZINHO', 'Vizinho(a)', 12),
        ('OUTRO', 'Outro', 99);
    
    PRINT '  [OK] SGC_TipoParentesco criada';
END
ELSE
    PRINT '  [--] SGC_TipoParentesco já existe';
GO


/* =============================================================================
   SGC_TipoSanguineo
   Descrição: Tipos sanguíneos com informações de compatibilidade.
              Informação crítica para emergências médicas em ambiente industrial.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoSanguineo', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoSanguineo](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(5)   NOT NULL,
        [Descricao]                VARCHAR(20)  NOT NULL,
        [PodeDoarPara]             VARCHAR(50)  NULL,
        [PodeReceberDe]            VARCHAR(50)  NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_TipoSanguineo_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoSanguineo_Ativo] DEFAULT (1),
        
        -- Auditoria (simplificada - tabela de domínio)
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoSanguineo_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        
        CONSTRAINT [PK_SGC_TipoSanguineo] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoSanguineo_Codigo]
    ON [dbo].[SGC_TipoSanguineo]([Codigo]) WHERE [IdSaas] IS NULL;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoSanguineo] ([Codigo], [Descricao], [PodeDoarPara], [PodeReceberDe], [Ordem])
    VALUES 
        ('A+',  'A Positivo',  'A+, AB+',           'A+, A-, O+, O-', 1),
        ('A-',  'A Negativo',  'A+, A-, AB+, AB-',  'A-, O-',         2),
        ('B+',  'B Positivo',  'B+, AB+',           'B+, B-, O+, O-', 3),
        ('B-',  'B Negativo',  'B+, B-, AB+, AB-',  'B-, O-',         4),
        ('AB+', 'AB Positivo', 'AB+',               'Todos',          5),
        ('AB-', 'AB Negativo', 'AB+, AB-',          'A-, B-, AB-, O-',6),
        ('O+',  'O Positivo',  'A+, B+, AB+, O+',   'O+, O-',         7),
        ('O-',  'O Negativo',  'Todos',             'O-',             8);
    
    PRINT '  [OK] SGC_TipoSanguineo criada';
END
ELSE
    PRINT '  [--] SGC_TipoSanguineo já existe';
GO


/* =============================================================================
   SGC_TipoDocumentoTerceiro
   Descrição: Catálogo de tipos de documentos controlados para terceiros.
              Define validade padrão e se é obrigatório para acesso.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoDocumentoTerceiro', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoDocumentoTerceiro](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(150) NOT NULL,
        [ValidadeEmMeses]          INT          NULL,
        [EhObrigatorio]            BIT          NOT NULL CONSTRAINT [DF_SGC_TipoDocumentoTerceiro_EhObrigatorio] DEFAULT (0),
        [UsadoEmPortaria]          BIT          NOT NULL CONSTRAINT [DF_SGC_TipoDocumentoTerceiro_UsadoEmPortaria] DEFAULT (1),
        [UsadoEmTreinamento]       BIT          NOT NULL CONSTRAINT [DF_SGC_TipoDocumentoTerceiro_UsadoEmTreinamento] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoDocumentoTerceiro_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoDocumentoTerceiro_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoDocumentoTerceiro] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoDocumentoTerceiro_Codigo]
    ON [dbo].[SGC_TipoDocumentoTerceiro]([Codigo]);
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoDocumentoTerceiro] ([Codigo], [Descricao], [ValidadeEmMeses], [EhObrigatorio], [UsadoEmPortaria], [UsadoEmTreinamento])
    VALUES 
        ('ASO', 'Atestado de Saúde Ocupacional', 12, 1, 1, 0),
        ('CNH', 'Carteira Nacional de Habilitação', 60, 0, 1, 0),
        ('NR06', 'Treinamento NR-06 (EPI)', 24, 0, 1, 1),
        ('NR10', 'Treinamento NR-10 (Eletricidade)', 24, 0, 1, 1),
        ('NR12', 'Treinamento NR-12 (Máquinas)', 24, 0, 1, 1),
        ('NR20', 'Treinamento NR-20 (Inflamáveis)', 12, 0, 1, 1),
        ('NR33', 'Treinamento NR-33 (Espaço Confinado)', 12, 0, 1, 1),
        ('NR35', 'Treinamento NR-35 (Trabalho em Altura)', 24, 0, 1, 1),
        ('INTEGRACAO', 'Integração de Segurança', 12, 1, 1, 1),
        ('MOPP', 'Movimentação Produtos Perigosos', 60, 0, 1, 0);
    
    PRINT '  [OK] SGC_TipoDocumentoTerceiro criada';
END
ELSE
    PRINT '  [--] SGC_TipoDocumentoTerceiro já existe';
GO


/* =============================================================================
   SGC_TipoTreinamento
   Descrição: Catálogo de tipos de treinamento com carga horária e validade.
              Base para controle de capacitação de funcionários e terceiros.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TipoTreinamento', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TipoTreinamento](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Registro
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Descricao]                VARCHAR(150) NOT NULL,
        [CargaHorariaHoras]        DECIMAL(5,2) NULL,
        [ValidadeEmMeses]          INT          NULL,
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TipoTreinamento_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TipoTreinamento_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TipoTreinamento] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_TipoTreinamento_Codigo]
    ON [dbo].[SGC_TipoTreinamento]([Codigo]);
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_TipoTreinamento] ([Codigo], [Descricao], [CargaHorariaHoras], [ValidadeEmMeses])
    VALUES 
        ('NR06', 'NR-06 - Equipamento de Proteção Individual', 4.00, 24),
        ('NR10', 'NR-10 - Segurança em Instalações Elétricas', 40.00, 24),
        ('NR12', 'NR-12 - Segurança em Máquinas e Equipamentos', 8.00, 24),
        ('NR20', 'NR-20 - Inflamáveis e Combustíveis', 8.00, 12),
        ('NR33', 'NR-33 - Espaço Confinado', 16.00, 12),
        ('NR35', 'NR-35 - Trabalho em Altura', 8.00, 24),
        ('INTEGRACAO', 'Integração de Segurança', 4.00, 12),
        ('CIPA', 'CIPA - Comissão Interna Prevenção Acidentes', 20.00, 12),
        ('BRIGADA', 'Brigada de Incêndio', 8.00, 12),
        ('PRIMEIROS_SOCORROS', 'Primeiros Socorros', 8.00, 24),
        ('DIRECAO_DEFENSIVA', 'Direção Defensiva', 8.00, 24);
    
    PRINT '  [OK] SGC_TipoTreinamento criada';
END
ELSE
    PRINT '  [--] SGC_TipoTreinamento já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 2: TABELAS DE CADASTRO (ENTIDADES PRINCIPAIS)
-- #############################################################################

PRINT '>>> PARTE 2: TABELAS DE CADASTRO';
PRINT '';

/* =============================================================================
   SGC_FornecedorEmpresa
   Descrição: Cadastro de empresas fornecedoras e prestadoras de serviço.
              Armazena dados cadastrais, contato principal e secundário.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_FornecedorEmpresa', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_FornecedorEmpresa](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresaContratante]     INT          NOT NULL,
        [CdFilialContratante]      INT          NOT NULL,
        [IdTipoFornecedor]         INT          NULL,
        
        -- Dados Cadastrais
        [RazaoSocial]              VARCHAR(150) NOT NULL,
        [NomeFantasia]             VARCHAR(100) NULL,
        [Cnpj]                     CHAR(14)     NOT NULL,
        [InscricaoEstadual]        VARCHAR(20)  NULL,
        [InscricaoMunicipal]       VARCHAR(20)  NULL,
        
        -- Contato Principal
        [NomeContatoPrincipal]     VARCHAR(80)  NULL,
        [EmailPrincipal]           VARCHAR(150) NULL,
        [EmailVerificado]          BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorEmpresa_EmailVerificado] DEFAULT (0),
        [EmailDataVerificacao]     DATETIME2(0) NULL,
        [EmailTokenVerificacao]    VARCHAR(100) NULL,
        [EmailTokenExpiracao]      DATETIME2(0) NULL,
        [TelefonePrincipal]        VARCHAR(20)  NULL,
        [CelularPrincipal]         VARCHAR(20)  NULL,
        [CelularPrincipalWhatsApp] BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorEmpresa_CelularWhatsApp] DEFAULT (0),
        
        -- Contato Secundário (Backup)
        [ContatoSecundarioNome]    VARCHAR(100) NULL,
        [ContatoSecundarioEmail]   VARCHAR(150) NULL,
        [ContatoSecundarioCelular] VARCHAR(20)  NULL,
        [ContatoSecundarioCelularWhatsApp] BIT  NOT NULL CONSTRAINT [DF_SGC_FornecedorEmpresa_ContatoSecWhatsApp] DEFAULT (0),
        
        -- Endereço
        [Logradouro]               VARCHAR(150) NULL,
        [Numero]                   VARCHAR(20)  NULL,
        [Complemento]              VARCHAR(60)  NULL,
        [Bairro]                   VARCHAR(80)  NULL,
        [Cidade]                   VARCHAR(80)  NULL,
        [Uf]                       CHAR(2)      NULL,
        [Cep]                      CHAR(8)      NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorEmpresa_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_FornecedorEmpresa_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_FornecedorEmpresa] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_FornecedorEmpresa_Cnpj]
    ON [dbo].[SGC_FornecedorEmpresa]([Cnpj]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_FornecedorEmpresa_RazaoSocial]
    ON [dbo].[SGC_FornecedorEmpresa]([RazaoSocial]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_FornecedorEmpresa_Filial]
    ON [dbo].[SGC_FornecedorEmpresa]([CdEmpresaContratante], [CdFilialContratante]);
    
    PRINT '  [OK] SGC_FornecedorEmpresa criada';
END
ELSE
    PRINT '  [--] SGC_FornecedorEmpresa já existe';
GO


/* =============================================================================
   SGC_FornecedorColaborador
   Descrição: Cadastro de colaboradores de empresas terceiras (prestadores).
              Inclui dados pessoais, contato, emergência e informações médicas.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_FornecedorColaborador', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_FornecedorColaborador](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdFornecedorEmpresa]      INT          NOT NULL,
        
        -- Dados Pessoais
        [NomeCompleto]             VARCHAR(150) NOT NULL,
        [NomeCracha]               VARCHAR(60)  NULL,
        [Cpf]                      CHAR(11)     NOT NULL,
        [Rg]                       VARCHAR(20)  NULL,
        [OrgaoRg]                  VARCHAR(20)  NULL,
        [UfRg]                     CHAR(2)      NULL,
        [DataNascimento]           DATE         NULL,
        [Sexo]                     CHAR(1)      NULL,
        
        -- Dados Profissionais
        [Funcao]                   VARCHAR(80)  NULL,
        [MatriculaFornecedor]      VARCHAR(30)  NULL,
        
        -- Contato
        [Email]                    VARCHAR(150) NULL,
        [EmailVerificado]          BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_EmailVerificado] DEFAULT (0),
        [EmailDataVerificacao]     DATETIME2(0) NULL,
        [EmailTokenVerificacao]    VARCHAR(100) NULL,
        [EmailTokenExpiracao]      DATETIME2(0) NULL,
        [Telefone]                 VARCHAR(20)  NULL,
        [Celular]                  VARCHAR(20)  NULL,
        [CelularWhatsApp]          BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_CelularWhatsApp] DEFAULT (0),
        
        -- Contato de Emergência
        [ContatoEmergenciaNome]    VARCHAR(100) NULL,
        [ContatoEmergenciaParentesco] VARCHAR(50) NULL,
        [ContatoEmergenciaCelular] VARCHAR(20)  NULL,
        [ContatoEmergenciaCelularWhatsApp] BIT  NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_EmergWhatsApp] DEFAULT (0),
        [ContatoEmergenciaTelefoneFixo] VARCHAR(20) NULL,
        
        -- Informações Médicas
        [TipoSanguineo]            VARCHAR(5)   NULL,
        [PossuiAlergia]            BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_PossuiAlergia] DEFAULT (0),
        [DescricaoAlergia]         VARCHAR(255) NULL,
        [PossuiDoencaCronica]      BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_DoencaCronica] DEFAULT (0),
        [DescricaoDoencaCronica]   VARCHAR(255) NULL,
        [MedicacaoUsoContinuo]     VARCHAR(255) NULL,
        
        -- Controle de Acesso
        [PodeAcessarPlanta]        BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_PodeAcessarPlanta] DEFAULT (1),
        [DataValidadeCadastro]     DATE         NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_FornecedorColaborador] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_FornecedorColaborador_Sexo] CHECK ([Sexo] IN ('M', 'F', 'O'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_FornecedorColaborador_Fornecedor_Cpf]
    ON [dbo].[SGC_FornecedorColaborador]([IdFornecedorEmpresa], [Cpf]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_FornecedorColaborador_Nome]
    ON [dbo].[SGC_FornecedorColaborador]([NomeCompleto]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_FornecedorColaborador_Cpf]
    ON [dbo].[SGC_FornecedorColaborador]([Cpf]);
    
    PRINT '  [OK] SGC_FornecedorColaborador criada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador já existe';
GO


/* =============================================================================
   SGC_Visitante
   Descrição: Cadastro de visitantes (pessoas não recorrentes) para portaria.
              Inclui dados pessoais, contato e contato de emergência.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Visitante', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Visitante](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdTipoPessoa]             INT          NOT NULL,
        
        -- Dados Pessoais
        [NomeCompleto]             VARCHAR(150) NOT NULL,
        [Cpf]                      CHAR(11)     NULL,
        [Rg]                       VARCHAR(20)  NULL,
        [OrgaoRg]                  VARCHAR(20)  NULL,
        
        -- Contato
        [Email]                    VARCHAR(150) NULL,
        [EmailVerificado]          BIT          NOT NULL CONSTRAINT [DF_SGC_Visitante_EmailVerificado] DEFAULT (0),
        [EmailDataVerificacao]     DATETIME2(0) NULL,
        [Telefone]                 VARCHAR(20)  NULL,
        [Celular]                  VARCHAR(20)  NULL,
        [CelularWhatsApp]          BIT          NOT NULL CONSTRAINT [DF_SGC_Visitante_CelularWhatsApp] DEFAULT (0),
        
        -- Contato de Emergência
        [ContatoEmergenciaNome]    VARCHAR(100) NULL,
        [ContatoEmergenciaCelular] VARCHAR(20)  NULL,
        [ContatoEmergenciaCelularWhatsApp] BIT  NOT NULL CONSTRAINT [DF_SGC_Visitante_EmergWhatsApp] DEFAULT (0),
        
        -- Origem
        [EmpresaOrigem]            VARCHAR(150) NULL,
        [CargoFuncao]              VARCHAR(80)  NULL,
        [FotoPath]                 VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Visitante_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Visitante_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Visitante] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Visitante_Cpf]
    ON [dbo].[SGC_Visitante]([Cpf]) WHERE [Cpf] IS NOT NULL;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Visitante_Nome]
    ON [dbo].[SGC_Visitante]([NomeCompleto]);
    
    PRINT '  [OK] SGC_Visitante criada';
END
ELSE
    PRINT '  [--] SGC_Visitante já existe';
GO


/* =============================================================================
   SGC_Instrutor
   Descrição: Cadastro de instrutores de treinamento (internos e externos).
              Pode ser vinculado a um funcionário interno ou empresa externa.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Instrutor', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Instrutor](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Dados Pessoais
        [NomeCompleto]             VARCHAR(150) NOT NULL,
        [Cpf]                      CHAR(11)     NULL,
        
        -- Contato
        [Email]                    VARCHAR(150) NULL,
        [EmailVerificado]          BIT          NOT NULL CONSTRAINT [DF_SGC_Instrutor_EmailVerificado] DEFAULT (0),
        [EmailDataVerificacao]     DATETIME2(0) NULL,
        [Telefone]                 VARCHAR(20)  NULL,
        [Celular]                  VARCHAR(20)  NULL,
        [CelularWhatsApp]          BIT          NOT NULL CONSTRAINT [DF_SGC_Instrutor_CelularWhatsApp] DEFAULT (0),
        
        -- Vínculo
        [EhInterno]                BIT          NOT NULL CONSTRAINT [DF_SGC_Instrutor_EhInterno] DEFAULT (1),
        [IdFuncionario]            UNIQUEIDENTIFIER NULL,
        [EmpresaExterna]           VARCHAR(150) NULL,
        
        -- Qualificação
        [Especialidades]           VARCHAR(500) NULL,
        [RegistroProfissional]     VARCHAR(50)  NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Instrutor_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Instrutor_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Instrutor] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Instrutor_Nome]
    ON [dbo].[SGC_Instrutor]([NomeCompleto]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Instrutor_Funcionario]
    ON [dbo].[SGC_Instrutor]([IdFuncionario]) WHERE [IdFuncionario] IS NOT NULL;
    
    PRINT '  [OK] SGC_Instrutor criada';
END
ELSE
    PRINT '  [--] SGC_Instrutor já existe';
GO


/* =============================================================================
   SGC_LocalTreinamento
   Descrição: Cadastro de locais (salas) para realização de treinamentos.
              Define capacidade e recursos disponíveis no local.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_LocalTreinamento', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_LocalTreinamento](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Dados do Local
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Nome]                     VARCHAR(100) NOT NULL,
        [Descricao]                VARCHAR(255) NULL,
        [Capacidade]               INT          NULL,
        [TemProjetor]              BIT          NOT NULL CONSTRAINT [DF_SGC_LocalTreinamento_Projetor] DEFAULT (0),
        [TemArCondicionado]        BIT          NOT NULL CONSTRAINT [DF_SGC_LocalTreinamento_AC] DEFAULT (0),
        [EhExterno]                BIT          NOT NULL CONSTRAINT [DF_SGC_LocalTreinamento_Externo] DEFAULT (0),
        [EnderecoExterno]          VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_LocalTreinamento_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_LocalTreinamento_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_LocalTreinamento] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_LocalTreinamento_Codigo]
    ON [dbo].[SGC_LocalTreinamento]([CdEmpresa], [CdFilial], [Codigo]);
    
    PRINT '  [OK] SGC_LocalTreinamento criada';
END
ELSE
    PRINT '  [--] SGC_LocalTreinamento já existe';
GO


/* =============================================================================
   SGC_Veiculo
   Descrição: Cadastro de veículos para controle de portaria e pesagem.
              Pode ser vinculado a empresa fornecedora ou funcionário.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Veiculo', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Veiculo](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdTipoVeiculo]            INT          NOT NULL,
        
        -- Dados do Veículo
        [Placa]                    CHAR(7)      NOT NULL,
        [PlacaFormatada]           VARCHAR(10)  NULL,
        [Renavam]                  VARCHAR(15)  NULL,
        [Marca]                    VARCHAR(50)  NULL,
        [Modelo]                   VARCHAR(50)  NULL,
        [AnoFabricacao]            INT          NULL,
        [AnoModelo]                INT          NULL,
        [Cor]                      VARCHAR(30)  NULL,
        [CapacidadeCarga]          DECIMAL(10,2) NULL,
        [TipoCarroceria]           VARCHAR(50)  NULL,
        
        -- Proprietário (E=Empresa, F=Funcionário, T=Terceiro)
        [ProprietarioTipo]         CHAR(1)      NOT NULL,
        [IdFornecedorEmpresa]      INT          NULL,
        [IdFuncionario]            UNIQUEIDENTIFIER NULL,
        [NomeProprietario]         VARCHAR(150) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Veiculo_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Veiculo_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Veiculo] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_Veiculo_ProprietarioTipo] CHECK ([ProprietarioTipo] IN ('E', 'F', 'T'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_Veiculo_Placa]
    ON [dbo].[SGC_Veiculo]([Placa]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Veiculo_TipoVeiculo]
    ON [dbo].[SGC_Veiculo]([IdTipoVeiculo]);
    
    PRINT '  [OK] SGC_Veiculo criada';
END
ELSE
    PRINT '  [--] SGC_Veiculo já existe';
GO


/* =============================================================================
   SGC_TreinamentoTurma
   Descrição: Turmas/sessões de treinamento com data, instrutor e local.
              Agrega participantes em SGC_TreinamentoParticipante.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TreinamentoTurma', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TreinamentoTurma](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdTipoTreinamento]        INT          NOT NULL,
        [IdInstrutor]              INT          NULL,
        [IdLocalTreinamento]       INT          NULL,
        
        -- Dados da Turma
        [DataInicio]               DATETIME2(0) NOT NULL,
        [DataFim]                  DATETIME2(0) NULL,
        [Local]                    VARCHAR(150) NULL,
        [Instrutor]                VARCHAR(150) NULL,
        [Observacao]               VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TreinamentoTurma_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TreinamentoTurma_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TreinamentoTurma] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_TreinamentoTurma_Data]
    ON [dbo].[SGC_TreinamentoTurma]([DataInicio] DESC);
    
    PRINT '  [OK] SGC_TreinamentoTurma criada';
END
ELSE
    PRINT '  [--] SGC_TreinamentoTurma já existe';
GO


/* =============================================================================
   SGC_TreinamentoParticipante
   Descrição: Participantes de turmas de treinamento (funcionários ou terceiros).
              Registra presença, aprovação e data de validade resultante.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_TreinamentoParticipante', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_TreinamentoParticipante](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdTreinamentoTurma]       INT          NOT NULL,
        
        -- Participante (apenas um deve ser preenchido)
        [IdFuncionario]            UNIQUEIDENTIFIER NULL,
        [IdFornecedorColaborador]  INT          NULL,
        
        -- Resultado
        [Presenca]                 BIT          NOT NULL CONSTRAINT [DF_SGC_TreinamentoParticipante_Presenca] DEFAULT (1),
        [DataValidadeResultante]   DATE         NULL,
        [Aprovado]                 BIT          NULL,
        [Observacao]               VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_TreinamentoParticipante_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_TreinamentoParticipante_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_TreinamentoParticipante] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_TreinamentoParticipante_Pessoa] CHECK (
            ([IdFuncionario] IS NOT NULL AND [IdFornecedorColaborador] IS NULL) OR
            ([IdFuncionario] IS NULL AND [IdFornecedorColaborador] IS NOT NULL)
        )
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_TreinamentoParticipante_Turma]
    ON [dbo].[SGC_TreinamentoParticipante]([IdTreinamentoTurma]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_TreinamentoParticipante_Funcionario]
    ON [dbo].[SGC_TreinamentoParticipante]([IdFuncionario]) WHERE [IdFuncionario] IS NOT NULL;
    
    PRINT '  [OK] SGC_TreinamentoParticipante criada';
END
ELSE
    PRINT '  [--] SGC_TreinamentoParticipante já existe';
GO


/* =============================================================================
   SGC_PessoaDocumento
   Descrição: Documentos de funcionários e terceiros (ASO, CNH, certificados).
              Controla validade e armazena caminho do arquivo digitalizado.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_PessoaDocumento', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_PessoaDocumento](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Pessoa (EhFuncionario define qual coluna usar)
        [EhFuncionario]            BIT          NOT NULL,
        [IdFuncionario]            UNIQUEIDENTIFIER NULL,
        [IdFornecedorColaborador]  INT          NULL,
        
        -- Documento
        [IdTipoDocumentoTerceiro]  INT          NOT NULL,
        [NumeroDocumento]          VARCHAR(50)  NULL,
        [DataEmissao]              DATE         NULL,
        [DataValidade]             DATE         NULL,
        [ArquivoPath]              VARCHAR(255) NULL,
        [Observacao]               VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_PessoaDocumento_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_PessoaDocumento_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_PessoaDocumento] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_PessoaDocumento_TipoPessoa] CHECK (
            ([EhFuncionario] = 1 AND [IdFuncionario] IS NOT NULL AND [IdFornecedorColaborador] IS NULL) OR
            ([EhFuncionario] = 0 AND [IdFuncionario] IS NULL AND [IdFornecedorColaborador] IS NOT NULL)
        )
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_PessoaDocumento_Funcionario]
    ON [dbo].[SGC_PessoaDocumento]([EhFuncionario], [IdFuncionario])
    WHERE [EhFuncionario] = 1;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_PessoaDocumento_FornecedorColaborador]
    ON [dbo].[SGC_PessoaDocumento]([EhFuncionario], [IdFornecedorColaborador])
    WHERE [EhFuncionario] = 0;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_PessoaDocumento_Tipo_DataValidade]
    ON [dbo].[SGC_PessoaDocumento]([IdTipoDocumentoTerceiro], [DataValidade]);
    
    PRINT '  [OK] SGC_PessoaDocumento criada';
END
ELSE
    PRINT '  [--] SGC_PessoaDocumento já existe';
GO


/* =============================================================================
   SGC_ChecklistModelo
   Descrição: Modelos de checklist para aplicação em acessos à empresa.
              Contém versão e lista de itens em SGC_ChecklistItem.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_ChecklistModelo', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_ChecklistModelo](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdTipoChecklist]          INT          NOT NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Dados do Modelo
        [Nome]                     VARCHAR(100) NOT NULL,
        [Descricao]                VARCHAR(255) NULL,
        [Versao]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_ChecklistModelo_Versao] DEFAULT ('1.0'),
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_ChecklistModelo_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_ChecklistModelo_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_ChecklistModelo] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT '  [OK] SGC_ChecklistModelo criada';
END
ELSE
    PRINT '  [--] SGC_ChecklistModelo já existe';
GO


/* =============================================================================
   SGC_ChecklistItem
   Descrição: Itens (perguntas) de um modelo de checklist.
              Define tipo de resposta esperada e se é bloqueante.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_ChecklistItem', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_ChecklistItem](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdChecklistModelo]        INT          NOT NULL,
        
        -- Dados do Item
        [Ordem]                    INT          NOT NULL,
        [Codigo]                   VARCHAR(20)  NULL,
        [Pergunta]                 VARCHAR(500) NOT NULL,
        [TipoResposta]             VARCHAR(20)  NOT NULL,
        [RespostaEsperada]         VARCHAR(50)  NULL,
        [EhBloqueante]             BIT          NOT NULL CONSTRAINT [DF_SGC_ChecklistItem_Bloqueante] DEFAULT (0),
        [Observacao]               VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_ChecklistItem_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_ChecklistItem_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_ChecklistItem] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_ChecklistItem_TipoResposta] CHECK ([TipoResposta] IN ('SIM_NAO', 'SIM_NAO_NA', 'TEXTO', 'NUMERO', 'DATA'))
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_ChecklistItem_Modelo]
    ON [dbo].[SGC_ChecklistItem]([IdChecklistModelo], [Ordem]);
    
    PRINT '  [OK] SGC_ChecklistItem criada';
END
ELSE
    PRINT '  [--] SGC_ChecklistItem já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 3: TABELAS TRANSACIONAIS
-- #############################################################################

PRINT '>>> PARTE 3: TABELAS TRANSACIONAIS';
PRINT '';

/* =============================================================================
   SGC_RegistroAcesso
   Descrição: Registro de entrada e saída de pessoas na empresa.
              Controla protocolo, status, veículo, carga e pesagem.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_RegistroAcesso', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_RegistroAcesso](
        -- Chave Primária
        [Id]                       BIGINT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Protocolo e Status
        [NumeroProtocolo]          VARCHAR(20)  NOT NULL,
        [IdStatusAcesso]           INT          NOT NULL,
        [IdMotivoAcesso]           INT          NOT NULL,
        
        -- Pessoa (F=Funcionário, T=Terceiro, V=Visitante)
        [TipoPessoa]               CHAR(1)      NOT NULL,
        [IdFuncionario]            UNIQUEIDENTIFIER NULL,
        [IdFornecedorColaborador]  INT          NULL,
        [IdVisitante]              INT          NULL,
        
        -- Destino
        [SetorDestino]             VARCHAR(100) NULL,
        [PessoaContato]            VARCHAR(100) NULL,
        
        -- Veículo
        [PossuiVeiculo]            BIT          NOT NULL CONSTRAINT [DF_SGC_RegistroAcesso_PossuiVeiculo] DEFAULT (0),
        [IdVeiculo]                INT          NULL,
        [PlacaVeiculoAvulso]       CHAR(7)      NULL,
        
        -- Carga e Pesagem
        [PossuiCarga]              BIT          NOT NULL CONSTRAINT [DF_SGC_RegistroAcesso_PossuiCarga] DEFAULT (0),
        [DescricaoCarga]           VARCHAR(255) NULL,
        [PesoEntrada]              DECIMAL(10,2) NULL,
        [PesoSaida]                DECIMAL(10,2) NULL,
        [NotaFiscal]               VARCHAR(50)  NULL,
        
        -- Datas
        [DataHoraEntrada]          DATETIME2(0) NOT NULL,
        [DataHoraSaida]            DATETIME2(0) NULL,
        [DataHoraPrevisaoSaida]    DATETIME2(0) NULL,
        
        -- Outros
        [NumeroCrachaVisitante]    VARCHAR(20)  NULL,
        [Observacao]               VARCHAR(500) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_RegistroAcesso_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_RegistroAcesso_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_RegistroAcesso] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_RegistroAcesso_TipoPessoa] CHECK ([TipoPessoa] IN ('F', 'T', 'V'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_RegistroAcesso_Protocolo]
    ON [dbo].[SGC_RegistroAcesso]([NumeroProtocolo]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_RegistroAcesso_Data]
    ON [dbo].[SGC_RegistroAcesso]([DataHoraEntrada] DESC);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_RegistroAcesso_Status]
    ON [dbo].[SGC_RegistroAcesso]([IdStatusAcesso]) WHERE [Ativo] = 1;
    
    PRINT '  [OK] SGC_RegistroAcesso criada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso já existe';
GO


/* =============================================================================
   SGC_RegistroAcessoChecklist
   Descrição: Respostas do checklist aplicado em um registro de acesso.
              Cada registro é uma resposta a um item do checklist.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_RegistroAcessoChecklist', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_RegistroAcessoChecklist](
        -- Chave Primária
        [Id]                       BIGINT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdRegistroAcesso]         BIGINT       NOT NULL,
        [IdChecklistItem]          INT          NOT NULL,
        
        -- Resposta
        [Resposta]                 VARCHAR(255) NULL,
        [Conforme]                 BIT          NULL,
        [Observacao]               VARCHAR(255) NULL,
        
        -- Auditoria (simplificada)
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_RegistroAcessoChecklist_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_RegistroAcessoChecklist] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_RegistroAcessoChecklist_Registro]
    ON [dbo].[SGC_RegistroAcessoChecklist]([IdRegistroAcesso]);
    
    PRINT '  [OK] SGC_RegistroAcessoChecklist criada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcessoChecklist já existe';
GO


/* =============================================================================
   SGC_Autorizacao
   Descrição: Autorizações pré-aprovadas para acesso de terceiros ou visitantes.
              Define período de vigência e restrições de horário/dias.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Autorizacao', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Autorizacao](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Autorização
        [NumeroAutorizacao]        VARCHAR(20)  NOT NULL,
        
        -- Pessoa (T=Terceiro, V=Visitante)
        [TipoPessoa]               CHAR(1)      NOT NULL,
        [IdFornecedorColaborador]  INT          NULL,
        [IdVisitante]              INT          NULL,
        
        -- Vigência
        [DataInicio]               DATE         NOT NULL,
        [DataFim]                  DATE         NOT NULL,
        [HoraInicioPermitida]      TIME         NULL,
        [HoraFimPermitida]         TIME         NULL,
        [DiasPermitidos]           VARCHAR(20)  NULL,
        
        -- Aprovação
        [IdFuncionarioAprovador]   UNIQUEIDENTIFIER NULL,
        [DataAprovacao]            DATETIME2(0) NULL,
        [MotivoAutorizacao]        VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Autorizacao_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Autorizacao_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Autorizacao] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_Autorizacao_TipoPessoa] CHECK ([TipoPessoa] IN ('T', 'V'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_Autorizacao_Numero]
    ON [dbo].[SGC_Autorizacao]([NumeroAutorizacao]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Autorizacao_Vigencia]
    ON [dbo].[SGC_Autorizacao]([DataInicio], [DataFim]) WHERE [Ativo] = 1;
    
    PRINT '  [OK] SGC_Autorizacao criada';
END
ELSE
    PRINT '  [--] SGC_Autorizacao já existe';
GO


/* =============================================================================
   SGC_AlertaVencimento
   Descrição: Alertas automáticos de vencimento de documentos/treinamentos.
              Controla workflow de leitura e resolução do alerta.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_AlertaVencimento', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_AlertaVencimento](
        -- Chave Primária
        [Id]                       BIGINT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Tipo e Origem
        [TipoAlerta]               VARCHAR(20)  NOT NULL,
        [IdPessoaDocumento]        INT          NULL,
        [IdTreinamentoParticipante] INT         NULL,
        [IdAutorizacao]            INT          NULL,
        
        -- Pessoa (F=Funcionário, T=Terceiro, V=Visitante)
        [TipoPessoa]               CHAR(1)      NOT NULL,
        [IdFuncionario]            UNIQUEIDENTIFIER NULL,
        [IdFornecedorColaborador]  INT          NULL,
        [IdVisitante]              INT          NULL,
        
        -- Alerta
        [DataVencimento]           DATE         NOT NULL,
        [DiasParaVencimento]       INT          NOT NULL,
        [Descricao]                VARCHAR(255) NOT NULL,
        
        -- Workflow
        [Lido]                     BIT          NOT NULL CONSTRAINT [DF_SGC_AlertaVencimento_Lido] DEFAULT (0),
        [DataLeitura]              DATETIME2(0) NULL,
        [Aud_IdUsuarioLeitura]     UNIQUEIDENTIFIER NULL,
        [Resolvido]                BIT          NOT NULL CONSTRAINT [DF_SGC_AlertaVencimento_Resolvido] DEFAULT (0),
        [DataResolucao]            DATETIME2(0) NULL,
        [Aud_IdUsuarioResolucao]   UNIQUEIDENTIFIER NULL,
        [ObservacaoResolucao]      VARCHAR(255) NULL,
        
        -- Auditoria (simplificada)
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_AlertaVencimento_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        
        CONSTRAINT [PK_SGC_AlertaVencimento] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_AlertaVencimento_TipoAlerta] CHECK ([TipoAlerta] IN ('DOCUMENTO', 'TREINAMENTO', 'ASO', 'AUTORIZACAO')),
        CONSTRAINT [CK_SGC_AlertaVencimento_TipoPessoa] CHECK ([TipoPessoa] IN ('F', 'T', 'V'))
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_AlertaVencimento_NaoLidos]
    ON [dbo].[SGC_AlertaVencimento]([CdEmpresa], [CdFilial], [DataVencimento])
    WHERE [Lido] = 0 AND [Resolvido] = 0;
    
    PRINT '  [OK] SGC_AlertaVencimento criada';
END
ELSE
    PRINT '  [--] SGC_AlertaVencimento já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 4: FOREIGN KEYS
-- #############################################################################

PRINT '>>> PARTE 4: FOREIGN KEYS';
PRINT '';

-- -----------------------------------------------------------------------------
-- 4.1 FKs de Relacionamento entre Tabelas SGC
-- -----------------------------------------------------------------------------

-- SGC_FornecedorEmpresa
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_FornecedorEmpresa_TipoFornecedor')
    ALTER TABLE [dbo].[SGC_FornecedorEmpresa] ADD CONSTRAINT [FK_SGC_FornecedorEmpresa_TipoFornecedor]
    FOREIGN KEY ([IdTipoFornecedor]) REFERENCES [dbo].[SGC_TipoFornecedor]([Id]);
GO

-- SGC_FornecedorColaborador
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_FornecedorColaborador_FornecedorEmpresa')
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD CONSTRAINT [FK_SGC_FornecedorColaborador_FornecedorEmpresa]
    FOREIGN KEY ([IdFornecedorEmpresa]) REFERENCES [dbo].[SGC_FornecedorEmpresa]([Id]);
GO

-- SGC_Visitante
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Visitante_TipoPessoa')
    ALTER TABLE [dbo].[SGC_Visitante] ADD CONSTRAINT [FK_SGC_Visitante_TipoPessoa]
    FOREIGN KEY ([IdTipoPessoa]) REFERENCES [dbo].[SGC_TipoPessoa]([Id]);
GO

-- SGC_Veiculo
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Veiculo_TipoVeiculo')
    ALTER TABLE [dbo].[SGC_Veiculo] ADD CONSTRAINT [FK_SGC_Veiculo_TipoVeiculo]
    FOREIGN KEY ([IdTipoVeiculo]) REFERENCES [dbo].[SGC_TipoVeiculo]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Veiculo_FornecedorEmpresa')
    ALTER TABLE [dbo].[SGC_Veiculo] ADD CONSTRAINT [FK_SGC_Veiculo_FornecedorEmpresa]
    FOREIGN KEY ([IdFornecedorEmpresa]) REFERENCES [dbo].[SGC_FornecedorEmpresa]([Id]);
GO

-- SGC_TreinamentoTurma
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_TreinamentoTurma_TipoTreinamento')
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD CONSTRAINT [FK_SGC_TreinamentoTurma_TipoTreinamento]
    FOREIGN KEY ([IdTipoTreinamento]) REFERENCES [dbo].[SGC_TipoTreinamento]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_TreinamentoTurma_Instrutor')
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD CONSTRAINT [FK_SGC_TreinamentoTurma_Instrutor]
    FOREIGN KEY ([IdInstrutor]) REFERENCES [dbo].[SGC_Instrutor]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_TreinamentoTurma_LocalTreinamento')
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD CONSTRAINT [FK_SGC_TreinamentoTurma_LocalTreinamento]
    FOREIGN KEY ([IdLocalTreinamento]) REFERENCES [dbo].[SGC_LocalTreinamento]([Id]);
GO

-- SGC_TreinamentoParticipante
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_TreinamentoParticipante_Turma')
    ALTER TABLE [dbo].[SGC_TreinamentoParticipante] ADD CONSTRAINT [FK_SGC_TreinamentoParticipante_Turma]
    FOREIGN KEY ([IdTreinamentoTurma]) REFERENCES [dbo].[SGC_TreinamentoTurma]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_TreinamentoParticipante_FornecedorColaborador')
    ALTER TABLE [dbo].[SGC_TreinamentoParticipante] ADD CONSTRAINT [FK_SGC_TreinamentoParticipante_FornecedorColaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

-- SGC_PessoaDocumento
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_PessoaDocumento_TipoDocumento')
    ALTER TABLE [dbo].[SGC_PessoaDocumento] ADD CONSTRAINT [FK_SGC_PessoaDocumento_TipoDocumento]
    FOREIGN KEY ([IdTipoDocumentoTerceiro]) REFERENCES [dbo].[SGC_TipoDocumentoTerceiro]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_PessoaDocumento_FornecedorColaborador')
    ALTER TABLE [dbo].[SGC_PessoaDocumento] ADD CONSTRAINT [FK_SGC_PessoaDocumento_FornecedorColaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

-- SGC_ChecklistModelo
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_ChecklistModelo_TipoChecklist')
    ALTER TABLE [dbo].[SGC_ChecklistModelo] ADD CONSTRAINT [FK_SGC_ChecklistModelo_TipoChecklist]
    FOREIGN KEY ([IdTipoChecklist]) REFERENCES [dbo].[SGC_TipoChecklist]([Id]);
GO

-- SGC_ChecklistItem
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_ChecklistItem_Modelo')
    ALTER TABLE [dbo].[SGC_ChecklistItem] ADD CONSTRAINT [FK_SGC_ChecklistItem_Modelo]
    FOREIGN KEY ([IdChecklistModelo]) REFERENCES [dbo].[SGC_ChecklistModelo]([Id]);
GO

-- SGC_RegistroAcesso
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_Status')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_Status]
    FOREIGN KEY ([IdStatusAcesso]) REFERENCES [dbo].[SGC_StatusAcesso]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_MotivoAcesso')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_MotivoAcesso]
    FOREIGN KEY ([IdMotivoAcesso]) REFERENCES [dbo].[SGC_MotivoAcesso]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_Veiculo')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_Veiculo]
    FOREIGN KEY ([IdVeiculo]) REFERENCES [dbo].[SGC_Veiculo]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_Visitante')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_Visitante]
    FOREIGN KEY ([IdVisitante]) REFERENCES [dbo].[SGC_Visitante]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_FornecedorColaborador')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_FornecedorColaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

-- SGC_RegistroAcessoChecklist
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcessoChecklist_RegistroAcesso')
    ALTER TABLE [dbo].[SGC_RegistroAcessoChecklist] ADD CONSTRAINT [FK_SGC_RegistroAcessoChecklist_RegistroAcesso]
    FOREIGN KEY ([IdRegistroAcesso]) REFERENCES [dbo].[SGC_RegistroAcesso]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcessoChecklist_ChecklistItem')
    ALTER TABLE [dbo].[SGC_RegistroAcessoChecklist] ADD CONSTRAINT [FK_SGC_RegistroAcessoChecklist_ChecklistItem]
    FOREIGN KEY ([IdChecklistItem]) REFERENCES [dbo].[SGC_ChecklistItem]([Id]);
GO

-- SGC_Autorizacao
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Autorizacao_FornecedorColaborador')
    ALTER TABLE [dbo].[SGC_Autorizacao] ADD CONSTRAINT [FK_SGC_Autorizacao_FornecedorColaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Autorizacao_Visitante')
    ALTER TABLE [dbo].[SGC_Autorizacao] ADD CONSTRAINT [FK_SGC_Autorizacao_Visitante]
    FOREIGN KEY ([IdVisitante]) REFERENCES [dbo].[SGC_Visitante]([Id]);
GO

PRINT '  [OK] FKs de Relacionamento criadas';
PRINT '';

-- -----------------------------------------------------------------------------
-- 4.2 FKs de Auditoria (para tuse1.id)
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Auditoria (tuse1)...';

-- Helper: Criar FK de auditoria se a coluna existir
DECLARE @sql NVARCHAR(MAX);
DECLARE @tableName VARCHAR(100);
DECLARE @tables TABLE (TableName VARCHAR(100));

INSERT INTO @tables VALUES 
    ('SGC_TipoFornecedor'), ('SGC_TipoVeiculo'), ('SGC_TipoPessoa'), ('SGC_TipoAso'),
    ('SGC_MotivoAcesso'), ('SGC_StatusAcesso'), ('SGC_TipoChecklist'), ('SGC_TipoParentesco'),
    ('SGC_TipoDocumentoTerceiro'), ('SGC_TipoTreinamento'), ('SGC_FornecedorEmpresa'),
    ('SGC_FornecedorColaborador'), ('SGC_Visitante'), ('SGC_Instrutor'), ('SGC_LocalTreinamento'),
    ('SGC_Veiculo'), ('SGC_TreinamentoTurma'), ('SGC_TreinamentoParticipante'),
    ('SGC_PessoaDocumento'), ('SGC_ChecklistModelo'), ('SGC_ChecklistItem'),
    ('SGC_RegistroAcesso'), ('SGC_RegistroAcessoChecklist'), ('SGC_Autorizacao');

-- Criar FKs para Aud_IdUsuarioCadastro
DECLARE cur CURSOR FOR SELECT TableName FROM @tables;
OPEN cur;
FETCH NEXT FROM cur INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- FK para Aud_IdUsuarioCadastro
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tableName) AND name = 'Aud_IdUsuarioCadastro')
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_' + @tableName + '_UsuarioCadastro')
    BEGIN
        SET @sql = 'ALTER TABLE dbo.' + @tableName + ' ADD CONSTRAINT FK_' + @tableName + '_UsuarioCadastro FOREIGN KEY (Aud_IdUsuarioCadastro) REFERENCES dbo.tuse1(id);';
        BEGIN TRY
            EXEC sp_executesql @sql;
        END TRY
        BEGIN CATCH
            -- Ignora erro se tuse1 não existir
        END CATCH
    END
    
    -- FK para Aud_IdUsuarioAtualizacao
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tableName) AND name = 'Aud_IdUsuarioAtualizacao')
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_' + @tableName + '_UsuarioAtualizacao')
    BEGIN
        SET @sql = 'ALTER TABLE dbo.' + @tableName + ' ADD CONSTRAINT FK_' + @tableName + '_UsuarioAtualizacao FOREIGN KEY (Aud_IdUsuarioAtualizacao) REFERENCES dbo.tuse1(id);';
        BEGIN TRY
            EXEC sp_executesql @sql;
        END TRY
        BEGIN CATCH
            -- Ignora erro se tuse1 não existir
        END CATCH
    END
    
    FETCH NEXT FROM cur INTO @tableName;
END

CLOSE cur;
DEALLOCATE cur;

-- FK especial para SGC_AlertaVencimento
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AlertaVencimento_UsuarioLeitura')
BEGIN
    BEGIN TRY
        ALTER TABLE [dbo].[SGC_AlertaVencimento] ADD CONSTRAINT [FK_SGC_AlertaVencimento_UsuarioLeitura]
        FOREIGN KEY ([Aud_IdUsuarioLeitura]) REFERENCES [dbo].[tuse1]([id]);
    END TRY
    BEGIN CATCH END CATCH
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AlertaVencimento_UsuarioResolucao')
BEGIN
    BEGIN TRY
        ALTER TABLE [dbo].[SGC_AlertaVencimento] ADD CONSTRAINT [FK_SGC_AlertaVencimento_UsuarioResolucao]
        FOREIGN KEY ([Aud_IdUsuarioResolucao]) REFERENCES [dbo].[tuse1]([id]);
    END TRY
    BEGIN CATCH END CATCH
END

PRINT '  [OK] FKs de Auditoria criadas (ou tuse1 não disponível)';
PRINT '';


GO

-- =============================================================================
-- Documentação: Campos de Auditoria (padrão)
-- =============================================================================
-- Aplicar a todas as tabelas que têm os campos de auditoria

DECLARE @tbl VARCHAR(100);
DECLARE @tables2 TABLE (TableName VARCHAR(100));

INSERT INTO @tables2 VALUES 
    ('SGC_TipoAso'), ('SGC_MotivoAcesso'), ('SGC_StatusAcesso'), ('SGC_TipoChecklist'),
    ('SGC_TipoParentesco'), ('SGC_TipoDocumentoTerceiro'), ('SGC_TipoTreinamento'),
    ('SGC_Visitante'), ('SGC_Instrutor'), ('SGC_LocalTreinamento'), ('SGC_Veiculo'),
    ('SGC_TreinamentoTurma'), ('SGC_TreinamentoParticipante'), ('SGC_PessoaDocumento'),
    ('SGC_ChecklistModelo'), ('SGC_ChecklistItem'), ('SGC_RegistroAcessoChecklist'),
    ('SGC_Autorizacao'), ('SGC_AlertaVencimento');

DECLARE cur2 CURSOR FOR SELECT TableName FROM @tables2;
OPEN cur2;
FETCH NEXT FROM cur2 INTO @tbl;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tbl) AND name = 'Aud_CreatedAt')
        EXEC #AddExtProp @tbl, 'Aud_CreatedAt', 'Data e hora UTC de criação do registro.';
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tbl) AND name = 'Aud_UpdatedAt')
        EXEC #AddExtProp @tbl, 'Aud_UpdatedAt', 'Data e hora UTC da última atualização do registro.';
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tbl) AND name = 'Aud_IdUsuarioCadastro')
        EXEC #AddExtProp @tbl, 'Aud_IdUsuarioCadastro', 'ID do usuário (tuse1.id) que criou o registro.';
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tbl) AND name = 'Aud_IdUsuarioAtualizacao')
        EXEC #AddExtProp @tbl, 'Aud_IdUsuarioAtualizacao', 'ID do usuário (tuse1.id) que realizou a última atualização.';
    
    FETCH NEXT FROM cur2 INTO @tbl;
END

CLOSE cur2;
DEALLOCATE cur2;
GO

DROP PROCEDURE #AddExtProp;
GO

PRINT '  [OK] Extended Properties criadas';
PRINT '';


-- #############################################################################
-- RESUMO FINAL
-- #############################################################################

PRINT '=============================================================================';
PRINT 'SCRIPT EXECUTADO COM SUCESSO!';
PRINT 'Término: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '=============================================================================';
PRINT '';
PRINT 'TABELAS AUXILIARES (LOOKUPS): 11';
PRINT '  - SGC_TipoFornecedor, SGC_TipoVeiculo, SGC_TipoPessoa, SGC_TipoAso';
PRINT '  - SGC_MotivoAcesso, SGC_StatusAcesso, SGC_TipoChecklist';
PRINT '  - SGC_TipoParentesco, SGC_TipoSanguineo';
PRINT '  - SGC_TipoDocumentoTerceiro, SGC_TipoTreinamento';
PRINT '';
PRINT 'TABELAS DE CADASTRO: 11';
PRINT '  - SGC_FornecedorEmpresa, SGC_FornecedorColaborador, SGC_Visitante';
PRINT '  - SGC_Instrutor, SGC_LocalTreinamento, SGC_Veiculo';
PRINT '  - SGC_TreinamentoTurma, SGC_TreinamentoParticipante, SGC_PessoaDocumento';
PRINT '  - SGC_ChecklistModelo, SGC_ChecklistItem';
PRINT '';
PRINT 'TABELAS TRANSACIONAIS: 4';
PRINT '  - SGC_RegistroAcesso, SGC_RegistroAcessoChecklist';
PRINT '  - SGC_Autorizacao, SGC_AlertaVencimento';
PRINT '';
PRINT 'TOTAL: 26 TABELAS';
PRINT '';
PRINT 'PADRÃO DE AUDITORIA:';
PRINT '  - Aud_CreatedAt            : DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME()';
PRINT '  - Aud_UpdatedAt            : DATETIME2(0) NULL';
PRINT '  - Aud_IdUsuarioCadastro    : UNIQUEIDENTIFIER NULL → FK tuse1.id';
PRINT '  - Aud_IdUsuarioAtualizacao : UNIQUEIDENTIFIER NULL → FK tuse1.id';
PRINT '=============================================================================';
GO



--tabela de usuario
select id  from tuse1  guid  ex: B4E0D1F0-5B4C-EB11-9402-005056BD6616

--tabela de empresa
select id from temp1  guid  ex: B4E0D1F0-5B4C-EB11-9402-005056BD6616 

--tabela de filial
select id from test1  int

--tabela de empregado
select id from func1 guid


