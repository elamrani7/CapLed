using StockManager.Core.Domain.Entities.Catalogue;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IArticleEtatDetailRepository
{
    Task<ArticleEtatDetail?> GetByArticleIdAsync(int articleId);
    Task AddAsync(ArticleEtatDetail detail);
    Task UpdateAsync(ArticleEtatDetail detail);
    Task DeleteAsync(ArticleEtatDetail detail);
    Task SaveChangesAsync();
}
