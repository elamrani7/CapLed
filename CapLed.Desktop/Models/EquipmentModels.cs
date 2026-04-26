using StockManager.Core.Domain.Enums;

namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors EquipmentListItemDto — used in DataGrids and lists.
/// </summary>
public class EquipmentListItemModel
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public StockAlertLevel AlertLevel { get; set; }
    // Champs ajoutés pour enrichir l'inventaire
    public string TypeGestionStock { get; set; } = string.Empty; // QUANTITE / LOT / SERIALISE
    public int MinThreshold { get; set; }
    public decimal? PrixVente { get; set; }
}

/// <summary>
/// Mirrors EquipmentReadDto — used in detail/edit views.
/// </summary>
public class EquipmentDetailModel : EquipmentListItemModel
{
    public string? Description { get; set; }
    public int CategoryId { get; set; } // Added to support selection in Edit form
    public new int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
    public bool VisibleSite { get; set; }
    public new decimal? PrixVente { get; set; }
    
    // EAV & Status Details
    public ArticleEtatDetailModel? EtatDetail { get; set; }
    public List<ArticleChampValeurModel> ChampsSpecifiques { get; set; } = new();

    public List<PhotoModel> Photos { get; set; } = new();
}

public class ArticleEtatDetailModel
{
    public string? GradeVisuel { get; set; }
    public string? PannesObservees { get; set; }
    public string? TestsFonctionnels { get; set; }
    public string? RevisionsEffectuees { get; set; }
    public int? GarantieOfferte { get; set; }
}

public class ArticleChampValeurModel
{
    public int Id { get; set; }
    public string NomChamp { get; set; } = string.Empty;
    public string TypeDonnee { get; set; } = string.Empty;
    public string? Valeur { get; set; }
}

/// <summary>
/// Mirrors EquipmentCreateDto / EquipmentUpdateDto — used in create/edit forms.
/// </summary>
public class EquipmentEditModel
{
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string Condition { get; set; } = "NEUF";
    public int MinThreshold { get; set; }
    public bool IsPublished { get; set; }
    public bool VisibleSite { get; set; }
    public decimal? PrixVente { get; set; }

    public ArticleEtatDetailModel? EtatDetail { get; set; }
    public List<ArticleChampValeurModel> ChampsSpecifiques { get; set; } = new();
}

/// <summary>
/// Mirrors PhotoDto.
/// </summary>
public class PhotoModel
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string FullUrl => $"https://capled-api.onrender.com{Url}";
    public bool IsPrimary { get; set; }
}
