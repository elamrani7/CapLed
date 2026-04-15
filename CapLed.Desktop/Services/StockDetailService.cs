using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

/// <summary>
/// Client API pour le détail du stock par article.
/// GET api/v2/stock/{articleId}/detail
/// </summary>
public class StockDetailService : ApiClientBase
{
    public StockDetailService(HttpClient httpClient) : base(httpClient) { }

    public async Task<ArticleStockDetailModel?> GetDetailAsync(int articleId)
    {
        var result = await GetAsync<ArticleStockDetailModel>($"api/v2/stock/{articleId}/detail");
        return result;
    }
}
