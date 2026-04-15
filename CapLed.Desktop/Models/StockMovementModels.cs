namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors StockMovementReadDto — used for movement history list.
/// </summary>
public class StockMovementModel
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Type { get; set; } = "ENTRY";
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public string UserName { get; set; } = string.Empty;

    // ── Traçabilité (Step 3) ──
    public string? NumeroLot { get; set; }
    public List<string>? NumeroSeries { get; set; }

    public string? TraceabiliteInfo { get; set; }

    // Helpers pour l'affichage WPF
    public bool IsHistoricalLot => TraceabiliteInfo?.StartsWith("LOT:") == true;
    public bool IsHistoricalSn => TraceabiliteInfo?.StartsWith("SN:") == true;
    public string? HistoricalDisplayData => TraceabiliteInfo != null && TraceabiliteInfo.Length > 4 
        ? TraceabiliteInfo.Substring(4).Trim() 
        : null;
}

/// <summary>
/// Mirrors CreateMouvementDto — used for registering new movements via api/v2/mouvements.
/// Property names match the backend DTO exactly (camelCase handled by HttpClient JsonOptions).
/// </summary>
public class StockMovementCreateModel
{
    public int ArticleId { get; set; }
    public int Quantite { get; set; }
    public string TypeMouvement { get; set; } = "ENTREE";
    public string? Remarks { get; set; }
    public int? DepotSourceId { get; set; }
    public int? DepotDestinationId { get; set; }

    // ── Mode LOT ──
    public string? NumeroLot { get; set; }
    public DateTime? DateEntreeLot { get; set; }
    public string? Fournisseur { get; set; }
    public string? Garantie { get; set; }
    public string? Certificat { get; set; }

    // ── Mode SERIALISE ──
    public List<string>? NumeroSeries { get; set; }
}

/// <summary>
/// Filter parameters for history search.
/// </summary>
public class StockMovementFilter
{
    public int? EquipmentId { get; set; }
    public string? Type { get; set; } // "ENTRY" or "EXIT"
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
