using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs.Catalogue;

public class FamilleDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Libelle { get; set; } = string.Empty;

    public string? Description { get; set; }
}
