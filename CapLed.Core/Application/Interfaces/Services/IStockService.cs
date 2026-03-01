using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IStockService
{
    Task<StockMovement> RecordEntryAsync(int equipmentId, int quantity, int userId, string? remarks = null);
    Task<StockMovement> RecordExitAsync(int equipmentId, int quantity, int userId, string? remarks = null);
    Task<IEnumerable<Equipment>> GetLowStockAlertsAsync();
    Task<int> GetStockLevelAsync(int equipmentId);
    Task<StockMovement?> GetMovementByIdAsync(int id);
    Task UpdateMovementAsync(int id, int newEquipmentId, MovementType newType, int newQuantity, string? newRemarks);
    Task DeleteMovementAsync(int id);
}

