using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CapLed.Desktop.Models;

public class BonCommandeModel
{
    public int Id { get; set; }

    [JsonPropertyName("numeroBC")]
    public string Numero { get; set; } = string.Empty;

    [JsonPropertyName("dateCommande")]
    public DateTime DateCreation { get; set; }

    [JsonPropertyName("clientNom")]
    public string ClientNom { get; set; } = string.Empty;

    public string Statut { get; set; } = string.Empty;
    public decimal MontantTotal { get; set; }
    public ClientModel? Client { get; set; }
    public List<LigneBcModel> Lignes { get; set; } = new();
}

public class LigneBcModel
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
}

public class BonLivraisonModel
{
    public int Id { get; set; }
    
    [JsonPropertyName("numeroBL")]
    public string Numero { get; set; } = string.Empty;
    
    public DateTime? DateLivraison { get; set; }
    
    [JsonPropertyName("statut")]
    public string StatutLivraison { get; set; } = string.Empty;
    
    public string DepotNom { get; set; } = string.Empty;
    
    [JsonPropertyName("numeroBC")]
    public string? NumeroBC { get; set; }
    
    [JsonIgnore]
    public BonCommandeModel BonCommande => new BonCommandeModel { Numero = NumeroBC ?? string.Empty };
    
    public List<LigneBlModel> Lignes { get; set; } = new();
}

public class LigneBlModel
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int QuantiteLivree { get; set; }
}
