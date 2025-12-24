using MediatR;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Auth.Commands;

/// <summary>
/// Command para logout de usuário.
/// </summary>
public sealed record LogoutCommand(string UserId, LogoutRequest Request)
    : IRequest<Result<bool>>;

/// <summary>
/// Handler do LogoutCommand.
/// </summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken ct)
    {
        return await _authService.LogoutAsync(request.UserId, request.Request, ct);
    }
}