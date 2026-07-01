namespace StockManager.Core.Domain.Entities.Stock;

/// <summary>
/// Entrepôt ou site de stockage physique. MLD: DEPOT
/// Permet la gestion multi-dépôt conformément au CDC v6 §3.
/// </summary>
public class Depot
{
    public int Id { get; set; }

    /// <summary>Nom du dépôt, unique dans le système.</summary>
    public string Nom { get; set; } = string.Empty;

    public string? Adresse { get; set; }

    /// <summary>FALSE = dépôt archivé, non visible dans les formulaires.</summary>
    public bool EstActif { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual ICollection<StockQuantite> StockQuantites { get; set; } = new List<StockQuantite>();
    public virtual ICollection<AlerteStock>   AlertesStock   { get; set; } = new List<AlerteStock>();

    // Mouvements liés à ce dépôt (source ou destination)
    // Wired in StockMovementConfiguration via two separate FKs
    public virtual ICollection<StockMovement> MouvementsSource      { get; set; } = new List<StockMovement>();
    public virtual ICollection<StockMovement> MouvementsDestination  { get; set; } = new List<StockMovement>();

    public virtual ICollection<Lot>           Lots           { get; set; } = new List<Lot>();
    public virtual ICollection<NumeroSerie>   NumerosSerie   { get; set; } = new List<NumeroSerie>();
}
