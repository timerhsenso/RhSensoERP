// ============================================================================
// ARQUIVO: src/Identity/Application/Features/Auth/Commands/LoginCommand.cs
// AÇÃO: SUBSTITUIR COMPLETAMENTE
// ============================================================================
// CORREÇÃO FASE 1: Alterado CdUsuario para LoginIdentifier
// ============================================================================

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Auth.Commands;

/// <summary>
/// Command para login de usuário.
/// </summary>
public sealed record LoginCommand(LoginRequest Request, string IpAddress, string? UserAgent)
    : IRequest<Result<AuthResponse>>;

/// <summary>
/// Handler do LoginCommand com tratamento de timeout.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _validator;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAuthService authService,
        IValidator<LoginRequest> validator,
        ILogger<LoginCommandHandler> logger)
    {
        _authService = authService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        try
        {
            // ✅ LOG: Início da validação (CORRIGIDO: LoginIdentifier)
            _logger.LogDebug("🔍 Iniciando validação do LoginRequest para {LoginIdentifier}", request.Request.LoginIdentifier);

            // ✅ VALIDAÇÃO com timeout de 5 segundos
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            var validationResult = await _validator.ValidateAsync(request.Request, cts.Token);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("❌ Validação falhou: {Errors}", errors);
                return Result<AuthResponse>.Failure("VALIDATION_ERROR", errors);
            }

            // ✅ LOG: Validação OK
            _logger.LogDebug("✅ Validação OK, chamando AuthService");

            return await _authService.LoginAsync(request.Request, request.IpAddress, request.UserAgent, ct);
        }
        catch (OperationCanceledException)
        {
            // ✅ LOG: Timeout (CORRIGIDO: LoginIdentifier)
            _logger.LogWarning("⏱️ Timeout na validação de login para {LoginIdentifier}", request.Request.LoginIdentifier);
            return Result<AuthResponse>.Failure("TIMEOUT", "A requisição excedeu o tempo limite.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro inesperado no LoginCommandHandler");
            return Result<AuthResponse>.Failure("INTERNAL_ERROR", "Erro ao processar requisição de login.");
        }
    }
}
