using FluentValidation;
using MediatR;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Auth.Commands;

/// <summary>
/// Command para renovação de tokens.
/// </summary>
public sealed record RefreshTokenCommand(RefreshTokenRequest Request, string IpAddress)
    : IRequest<Result<AuthResponse>>;

/// <summary>
/// Handler do RefreshTokenCommand.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;
    private readonly IValidator<RefreshTokenRequest> _validator;

    public RefreshTokenCommandHandler(IAuthService authService, IValidator<RefreshTokenRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request.Request, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<AuthResponse>.Failure("VALIDATION_ERROR", errors);
        }

        return await _authService.RefreshTokenAsync(request.Request, request.IpAddress, ct);
    }
}