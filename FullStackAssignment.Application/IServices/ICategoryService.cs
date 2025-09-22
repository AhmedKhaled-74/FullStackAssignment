using FullStackAssignment.Application.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IServices
{
    public interface ICategoryService
    {
        Task<CategoryResult> CreatNewCategoryAsync(CategoryRequestAdd? categoryAdd);
        Task<CategoryResult> UpdateCategoryAsync(Guid? categoryId, CategoryRequestUpdate? categoryUpdate);
        Task DeleteCategoryAsync(Guid? categoryId);
        Task<List<CategoryResult>> GetAllCategoriesAsync();
        Task<CategoryResult?> GetCategoryByIdAsync(Guid? categoryId);
    }
}
