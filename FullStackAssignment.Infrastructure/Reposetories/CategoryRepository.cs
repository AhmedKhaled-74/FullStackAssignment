using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FullStackAssignment.Infrastructure.Reposetories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Category> CreatNewCategoryAsync(Category product)
        {
            await _dbContext.Categories.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var product = await _dbContext.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id);
            if (product == null)
                throw new ArgumentException("There is No Category for this id");

            _dbContext.Categories.Remove(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _dbContext.Categories
                .FirstOrDefaultAsync(p => p.CategoryId == id);
        }

        public async Task<Category> UpdateCategoryAsync(Guid id, Category category)
        {
            if (id != category.CategoryId)
                throw new ArgumentException("There is id mismatch");

            var exsistCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(p => p.CategoryId == id);
            if (exsistCategory == null)
                throw new ArgumentException("There is No Category for this id");

            exsistCategory.CategoryName = category.CategoryName;
            _dbContext.Categories.Update(exsistCategory);
            await _dbContext.SaveChangesAsync();
            return exsistCategory;
        }
    }
}
