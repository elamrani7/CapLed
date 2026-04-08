using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    /// <summary>ID de la famille parente. MLD: CATEGORIE.famille_id</summary>
    public int? FamilleId { get; set; }

    /// <summary>Libellé de la famille (Read-only for UI display)</summary>
    public string? FamilleLibelle { get; set; }

    /// <summary>Mode de gestion de stock. Valeurs : QUANTITE | LOT | SERIALISE</summary>
    public string TypeGestionStock { get; set; } = "QUANTITE";
}
