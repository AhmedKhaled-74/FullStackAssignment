using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.DTOs.ProductDTOs
{
    public class ProductResult
    {
        public string ProductCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Image { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal DiscountRate { get; set; }
        public int MinimumQuantity { get; set; }

    }
}
