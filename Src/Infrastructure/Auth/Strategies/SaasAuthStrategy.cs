using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Application.Auth;
using RhSensoERP.Core.Security.Auth;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Security.SaaS;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RhSensoERP.Infrastructure.Auth.Strategies;

/// <summary>
/// Estratégia de autenticação SaaS com JWT e validação de senha segura
/// Implementa IAuthStrategy para integração com sistema unificado
/// </summary>
public class SaasAuthStrategy : IAuthStrategy
{
    private readonly ISaasUserRepository _userRepository;
    private readonly AuthOptions _authOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<SaasAuthStrategy> _logger;

    public AuthMode Mode => AuthMode.SaaS;

    public SaasAuthStrategy(
        ISaasUserRepository userRepository,
        IOptionsMonitor<AuthOptions> authOptions,
        IOptionsMonitor<JwtOptions> jwtOptions,
        ILogger<SaasAuthStrategy> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _authOptions = authOptions.CurrentValue;
        _jwtOptions = jwtOptions.CurrentValue;
        _logger = logger;
    }

    /// <summary>
    /// Executa autenticação SaaS
    /// </summary>
    public async Task<AuthStrategyResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Iniciando autenticação SaaS para email {Email}", request.UserOrEmail);

            // Validação básica
            if (string.IsNullOrWhiteSpace(request.UserOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: "Email e senha são obrigatórios",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "SaaS");
            }

            // Buscar usuário por email
            var user = await _userRepository.GetByEmailAsync(request.UserOrEmail!);
            if (user == null)
            {
                _logger.LogWarning("Usuário SaaS não encontrado para email {Email}", request.UserOrEmail);
                return new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: "Credenciais inválidas",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "SaaS");
            }

            // Verificar se usuário pode fazer login
            if (!user.CanLogin)
            {
                var reason = !user.IsActive ? "inativo" :
                           !user.EmailConfirmed ? "email não confirmado" :
                           user.IsLocked ? "temporariamente bloqueado" : "não autorizado";

                _logger.LogWarning("Login SaaS negado para {Email}: usuário {Reason}", request.UserOrEmail, reason);

                return new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: $"Usuário {reason}",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "SaaS");
            }

            // Validar senha
            var isPasswordValid = IsPasswordValid(request.Password!, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Senha inválida para usuário SaaS {Email}", request.UserOrEmail);

                // Incrementar tentativas de login
                user.IncrementLoginAttempts();
                await _userRepository.UpdateAsync(user);

                return new AuthStrategyResult(
                    Success: false,
                    ErrorMessage: "Credenciais inválidas",
                    UserKey: null,
                    DisplayName: null,
                    Provider: "SaaS");
            }

            // Sucesso - atualizar último login
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Autenticação SaaS bem-sucedida para {Email}", user.Email);

            return new AuthStrategyResult(
                Success: true,
                ErrorMessage: null,
                UserKey: user.Email,
                DisplayName: user.FullName,
                Provider: "SaaS",
                TenantId: user.TenantId,
                AdditionalData: new Dictionary<string, object>
                {
                    ["UserId"] = user.Id,
                    ["Email"] = user.Email,
                    ["FullName"] = user.FullName,
                    ["TenantId"] = user.TenantId,
                    ["EmailConfirmed"] = user.EmailConfirmed,
                    ["LastLoginAt"] = user.LastLoginAt,
                    ["User"] = user // Para uso no serviço
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante autenticação SaaS para {Email}", request.UserOrEmail);

            return new AuthStrategyResult(
                Success: false,
                ErrorMessage: "Erro interno do servidor",
                UserKey: null,
                DisplayName: null,
                Provider: "SaaS");
        }
    }

    // ========================================
    // MÉTODOS AUXILIARES PARA AUTENTICAÇÃO SAAS
    // ========================================

    /// <summary>
    /// Cria hash da senha com salt único
    /// </summary>
    public (string passwordHash, string salt) CreatePasswordHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        var salt = GenerateSalt();
        var hash = HashPassword(password, salt);

        return (hash, salt);
    }

    /// <summary>
    /// Valida se a senha fornecida corresponde ao hash armazenado
    /// </summary>
    public bool IsPasswordValid(string password, string storedHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(storedHash) ||
            string.IsNullOrWhiteSpace(salt))
            return false;

        var computedHash = HashPassword(password, salt);
        return storedHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gera token JWT para o usuário autenticado
    /// </summary>
    public string GenerateToken(SaasUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetSigningKey());

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim("user_type", "saas"),
            new Claim("email_confirmed", user.EmailConfirmed.ToString().ToLowerInvariant())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.SaaS.TokenExpirationMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Gera refresh token único
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Valida se o token JWT é válido
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetSigningKey());

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    // ========================================
    // MÉTODOS PRIVADOS
    // ========================================

    /// <summary>
    /// Obtém chave de assinatura - usa SecretKey para desenvolvimento
    /// </summary>
    private string GetSigningKey()
    {
        return _jwtOptions.SecretKey ?? "RhSensoERP-Super-Secret-Key-For-Development-Only-2024!";
    }

    /// <summary>
    /// Gera salt único para hash de senha
    /// </summary>
    private static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Cria hash da senha usando PBKDF2
    /// </summary>
    private static string HashPassword(string password, string salt)
    {
        const int iterations = 10000;
        const int hashLength = 32;

        var saltBytes = Convert.FromBase64String(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(hashLength);

        return Convert.ToBase64String(hashBytes);
    }
}