using Asp.Versioning;
using FullStackAssignment.Application.DTOs.CategoryDTOs;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Bootstrapper.Controllers;
using Microsoft.AspNetCore.Authorization;
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
    /// Handles category management operations: create, update, delete, get by ID, get all.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    public class CategoryController : CustomControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        [HttpPost("new-category")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestAdd request)
        {
            try
            {
                var result = await _categoryService.CreatNewCategoryAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Updates an existing category by ID.
        /// </summary>
        [HttpPut("update-category/{categoryId:guid}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid categoryId, [FromBody] CategoryRequestUpdate request)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(categoryId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category {categoryId}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Deletes a category by ID.
        /// </summary>
        [HttpDelete("delete-category/{categoryId:guid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid categoryId)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(categoryId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category {categoryId}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Retrieves a category by ID.
        /// </summary>
        [HttpGet("{categoryId:guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching category {categoryId}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all categories");
                return ExceptionHandel(ex);
            }
        }
    }
}
