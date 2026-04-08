using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface ILotRepository
{
    /// <summary>Finds an existing lot by its number within an article+depot combination.</summary>
    Task<Lot?> GetByNumberAsync(int articleId, int depotId, string numeroLot);

    Task AddAsync(Lot lot);
    Task UpdateAsync(Lot lot);
    Task DeleteAsync(Lot lot);
}
