using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(int id);
    Task<Lead?> GetByNumeroDevisAsync(string numeroDevis);
    Task<List<Lead>> GetAllAsync();
    Task<List<Lead>> GetByStatutAsync(string statut);
    Task<int> CountByYearAsync(int year);
    Task AddAsync(Lead lead);
    Task UpdateAsync(Lead lead);
}
