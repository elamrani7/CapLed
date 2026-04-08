namespace StockManager.Core.Domain.Entities.Commercial;

/// <summary>
/// Client ayant soumis une demande de devis. MLD: CLIENT
/// Peut être un prospect ou un client existant.
/// </summary>
public class Client
{
    public int Id { get; set; }

    public string Nom     { get; set; } = string.Empty;
    public string? Prenom { get; set; }

    /// <summary>E-mail unique — identifiant fonctionnel du client.</summary>
    public string Email { get; set; } = string.Empty;

    public string? Telephone { get; set; }
    public string? Societe   { get; set; }
    public string? Adresse   { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
