using System.ComponentModel.DataAnnotations;

namespace ECommercePlatform.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;
        
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        public decimal TotalPrice => Quantity * UnitPrice;
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
