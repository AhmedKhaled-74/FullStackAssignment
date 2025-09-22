using System.ComponentModel.DataAnnotations;

namespace FullStackAssignment.Application.DTOs.CategoryDTOs
{
    public class CategoryRequestUpdate
    {
        [Required]
        public Guid CategoryId { get; set; }

        [StringLength(100)]
        public string CategoryName { get; set; } = null!;
    }
}
