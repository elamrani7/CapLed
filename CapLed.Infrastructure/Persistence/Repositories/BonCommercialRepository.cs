using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Infrastructure.Persistence;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class BonCommandeRepository : IBonCommandeRepository
{
    private readonly StockManagementDbContext _ctx;
    public BonCommandeRepository(StockManagementDbContext ctx) => _ctx = ctx;

    public async Task<BonCommande?> GetByIdAsync(int id)
    {
        return await _ctx.BonsCommande
            .Include(bc => bc.Client)
            .Include(bc => bc.Lignes)
                .ThenInclude(l => l.Article)
            .FirstOrDefaultAsync(bc => bc.Id == id);
    }

    public async Task<BonCommande?> GetByNumeroAsync(string numeroBC)
    {
        return await _ctx.BonsCommande
            .Include(bc => bc.Client)
            .Include(bc => bc.Lignes)
                .ThenInclude(l => l.Article)
            .FirstOrDefaultAsync(bc => bc.NumeroBC == numeroBC);
    }

    public Task<List<BonCommande>> GetAllAsync() => _ctx.BonsCommande.ToListAsync();

    public async Task AddAsync(BonCommande bc) => await _ctx.BonsCommande.AddAsync(bc);
    
    public Task UpdateAsync(BonCommande bc) { _ctx.BonsCommande.Update(bc); return Task.CompletedTask; }

    public async Task<string> GetLastNumeroAsync(string prefix)
    {
        return await _ctx.BonsCommande
            .Where(bc => bc.NumeroBC.StartsWith(prefix))
            .OrderByDescending(bc => bc.NumeroBC)
            .Select(bc => bc.NumeroBC)
            .FirstOrDefaultAsync() ?? string.Empty;
    }
}

public class BonLivraisonRepository : IBonLivraisonRepository
{
    private readonly StockManagementDbContext _ctx;
    public BonLivraisonRepository(StockManagementDbContext ctx) => _ctx = ctx;

    public async Task<BonLivraison?> GetByIdAsync(int id)
    {
        return await _ctx.BonsLivraison
            .Include(bl => bl.Client)
            .Include(bl => bl.BonCommande)
            .Include(bl => bl.Lignes)
                .ThenInclude(l => l.Article)
            .FirstOrDefaultAsync(bl => bl.Id == id);
    }

    public async Task<BonLivraison?> GetByNumeroAsync(string numeroBL)
    {
        return await _ctx.BonsLivraison
            .Include(bl => bl.Client)
            .Include(bl => bl.BonCommande)
            .Include(bl => bl.Lignes)
                .ThenInclude(l => l.Article)
            .FirstOrDefaultAsync(bl => bl.NumeroBL == numeroBL);
    }

    public Task<List<BonLivraison>> GetAllAsync() => _ctx.BonsLivraison.ToListAsync();

    public async Task AddAsync(BonLivraison bl) => await _ctx.BonsLivraison.AddAsync(bl);

    public Task UpdateAsync(BonLivraison bl) { _ctx.BonsLivraison.Update(bl); return Task.CompletedTask; }

    public async Task<string> GetLastNumeroAsync(string prefix)
    {
        return await _ctx.BonsLivraison
            .Where(bl => bl.NumeroBL.StartsWith(prefix))
            .OrderByDescending(bl => bl.NumeroBL)
            .Select(bl => bl.NumeroBL)
            .FirstOrDefaultAsync() ?? string.Empty;
    }
}
