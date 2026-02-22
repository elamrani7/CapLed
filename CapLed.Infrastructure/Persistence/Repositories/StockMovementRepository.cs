using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using StockManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly StockManagementDbContext _context;

    public StockMovementRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovement> AddAsync(StockMovement movement)
    {
        await _context.StockMovements.AddAsync(movement);
        await _context.SaveChangesAsync();
        return movement;
    }

    public async Task<IEnumerable<StockMovement>> GetByEquipmentIdAsync(int equipmentId)
    {
        return await _context.StockMovements
            .Include(m => m.Equipment)
            .Where(m => m.EquipmentId == equipmentId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<StockMovement> Items, int TotalCount)> GetAllAsync(
        int? equipmentId = null,
        StockManager.Core.Domain.Enums.MovementType? type = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.StockMovements
            .Include(m => m.Equipment)
            .AsQueryable();

        if (equipmentId.HasValue)
            query = query.Where(m => m.EquipmentId == equipmentId.Value);

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        if (dateFrom.HasValue)
            query = query.Where(m => m.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(m => m.CreatedAt <= dateTo.Value);

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

