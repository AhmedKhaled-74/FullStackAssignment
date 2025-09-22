using FullStackAssignment.Application.DTOs.CategoryDTOs;
using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResult> CreatNewCategoryAsync(CategoryRequestAdd? categoryAdd)
        {
            if (categoryAdd == null)
                throw new ArgumentNullException(nameof(categoryAdd));

            var category = categoryAdd.ToCategoryEntity();
            var addedCategory = await _categoryRepository.CreatNewCategoryAsync(category);

            return addedCategory.ToCategoryResultEntity();
        }

        public async Task DeleteCategoryAsync(Guid? categoryId)
        {
            if (!categoryId.HasValue)
                throw new ArgumentNullException(nameof(categoryId));

            await _categoryRepository.DeleteCategoryAsync(categoryId.Value);
        }

        public async Task<List<CategoryResult>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => c.ToCategoryResultEntity()).ToList();
        }

        public async Task<CategoryResult?> GetCategoryByIdAsync(Guid? categoryId)
        {
            if (!categoryId.HasValue)
                throw new ArgumentNullException(nameof(categoryId));
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId.Value);
            if (category == null)
                throw new ArgumentException("category NotFound");

            return category.ToCategoryResultEntity();
        }

        public async Task<CategoryResult> UpdateCategoryAsync(Guid? categoryId, CategoryRequestUpdate? categoryUpdate)
        {
            if (!categoryId.HasValue)
                throw new ArgumentNullException(nameof(categoryId));
            if (categoryUpdate == null)
                throw new ArgumentNullException(nameof(categoryUpdate));

            var category = categoryUpdate.ToCategoryEntityByUpdate();
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(categoryId.Value, category);

            return updatedCategory.ToCategoryResultEntity();
        }
    }
}
