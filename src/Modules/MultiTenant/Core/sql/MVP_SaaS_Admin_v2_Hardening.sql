/* =====================================================================
   MVP SAAS ADMIN - V2 HARDENING (SQL SERVER / dbo)
   CdSistema: SAS
   Inclui:
   - CHECK constraints (Status / domínios)
   - Soft delete (IsDeleted) em tabelas "vivas"
   - Índice filtrado: 1 assinatura ativa por Tenant
   - Constraints financeiras (valores >= 0)
   ===================================================================== */

-- Este script assume que dbo.SaasTenants já existe com PK (Id).
-- Ele não recria dbo.SaasTenants nem altera colunas existentes, apenas hardening.

SET NOCOUNT ON;
GO

/* ==========================================================
   1) Soft Delete (IsDeleted) - tabelas "vivas"
   ========================================================== */

-- SaasTenants
IF COL_LENGTH('dbo.SaasTenants', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.SaasTenants
    ADD IsDeleted BIT NOT NULL CONSTRAINT DF_SaasTenants_IsDeleted DEFAULT (CONVERT(bit,(0)));
END
GO

-- SaasUsers
IF COL_LENGTH('dbo.SaasUsers', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.SaasUsers
    ADD IsDeleted BIT NOT NULL CONSTRAINT DF_SaasUsers_IsDeleted DEFAULT (CONVERT(bit,(0)));
END
GO

-- SaasPlans
IF COL_LENGTH('dbo.SaasPlans', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.SaasPlans
    ADD IsDeleted BIT NOT NULL CONSTRAINT DF_SaasPlans_IsDeleted DEFAULT (CONVERT(bit,(0)));
END
GO


/* ==========================================================
   2) CHECK Constraints (domínios)
   ========================================================== */

-- SaasSubscriptions.Status
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasSubscriptions_Status'
)
BEGIN
    ALTER TABLE dbo.SaasSubscriptions
    ADD CONSTRAINT CK_SaasSubscriptions_Status
    CHECK (Status IN ('Trialing','Active','PastDue','Canceled'));
END
GO

-- SaasInvoices.Status
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasInvoices_Status'
)
BEGIN
    ALTER TABLE dbo.SaasInvoices
    ADD CONSTRAINT CK_SaasInvoices_Status
    CHECK (Status IN ('Draft','Open','Paid','Void','Uncollectible'));
END
GO

-- SaasPayments.Status
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasPayments_Status'
)
BEGIN
    ALTER TABLE dbo.SaasPayments
    ADD CONSTRAINT CK_SaasPayments_Status
    CHECK (Status IN ('Pending','Succeeded','Failed','Refunded'));
END
GO

-- SaasPlanPrices.BillingCycle
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasPlanPrices_BillingCycle'
)
BEGIN
    ALTER TABLE dbo.SaasPlanPrices
    ADD CONSTRAINT CK_SaasPlanPrices_BillingCycle
    CHECK (BillingCycle IN ('Monthly','Yearly'));
END
GO

-- SaasUserTenants.Status
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasUserTenants_Status'
)
BEGIN
    ALTER TABLE dbo.SaasUserTenants
    ADD CONSTRAINT CK_SaasUserTenants_Status
    CHECK (Status IN ('Active','Invited','Disabled'));
END
GO

-- SaasUserTenants.Role
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasUserTenants_Role'
)
BEGIN
    ALTER TABLE dbo.SaasUserTenants
    ADD CONSTRAINT CK_SaasUserTenants_Role
    CHECK (Role IN ('Owner','Admin','Billing','Viewer'));
END
GO


/* ==========================================================
   3) Constraints financeiras (não-negativos / coerência)
   ========================================================== */

-- SaasPlanPrices.Amount / SetupFee
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasPlanPrices_Amounts'
)
BEGIN
    ALTER TABLE dbo.SaasPlanPrices
    ADD CONSTRAINT CK_SaasPlanPrices_Amounts
    CHECK (Amount >= 0 AND (SetupFee IS NULL OR SetupFee >= 0) AND (TrialDays IS NULL OR TrialDays >= 0));
END
GO

-- SaasInvoices.TotalAmount
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasInvoices_TotalAmount'
)
BEGIN
    ALTER TABLE dbo.SaasInvoices
    ADD CONSTRAINT CK_SaasInvoices_TotalAmount
    CHECK (TotalAmount >= 0);
END
GO

-- SaasInvoiceLines (Qty > 0, valores >= 0)
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasInvoiceLines_Amounts'
)
BEGIN
    ALTER TABLE dbo.SaasInvoiceLines
    ADD CONSTRAINT CK_SaasInvoiceLines_Amounts
    CHECK (Qty > 0 AND UnitAmount >= 0 AND Amount >= 0);
END
GO

-- SaasPayments.Amount
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_SaasPayments_Amount'
)
BEGIN
    ALTER TABLE dbo.SaasPayments
    ADD CONSTRAINT CK_SaasPayments_Amount
    CHECK (Amount >= 0);
END
GO


/* ==========================================================
   4) Índice filtrado: 1 assinatura ativa por Tenant
   ========================================================== */

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_SaasSubscriptions_OneActive'
      AND object_id = OBJECT_ID('dbo.SaasSubscriptions')
)
BEGIN
    CREATE UNIQUE INDEX UX_SaasSubscriptions_OneActive
    ON dbo.SaasSubscriptions(TenantId)
    WHERE Status = 'Active' AND EndAt IS NULL;
END
GO
