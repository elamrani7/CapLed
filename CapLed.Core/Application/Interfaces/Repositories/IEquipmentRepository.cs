using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(int id);
    Task<(IEnumerable<Equipment> Items, int TotalCount)> GetAllAsync(
        int? categoryId = null, 
        EquipmentCondition? condition = null, 
        bool? isPublished = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10);
    Task AddAsync(Equipment equipment);
    Task UpdateAsync(Equipment equipment);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string reference);
}

