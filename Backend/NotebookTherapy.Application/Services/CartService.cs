using AutoMapper;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartAsync(int? userId, string? sessionId)
    {
        Cart? cart = null;

        if (userId.HasValue)
        {
            cart = await _unitOfWork.Carts.GetByUserIdAsync(userId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _unitOfWork.Carts.GetBySessionIdAsync(sessionId);
        }

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                SessionId = sessionId
            };
            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            cart = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
        }

        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> AddItemToCartAsync(int? userId, string? sessionId, int productId, int? productVariantId, int quantity)
    {
        var cart = await GetCartAsync(userId, sessionId);
        var cartEntity = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
        
        if (cartEntity == null)
        {
            cartEntity = new Cart
            {
                UserId = userId,
                SessionId = sessionId
            };
            await _unitOfWork.Carts.AddAsync(cartEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        var existingItem = cartEntity.Items.FirstOrDefault(i => i.ProductId == productId && i.ProductVariantId == productVariantId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            await _unitOfWork.Carts.UpdateAsync(cartEntity);
        }
        else
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId) ?? throw new Exception("Product not found");

            ProductVariant? variant = null;
            if (productVariantId.HasValue)
            {
                variant = await _unitOfWork.ProductVariants.GetByIdAsync(productVariantId.Value);
                if (variant == null || variant.ProductId != productId)
                    throw new Exception("Variant not found for product");
                if (!variant.IsActive)
                    throw new Exception("Variant is inactive");
            }

            var price = variant?.Price ?? product.DiscountPrice ?? product.Price;

            var cartItem = new CartItem
            {
                CartId = cartEntity.Id,
                ProductId = productId,
                ProductVariantId = productVariantId,
                Quantity = quantity,
                UnitPrice = price
            };
            
            cartEntity.Items.Add(cartItem);
            await _unitOfWork.Carts.UpdateAsync(cartEntity);
        }

        await _unitOfWork.SaveChangesAsync();
        cartEntity = await _unitOfWork.Carts.GetCartWithItemsAsync(cartEntity.Id);
        return _mapper.Map<CartDto>(cartEntity);
    }

    public async Task<CartDto> UpdateCartItemAsync(int cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            await RemoveCartItemAsync(cartItemId);
            // try to return the cart that contained the item
            var cartAfterRemove = await _unitOfWork.Carts.GetCartByCartItemIdAsync(cartItemId);
            if (cartAfterRemove != null)
                return _mapper.Map<CartDto>(cartAfterRemove);
            // if cart not found (item removed), return empty cart dto
            return new CartDto();
        }

        var cart = await _unitOfWork.Carts.GetCartByCartItemIdAsync(cartItemId);
        if (cart == null)
            throw new Exception("Cart item not found");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            throw new Exception("Cart item not found");

        item.Quantity = quantity;
        await _unitOfWork.Carts.UpdateAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        var updatedCart = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
        return _mapper.Map<CartDto>(updatedCart);
    }

    public async Task<bool> RemoveCartItemAsync(int cartItemId)
    {
        var cart = await _unitOfWork.Carts.GetCartByCartItemIdAsync(cartItemId);
        if (cart == null)
            return false;

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return false;

        cart.Items.Remove(item);
        await _unitOfWork.Carts.UpdateAsync(cart);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int? userId, string? sessionId)
    {
        var cart = await GetCartAsync(userId, sessionId);
        var cartEntity = await _unitOfWork.Carts.GetByIdAsync(cart.Id);
        
        if (cartEntity != null)
        {
            cartEntity.Items.Clear();
            await _unitOfWork.Carts.UpdateAsync(cartEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        
        return false;
    }

    public async Task<CartDto> MergeCartAsync(int userId, string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new ArgumentException("sessionId is required", nameof(sessionId));

        var sessionCart = await _unitOfWork.Carts.GetBySessionIdAsync(sessionId);
        if (sessionCart == null)
            return await GetCartAsync(userId, null);

        var userCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        if (userCart == null)
        {
            // attach user id to session cart
            sessionCart.UserId = userId;
            sessionCart.SessionId = null;
            await _unitOfWork.Carts.UpdateAsync(sessionCart);
            await _unitOfWork.SaveChangesAsync();
            var updated = await _unitOfWork.Carts.GetCartWithItemsAsync(sessionCart.Id);
            return _mapper.Map<CartDto>(updated);
        }

        // merge items
        foreach (var item in sessionCart.Items)
        {
            var existing = userCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                item.CartId = userCart.Id;
                userCart.Items.Add(item);
            }
        }

        // remove sessionCart
        await _unitOfWork.Carts.UpdateAsync(userCart);
        await _unitOfWork.Carts.DeleteAsync(sessionCart);
        await _unitOfWork.SaveChangesAsync();

        var merged = await _unitOfWork.Carts.GetCartWithItemsAsync(userCart.Id);
        return _mapper.Map<CartDto>(merged);
    }
}
