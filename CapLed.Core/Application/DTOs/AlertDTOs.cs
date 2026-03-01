using StockManager.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs;

public class AlertReadDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int Threshold { get; set; }
    public StockAlertLevel AlertLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Active"; 
}

public class AlertUpdateDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
