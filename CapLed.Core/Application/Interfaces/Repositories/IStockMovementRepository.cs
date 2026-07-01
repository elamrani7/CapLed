using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IStockMovementRepository
{
    Task<StockMovement> AddAsync(StockMovement movement);
    Task<IEnumerable<StockMovement>> GetByEquipmentIdAsync(int equipmentId);
    Task<(IEnumerable<StockMovement> Items, int TotalCount)> GetAllAsync(
        int? equipmentId = null,
        StockManager.Core.Domain.Enums.MovementType? type = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int page = 1,
        int pageSize = 10);
    Task<StockMovement?> GetByIdAsync(int id);
    Task UpdateAsync(StockMovement movement);
    Task DeleteAsync(int id);
}

