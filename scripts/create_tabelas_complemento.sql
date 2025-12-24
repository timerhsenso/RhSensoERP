USE [bd_rhu_copenor];
GO

/* =============================================================================
   ███████╗ ██████╗  ██████╗    ██╗   ██╗██████╗    ██████╗ 
   ██╔════╝██╔════╝ ██╔════╝    ██║   ██║╚════██╗  ██╔═████╗
   ███████╗██║  ███╗██║         ██║   ██║ █████╔╝  ██║██╔██║
   ╚════██║██║   ██║██║         ╚██╗ ██╔╝ ╚═══██╗  ████╔╝██║
   ███████║╚██████╔╝╚██████╗     ╚████╔╝ ██████╔╝  ╚██████╔╝
   ╚══════╝ ╚═════╝  ╚═════╝      ╚═══╝  ╚═════╝    ╚═════╝ 
   
   SISTEMA DE GESTÃO CORPORATIVA - SCRIPT COMPLEMENTAR
   ====================================================
   
   Versão: 2.0
   Data: Dezembro/2025
   Autor: RhSensoERP
   Banco: SQL Server 2019+
   
   DESCRIÇÃO:
   Script complementar com tabelas novas para:
   - Controle de Portarias
   - Configurações Multi-Tenant
   - Gestão de Contratos
   - Agendamento de Cargas
   - Recebimento e Conferência
   - Registro de Ocorrências
   - Controle de Crachás
   
   PRÉ-REQUISITO:
   Executar primeiro o script create_tabelas.sql (v3.0)
   
   PADRÃO DE AUDITORIA:
   - Aud_CreatedAt          : Data/hora de criação (UTC)
   - Aud_UpdatedAt          : Data/hora de atualização (UTC)
   - Aud_IdUsuarioCadastro  : FK → tuse1.id (usuário que criou)
   - Aud_IdUsuarioAtualizacao: FK → tuse1.id (usuário que atualizou)
   
   ORDEM DE EXECUÇÃO:
   1. Tabelas de Configuração (Portaria, Config, Motivos)
   2. Tabelas de Contratos
   3. Tabelas de Agendamento
   4. Tabelas de Recebimento
   5. Tabelas de Ocorrências e Crachás
   6. Alterações em Tabelas Existentes
   7. Foreign Keys
   8. Extended Properties (Documentação)
   
============================================================================= */

SET NOCOUNT ON;
PRINT '=============================================================================';
PRINT 'SGC - SCRIPT COMPLEMENTAR v2.0';
PRINT 'Início: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '=============================================================================';
PRINT '';


-- #############################################################################
-- PARTE 1: TABELAS DE CONFIGURAÇÃO
-- #############################################################################

PRINT '>>> PARTE 1: TABELAS DE CONFIGURAÇÃO';
PRINT '';


