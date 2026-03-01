using StockManager.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs;

/* --- BACK-OFFICE DTOs --- */

public class EquipmentListItemDto
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public EquipmentCondition Condition { get; set; }
    public int Quantity { get; set; }
    public StockAlertLevel AlertLevel { get; set; }
}

public class EquipmentReadDto : EquipmentListItemDto
{
    public string? Description { get; set; }
    public int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
    public List<PhotoDto> Photos { get; set; } = new();
}

public class EquipmentCreateDto
{
    [Required(ErrorMessage = "La référence est obligatoire.")]
    [StringLength(50)]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public EquipmentCondition Condition { get; set; }

    [Range(0, int.MaxValue)]
    public int MinThreshold { get; set; }

    public bool IsPublished { get; set; }
}

public class EquipmentUpdateDto : EquipmentCreateDto
{
    // Inherits all fields from CreateDto
}

/* --- FRONT-OFFICE (CATALOG) DTOs --- */

public class EquipmentCatalogItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public EquipmentCondition Condition { get; set; }
    public string? MainPhotoUrl { get; set; }
    public int AvailableQuantity { get; set; }
}

public class EquipmentCatalogDetailDto : EquipmentCatalogItemDto
{
    public string? Description { get; set; }
    public List<PhotoDto> Photos { get; set; } = new();
    // Simplified related equipment could be added here later
}
