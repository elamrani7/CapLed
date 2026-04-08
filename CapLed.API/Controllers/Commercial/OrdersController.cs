using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Services;

namespace StockManager.API.Controllers.Commercial;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("bc")]
    public async Task<ActionResult<BonCommandeReadDto>> CreateBonCommande(CreateBonCommandeDto dto)
    {
        var bc = await _orderService.CreateBonCommandeAsync(dto);
        return CreatedAtAction(nameof(GetBonCommande), new { id = bc.Id }, bc);
    }

    [HttpPost("bl")]
    public async Task<ActionResult<BonLivraisonReadDto>> CreateBonLivraison(CreateBonLivraisonDto dto)
    {
        var bl = await _orderService.CreateBonLivraisonAsync(dto);
        return CreatedAtAction(nameof(GetBonLivraison), new { id = bl.Id }, bl);
    }

    [HttpGet("bc/{id}")]
    public async Task<ActionResult<BonCommandeReadDto>> GetBonCommande(int id)
    {
        var bc = await _orderService.GetBonCommandeAsync(id);
        if (bc == null) return NotFound();
        return Ok(bc);
    }

    [HttpGet("bl/{id}")]
    public async Task<ActionResult<BonLivraisonReadDto>> GetBonLivraison(int id)
    {
        var bl = await _orderService.GetBonLivraisonAsync(id);
        if (bl == null) return NotFound();
        return Ok(bl);
    }
}
