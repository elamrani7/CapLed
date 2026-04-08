using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IStockQuantiteRepository
{
    Task<StockQuantite?> GetByArticleAndDepotAsync(int articleId, int depotId);
    Task<int> GetTotalStockAsync(int articleId);
    Task AddAsync(StockQuantite stockQuantite);
    Task UpdateAsync(StockQuantite stockQuantite);
}
