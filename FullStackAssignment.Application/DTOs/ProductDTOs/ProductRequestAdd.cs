using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FullStackAssignment.Application.DTOs.ProductDTOs
{
    public class ProductRequestAdd
    {
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }

        // Just the image bytes and extension
        public byte[] ImageContent { get; set; } = null!;
        public string ImageExtension { get; set; } = null!;

        public decimal Price { get; set; }
        public decimal DiscountRate { get; set; } = 0m;
        public int MinimumQuantity { get; set; } = 1;
    }
}
