using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.DTOs.CategoryDTOs
{
    public class CategoryResult
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
