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
            .Include(e => e.ChampsSpecifiques)
                .ThenInclude(v => v.ChampSpecifique)
            .Include(e => e.EtatDetail)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(IEnumerable<Equipment> Items, int TotalCount)> GetAllAsync(
        int? familleId = null,
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

        if (familleId.HasValue)
            query = query.Where(e => e.Category.FamilleId == familleId.Value);

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

    public async Task<(IEnumerable<Equipment> Items, int TotalCount)> SearchPublicAsync(StockManager.Core.Application.DTOs.Catalogue.CatalogueFilterDto filters)
    {
        var query = _context.Equipments
            .AsNoTracking()
            .Include(e => e.Category)
                .ThenInclude(c => c.Famille)
            .Include(e => e.Photos)
            .Include(e => e.ChampsSpecifiques)
                .ThenInclude(v => v.ChampSpecifique)
            .Include(e => e.EtatDetail)
            .Where(e => e.IsPublished && e.VisibleSite);

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var s = filters.Search.ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(s) || 
                                     e.Reference.ToLower().Contains(s) ||
                                     (e.Description != null && e.Description.ToLower().Contains(s)));
        }

        if (filters.FamilleId.HasValue)
            query = query.Where(e => e.Category.FamilleId == filters.FamilleId.Value);

        if (filters.CategorieId.HasValue)
            query = query.Where(e => e.CategoryId == filters.CategorieId.Value);

        if (!string.IsNullOrWhiteSpace(filters.Condition))
        {
            if (Enum.TryParse<StockManager.Core.Domain.Enums.EquipmentCondition>(filters.Condition, true, out var conditionEnum))
            {
                query = query.Where(e => e.Condition == conditionEnum);
            }
        }

        if (filters.PrixMin.HasValue)
            query = query.Where(e => e.PrixVente >= filters.PrixMin.Value);

        if (filters.PrixMax.HasValue)
            query = query.Where(e => e.PrixVente <= filters.PrixMax.Value);

        if (!string.IsNullOrWhiteSpace(filters.Disponibilite))
            query = query.Where(e => e.DisponibiliteSite == filters.Disponibilite);

        // Dynamic Specs (EAV) filtering
        if (filters.DynamicSpecs != null && filters.DynamicSpecs.Any())
        {
            foreach (var spec in filters.DynamicSpecs)
            {
                var specValue = spec.Value.ToLower();
                query = query.Where(e => e.ChampsSpecifiques.Any(v => 
                    v.ChampSpecifique.NomChamp == spec.Key && 
                    v.Valeur != null && 
                    v.Valeur.ToLower().Contains(specValue)));
            }
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

