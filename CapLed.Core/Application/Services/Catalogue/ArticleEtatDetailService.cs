using AutoMapper;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Catalogue;
using StockManager.Core.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Services.Catalogue;

public class ArticleEtatDetailService : IArticleEtatDetailService
{
    private readonly IArticleEtatDetailRepository _repo;
    private readonly IEquipmentRepository _articleRepo;
    private readonly IMapper _mapper;

    public ArticleEtatDetailService(
        IArticleEtatDetailRepository repo, 
        IEquipmentRepository articleRepo,
        IMapper mapper)
    {
        _repo = repo;
        _articleRepo = articleRepo;
        _mapper = mapper;
    }

    public async Task<ArticleEtatDetail?> GetByArticleIdAsync(int articleId)
    {
        return await _repo.GetByArticleIdAsync(articleId);
    }

    public async Task<ArticleEtatDetail> CreateOrUpdateAsync(int articleId, CreateOrUpdateArticleEtatDetailDto dto)
    {
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null) throw new Exception("Article not found.");

        // Validation: Only OCCASION or RECONDITIONNE
        if (article.Condition != EquipmentCondition.OCCASION && article.Condition != EquipmentCondition.RECONDITIONNE)
        {
            throw new Exception("Condition details can only be added for OCCASION or RECONDITIONNE articles.");
        }

        // Validation: Grade A, B, C
        if (!string.IsNullOrEmpty(dto.GradeVisuel))
        {
            var validGrades = new[] { "A", "B", "C" };
            if (!validGrades.Contains(dto.GradeVisuel.ToUpper()))
            {
                throw new Exception("GradeVisuel must be A, B, or C.");
            }
        }

        // Validation: Warranty
        if (dto.GarantieOfferte.HasValue && dto.GarantieOfferte.Value < 0)
        {
            throw new Exception("GarantieOfferte must be >= 0.");
        }

        var existing = await _repo.GetByArticleIdAsync(articleId);
        if (existing != null)
        {
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();
            return existing;
        }
        else
        {
            var detail = _mapper.Map<ArticleEtatDetail>(dto);
            detail.ArticleId = articleId;
            await _repo.AddAsync(detail);
            await _repo.SaveChangesAsync();
            return detail;
        }
    }

    public async Task DeleteAsync(int articleId)
    {
        var detail = await _repo.GetByArticleIdAsync(articleId);
        if (detail != null)
        {
            await _repo.DeleteAsync(detail);
            await _repo.SaveChangesAsync();
        }
    }
}
