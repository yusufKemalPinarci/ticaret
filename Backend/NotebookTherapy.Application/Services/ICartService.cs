using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int? userId, string? sessionId);
    Task<CartDto> AddItemToCartAsync(int? userId, string? sessionId, int productId, int? productVariantId, int quantity);
    Task<CartDto> UpdateCartItemAsync(int cartItemId, int quantity);
    Task<bool> RemoveCartItemAsync(int cartItemId);
    Task<bool> ClearCartAsync(int? userId, string? sessionId);
    Task<CartDto> MergeCartAsync(int userId, string sessionId);
}
