namespace StockManager.Core.Domain.Entities.Stock;

/// <summary>
/// Gestion par Lot. MLD: LOT
/// Permet de regrouper des articles par lot avec des informations de traçabilité.
/// </summary>
public class Lot
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int DepotId { get; set; }

    /// <summary>Numéro d'identification du lot (ex: LOT-2024-001).</summary>
    public string NumeroLot { get; set; } = string.Empty;

    /// <summary>Quantité disponible dans ce lot pour ce dépôt.</summary>
    public int Quantite { get; set; }

    /// <summary>Nom du fournisseur à l'origine du lot.</summary>
    public string? Fournisseur { get; set; }

    /// <summary>Date d'entrée en stock du lot.</summary>
    public DateTime DateEntree { get; set; }

    /// <summary>Informations sur la garantie (ex: "24 mois").</summary>
    public string? Garantie { get; set; }

    /// <summary>Référence ou URL vers un certificat de conformité.</summary>
    public string? Certificat { get; set; }

    // Navigation
    public virtual Equipment Article { get; set; } = null!;
    public virtual Depot     Depot   { get; set; } = null!;
}
