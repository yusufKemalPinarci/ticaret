using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Users.Handlers;

public class UserQueryHandlers : IRequestHandler<NotebookTherapy.Application.Features.Users.GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string AllUsersKey = "users_all";

    public UserQueryHandlers(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<UserDto>> Handle(NotebookTherapy.Application.Features.Users.GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(AllUsersKey, out IEnumerable<UserDto> cached))
            return cached;

        var users = await _uow.Users.GetAllAsync();
        var dtos = users.Select(u => _mapper.Map<UserDto>(u));
        _cache.Set(AllUsersKey, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(10) });
        return dtos;
    }
}
