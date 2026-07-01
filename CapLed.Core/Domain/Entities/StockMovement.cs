using StockManager.Core.Domain.Enums;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Domain.Entities;

public class StockMovement
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public int UserId { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Remarks { get; set; }

    // ── Step 1A: ERP / Multi-dépôt columns ───────────────────────────────────
    public int? DepotSourceId { get; set; }
    public int? DepotDestinationId { get; set; }
    public string TypeMouvement { get; set; } = "ENTREE";

    // ── Step 3: LOT / SERIAL traceability ─────────────────────────────────────
    /// <summary>Lot concerné par ce mouvement (mode LOT uniquement). FK → LOT(Id).</summary>
    public int? LotId { get; set; }

    /// <summary>Numéro de série concerné (mode SERIALISE, 1 ligne = 1 unité). FK → NUMERO_SERIE(Id).</summary>
    public int? NumeroSerieId { get; set; }

    /// <summary>Traceabilité textuelle stockée dans l'historique ("LOT: 123", "SN: A, B")</summary>
    [System.ComponentModel.DataAnnotations.MaxLength(1000)]
    public string? TraceabiliteInfo { get; set; }
    // ─────────────────────────────────────────────────────────────────────────

    // Navigation Properties (legacy)
    public virtual Equipment Equipment { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    // ── Step 1B: Depot navigation properties ─────────────────────────────────
    public virtual Depot? DepotSource      { get; set; }
    public virtual Depot? DepotDestination { get; set; }

    // ── Step 3: LOT / SERIAL navigation ──────────────────────────────────────
    public virtual Lot?         Lot         { get; set; }
    public virtual NumeroSerie? NumeroSerie { get; set; }
    // ─────────────────────────────────────────────────────────────────────────
}