/* =============================================================================
   SGC_Portaria
   Descrição: Cadastro de portarias/guaritas da empresa.
              Indústrias geralmente têm portaria principal e de carga.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Portaria', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Portaria](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Dados da Portaria
        [Codigo]                   VARCHAR(20)  NOT NULL,
        [Nome]                     VARCHAR(100) NOT NULL,
        [Tipo]                     VARCHAR(20)  NOT NULL,
        [LocalizacaoDescricao]     VARCHAR(200) NULL,
        [Latitude]                 DECIMAL(10,8) NULL,
        [Longitude]                DECIMAL(11,8) NULL,
        
        -- Horário de Funcionamento
        [HorarioAbertura]          TIME         NULL,
        [HorarioFechamento]        TIME         NULL,
        [Funciona24Horas]          BIT          NOT NULL CONSTRAINT [DF_SGC_Portaria_24Horas] DEFAULT (0),
        [FuncionaFimDeSemana]      BIT          NOT NULL CONSTRAINT [DF_SGC_Portaria_FimSemana] DEFAULT (0),
        
        -- Permissões
        [PermiteVeiculos]          BIT          NOT NULL CONSTRAINT [DF_SGC_Portaria_Veiculos] DEFAULT (1),
        [PermitePedestres]         BIT          NOT NULL CONSTRAINT [DF_SGC_Portaria_Pedestres] DEFAULT (1),
        [PermiteCarga]             BIT          NOT NULL CONSTRAINT [DF_SGC_Portaria_Carga] DEFAULT (0),
        
        -- Contato
        [Telefone]                 VARCHAR(20)  NULL,
        [Ramal]                    VARCHAR(10)  NULL,
        
        -- Controle
        [Observacao]               VARCHAR(500) NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_Portaria_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Portaria_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Portaria_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Portaria] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_Portaria_Tipo] CHECK ([Tipo] IN ('PRINCIPAL', 'CARGA', 'EMERGENCIA', 'PEDESTRES', 'OUTRO'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_Portaria_Codigo]
    ON [dbo].[SGC_Portaria]([CdEmpresa], [CdFilial], [Codigo]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Portaria_Filial]
    ON [dbo].[SGC_Portaria]([CdEmpresa], [CdFilial]) WHERE [Ativo] = 1;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_Portaria] ([CdEmpresa], [CdFilial], [Codigo], [Nome], [Tipo], [PermiteVeiculos], [PermitePedestres], [PermiteCarga], [Funciona24Horas], [Ordem])
    VALUES 
        (1, 1, 'PORT01', 'Portaria Principal', 'PRINCIPAL', 1, 1, 0, 0, 1),
        (1, 1, 'PORT02', 'Portaria de Carga', 'CARGA', 1, 0, 1, 0, 2),
        (1, 1, 'PORT03', 'Portaria de Emergência', 'EMERGENCIA', 1, 1, 0, 1, 3);
    
    PRINT '  [OK] SGC_Portaria criada';
END
ELSE
    PRINT '  [--] SGC_Portaria já existe';
GO


/* =============================================================================
   SGC_ConfiguracaoFilial
   Descrição: Parâmetros gerais de funcionamento por empresa/filial.
              Define comportamentos padrão do sistema (multi-tenant configurável).
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_ConfiguracaoFilial', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_ConfiguracaoFilial](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Configurações de Agendamento
        [ExigeAgendamentoCarga]    BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_ExigeAgend] DEFAULT (0),
        [DiasAntecedenciaMinima]   INT          NULL,
        [PermiteAgendamentoMesmoDia] BIT        NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_AgendMesmoDia] DEFAULT (1),
        [ExigeAprovacaoAgendamento] BIT         NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_AprovAgend] DEFAULT (0),
        
        -- Configurações de Acesso
        [ExigeFotoEntrada]         BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_Foto] DEFAULT (0),
        [ExigeBiometriaEntrada]    BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_Biometria] DEFAULT (0),
        [TempoMaximoPermanenciaHoras] INT       NULL,
        [BloqueiaDocumentoVencido] BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_BloqDocVenc] DEFAULT (1),
        [DiasAlertaVencimento]     INT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_DiasAlerta] DEFAULT (30),
        
        -- Configurações de Carga
        [PermiteRecusaParcial]     BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_RecusaParcial] DEFAULT (1),
        [ExigePesagemEntrada]      BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_PesagemEnt] DEFAULT (0),
        [ExigePesagemSaida]        BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_PesagemSai] DEFAULT (0),
        
        -- Configurações de Crachá
        [UsaCrachaProvisorio]      BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_Cracha] DEFAULT (1),
        [ExigeDevolucaoCracha]     BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_DevCracha] DEFAULT (1),
        
        -- Horários de Carga
        [HorarioInicioCarga]       TIME         NULL,
        [HorarioFimCarga]          TIME         NULL,
        [PermiteCargaFimDeSemana]  BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigFilial_CargaFimSem] DEFAULT (0),
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_ConfiguracaoFilial_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_ConfiguracaoFilial_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_ConfiguracaoFilial] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_ConfiguracaoFilial_Filial]
    ON [dbo].[SGC_ConfiguracaoFilial]([CdEmpresa], [CdFilial]);
    
    -- Configuração padrão
    INSERT INTO [dbo].[SGC_ConfiguracaoFilial] 
        ([CdEmpresa], [CdFilial], [DiasAlertaVencimento], [HorarioInicioCarga], [HorarioFimCarga])
    VALUES 
        (1, 1, 30, '07:00', '17:00');
    
    PRINT '  [OK] SGC_ConfiguracaoFilial criada';
END
ELSE
    PRINT '  [--] SGC_ConfiguracaoFilial já existe';
GO


/* =============================================================================
   SGC_ConfiguracaoObrigatoriedade
   Descrição: Regras de validação configuráveis por empresa.
              Cada empresa define o que é obrigatório para ela (multi-tenant).
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_ConfiguracaoObrigatoriedade', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_ConfiguracaoObrigatoriedade](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        
        -- Regra
        [Contexto]                 VARCHAR(30)  NOT NULL,
        [CodigoValidacao]          VARCHAR(30)  NOT NULL,
        [Descricao]                VARCHAR(150) NOT NULL,
        
        -- Configuração
        [EhObrigatorio]            BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigObrig_Obrigatorio] DEFAULT (0),
        [BloqueiaSeNaoAtender]     BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigObrig_Bloqueia] DEFAULT (0),
        [ValidadeMinimaEmDias]     INT          NULL,
        [MensagemBloqueio]         VARCHAR(255) NULL,
        
        -- Controle
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_ConfigObrig_Ordem] DEFAULT (0),
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_ConfigObrig_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_ConfigObrig_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_ConfiguracaoObrigatoriedade] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_ConfigObrig_Contexto] CHECK ([Contexto] IN (
            'ACESSO_TERCEIRO', 'ACESSO_VISITANTE', 'ACESSO_MOTORISTA', 
            'CARGA_RECEBIMENTO', 'CARGA_PERIGOSA', 'VEICULO_ENTRADA'
        )),
        CONSTRAINT [CK_SGC_ConfigObrig_Validacao] CHECK ([CodigoValidacao] IN (
            'CNH', 'CNH_CATEGORIA', 'ASO', 'MOPP', 'NR06', 'NR10', 'NR12', 'NR20', 'NR33', 'NR35',
            'INTEGRACAO', 'FOTO', 'FISPQ', 'CONTRATO_ATIVO', 'AGENDAMENTO_PREVIO', 'CRLV'
        ))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_ConfigObrig_Regra]
    ON [dbo].[SGC_ConfiguracaoObrigatoriedade]([CdEmpresa], [CdFilial], [Contexto], [CodigoValidacao]);
    
    -- Configurações padrão para empresa 1 (exemplo: indústria química)
    INSERT INTO [dbo].[SGC_ConfiguracaoObrigatoriedade] 
        ([CdEmpresa], [CdFilial], [Contexto], [CodigoValidacao], [Descricao], [EhObrigatorio], [BloqueiaSeNaoAtender], [ValidadeMinimaEmDias], [Ordem])
    VALUES 
        -- Terceiros
        (1, 1, 'ACESSO_TERCEIRO', 'ASO', 'Atestado de Saúde Ocupacional', 1, 1, 30, 1),
        (1, 1, 'ACESSO_TERCEIRO', 'INTEGRACAO', 'Integração de Segurança', 1, 1, NULL, 2),
        (1, 1, 'ACESSO_TERCEIRO', 'CONTRATO_ATIVO', 'Contrato Ativo', 1, 1, NULL, 3),
        -- Motoristas
        (1, 1, 'ACESSO_MOTORISTA', 'CNH', 'Carteira Nacional de Habilitação', 1, 1, 30, 1),
        (1, 1, 'ACESSO_MOTORISTA', 'ASO', 'Atestado de Saúde Ocupacional', 1, 1, 30, 2),
        -- Carga Perigosa
        (1, 1, 'CARGA_PERIGOSA', 'MOPP', 'Movimentação Produtos Perigosos', 1, 1, 30, 1),
        (1, 1, 'CARGA_PERIGOSA', 'NR20', 'Treinamento NR-20 (Inflamáveis)', 1, 1, NULL, 2),
        (1, 1, 'CARGA_PERIGOSA', 'FISPQ', 'Ficha de Segurança do Produto', 1, 1, NULL, 3),
        -- Veículos
        (1, 1, 'VEICULO_ENTRADA', 'CRLV', 'Documento do Veículo (CRLV)', 0, 0, NULL, 1);
    
    PRINT '  [OK] SGC_ConfiguracaoObrigatoriedade criada';
END
ELSE
    PRINT '  [--] SGC_ConfiguracaoObrigatoriedade já existe';
GO


/* =============================================================================
   SGC_MotivoRecusa
   Descrição: Motivos padronizados para recusa de acesso, carga ou veículo.
              Permite configurar consequências automáticas (bloqueio, ocorrência).
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_MotivoRecusa', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_MotivoRecusa](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        
        -- Dados do Motivo
        [Codigo]                   VARCHAR(30)  NOT NULL,
        [Descricao]                VARCHAR(150) NOT NULL,
        [Tipo]                     VARCHAR(20)  NOT NULL,
        
        -- Comportamento
        [ExigeObservacao]          BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoRecusa_ExigeObs] DEFAULT (0),
        [GeraOcorrencia]           BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoRecusa_GeraOcorr] DEFAULT (0),
        [GeraBloqueioTemporario]   BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoRecusa_GeraBloq] DEFAULT (0),
        [DiasBloqueio]             INT          NULL,
        
        -- Apresentação
        [Icone]                    VARCHAR(50)  NULL,
        [CorHex]                   CHAR(7)      NULL,
        [Ordem]                    INT          NOT NULL CONSTRAINT [DF_SGC_MotivoRecusa_Ordem] DEFAULT (0),
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_MotivoRecusa_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_MotivoRecusa_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_MotivoRecusa] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_MotivoRecusa_Tipo] CHECK ([Tipo] IN ('ACESSO', 'CARGA', 'VEICULO', 'DOCUMENTO'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_MotivoRecusa_Codigo]
    ON [dbo].[SGC_MotivoRecusa]([Codigo]) WHERE [IdSaas] IS NULL;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_MotivoRecusa_Tipo]
    ON [dbo].[SGC_MotivoRecusa]([Tipo]) WHERE [Ativo] = 1;
    
    -- Dados Iniciais
    INSERT INTO [dbo].[SGC_MotivoRecusa] ([Codigo], [Descricao], [Tipo], [ExigeObservacao], [GeraOcorrencia], [GeraBloqueioTemporario], [DiasBloqueio], [CorHex], [Ordem])
    VALUES 
        -- Acesso
        ('DOC_VENCIDO', 'Documentação vencida', 'ACESSO', 0, 0, 0, NULL, '#FFA500', 1),
        ('DOC_INVALIDO', 'Documentação inválida/ilegível', 'ACESSO', 1, 0, 0, NULL, '#FFA500', 2),
        ('SEM_AGENDAMENTO', 'Sem agendamento prévio', 'ACESSO', 0, 0, 0, NULL, '#FF6600', 3),
        ('FORA_HORARIO', 'Fora do horário permitido', 'ACESSO', 0, 0, 0, NULL, '#FF6600', 4),
        ('CONTRATO_INATIVO', 'Contrato não ativo', 'ACESSO', 0, 0, 0, NULL, '#FF0000', 5),
        ('BLOQUEIO_ANTERIOR', 'Bloqueado por ocorrência anterior', 'ACESSO', 0, 1, 0, NULL, '#FF0000', 6),
        ('NAO_AUTORIZADO', 'Pessoa não autorizada', 'ACESSO', 1, 1, 0, NULL, '#FF0000', 7),
        -- Carga
        ('CARGA_DESACORDO', 'Carga em desacordo com pedido', 'CARGA', 1, 0, 0, NULL, '#FFA500', 10),
        ('CARGA_DANIFICADA', 'Embalagem/produto danificado', 'CARGA', 1, 0, 0, NULL, '#FF6600', 11),
        ('CARGA_VENCIDA', 'Produto fora da validade', 'CARGA', 1, 1, 0, NULL, '#FF0000', 12),
        ('CARGA_SEM_FISPQ', 'Sem ficha de segurança (FISPQ)', 'CARGA', 0, 0, 0, NULL, '#FF0000', 13),
        ('CARGA_QUANTIDADE', 'Quantidade incorreta', 'CARGA', 1, 0, 0, NULL, '#FFA500', 14),
        ('CHECKLIST_REPROVADO', 'Reprovado no checklist de segurança', 'CARGA', 1, 1, 0, NULL, '#FF0000', 15),
        -- Veículo
        ('VEICULO_IRREGULAR', 'Veículo em condições inadequadas', 'VEICULO', 1, 1, 0, NULL, '#FF0000', 20),
        ('VEICULO_SEM_CRLV', 'Sem documentação do veículo', 'VEICULO', 0, 0, 0, NULL, '#FFA500', 21),
        ('MOTORISTA_SEM_CNH', 'Motorista sem CNH válida', 'VEICULO', 0, 0, 0, NULL, '#FF0000', 22),
        ('MOTORISTA_SEM_MOPP', 'Motorista sem MOPP (carga perigosa)', 'VEICULO', 0, 0, 0, NULL, '#FF0000', 23),
        -- Documento
        ('DOC_NAO_APRESENTADO', 'Documento não apresentado', 'DOCUMENTO', 0, 0, 0, NULL, '#FFA500', 30),
        ('DOC_ADULTERADO', 'Documento com indícios de adulteração', 'DOCUMENTO', 1, 1, 1, 30, '#FF0000', 31);
    
    PRINT '  [OK] SGC_MotivoRecusa criada';
END
ELSE
    PRINT '  [--] SGC_MotivoRecusa já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 2: TABELAS DE CONTRATOS
-- #############################################################################

PRINT '>>> PARTE 2: TABELAS DE CONTRATOS';
PRINT '';


/* =============================================================================
   SGC_Contrato
   Descrição: Contratos com empresas fornecedoras.
              Terceiro só pode entrar se tiver contrato ativo.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Contrato', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Contrato](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        [IdFornecedorEmpresa]      INT          NOT NULL,
        
        -- Dados do Contrato
        [NumeroContrato]           VARCHAR(50)  NOT NULL,
        [Descricao]                VARCHAR(255) NULL,
        [Objeto]                   VARCHAR(500) NULL,
        
        -- Vigência
        [DataInicio]               DATE         NOT NULL,
        [DataFim]                  DATE         NOT NULL,
        [DataAssinatura]           DATE         NULL,
      
        -- Status e Regras
        [Status]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_Contrato_Status] DEFAULT ('ATIVO'),
        [PermiteAcessoSemAgendamento] BIT       NOT NULL CONSTRAINT [DF_SGC_Contrato_SemAgend] DEFAULT (0),
        [QuantidadeMaximaColaboradores] INT     NULL,
        
        -- Controle
        [Observacao]               VARCHAR(500) NULL,
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Contrato_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Contrato_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Contrato] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_Contrato_Status] CHECK ([Status] IN ('ATIVO', 'SUSPENSO', 'ENCERRADO', 'CANCELADO'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_Contrato_Numero]
    ON [dbo].[SGC_Contrato]([CdEmpresa], [CdFilial], [NumeroContrato]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Contrato_Fornecedor]
    ON [dbo].[SGC_Contrato]([IdFornecedorEmpresa]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Contrato_Vigencia]
    ON [dbo].[SGC_Contrato]([Status], [DataFim]) WHERE [Ativo] = 1;
    
    PRINT '  [OK] SGC_Contrato criada';
END
ELSE
    PRINT '  [--] SGC_Contrato já existe';
GO


/* =============================================================================
   SGC_ContratoResponsavel
   Descrição: Responsáveis/gestores do contrato.
              Permite múltiplos responsáveis com tipos diferentes (gestor, suplente, etc).
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_ContratoResponsavel', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_ContratoResponsavel](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdContrato]               INT          NOT NULL,
        [IdFuncionario]            UNIQUEIDENTIFIER NOT NULL,
        
        -- Tipo de Responsabilidade
        [TipoResponsavel]          VARCHAR(20)  NOT NULL,
        [DataInicio]               DATE         NOT NULL,
        [DataFim]                  DATE         NULL,
        
        -- Permissões
        [PodeAprovarAgendamento]   BIT          NOT NULL CONSTRAINT [DF_SGC_ContratoResp_AprovaAgend] DEFAULT (0),
        [PodeAutorizarAcesso]      BIT          NOT NULL CONSTRAINT [DF_SGC_ContratoResp_AutorizaAcesso] DEFAULT (0),
        [RecebeAlertaVencimento]   BIT          NOT NULL CONSTRAINT [DF_SGC_ContratoResp_RecebeAlerta] DEFAULT (1),
        
        -- Controle
        [Observacao]               VARCHAR(255) NULL,
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_ContratoResponsavel_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_ContratoResponsavel_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_ContratoResponsavel] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_ContratoResp_Tipo] CHECK ([TipoResponsavel] IN ('GESTOR', 'SUPLENTE', 'FISCAL', 'TECNICO'))
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_ContratoResponsavel_Contrato]
    ON [dbo].[SGC_ContratoResponsavel]([IdContrato]) WHERE [Ativo] = 1;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_ContratoResponsavel_Funcionario]
    ON [dbo].[SGC_ContratoResponsavel]([IdFuncionario]) WHERE [Ativo] = 1;
    
    PRINT '  [OK] SGC_ContratoResponsavel criada';
END
ELSE
    PRINT '  [--] SGC_ContratoResponsavel já existe';
GO


/* =============================================================================
   SGC_ContratoColaborador
   Descrição: Colaboradores terceiros vinculados a um contrato específico.
              Define quem do fornecedor pode acessar via este contrato.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_ContratoColaborador', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_ContratoColaborador](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdContrato]               INT          NOT NULL,
        [IdFornecedorColaborador]  INT          NOT NULL,
        
        -- Dados no Contrato
        [FuncaoNoContrato]         VARCHAR(80)  NULL,
        [DataInicio]               DATE         NOT NULL,
        [DataFim]                  DATE         NULL,
        
        -- Horário Padrão
        [HorarioEntradaPadrao]     TIME         NULL,
        [HorarioSaidaPadrao]       TIME         NULL,
        [DiasPermitidos]           VARCHAR(20)  NULL,
        
        -- Controle
        [Observacao]               VARCHAR(255) NULL,
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_ContratoColaborador_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_ContratoColaborador_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_ContratoColaborador] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_ContratoColaborador_Vinculo]
    ON [dbo].[SGC_ContratoColaborador]([IdContrato], [IdFornecedorColaborador]) WHERE [Ativo] = 1;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_ContratoColaborador_Colaborador]
    ON [dbo].[SGC_ContratoColaborador]([IdFornecedorColaborador]);
    
    PRINT '  [OK] SGC_ContratoColaborador criada';
END
ELSE
    PRINT '  [--] SGC_ContratoColaborador já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 3: TABELAS DE AGENDAMENTO
-- #############################################################################

PRINT '>>> PARTE 3: TABELAS DE AGENDAMENTO';
PRINT '';


/* =============================================================================
   SGC_AgendamentoCarga
   Descrição: Pré-aviso de entregas e cargas.
              Permite que a portaria saiba antecipadamente o que vai chegar.
              Suporta workflow de aprovação configurável.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_AgendamentoCarga', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_AgendamentoCarga](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        [IdPortaria]               INT          NULL,
        
        -- Identificação
        [NumeroAgendamento]        VARCHAR(20)  NOT NULL,
        
        -- Origem
        [IdContrato]               INT          NULL,
        [IdFornecedorEmpresa]      INT          NULL,
        [FornecedorAvulso]         VARCHAR(150) NULL,
        
        -- Previsão
        [DataPrevista]             DATE         NOT NULL,
        [HoraInicioPrevista]       TIME         NULL,
        [HoraFimPrevista]          TIME         NULL,
        
        -- Tipo de Entrega
        [TipoEntrega]              VARCHAR(20)  NOT NULL,
        [TipoCarga]                VARCHAR(20)  NULL,
        
        -- Motorista Previsto
        [IdFornecedorColaborador]  INT          NULL,
        [MotoristaPrevistoNome]    VARCHAR(100) NULL,
        [MotoristaPrevistoCpf]     CHAR(11)     NULL,
        
        -- Veículo Previsto
        [IdVeiculo]                INT          NULL,
        [VeiculoPrevistoPlaca]     CHAR(7)      NULL,
        [VeiculoPrevistoTipo]      VARCHAR(50)  NULL,
        
        -- Destino
        [SetorDestino]             VARCHAR(100) NULL,
        [LocalDescarga]            VARCHAR(100) NULL,
        
        -- Solicitante/Responsável
        [IdFuncionarioSolicitante] UNIQUEIDENTIFIER NOT NULL,
        [IdFuncionarioRecebedor]   UNIQUEIDENTIFIER NULL,
        
        -- Checklist
        [IdChecklistModelo]        INT          NULL,
        [ExigeChecklist]           BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCarga_ExigeCheck] DEFAULT (0),
        
        -- Instruções para Portaria
        [ObservacaoPortaria]       VARCHAR(500) NULL,
        [AlertaEspecial]           VARCHAR(255) NULL,
        [CorAlerta]                CHAR(7)      NULL,
        
        -- Workflow
        [Status]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_AgendCarga_Status] DEFAULT ('RASCUNHO'),
        [ExigeAprovacao]           BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCarga_ExigeAprov] DEFAULT (0),
        [DataAprovacao]            DATETIME2(0) NULL,
        [IdFuncionarioAprovador]   UNIQUEIDENTIFIER NULL,
        [MotivoRejeicao]           VARCHAR(255) NULL,
        
        -- Execução
        [IdRegistroAcesso]         BIGINT       NULL,
        [DataHoraChegada]          DATETIME2(0) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_AgendamentoCarga_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_AgendamentoCarga_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_AgendamentoCarga] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_AgendCarga_TipoEntrega] CHECK ([TipoEntrega] IN ('CARGA', 'ENTREGA_SIMPLES', 'COLETA', 'DEVOLUCAO')),
        CONSTRAINT [CK_SGC_AgendCarga_TipoCarga] CHECK ([TipoCarga] IS NULL OR [TipoCarga] IN ('NORMAL', 'QUIMICA', 'INFLAMAVEL', 'FRAGIL', 'REFRIGERADA', 'PERIGOSA')),
        CONSTRAINT [CK_SGC_AgendCarga_Status] CHECK ([Status] IN (
            'RASCUNHO', 'PENDENTE_APROVACAO', 'APROVADO', 'REJEITADO', 
            'EM_ANDAMENTO', 'CONCLUIDO', 'CONCLUIDO_PARCIAL', 'CANCELADO', 'NAO_COMPARECEU'
        ))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_AgendamentoCarga_Numero]
    ON [dbo].[SGC_AgendamentoCarga]([NumeroAgendamento]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_AgendamentoCarga_Data]
    ON [dbo].[SGC_AgendamentoCarga]([CdEmpresa], [CdFilial], [DataPrevista], [Status]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_AgendamentoCarga_Portaria]
    ON [dbo].[SGC_AgendamentoCarga]([IdPortaria], [DataPrevista]) WHERE [Status] IN ('APROVADO', 'EM_ANDAMENTO');
    
    CREATE NONCLUSTERED INDEX [IX_SGC_AgendamentoCarga_Contrato]
    ON [dbo].[SGC_AgendamentoCarga]([IdContrato]) WHERE [IdContrato] IS NOT NULL;
    
    PRINT '  [OK] SGC_AgendamentoCarga criada';
END
ELSE
    PRINT '  [--] SGC_AgendamentoCarga já existe';
GO


/* =============================================================================
   SGC_AgendamentoCargaProduto
   Descrição: Itens/produtos previstos no agendamento de carga.
              Permite controle detalhado do que será recebido, incluindo FISPQ.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_AgendamentoCargaProduto', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_AgendamentoCargaProduto](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdAgendamentoCarga]       INT          NOT NULL,
        
        -- Dados do Produto
        [Ordem]                    INT          NOT NULL,
        [CodigoProduto]            VARCHAR(50)  NULL,
        [DescricaoProduto]         VARCHAR(200) NOT NULL,
        [QuantidadePrevista]       DECIMAL(15,4) NOT NULL,
        [UnidadeMedida]            VARCHAR(20)  NOT NULL,
        
        -- Produto Controlado
        [EhProdutoQuimico]         BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCargaProd_Quimico] DEFAULT (0),
        [EhProdutoInflamavel]      BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCargaProd_Inflamavel] DEFAULT (0),
        [EhProdutoControlado]      BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCargaProd_Controlado] DEFAULT (0),
        [NumeroONU]                VARCHAR(10)  NULL,
        [ClasseRisco]              VARCHAR(20)  NULL,
        
        -- FISPQ
        [ExigeFISPQ]               BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCargaProd_ExigeFISPQ] DEFAULT (0),
        [FISPQPath]                VARCHAR(255) NULL,
        [FISPQValidada]            BIT          NOT NULL CONSTRAINT [DF_SGC_AgendCargaProd_FISPQValid] DEFAULT (0),
        
        -- Outros
        [PesoEstimadoKg]           DECIMAL(15,4) NULL,
        [ValorUnitario]            DECIMAL(15,4) NULL,
        [Observacao]               VARCHAR(255) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_AgendamentoCargaProduto_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_AgendamentoCargaProduto_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_AgendamentoCargaProduto] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_AgendamentoCargaProduto_Agend]
    ON [dbo].[SGC_AgendamentoCargaProduto]([IdAgendamentoCarga], [Ordem]);
    
    PRINT '  [OK] SGC_AgendamentoCargaProduto criada';
END
ELSE
    PRINT '  [--] SGC_AgendamentoCargaProduto já existe';
GO


/* =============================================================================
   SGC_AgendamentoAprovacao
   Descrição: Histórico de aprovações/rejeições do agendamento.
              Permite rastrear todas as ações do workflow.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_AgendamentoAprovacao', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_AgendamentoAprovacao](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdAgendamentoCarga]       INT          NOT NULL,
        [IdFuncionario]            UNIQUEIDENTIFIER NOT NULL,
        
        -- Ação
        [Acao]                     VARCHAR(20)  NOT NULL,
        [DataHoraAcao]             DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_AgendAprov_DataAcao] DEFAULT (SYSUTCDATETIME()),
        [Observacao]               VARCHAR(500) NULL,
        
        -- Auditoria (simplificada)
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_AgendamentoAprovacao_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        
        CONSTRAINT [PK_SGC_AgendamentoAprovacao] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_AgendAprov_Acao] CHECK ([Acao] IN ('ENVIADO', 'APROVADO', 'REJEITADO', 'DEVOLVIDO', 'CANCELADO'))
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_AgendamentoAprovacao_Agend]
    ON [dbo].[SGC_AgendamentoAprovacao]([IdAgendamentoCarga], [DataHoraAcao] DESC);
    
    PRINT '  [OK] SGC_AgendamentoAprovacao criada';
END
ELSE
    PRINT '  [--] SGC_AgendamentoAprovacao já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 4: TABELAS DE RECEBIMENTO
-- #############################################################################

PRINT '>>> PARTE 4: TABELAS DE RECEBIMENTO';
PRINT '';


/* =============================================================================
   SGC_RecebimentoCarga
   Descrição: Registro do recebimento efetivo da carga.
              Permite aceitação total, parcial ou recusa.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_RecebimentoCarga', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_RecebimentoCarga](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        [IdAgendamentoCarga]       INT          NULL,
        [IdRegistroAcesso]         BIGINT       NOT NULL,
        
        -- Identificação
        [NumeroRecebimento]        VARCHAR(20)  NOT NULL,
        
        -- Conferência
        [IdFuncionarioConferente]  UNIQUEIDENTIFIER NOT NULL,
        [DataHoraInicioConferencia] DATETIME2(0) NOT NULL,
        [DataHoraFimConferencia]   DATETIME2(0) NULL,
        
        -- Resultado
        [Status]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_RecebCarga_Status] DEFAULT ('EM_CONFERENCIA'),
        [IdMotivoRecusa]           INT          NULL,
        [ObservacaoRecusa]         VARCHAR(500) NULL,
        
        -- Checklist
        [IdChecklistModelo]        INT          NULL,
        [ResultadoChecklist]       VARCHAR(20)  NULL,
        
        -- Pesagem
        [PesoBrutoKg]              DECIMAL(15,4) NULL,
        [TaraKg]                   DECIMAL(15,4) NULL,
        [PesoLiquidoKg]            DECIMAL(15,4) NULL,
        
        -- Outros
        [Observacao]               VARCHAR(500) NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_RecebimentoCarga_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_RecebimentoCarga_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_RecebimentoCarga] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_RecebCarga_Status] CHECK ([Status] IN ('EM_CONFERENCIA', 'ACEITO', 'ACEITO_PARCIAL', 'RECUSADO')),
        CONSTRAINT [CK_SGC_RecebCarga_Checklist] CHECK ([ResultadoChecklist] IS NULL OR [ResultadoChecklist] IN ('APROVADO', 'REPROVADO', 'NA'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_RecebimentoCarga_Numero]
    ON [dbo].[SGC_RecebimentoCarga]([NumeroRecebimento]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_RecebimentoCarga_Agendamento]
    ON [dbo].[SGC_RecebimentoCarga]([IdAgendamentoCarga]) WHERE [IdAgendamentoCarga] IS NOT NULL;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_RecebimentoCarga_RegistroAcesso]
    ON [dbo].[SGC_RecebimentoCarga]([IdRegistroAcesso]);
    
    PRINT '  [OK] SGC_RecebimentoCarga criada';
END
ELSE
    PRINT '  [--] SGC_RecebimentoCarga já existe';
GO


/* =============================================================================
   SGC_RecebimentoCargaProduto
   Descrição: Conferência item a item do recebimento.
              Permite aceitar/recusar produtos individualmente.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_RecebimentoCargaProduto', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_RecebimentoCargaProduto](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [IdRecebimentoCarga]       INT          NOT NULL,
        [IdAgendamentoCargaProduto] INT         NULL,
        
        -- Dados do Produto
        [Ordem]                    INT          NOT NULL,
        [DescricaoProduto]         VARCHAR(200) NOT NULL,
        
        -- Quantidades
        [QuantidadePrevista]       DECIMAL(15,4) NULL,
        [QuantidadeRecebida]       DECIMAL(15,4) NOT NULL,
        [QuantidadeAceita]         DECIMAL(15,4) NOT NULL,
        [QuantidadeRecusada]       DECIMAL(15,4) NOT NULL CONSTRAINT [DF_SGC_RecebCargaProd_QtdRecusada] DEFAULT (0),
        [UnidadeMedida]            VARCHAR(20)  NOT NULL,
        
        -- Resultado do Item
        [Status]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_RecebCargaProd_Status] DEFAULT ('ACEITO'),
        [IdMotivoRecusa]           INT          NULL,
        [ObservacaoRecusa]         VARCHAR(255) NULL,
        
        -- FISPQ
        [FISPQRecebida]            BIT          NOT NULL CONSTRAINT [DF_SGC_RecebCargaProd_FISPQRecebida] DEFAULT (0),
        [FISPQPath]                VARCHAR(255) NULL,
        
        -- Rastreabilidade
        [LoteNumero]               VARCHAR(50)  NULL,
        [DataFabricacao]           DATE         NULL,
        [DataValidade]             DATE         NULL,
        
        -- Outros
        [Observacao]               VARCHAR(255) NULL,
        
        -- Auditoria (simplificada)
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_RecebimentoCargaProduto_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_RecebimentoCargaProduto] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_RecebCargaProd_Status] CHECK ([Status] IN ('ACEITO', 'ACEITO_PARCIAL', 'RECUSADO'))
    );
    
    CREATE NONCLUSTERED INDEX [IX_SGC_RecebimentoCargaProduto_Receb]
    ON [dbo].[SGC_RecebimentoCargaProduto]([IdRecebimentoCarga], [Ordem]);
    
    PRINT '  [OK] SGC_RecebimentoCargaProduto criada';
END
ELSE
    PRINT '  [--] SGC_RecebimentoCargaProduto já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 5: TABELAS DE OCORRÊNCIAS E CRACHÁS
-- #############################################################################

PRINT '>>> PARTE 5: TABELAS DE OCORRÊNCIAS E CRACHÁS';
PRINT '';


/* =============================================================================
   SGC_Ocorrencia
   Descrição: Registro de incidentes, problemas e ocorrências.
              Permite rastreabilidade e histórico de problemas.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Ocorrencia', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Ocorrencia](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        [IdPortaria]               INT          NULL,
        
        -- Identificação
        [NumeroOcorrencia]         VARCHAR(20)  NOT NULL,
        [DataHoraOcorrencia]       DATETIME2(0) NOT NULL,
        
        -- Tipo e Gravidade
        [TipoOcorrencia]           VARCHAR(30)  NOT NULL,
        [Gravidade]                VARCHAR(20)  NOT NULL,
        
        -- Relacionamentos
        [IdRegistroAcesso]         BIGINT       NULL,
        [IdRecebimentoCarga]       INT          NULL,
        [IdAgendamentoCarga]       INT          NULL,
        
        -- Pessoa Envolvida
        [TipoPessoaEnvolvida]      CHAR(1)      NULL,
        [IdFuncionarioEnvolvido]   UNIQUEIDENTIFIER NULL,
        [IdFornecedorColaborador]  INT          NULL,
        [IdVisitante]              INT          NULL,
        [IdFornecedorEmpresa]      INT          NULL,
        
        -- Descrição
        [Titulo]                   VARCHAR(150) NOT NULL,
        [Descricao]                VARCHAR(2000) NOT NULL,
        [Providencias]             VARCHAR(1000) NULL,
        
        -- Workflow
        [Status]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_Ocorrencia_Status] DEFAULT ('ABERTA'),
        [IdFuncionarioRegistrou]   UNIQUEIDENTIFIER NOT NULL,
        [IdFuncionarioResponsavel] UNIQUEIDENTIFIER NULL,
        [DataResolucao]            DATETIME2(0) NULL,
        [ObservacaoResolucao]      VARCHAR(500) NULL,
        
        -- Consequências
        [GeraAdvertencia]          BIT          NOT NULL CONSTRAINT [DF_SGC_Ocorrencia_Advertencia] DEFAULT (0),
        [GeraBloqueio]             BIT          NOT NULL CONSTRAINT [DF_SGC_Ocorrencia_Bloqueio] DEFAULT (0),
        [DiasBloqueio]             INT          NULL,
        [DataFimBloqueio]          DATE         NULL,
        
        -- Controle
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Ocorrencia_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Ocorrencia_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Ocorrencia] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_Ocorrencia_Tipo] CHECK ([TipoOcorrencia] IN (
            'ACESSO_NEGADO', 'DOCUMENTO_IRREGULAR', 'CARGA_RECUSADA', 'ACIDENTE', 
            'COMPORTAMENTO', 'SEGURANCA', 'FURTO_ROUBO', 'DANO_PATRIMONIO', 'OUTRO'
        )),
        CONSTRAINT [CK_SGC_Ocorrencia_Gravidade] CHECK ([Gravidade] IN ('BAIXA', 'MEDIA', 'ALTA', 'CRITICA')),
        CONSTRAINT [CK_SGC_Ocorrencia_Status] CHECK ([Status] IN ('ABERTA', 'EM_ANALISE', 'RESOLVIDA', 'ARQUIVADA')),
        CONSTRAINT [CK_SGC_Ocorrencia_TipoPessoa] CHECK ([TipoPessoaEnvolvida] IS NULL OR [TipoPessoaEnvolvida] IN ('F', 'T', 'V'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_Ocorrencia_Numero]
    ON [dbo].[SGC_Ocorrencia]([NumeroOcorrencia]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Ocorrencia_Data]
    ON [dbo].[SGC_Ocorrencia]([CdEmpresa], [CdFilial], [DataHoraOcorrencia] DESC);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Ocorrencia_Status]
    ON [dbo].[SGC_Ocorrencia]([Status]) WHERE [Status] IN ('ABERTA', 'EM_ANALISE');
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Ocorrencia_Bloqueio]
    ON [dbo].[SGC_Ocorrencia]([GeraBloqueio], [DataFimBloqueio]) WHERE [GeraBloqueio] = 1;
    
    PRINT '  [OK] SGC_Ocorrencia criada';
END
ELSE
    PRINT '  [--] SGC_Ocorrencia já existe';
GO


/* =============================================================================
   SGC_Cracha
   Descrição: Controle de crachás provisórios.
              Permite rastrear empréstimo e devolução.
   ============================================================================= */
