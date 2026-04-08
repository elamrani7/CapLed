using StockManager.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Domain.Entities.Commercial;

public class BonLivraison
{
    public int Id { get; set; }
    
    [Required]
    public string NumeroBL { get; set; } = string.Empty; // BL-AAAA-NNNN
    
    public int? BonCommandeId { get; set; }
    public virtual BonCommande? BonCommande { get; set; }
    
    public int ClientId { get; set; }
    public virtual Client Client { get; set; } = null!;
    
    public DateTime DateLivraison { get; set; } = DateTime.UtcNow;
    public string Statut { get; set; } = "BROUILLON"; // BROUILLON | VALIDE | ANNULE
    
    public string? AdresseLivraison { get; set; }
    public string? Transporteur { get; set; }
    public string? NumeroSuivi { get; set; }
    
    public virtual ICollection<LigneBL> Lignes { get; set; } = new List<LigneBL>();
}

public class LigneBL
{
    public int Id { get; set; }
    public int BonLivraisonId { get; set; }
    public virtual BonLivraison BonLivraison { get; set; } = null!;
    
    public int ArticleId { get; set; }
    public virtual Equipment Article { get; set; } = null!;
    
    public int QuantiteLivree { get; set; }
    
    // For traceability
    public int? LotId { get; set; }
    public string? NumeroSerie { get; set; }
}
