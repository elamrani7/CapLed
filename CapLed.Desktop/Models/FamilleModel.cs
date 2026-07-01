namespace CapLed.Desktop.Models;

public class FamilleModel
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }

    public override string ToString() => Libelle;
}
