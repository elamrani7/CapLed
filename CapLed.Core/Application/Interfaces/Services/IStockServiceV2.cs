using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Domain.Entities;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IStockServiceV2
{
    Task<StockMovement> CreateMouvementAsync(CreateMouvementDto dto, int utilisateurId);
}
