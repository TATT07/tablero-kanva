using Kanban.Application.Interfaces;
using Kanban.Core.Entities;
using Kanban.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<Kanban.Core.Entities.Task>> GetAllAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async System.Threading.Tasks.Task<Kanban.Core.Entities.Task?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async System.Threading.Tasks.Task AddAsync(Kanban.Core.Entities.Task task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async System.Threading.Tasks.Task UpdateAsync(Kanban.Core.Entities.Task task)
    {
        _context.Tasks.Update(task);
    }

    public async System.Threading.Tasks.Task DeleteAsync(Kanban.Core.Entities.Task task)
    {
        _context.Tasks.Remove(task);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}