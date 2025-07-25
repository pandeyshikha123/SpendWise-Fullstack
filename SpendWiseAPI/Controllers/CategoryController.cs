using Microsoft.AspNetCore.Mvc;
using SpendWiseAPI.Services;
using SpendWiseAPI.Models;
using SpendWiseAPI.DTOs;

namespace SpendWiseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST /api/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryDto.Name))
                {
                    return BadRequest(new { message = "Category name is required" });
                }

                var trimmedName = categoryDto.Name.Trim();
                
                // Check if category already exists
                var existing = await _categoryService.GetByNameAsync(trimmedName);
                if (existing != null)
                {
                    return BadRequest(new { message = "Category already exists" });
                }

                var category = new Category { Name = trimmedName };
                var createdCategory = await _categoryService.CreateAsync(category);
                
                return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
                return StatusCode(500, new { message = "Server error" });
            }
        }

        // GET /api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return StatusCode(500, new { message = "Server error" });
            }
        }

        // GET /api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(string id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching category: {ex.Message}");
                return StatusCode(500, new { message = "Server error" });
            }
        }

        // PUT /api/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryUpdateDto categoryDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryDto.Name))
                {
                    return BadRequest(new { message = "Category name is required" });
                }

                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                var trimmedName = categoryDto.Name.Trim();
                
                // Check if new name is unique
                var existing = await _categoryService.GetByNameAsync(trimmedName);
                if (existing != null && existing.Id != id)
                {
                    return BadRequest(new { message = "Category name already exists" });
                }

                category.Name = trimmedName;
                var updatedCategory = await _categoryService.UpdateAsync(id, category);
                
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                return StatusCode(500, new { message = "Server error" });
            }
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                await _categoryService.DeleteAsync(id);
                return Ok(new { message = "Category deleted" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return StatusCode(500, new { message = "Server error" });
            }
        }
    }
}