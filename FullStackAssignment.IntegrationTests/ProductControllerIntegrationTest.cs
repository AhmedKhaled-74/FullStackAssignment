using FluentAssertions;
using FullStackAssignment.Application.DTOs.ProductDTOs;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Infrastructure.DbContexts;
using FullStackAssignment.IntegrationTests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Xunit;


namespace FullStackAssignment.Tests.Integration
{
    public class ProductControllerIntegrationTest : IClassFixture<WebAppFactory>
    {
        private readonly HttpClient _client;
        private readonly WebAppFactory _factory; // store the factory
        private const string BaseUrl = "/api/v1/product";

        public ProductControllerIntegrationTest(WebAppFactory factory)
        {
            _factory = factory; // assign to field
            _client = factory.CreateClient();

            // add test authentication header
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
        }


        [Fact]
        public async Task CreateProduct_ShouldReturnOk_WhenValidProduct()
        {
            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("dummy image content"));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Test Category"
            };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Categories.Add(category);
            context.SaveChanges();
            var content = new MultipartFormDataContent
            {
                { new StringContent("Test Product"), "Name" },
                { new StringContent(category.CategoryId.ToString()), "CategoryId" },
                { new StringContent("100"), "Price" },
                { new StringContent("0.05"), "DiscountRate" },
                { new StringContent("1"), "MinimumQuantity" },
                { fileContent, "ImageFile", "test.png" }
            };

            var response = await _client.PostAsync($"{BaseUrl}/new-product", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var product = await response.Content.ReadFromJsonAsync<ProductResult>();
            product.Should().NotBeNull();
            product.Name.Should().Be("Test Product");
            product.Image.Should().NotBeNullOrEmpty();
            product.ProductCode.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnOk_WhenValidUpdate()
        {
            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("dummy image content"));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Test Category"
            };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Categories.Add(category);
            context.SaveChanges();
            var content = new MultipartFormDataContent
            {
                { new StringContent("Test Product"), "Name" },
                { new StringContent(category.CategoryId.ToString()), "CategoryId" },
                { new StringContent("100"), "Price" },
                { new StringContent("0.05"), "DiscountRate" },
                { new StringContent("1"), "MinimumQuantity" },
                { fileContent, "ImageFile", "test.png" }
            };

            var response = await _client.PostAsync($"{BaseUrl}/new-product", content);
            var createdProduct = await response.Content.ReadFromJsonAsync<ProductResult>();
            createdProduct.Should().NotBeNull();

            // Update product
            var newFileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("updated image"));
            newFileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            var updateContent = new MultipartFormDataContent
            {
                { new StringContent(createdProduct.ProductCode), "ProductCode" },
                { new StringContent("Updated Product"), "Name" },
                { new StringContent(category.CategoryId.ToString()), "CategoryId" },
                { new StringContent("120"), "Price" },
                { new StringContent("0.1"), "DiscountRate" },
                { new StringContent("2"), "MinimumQuantity" },
                { newFileContent, "ImageFile", "updated.png" }
            };

            var updateResponse = await _client.PutAsync($"{BaseUrl}/update-product/{createdProduct.ProductCode}", updateContent);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductResult>();
            updatedProduct.Name.Should().Be("Updated Product");
            updatedProduct.Image.Should().NotBeNullOrEmpty();
        }
    }
}