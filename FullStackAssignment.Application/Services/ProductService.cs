using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Mappers;

namespace FullStackAssignment.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileRepo _fileRepo;

        public ProductService(IProductRepository productRepository , IFileRepo fileRepo)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _fileRepo = fileRepo;
        }
        public async Task<ProductResult> CreatNewProductAsync(ProductRequestAdd? productAdd)
        {
            if (productAdd == null)
                throw new ArgumentNullException(nameof(productAdd));

            // Save image using infrastructure service
            var imageUrl = await _fileRepo.SaveProductImageAsync(productAdd.ImageContent, productAdd.ImageExtension);

            var product = productAdd.ToProductEntity();
            product.Image = imageUrl;
            try
            {
                var addedProduct = await _productRepository.CreatNewProductAsync(product);
                return addedProduct.ToProductResultEntity();
            }
            catch (Exception ex) 
            { 
                await _fileRepo.DeleteOldImageAsync(imageUrl);
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteProductAsync(string? productCode)
        {
            if (string.IsNullOrEmpty(productCode)) 
                throw new ArgumentNullException(nameof(productCode));
            var product = await _productRepository.GetProductByCodeAsync(productCode);
            if (product == null)
                throw new ArgumentException("Invalid product code",nameof(productCode));

            await _productRepository.DeleteProductAsync(productCode);
            await _fileRepo.DeleteOldImageAsync(product.Image);
        }

        public async Task<List<ProductResult>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p=>p.ToProductResultEntity()).ToList();
        }

        public async Task<ProductResult?> GetProductByCodeAsync(string? productCode)
        {
            if (string.IsNullOrEmpty(productCode))
                throw new ArgumentNullException(nameof(productCode));

            var product = await _productRepository.GetProductByCodeAsync(productCode);
            if (product == null)
                throw new ArgumentException("Product NotFound");

            return product.ToProductResultEntity();
        }

        public async Task<ProductResult> UpdateProductAsync(string? productCode, ProductRequestUpdate? productUpdate)
        {
            if (productUpdate == null)
                throw new ArgumentNullException(nameof(productUpdate));
            if (string.IsNullOrEmpty(productCode))
                throw new ArgumentNullException(nameof(productCode));

            if (productUpdate.ImageContent != null && !string.IsNullOrEmpty(productUpdate.ImageExtension))
            {
                // Save image using infrastructure service and delete old
                var imageUrl = await _fileRepo.SaveProductImageAsync(productUpdate.ImageContent, productUpdate.ImageExtension);
                var product = productUpdate.ToProductEntityByUpdate();
                var exsistProduct = await GetProductByCodeAsync(productUpdate.ProductCode);
                if (exsistProduct == null)
                    throw new ArgumentException(nameof(exsistProduct));
                await _fileRepo.DeleteOldImageAsync(exsistProduct.Image);
                product.Image = imageUrl;
                var updatedProduct = await _productRepository.UpdateProductAsync(productCode, product);
                return updatedProduct.ToProductResultEntity();
            }

            var productSameImg = productUpdate.ToProductEntityByUpdate();
            var updatedProductSameImg = await _productRepository.UpdateProductAsync(productCode, productSameImg);

            return updatedProductSameImg.ToProductResultEntity();
        }
    }
}
