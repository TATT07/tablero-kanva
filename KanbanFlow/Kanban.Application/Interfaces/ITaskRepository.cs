using Kanban.Core.Entities;

namespace Kanban.Application.Interfaces
{
    public interface ITaskRepository
    {
        System.Threading.Tasks.Task<IEnumerable<Kanban.Core.Entities.Task>> GetAllAsync();
        System.Threading.Tasks.Task<Kanban.Core.Entities.Task?> GetByIdAsync(int id);
        System.Threading.Tasks.Task AddAsync(Kanban.Core.Entities.Task task);
        System.Threading.Tasks.Task UpdateAsync(Kanban.Core.Entities.Task task);
        System.Threading.Tasks.Task DeleteAsync(Kanban.Core.Entities.Task task);
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}