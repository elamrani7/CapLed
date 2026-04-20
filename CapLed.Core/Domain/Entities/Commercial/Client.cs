namespace StockManager.Core.Domain.Entities.Commercial;

/// <summary>
/// Client ayant soumis une demande de devis. MLD: CLIENT
/// Peut être un prospect ou un client existant.
/// Sert aussi de compte utilisateur pour le site public (authentification JWT).
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

    // ── Authentification site public ────────────────────────────────────
    /// <summary>Mot de passe hashé (null = compte CRM sans accès web).</summary>
    public string? PasswordHash { get; set; }

    /// <summary>Indique si l'e-mail a été confirmé via le lien envoyé.</summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>Token unique envoyé par e-mail pour la confirmation.</summary>
    public string? ConfirmationToken { get; set; }

    /// <summary>Date d'expiration du token de confirmation.</summary>
    public DateTime? TokenExpiry { get; set; }

    // Navigation
    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
