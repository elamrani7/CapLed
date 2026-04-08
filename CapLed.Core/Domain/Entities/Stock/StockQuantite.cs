namespace StockManager.Core.Domain.Entities.Stock;

/// <summary>
/// Quantité d'un article dans un dépôt donné. MLD: STOCK_QUANTITE
/// Une ligne par couple (ArticleId × DepotId).
/// Business rule RG-01: Quantite must never go negative (enforced in StockService v2).
/// </summary>
public class StockQuantite
{
    public int Id { get; set; }

    /// <summary>FK → Equipments.Id (future ARTICLE)</summary>
    public int ArticleId { get; set; }

    /// <summary>FK → Depots.Id</summary>
    public int DepotId { get; set; }

    /// <summary>Quantité courante. Ne peut pas être négative (RG-01).</summary>
    public int Quantite { get; set; } = 0;

    /// <summary>Seuil déclenchant une alerte. MLD: seuil_minimum</summary>
    public int SeuilMinimum { get; set; } = 0;

    public DateTime LastUpdatedAt { get; set; }

    // Navigation
    public virtual Equipment Article { get; set; } = null!;
    public virtual Depot     Depot   { get; set; } = null!;
}
