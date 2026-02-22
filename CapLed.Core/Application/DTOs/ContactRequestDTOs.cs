using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs;

public class ContactRequestCreateDto
{
    [Required]
    [StringLength(100)]
    public string VisitorName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    public List<int> EquipmentIds { get; set; } = new();
}

public class ContactRequestReadDto
{
    public int Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<string> RelatedEquipmentNames { get; set; } = new();
}
