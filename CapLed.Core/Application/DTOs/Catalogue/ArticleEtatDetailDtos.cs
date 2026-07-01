namespace StockManager.Core.Application.DTOs.Catalogue;

public class ArticleEtatDetailDto
{
    public string? GradeVisuel { get; set; }
    public string? PannesObservees { get; set; }
    public string? TestsFonctionnels { get; set; }
    public string? RevisionsEffectuees { get; set; }
    public int? GarantieOfferte { get; set; }
}

public class CreateOrUpdateArticleEtatDetailDto : ArticleEtatDetailDto
{
}