IF OBJECT_ID('dbo.SGC_Cracha', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SGC_Cracha](
        -- Chave Primária
        [Id]                       INT IDENTITY(1,1) NOT NULL,
        
        -- Multi-Tenant e Vínculo
        [IdSaas]                   UNIQUEIDENTIFIER  NULL,
        [CdEmpresa]                INT          NOT NULL,
        [CdFilial]                 INT          NOT NULL,
        [IdPortaria]               INT          NULL,
        
        -- Dados do Crachá
        [NumeroCracha]             VARCHAR(20)  NOT NULL,
        [Tipo]                     VARCHAR(20)  NOT NULL,
        [CorCracha]                VARCHAR(30)  NULL,
        
        -- Status Atual
        [Status]                   VARCHAR(20)  NOT NULL CONSTRAINT [DF_SGC_Cracha_Status] DEFAULT ('DISPONIVEL'),
        [IdRegistroAcessoAtual]    BIGINT       NULL,
        
        -- Controle de Uso
        [DataUltimoUso]            DATETIME2(0) NULL,
        [QuantidadeUsos]           INT          NOT NULL CONSTRAINT [DF_SGC_Cracha_QtdUsos] DEFAULT (0),
        
        -- Outros
        [Observacao]               VARCHAR(255) NULL,
        [Ativo]                    BIT          NOT NULL CONSTRAINT [DF_SGC_Cracha_Ativo] DEFAULT (1),
        
        -- Auditoria
        [Aud_CreatedAt]            DATETIME2(0) NOT NULL CONSTRAINT [DF_SGC_Cracha_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [Aud_UpdatedAt]            DATETIME2(0) NULL,
        [Aud_IdUsuarioCadastro]    UNIQUEIDENTIFIER NULL,
        [Aud_IdUsuarioAtualizacao] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [PK_SGC_Cracha] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [CK_SGC_Cracha_Tipo] CHECK ([Tipo] IN ('VISITANTE', 'TERCEIRO', 'MOTORISTA', 'TEMPORARIO')),
        CONSTRAINT [CK_SGC_Cracha_Status] CHECK ([Status] IN ('DISPONIVEL', 'EM_USO', 'EXTRAVIADO', 'INATIVO'))
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [UX_SGC_Cracha_Numero]
    ON [dbo].[SGC_Cracha]([CdEmpresa], [CdFilial], [NumeroCracha]);
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Cracha_Status]
    ON [dbo].[SGC_Cracha]([CdEmpresa], [CdFilial], [Status]) WHERE [Ativo] = 1;
    
    CREATE NONCLUSTERED INDEX [IX_SGC_Cracha_Portaria]
    ON [dbo].[SGC_Cracha]([IdPortaria], [Status]) WHERE [IdPortaria] IS NOT NULL AND [Ativo] = 1;
    
    -- Dados Iniciais (exemplo)
    INSERT INTO [dbo].[SGC_Cracha] ([CdEmpresa], [CdFilial], [NumeroCracha], [Tipo], [CorCracha])
    VALUES 
        (1, 1, 'V001', 'VISITANTE', 'Verde'),
        (1, 1, 'V002', 'VISITANTE', 'Verde'),
        (1, 1, 'V003', 'VISITANTE', 'Verde'),
        (1, 1, 'V004', 'VISITANTE', 'Verde'),
        (1, 1, 'V005', 'VISITANTE', 'Verde'),
        (1, 1, 'T001', 'TERCEIRO', 'Azul'),
        (1, 1, 'T002', 'TERCEIRO', 'Azul'),
        (1, 1, 'T003', 'TERCEIRO', 'Azul'),
        (1, 1, 'M001', 'MOTORISTA', 'Laranja'),
        (1, 1, 'M002', 'MOTORISTA', 'Laranja');
    
    PRINT '  [OK] SGC_Cracha criada';
END
ELSE
    PRINT '  [--] SGC_Cracha já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 6: ALTERAÇÕES EM TABELAS EXISTENTES
-- #############################################################################

PRINT '>>> PARTE 6: ALTERAÇÕES EM TABELAS EXISTENTES';
PRINT '';


/* =============================================================================
   Alterações em SGC_RegistroAcesso
   ============================================================================= */

-- Adicionar IdPortaria
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'IdPortaria')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [IdPortaria] INT NULL;
    PRINT '  [OK] SGC_RegistroAcesso.IdPortaria adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.IdPortaria já existe';
