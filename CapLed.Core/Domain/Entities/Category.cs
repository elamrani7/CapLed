using StockManager.Core.Domain.Entities.Catalogue;
using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }

    // ── Step 1A: ERP columns ─────────────────────────────────────────────────
    /// <summary>FK vers la table Familles (créée en Step 1B). Nullable durant la transition.
    /// MLD: CATEGORIE.famille_id</summary>
    public int? FamilleId { get; set; }

    /// <summary>Mode de gestion du stock pour cette catégorie. MLD: CATEGORIE.type_gestion_stock
    /// Valeurs : QUANTITE | LOT | SERIALISE</summary>
    public string TypeGestionStock { get; set; } = "QUANTITE";
    // ─────────────────────────────────────────────────────────────────────────

    // Navigation Properties
    public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    public virtual ICollection<StockManager.Core.Domain.Entities.Catalogue.ChampSpecifique> ChampsSpecifiques { get; set; } = new List<StockManager.Core.Domain.Entities.Catalogue.ChampSpecifique>();

    // ── Step 1B: Famille navigation ──────────────────────────────────────────
    public virtual Famille? Famille { get; set; }
    // ─────────────────────────────────────────────────────────────────────────
}
