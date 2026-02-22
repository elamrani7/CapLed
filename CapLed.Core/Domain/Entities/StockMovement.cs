using StockManager.Core.Domain.Enums;

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

    // Navigation Properties
    public virtual Equipment Equipment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

