using System.ComponentModel.DataAnnotations;

namespace ECommercePlatform.Models
{    public class Cart
    {
        public int Id { get; set; }
        
        [Required]
        public string SessionId { get; set; } = string.Empty;
        
        public string? UserId { get; set; } // For authenticated users
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice);
        
        public int TotalItems => CartItems.Sum(item => item.Quantity);
    }
}
