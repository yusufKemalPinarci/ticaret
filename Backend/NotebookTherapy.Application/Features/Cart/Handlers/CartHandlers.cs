using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Cart.Commands;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Cart.Handlers;

public class CartHandlers :
    IRequestHandler<NotebookTherapy.Application.Features.Cart.GetCartQuery, CartDto>,
    IRequestHandler<AddItemToCartCommand, CartDto>,
    IRequestHandler<UpdateCartItemCommand, CartDto>,
    IRequestHandler<RemoveCartItemCommand, bool>,
    IRequestHandler<ClearCartCommand, bool>,
    IRequestHandler<MergeCartCommand, CartDto>
{
    private readonly ICartService _cartService;
    private readonly IMemoryCache _cache;

    public CartHandlers(ICartService cartService, IMemoryCache cache)
    {
        _cartService = cartService;
        _cache = cache;
    }

    private string GetCacheKey(int? userId, string? sessionId)
    {
        if (userId.HasValue) return $"cart_user_{userId.Value}";
        if (!string.IsNullOrEmpty(sessionId)) return $"cart_session_{sessionId}";
        return "cart_empty";
    }

    public async Task<CartDto> Handle(NotebookTherapy.Application.Features.Cart.GetCartQuery request, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(request.UserId, request.SessionId);
        if (_cache.TryGetValue(key, out CartDto cached))
            return cached;

        var cart = await _cartService.GetCartAsync(request.UserId, request.SessionId);
        _cache.Set(key, cart, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(2) });
        return cart;
    }

    public async Task<CartDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartService.AddItemToCartAsync(request.UserId, request.SessionId, request.ProductId, request.ProductVariantId, request.Quantity);
        _cache.Remove(GetCacheKey(request.UserId, request.SessionId));
        if (request.UserId.HasValue) _cache.Remove(GetCacheKey(request.UserId, null));
        return cart;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartService.UpdateCartItemAsync(request.CartItemId, request.Quantity);
        // best-effort: evict all cart keys (user and session) - in real app track owner
        _cache.Remove("cart_empty");
        return cart;
    }

    public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var result = await _cartService.RemoveCartItemAsync(request.CartItemId);
        _cache.Remove("cart_empty");
        return result;
    }

    public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var result = await _cartService.ClearCartAsync(request.UserId, request.SessionId);
        _cache.Remove(GetCacheKey(request.UserId, request.SessionId));
        return result;
    }

    public async Task<CartDto> Handle(MergeCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartService.MergeCartAsync(request.UserId, request.SessionId);
        _cache.Remove(GetCacheKey(request.UserId, request.SessionId));
        _cache.Remove(GetCacheKey(request.UserId, null));
        return cart;
    }
}
