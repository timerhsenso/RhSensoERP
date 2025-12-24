USE [bd_rhu_copenor];
GO

/* =============================================================================
   SCRIPT DE LIMPEZA - DROP DE TODAS AS TABELAS SGC v3.0
   ======================================================
   
   ⚠️  ATENÇÃO: Este script REMOVE PERMANENTEMENTE todas as tabelas SGC!
   
============================================================================= */

SET NOCOUNT ON;
PRINT '=============================================================================';
PRINT '⚠️  LIMPEZA SGC v3.0 - REMOVENDO TODAS AS TABELAS';
PRINT '=============================================================================';
PRINT '';

-- =============================================================================
-- PARTE 1: REMOVER TODAS AS FOREIGN KEYS
-- =============================================================================

PRINT '>>> REMOVENDO FOREIGN KEYS...';

DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
FROM sys.foreign_keys
WHERE OBJECT_NAME(parent_object_id) LIKE 'SGC_%';

IF LEN(@sql) > 0
BEGIN
    EXEC sp_executesql @sql;
    PRINT '  [OK] Foreign Keys removidas';
END
ELSE
    PRINT '  [--] Nenhuma FK encontrada';

PRINT '';

-- =============================================================================
-- PARTE 2: REMOVER TABELAS TRANSACIONAIS
-- =============================================================================

PRINT '>>> REMOVENDO TABELAS TRANSACIONAIS...';

IF OBJECT_ID('dbo.SGC_RegistroAcessoChecklist', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_RegistroAcessoChecklist]; PRINT '  [OK] SGC_RegistroAcessoChecklist'; END

IF OBJECT_ID('dbo.SGC_RegistroAcesso', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_RegistroAcesso]; PRINT '  [OK] SGC_RegistroAcesso'; END

IF OBJECT_ID('dbo.SGC_AlertaVencimento', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_AlertaVencimento]; PRINT '  [OK] SGC_AlertaVencimento'; END

IF OBJECT_ID('dbo.SGC_Autorizacao', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_Autorizacao]; PRINT '  [OK] SGC_Autorizacao'; END

PRINT '';

-- =============================================================================
-- PARTE 3: REMOVER TABELAS DE CHECKLIST
-- =============================================================================

PRINT '>>> REMOVENDO TABELAS DE CHECKLIST...';

IF OBJECT_ID('dbo.SGC_ChecklistItem', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_ChecklistItem]; PRINT '  [OK] SGC_ChecklistItem'; END

IF OBJECT_ID('dbo.SGC_ChecklistModelo', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_ChecklistModelo]; PRINT '  [OK] SGC_ChecklistModelo'; END

PRINT '';

-- =============================================================================
-- PARTE 4: REMOVER TABELAS DE CADASTRO
-- =============================================================================

PRINT '>>> REMOVENDO TABELAS DE CADASTRO...';

IF OBJECT_ID('dbo.SGC_PessoaDocumento', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_PessoaDocumento]; PRINT '  [OK] SGC_PessoaDocumento'; END

IF OBJECT_ID('dbo.SGC_TreinamentoParticipante', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TreinamentoParticipante]; PRINT '  [OK] SGC_TreinamentoParticipante'; END

IF OBJECT_ID('dbo.SGC_TreinamentoTurma', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TreinamentoTurma]; PRINT '  [OK] SGC_TreinamentoTurma'; END

IF OBJECT_ID('dbo.SGC_Veiculo', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_Veiculo]; PRINT '  [OK] SGC_Veiculo'; END

IF OBJECT_ID('dbo.SGC_Visitante', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_Visitante]; PRINT '  [OK] SGC_Visitante'; END

IF OBJECT_ID('dbo.SGC_Instrutor', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_Instrutor]; PRINT '  [OK] SGC_Instrutor'; END

IF OBJECT_ID('dbo.SGC_LocalTreinamento', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_LocalTreinamento]; PRINT '  [OK] SGC_LocalTreinamento'; END

