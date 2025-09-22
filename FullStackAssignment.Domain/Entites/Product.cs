using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Domain.Entites
{
    public class Product
    {
        [Key]
        public string ProductCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }
        [Url]
        public string Image {  get; set; } = null!;
        public decimal Price { get; set; }
        public decimal DiscountRate { get; set; }
        public int MinimumQuantity { get; set; }

        //navigation prop
        public virtual Category Category { get; set; } = null!;
    }
}
