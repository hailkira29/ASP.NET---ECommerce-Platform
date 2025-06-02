using ECommercePlatform.Models;

namespace ECommercePlatform.Services
{    public interface ICartService
    {
        Task<Cart> GetCartAsync(string sessionId, string? userId = null);
        Task<Cart> AddToCartAsync(string sessionId, int productId, int quantity = 1, string? userId = null);
        Task<Cart> UpdateCartItemAsync(string sessionId, int productId, int quantity, string? userId = null);
        Task<Cart> RemoveFromCartAsync(string sessionId, int productId, string? userId = null);
        Task ClearCartAsync(string sessionId, string? userId = null);
        Task<int> GetCartItemCountAsync(string sessionId, string? userId = null);
        Task<decimal> GetCartTotalAsync(string sessionId, string? userId = null);
        Task MergeCartsAsync(string sessionId, string userId);
    }
}
