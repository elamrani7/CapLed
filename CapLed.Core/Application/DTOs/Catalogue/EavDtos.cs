using System.Collections.Generic;

namespace StockManager.Core.Application.DTOs.Catalogue;

public class CreateChampSpecifiqueDto
{
    public int CategorieId { get; set; }
    public string NomChamp { get; set; } = string.Empty;
    public string TypeDonnee { get; set; } = "TEXTE";
    public bool Obligatoire { get; set; }
    public int Ordre { get; set; }
}

public class ChampSpecifiqueReadDto
{
    public int Id { get; set; }
    public string NomChamp { get; set; } = string.Empty;
    public string TypeDonnee { get; set; } = string.Empty;
    public bool Obligatoire { get; set; }
    public int Ordre { get; set; }
}

public class ArticleChampValeurDto
{
    public int ChampSpecifiqueId { get; set; }
    public string NomChamp { get; set; } = string.Empty;
    public string TypeDonnee { get; set; } = string.Empty;
    public string? Valeur { get; set; }
}

public class UpdateArticleDynamicFieldsDto
{
    public List<ArticleChampValeurDto> Values { get; set; } = new();
}
