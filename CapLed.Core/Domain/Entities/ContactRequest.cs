using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class ContactRequest
{
    public int Id { get; set; }
    public int? EquipmentId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ContactStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public virtual Equipment? Equipment { get; set; }
}

