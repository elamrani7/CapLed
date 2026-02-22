namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors CategoryDto — used for both read and write operations.
/// </summary>
public class CategoryModel
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
}
