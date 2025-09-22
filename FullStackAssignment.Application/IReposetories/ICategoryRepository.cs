using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IReposetories
{
    public interface ICategoryRepository
    {
        Task<Category> CreatNewCategoryAsync(Category categoryId);
        Task<Category> UpdateCategoryAsync(Guid categoryId, Category category);
        Task DeleteCategoryAsync(Guid categoryId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    }
}
