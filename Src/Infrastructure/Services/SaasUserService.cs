using Microsoft.Extensions.Logging;
using RhSensoERP.Application.Security.SaaS;
using RhSensoERP.Core.Abstractions.Services;
using RhSensoERP.Core.Common;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Security.SaaS;
using RhSensoERP.Core.Shared;
using RhSensoERP.Infrastructure.Auth.Strategies;

namespace RhSensoERP.Infrastructure.Services;

/// <summary>
/// Serviço de usuários SaaS - implementa lógica de negócio para autenticação SaaS
/// Segue princípios de Clean Architecture com separação clara de responsabilidades
/// Implementa funcionalidades de registro, autenticação, confirmação de email e renovação de tokens
/// </summary>
public class SaasUserService : ISaasUserService
{
    private readonly ISaasUserRepository _saasUserRepository;
    private readonly SaasAuthStrategy _authStrategy;
    private readonly ILogger<SaasUserService> _logger;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Construtor do serviço SaaS com todas as dependências necessárias
    /// </summary>
    /// <param name="saasUserRepository">Repositório para operações de dados dos usuários SaaS</param>
    /// <param name="authStrategy">Estratégia de autenticação para geração de tokens e validação de senhas</param>
    /// <param name="logger">Logger estruturado para auditoria e debugging</param>
    /// <param name="emailService">Serviço para envio de emails de confirmação e notificações</param>
    public SaasUserService(
        ISaasUserRepository saasUserRepository,
        SaasAuthStrategy authStrategy,
        ILogger<SaasUserService> logger,
        IEmailService emailService)
    {
        _saasUserRepository = saasUserRepository ?? throw new ArgumentNullException(nameof(saasUserRepository));
        _authStrategy = authStrategy ?? throw new ArgumentNullException(nameof(authStrategy));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    /// <summary>
    /// Autentica usuário SaaS validando credenciais e estado da conta
    /// Implementa controle de tentativas de login e bloqueio temporário por segurança
    /// </summary>
    /// <param name="request">Dados de autenticação contendo email e senha</param>
    /// <returns>Resultado da autenticação com tokens JWT e dados do usuário</returns>
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
    /// Registra novo usuário SaaS com validações de segurança e envio de email de confirmação
    /// Cria usuário com email não confirmado por padrão, requerindo validação por email
    /// </summary>
    /// <param name="request">Dados de registro contendo nome, email, senha e tenant</param>
    /// <returns>Resultado do registro com dados do usuário criado</returns>
    public async Task<ServiceResult<SaasRegistrationResponse>> RegisterAsync(SaasRegistrationRequest request)
    {
        try
        {
            _logger.LogDebug("Iniciando registro SaaS para {Email}", request.Email);

            // Validações de entrada
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<SaasRegistrationResponse>.Fail("Nome é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return ServiceResult<SaasRegistrationResponse>.Fail("Email é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return ServiceResult<SaasRegistrationResponse>.Fail("Senha é obrigatória");
            }

            if (request.Password.Length < 6)
            {
                return ServiceResult<SaasRegistrationResponse>.Fail("Senha deve ter no mínimo 6 caracteres");
            }

            if (request.TenantId == Guid.Empty)
            {
                return ServiceResult<SaasRegistrationResponse>.Fail("TenantId é obrigatório");
            }

            // Verificar se email já existe
            var existingUser = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Tentativa de registro com email já existente: {Email}", request.Email);
                return ServiceResult<SaasRegistrationResponse>.Fail("Este email já está em uso");
            }

            // Verificar se email já existe no tenant (dupla validação)
            var emailExistsInTenant = await _saasUserRepository.EmailExistsInTenantAsync(request.Email, request.TenantId);
            if (emailExistsInTenant)
            {
                return ServiceResult<SaasRegistrationResponse>.Fail("Este email já está em uso neste tenant");
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

            // NOVA FUNCIONALIDADE: Enviar email de confirmação
            var emailSent = await _emailService.SendEmailConfirmationAsync(
                newUser.Email,
                newUser.FullName,
                newUser.EmailConfirmationToken!);

            if (!emailSent)
            {
                _logger.LogWarning("Usuário criado mas falha no envio do email de confirmação: {Email}", request.Email);
            }

            // Montar resposta usando as propriedades corretas
            var response = new SaasRegistrationResponse
            {
                Id = newUser.Id,
                Name = newUser.FullName,
                Email = newUser.Email,
                TenantId = newUser.TenantId,
                CreatedAt = newUser.CreatedAt,
                RequiresEmailConfirmation = true,
                EmailConfirmationSent = emailSent,
                Message = emailSent ? "Usuário registrado com sucesso. Verifique seu email para confirmar a conta."
                                   : "Usuário registrado com sucesso, mas houve falha no envio do email de confirmação."
            };

            _logger.LogInformation("Registro SaaS bem-sucedido para {Email} no tenant {TenantId}",
                newUser.Email, newUser.TenantId);

            return ServiceResult<SaasRegistrationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante registro SaaS para {Email}", request.Email);
            return ServiceResult<SaasRegistrationResponse>.Fail("Erro interno do servidor");
        }
    }

    /// <summary>
    /// Renova token de acesso usando refresh token válido
    /// Invalida o refresh token antigo e gera novos tokens por segurança
    /// </summary>
    /// <param name="request">Refresh token a ser validado e renovado</param>
    /// <returns>Novos tokens de acesso e refresh token</returns>
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

    /// <summary>
    /// Confirma email do usuário usando token enviado por email
    /// Ativa a conta permitindo login após validação do token de confirmação
    /// </summary>
    /// <param name="request">Email e token de confirmação</param>
    /// <returns>Status da confirmação e dados da operação</returns>
    public async Task<ServiceResult<ConfirmEmailResponse>> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        try
        {
            var normalizedEmail = request.Email.Trim().ToUpperInvariant();

            // Buscar usuário
            var user = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de confirmação de email para usuário inexistente: {Email}", request.Email);
                return ServiceResult<ConfirmEmailResponse>.Fail("Usuário não encontrado");
            }

            // Verificar se já está confirmado
            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Tentativa de confirmação de email já confirmado: {Email}", request.Email);
                return ServiceResult<ConfirmEmailResponse>.Success(new ConfirmEmailResponse
                {
                    EmailConfirmed = true,
                    Message = "Email já estava confirmado",
                    ConfirmedAt = user.UpdatedAt
                });
            }

