select * from SaasUsers
where Email = 'cto@corporacao.com'
--and PasswordHash = '123456'
and PasswordSalt = '123456'



-- 1. Verificar se o usu�rio existe (sem filtros de senha)
SELECT 
    Email,
    EmailConfirmed,
    IsActive,
    LoginAttempts,
    LockedUntil,
    TenantId,
    CreatedAt,
    LEN(PasswordHash) as HashLength,
    LEN(PasswordSalt) as SaltLength
FROM SaasUsers 
WHERE Email = 'cto@corporacao.com';

-- 2. Verificar todos os usu�rios na tabela
SELECT 
    Email,
    EmailConfirmed,
    IsActive,
    LoginAttempts,
    CreatedAt
FROM SaasUsers 
ORDER BY CreatedAt DESC;

-- 3. Verificar se existe algum tenant
SELECT * FROM SaasTenants;

-- 4. Se quiser deletar o usu�rio problem�tico para recri�-lo:
-- DELETE FROM SaasUsers WHERE Email = 'cto@corporacao.com';