using ECommercePlatform.Models;
using ECommercePlatform.Repositories;

namespace ECommercePlatform.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }        public async Task<Cart> GetCartAsync(string sessionId, string? userId = null)
        {
            Cart? cart = null;
            
            // For authenticated users, prioritize user cart
            if (!string.IsNullOrEmpty(userId))
            {
                cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = await _cartRepository.CreateCartAsync(sessionId, userId);
                }
            }
            else
            {
                cart = await _cartRepository.GetCartBySessionIdAsync(sessionId);
                if (cart == null)
                {
                    cart = await _cartRepository.CreateCartAsync(sessionId);
                }
            }
            
            return cart;
        }

        public async Task<Cart> AddToCartAsync(string sessionId, int productId, int quantity = 1, string? userId = null)
        {
            // Input sanitization
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
            
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));            // Verify product exists
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException("Product not found", nameof(productId));

            var cart = await GetCartAsync(sessionId, userId);
            var existingCartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                await _cartRepository.UpdateCartItemAsync(existingCartItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            return await _cartRepository.SaveCartAsync(cart);
        }        public async Task<Cart> UpdateCartItemAsync(string sessionId, int productId, int quantity, string? userId = null)
        {
            // Input sanitization
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
            
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));
            
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

            var cart = await GetCartAsync(sessionId, userId);
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);

            if (cartItem != null)
            {
                if (quantity == 0)
                {
                    await _cartRepository.DeleteCartItemAsync(cartItem.Id);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    await _cartRepository.UpdateCartItemAsync(cartItem);
                }
            }

            return await _cartRepository.SaveCartAsync(cart);
        }

        public async Task<Cart> RemoveFromCartAsync(string sessionId, int productId, string? userId = null)
        {
            // Input sanitization
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
              if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));

            var cart = await GetCartAsync(sessionId, userId);
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);

            if (cartItem != null)
            {
                await _cartRepository.DeleteCartItemAsync(cartItem.Id);
            }

            return await _cartRepository.SaveCartAsync(cart);
        }

        public async Task ClearCartAsync(string sessionId, string? userId = null)
        {
            // Input sanitization            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));

            var cart = await GetCartAsync(sessionId, userId);
            await _cartRepository.ClearCartAsync(cart.Id);
        }

        public async Task<int> GetCartItemCountAsync(string sessionId, string? userId = null)
        {            if (string.IsNullOrWhiteSpace(sessionId))
                return 0;

            var cart = !string.IsNullOrEmpty(userId) 
                ? await _cartRepository.GetCartByUserIdAsync(userId)
                : await _cartRepository.GetCartBySessionIdAsync(sessionId);
            return cart?.TotalItems ?? 0;
        }

        public async Task<decimal> GetCartTotalAsync(string sessionId, string? userId = null)
        {            if (string.IsNullOrWhiteSpace(sessionId))
                return 0;

            var cart = !string.IsNullOrEmpty(userId) 
                ? await _cartRepository.GetCartByUserIdAsync(userId)
                : await _cartRepository.GetCartBySessionIdAsync(sessionId);
            return cart?.TotalAmount ?? 0;
        }

        public async Task MergeCartsAsync(string sessionId, string userId)
        {
            var sessionCart = await _cartRepository.GetCartBySessionIdAsync(sessionId);
            var userCart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (sessionCart == null || !sessionCart.CartItems.Any())
                return;

            if (userCart == null)
            {
                // Create a new user cart and transfer session cart items
                userCart = await _cartRepository.CreateCartAsync(sessionId, userId);
                foreach (var item in sessionCart.CartItems)
                {
                    item.CartId = userCart.Id;
                    await _cartRepository.UpdateCartItemAsync(item);
                }
            }
            else
            {
                // Merge session cart items into existing user cart
                foreach (var sessionItem in sessionCart.CartItems)
                {
                    var existingUserItem = await _cartRepository.GetCartItemAsync(userCart.Id, sessionItem.ProductId);
                    if (existingUserItem != null)
                    {
                        existingUserItem.Quantity += sessionItem.Quantity;
                        await _cartRepository.UpdateCartItemAsync(existingUserItem);
                    }
                    else
                    {
                        sessionItem.CartId = userCart.Id;
                        await _cartRepository.UpdateCartItemAsync(sessionItem);
                    }
                }
            }

            // Delete the session cart
            await _cartRepository.DeleteCartAsync(sessionCart.Id);
        }
    }
}
