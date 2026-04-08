using AutoMapper;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Core.Application.DTOs.Stock;

namespace StockManager.Core.Application.Services;

public class OrderService : IOrderService
{
    private readonly IBonCommandeRepository _bcRepo;
    private readonly IBonLivraisonRepository _blRepo;
    private readonly IStockServiceV3 _stockService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(
        IBonCommandeRepository bcRepo,
        IBonLivraisonRepository blRepo,
        IStockServiceV3 stockService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _bcRepo = bcRepo;
        _blRepo = blRepo;
        _stockService = stockService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BonCommandeReadDto> CreateBonCommandeAsync(CreateBonCommandeDto dto)
    {
        var bc = _mapper.Map<BonCommande>(dto);
        bc.NumeroBC = await GenerateNumeroAsync("BC");
        bc.Statut = "EN_ATTENTE";
        
        await _bcRepo.AddAsync(bc);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<BonCommandeReadDto>(bc);
    }

    public async Task<BonLivraisonReadDto> CreateBonLivraisonAsync(CreateBonLivraisonDto dto)
    {
        var bl = _mapper.Map<BonLivraison>(dto);
        bl.NumeroBL = await GenerateNumeroAsync("BL");
        bl.Statut = "VALIDE"; // In a real app, maybe BROUILLON -> VALIDE
        
        // 1. Save the BL
        await _blRepo.AddAsync(bl);
        
        // 2. TRIGGER STOCK SORTIE (Requirement: BL SORTIE triggers StockServiceV3 SORTIE)
        foreach (var ligne in bl.Lignes)
        {
            var movementDto = new CreateMouvementDto
            {
                ArticleId = ligne.ArticleId,
                TypeMouvement = "SORTIE",
                Quantite = ligne.QuantiteLivree,
                DepotSourceId = 1, // Default depot or taken from logic? Assumed 1 for now or needs to be in DTO
                Remarks = $"Livraison {bl.NumeroBL}",
                NumeroLot = null, // Needs to be looked up if needed, or taken from ligne.LotId
                NumeroSeries = !string.IsNullOrEmpty(ligne.NumeroSerie) ? new List<string> { ligne.NumeroSerie } : new List<string>()
            };
            
            await _stockService.CreateMouvementAsync(movementDto, 1); // Pass system user ID 1
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<BonLivraisonReadDto>(bl);
    }

    public async Task<BonCommandeReadDto?> GetBonCommandeAsync(int id)
    {
        var bc = await _bcRepo.GetByIdAsync(id);
        return _mapper.Map<BonCommandeReadDto>(bc);
    }

    public async Task<BonLivraisonReadDto?> GetBonLivraisonAsync(int id)
    {
        var bl = await _blRepo.GetByIdAsync(id);
        return _mapper.Map<BonLivraisonReadDto>(bl);
    }

    private async Task<string> GenerateNumeroAsync(string type)
    {
        string year = DateTime.UtcNow.Year.ToString();
        string prefix = $"{type}-{year}-";
        
        string lastNum = type == "BC" 
            ? await _bcRepo.GetLastNumeroAsync(prefix)
            : await _blRepo.GetLastNumeroAsync(prefix);
            
        int nextId = 1;
        if (!string.IsNullOrEmpty(lastNum))
        {
            string suffix = lastNum.Substring(prefix.Length);
            if (int.TryParse(suffix, out int lastId))
            {
                nextId = lastId + 1;
            }
        }
        
        return $"{prefix}{nextId:D4}";
    }
}
