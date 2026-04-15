using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface ILotRepository
{
    Task<Lot?> GetByNumberAsync(int articleId, int depotId, string numeroLot);
    Task<List<Lot>> GetByArticleAsync(int articleId);
    Task AddAsync(Lot lot);
    Task UpdateAsync(Lot lot);
    Task DeleteAsync(Lot lot);
}
