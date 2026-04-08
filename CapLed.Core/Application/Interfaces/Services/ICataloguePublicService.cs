using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.DTOs.Catalogue;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Interfaces.Services;

public interface ICataloguePublicService
{
    Task<PagedResultDto<PublicArticleListItemDto>> SearchAsync(CatalogueFilterDto filters);
    Task<PublicArticleDetailDto?> GetDetailsAsync(int id);
}
