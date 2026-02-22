using StockManager.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs;

public class StockMovementReadDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public MovementType Type { get; set; }
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class StockMovementCreateDto
{
    [Required]
    public int EquipmentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public MovementType Type { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Comment { get; set; }
}
