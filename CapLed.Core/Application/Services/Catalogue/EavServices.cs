using AutoMapper;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Catalogue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace StockManager.Core.Application.Services.Catalogue;

public class ChampSpecifiqueService : IChampSpecifiqueService
{
    private readonly IChampSpecifiqueRepository _repo;
    private readonly IMapper _mapper;

    public ChampSpecifiqueService(IChampSpecifiqueRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<ChampSpecifique>> GetByCategorieAsync(int categorieId)
    {
        return await _repo.GetByCategorieAsync(categorieId);
    }

    public async Task<ChampSpecifique> CreateAsync(CreateChampSpecifiqueDto dto)
    {
        var champ = _mapper.Map<ChampSpecifique>(dto);
        await _repo.AddAsync(champ);
        await _repo.SaveChangesAsync();
        return champ;
    }

    public async Task DeleteAsync(int id)
    {
        var champ = await _repo.GetByIdAsync(id);
        if (champ != null)
        {
            await _repo.DeleteAsync(champ);
            await _repo.SaveChangesAsync();
        }
    }
}

public class ArticleDynamicFieldService : IArticleDynamicFieldService
{
    private readonly IArticleChampValeurRepository _valueRepo;
    private readonly IChampSpecifiqueRepository _fieldRepo;
    private readonly IEquipmentRepository _articleRepo;

    public ArticleDynamicFieldService(
        IArticleChampValeurRepository valueRepo, 
        IChampSpecifiqueRepository fieldRepo,
        IEquipmentRepository articleRepo)
    {
        _valueRepo = valueRepo;
        _fieldRepo = fieldRepo;
        _articleRepo = articleRepo;
    }

    public async Task<List<ArticleChampValeur>> GetValuesAsync(int articleId)
    {
        return await _valueRepo.GetByArticleAsync(articleId);
    }

    public async Task UpdateValuesAsync(int articleId, List<ArticleChampValeurDto> values)
    {
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null) throw new Exception("Article not found.");

        // Get all possible fields for this article's category
        var allowedFields = await _fieldRepo.GetByCategorieAsync(article.CategoryId);
        var allowedIds = allowedFields.Select(f => f.Id).ToList();

        var toUpsert = new List<ArticleChampValeur>();

        foreach (var valDto in values)
        {
            var field = allowedFields.FirstOrDefault(f => f.Id == valDto.ChampSpecifiqueId);
            if (field == null) continue; // Skip fields not belonging to this category

            // Validate Type
            if (!string.IsNullOrEmpty(valDto.Valeur))
            {
                ValidateValue(valDto.Valeur, field.TypeDonnee);
            }
            else if (field.Obligatoire)
            {
                throw new Exception($"Field '{field.NomChamp}' is mandatory.");
            }

            toUpsert.Add(new ArticleChampValeur
            {
                ChampSpecifiqueId = valDto.ChampSpecifiqueId,
                Valeur = valDto.Valeur
            });
        }

        await _valueRepo.UpsertValuesAsync(articleId, toUpsert);
        await _valueRepo.SaveChangesAsync();
    }

    private void ValidateValue(string value, string type)
    {
        switch (type.ToUpper())
        {
            case "NOMBRE":
                if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    throw new Exception($"Invalid value '{value}' for type NOMBRE.");
                break;
            case "DATE":
                if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    throw new Exception($"Invalid value '{value}' for type DATE.");
                break;
            case "BOOLEEN":
                var lower = value.ToLower();
                if (lower != "true" && lower != "false" && lower != "1" && lower != "0")
                    throw new Exception($"Invalid value '{value}' for type BOOLEEN.");
                break;
            // TEXTE is always valid if not null
        }
    }
}
