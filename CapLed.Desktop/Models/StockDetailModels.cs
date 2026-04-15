namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors ArticleStockDetailDto — détail du stock par article.
/// </summary>
public class ArticleStockDetailModel
{
    public int    ArticleId     { get; set; }
    public string ArticleName   { get; set; } = string.Empty;
    public string TypeGestion   { get; set; } = "QUANTITE";
    public int    TotalQuantite { get; set; }
    public List<LotDetailModel>?   Lots   { get; set; }
    public List<SerieDetailModel>? Series { get; set; }
}

public class LotDetailModel
{
    public int      Id          { get; set; }
    public string   NumeroLot   { get; set; } = string.Empty;
    public int      Quantite    { get; set; }
    public DateTime DateEntree  { get; set; }
    public string?  Fournisseur { get; set; }
    public string?  Garantie    { get; set; }
    public string?  Certificat  { get; set; }
    public string   DepotNom    { get; set; } = string.Empty;
}

public class SerieDetailModel
{
    public int      Id          { get; set; }
    public string   NumeroSerie { get; set; } = string.Empty;
    public string   Statut      { get; set; } = "DISPONIBLE";
    public DateTime DateEntree  { get; set; }
    public string   DepotNom    { get; set; } = string.Empty;

    public bool IsDisponible => Statut == "DISPONIBLE";
}
