using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Domain.Entities.Commercial;

/// <summary>
/// Ligne d'une demande de devis — article + quantité demandée. MLD: LIGNE_LEAD
/// </summary>
public class LigneLead
{
    public int Id { get; set; }

    public int LeadId    { get; set; }
    public int ArticleId { get; set; }

    public int     QuantiteDemandee { get; set; }
    public string? Commentaire      { get; set; }

    // Navigation
    public virtual Lead      Lead    { get; set; } = null!;
    public virtual Equipment Article { get; set; } = null!;
}
