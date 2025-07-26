using SpendWiseAPI.Models;
using SpendWiseAPI.Repositories.Interfaces;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace SpendWiseAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        // IMongoDatabase injected by DI
        public UserRepository(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            await _users.InsertOneAsync(user);
        }

        public async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            return await _users.Find(u => u.RefreshToken == refreshToken).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _users.ReplaceOneAsync(filter, user);
        }
    }
}
