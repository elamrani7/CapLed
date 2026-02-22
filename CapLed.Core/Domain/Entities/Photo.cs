using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class Photo
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    // Navigation Properties
    public virtual Equipment Equipment { get; set; } = null!;
}

