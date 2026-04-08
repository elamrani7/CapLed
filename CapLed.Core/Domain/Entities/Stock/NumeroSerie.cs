using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities.Stock;

/// <summary>
/// Gestion par Numéro de Série. MLD: NUMERO_SERIE
/// Utilisé pour le suivi individuel d'articles critiques ou onéreux.
/// </summary>
public class NumeroSerie
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int DepotId { get; set; }

    /// <summary>Numéro de série unique de l'unité.</summary>
    public string NumeroSerieLabel { get; set; } = string.Empty;

    /// <summary>Statut actuel de l'unité : DISPONIBLE, SORTI, RESERVE, DEFECTUEUX.</summary>
    public SerialStatus Statut { get; set; } = SerialStatus.DISPONIBLE;

    /// <summary>Date d'entrée en stock de cette unité précise.</summary>
    public DateTime DateEntree { get; set; }

    // Navigation
    public virtual Equipment Article { get; set; } = null!;
    public virtual Depot     Depot   { get; set; } = null!;
}
