using Kanban.Core.Entities;

namespace Kanban.Application.Interfaces
{
    public interface IUserRepository
    {
        System.Threading.Tasks.Task<User?> GetByEmailAsync(string email);
        System.Threading.Tasks.Task<User?> GetByIdAsync(int id);
        System.Threading.Tasks.Task AddAsync(User user);
        System.Threading.Tasks.Task UpdateAsync(User user);
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}