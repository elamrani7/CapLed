using StockManager.Core.Application.DTOs.Commercial;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IOrderService
{
    Task<BonCommandeReadDto> CreateBonCommandeAsync(CreateBonCommandeDto dto);
    Task<BonLivraisonReadDto> CreateBonLivraisonAsync(CreateBonLivraisonDto dto);
    Task<BonCommandeReadDto?> GetBonCommandeAsync(int id);
    Task<BonLivraisonReadDto?> GetBonLivraisonAsync(int id);
}
