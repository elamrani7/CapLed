using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using System.Security.Claims;

namespace StockManager.API.Controllers.v2;

[ApiController]
[Route("api/v2/mouvements")]
[Authorize]
public class MouvementsController : ControllerBase
{
    private readonly IStockServiceV3         _stockService;
    private readonly IStockMovementRepository _movementRepository;
    private readonly IMapper                  _mapper;

    public MouvementsController(
        IStockServiceV3          stockService,
        IStockMovementRepository movementRepository,
        IMapper                  mapper)
    {
        _stockService       = stockService;
        _movementRepository = movementRepository;
        _mapper             = mapper;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    /// <summary>
    /// Créer un mouvement de stock (ENTREE, SORTIE, TRANSFERT, RETOUR).
    /// Supporte les modes QUANTITE, LOT et SERIALISE selon la catégorie de l'article.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MouvementStockReadDto>> CreateMouvement(CreateMouvementDto request)
    {
        var userId   = GetCurrentUserId();
        var movement = await _stockService.CreateMouvementAsync(request, userId);

        var created = await _movementRepository.GetByIdAsync(movement.Id);
        return Ok(_mapper.Map<MouvementStockReadDto>(created));
    }
}
