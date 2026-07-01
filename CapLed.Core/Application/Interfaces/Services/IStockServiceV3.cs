using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Application.Interfaces.Services;

/// <summary>
/// Service de gestion des mouvements de stock v3.
/// Supporte les 3 modes : QUANTITE (délégation à v2), LOT, SERIALISE.
/// </summary>
public interface IStockServiceV3
{
    Task<StockMovement> CreateMouvementAsync(CreateMouvementDto dto, int utilisateurId);
}
