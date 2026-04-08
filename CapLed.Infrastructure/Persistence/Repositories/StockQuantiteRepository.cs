using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class StockQuantiteRepository : IStockQuantiteRepository
{
    private readonly StockManagementDbContext _context;

    public StockQuantiteRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<StockQuantite?> GetByArticleAndDepotAsync(int articleId, int depotId)
    {
        return await _context.StockQuantites
            .FirstOrDefaultAsync(sq => sq.ArticleId == articleId && sq.DepotId == depotId);
    }

    public async Task<int> GetTotalStockAsync(int articleId)
    {
        return await _context.StockQuantites
            .Where(sq => sq.ArticleId == articleId)
            .SumAsync(sq => sq.Quantite);
    }

    public async Task AddAsync(StockQuantite stockQuantite)
    {
        await _context.StockQuantites.AddAsync(stockQuantite);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StockQuantite stockQuantite)
    {
        _context.StockQuantites.Update(stockQuantite);
        await _context.SaveChangesAsync();
    }
}
