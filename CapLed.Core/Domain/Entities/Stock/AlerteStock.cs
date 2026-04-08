namespace StockManager.Core.Domain.Entities.Stock;

/// <summary>
/// Alerte de stock déclenchée quand la quantité tombe sous le seuil. MLD: ALERTE_STOCK
/// Remplace la logique d'alerte en mémoire de l'ancien StockAlertHelper.
/// Valeurs de NiveauAlerte : AVERTISSEMENT | CRITIQUE | RUPTURE
/// </summary>
public class AlerteStock
{
    public int Id { get; set; }

    /// <summary>FK → Equipments.Id (future ARTICLE)</summary>
    public int ArticleId { get; set; }

    /// <summary>FK → Depots.Id</summary>
    public int DepotId { get; set; }

    /// <summary>Sévérité de l'alerte : AVERTISSEMENT, CRITIQUE, RUPTURE.</summary>
    public string NiveauAlerte { get; set; } = string.Empty;

    /// <summary>Quantité au moment du déclenchement de l'alerte.</summary>
    public int QuantiteAuDeclenchement { get; set; }

    /// <summary>Seuil minimum utilisé pour le calcul.</summary>
    public int SeuilUtilise { get; set; }

    /// <summary>FALSE = alerte en cours, TRUE = résolue (stock remonté).</summary>
    public bool EstResolue { get; set; } = false;

    public DateTime  DateCreation   { get; set; }
    public DateTime? DateResolution { get; set; }

    // Navigation
    public virtual Equipment Article { get; set; } = null!;
    public virtual Depot     Depot   { get; set; } = null!;
}
