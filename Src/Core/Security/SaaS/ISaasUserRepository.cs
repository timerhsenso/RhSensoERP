using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Core.Security.SaaS;

/// <summary>
/// Interface para repositório de usuários SaaS
/// Define contratos específicos para operações com usuários SaaS
/// </summary>
public interface ISaasUserRepository
{
    /// <summary>
    /// Busca usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<SaasUser?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca usuário por refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<SaasUser?> GetByRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Busca usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<SaasUser?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca usuário por token de confirmação de email
    /// </summary>
    /// <param name="token">Token de confirmação</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<SaasUser?> GetByEmailConfirmationTokenAsync(string token);

    /// <summary>
    /// Busca usuário por token de reset de senha
    /// </summary>
    /// <param name="token">Token de reset</param>
    /// <returns>Usuário encontrado ou null</returns>
    Task<SaasUser?> GetByPasswordResetTokenAsync(string token);

    /// <summary>
    /// Adiciona novo usuário
    /// </summary>
    /// <param name="user">Usuário a ser adicionado</param>
    /// <returns>Task</returns>
    Task AddAsync(SaasUser user);

    /// <summary>
    /// Atualiza usuário existente
    /// </summary>
    /// <param name="user">Usuário a ser atualizado</param>
    /// <returns>Task</returns>
    Task UpdateAsync(SaasUser user);

    /// <summary>
    /// Remove usuário (soft delete)
    /// </summary>
    /// <param name="user">Usuário a ser removido</param>
    /// <returns>Task</returns>
    Task DeleteAsync(SaasUser user);

    /// <summary>
    /// Verifica se email já existe no tenant
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <param name="tenantId">ID do tenant</param>
    /// <returns>True se existe, false se não</returns>
    Task<bool> EmailExistsInTenantAsync(string email, Guid tenantId);

    /// <summary>
    /// Lista usuários do tenant com paginação
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="page">Página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista de usuários</returns>
    Task<(List<SaasUser> Users, int TotalCount)> GetByTenantAsync(Guid tenantId, int page = 1, int pageSize = 20);

    /// <summary>
    /// Conta usuários ativos do tenant
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <returns>Número de usuários ativos</returns>
    Task<int> CountActiveUsersByTenantAsync(Guid tenantId);
}