GO

-- Adicionar IdContrato
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'IdContrato')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [IdContrato] INT NULL;
    PRINT '  [OK] SGC_RegistroAcesso.IdContrato adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.IdContrato já existe';
GO

-- Adicionar IdAgendamentoCarga
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'IdAgendamentoCarga')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [IdAgendamentoCarga] INT NULL;
    PRINT '  [OK] SGC_RegistroAcesso.IdAgendamentoCarga adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.IdAgendamentoCarga já existe';
GO

-- Adicionar IdCracha
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'IdCracha')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [IdCracha] INT NULL;
    PRINT '  [OK] SGC_RegistroAcesso.IdCracha adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.IdCracha já existe';
GO

-- Adicionar IdMotivoRecusa
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'IdMotivoRecusa')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [IdMotivoRecusa] INT NULL;
    PRINT '  [OK] SGC_RegistroAcesso.IdMotivoRecusa adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.IdMotivoRecusa já existe';
GO

-- Adicionar FotoEntradaPath
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'FotoEntradaPath')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [FotoEntradaPath] VARCHAR(255) NULL;
    PRINT '  [OK] SGC_RegistroAcesso.FotoEntradaPath adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.FotoEntradaPath já existe';
GO

-- Adicionar BiometriaValidada
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'BiometriaValidada')
BEGIN
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD [BiometriaValidada] BIT NULL;
    PRINT '  [OK] SGC_RegistroAcesso.BiometriaValidada adicionada';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.BiometriaValidada já existe';
