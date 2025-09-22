using System.ComponentModel.DataAnnotations;

namespace FullStackAssignment.Application.DTOs.CategoryDTOs
{
    public class CategoryRequestAdd
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;
    }
}
