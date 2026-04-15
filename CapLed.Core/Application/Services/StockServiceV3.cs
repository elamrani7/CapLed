using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Entities.Stock;
using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Application.Services;

/// <summary>
/// StockServiceV3 — Routes movements to the correct handler based on Category.TypeGestionStock.
/// 
/// QUANTITE → delegates entirely to StockServiceV2 (legacy-compatible path).
/// LOT      → manages LOT rows + global StockQuantites atomically.
/// SERIALISE → manages NUMERO_SERIE rows per unit + global StockQuantites atomically.
/// 
/// StockServiceV2 remains untouched.
/// </summary>
public class StockServiceV3 : IStockServiceV3
{
    private readonly IStockServiceV2          _v2;
    private readonly IUnitOfWork              _uow;
    private readonly IEquipmentRepository     _equipmentRepo;
    private readonly ICategoryRepository      _categoryRepo;
    private readonly ILotRepository           _lotRepo;
    private readonly INumeroSerieRepository   _serieRepo;
    private readonly IStockQuantiteRepository _stockQuantiteRepo;
    private readonly IAlerteStockRepository   _alerteRepo;
    private readonly IStockMovementRepository _movementRepo;

    public StockServiceV3(
        IStockServiceV2          v2,
        IUnitOfWork              uow,
        IEquipmentRepository     equipmentRepo,
        ICategoryRepository      categoryRepo,
        ILotRepository           lotRepo,
        INumeroSerieRepository   serieRepo,
        IStockQuantiteRepository stockQuantiteRepo,
        IAlerteStockRepository   alerteRepo,
        IStockMovementRepository movementRepo)
    {
        _v2                = v2;
        _uow               = uow;
        _equipmentRepo     = equipmentRepo;
        _categoryRepo      = categoryRepo;
        _lotRepo           = lotRepo;
        _serieRepo         = serieRepo;
        _stockQuantiteRepo = stockQuantiteRepo;
        _alerteRepo        = alerteRepo;
        _movementRepo      = movementRepo;
    }

    // ── Public entry point ───────────────────────────────────────────────────

    public async Task<StockMovement> CreateMouvementAsync(CreateMouvementDto dto, int utilisateurId)
    {
        var article = await _equipmentRepo.GetByIdAsync(dto.ArticleId)
            ?? throw new Exception($"Article {dto.ArticleId} introuvable.");

        var category = await _categoryRepo.GetByIdAsync(article.CategoryId)
            ?? throw new Exception($"Catégorie {article.CategoryId} introuvable.");

        return category.TypeGestionStock switch
        {
            "LOT"       => await ProcessLotModeAsync(dto, utilisateurId, article),
            "SERIALISE" => await ProcessSerialModeAsync(dto, utilisateurId, article),
            _           => await _v2.CreateMouvementAsync(dto, utilisateurId) // QUANTITE
        };
    }

    // ── LOT MODE ─────────────────────────────────────────────────────────────

