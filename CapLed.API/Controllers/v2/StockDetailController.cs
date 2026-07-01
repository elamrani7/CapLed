using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.Interfaces.Repositories;

namespace StockManager.API.Controllers.v2;

/// <summary>
/// Expose le détail du stock par article : lots ou numéros de série.
/// </summary>
[ApiController]
[Route("api/v2/stock")]
[Authorize]
public class StockDetailController : ControllerBase
{
    private readonly ILotRepository          _lotRepo;
    private readonly INumeroSerieRepository  _serieRepo;
    private readonly IEquipmentRepository    _equipmentRepo;
    private readonly ICategoryRepository     _categoryRepo;

    public StockDetailController(
        ILotRepository         lotRepo,
        INumeroSerieRepository serieRepo,
        IEquipmentRepository   equipmentRepo,
        ICategoryRepository    categoryRepo)
    {
        _lotRepo       = lotRepo;
        _serieRepo     = serieRepo;
        _equipmentRepo = equipmentRepo;
        _categoryRepo  = categoryRepo;
    }

    /// <summary>
    /// GET api/v2/stock/{articleId}/detail
    /// Retourne le détail du stock pour un article (lots ou séries selon TypeGestionStock).
    /// </summary>
    [HttpGet("{articleId:int}/detail")]
    public async Task<ActionResult<ArticleStockDetailDto>> GetDetail(int articleId)
    {
        var article = await _equipmentRepo.GetByIdAsync(articleId);
        if (article == null) return NotFound($"Article {articleId} introuvable.");

        var category = await _categoryRepo.GetByIdAsync(article.CategoryId);
        var typeGestion = category?.TypeGestionStock ?? "QUANTITE";

        var result = new ArticleStockDetailDto
        {
            ArticleId    = articleId,
            ArticleName  = article.Name,
            TypeGestion  = typeGestion,
            TotalQuantite = article.Quantity
        };

        switch (typeGestion)
        {
            case "LOT":
            {
                var lots = await _lotRepo.GetByArticleAsync(articleId);
                result.Lots = lots.Select(l => new LotDetailDto
                {
                    Id          = l.Id,
                    NumeroLot   = l.NumeroLot,
                    Quantite    = l.Quantite,
                    DateEntree  = l.DateEntree,
                    Fournisseur = l.Fournisseur,
                    Garantie    = l.Garantie,
                    Certificat  = l.Certificat,
                    DepotNom    = l.Depot?.Nom ?? $"Dépôt {l.DepotId}"
                }).ToList();
                break;
            }
            case "SERIALISE":
            {
                var series = await _serieRepo.GetByArticleAsync(articleId);
                result.Series = series.Select(s => new SerieDetailDto
                {
                    Id               = s.Id,
                    NumeroSerie      = s.NumeroSerieLabel,
                    Statut           = s.Statut.ToString(),
                    DateEntree       = s.DateEntree,
                    DepotNom         = s.Depot?.Nom ?? $"Dépôt {s.DepotId}"
                }).ToList();
                break;
            }
        }

        return Ok(result);
    }
}

// ── DTOs ─────────────────────────────────────────────────────────────────
public class ArticleStockDetailDto
{
    public int    ArticleId     { get; set; }
    public string ArticleName   { get; set; } = string.Empty;
    public string TypeGestion   { get; set; } = "QUANTITE";
    public int    TotalQuantite { get; set; }
    public List<LotDetailDto>?  Lots   { get; set; }
    public List<SerieDetailDto>? Series { get; set; }
}

public class LotDetailDto
{
    public int      Id          { get; set; }
    public string   NumeroLot   { get; set; } = string.Empty;
    public int      Quantite    { get; set; }
    public DateTime DateEntree  { get; set; }
    public string?  Fournisseur { get; set; }
    public string?  Garantie    { get; set; }
    public string?  Certificat  { get; set; }
    public string   DepotNom    { get; set; } = string.Empty;
}

public class SerieDetailDto
{
    public int      Id          { get; set; }
    public string   NumeroSerie { get; set; } = string.Empty;
    public string   Statut      { get; set; } = "DISPONIBLE";
    public DateTime DateEntree  { get; set; }
    public string   DepotNom    { get; set; } = string.Empty;
}
