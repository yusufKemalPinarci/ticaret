using System.Linq;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Users.Commands;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Features.Users.Handlers;

public class UserSessionHandlers :
    IRequestHandler<GetUserSessionsQuery, IEnumerable<SessionDto>>,
    IRequestHandler<RevokeSessionCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public UserSessionHandlers(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<SessionDto>> Handle(GetUserSessionsQuery request, CancellationToken cancellationToken)
    {
        var tokens = await _uow.RefreshTokens.GetByUserAsync(request.UserId);
        return tokens.Select(t => new SessionDto
        {
            Token = t.Token,
            ExpiresAt = t.ExpiresAt,
            RevokedAt = t.RevokedAt,
            CreatedAt = t.CreatedAt,
            CreatedByIp = t.CreatedByIp,
            RevokedByIp = t.RevokedByIp,
            ReplacedByToken = t.ReplacedByToken,
            IsActive = t.IsActive
        });
    }

    public async Task<bool> Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
    {
        var token = await _uow.RefreshTokens.GetByTokenAsync(request.Token);
        if (token == null) return false;
        if (!request.IsAdmin && token.UserId != request.UserId) return false;
        if (request.IsAdmin && token.UserId != request.UserId) return false; // Admin caller supplies target userId to avoid mistakes

        await _uow.RefreshTokens.RevokeAsync(token, request.RevokedByIp);
        await _uow.SaveChangesAsync();
        return true;
    }
}
