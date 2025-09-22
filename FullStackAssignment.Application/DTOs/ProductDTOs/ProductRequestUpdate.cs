using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FullStackAssignment.Application.DTOs.ProductDTOs
{
    public class ProductRequestUpdate
    {
        public string ProductCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }

        // Just the image bytes and extension, no ASP.NET types here
        public byte[]? ImageContent { get; set; }
        public string? ImageExtension { get; set; }

        public decimal Price { get; set; }
        public decimal DiscountRate { get; set; } = 0m;
        public int MinimumQuantity { get; set; } = 1;
    }
}
