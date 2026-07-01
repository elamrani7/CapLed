using StockManager.Core.Domain.Entities.Catalogue;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IFamilleRepository
{
    Task<IEnumerable<Famille>> GetAllAsync();
    Task<Famille?> GetByIdAsync(int id);
    Task<Famille> AddAsync(Famille famille);
    Task UpdateAsync(Famille famille);
    Task DeleteAsync(int id);
}
