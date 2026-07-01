using System.Collections.Generic;

namespace StockManager.Core.Domain.Entities.Catalogue;

public class ChampSpecifique
{
    public int Id { get; set; }
    public int CategorieId { get; set; }
    public virtual Category Categorie { get; set; } = null!;
    
    public string NomChamp { get; set; } = string.Empty;
    public string TypeDonnee { get; set; } = "TEXTE"; // TEXTE, NOMBRE, DATE, BOOLEEN
    public bool Obligatoire { get; set; }
    public int Ordre { get; set; }
    
    public virtual ICollection<ArticleChampValeur> ArticleValues { get; set; } = new List<ArticleChampValeur>();
}
