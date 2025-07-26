using SpendWiseAPI.Models;

namespace SpendWiseAPI.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(string id);
        Task<Category?> GetByNameAsync(string name);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(string id, Category category);
        Task DeleteAsync(string id);
    }
}