using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SpendWiseAPI.Config;
using SpendWiseAPI.Models; // Ensure this namespace contains Category.cs

namespace SpendWiseAPI.Services
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // You can still use the generic method:
        public IMongoCollection<T> GetCollection<T>(string collectionName) =>
            _database.GetCollection<T>(collectionName);

        // Optional shortcut for Categories collection
        public IMongoCollection<Category> Categories =>
            _database.GetCollection<Category>("Categories");
        
        // Optional shortcut for Expenses collection
        // This assumes you have an Expense model defined in SpendWiseAPI.Models    
         public IMongoCollection<Expense> Expenses => _database.GetCollection<Expense>("Expenses");
        
        // Optional shortcut for Users collection
        public IMongoCollection<User> Users =>
            _database.GetCollection<User>("Users"); 
    }
}
