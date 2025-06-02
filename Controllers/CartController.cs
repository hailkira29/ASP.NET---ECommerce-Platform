using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommercePlatform.Services;
using ECommercePlatform.Models;
using System.Text.Json;

namespace ECommercePlatform.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }        private string GetSessionId()
        {
            var sessionId = HttpContext.Session.GetString("SessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("SessionId", sessionId);
            }
            return sessionId;
        }

        private string? GetUserId()
        {
            return User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        }        // GET: Cart
        public async Task<IActionResult> Index()
        {
            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                var cart = await _cartService.GetCartAsync(sessionId, userId);
                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                TempData["ErrorMessage"] = "Unable to load cart. Please try again.";
                return RedirectToAction("Index", "Product");
            }
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                await _cartService.AddToCartAsync(sessionId, productId, quantity, userId);
                
                TempData["SuccessMessage"] = "Product added to cart successfully!";
                
                // Return JSON for AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var cartCount = await _cartService.GetCartItemCountAsync(sessionId, userId);
                    return Json(new { success = true, cartCount, message = "Product added to cart!" });
                }
                
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for AddToCart");
                TempData["ErrorMessage"] = ex.Message;
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }
                
                return RedirectToAction("Details", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                TempData["ErrorMessage"] = "Unable to add product to cart. Please try again.";
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Unable to add product to cart." });
                }
                
                return RedirectToAction("Details", "Product", new { id = productId });
            }
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                await _cartService.UpdateCartItemAsync(sessionId, productId, quantity, userId);
                
                TempData["SuccessMessage"] = "Cart updated successfully!";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for UpdateQuantity");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                TempData["ErrorMessage"] = "Unable to update cart. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int productId)
        {            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                await _cartService.RemoveFromCartAsync(sessionId, productId, userId);
                
                TempData["SuccessMessage"] = "Item removed from cart!";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for RemoveItem");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                TempData["ErrorMessage"] = "Unable to remove item. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                await _cartService.ClearCartAsync(sessionId, userId);
                
                TempData["SuccessMessage"] = "Cart cleared successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                TempData["ErrorMessage"] = "Unable to clear cart. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // GET: Cart/GetCartCount (for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                var count = await _cartService.GetCartItemCountAsync(sessionId, userId);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }

        // POST: Cart/MergeCarts (called after login)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MergeCarts()
        {
            try
            {
                var sessionId = GetSessionId();
                var userId = GetUserId();
                
                if (!string.IsNullOrEmpty(userId))
                {
                    await _cartService.MergeCartsAsync(sessionId, userId);
                }
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging carts");
                return Json(new { success = false, message = "Error merging carts" });
            }
        }
    }
}
