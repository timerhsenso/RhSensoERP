select * from SaasTenants

select * from SaasUsers


delete SaasUsers where Email = 'cadu20a@hotmail.com'


UPDATE dbo.SaasUsers 
SET EmailConfirmed = 1, 
    EmailConfirmationToken = NULL,
    UpdatedAt = GETUTCDATE()
WHERE Email = 'cadu20a@hotmail.com';