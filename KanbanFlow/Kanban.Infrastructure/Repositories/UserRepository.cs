using Kanban.Application.Interfaces;
using Kanban.Core.Entities;
using Kanban.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async System.Threading.Tasks.Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async System.Threading.Tasks.Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async System.Threading.Tasks.Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}