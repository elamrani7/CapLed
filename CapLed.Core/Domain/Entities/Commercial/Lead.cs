using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Domain.Entities.Commercial;

/// <summary>
/// Demande de devis entrante. MLD: LEAD
/// Workflow: NOUVEAU → EN_COURS → DEVIS_ENVOYE → ACCEPTE / REFUSE
/// </summary>
public class Lead
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    /// <summary>Commercial affecté au lead (nullable — peut être non assigné à la création).</summary>
    public int? CommercialId { get; set; }

    /// <summary>Numéro unique format DEVIS-AAAA-NNNN (RG-07).</summary>
    public string NumeroDevis { get; set; } = string.Empty;

    /// <summary>Statut du workflow de traitement.</summary>
    public string Statut { get; set; } = "NOUVEAU";

    public DateTime DateSoumission { get; set; }
    public DateTime? DateTraitement { get; set; }

    public string? Commentaire { get; set; }

    /// <summary>Canal d'acquisition marketing (SEO, LINKEDIN, FACEBOOK, WHATSAPP, EMAIL, MARKETPLACE, DIRECT).</summary>
    public string SourceAcquisition { get; set; } = "DIRECT";

    // Navigation
    public virtual Client     Client     { get; set; } = null!;
    public virtual User?      Commercial { get; set; }
    public virtual ICollection<LigneLead> Lignes { get; set; } = new List<LigneLead>();
}
