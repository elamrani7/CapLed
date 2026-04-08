using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs.Commercial;

// ── DTOs CLIENT ──────────────────────────────────────────────────────────────

public class CreateClientDto
{
    [Required] [MaxLength(100)] public string Nom     { get; set; } = string.Empty;
    [MaxLength(100)]            public string? Prenom  { get; set; }
    [Required] [EmailAddress]   public string Email   { get; set; } = string.Empty;
    [MaxLength(20)]             public string? Telephone { get; set; }
    [MaxLength(200)]            public string? Societe   { get; set; }
    public string? Adresse { get; set; }
}

public class ClientReadDto
{
    public int     Id        { get; set; }
    public string  Nom       { get; set; } = string.Empty;
    public string? Prenom    { get; set; }
    public string  Email     { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Societe   { get; set; }
    public string? Adresse   { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ── DTOs LEAD ────────────────────────────────────────────────────────────────

public class CreateLeadDto
{
    // Client info — creates or matches existing client by email
    [Required] [MaxLength(100)] public string  NomClient   { get; set; } = string.Empty;
    [MaxLength(100)]            public string? PrenomClient { get; set; }
    [Required] [EmailAddress]   public string  EmailClient  { get; set; } = string.Empty;
    [MaxLength(20)]             public string? Telephone    { get; set; }
    [MaxLength(200)]            public string? Societe      { get; set; }

    public string? Commentaire      { get; set; }
    public string  SourceAcquisition { get; set; } = "DIRECT";

    [Required] [MinLength(1)]
    public List<CreateLigneLeadDto> Lignes { get; set; } = new();
}

public class CreateLigneLeadDto
{
    [Required] [Range(1, int.MaxValue)] public int ArticleId        { get; set; }
    [Required] [Range(1, int.MaxValue)] public int QuantiteDemandee { get; set; }
    public string? Commentaire { get; set; }
}

public class LeadReadDto
{
    public int      Id               { get; set; }
    public string   NumeroDevis      { get; set; } = string.Empty;
    public string   Statut           { get; set; } = string.Empty;
    public string   SourceAcquisition { get; set; } = string.Empty;
    public DateTime DateSoumission   { get; set; }
    public DateTime? DateTraitement  { get; set; }
    public string?  Commentaire      { get; set; }

    public ClientReadDto Client { get; set; } = null!;
    public string?  CommercialNom { get; set; }

    public List<LigneLeadReadDto> Lignes { get; set; } = new();
}

public class LigneLeadReadDto
{
    public int     Id               { get; set; }
    public int     ArticleId        { get; set; }
    public string  ArticleRef       { get; set; } = string.Empty;
    public string  ArticleName      { get; set; } = string.Empty;
    public int     QuantiteDemandee { get; set; }
    public string? Commentaire      { get; set; }
}

// ── DTO: Mise à jour statut ───────────────────────────────────────────────────

public class UpdateLeadStatutDto
{
    [Required]
    [RegularExpression("NOUVEAU|EN_COURS|DEVIS_ENVOYE|ACCEPTE|REFUSE",
        ErrorMessage = "Statut invalide. Valeurs: NOUVEAU, EN_COURS, DEVIS_ENVOYE, ACCEPTE, REFUSE")]
    public string Statut { get; set; } = string.Empty;

    public int? CommercialId { get; set; }
    public string? Commentaire { get; set; }
}
