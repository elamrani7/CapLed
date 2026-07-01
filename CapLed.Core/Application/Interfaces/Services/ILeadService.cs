using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Core.Application.Interfaces.Services;

public interface ILeadService
{
    Task<Lead> CreateLeadAsync(CreateLeadDto dto);
    Task<Lead> UpdateStatutAsync(int leadId, UpdateLeadStatutDto dto);
    Task<Lead?> GetByIdAsync(int id);
    Task<List<Lead>> GetAllAsync();
    Task<List<Lead>> GetByStatutAsync(string statut);
}