GO

-- Remover NotaFiscal (se existir)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_RegistroAcesso') AND name = 'NotaFiscal')
BEGIN
    -- Primeiro verifica se há dados
    DECLARE @temDados BIT = 0;
    EXEC sp_executesql N'IF EXISTS (SELECT 1 FROM dbo.SGC_RegistroAcesso WHERE NotaFiscal IS NOT NULL) SET @temDados = 1', N'@temDados BIT OUTPUT', @temDados OUTPUT;
    
    IF @temDados = 0
    BEGIN
        ALTER TABLE [dbo].[SGC_RegistroAcesso] DROP COLUMN [NotaFiscal];
        PRINT '  [OK] SGC_RegistroAcesso.NotaFiscal removida';
    END
    ELSE
        PRINT '  [!!] SGC_RegistroAcesso.NotaFiscal possui dados - não removida';
END
ELSE
    PRINT '  [--] SGC_RegistroAcesso.NotaFiscal não existe';
GO


/* =============================================================================
   Alterações em SGC_FornecedorColaborador
   ============================================================================= */

-- Adicionar EhMotorista
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'EhMotorista')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [EhMotorista] BIT NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_EhMotorista] DEFAULT (0);
    PRINT '  [OK] SGC_FornecedorColaborador.EhMotorista adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.EhMotorista já existe';
GO

-- Adicionar CategoriaCNH
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'CategoriaCNH')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [CategoriaCNH] VARCHAR(5) NULL;
    PRINT '  [OK] SGC_FornecedorColaborador.CategoriaCNH adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.CategoriaCNH já existe';
GO

-- Adicionar NumeroCNH
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'NumeroCNH')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [NumeroCNH] VARCHAR(20) NULL;
    PRINT '  [OK] SGC_FornecedorColaborador.NumeroCNH adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.NumeroCNH já existe';
GO

-- Adicionar DataVencimentoCNH
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'DataVencimentoCNH')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [DataVencimentoCNH] DATE NULL;
    PRINT '  [OK] SGC_FornecedorColaborador.DataVencimentoCNH adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.DataVencimentoCNH já existe';
GO

-- Adicionar PossuiMOPP
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'PossuiMOPP')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [PossuiMOPP] BIT NOT NULL CONSTRAINT [DF_SGC_FornecedorColaborador_PossuiMOPP] DEFAULT (0);
    PRINT '  [OK] SGC_FornecedorColaborador.PossuiMOPP adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.PossuiMOPP já existe';
GO

-- Adicionar DataVencimentoMOPP
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'DataVencimentoMOPP')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [DataVencimentoMOPP] DATE NULL;
    PRINT '  [OK] SGC_FornecedorColaborador.DataVencimentoMOPP adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.DataVencimentoMOPP já existe';
GO

-- Adicionar IdTipoSanguineo
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_FornecedorColaborador') AND name = 'IdTipoSanguineo')
BEGIN
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD [IdTipoSanguineo] INT NULL;
    PRINT '  [OK] SGC_FornecedorColaborador.IdTipoSanguineo adicionada';
END
ELSE
    PRINT '  [--] SGC_FornecedorColaborador.IdTipoSanguineo já existe';
GO


/* =============================================================================
   Alterações em SGC_TipoVeiculo
   ============================================================================= */

-- Adicionar ExigeAgendamento
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_TipoVeiculo') AND name = 'ExigeAgendamento')
BEGIN
    ALTER TABLE [dbo].[SGC_TipoVeiculo] ADD [ExigeAgendamento] BIT NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_ExigeAgend] DEFAULT (0);
    PRINT '  [OK] SGC_TipoVeiculo.ExigeAgendamento adicionada';
