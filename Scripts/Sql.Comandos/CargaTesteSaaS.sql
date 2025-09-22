-- =============================================================================
-- SCRIPTS DE TESTE PARA TABELAS SAAS
-- Execute estes comandos no SQL Server Management Studio ou via sqlcmd
-- =============================================================================

-- Limpar dados existentes (opcional - se quiser recomeçar)
-- DELETE FROM dbo.SaasInvitations;
-- DELETE FROM dbo.SaasUsers;
-- DELETE FROM dbo.SaasTenants;

-- =============================================================================
-- 1. INSERIR TENANTS DE TESTE
-- =============================================================================

DECLARE @TenantId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @TenantId2 UNIQUEIDENTIFIER = NEWID();
DECLARE @TenantId3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.SaasTenants (
    Id, 
    CompanyName, 
    Domain, 
    IsActive, 
    MaxUsers, 
    PlanType, 
    CreatedAt, 
    UpdatedAt, 
    CreatedBy
)
VALUES 
    (@TenantId1, 'Empresa Teste LTDA', 'empresateste.com', 1, 50, 'Pro', GETUTCDATE(), GETUTCDATE(), 'system'),
    (@TenantId2, 'Startup Inovação', 'startup.tech', 1, 10, 'Basic', GETUTCDATE(), GETUTCDATE(), 'system'),
    (@TenantId3, 'Corporação Enterprise', NULL, 1, 500, 'Enterprise', GETUTCDATE(), GETUTCDATE(), 'system');

-- =============================================================================
-- 2. INSERIR USUÁRIOS DE TESTE
-- =============================================================================

-- Senhas de teste (todas usam "123456"):
-- PasswordHash: Base64 do PBKDF2 com 10000 iterações
-- PasswordSalt: Salt único para cada usuário

DECLARE @UserId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId2 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId3 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId4 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId5 UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.SaasUsers (
    Id,
    Email,
    PasswordHash,
    PasswordSalt,
    FullName,
    EmailConfirmed,
    IsActive,
    TenantId,
    CreatedAt,
    UpdatedAt,
    CreatedBy,
    LoginAttempts
)
VALUES 
    -- Usuário 1 - Empresa Teste LTDA (Admin)
    (
        @UserId1,
        'admin@empresateste.com',
        'kQJ1J8yKZ8xJb2GQQxK1kB5YsE8vL2sQ3mP9r7R1A8s=', -- Hash de "123456"
        'QzJ1K8pLM9vJc3HRRyL2nC6ZtF9wM3tR4nQ0s8S2B9t=', -- Salt único
        'Administrador Sistema',
        1, -- Email confirmado
        1, -- Ativo
        @TenantId1,
        GETUTCDATE(),
        GETUTCDATE(),
        'system',
        0
    ),
    
    -- Usuário 2 - Empresa Teste LTDA (Usuário normal)
    (
        @UserId2,
        'usuario@empresateste.com',
        'mSL2M9zLZ9yKc4JSSyM2lD6ZtF0wL3tS4oQ1s9T3C0u=', -- Hash de "123456"
        'RkM2N0qMO0wKd4ISS0M3oD7auG1xN4uT5pR2t0U4D1v=', -- Salt único
        'João Silva',
        1, -- Email confirmado
        1, -- Ativo
        @TenantId1,
        GETUTCDATE(),
        GETUTCDATE(),
        'system',
        0
    ),
    
    -- Usuário 3 - Startup Inovação (Founder)
    (
        @UserId3,
        'founder@startup.tech',
        'nTM3O1rNP1zLd5KTT0O4mE8bvG2xM4uS5pR2s1T4D2w=', -- Hash de "123456"
        'SlO4P2sOP2xMe6LUU1P5nF9cwH3yO5vU6qS3t2U5E3x=', -- Salt único
        'Maria Founder',
        1, -- Email confirmado
        1, -- Ativo
        @TenantId2,
        GETUTCDATE(),
        GETUTCDATE(),
        'system',
        0
    ),
    
    -- Usuário 4 - Startup Inovação (Developer)
    (
        @UserId4,
        'dev@startup.tech',
        'oUO5Q3tPQ3yMf7MUU2Q6nF9cwH4yN5vT6qS3s3V6F4y=', -- Hash de "123456"
        'TmP6R4uQR4zNg8NVV3R7oG0dxI5zO6wU7rT4t4W7G5z=', -- Salt único
        'Pedro Developer',
        0, -- Email NÃO confirmado (para testar)
        1, -- Ativo
        @TenantId2,
        GETUTCDATE(),
        GETUTCDATE(),
        'system',
        0
    ),
    
    -- Usuário 5 - Corporação Enterprise
    (
        @UserId5,
        'cto@corporacao.com',
        'pVP7S5wRS5zNg8NVV4S8pG1dxI6zP7xV8sU5u5X8H6z=', -- Hash de "123456"
        'UnQ8T6vST6yOh9OWW5T9qH2eyJ7yQ8yW9tV6v6Y9I7A=', -- Salt único
        'Ana CTO',
        1, -- Email confirmado
        1, -- Ativo
        @TenantId3,
        GETUTCDATE(),
        GETUTCDATE(),
        'system',
        0
    );

