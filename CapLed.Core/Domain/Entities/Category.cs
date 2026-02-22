using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Properties
    public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}

