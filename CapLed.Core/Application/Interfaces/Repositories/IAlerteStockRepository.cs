using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IAlerteStockRepository
{
    Task<AlerteStock?> GetActiveAlertAsync(int articleId, int depotId);
    Task AddAsync(AlerteStock alerte);
    Task UpdateAsync(AlerteStock alerte);
}
