using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface INumeroSerieRepository
{
    Task<NumeroSerie?> GetBySerialAsync(string numeroSerie);
    Task<List<NumeroSerie>> GetBySerialListAsync(List<string> serials, int depotId);
    Task AddRangeAsync(IEnumerable<NumeroSerie> series);
    Task UpdateRangeAsync(IEnumerable<NumeroSerie> series);
}