END
ELSE
    PRINT '  [--] SGC_TipoVeiculo.ExigeAgendamento já existe';
GO

-- Adicionar ExigeFISPQ
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_TipoVeiculo') AND name = 'ExigeFISPQ')
BEGIN
    ALTER TABLE [dbo].[SGC_TipoVeiculo] ADD [ExigeFISPQ] BIT NOT NULL CONSTRAINT [DF_SGC_TipoVeiculo_ExigeFISPQ] DEFAULT (0);
    PRINT '  [OK] SGC_TipoVeiculo.ExigeFISPQ adicionada';
END
ELSE
    PRINT '  [--] SGC_TipoVeiculo.ExigeFISPQ já existe';
GO

-- Atualizar dados existentes
UPDATE [dbo].[SGC_TipoVeiculo] SET [ExigeAgendamento] = 1 WHERE [Codigo] IN ('CAMINHAO_G', 'CARRETA', 'TANQUE', 'BITREM');
UPDATE [dbo].[SGC_TipoVeiculo] SET [ExigeFISPQ] = 1 WHERE [Codigo] = 'TANQUE';
PRINT '  [OK] SGC_TipoVeiculo dados atualizados';
GO


/* =============================================================================
   Alterações em SGC_TreinamentoTurma
   ============================================================================= */

-- Adicionar CdEmpresa
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_TreinamentoTurma') AND name = 'CdEmpresa')
BEGIN
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD [CdEmpresa] INT NULL;
    PRINT '  [OK] SGC_TreinamentoTurma.CdEmpresa adicionada';
END
ELSE
    PRINT '  [--] SGC_TreinamentoTurma.CdEmpresa já existe';
GO

-- Adicionar CdFilial
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_TreinamentoTurma') AND name = 'CdFilial')
BEGIN
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD [CdFilial] INT NULL;
    PRINT '  [OK] SGC_TreinamentoTurma.CdFilial adicionada';
END
ELSE
    PRINT '  [--] SGC_TreinamentoTurma.CdFilial já existe';
GO

-- Adicionar Status
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_TreinamentoTurma') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD [Status] VARCHAR(20) NOT NULL CONSTRAINT [DF_SGC_TreinamentoTurma_Status] DEFAULT ('AGENDADA');
    PRINT '  [OK] SGC_TreinamentoTurma.Status adicionada';
END
ELSE
    PRINT '  [--] SGC_TreinamentoTurma.Status já existe';
GO

-- Adicionar VagasDisponiveis
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_TreinamentoTurma') AND name = 'VagasDisponiveis')
BEGIN
    ALTER TABLE [dbo].[SGC_TreinamentoTurma] ADD [VagasDisponiveis] INT NULL;
    PRINT '  [OK] SGC_TreinamentoTurma.VagasDisponiveis adicionada';
END
ELSE
    PRINT '  [--] SGC_TreinamentoTurma.VagasDisponiveis já existe';
GO


/* =============================================================================
   Alterações em SGC_Veiculo
   ============================================================================= */

-- Adicionar NumeroCRLV
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_Veiculo') AND name = 'NumeroCRLV')
BEGIN
    ALTER TABLE [dbo].[SGC_Veiculo] ADD [NumeroCRLV] VARCHAR(20) NULL;
    PRINT '  [OK] SGC_Veiculo.NumeroCRLV adicionada';
END
ELSE
    PRINT '  [--] SGC_Veiculo.NumeroCRLV já existe';
GO

-- Adicionar DataVencimentoCRLV
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SGC_Veiculo') AND name = 'DataVencimentoCRLV')
BEGIN
    ALTER TABLE [dbo].[SGC_Veiculo] ADD [DataVencimentoCRLV] DATE NULL;
    PRINT '  [OK] SGC_Veiculo.DataVencimentoCRLV adicionada';
END
ELSE
    PRINT '  [--] SGC_Veiculo.DataVencimentoCRLV já existe';
GO

PRINT '';


-- #############################################################################
-- PARTE 7: FOREIGN KEYS
-- #############################################################################

PRINT '>>> PARTE 7: FOREIGN KEYS';
PRINT '';

-- -----------------------------------------------------------------------------
-- 7.1 FKs das Tabelas de Configuração
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Configuração...';

-- SGC_Portaria não tem FKs para outras tabelas SGC (só empresa/filial do legado)

-- SGC_ConfiguracaoFilial não tem FKs para outras tabelas SGC

-- SGC_ConfiguracaoObrigatoriedade não tem FKs para outras tabelas SGC

-- SGC_MotivoRecusa não tem FKs

PRINT '  [OK] FKs de Configuração';

-- -----------------------------------------------------------------------------
-- 7.2 FKs das Tabelas de Contratos
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Contratos...';

-- SGC_Contrato
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Contrato_FornecedorEmpresa')
    ALTER TABLE [dbo].[SGC_Contrato] ADD CONSTRAINT [FK_SGC_Contrato_FornecedorEmpresa]
    FOREIGN KEY ([IdFornecedorEmpresa]) REFERENCES [dbo].[SGC_FornecedorEmpresa]([Id]);
GO

-- SGC_ContratoResponsavel
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_ContratoResponsavel_Contrato')
    ALTER TABLE [dbo].[SGC_ContratoResponsavel] ADD CONSTRAINT [FK_SGC_ContratoResponsavel_Contrato]
    FOREIGN KEY ([IdContrato]) REFERENCES [dbo].[SGC_Contrato]([Id]);
GO

-- SGC_ContratoColaborador
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_ContratoColaborador_Contrato')
    ALTER TABLE [dbo].[SGC_ContratoColaborador] ADD CONSTRAINT [FK_SGC_ContratoColaborador_Contrato]
    FOREIGN KEY ([IdContrato]) REFERENCES [dbo].[SGC_Contrato]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_ContratoColaborador_Colaborador')
    ALTER TABLE [dbo].[SGC_ContratoColaborador] ADD CONSTRAINT [FK_SGC_ContratoColaborador_Colaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

PRINT '  [OK] FKs de Contratos';

-- -----------------------------------------------------------------------------
-- 7.3 FKs das Tabelas de Agendamento
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Agendamento...';

-- SGC_AgendamentoCarga
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_Portaria')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_Portaria]
    FOREIGN KEY ([IdPortaria]) REFERENCES [dbo].[SGC_Portaria]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_Contrato')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_Contrato]
    FOREIGN KEY ([IdContrato]) REFERENCES [dbo].[SGC_Contrato]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_FornecedorEmpresa')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_FornecedorEmpresa]
    FOREIGN KEY ([IdFornecedorEmpresa]) REFERENCES [dbo].[SGC_FornecedorEmpresa]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_FornecedorColaborador')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_FornecedorColaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_Veiculo')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_Veiculo]
    FOREIGN KEY ([IdVeiculo]) REFERENCES [dbo].[SGC_Veiculo]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_ChecklistModelo')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_ChecklistModelo]
    FOREIGN KEY ([IdChecklistModelo]) REFERENCES [dbo].[SGC_ChecklistModelo]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCarga_RegistroAcesso')
    ALTER TABLE [dbo].[SGC_AgendamentoCarga] ADD CONSTRAINT [FK_SGC_AgendamentoCarga_RegistroAcesso]
    FOREIGN KEY ([IdRegistroAcesso]) REFERENCES [dbo].[SGC_RegistroAcesso]([Id]);
GO

-- SGC_AgendamentoCargaProduto
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoCargaProduto_Agendamento')
    ALTER TABLE [dbo].[SGC_AgendamentoCargaProduto] ADD CONSTRAINT [FK_SGC_AgendamentoCargaProduto_Agendamento]
    FOREIGN KEY ([IdAgendamentoCarga]) REFERENCES [dbo].[SGC_AgendamentoCarga]([Id]);
GO

-- SGC_AgendamentoAprovacao
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_AgendamentoAprovacao_Agendamento')
    ALTER TABLE [dbo].[SGC_AgendamentoAprovacao] ADD CONSTRAINT [FK_SGC_AgendamentoAprovacao_Agendamento]
    FOREIGN KEY ([IdAgendamentoCarga]) REFERENCES [dbo].[SGC_AgendamentoCarga]([Id]);
GO

PRINT '  [OK] FKs de Agendamento';

-- -----------------------------------------------------------------------------
-- 7.4 FKs das Tabelas de Recebimento
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Recebimento...';

-- SGC_RecebimentoCarga
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCarga_Agendamento')
    ALTER TABLE [dbo].[SGC_RecebimentoCarga] ADD CONSTRAINT [FK_SGC_RecebimentoCarga_Agendamento]
    FOREIGN KEY ([IdAgendamentoCarga]) REFERENCES [dbo].[SGC_AgendamentoCarga]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCarga_RegistroAcesso')
    ALTER TABLE [dbo].[SGC_RecebimentoCarga] ADD CONSTRAINT [FK_SGC_RecebimentoCarga_RegistroAcesso]
    FOREIGN KEY ([IdRegistroAcesso]) REFERENCES [dbo].[SGC_RegistroAcesso]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCarga_MotivoRecusa')
    ALTER TABLE [dbo].[SGC_RecebimentoCarga] ADD CONSTRAINT [FK_SGC_RecebimentoCarga_MotivoRecusa]
    FOREIGN KEY ([IdMotivoRecusa]) REFERENCES [dbo].[SGC_MotivoRecusa]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCarga_ChecklistModelo')
    ALTER TABLE [dbo].[SGC_RecebimentoCarga] ADD CONSTRAINT [FK_SGC_RecebimentoCarga_ChecklistModelo]
    FOREIGN KEY ([IdChecklistModelo]) REFERENCES [dbo].[SGC_ChecklistModelo]([Id]);
GO

