using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Domain.Entities.Catalogue;

/// <summary>
/// Groupe de catégories homogènes. MLD: FAMILLE
/// Exemples : Transformateurs, Câbles, Automates, etc.
/// </summary>
public class Famille
{
    public int Id { get; set; }

    /// <summary>Code technique court et unique. Ex: "TRANSFO", "CABLE".</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Libellé affiché à l'utilisateur.</summary>
    public string Libelle { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