    private async Task<StockMovement> ProcessLotModeAsync(CreateMouvementDto dto, int utilisateurId, Equipment article)
    {
        if (string.IsNullOrWhiteSpace(dto.NumeroLot))
            throw new Exception("NumeroLot est obligatoire pour un article géré par LOT.");

        await _uow.BeginTransactionAsync();
        try
        {
            int?          lotId    = null;
            StockMovement movement = BuildMovement(dto, utilisateurId);

            switch (dto.TypeMouvement)
            {
                case "ENTREE":
                case "RETOUR":
                {
                    int depotId = dto.DepotDestinationId
                        ?? throw new Exception("DepotDestinationId requis pour ENTREE/RETOUR.");

                    var lot = await _lotRepo.GetByNumberAsync(dto.ArticleId, depotId, dto.NumeroLot);
                    if (lot == null)
                    {
                        lot = new Lot
                        {
                            ArticleId   = dto.ArticleId,
                            DepotId     = depotId,
                            NumeroLot   = dto.NumeroLot,
                            Quantite    = dto.Quantite,
                            Fournisseur = dto.Fournisseur,
                            Garantie    = dto.Garantie,
                            Certificat  = dto.Certificat,
                            DateEntree  = dto.DateEntreeLot?.ToUniversalTime() ?? DateTime.UtcNow
                        };
                        await _lotRepo.AddAsync(lot);
                        // Save immediately so MySQL assigns lot.Id before we use it as FK
                        await _uow.SaveChangesAsync();
                    }
                    else
                    {
                        lot.Quantite += dto.Quantite;
                        await _lotRepo.UpdateAsync(lot);
                    }
                    lotId = lot.Id; // Now Id is correctly assigned by the DB

                    await AdjustStockQuantiteAsync(dto.ArticleId, depotId, dto.Quantite, article.Quantity);
                    break;
                }

                case "SORTIE":
                {
                    int depotId = dto.DepotSourceId
                        ?? throw new Exception("DepotSourceId requis pour SORTIE.");

                    var lot = await _lotRepo.GetByNumberAsync(dto.ArticleId, depotId, dto.NumeroLot)
                        ?? throw new Exception($"Lot '{dto.NumeroLot}' introuvable dans le dépôt {depotId}.");

                    // RG-01
                    if (lot.Quantite < dto.Quantite)
                        throw new Exception($"Stock insuffisant dans le lot '{dto.NumeroLot}'. Disponible: {lot.Quantite}, Requis: {dto.Quantite}.");

                    lot.Quantite -= dto.Quantite;
                    lotId = lot.Id;

                    await _lotRepo.UpdateAsync(lot);

                    await AdjustStockQuantiteAsync(dto.ArticleId, depotId, -dto.Quantite, article.Quantity);
                    break;
                }

                case "TRANSFERT":
                {
                    int srcId  = dto.DepotSourceId      ?? throw new Exception("DepotSourceId requis pour TRANSFERT.");
                    int dstId  = dto.DepotDestinationId ?? throw new Exception("DepotDestinationId requis pour TRANSFERT.");

                    var srcLot = await _lotRepo.GetByNumberAsync(dto.ArticleId, srcId, dto.NumeroLot)
                        ?? throw new Exception($"Lot '{dto.NumeroLot}' introuvable dans le dépôt source {srcId}.");

                    if (srcLot.Quantite < dto.Quantite)
                        throw new Exception($"Stock insuffisant dans le lot '{dto.NumeroLot}'. Disponible: {srcLot.Quantite}, Requis: {dto.Quantite}.");

                    srcLot.Quantite -= dto.Quantite;
                    lotId = srcLot.Id;

                    await _lotRepo.UpdateAsync(srcLot);

                    // Merge or create lot at destination
                    var dstLot = await _lotRepo.GetByNumberAsync(dto.ArticleId, dstId, dto.NumeroLot);
                    if (dstLot != null)
                    {
                        dstLot.Quantite += dto.Quantite;
                        await _lotRepo.UpdateAsync(dstLot);
                    }
                    else
                    {
                        await _lotRepo.AddAsync(new Lot
                        {
                            ArticleId  = dto.ArticleId,
                            DepotId    = dstId,
                            NumeroLot  = dto.NumeroLot,
                            Quantite   = dto.Quantite,
                            DateEntree = DateTime.UtcNow
                        });
                    }

                    await AdjustStockQuantiteAsync(dto.ArticleId, srcId, -dto.Quantite, article.Quantity);
                    await AdjustStockQuantiteAsync(dto.ArticleId, dstId,  dto.Quantite, article.Quantity);
                    break;
                }

                default:
                    throw new Exception($"Type de mouvement inconnu : {dto.TypeMouvement}");
            }

            movement.LotId = lotId;
            movement.TraceabiliteInfo = $"LOT: {dto.NumeroLot}";
            await _movementRepo.AddAsync(movement);

            // Sync legacy Quantity
            article.Quantity = await _stockQuantiteRepo.GetTotalStockAsync(dto.ArticleId);
            await _equipmentRepo.UpdateAsync(article);

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

    // ── SERIAL MODE ───────────────────────────────────────────────────────────

    private async Task<StockMovement> ProcessSerialModeAsync(CreateMouvementDto dto, int utilisateurId, Equipment article)
    {
        if (dto.NumeroSeries == null || dto.NumeroSeries.Count == 0)
            throw new Exception("NumeroSeries est obligatoire pour un article géré en mode SERIALISE.");

        if (dto.NumeroSeries.Count != dto.Quantite)
            throw new Exception($"Le nombre de numéros de série ({dto.NumeroSeries.Count}) doit correspondre à la quantité ({dto.Quantite}).");

        if (dto.NumeroSeries.Distinct().Count() != dto.NumeroSeries.Count)
            throw new Exception("Les numéros de série fournis contiennent des doublons.");

        await _uow.BeginTransactionAsync();
        try
        {
            StockMovement movement = BuildMovement(dto, utilisateurId);

            switch (dto.TypeMouvement)
            {
                case "ENTREE":
                case "RETOUR":
                {
                    int depotId = dto.DepotDestinationId
                        ?? throw new Exception("DepotDestinationId requis pour ENTREE/RETOUR.");

                    // Validate no existing serial with same number, unless it is SORTI or DEFECTUEUX
                    var newSeries = new List<NumeroSerie>();
                    var existingSeriesToUpdate = new List<NumeroSerie>();

                    foreach (var sn in dto.NumeroSeries)
                    {
                        var existing = await _serieRepo.GetBySerialAsync(sn);
                        if (existing != null)
                        {
                            if (existing.Statut == SerialStatus.SORTI || existing.Statut == SerialStatus.DEFECTUEUX)
                            {
                                existing.Statut = SerialStatus.DISPONIBLE;
                                existing.DepotId = depotId;
                                existing.DateEntree = dto.DateEntreeLot?.ToUniversalTime() ?? DateTime.UtcNow;
                                existingSeriesToUpdate.Add(existing);
                            }
                            else
                            {
                                throw new Exception($"Le numéro de série '{sn}' est déjà {existing.Statut} dans le système.");
                            }
                        }
                        else
                        {
                            newSeries.Add(new NumeroSerie
                            {
                                ArticleId        = dto.ArticleId,
                                DepotId          = depotId,
                                NumeroSerieLabel = sn,
                                Statut           = SerialStatus.DISPONIBLE,
                                DateEntree       = dto.DateEntreeLot?.ToUniversalTime() ?? DateTime.UtcNow
                            });
                        }
                    }

                    if (newSeries.Any())
                        await _serieRepo.AddRangeAsync(newSeries);
                        
                    if (existingSeriesToUpdate.Any())
                        await _serieRepo.UpdateRangeAsync(existingSeriesToUpdate);
                    await AdjustStockQuantiteAsync(dto.ArticleId, depotId, dto.Quantite, article.Quantity);
                    break;
                }

                case "SORTIE":
                {
                    int depotId = dto.DepotSourceId
                        ?? throw new Exception("DepotSourceId requis pour SORTIE.");

                    var series = await _serieRepo.GetBySerialListAsync(dto.NumeroSeries, depotId);
                    ValidateSeriesForRemoval(dto.NumeroSeries, series, depotId);

                    foreach (var ns in series)
                        ns.Statut = SerialStatus.SORTI;

                    await _serieRepo.UpdateRangeAsync(series);
                    await AdjustStockQuantiteAsync(dto.ArticleId, depotId, -dto.Quantite, article.Quantity);
                    break;
                }

                case "TRANSFERT":
                {
                    int srcId = dto.DepotSourceId      ?? throw new Exception("DepotSourceId requis pour TRANSFERT.");
                    int dstId = dto.DepotDestinationId ?? throw new Exception("DepotDestinationId requis pour TRANSFERT.");

                    var series = await _serieRepo.GetBySerialListAsync(dto.NumeroSeries, srcId);
                    ValidateSeriesForRemoval(dto.NumeroSeries, series, srcId);

                    foreach (var ns in series)
                        ns.DepotId = dstId;

                    await _serieRepo.UpdateRangeAsync(series);
                    await AdjustStockQuantiteAsync(dto.ArticleId, srcId, -dto.Quantite, article.Quantity);
                    await AdjustStockQuantiteAsync(dto.ArticleId, dstId,  dto.Quantite, article.Quantity);
                    break;
                }

                default:
                    throw new Exception($"Type de mouvement inconnu : {dto.TypeMouvement}");
            }

            movement.TraceabiliteInfo = $"SN: {string.Join(", ", dto.NumeroSeries)}";
            await _movementRepo.AddAsync(movement);

            // Sync legacy Quantity
            article.Quantity = await _stockQuantiteRepo.GetTotalStockAsync(dto.ArticleId);
            await _equipmentRepo.UpdateAsync(article);

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

    // ── Helpers ───────────────────────────────────────────────────────────────

    private StockMovement BuildMovement(CreateMouvementDto dto, int utilisateurId) => new()
    {
        EquipmentId        = dto.ArticleId,
        UserId             = utilisateurId,
        TypeMouvement      = dto.TypeMouvement,
        Quantity           = dto.Quantite,
        Remarks            = dto.Remarks,
        CreatedAt          = DateTime.UtcNow,
        DepotSourceId      = dto.DepotSourceId,
        DepotDestinationId = dto.DepotDestinationId,
        Type = dto.TypeMouvement switch
        {
            "ENTREE" or "RETOUR" => MovementType.ENTRY,
            _                    => MovementType.EXIT
        }
    };

    private static void ValidateSeriesForRemoval(
        List<string> requested, List<NumeroSerie> found, int depotId)
    {
        if (found.Count != requested.Count)
        {
            var missing = requested.Except(found.Select(s => s.NumeroSerieLabel)).ToList();
            throw new Exception($"Numéros de série introuvables dans le dépôt {depotId}: {string.Join(", ", missing)}");
        }

        var notAvailable = found.Where(s => s.Statut != SerialStatus.DISPONIBLE).ToList();
        if (notAvailable.Any())
            throw new Exception($"Numéros de série non disponibles: {string.Join(", ", notAvailable.Select(s => $"{s.NumeroSerieLabel} ({s.Statut})"))}");
    }

    private async Task AdjustStockQuantiteAsync(int articleId, int depotId, int delta, int legacyQuantity = 0)
    {
        var sq = await _stockQuantiteRepo.GetByArticleAndDepotAsync(articleId, depotId);

        if (sq == null)
        {
            // Migration logic: if no multi-depot record exists, use legacy quantity as base
            // to avoid overwriting existing stock with just the delta
            int baseQuantity = legacyQuantity;

            if (baseQuantity + delta < 0)
                throw new Exception($"Stock insuffisant. Global: {baseQuantity}, Requis: {-delta}.");

            sq = new StockQuantite
            {
                ArticleId     = articleId,
                DepotId       = depotId,
                Quantite      = baseQuantity + delta,
                LastUpdatedAt = DateTime.UtcNow
            };
            await _stockQuantiteRepo.AddAsync(sq);
        }
        else
        {
            if (sq.Quantite + delta < 0)
                throw new Exception($"Stock insuffisant dans le dépôt {depotId}. Disponible: {sq.Quantite}, Requis: {-delta}.");
            sq.Quantite      += delta;
            sq.LastUpdatedAt  = DateTime.UtcNow;
            await _stockQuantiteRepo.UpdateAsync(sq);
        }

        await EvaluateAlertAsync(sq);
    }

    private async Task EvaluateAlertAsync(StockQuantite sq)
    {
        string level = string.Empty;

        if (sq.Quantite == 0) level = "RUPTURE";
        else if (sq.SeuilMinimum > 0)
        {
            if (sq.Quantite <= sq.SeuilMinimum * 0.25) level = "CRITIQUE";
            else if (sq.Quantite <= sq.SeuilMinimum * 0.50) level = "AVERTISSEMENT";
        }

        var activeAlert = await _alerteRepo.GetActiveAlertAsync(sq.ArticleId, sq.DepotId);

        if (!string.IsNullOrEmpty(level))
        {
            if (activeAlert == null)
            {
                await _alerteRepo.AddAsync(new AlerteStock
                {
                    ArticleId              = sq.ArticleId,
                    DepotId                = sq.DepotId,
                    NiveauAlerte           = level,
                    QuantiteAuDeclenchement = sq.Quantite,
                    SeuilUtilise           = sq.SeuilMinimum,
                    DateCreation           = DateTime.UtcNow
                });
            }
            else if (activeAlert.NiveauAlerte != level)
            {
                activeAlert.NiveauAlerte = level;
                await _alerteRepo.UpdateAsync(activeAlert);
            }
        }
        else if (activeAlert != null)
        {
            activeAlert.EstResolue    = true;
            activeAlert.DateResolution = DateTime.UtcNow;
            await _alerteRepo.UpdateAsync(activeAlert);
        }
    }
}
