using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class AlerteStockRepository : IAlerteStockRepository
{
    private readonly StockManagementDbContext _context;

    public AlerteStockRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<AlerteStock?> GetActiveAlertAsync(int articleId, int depotId)
    {
        return await _context.AlertesStock
            .FirstOrDefaultAsync(a => a.ArticleId == articleId && a.DepotId == depotId && !a.EstResolue);
    }

    public async Task AddAsync(AlerteStock alerte)
    {
        await _context.AlertesStock.AddAsync(alerte);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AlerteStock alerte)
    {
        _context.AlertesStock.Update(alerte);
        await _context.SaveChangesAsync();
    }
}
