using MongoDB.Driver;
using SpendWiseAPI.Models;

namespace SpendWiseAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(MongoDbContext context)
        {
            _categories = context.Categories;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _categories.Find(_ => true).SortBy(c => c.Name).ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(string id)
        {
            return await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _categories.Find(c => c.Name == name).FirstOrDefaultAsync();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _categories.InsertOneAsync(category);
            return category;
        }

        public async Task<Category> UpdateAsync(string id, Category category)
        {
            await _categories.ReplaceOneAsync(c => c.Id == id, category);
            return category;
        }

        public async Task DeleteAsync(string id)
        {
            await _categories.DeleteOneAsync(c => c.Id == id);
        }
    }
}