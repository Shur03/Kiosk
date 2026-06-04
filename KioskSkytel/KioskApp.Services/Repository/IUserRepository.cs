using KioskApp.Models;

namespace KioskApp.Services.Database.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByRegisterNumberAsync(string registerNumber);
        Task<int> InsertAsync(User user);
        Task<int> UpdateAsync(User user);
        Task<int> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}