-- SGC_RecebimentoCargaProduto
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCargaProduto_Recebimento')
    ALTER TABLE [dbo].[SGC_RecebimentoCargaProduto] ADD CONSTRAINT [FK_SGC_RecebimentoCargaProduto_Recebimento]
    FOREIGN KEY ([IdRecebimentoCarga]) REFERENCES [dbo].[SGC_RecebimentoCarga]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCargaProduto_AgendProduto')
    ALTER TABLE [dbo].[SGC_RecebimentoCargaProduto] ADD CONSTRAINT [FK_SGC_RecebimentoCargaProduto_AgendProduto]
    FOREIGN KEY ([IdAgendamentoCargaProduto]) REFERENCES [dbo].[SGC_AgendamentoCargaProduto]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RecebimentoCargaProduto_MotivoRecusa')
    ALTER TABLE [dbo].[SGC_RecebimentoCargaProduto] ADD CONSTRAINT [FK_SGC_RecebimentoCargaProduto_MotivoRecusa]
    FOREIGN KEY ([IdMotivoRecusa]) REFERENCES [dbo].[SGC_MotivoRecusa]([Id]);
GO

PRINT '  [OK] FKs de Recebimento';

-- -----------------------------------------------------------------------------
-- 7.5 FKs das Tabelas de Ocorrências e Crachás
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Ocorrências e Crachás...';

-- SGC_Ocorrencia
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_Portaria')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_Portaria]
    FOREIGN KEY ([IdPortaria]) REFERENCES [dbo].[SGC_Portaria]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_RegistroAcesso')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_RegistroAcesso]
    FOREIGN KEY ([IdRegistroAcesso]) REFERENCES [dbo].[SGC_RegistroAcesso]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_RecebimentoCarga')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_RecebimentoCarga]
    FOREIGN KEY ([IdRecebimentoCarga]) REFERENCES [dbo].[SGC_RecebimentoCarga]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_AgendamentoCarga')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_AgendamentoCarga]
    FOREIGN KEY ([IdAgendamentoCarga]) REFERENCES [dbo].[SGC_AgendamentoCarga]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_FornecedorColaborador')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_FornecedorColaborador]
    FOREIGN KEY ([IdFornecedorColaborador]) REFERENCES [dbo].[SGC_FornecedorColaborador]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_Visitante')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_Visitante]
    FOREIGN KEY ([IdVisitante]) REFERENCES [dbo].[SGC_Visitante]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Ocorrencia_FornecedorEmpresa')
    ALTER TABLE [dbo].[SGC_Ocorrencia] ADD CONSTRAINT [FK_SGC_Ocorrencia_FornecedorEmpresa]
    FOREIGN KEY ([IdFornecedorEmpresa]) REFERENCES [dbo].[SGC_FornecedorEmpresa]([Id]);
GO

-- SGC_Cracha
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Cracha_Portaria')
    ALTER TABLE [dbo].[SGC_Cracha] ADD CONSTRAINT [FK_SGC_Cracha_Portaria]
    FOREIGN KEY ([IdPortaria]) REFERENCES [dbo].[SGC_Portaria]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_Cracha_RegistroAcesso')
    ALTER TABLE [dbo].[SGC_Cracha] ADD CONSTRAINT [FK_SGC_Cracha_RegistroAcesso]
    FOREIGN KEY ([IdRegistroAcessoAtual]) REFERENCES [dbo].[SGC_RegistroAcesso]([Id]);
GO

PRINT '  [OK] FKs de Ocorrências e Crachás';

-- -----------------------------------------------------------------------------
-- 7.6 FKs das Alterações em Tabelas Existentes
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Alterações em Tabelas Existentes...';

