using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IReposetories
{
    public interface IProductRepository
    {
        Task<Product> CreatNewProductAsync(Product product);
        Task<Product> UpdateProductAsync(string productCode, Product product);
        Task DeleteProductAsync(string productCode);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByCodeAsync(string productCode);
    }
}
