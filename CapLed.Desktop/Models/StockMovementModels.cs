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
    public string Type { get; set; } = "ENTRY"; // "ENTRY" or "EXIT"
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// Mirrors StockMovementCreateDto — used for registering new movements.
/// </summary>
public class StockMovementCreateModel
{
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; } = "ENTRY";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }
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
