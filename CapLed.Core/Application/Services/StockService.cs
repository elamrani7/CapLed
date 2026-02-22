using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Application.Services;

public class StockService : IStockService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IStockMovementRepository _movementRepository;

    public StockService(IEquipmentRepository equipmentRepository, IStockMovementRepository movementRepository)
    {
        _equipmentRepository = equipmentRepository;
        _movementRepository = movementRepository;
    }

    public async Task<StockMovement> RecordEntryAsync(int equipmentId, int quantity, int userId, string? remarks = null)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);
        if (equipment == null) throw new Exception("Equipment not found");

        // 1. Create the movement record
        var movement = new StockMovement
        {
            EquipmentId = equipmentId,
            UserId = userId,
            Type = MovementType.ENTRY,
            Quantity = quantity,
            Remarks = remarks,
            CreatedAt = DateTime.UtcNow
        };

        // 2. Update equipment quantity
        equipment.Quantity += quantity;

        // 3. Save changes (Repositories handle SaveChanges internaly in this simple design)
        await _movementRepository.AddAsync(movement);
        await _equipmentRepository.UpdateAsync(equipment);

        return movement;
    }

    public async Task<StockMovement> RecordExitAsync(int equipmentId, int quantity, int userId, string? remarks = null)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);
        if (equipment == null) throw new Exception("Equipment not found");

        if (equipment.Quantity < quantity)
            throw new Exception("Insufficient stock available for this exit.");

        // 1. Create the movement record
        var movement = new StockMovement
        {
            EquipmentId = equipmentId,
            UserId = userId,
            Type = MovementType.EXIT,
            Quantity = quantity,
            Remarks = remarks,
            CreatedAt = DateTime.UtcNow
        };

        // 2. Update equipment quantity
        equipment.Quantity -= quantity;

        // 3. Save changes
        await _movementRepository.AddAsync(movement);
        await _equipmentRepository.UpdateAsync(equipment);

        return movement;
    }

    public async Task<IEnumerable<Equipment>> GetLowStockAlertsAsync()
    {
        // For alerts, we might want all of them, so we set a large page size
        var (items, _) = await _equipmentRepository.GetAllAsync(pageSize: 1000);
        return items.Where(e => e.Quantity <= e.MinThreshold);
    }

    public async Task<int> GetStockLevelAsync(int equipmentId)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);
        return equipment?.Quantity ?? 0;
    }
}

