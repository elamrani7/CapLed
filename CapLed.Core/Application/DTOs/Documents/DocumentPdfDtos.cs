using System;
using System.Collections.Generic;

namespace StockManager.Core.Application.DTOs.Documents;

public class DevisPdfDto
{
    public string NumeroDevis { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientTelephone { get; set; }
    public string? ClientSociete { get; set; }
    public string? ClientAdresse { get; set; }
    public List<DocumentLinePdfDto> Lines { get; set; } = new();
    public decimal TotalHT { get; set; }
}

public class BonCommandePdfDto
{
    public string NumeroBC { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientTelephone { get; set; }
    public string? ClientSociete { get; set; }
    public string? ClientAdresse { get; set; }
    public List<DocumentLinePdfDto> Lines { get; set; } = new();
    public decimal TotalHT { get; set; }
}

public class BonLivraisonPdfDto
{
    public string NumeroBL { get; set; } = string.Empty;
    public string? NumeroBC { get; set; }
    public DateTime DateLivraison { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientTelephone { get; set; }
    public string? AdresseLivraison { get; set; }
    public List<DocumentLinePdfDto> Lines { get; set; } = new();
    public List<int> RelatedStockMovementIds { get; set; } = new();
}

public class DocumentLinePdfDto
{
    public string ArticleRef { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal TotalLigne => Quantite * PrixUnitaire;
}
