using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FullStackAssignment.Presentation.Models
{
    public class ProductRequestUpdateApi
    {
        [Required]
        public string ProductCode { get; set; } = null!;
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; } 

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 1)]
        public decimal DiscountRate { get; set; } = 0m;

        [Range(1, int.MaxValue)]
        public int MinimumQuantity { get; set; } = 1;
    }
}
