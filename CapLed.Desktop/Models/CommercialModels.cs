using System;
using System.Collections.Generic;

namespace CapLed.Desktop.Models;

public class ClientModel
{
    public int Id { get; set; }
    public string RaisonSociale { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Siret { get; set; }
}

public class LeadModel
{
    public int Id { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string NomContact { get; set; } = string.Empty;
    public string EmailContact { get; set; } = string.Empty;
    public string? TelephoneContact { get; set; }
    public string? MessageInitial { get; set; }
    public DateTime DateCreation { get; set; }
    public ClientModel? Client { get; set; }
    public string? CommercialNom { get; set; }
    public List<LigneLeadModel> Lignes { get; set; } = new();
}

public class LigneLeadModel
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public string ArticleReference { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal? PrixUnitairePropose { get; set; }
}

public class UpdateLeadStatusModel
{
    public string Statut { get; set; } = string.Empty;
}
