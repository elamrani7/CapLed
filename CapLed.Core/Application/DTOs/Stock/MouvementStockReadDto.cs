namespace StockManager.Core.Application.DTOs.Stock;

public class MouvementStockReadDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public string TypeMouvement { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Remarks { get; set; }

    public int? DepotSourceId { get; set; }
    public string? DepotSourceNom { get; set; }

    public int? DepotDestinationId { get; set; }
    public string? DepotDestinationNom { get; set; }

    public string UserName { get; set; } = string.Empty;

    // ── Traçabilité LOT / SERIAL (Step 3) ────────────────────────────────────
    public int? LotId { get; set; }
    public string? NumeroLot { get; set; }
    public List<string>? NumeroSeries { get; set; }
}

