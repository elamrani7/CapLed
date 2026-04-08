using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Catalogue;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class ChampSpecifiqueRepository : IChampSpecifiqueRepository
{
    private readonly StockManagementDbContext _context;

    public ChampSpecifiqueRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChampSpecifique>> GetByCategorieAsync(int categorieId)
    {
        return await _context.ChampsSpecifiques
            .Where(c => c.CategorieId == categorieId)
            .OrderBy(c => c.Ordre)
            .ToListAsync();
    }

    public async Task<ChampSpecifique?> GetByIdAsync(int id)
    {
        return await _context.ChampsSpecifiques.FindAsync(id);
    }

    public async Task AddAsync(ChampSpecifique champ)
    {
        await _context.ChampsSpecifiques.AddAsync(champ);
    }

    public async Task DeleteAsync(ChampSpecifique champ)
    {
        _context.ChampsSpecifiques.Remove(champ);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public class ArticleChampValeurRepository : IArticleChampValeurRepository
{
    private readonly StockManagementDbContext _context;

    public ArticleChampValeurRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<ArticleChampValeur>> GetByArticleAsync(int articleId)
    {
        return await _context.ArticleChampValeurs
            .Include(v => v.ChampSpecifique)
            .Where(v => v.ArticleId == articleId)
            .OrderBy(v => v.ChampSpecifique.Ordre)
            .ToListAsync();
    }

    public async Task UpsertValuesAsync(int articleId, List<ArticleChampValeur> values)
    {
        var existing = await _context.ArticleChampValeurs
            .Where(v => v.ArticleId == articleId)
            .ToListAsync();

        // Delete missing
        var incomingIds = values.Select(v => v.ChampSpecifiqueId).ToList();
        var toDelete = existing.Where(e => !incomingIds.Contains(e.ChampSpecifiqueId)).ToList();
        _context.ArticleChampValeurs.RemoveRange(toDelete);

        // Update or Insert
        foreach (var val in values)
        {
            var exists = existing.FirstOrDefault(e => e.ChampSpecifiqueId == val.ChampSpecifiqueId);
            if (exists != null)
            {
                exists.Valeur = val.Valeur;
            }
            else
            {
                val.ArticleId = articleId;
                await _context.ArticleChampValeurs.AddAsync(val);
            }
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
