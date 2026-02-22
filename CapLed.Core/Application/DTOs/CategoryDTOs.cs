using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;
    
    public string? Description { get; set; }
}
