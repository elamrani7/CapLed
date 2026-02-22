using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IContactRequestRepository
{
    Task<ContactRequest?> GetByIdAsync(int id);
    Task<IEnumerable<ContactRequest>> GetAllAsync();
    Task AddAsync(ContactRequest request);
    Task UpdateAsync(ContactRequest request);
}
