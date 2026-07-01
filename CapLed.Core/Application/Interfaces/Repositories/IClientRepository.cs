using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<Client?> GetByEmailAsync(string email);
    Task<List<Client>> GetAllAsync();
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
}
