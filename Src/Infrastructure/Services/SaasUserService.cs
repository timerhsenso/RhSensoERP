using Microsoft.Extensions.Logging;
using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Common;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Security.SaaS;
using RhSensoERP.Infrastructure.Auth.Strategies;

namespace RhSensoERP.Infrastructure.Services;

/// <summary>
/// Serviço de usuários SaaS - implementa lógica de negócio para autenticação SaaS
/// Segue princípios de Clean Architecture com separação clara de responsabilidades
/// </summary>
public class SaasUserService : ISaasUserService
{
    private readonly ISaasUserRepository _saasUserRepository;
    private readonly SaasAuthStrategy _authStrategy;
    private readonly ILogger<SaasUserService> _logger;

    public SaasUserService(
        ISaasUserRepository saasUserRepository,
        SaasAuthStrategy authStrategy,
        ILogger<SaasUserService> logger)
    {
        _saasUserRepository = saasUserRepository ?? throw new ArgumentNullException(nameof(saasUserRepository));
        _authStrategy = authStrategy ?? throw new ArgumentNullException(nameof(authStrategy));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Autentica usuário SaaS
    /// </summary>
    public async Task<Result<SaasAuthenticationResponse>> AuthenticateAsync(SaasAuthenticationRequest request)
    {
        try
        {
            _logger.LogDebug("Iniciando autenticação SaaS para {Email}", request.Email);

            // Validações de entrada
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<SaasAuthenticationResponse>.Failure("Email é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<SaasAuthenticationResponse>.Failure("Senha é obrigatória");
            }

            // Buscar usuário por email
            var user = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com email não cadastrado: {Email}", request.Email);
                return Result<SaasAuthenticationResponse>.Failure("Credenciais inválidas");
            }

            // Verificar se usuário pode fazer login
            if (!user.CanLogin)
            {
                var reason = !user.IsActive ? "Usuário inativo" :
                           !user.EmailConfirmed ? "Email não confirmado" :
                           user.IsLocked ? "Usuário temporariamente bloqueado" :
                           "Usuário não autorizado";

                _logger.LogWarning("Login negado para {Email}: {Reason}", request.Email, reason);
                return Result<SaasAuthenticationResponse>.Failure(reason);
            }

            // Validar senha
            var isPasswordValid = _authStrategy.IsPasswordValid(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Senha inválida para usuário {Email}", request.Email);

                // Incrementar tentativas de login
                user.IncrementLoginAttempts();
                await _saasUserRepository.UpdateAsync(user);

                return Result<SaasAuthenticationResponse>.Failure("Credenciais inválidas");
            }

            // Sucesso - gerar tokens
            var token = _authStrategy.GenerateToken(user);
            var refreshToken = _authStrategy.GenerateRefreshToken();

            // Atualizar usuário com novo refresh token e último login
            user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            user.UpdateLastLogin();
            await _saasUserRepository.UpdateAsync(user);

            // Montar resposta
            var response = new SaasAuthenticationResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new SaasUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.FullName,
                    TenantId = user.TenantId
                }
            };

            _logger.LogInformation("Autenticação SaaS bem-sucedida para {Email}", user.Email);
            return Result<SaasAuthenticationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante autenticação SaaS para {Email}", request.Email);
            return Result<SaasAuthenticationResponse>.Failure("Erro interno do servidor");
        }
    }

    /// <summary>
    /// Registra novo usuário SaaS
    /// </summary>
    public async Task<Result<SaasRegistrationResponse>> RegisterAsync(SaasRegistrationRequest request)
    {
        try
        {
            _logger.LogDebug("Iniciando registro SaaS para {Email}", request.Email);

            // Validações de entrada
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<SaasRegistrationResponse>.Failure("Nome é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<SaasRegistrationResponse>.Failure("Email é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<SaasRegistrationResponse>.Failure("Senha é obrigatória");
            }

            if (request.Password.Length < 6)
            {
                return Result<SaasRegistrationResponse>.Failure("Senha deve ter no mínimo 6 caracteres");
            }

            if (request.TenantId == Guid.Empty)
            {
                return Result<SaasRegistrationResponse>.Failure("TenantId é obrigatório");
            }

            // Verificar se email já existe
            var existingUser = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Tentativa de registro com email já existente: {Email}", request.Email);
                return Result<SaasRegistrationResponse>.Failure("Este email já está em uso");
            }

            // Verificar se email já existe no tenant (dupla validação)
            var emailExistsInTenant = await _saasUserRepository.EmailExistsInTenantAsync(request.Email, request.TenantId);
            if (emailExistsInTenant)
            {
                return Result<SaasRegistrationResponse>.Failure("Este email já está em uso neste tenant");
            }

            // Criar hash da senha
            var (passwordHash, salt) = _authStrategy.CreatePasswordHash(request.Password);

            // Criar novo usuário usando método factory
            var newUser = SaasUser.Create(
                request.Name,
                request.Email,
                passwordHash,
                salt,
                request.TenantId);

            // Salvar no banco
            await _saasUserRepository.AddAsync(newUser);

            // Montar resposta
            var response = new SaasRegistrationResponse
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Name = newUser.FullName,
                TenantId = newUser.TenantId,
                CreatedAt = newUser.CreatedAt
            };

            _logger.LogInformation("Registro SaaS bem-sucedido para {Email} no tenant {TenantId}",
                newUser.Email, newUser.TenantId);

            return Result<SaasRegistrationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante registro SaaS para {Email}", request.Email);
            return Result<SaasRegistrationResponse>.Failure("Erro interno do servidor");
        }
    }

    /// <summary>
    /// Renova token de acesso usando refresh token
    /// </summary>
    public async Task<Result<SaasAuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            _logger.LogDebug("Iniciando renovação de token SaaS");

            // Validação de entrada
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Result<SaasAuthenticationResponse>.Failure("Refresh token é obrigatório");
            }

            // Buscar usuário pelo refresh token
            var user = await _saasUserRepository.GetByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Refresh token não encontrado ou expirado");
                return Result<SaasAuthenticationResponse>.Failure("Refresh token inválido ou expirado");
            }

            // Verificar se refresh token ainda é válido (dupla validação)
            if (!user.IsRefreshTokenValid(request.RefreshToken))
            {
                _logger.LogWarning("Refresh token inválido para usuário {Email}", user.Email);
                return Result<SaasAuthenticationResponse>.Failure("Refresh token inválido ou expirado");
            }

            // Verificar se usuário ainda pode fazer login
            if (!user.CanLogin)
            {
                _logger.LogWarning("Usuário {Email} não pode mais fazer login durante refresh", user.Email);
                return Result<SaasAuthenticationResponse>.Failure("Usuário não autorizado");
            }

            // Gerar novos tokens
            var newToken = _authStrategy.GenerateToken(user);
            var newRefreshToken = _authStrategy.GenerateRefreshToken();

            // Atualizar usuário com novo refresh token
            user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _saasUserRepository.UpdateAsync(user);

            // Montar resposta
            var response = new SaasAuthenticationResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new SaasUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.FullName,
                    TenantId = user.TenantId
                }
            };

            _logger.LogInformation("Refresh token renovado com sucesso para usuário {Email}", user.Email);
            return Result<SaasAuthenticationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante renovação de token SaaS");
            return Result<SaasAuthenticationResponse>.Failure("Erro interno do servidor");
        }
    }
}