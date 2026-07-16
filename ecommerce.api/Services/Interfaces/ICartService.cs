using System;

namespace ecommerce.api;

public interface ICartService
{
    Task<CartDto>GetCartAsync(int userId);
    Task<CartDto>AddToCartAsync(int userId, AddToCartDto addToCartDto);
    Task<CartDto?> UpdateCartItemAsync (int userId, int productId, UpdateCartItemDto updateCartItemDto);
    Task<bool> RemoveFromCartAsync (int userId, int productId);

}
