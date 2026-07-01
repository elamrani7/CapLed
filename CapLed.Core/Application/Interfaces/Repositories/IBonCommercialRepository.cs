using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IBonCommandeRepository
{
    Task<BonCommande?> GetByIdAsync(int id);
    Task<BonCommande?> GetByNumeroAsync(string numeroBC);
    Task<List<BonCommande>> GetAllAsync();
    Task AddAsync(BonCommande bc);
    Task UpdateAsync(BonCommande bc);
    Task<string> GetLastNumeroAsync(string prefix);
    Task<BonCommande?> GetByLeadIdAsync(int leadId);
    Task DeleteAsync(BonCommande bc);
}

public interface IBonLivraisonRepository
{
    Task<BonLivraison?> GetByIdAsync(int id);
    Task<BonLivraison?> GetByNumeroAsync(string numeroBL);
    Task<List<BonLivraison>> GetAllAsync();
    Task AddAsync(BonLivraison bl);
    Task UpdateAsync(BonLivraison bl);
    Task<string> GetLastNumeroAsync(string prefix);
}
