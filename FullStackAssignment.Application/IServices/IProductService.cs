using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IServices
{
    public interface IProductService
    {
        Task<ProductResult> CreatNewProductAsync(ProductRequestAdd? productAdd);
        Task<ProductResult> UpdateProductAsync(string? productCode, ProductRequestUpdate? productUpdate);
        Task DeleteProductAsync(string? productCode);
        Task<List<ProductResult>> GetAllProductsAsync();
        Task<ProductResult?> GetProductByCodeAsync(string? productCode);
    }
}
