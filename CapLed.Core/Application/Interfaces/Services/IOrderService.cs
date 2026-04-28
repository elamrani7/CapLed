using StockManager.Core.Application.DTOs.Commercial;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IOrderService
{
    Task<BonCommandeReadDto> CreateBonCommandeAsync(CreateBonCommandeDto dto);
    Task<BonCommandeReadDto> CreateBonCommandeFromLeadAsync(int leadId);
    Task<BonLivraisonReadDto> CreateBonLivraisonAsync(CreateBonLivraisonDto dto);
    Task<BonLivraisonReadDto> CreateBonLivraisonFromBcAsync(int bcId, int depotId);
    Task<BonCommandeReadDto?> GetBonCommandeAsync(int id);
    Task<BonLivraisonReadDto?> GetBonLivraisonAsync(int id);
    Task<List<BonCommandeReadDto>> GetAllBonsCommandeAsync();
    Task DeleteBonCommandeAsync(int id);
}
