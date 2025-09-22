using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Infrastructure.Reposetories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Product> CreatNewProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.ProductCode))
            {
                var nextVal = await _dbContext.GetNextProductCodeAsync();
                product.ProductCode = $"P{nextVal:D5}";
            }
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            // Load the category explicitly if you want it returned
            await _dbContext.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();
            return product;
        }


        public async Task DeleteProductAsync(string code)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.ProductCode.ToLower() == code.ToLower());
            if (product == null) 
                throw new ArgumentException("There is No Product for this code");

             _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Products.Include(p=>p.Category).ToListAsync();
        }

        public async Task<Product?> GetProductByCodeAsync(string code)
        {
            return await _dbContext.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductCode.ToLower() == code.ToLower());
        }

        public async Task<Product> UpdateProductAsync(string code, Product product)
        {
            if (code != product.ProductCode)
                throw new ArgumentException("There is code mismatch");

            var exsistProduct = await _dbContext.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p=>p.ProductCode.ToLower()==code.ToLower());
            if (exsistProduct == null)
                throw new ArgumentException("There is No Product for this code");

            exsistProduct.Name = product.Name;
            exsistProduct.Price = product.Price;
            exsistProduct.MinimumQuantity = product.MinimumQuantity;
            exsistProduct.Image = product.Image ?? exsistProduct.Image;
            exsistProduct.DiscountRate = product.DiscountRate;
            exsistProduct.CategoryId = product.CategoryId;

            _dbContext.Products.Update(exsistProduct);
            await _dbContext.SaveChangesAsync();
            // Load the category explicitly if you want it returned
            await _dbContext.Entry(exsistProduct)
                .Reference(p => p.Category)
                .LoadAsync();
            return exsistProduct;
        }
    }
}
