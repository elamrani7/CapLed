using System;

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

    // UI Helper Properties
    public bool IsCritical => CurrentQuantity < (Threshold / 2.0);
    public bool IsWarning => !IsCritical && CurrentQuantity <= Threshold;
}

public class AlertUpdateModel
{
    public string Status { get; set; } = string.Empty;
}
