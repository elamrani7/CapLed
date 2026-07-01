using StockManager.Core.Application.DTOs;

namespace StockManager.Core.Application.DTOs.Commercial;

public class BonCommandeReadDto
{
    public int Id { get; set; }
    public string NumeroBC { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientNom { get; set; } = string.Empty;
    public DateTime DateCommande { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
    public int? LeadId { get; set; }
    public string? NumeroDevis { get; set; }
    public decimal MontantTotal { get; set; }
    public List<LigneBCReadDto> Lignes { get; set; } = new();
}

public class LigneBCReadDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleRef { get; set; } = string.Empty;
    public string ArticleName { get; set; } = string.Empty;
    public int QuantiteCommandee { get; set; }
    public decimal PrixUnitaire { get; set; }
}

public class CreateBonCommandeDto
{
    public int ClientId { get; set; }
    public string? Commentaire { get; set; }
    public List<CreateLigneBCDto> Lignes { get; set; } = new();
}

public class CreateLigneBCDto
{
    public int ArticleId { get; set; }
    public int QuantiteCommandee { get; set; }
}

// Same for BL
public class BonLivraisonReadDto
{
    public int Id { get; set; }
    public string NumeroBL { get; set; } = string.Empty;
    public string? NumeroBC { get; set; }
    public string ClientNom { get; set; } = string.Empty;
    public DateTime DateLivraison { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string? AdresseLivraison { get; set; }
    
    // Multi-dépôt
    public int DepotId { get; set; }
    public string DepotNom { get; set; } = string.Empty;

    public List<LigneBLReadDto> Lignes { get; set; } = new();
}

public class LigneBLReadDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleRef { get; set; } = string.Empty;
    public string ArticleName { get; set; } = string.Empty;
    public int QuantiteLivree { get; set; }
    public string? NumeroSerie { get; set; }
}

public class CreateBonLivraisonDto
{
    public int? BonCommandeId { get; set; }
    public int ClientId { get; set; }
    public string? AdresseLivraison { get; set; }
    public string? Transporteur { get; set; }
    
    // Multi-dépôt : Obligatoire
    public int DepotId { get; set; }
    
    public List<CreateLigneBLDto> Lignes { get; set; } = new();
}

public class CreateLigneBLDto
{
    public int ArticleId { get; set; }
    public int QuantiteLivree { get; set; }
    public int? LotId { get; set; }
    public string? NumeroSerie { get; set; }
}
