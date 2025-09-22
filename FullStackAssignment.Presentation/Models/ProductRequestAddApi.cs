using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Presentation.Models
{
    public class ProductRequestAddApi
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; } = null!; // Frontend-friendly

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 1)]
        public decimal DiscountRate { get; set; } = 0m;

        [Range(1, int.MaxValue)]
        public int MinimumQuantity { get; set; } = 1;
    }
}
