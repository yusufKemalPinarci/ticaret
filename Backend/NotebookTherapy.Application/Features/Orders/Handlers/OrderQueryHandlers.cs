using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Orders.Handlers;

public class OrderQueryHandlers :
    IRequestHandler<NotebookTherapy.Application.Features.Orders.GetAllOrdersQuery, IEnumerable<OrderDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Orders.GetOrderByIdQuery, OrderDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string AllOrdersKey = "orders_all";

    public OrderQueryHandlers(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<OrderDto>> Handle(NotebookTherapy.Application.Features.Orders.GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(AllOrdersKey, out IEnumerable<OrderDto> cached))
            return cached;

        var orders = await _uow.Orders.GetAllAsync();
        var dtos = orders.Select(o => _mapper.Map<OrderDto>(o));
        _cache.Set(AllOrdersKey, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<OrderDto?> Handle(NotebookTherapy.Application.Features.Orders.GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var key = $"order_{request.Id}";
        if (_cache.TryGetValue(key, out OrderDto cached))
            return cached;

        var order = await _uow.Orders.GetOrderWithItemsAsync(request.Id);
        if (order == null) return null;
        var dto = _mapper.Map<OrderDto>(order);
        _cache.Set(key, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(10) });
        return dto;
    }
}
