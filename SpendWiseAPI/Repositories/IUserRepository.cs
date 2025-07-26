using SpendWiseAPI.Models;
using System.Threading.Tasks;

namespace SpendWiseAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task AddUserAsync(User user);
        Task<User?> GetUserByRefreshToken(string refreshToken);
        Task UpdateUserAsync(User user);
    }
}
