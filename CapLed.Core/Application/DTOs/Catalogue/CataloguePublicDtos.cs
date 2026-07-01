using System.Collections.Generic;

namespace StockManager.Core.Application.DTOs.Catalogue;

public class CatalogueFilterDto
{
    public string? Search { get; set; }
    public int? FamilleId { get; set; }
    public int? CategorieId { get; set; }
    public string? Condition { get; set; }
    public decimal? PrixMin { get; set; }
    public decimal? PrixMax { get; set; }
    public string? Disponibilite { get; set; }
    
    // Dynamic Specs: Dictionary containing spec_* filters
    public Dictionary<string, string>? DynamicSpecs { get; set; }
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class PublicArticleListItemDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal? PrixVente { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string DisponibiliteSite { get; set; } = string.Empty;
    public string BadgeDisponibilite { get; set; } = string.Empty;
    public string BadgeCondition { get; set; } = string.Empty;
    public string? UrlImagePrincipale { get; set; }
    public string FamilleNom { get; set; } = string.Empty;
    public string CategorieNom { get; set; } = string.Empty;
}

public class PublicArticleDetailDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? PrixVente { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string DisponibiliteSite { get; set; } = string.Empty;
    public string DisponibiliteBadge { get; set; } = string.Empty;
    public string ConditionBadge { get; set; } = string.Empty;
    
    public ArticleEtatDetailDto? EtatDetail { get; set; }
    public List<PublicSpecDto> CaracteristiquesPrincipales { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<PublicArticleListItemDto> ArticlesSimilaires { get; set; } = new();
}

public class PublicSpecDto
{
    public string Nom { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Valeur { get; set; }
}
