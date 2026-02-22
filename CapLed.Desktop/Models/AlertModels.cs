namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors AlertReadDto — displayed in the low-stock alerts panel.
/// </summary>
public class AlertModel
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Threshold { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Active";
}
