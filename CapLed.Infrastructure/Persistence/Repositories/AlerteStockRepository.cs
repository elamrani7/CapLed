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
        // SaveChanges is handled by UnitOfWork transaction
    }

    public async Task UpdateAsync(AlerteStock alerte)
    {
        _context.AlertesStock.Update(alerte);
        // SaveChanges is handled by UnitOfWork transaction
    }
}
