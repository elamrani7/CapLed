using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs.Stock;

public class CreateMouvementDto
{
    [Required]
    public int ArticleId { get; set; }

    [Required]
    [RegularExpression("ENTREE|SORTIE|TRANSFERT|RETOUR", ErrorMessage = "Le type de mouvement doit être ENTREE, SORTIE, TRANSFERT ou RETOUR.")]
    public string TypeMouvement { get; set; } = "ENTREE";

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0.")]
    public int Quantite { get; set; }

    public int? DepotSourceId { get; set; }
    public int? DepotDestinationId { get; set; }
    public string? Remarks { get; set; }

    // ── Mode LOT ─────────────────────────────────────────────────────────────
    /// <summary>Numéro d'identification du lot concerné par ce mouvement.</summary>
    public string? NumeroLot { get; set; }

    /// <summary>Date d'entrée en stock du lot (ENTREE/RETOUR seulement).</summary>
    public DateTime? DateEntreeLot { get; set; }

    public string? Fournisseur { get; set; }
    public string? Garantie { get; set; }
    public string? Certificat { get; set; }

    // ── Mode SERIALISE ────────────────────────────────────────────────────────
    /// <summary>
    /// Liste des numéros de série concernés par ce mouvement.
    /// Le nombre d'éléments doit être cohérent avec Quantite.
    /// </summary>
    public List<string>? NumeroSeries { get; set; }
}

