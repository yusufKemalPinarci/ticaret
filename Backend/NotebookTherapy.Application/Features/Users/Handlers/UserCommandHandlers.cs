using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Application.Features.Users.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Users.Handlers;

public class UserCommandHandlers :
    IRequestHandler<UpdateUserRoleCommand, bool>,
    IRequestHandler<UpdateUserAdminCommand, bool>,
    IRequestHandler<UpdateProfileCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMemoryCache _cache;
    private const string AllUsersKey = "users_all";

    public UserCommandHandlers(IUnitOfWork uow, IMemoryCache cache)
    {
        _uow = uow;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByIdAsync(request.UserId);
        if (user == null) return false;
        user.Role = request.Role;
        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllUsersKey);
        _cache.Remove($"user_{request.UserId}");
        return true;
    }

    public async Task<bool> Handle(UpdateUserAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByIdAsync(request.UserId);
        if (user == null) return false;

        if (request.UpdateDto.Role != null)
            user.Role = request.UpdateDto.Role;
        if (request.UpdateDto.IsLocked.HasValue)
            user.IsLocked = request.UpdateDto.IsLocked.Value;
        if (request.UpdateDto.IsActive.HasValue)
            user.IsActive = request.UpdateDto.IsActive.Value;
        if (request.UpdateDto.VerifyEmail == true)
            user.IsEmailVerified = true;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllUsersKey);
        _cache.Remove($"user_{request.UserId}");
        return true;
    }

    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByIdAsync(request.UserId);
        if (user == null) return false;

        user.FirstName = request.Dto.FirstName;
        user.LastName = request.Dto.LastName;
        user.PhoneNumber = request.Dto.PhoneNumber;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        _cache.Remove($"user_{request.UserId}");
        _cache.Remove(AllUsersKey);

        return true;
    }
}
