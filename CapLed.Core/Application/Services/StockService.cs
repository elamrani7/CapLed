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
        
        // New business rule: alerts start at 5 and below
        return items.Where(e => e.Quantity <= 5);
    }

    public async Task<int> GetStockLevelAsync(int equipmentId)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);
        return equipment?.Quantity ?? 0;
    }

    public async Task<StockMovement?> GetMovementByIdAsync(int id)
    {
        return await _movementRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Updates an existing movement with a correct 4-step stock recalculation:
    /// 1. Load the OLD movement.
    /// 2. REVERT the OLD movement's impact on the OLD equipment's stock.
    /// 3. APPLY the NEW movement's impact on the (possibly different) NEW equipment's stock.
    /// 4. Validate no stock goes below zero, then save.
    /// </summary>
    public async Task UpdateMovementAsync(int id, int newEquipmentId, MovementType newType, int newQuantity, string? newRemarks)
    {
        // Step 1 — Load OLD movement + its equipment
        var oldMovement = await _movementRepository.GetByIdAsync(id);
        if (oldMovement == null) throw new Exception("Mouvement introuvable.");

        var oldEquipment = await _equipmentRepository.GetByIdAsync(oldMovement.EquipmentId);
        if (oldEquipment == null) throw new Exception("Équipement original introuvable.");

        // Step 2 — REVERT OLD movement's stock impact
        if (oldMovement.Type == MovementType.ENTRY)
            oldEquipment.Quantity -= oldMovement.Quantity;  // undo the entry
        else
            oldEquipment.Quantity += oldMovement.Quantity;  // undo the exit

        // Safety check after revert (cannot be negative — data integrity)
        if (oldEquipment.Quantity < 0)
            throw new Exception("Incohérence de stock détectée sur l'équipement original.");

        // Step 3 — APPLY NEW movement's stock impact (may be on a different equipment)
        bool equipmentChanged = newEquipmentId != oldMovement.EquipmentId;
        Equipment newEquipment;

        if (equipmentChanged)
        {
            newEquipment = await _equipmentRepository.GetByIdAsync(newEquipmentId)
                           ?? throw new Exception("Nouvel équipement introuvable.");
        }
        else
        {
            newEquipment = oldEquipment; // same object, already reverted
        }

        if (newType == MovementType.ENTRY)
            newEquipment.Quantity += newQuantity;
        else
        {
            newEquipment.Quantity -= newQuantity;
            if (newEquipment.Quantity < 0)
                throw new Exception(
                    $"Stock insuffisant sur '{newEquipment.Name}'. " +
                    $"Stock après correction : {newEquipment.Quantity + newQuantity}, " +
                    $"sortie demandée : {newQuantity}.");
        }

        // Step 4 — Persist all changes
        oldMovement.EquipmentId = newEquipmentId;
        oldMovement.Type        = newType;
        oldMovement.Quantity    = newQuantity;
        oldMovement.Remarks     = newRemarks;

        await _movementRepository.UpdateAsync(oldMovement);
        await _equipmentRepository.UpdateAsync(oldEquipment);

        if (equipmentChanged)
            await _equipmentRepository.UpdateAsync(newEquipment);
    }

    public async Task DeleteMovementAsync(int id)
    {
        var movement = await _movementRepository.GetByIdAsync(id);
        if (movement == null) throw new Exception("Mouvement introuvable.");

        var equipment = await _equipmentRepository.GetByIdAsync(movement.EquipmentId);
        if (equipment != null)
        {
            // Reverse stock change
            if (movement.Type == MovementType.ENTRY)
                equipment.Quantity -= movement.Quantity;
            else
                equipment.Quantity += movement.Quantity;

            await _equipmentRepository.UpdateAsync(equipment);
        }

        await _movementRepository.DeleteAsync(id);
    }
}

