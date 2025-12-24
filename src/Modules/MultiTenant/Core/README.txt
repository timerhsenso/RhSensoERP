MVP SaaS Admin - ZIP V2 (Hardening)
CdSistema: SAS

Conteúdo:
- sql/MVP_SaaS_Admin_v2_Hardening.sql
- entities/*.cs (entidades com IsDeleted e validações)
- Enums/*.cs (enums opcionais)

O SQL V2:
- Adiciona IsDeleted (soft delete) em SaasTenants, SaasUsers, SaasPlans (se ainda não existir).
- Adiciona CHECK constraints para Status e domínios.
- Adiciona constraints financeiras (valores não-negativos).
- Cria índice filtrado: 1 assinatura ativa por tenant (Status='Active' e EndAt IS NULL).