-- SGC_RegistroAcesso
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_Portaria')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_Portaria]
    FOREIGN KEY ([IdPortaria]) REFERENCES [dbo].[SGC_Portaria]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_Contrato')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_Contrato]
    FOREIGN KEY ([IdContrato]) REFERENCES [dbo].[SGC_Contrato]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_AgendamentoCarga')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_AgendamentoCarga]
    FOREIGN KEY ([IdAgendamentoCarga]) REFERENCES [dbo].[SGC_AgendamentoCarga]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_Cracha')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_Cracha]
    FOREIGN KEY ([IdCracha]) REFERENCES [dbo].[SGC_Cracha]([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_RegistroAcesso_MotivoRecusa')
    ALTER TABLE [dbo].[SGC_RegistroAcesso] ADD CONSTRAINT [FK_SGC_RegistroAcesso_MotivoRecusa]
    FOREIGN KEY ([IdMotivoRecusa]) REFERENCES [dbo].[SGC_MotivoRecusa]([Id]);
GO

-- SGC_FornecedorColaborador
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SGC_FornecedorColaborador_TipoSanguineo')
    ALTER TABLE [dbo].[SGC_FornecedorColaborador] ADD CONSTRAINT [FK_SGC_FornecedorColaborador_TipoSanguineo]
    FOREIGN KEY ([IdTipoSanguineo]) REFERENCES [dbo].[SGC_TipoSanguineo]([Id]);
GO

PRINT '  [OK] FKs de Alterações';

-- -----------------------------------------------------------------------------
-- 7.7 FKs de Auditoria (tuse1)
-- -----------------------------------------------------------------------------

PRINT '  Criando FKs de Auditoria (tuse1)...';

DECLARE @sql NVARCHAR(MAX);
DECLARE @tableName VARCHAR(100);
DECLARE @tablesAudit TABLE (TableName VARCHAR(100));

INSERT INTO @tablesAudit VALUES 
    ('SGC_Portaria'), ('SGC_ConfiguracaoFilial'), ('SGC_ConfiguracaoObrigatoriedade'), 
    ('SGC_MotivoRecusa'), ('SGC_Contrato'), ('SGC_ContratoResponsavel'), ('SGC_ContratoColaborador'),
    ('SGC_AgendamentoCarga'), ('SGC_AgendamentoCargaProduto'),
    ('SGC_RecebimentoCarga'), ('SGC_Ocorrencia'), ('SGC_Cracha');

DECLARE curAudit CURSOR FOR SELECT TableName FROM @tablesAudit;
OPEN curAudit;
FETCH NEXT FROM curAudit INTO @tableName;

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
        BEGIN CATCH END CATCH
    END
    
    -- FK para Aud_IdUsuarioAtualizacao
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.' + @tableName) AND name = 'Aud_IdUsuarioAtualizacao')
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_' + @tableName + '_UsuarioAtualizacao')
    BEGIN
        SET @sql = 'ALTER TABLE dbo.' + @tableName + ' ADD CONSTRAINT FK_' + @tableName + '_UsuarioAtualizacao FOREIGN KEY (Aud_IdUsuarioAtualizacao) REFERENCES dbo.tuse1(id);';
        BEGIN TRY
            EXEC sp_executesql @sql;
        END TRY
        BEGIN CATCH END CATCH
    END
    
    FETCH NEXT FROM curAudit INTO @tableName;
END

CLOSE curAudit;
DEALLOCATE curAudit;

PRINT '  [OK] FKs de Auditoria criadas (ou tuse1 não disponível)';
PRINT '';


-- #############################################################################
-- PARTE 8: EXTENDED PROPERTIES (DOCUMENTAÇÃO)
-- #############################################################################

PRINT '>>> PARTE 8: EXTENDED PROPERTIES (DOCUMENTAÇÃO)';
PRINT '';

-- Procedure temporária para adicionar Extended Properties
IF OBJECT_ID('tempdb..#AddExtPropV2') IS NOT NULL DROP PROCEDURE #AddExtPropV2;
GO

CREATE PROCEDURE #AddExtPropV2
    @TableName VARCHAR(100),
    @ColumnName VARCHAR(100),
    @Description VARCHAR(500)
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM sys.extended_properties ep
        JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
        WHERE ep.name = 'MS_Description'
          AND c.object_id = OBJECT_ID('dbo.' + @TableName)
          AND c.name = @ColumnName
    )
    BEGIN
        EXEC sp_addextendedproperty 
            @name = N'MS_Description',
            @value = @Description,
            @level0type = N'SCHEMA', @level0name = 'dbo',
            @level1type = N'TABLE',  @level1name = @TableName,
            @level2type = N'COLUMN', @level2name = @ColumnName;
    END
END
GO

-- SGC_Portaria
EXEC #AddExtPropV2 'SGC_Portaria', 'Codigo', 'Código único da portaria (ex: PORT01).';
EXEC #AddExtPropV2 'SGC_Portaria', 'Nome', 'Nome descritivo da portaria.';
EXEC #AddExtPropV2 'SGC_Portaria', 'Tipo', 'Tipo: PRINCIPAL, CARGA, EMERGENCIA, PEDESTRES, OUTRO.';
EXEC #AddExtPropV2 'SGC_Portaria', 'PermiteVeiculos', 'Indica se a portaria permite entrada de veículos.';
EXEC #AddExtPropV2 'SGC_Portaria', 'PermitePedestres', 'Indica se a portaria permite entrada de pedestres.';
EXEC #AddExtPropV2 'SGC_Portaria', 'PermiteCarga', 'Indica se a portaria permite recebimento de carga.';

-- SGC_ConfiguracaoFilial
EXEC #AddExtPropV2 'SGC_ConfiguracaoFilial', 'ExigeAgendamentoCarga', 'Se TRUE, carga só pode entrar com agendamento prévio.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoFilial', 'ExigeAprovacaoAgendamento', 'Se TRUE, agendamento precisa de aprovação de gestor.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoFilial', 'BloqueiaDocumentoVencido', 'Se TRUE, impede entrada se documento estiver vencido.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoFilial', 'PermiteRecusaParcial', 'Se TRUE, permite aceitar parte da carga e recusar o resto.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoFilial', 'DiasAlertaVencimento', 'Quantos dias antes do vencimento gerar alerta.';

-- SGC_ConfiguracaoObrigatoriedade
EXEC #AddExtPropV2 'SGC_ConfiguracaoObrigatoriedade', 'Contexto', 'Onde a regra se aplica: ACESSO_TERCEIRO, ACESSO_VISITANTE, ACESSO_MOTORISTA, CARGA_RECEBIMENTO, CARGA_PERIGOSA, VEICULO_ENTRADA.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoObrigatoriedade', 'CodigoValidacao', 'O que valida: CNH, ASO, MOPP, NR10, NR20, NR33, NR35, INTEGRACAO, FOTO, FISPQ, CONTRATO_ATIVO, AGENDAMENTO_PREVIO, CRLV.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoObrigatoriedade', 'EhObrigatorio', 'Se TRUE, esta validação é obrigatória para o contexto.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoObrigatoriedade', 'BloqueiaSeNaoAtender', 'Se TRUE, impede entrada se não atender à validação.';
EXEC #AddExtPropV2 'SGC_ConfiguracaoObrigatoriedade', 'ValidadeMinimaEmDias', 'Documento deve ter pelo menos X dias de validade restante.';

-- SGC_MotivoRecusa
EXEC #AddExtPropV2 'SGC_MotivoRecusa', 'Tipo', 'Tipo: ACESSO, CARGA, VEICULO, DOCUMENTO.';
EXEC #AddExtPropV2 'SGC_MotivoRecusa', 'ExigeObservacao', 'Se TRUE, obriga informar observação ao selecionar este motivo.';
EXEC #AddExtPropV2 'SGC_MotivoRecusa', 'GeraOcorrencia', 'Se TRUE, cria automaticamente uma ocorrência ao usar este motivo.';
EXEC #AddExtPropV2 'SGC_MotivoRecusa', 'GeraBloqueioTemporario', 'Se TRUE, bloqueia a pessoa/empresa temporariamente.';
EXEC #AddExtPropV2 'SGC_MotivoRecusa', 'DiasBloqueio', 'Quantidade de dias de bloqueio (se GeraBloqueioTemporario = TRUE).';

-- SGC_Contrato
EXEC #AddExtPropV2 'SGC_Contrato', 'NumeroContrato', 'Número único do contrato.';
EXEC #AddExtPropV2 'SGC_Contrato', 'Status', 'Status: ATIVO, SUSPENSO, ENCERRADO, CANCELADO.';
EXEC #AddExtPropV2 'SGC_Contrato', 'PermiteAcessoSemAgendamento', 'Se TRUE, terceiros deste contrato podem entrar sem agendamento.';
EXEC #AddExtPropV2 'SGC_Contrato', 'QuantidadeMaximaColaboradores', 'Limite de colaboradores simultâneos (NULL = ilimitado).';

-- SGC_ContratoResponsavel
EXEC #AddExtPropV2 'SGC_ContratoResponsavel', 'TipoResponsavel', 'Tipo: GESTOR, SUPLENTE, FISCAL, TECNICO.';
EXEC #AddExtPropV2 'SGC_ContratoResponsavel', 'PodeAprovarAgendamento', 'Se TRUE, pode aprovar agendamentos relacionados a este contrato.';
EXEC #AddExtPropV2 'SGC_ContratoResponsavel', 'PodeAutorizarAcesso', 'Se TRUE, pode autorizar acesso direto de terceiros.';
EXEC #AddExtPropV2 'SGC_ContratoResponsavel', 'RecebeAlertaVencimento', 'Se TRUE, recebe alertas de documentos vencendo.';

-- SGC_AgendamentoCarga
EXEC #AddExtPropV2 'SGC_AgendamentoCarga', 'TipoEntrega', 'Tipo: CARGA, ENTREGA_SIMPLES, COLETA, DEVOLUCAO.';
EXEC #AddExtPropV2 'SGC_AgendamentoCarga', 'TipoCarga', 'Tipo: NORMAL, QUIMICA, INFLAMAVEL, FRAGIL, REFRIGERADA, PERIGOSA.';
EXEC #AddExtPropV2 'SGC_AgendamentoCarga', 'Status', 'Status: RASCUNHO, PENDENTE_APROVACAO, APROVADO, REJEITADO, EM_ANDAMENTO, CONCLUIDO, CONCLUIDO_PARCIAL, CANCELADO, NAO_COMPARECEU.';
EXEC #AddExtPropV2 'SGC_AgendamentoCarga', 'AlertaEspecial', 'Mensagem de alerta destacada para a portaria (ex: PRODUTO QUÍMICO!).';
EXEC #AddExtPropV2 'SGC_AgendamentoCarga', 'ExigeChecklist', 'Se TRUE, obriga aplicar checklist na entrada.';

-- SGC_AgendamentoCargaProduto
EXEC #AddExtPropV2 'SGC_AgendamentoCargaProduto', 'EhProdutoQuimico', 'Indica se é produto químico.';
EXEC #AddExtPropV2 'SGC_AgendamentoCargaProduto', 'EhProdutoInflamavel', 'Indica se é produto inflamável.';
EXEC #AddExtPropV2 'SGC_AgendamentoCargaProduto', 'NumeroONU', 'Número ONU para produtos perigosos.';
EXEC #AddExtPropV2 'SGC_AgendamentoCargaProduto', 'ExigeFISPQ', 'Se TRUE, exige Ficha de Informações de Segurança de Produtos Químicos.';

-- SGC_RecebimentoCarga
EXEC #AddExtPropV2 'SGC_RecebimentoCarga', 'Status', 'Status: EM_CONFERENCIA, ACEITO, ACEITO_PARCIAL, RECUSADO.';
EXEC #AddExtPropV2 'SGC_RecebimentoCarga', 'ResultadoChecklist', 'Resultado: APROVADO, REPROVADO, NA (não aplicável).';
EXEC #AddExtPropV2 'SGC_RecebimentoCarga', 'PesoBrutoKg', 'Peso bruto em quilogramas.';
EXEC #AddExtPropV2 'SGC_RecebimentoCarga', 'TaraKg', 'Tara (peso do veículo vazio) em quilogramas.';
EXEC #AddExtPropV2 'SGC_RecebimentoCarga', 'PesoLiquidoKg', 'Peso líquido (bruto - tara) em quilogramas.';

-- SGC_Ocorrencia
EXEC #AddExtPropV2 'SGC_Ocorrencia', 'TipoOcorrencia', 'Tipo: ACESSO_NEGADO, DOCUMENTO_IRREGULAR, CARGA_RECUSADA, ACIDENTE, COMPORTAMENTO, SEGURANCA, FURTO_ROUBO, DANO_PATRIMONIO, OUTRO.';
EXEC #AddExtPropV2 'SGC_Ocorrencia', 'Gravidade', 'Gravidade: BAIXA, MEDIA, ALTA, CRITICA.';
EXEC #AddExtPropV2 'SGC_Ocorrencia', 'Status', 'Status: ABERTA, EM_ANALISE, RESOLVIDA, ARQUIVADA.';
EXEC #AddExtPropV2 'SGC_Ocorrencia', 'GeraBloqueio', 'Se TRUE, bloqueia a pessoa/empresa.';
EXEC #AddExtPropV2 'SGC_Ocorrencia', 'DataFimBloqueio', 'Data em que o bloqueio expira.';

-- SGC_Cracha
EXEC #AddExtPropV2 'SGC_Cracha', 'Tipo', 'Tipo: VISITANTE, TERCEIRO, MOTORISTA, TEMPORARIO.';
EXEC #AddExtPropV2 'SGC_Cracha', 'Status', 'Status: DISPONIVEL, EM_USO, EXTRAVIADO, INATIVO.';
EXEC #AddExtPropV2 'SGC_Cracha', 'IdRegistroAcessoAtual', 'FK do registro de acesso que está usando o crachá (NULL = disponível).';
EXEC #AddExtPropV2 'SGC_Cracha', 'QuantidadeUsos', 'Contador de quantas vezes o crachá foi utilizado.';

DROP PROCEDURE #AddExtPropV2;
GO

PRINT '  [OK] Extended Properties criadas';
PRINT '';


-- #############################################################################
-- RESUMO FINAL
-- #############################################################################

PRINT '=============================================================================';
PRINT 'SCRIPT COMPLEMENTAR EXECUTADO COM SUCESSO!';
PRINT 'Término: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '=============================================================================';
PRINT '';
PRINT 'TABELAS DE CONFIGURAÇÃO: 4';
PRINT '  - SGC_Portaria';
PRINT '  - SGC_ConfiguracaoFilial';
PRINT '  - SGC_ConfiguracaoObrigatoriedade';
PRINT '  - SGC_MotivoRecusa';
PRINT '';
PRINT 'TABELAS DE CONTRATOS: 3';
PRINT '  - SGC_Contrato';
PRINT '  - SGC_ContratoResponsavel';
PRINT '  - SGC_ContratoColaborador';
PRINT '';
PRINT 'TABELAS DE AGENDAMENTO: 3';
PRINT '  - SGC_AgendamentoCarga';
PRINT '  - SGC_AgendamentoCargaProduto';
PRINT '  - SGC_AgendamentoAprovacao';
PRINT '';
PRINT 'TABELAS DE RECEBIMENTO: 2';
PRINT '  - SGC_RecebimentoCarga';
PRINT '  - SGC_RecebimentoCargaProduto';
PRINT '';
PRINT 'TABELAS DE OCORRÊNCIAS E CRACHÁS: 2';
PRINT '  - SGC_Ocorrencia';
PRINT '  - SGC_Cracha';
PRINT '';
PRINT 'TOTAL NOVAS TABELAS: 14';
PRINT '';
PRINT 'ALTERAÇÕES EM TABELAS EXISTENTES:';
PRINT '  - SGC_RegistroAcesso: +6 campos (IdPortaria, IdContrato, IdAgendamentoCarga, IdCracha, IdMotivoRecusa, FotoEntradaPath, BiometriaValidada)';
PRINT '  - SGC_FornecedorColaborador: +7 campos (EhMotorista, CategoriaCNH, NumeroCNH, DataVencimentoCNH, PossuiMOPP, DataVencimentoMOPP, IdTipoSanguineo)';
PRINT '  - SGC_TipoVeiculo: +2 campos (ExigeAgendamento, ExigeFISPQ)';
PRINT '  - SGC_TreinamentoTurma: +4 campos (CdEmpresa, CdFilial, Status, VagasDisponiveis)';
PRINT '  - SGC_Veiculo: +2 campos (NumeroCRLV, DataVencimentoCRLV)';
PRINT '';
PRINT 'TOTAL GERAL DO SISTEMA: 40 TABELAS';
PRINT '=============================================================================';
GO