namespace StockManager.Core.Application.DTOs;

public class PhotoDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public string? Caption { get; set; }
}
