using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Core.Application.Services;

public class StockServiceV2 : IStockServiceV2
{
    private readonly IUnitOfWork _uow;
    private readonly IStockQuantiteRepository _stockQuantiteRepository;
    private readonly IAlerteStockRepository _alerteStockRepository;
    private readonly IStockMovementRepository _movementRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public StockServiceV2(
        IUnitOfWork uow,
        IStockQuantiteRepository stockQuantiteRepository,
        IAlerteStockRepository alerteStockRepository,
        IStockMovementRepository movementRepository,
        IEquipmentRepository equipmentRepository)
    {
        _uow = uow;
        _stockQuantiteRepository = stockQuantiteRepository;
        _alerteStockRepository = alerteStockRepository;
        _movementRepository = movementRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<StockMovement> CreateMouvementAsync(CreateMouvementDto dto, int utilisateurId)
    {
        // Atomic transaction (RG-03 for TRANSFERT) via UnitOfWork abstraction
        await _uow.BeginTransactionAsync();
        try
        {
            var article = await _equipmentRepository.GetByIdAsync(dto.ArticleId) 
                ?? throw new Exception($"Article {dto.ArticleId} introuvable.");

            var movement = new StockMovement
            {
                EquipmentId = dto.ArticleId,
                UserId = utilisateurId,
                TypeMouvement = dto.TypeMouvement,
                Quantity = dto.Quantite,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                DepotSourceId = dto.DepotSourceId,
                DepotDestinationId = dto.DepotDestinationId
            };

            // Mapping legacy Type (entry/exit) for backward compatibility
            movement.Type = dto.TypeMouvement switch
            {
                "ENTREE" or "RETOUR" => StockManager.Core.Domain.Enums.MovementType.ENTRY,
                "SORTIE" => StockManager.Core.Domain.Enums.MovementType.EXIT,
                _ => StockManager.Core.Domain.Enums.MovementType.EXIT // default
            };

            switch (dto.TypeMouvement)
            {
                case "ENTREE":
                    if (dto.DepotDestinationId == null) throw new Exception("Dépôt destination requis pour une ENTREE.");
                    await AdjustStockAsync(dto.ArticleId, dto.DepotDestinationId.Value, dto.Quantite);
                    break;

                case "SORTIE":
                    if (dto.DepotSourceId == null) throw new Exception("Dépôt source requis pour une SORTIE.");
                    await AdjustStockAsync(dto.ArticleId, dto.DepotSourceId.Value, -dto.Quantite);
                    break;

                case "RETOUR":
                    if (dto.DepotDestinationId == null) throw new Exception("Dépôt destination requis pour un RETOUR.");
                    await AdjustStockAsync(dto.ArticleId, dto.DepotDestinationId.Value, dto.Quantite);
                    break;

                case "TRANSFERT":
                    if (dto.DepotSourceId == null || dto.DepotDestinationId == null)
                        throw new Exception("Dépôt source et destination requis pour un TRANSFERT.");
                    
                    await AdjustStockAsync(dto.ArticleId, dto.DepotSourceId.Value, -dto.Quantite);
                    await AdjustStockAsync(dto.ArticleId, dto.DepotDestinationId.Value, dto.Quantite);
                    break;

                default:
                    throw new Exception($"Type de mouvement inconnu : {dto.TypeMouvement}");
            }

            // RG-02: Save movement record
            await _movementRepository.AddAsync(movement);

            // Sync legacy Quantity for backward compatibility with v1
            // Get total stock across ALL depots using repository method
            article.Quantity = await _stockQuantiteRepository.GetTotalStockAsync(dto.ArticleId);
            await _equipmentRepository.UpdateAsync(article);

            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();
            
            return movement;
        }
        catch
        {
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task AdjustStockAsync(int articleId, int depotId, int delta)
    {
        var sq = await _stockQuantiteRepository.GetByArticleAndDepotAsync(articleId, depotId);
        
        if (sq == null)
        {
            if (delta < 0) throw new Exception("Stock insuffisant (dépôt vide).");
            
            sq = new StockQuantite
            {
                ArticleId = articleId,
                DepotId = depotId,
                Quantite = delta,
                LastUpdatedAt = DateTime.UtcNow
            };
            await _stockQuantiteRepository.AddAsync(sq);
        }
        else
        {
            // RG-01: Prevent negative stock
            if (sq.Quantite + delta < 0)
                throw new Exception($"Stock insuffisant dans le dépôt {depotId}. Disponible: {sq.Quantite}, Requis: {-delta}.");

            sq.Quantite += delta;
            sq.LastUpdatedAt = DateTime.UtcNow;
            await _stockQuantiteRepository.UpdateAsync(sq);
        }

        // RG-05/RG-06: Evaluate alerts
        await EvaluateAlertAsync(sq);
    }

    private async Task EvaluateAlertAsync(StockQuantite sq)
    {
        string level = string.Empty;
        
        if (sq.Quantite == 0) level = "RUPTURE";
        else if (sq.Quantite <= sq.SeuilMinimum * 0.25) level = "CRITIQUE";
        else if (sq.Quantite <= sq.SeuilMinimum * 0.50) level = "AVERTISSEMENT";

        var activeAlert = await _alerteStockRepository.GetActiveAlertAsync(sq.ArticleId, sq.DepotId);

        if (!string.IsNullOrEmpty(level))
        {
            // Trigger or update alert
            if (activeAlert == null)
            {
                await _alerteStockRepository.AddAsync(new AlerteStock
                {
                    ArticleId = sq.ArticleId,
                    DepotId = sq.DepotId,
                    NiveauAlerte = level,
                    QuantiteAuDeclenchement = sq.Quantite,
                    SeuilUtilise = sq.SeuilMinimum,
                    DateCreation = DateTime.UtcNow
                });
            }
            else if (activeAlert.NiveauAlerte != level)
            {
                activeAlert.NiveauAlerte = level;
                await _alerteStockRepository.UpdateAsync(activeAlert);
            }
        }
        else if (activeAlert != null)
        {
            // RG-06: Resolve alert
            activeAlert.EstResolue = true;
            activeAlert.DateResolution = DateTime.UtcNow;
            await _alerteStockRepository.UpdateAsync(activeAlert);
        }
    }
}