IF OBJECT_ID('dbo.SGC_FornecedorColaborador', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_FornecedorColaborador]; PRINT '  [OK] SGC_FornecedorColaborador'; END

IF OBJECT_ID('dbo.SGC_FornecedorEmpresa', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_FornecedorEmpresa]; PRINT '  [OK] SGC_FornecedorEmpresa'; END

PRINT '';

-- =============================================================================
-- PARTE 5: REMOVER TABELAS AUXILIARES (LOOKUPS)
-- =============================================================================

PRINT '>>> REMOVENDO TABELAS AUXILIARES (LOOKUPS)...';

IF OBJECT_ID('dbo.SGC_TipoTreinamento', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoTreinamento]; PRINT '  [OK] SGC_TipoTreinamento'; END

IF OBJECT_ID('dbo.SGC_TipoDocumentoTerceiro', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoDocumentoTerceiro]; PRINT '  [OK] SGC_TipoDocumentoTerceiro'; END

IF OBJECT_ID('dbo.SGC_TipoChecklist', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoChecklist]; PRINT '  [OK] SGC_TipoChecklist'; END

IF OBJECT_ID('dbo.SGC_StatusAcesso', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_StatusAcesso]; PRINT '  [OK] SGC_StatusAcesso'; END

IF OBJECT_ID('dbo.SGC_MotivoAcesso', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_MotivoAcesso]; PRINT '  [OK] SGC_MotivoAcesso'; END

IF OBJECT_ID('dbo.SGC_TipoAso', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoAso]; PRINT '  [OK] SGC_TipoAso'; END

IF OBJECT_ID('dbo.SGC_TipoPessoa', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoPessoa]; PRINT '  [OK] SGC_TipoPessoa'; END

IF OBJECT_ID('dbo.SGC_TipoVeiculo', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoVeiculo]; PRINT '  [OK] SGC_TipoVeiculo'; END

IF OBJECT_ID('dbo.SGC_TipoFornecedor', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoFornecedor]; PRINT '  [OK] SGC_TipoFornecedor'; END

IF OBJECT_ID('dbo.SGC_TipoParentesco', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoParentesco]; PRINT '  [OK] SGC_TipoParentesco'; END

IF OBJECT_ID('dbo.SGC_TipoSanguineo', 'U') IS NOT NULL
BEGIN DROP TABLE [dbo].[SGC_TipoSanguineo]; PRINT '  [OK] SGC_TipoSanguineo'; END

PRINT '';

-- =============================================================================
-- PARTE 6: REMOVER VIEWS E PROCEDURES (SE EXISTIREM)
-- =============================================================================

PRINT '>>> REMOVENDO VIEWS E PROCEDURES...';

IF OBJECT_ID('dbo.vw_SGC_DicionarioDados', 'V') IS NOT NULL
BEGIN DROP VIEW [dbo].[vw_SGC_DicionarioDados]; PRINT '  [OK] vw_SGC_DicionarioDados'; END

IF OBJECT_ID('dbo.sp_SGC_GerarDicionarioDados', 'P') IS NOT NULL
BEGIN DROP PROCEDURE [dbo].[sp_SGC_GerarDicionarioDados]; PRINT '  [OK] sp_SGC_GerarDicionarioDados'; END

PRINT '';

-- =============================================================================
-- VERIFICAÇÃO FINAL
-- =============================================================================

PRINT '=============================================================================';
PRINT 'VERIFICANDO SE RESTOU ALGUMA TABELA SGC...';
PRINT '=============================================================================';

SELECT TABLE_NAME AS [Tabelas SGC Restantes]
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
  AND TABLE_NAME LIKE 'SGC_%'
ORDER BY TABLE_NAME;

IF @@ROWCOUNT = 0
    PRINT '✅ LIMPEZA CONCLUÍDA - Nenhuma tabela SGC encontrada!';
ELSE
    PRINT '⚠️  ATENÇÃO: Ainda existem tabelas SGC no banco!';

PRINT '';
PRINT '=============================================================================';
GO