using StockManager.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Domain.Entities.Commercial;

public class BonCommande
{
    public int Id { get; set; }
    
    [Required]
    public string NumeroBC { get; set; } = string.Empty; // BC-AAAA-NNNN
    
    public int ClientId { get; set; }
    public virtual Client Client { get; set; } = null!;
    
    public DateTime DateCommande { get; set; } = DateTime.UtcNow;
    public string Statut { get; set; } = "EN_ATTENTE"; // EN_ATTENTE | CONFIRME | EN_COURS_LIVRAISON | TERMINE | ANNULE
    
    public string? Commentaire { get; set; }
    
    public virtual ICollection<LigneBC> Lignes { get; set; } = new List<LigneBC>();
    public virtual ICollection<BonLivraison> BonsLivraison { get; set; } = new List<BonLivraison>();
}

public class LigneBC
{
    public int Id { get; set; }
    public int BonCommandeId { get; set; }
    public virtual BonCommande BonCommande { get; set; } = null!;
    
    public int ArticleId { get; set; }
    public virtual Equipment Article { get; set; } = null!;
    
    public int QuantiteCommandee { get; set; }
}
