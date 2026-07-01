using AutoMapper;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Services.Catalogue;

public class CataloguePublicService : ICataloguePublicService
{
    private readonly IEquipmentRepository _equipmentRepo;
    private readonly IMapper _mapper;

    public CataloguePublicService(IEquipmentRepository equipmentRepo, IMapper mapper)
    {
        _equipmentRepo = equipmentRepo;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<PublicArticleListItemDto>> SearchAsync(CatalogueFilterDto filters)
    {
        var (entities, totalCount) = await _equipmentRepo.SearchPublicAsync(filters);

        var dtos = _mapper.Map<IEnumerable<PublicArticleListItemDto>>(entities);
        
        // Post-processing for badges is handled within the mapping profile or down below
        // Since the prompt asks to compute badges, we will ensure it's in the mapping profile
        // but can do final touch up here if needed.

        return new PagedResultDto<PublicArticleListItemDto>(dtos, totalCount, filters.Page, filters.PageSize);
    }

    public async Task<PublicArticleDetailDto?> GetDetailsAsync(int id)
    {
        var entity = await _equipmentRepo.GetByIdAsync(id);
        if (entity == null || !entity.IsPublished || !entity.VisibleSite) return null;

        var dto = _mapper.Map<PublicArticleDetailDto>(entity);

        // Fetch suggestions (top 4 same category, excluding self)
        var filter = new CatalogueFilterDto
        {
            CategorieId = entity.CategoryId,
            Page = 1,
            PageSize = 5 // Fetch 5 to ensure we get 4 after excluding self
        };

        var (similarEntities, _) = await _equipmentRepo.SearchPublicAsync(filter);
        var filteredSimilar = similarEntities.Where(e => e.Id != id).Take(4);

        dto.ArticlesSimilaires = _mapper.Map<List<PublicArticleListItemDto>>(filteredSimilar);

        return dto;
    }
}
