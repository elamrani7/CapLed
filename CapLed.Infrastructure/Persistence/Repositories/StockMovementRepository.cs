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
            .Include(m => m.User)
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
            .Include(m => m.User)
            .AsQueryable();

        if (equipmentId.HasValue)
            query = query.Where(m => m.EquipmentId == equipmentId.Value);

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        if (dateFrom.HasValue)
            query = query.Where(m => m.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(m => m.CreatedAt < dateTo.Value.Date.AddDays(1));

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<StockMovement?> GetByIdAsync(int id)
    {
        return await _context.StockMovements
            .Include(m => m.Equipment)
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task UpdateAsync(StockMovement movement)
    {
        _context.StockMovements.Update(movement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var movement = await _context.StockMovements.FindAsync(id);
        if (movement != null)
        {
            _context.StockMovements.Remove(movement);
            await _context.SaveChangesAsync();
        }
    }
}

