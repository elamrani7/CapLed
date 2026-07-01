using System;
using StockManager.Core.Domain.Enums;

namespace CapLed.Desktop.Models;

public class AlertModel
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Threshold { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Active";
    public StockAlertLevel AlertLevel { get; set; }
}

public class AlertUpdateModel
{
    public string Status { get; set; } = string.Empty;
}
