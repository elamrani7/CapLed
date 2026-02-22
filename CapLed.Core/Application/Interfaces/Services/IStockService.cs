using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IStockService
{
    Task<StockMovement> RecordEntryAsync(int equipmentId, int quantity, int userId, string? remarks = null);
    Task<StockMovement> RecordExitAsync(int equipmentId, int quantity, int userId, string? remarks = null);
    Task<IEnumerable<Equipment>> GetLowStockAlertsAsync();
    Task<int> GetStockLevelAsync(int equipmentId);
}

