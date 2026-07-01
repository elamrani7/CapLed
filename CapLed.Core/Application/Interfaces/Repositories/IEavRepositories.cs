using StockManager.Core.Domain.Entities.Catalogue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Interfaces.Repositories;

public interface IChampSpecifiqueRepository
{
    Task<List<ChampSpecifique>> GetByCategorieAsync(int categorieId);
    Task<ChampSpecifique?> GetByIdAsync(int id);
    Task AddAsync(ChampSpecifique champ);
    Task DeleteAsync(ChampSpecifique champ);
    Task SaveChangesAsync();
}

public interface IArticleChampValeurRepository
{
    Task<List<ArticleChampValeur>> GetByArticleAsync(int articleId);
    Task UpsertValuesAsync(int articleId, List<ArticleChampValeur> values);
    Task SaveChangesAsync();
}
