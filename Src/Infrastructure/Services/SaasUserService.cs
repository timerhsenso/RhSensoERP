using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Common;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Security.SaaS;
using RhSensoERP.Infrastructure.Auth.Strategies;

namespace RhSensoERP.Infrastructure.Services;

public class SaasUserService : ISaasUserService
{
    private readonly ISaasUserRepository _saasUserRepository;
    private readonly SaasAuthStrategy _authStrategy;

    public SaasUserService(
        ISaasUserRepository saasUserRepository,
        SaasAuthStrategy authStrategy)
    {
        _saasUserRepository = saasUserRepository ?? throw new ArgumentNullException(nameof(saasUserRepository));
        _authStrategy = authStrategy ?? throw new ArgumentNullException(nameof(authStrategy));
    }

    public async Task<Result<SaasAuthenticationResponse>> AuthenticateAsync(SaasAuthenticationRequest request)
    {
        try
        {
            var user = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<SaasAuthenticationResponse>.Failure("Credenciais inválidas.");
            }

            // Usando método público para validação de senha
            var isPasswordValid = _authStrategy.IsPasswordValid(request.Password, user.PasswordHash, user.Salt);
            if (!isPasswordValid)
            {
                return Result<SaasAuthenticationResponse>.Failure("Credenciais inválidas.");
            }

            var token = _authStrategy.GenerateToken(user);
            var refreshToken = _authStrategy.GenerateRefreshToken();

            // Atualizar refresh token no usuário
            user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            await _saasUserRepository.UpdateAsync(user);

            var response = new SaasAuthenticationResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new SaasUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    TenantId = user.TenantId
                }
            };

            return Result<SaasAuthenticationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<SaasAuthenticationResponse>.Failure($"Erro interno: {ex.Message}");
        }
    }

    public async Task<Result<SaasRegistrationResponse>> RegisterAsync(SaasRegistrationRequest request)
    {
        try
        {
            // Verificar se email já existe
            var existingUser = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result<SaasRegistrationResponse>.Failure("Email já está em uso.");
            }

            // Criar hash da senha
            var (passwordHash, salt) = _authStrategy.CreatePasswordHash(request.Password);

            // Criar novo usuário
            var newUser = SaasUser.Create(
                request.Name,
                request.Email,
                passwordHash,
                salt,
                request.TenantId);

            await _saasUserRepository.AddAsync(newUser);

            var response = new SaasRegistrationResponse
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Name = newUser.Name,
                TenantId = newUser.TenantId,
                CreatedAt = newUser.CreatedAt
            };

            return Result<SaasRegistrationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<SaasRegistrationResponse>.Failure($"Erro interno: {ex.Message}");
        }
    }

    public async Task<Result<SaasAuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var user = await _saasUserRepository.GetByRefreshTokenAsync(request.RefreshToken);
            if (user == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                return Result<SaasAuthenticationResponse>.Failure("Refresh token inválido ou expirado.");
            }

            var newToken = _authStrategy.GenerateToken(user);
            var newRefreshToken = _authStrategy.GenerateRefreshToken();

            user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _saasUserRepository.UpdateAsync(user);

            var response = new SaasAuthenticationResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new SaasUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    TenantId = user.TenantId
                }
            };

            return Result<SaasAuthenticationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<SaasAuthenticationResponse>.Failure($"Erro interno: {ex.Message}");
        }
    }
}