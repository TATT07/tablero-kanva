using Kanban.Application.Interfaces;
using Kanban.Core.Entities;
using Kanban.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async System.Threading.Tasks.Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public async System.Threading.Tasks.Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}