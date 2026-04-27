using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Services;

namespace StockManager.API.Controllers.Commercial;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Liste paginée des Bons de Commande.</summary>
    [HttpGet("bc")]
    public async Task<ActionResult> GetAllBonsCommande()
    {
        var bcs = await _orderService.GetAllBonsCommandeAsync();
        return Ok(new
        {
            Items = bcs,
            TotalCount = bcs.Count,
            Page = 1,
            PageSize = bcs.Count
        });
    }

    /// <summary>Créer un BC manuellement (sans Lead).</summary>
    [HttpPost("bc")]
    public async Task<ActionResult<BonCommandeReadDto>> CreateBonCommande(CreateBonCommandeDto dto)
    {
        var bc = await _orderService.CreateBonCommandeAsync(dto);
        return CreatedAtAction(nameof(GetBonCommande), new { id = bc.Id }, bc);
    }

    /// <summary>Créer un BC depuis un Lead ACCEPTE (workflow ERP).</summary>
    [HttpPost("bc/from-lead/{leadId:int}")]
    public async Task<ActionResult<BonCommandeReadDto>> CreateBonCommandeFromLead(int leadId)
    {
        try
        {
            var bc = await _orderService.CreateBonCommandeFromLeadAsync(leadId);
            return CreatedAtAction(nameof(GetBonCommande), new { id = bc.Id }, bc);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = "BUSINESS_RULE", message = ex.Message });
        }
    }

    [HttpGet("bc/{id}")]
    public async Task<ActionResult<BonCommandeReadDto>> GetBonCommande(int id)
    {
        var bc = await _orderService.GetBonCommandeAsync(id);
        if (bc == null) return NotFound();
        return Ok(bc);
    }

    /// <summary>Créer un BL (déclenche un mouvement de stock SORTIE).</summary>
    [HttpPost("bl")]
    public async Task<ActionResult<BonLivraisonReadDto>> CreateBonLivraison(CreateBonLivraisonDto dto)
    {
        var bl = await _orderService.CreateBonLivraisonAsync(dto);
        return CreatedAtAction(nameof(GetBonLivraison), new { id = bl.Id }, bl);
    }

    [HttpGet("bl/{id}")]
    public async Task<ActionResult<BonLivraisonReadDto>> GetBonLivraison(int id)
    {
        var bl = await _orderService.GetBonLivraisonAsync(id);
        if (bl == null) return NotFound();
        return Ok(bl);
    }
}
