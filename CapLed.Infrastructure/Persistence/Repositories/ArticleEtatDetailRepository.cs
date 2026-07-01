using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Catalogue;
using System.Threading.Tasks;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class ArticleEtatDetailRepository : IArticleEtatDetailRepository
{
    private readonly StockManagementDbContext _context;

    public ArticleEtatDetailRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ArticleEtatDetail?> GetByArticleIdAsync(int articleId)
    {
        return await _context.ArticleEtatDetails
            .FirstOrDefaultAsync(e => e.ArticleId == articleId);
    }

    public async Task AddAsync(ArticleEtatDetail detail)
    {
        await _context.ArticleEtatDetails.AddAsync(detail);
    }

    public async Task UpdateAsync(ArticleEtatDetail detail)
    {
        _context.ArticleEtatDetails.Update(detail);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(ArticleEtatDetail detail)
    {
        _context.ArticleEtatDetails.Remove(detail);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
