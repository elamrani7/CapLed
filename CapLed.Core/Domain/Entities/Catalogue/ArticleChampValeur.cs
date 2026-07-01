namespace StockManager.Core.Domain.Entities.Catalogue;

public class ArticleChampValeur
{
    public int Id { get; set; }
    
    public int ArticleId { get; set; }
    public virtual Equipment Article { get; set; } = null!;
    
    public int ChampSpecifiqueId { get; set; }
    public virtual ChampSpecifique ChampSpecifique { get; set; } = null!;
    
    public string? Valeur { get; set; }
}
