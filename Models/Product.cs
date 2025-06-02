using System.ComponentModel.DataAnnotations;

namespace ECommercePlatform.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
