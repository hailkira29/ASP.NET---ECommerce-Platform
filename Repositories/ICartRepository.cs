using ECommercePlatform.Models;

namespace ECommercePlatform.Repositories
{    public interface ICartRepository
    {
        Task<Cart?> GetCartBySessionIdAsync(string sessionId);
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart> CreateCartAsync(string sessionId, string? userId = null);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
        Task<Cart> SaveCartAsync(Cart cart);
        Task DeleteCartAsync(int cartId);
    }
}
