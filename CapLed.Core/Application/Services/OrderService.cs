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
    private readonly ILeadRepository _leadRepo;
    private readonly IStockServiceV3 _stockService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(
        IBonCommandeRepository bcRepo,
        IBonLivraisonRepository blRepo,
        ILeadRepository leadRepo,
        IStockServiceV3 stockService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _bcRepo = bcRepo;
        _blRepo = blRepo;
        _leadRepo = leadRepo;
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

    /// <summary>
    /// Crée un Bon de Commande à partir d'un Lead ACCEPTE.
    /// Règles métier :
    ///   - Le Lead doit exister
    ///   - Le Lead doit avoir le statut ACCEPTE
    ///   - Un seul BC par Lead (relation 1:1)
    /// </summary>
    public async Task<BonCommandeReadDto> CreateBonCommandeFromLeadAsync(int leadId)
    {
        // 1. Charger le Lead avec ses lignes et son client
        var lead = await _leadRepo.GetByIdAsync(leadId);
        if (lead == null)
            throw new InvalidOperationException("Lead introuvable.");

        // 2. Vérifier le statut
        if (lead.Statut != "ACCEPTE")
            throw new InvalidOperationException($"Le Lead doit être au statut ACCEPTE pour générer un BC. Statut actuel : {lead.Statut}.");

        // 3. Vérifier qu'aucun BC n'existe déjà pour ce Lead
        var existingBc = await _bcRepo.GetByLeadIdAsync(leadId);
        if (existingBc != null)
            throw new InvalidOperationException($"Un Bon de Commande ({existingBc.NumeroBC}) existe déjà pour ce Lead.");

        // 4. Construire le BC depuis les données du Lead
        var bc = new BonCommande
        {
            NumeroBC = await GenerateNumeroAsync("BC"),
            ClientId = lead.ClientId,
            DateCommande = DateTime.UtcNow,
            Statut = "EN_ATTENTE",
            LeadId = lead.Id,
            Commentaire = $"Généré depuis le devis {lead.NumeroDevis}",
            Lignes = lead.Lignes.Select(ll => new LigneBC
            {
                ArticleId = ll.ArticleId,
                QuantiteCommandee = ll.QuantiteDemandee
            }).ToList()
        };

        await _bcRepo.AddAsync(bc);
        await _unitOfWork.SaveChangesAsync();

        // 5. Recharger avec les navigations pour le DTO
        var saved = await _bcRepo.GetByIdAsync(bc.Id);
        var dto = _mapper.Map<BonCommandeReadDto>(saved);
        dto.LeadId = lead.Id;
        dto.NumeroDevis = lead.NumeroDevis;
        return dto;
    }

    public async Task<BonLivraisonReadDto> CreateBonLivraisonAsync(CreateBonLivraisonDto dto)
    {
        var bl = _mapper.Map<BonLivraison>(dto);
        bl.NumeroBL = await GenerateNumeroAsync("BL");
        bl.Statut = "VALIDE";
        
        await _blRepo.AddAsync(bl);
        
        foreach (var ligne in bl.Lignes)
        {
            var movementDto = new CreateMouvementDto
            {
                ArticleId = ligne.ArticleId,
                TypeMouvement = "SORTIE",
                Quantite = ligne.QuantiteLivree,
                DepotSourceId = 1,
                Remarks = $"Livraison {bl.NumeroBL}",
                NumeroLot = null,
                NumeroSeries = !string.IsNullOrEmpty(ligne.NumeroSerie) ? new List<string> { ligne.NumeroSerie } : new List<string>()
            };
            
            await _stockService.CreateMouvementAsync(movementDto, 1);
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<BonLivraisonReadDto>(bl);
    }

    public async Task<BonCommandeReadDto?> GetBonCommandeAsync(int id)
    {
        var bc = await _bcRepo.GetByIdAsync(id);
        if (bc == null) return null;
        var dto = _mapper.Map<BonCommandeReadDto>(bc);
        dto.LeadId = bc.LeadId;
        dto.NumeroDevis = bc.Lead?.NumeroDevis;
        return dto;
    }

    public async Task<BonLivraisonReadDto?> GetBonLivraisonAsync(int id)
    {
        var bl = await _blRepo.GetByIdAsync(id);
        return _mapper.Map<BonLivraisonReadDto>(bl);
    }

    public async Task<List<BonCommandeReadDto>> GetAllBonsCommandeAsync()
    {
        var bcs = await _bcRepo.GetAllAsync();
        return bcs.Select(bc =>
        {
            var dto = _mapper.Map<BonCommandeReadDto>(bc);
            dto.LeadId = bc.LeadId;
            dto.NumeroDevis = bc.Lead?.NumeroDevis;
            return dto;
        }).ToList();
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