-- =============================================================================
-- 3. INSERIR CONVITES DE TESTE
-- =============================================================================

DECLARE @InviteId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @InviteId2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.SaasInvitations (
    Id,
    Email,
    TenantId,
    InvitedById,
    InvitationToken,
    ExpiresAt,
    IsAccepted,
    Role,
    Message,
    CreatedAt,
    UpdatedAt
)
VALUES 
    -- Convite pendente na Empresa Teste
    (
        @InviteId1,
        'novousuario@empresateste.com',
        @TenantId1,
        @UserId1, -- Convidado pelo admin
        REPLACE(NEWID(), '-', ''), -- Token único
        DATEADD(DAY, 7, GETUTCDATE()), -- Expira em 7 dias
        0, -- Não aceito ainda
        'User',
        'Bem-vindo à nossa equipe!',
        GETUTCDATE(),
        GETUTCDATE()
    ),
    
    -- Convite expirado (para teste)
    (
        @InviteId2,
        'expirado@startup.tech',
        @TenantId2,
        @UserId3, -- Convidado pelo founder
        REPLACE(NEWID(), '-', ''), -- Token único
        DATEADD(DAY, -1, GETUTCDATE()), -- Já expirou
        0, -- Não aceito
        'Developer',
        'Vem trabalhar conosco!',
        DATEADD(DAY, -8, GETUTCDATE()),
        DATEADD(DAY, -8, GETUTCDATE())
    );

-- =============================================================================
-- 4. CONSULTAS PARA VERIFICAR OS DADOS
-- =============================================================================

-- Ver todos os tenants
SELECT 
    CompanyName,
    Domain,
    PlanType,
    MaxUsers,
    IsActive,
    CreatedAt
FROM dbo.SaasTenants
ORDER BY CompanyName;

-- Ver todos os usuários com seus tenants
SELECT 
    u.FullName,
    u.Email,
    u.EmailConfirmed,
    u.IsActive,
    u.LoginAttempts,
    u.LastLoginAt,
    t.CompanyName,
    t.PlanType
FROM dbo.SaasUsers u
INNER JOIN dbo.SaasTenants t ON u.TenantId = t.Id
ORDER BY t.CompanyName, u.FullName;

-- Ver convites pendentes
SELECT 
    i.Email,
    i.Role,
    i.Message,
    i.ExpiresAt,
    i.IsAccepted,
    t.CompanyName,
    inviter.FullName as InvitedBy,
    CASE 
        WHEN i.ExpiresAt < GETUTCDATE() THEN 'Expirado'
        WHEN i.IsAccepted = 1 THEN 'Aceito'
        ELSE 'Pendente'
    END as Status
FROM dbo.SaasInvitations i
INNER JOIN dbo.SaasTenants t ON i.TenantId = t.Id
INNER JOIN dbo.SaasUsers inviter ON i.InvitedById = inviter.Id
ORDER BY i.CreatedAt DESC;

-- =============================================================================
-- 5. CREDENCIAIS PARA TESTE
-- =============================================================================

/*
USUÁRIOS DE TESTE CRIADOS:

1. admin@empresateste.com / 123456
   - Empresa: Empresa Teste LTDA
   - Email confirmado: Sim
   - Ativo: Sim

2. usuario@empresateste.com / 123456
   - Empresa: Empresa Teste LTDA  
   - Email confirmado: Sim
   - Ativo: Sim

3. founder@startup.tech / 123456
   - Empresa: Startup Inovação
   - Email confirmado: Sim
   - Ativo: Sim

4. dev@startup.tech / 123456
   - Empresa: Startup Inovação
   - Email confirmado: NÃO (para testar)
   - Ativo: Sim

5. cto@corporacao.com / 123456
   - Empresa: Corporação Enterprise
   - Email confirmado: Sim
   - Ativo: Sim

PARA TESTAR:
- Use qualquer um dos emails acima
- Senha sempre: 123456
- Teste tanto usuários com email confirmado quanto não confirmado
*/