            // Validar token
            if (string.IsNullOrEmpty(user.EmailConfirmationToken) ||
                user.EmailConfirmationToken != request.Token)
            {
                _logger.LogWarning("Token de confirmação inválido para {Email}", request.Email);
                return ServiceResult<ConfirmEmailResponse>.Fail("Token de confirmação inválido");
            }

            // Confirmar email
            user.ConfirmEmail();
            await _saasUserRepository.UpdateAsync(user);

            _logger.LogInformation("Email confirmado com sucesso para {Email}", request.Email);

            // Enviar email de boas-vindas (assíncrono, não bloqueia resposta)
            _ = Task.Run(async () => await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName));

            return ServiceResult<ConfirmEmailResponse>.Success(new ConfirmEmailResponse
            {
                EmailConfirmed = true,
                Message = "Email confirmado com sucesso",
                ConfirmedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante confirmação de email para {Email}", request.Email);
            return ServiceResult<ConfirmEmailResponse>.Fail("Erro interno do servidor");
        }
    }

    /// <summary>
    /// Reenvia email de confirmação para usuários com email não confirmado
    /// Gera novo token por segurança antes do reenvio
    /// </summary>
    /// <param name="request">Email do usuário que solicita reenvio</param>
    /// <returns>Status do reenvio do email de confirmação</returns>
    public async Task<ServiceResult<object>> ResendConfirmationEmailAsync(ResendConfirmationRequest request)
    {
        try
        {
            var user = await _saasUserRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de reenvio de confirmação para usuário inexistente: {Email}", request.Email);
                return ServiceResult<object>.Fail("Usuário não encontrado");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Tentativa de reenvio para email já confirmado: {Email}", request.Email);
                return ServiceResult<object>.Fail("Email já está confirmado");
            }

            // Gerar novo token (por segurança)
            user.GenerateEmailConfirmationToken();
            await _saasUserRepository.UpdateAsync(user);

            // Enviar email
            var emailSent = await _emailService.SendEmailConfirmationAsync(
                user.Email,
                user.FullName,
                user.EmailConfirmationToken!);

            if (!emailSent)
            {
                _logger.LogError("Falha ao enviar email de confirmação para {Email}", request.Email);
                return ServiceResult<object>.Fail("Falha ao enviar email");
            }

            _logger.LogInformation("Email de confirmação reenviado para {Email}", request.Email);
            return ServiceResult<object>.Success(null, "Email de confirmação reenviado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante reenvio de confirmação para {Email}", request.Email);
            return ServiceResult<object>.Fail("Erro interno do servidor");
        }
    }
}
