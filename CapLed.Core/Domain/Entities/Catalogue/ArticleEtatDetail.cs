namespace StockManager.Core.Domain.Entities.Catalogue;

public class ArticleEtatDetail
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public virtual Equipment Article { get; set; } = null!;
    
    public string? GradeVisuel { get; set; } // A, B, C
    public string? PannesObservees { get; set; }
    public string? TestsFonctionnels { get; set; }
    public string? RevisionsEffectuees { get; set; }
    public int? GarantieOfferte { get; set; }
}
