using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EquipmentCondition Condition { get; set; }
    public int Quantity { get; set; }
    public int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public virtual ICollection<ContactRequest> ContactRequests { get; set; } = new List<ContactRequest>();
}

