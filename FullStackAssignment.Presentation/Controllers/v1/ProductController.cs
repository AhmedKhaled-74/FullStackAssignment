using Asp.Versioning;
using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Bootstrapper.Controllers;
using FullStackAssignment.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Presentation.Controllers.v1
{
    /// <summary>
    ///  product endpoints controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    public class ProductController : CustomControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new product with optional image upload.
        /// </summary>
        [HttpPost("new-product")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductRequestAddApi request)
        {
            try
            {
                var imageBytes = await GetFileBytesAsync(request.ImageFile);
                var appDto = new ProductRequestAdd
                {
                    Name = request.Name,
                    CategoryId = request.CategoryId,
                    ImageContent = imageBytes,
                    ImageExtension = Path.GetExtension(request.ImageFile.FileName),
                    Price = request.Price,
                    DiscountRate = request.DiscountRate,
                    MinimumQuantity = request.MinimumQuantity
                };

                var result = await _productService.CreatNewProductAsync(appDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in creation");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Updates an existing product, optionally updating the image.
        /// </summary>
        [HttpPut("update-product/{productCode}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] string productCode, [FromForm] ProductRequestUpdateApi request)
        {
            try
            {
                byte[]? imageBytes = null;
                string? imageExt = null;

                if (request.ImageFile != null)
                {
                    imageBytes = await GetFileBytesAsync(request.ImageFile);
                    imageExt = Path.GetExtension(request.ImageFile.FileName);
                }

                var updateDto = new ProductRequestUpdate
                {
                    ProductCode = productCode,
                    Name = request.Name,
                    CategoryId = request.CategoryId,
                    ImageContent = imageBytes,
                    ImageExtension = imageExt,
                    Price = request.Price,
                    DiscountRate = request.DiscountRate,
                    MinimumQuantity = request.MinimumQuantity
                };

                var result = await _productService.UpdateProductAsync(productCode, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {productCode}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Deletes a product by its product code.
        /// </summary>
        [HttpDelete("delete-product/{productCode}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] string productCode)
        {
            try
            {
                await _productService.DeleteProductAsync(productCode);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {productCode}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Retrieves a single product by its product code.
        /// </summary>
        [HttpGet("{productCode}")]
        public async Task<IActionResult> GetProductByCode([FromRoute] string productCode)
        {
            try
            {
                var product = await _productService.GetProductByCodeAsync(productCode);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product {productCode}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Helper: Reads an IFormFile into a byte array.
        /// </summary>
        private static async Task<byte[]> GetFileBytesAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
