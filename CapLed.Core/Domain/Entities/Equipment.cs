using StockManager.Core.Domain.Entities.Stock;
using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EquipmentCondition Condition { get; set; }
    public int Quantity { get; set; }
    public int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }

    // ── Step 1A: ERP / Site Vitrine columns ──────────────────────────────────
    /// <summary>URL vers la fiche technique (PDF, datasheet). MLD: ARTICLE.datasheet_url</summary>
    public string? DatasheetUrl { get; set; }

    /// <summary>Disponibilité affichée sur le site public. MLD: ARTICLE.disponibilite_site
    /// Valeurs possibles : EN_STOCK | STOCK_LIMITE | SUR_DEMANDE | INDISPONIBLE</summary>
    public string DisponibiliteSite { get; set; } = "EN_STOCK";

    /// <summary>L'article est-il visible sur le catalogue public ? MLD: ARTICLE.visible_site</summary>
    public bool VisibleSite { get; set; } = false;

    /// <summary>IDs des articles similaires (JSON array, ex: "[3,7,12]"). MLD: ARTICLE.article_similaire_ids</summary>
    public string? ArticleSimilaireIds { get; set; }

    /// <summary>Prix de vente HT. MLD: ARTICLE.prix_vente</summary>
    public decimal? PrixVente { get; set; }

    /// <summary>Prix d'achat HT. MLD: ARTICLE.prix_achat</summary>
    public decimal? PrixAchat { get; set; }
    // ─────────────────────────────────────────────────────────────────────────

    // Navigation Properties (legacy)
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public virtual ICollection<StockManager.Core.Domain.Entities.Catalogue.ArticleChampValeur> ChampsSpecifiques { get; set; } = new List<StockManager.Core.Domain.Entities.Catalogue.ArticleChampValeur>();
    public virtual StockManager.Core.Domain.Entities.Catalogue.ArticleEtatDetail? EtatDetail { get; set; }
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public virtual ICollection<ContactRequest> ContactRequests { get; set; } = new List<ContactRequest>();

    // ── Step 1B: Multi-depot inventory ───────────────────────────────────────
    public virtual ICollection<StockQuantite> StockQuantites { get; set; } = new List<StockQuantite>();
    public virtual ICollection<AlerteStock>   AlertesStock   { get; set; } = new List<AlerteStock>();
    
    public virtual ICollection<Lot>           Lots           { get; set; } = new List<Lot>();
    public virtual ICollection<NumeroSerie>   NumerosSerie   { get; set; } = new List<NumeroSerie>();
    // ─────────────────────────────────────────────────────────────────────────
}
