using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;
using StockManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly StockManagementDbContext _context;

    public EquipmentRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Equipment?> GetByIdAsync(int id)
    {
        return await _context.Equipments
            .Include(e => e.Category)
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(IEnumerable<Equipment> Items, int TotalCount)> GetAllAsync(
        int? categoryId = null, 
        EquipmentCondition? condition = null, 
        bool? isPublished = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.Equipments
            .Include(e => e.Category)
            .Include(e => e.Photos)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);
        
        if (condition.HasValue)
            query = query.Where(e => e.Condition == condition.Value);

        if (isPublished.HasValue)
            query = query.Where(e => e.IsPublished == isPublished.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e => e.Name.Contains(searchTerm) || 
                                    e.Reference.Contains(searchTerm) ||
                                    e.Description!.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Equipment equipment)
    {
        await _context.Equipments.AddAsync(equipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Equipment equipment)
    {
        _context.Equipments.Update(equipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var equipment = await _context.Equipments.FindAsync(id);
        if (equipment != null)
        {
            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string reference)
    {
        return await _context.Equipments.AnyAsync(e => e.Reference == reference);
    }
}

