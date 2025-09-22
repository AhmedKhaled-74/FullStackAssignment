using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Services;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Application.Mappers;
using Xunit;

namespace FullStackAssignment.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IFileRepo> _fileRepoMock;
        private readonly ProductService _productService;
        private readonly IFixture _fixture;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _fileRepoMock = new Mock<IFileRepo>();
            _productService = new ProductService(_productRepoMock.Object, _fileRepoMock.Object);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        #region CreatNewProductAsync
        [Fact]
        public async Task CreatNewProductAsync_NullProductAdd_ShouldThrowArgumentNullException()
        {
            ProductRequestAdd? dto = null;

            Func<Task> act = async () => await _productService.CreatNewProductAsync(dto);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreatNewProductAsync_ValidProduct_ShouldReturnProductResult()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                                   .With(c => c.CategoryId, Guid.NewGuid())
                                   .With(c => c.CategoryName, "mockcat")
                                   .Create();

            var productAdd = _fixture.Build<ProductRequestAdd>()
                                     .With(p => p.ImageContent, new byte[] { 1, 2, 3 })
                                     .With(p => p.ImageExtension, ".png")
                                     .With(p => p.Name, "Test Product")
                                     .With(p => p.Price, 100m)
                                     .With(p => p.CategoryId, category.CategoryId)
                                     .With(p => p.DiscountRate, 0.05m)
                                     .Create();

            _fileRepoMock.Setup(f => f.SaveProductImageAsync(productAdd.ImageContent, productAdd.ImageExtension))
                         .ReturnsAsync("image-url");

            _productRepoMock.Setup(r => r.CreatNewProductAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => {
                    p.Category = category; // assign the category here
                    return p;
                }); // <- return the same product passed in

            // Act
            var result = await _productService.CreatNewProductAsync(productAdd);

            // Assert
            result.Should().NotBeNull();
            result.Image.Should().Be("image-url");
            result.Name.Should().Be(productAdd.Name);
            _productRepoMock.Verify(r => r.CreatNewProductAsync(It.IsAny<Product>()), Times.Once);
            _fileRepoMock.Verify(f => f.SaveProductImageAsync(productAdd.ImageContent, productAdd.ImageExtension), Times.Once);
        }
        #endregion

        #region DeleteProductAsync
        [Fact]
        public async Task DeleteProductAsync_NullOrEmptyCode_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _productService.DeleteProductAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region GetAllProductsAsync
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnListOfProductResults()
        {
            var products = _fixture.Build<Product>().CreateMany(3).ToList();
            _productRepoMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(products);

            var result = await _productService.GetAllProductsAsync();

            result.Should().HaveCount(3);
            result.First().Should().BeOfType<ProductResult>();
        }
        #endregion

        #region GetProductByCodeAsync
        [Fact]
        public async Task GetProductByCodeAsync_NullOrEmptyCode_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _productService.GetProductByCodeAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetProductByCodeAsync_NotFound_ShouldThrowArgumentException()
        {
            var code = "P123";
            _productRepoMock.Setup(r => r.GetProductByCodeAsync(code))
                            .ReturnsAsync((Product?)null);

            Func<Task> act = async () => await _productService.GetProductByCodeAsync(code);

            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage("*NotFound*");
        }

        [Fact]
        public async Task GetProductByCodeAsync_Found_ShouldReturnProductResult()
        {
            var product = _fixture.Build<Product>().Create();
            _productRepoMock.Setup(r => r.GetProductByCodeAsync(product.ProductCode))
                            .ReturnsAsync(product);

            var result = await _productService.GetProductByCodeAsync(product.ProductCode);

            result.Should().NotBeNull();
            result.ProductCode.Should().Be(product.ProductCode);
        }
        #endregion

        #region UpdateProductAsync
        [Fact]
        public async Task UpdateProductAsync_NullProductUpdate_ShouldThrowArgumentNullException()
        {
            Func<Task> act = async () => await _productService.UpdateProductAsync("P123", null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateProductAsync_NullOrEmptyCode_ShouldThrowArgumentNullException()
        {
            var dto = _fixture.Build<ProductRequestUpdate>().Create();

            Func<Task> act = async () => await _productService.UpdateProductAsync(null, dto);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateProductAsync_WithNewImage_ShouldUpdateProduct()
        {
            // Arrange
            var category = _fixture.Build<Category>()
                                  .With(c => c.CategoryId, Guid.NewGuid())
                                  .With(c => c.CategoryName, "mockcat")
                                  .Create();

            var dto = _fixture.Build<ProductRequestUpdate>()
                                     .With(p => p.ImageContent, new byte[] { 1, 2, 3 })
                                     .With(p => p.ImageExtension, ".png")
                                     .With(p => p.Name, "Test Product")
                                     .With(p => p.Price, 100m)
                                     .With(p => p.CategoryId, category.CategoryId)
                                     .With(p => p.DiscountRate, 0.05m)
                                     .Create();


            _fileRepoMock.Setup(f => f.SaveProductImageAsync(dto.ImageContent, dto.ImageExtension))
                         .ReturnsAsync("new-image");

            _fileRepoMock.Setup(f => f.DeleteOldImageAsync(It.IsAny<string>()))
                         .Returns(Task.CompletedTask);

            _productRepoMock.Setup(r => r.UpdateProductAsync(It.IsAny<string>(), It.IsAny<Product>()))
                  .ReturnsAsync((string code, Product p) => {
                      p.Category = category; // <- assign Category to avoid NRE
                      return p;
                  });
            // Act
            var result = await _productService.UpdateProductAsync("P123", dto);

            // Assert
            result.Should().NotBeNull();
            result.Image.Should().Be("new-image");
            _fileRepoMock.Verify(f => f.SaveProductImageAsync(dto.ImageContent, dto.ImageExtension), Times.Once);
            _fileRepoMock.Verify(f => f.DeleteOldImageAsync(It.IsAny<string>()), Times.Once);
            _productRepoMock.Verify(r => r.UpdateProductAsync("P123", It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_WithoutNewImage_ShouldUpdateProduct()
        {
            var category = _fixture.Build<Category>()
                         .With(c => c.CategoryId, Guid.NewGuid())
                         .With(c => c.CategoryName, "mockcat")
                         .Create();

            var dto = _fixture.Build<ProductRequestUpdate>()
                     .With(p => p.ImageContent, (byte[]?)null)
                     .Create();

            var updatedProduct = dto.ToProductEntityByUpdate();
            _productRepoMock.Setup(r => r.UpdateProductAsync(It.IsAny<string>(), It.IsAny<Product>()))
                       .ReturnsAsync((string code, Product p) => {
                           p.Category = category; // <- assign Category to avoid NRE
                           return p;
                       });

            var result = await _productService.UpdateProductAsync("P123", dto);

            result.Should().NotBeNull();
            _productRepoMock.Verify(r => r.UpdateProductAsync("P123", It.IsAny<Product>()), Times.Once);
        }
        #endregion
    }
}
