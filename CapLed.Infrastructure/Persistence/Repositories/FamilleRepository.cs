using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Catalogue;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class FamilleRepository : IFamilleRepository
{
    private readonly StockManagementDbContext _context;

    public FamilleRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Famille>> GetAllAsync()
    {
        return await _context.Familles.ToListAsync();
    }

    public async Task<Famille?> GetByIdAsync(int id)
    {
        return await _context.Familles.FindAsync(id);
    }

    public async Task<Famille> AddAsync(Famille famille)
    {
        famille.CreatedAt = DateTime.UtcNow;
        _context.Familles.Add(famille);
        await _context.SaveChangesAsync();
        return famille;
    }

    public async Task UpdateAsync(Famille famille)
    {
        _context.Entry(famille).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var famille = await _context.Familles.FindAsync(id);
        if (famille != null)
        {
            _context.Familles.Remove(famille);
            await _context.SaveChangesAsync();
        }
    }
}
