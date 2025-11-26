using Kanban.Core.Entities;

namespace Kanban.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        System.Threading.Tasks.Task<RefreshToken?> GetByTokenAsync(string token);
        System.Threading.Tasks.Task AddAsync(RefreshToken token);
        System.Threading.Tasks.Task UpdateAsync(RefreshToken token);
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}