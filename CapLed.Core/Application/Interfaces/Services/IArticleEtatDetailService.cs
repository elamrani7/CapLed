using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Domain.Entities.Catalogue;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IArticleEtatDetailService
{
    Task<ArticleEtatDetail?> GetByArticleIdAsync(int articleId);
    Task<ArticleEtatDetail> CreateOrUpdateAsync(int articleId, CreateOrUpdateArticleEtatDetailDto dto);
    Task DeleteAsync(int articleId);
}
