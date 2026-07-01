using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Domain.Entities.Catalogue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IChampSpecifiqueService
{
    Task<List<ChampSpecifique>> GetByCategorieAsync(int categorieId);
    Task<ChampSpecifique> CreateAsync(CreateChampSpecifiqueDto dto);
    Task DeleteAsync(int id);
}

public interface IArticleDynamicFieldService
{
    Task<List<ArticleChampValeur>> GetValuesAsync(int articleId);
    Task UpdateValuesAsync(int articleId, List<ArticleChampValeurDto> values);
}
