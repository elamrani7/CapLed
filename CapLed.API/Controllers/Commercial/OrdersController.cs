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

    /// <summary>Supprimer un BC (si EN_ATTENTE).</summary>
    [HttpDelete("bc/{id}")]
    public async Task<ActionResult> DeleteBonCommande(int id)
    {
        try
        {
            await _orderService.DeleteBonCommandeAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = "BUSINESS_RULE", message = ex.Message });
        }
    }

    /// <summary>Créer un BL (déclenche un mouvement de stock SORTIE).</summary>
    [HttpPost("bl")]
    public async Task<ActionResult<BonLivraisonReadDto>> CreateBonLivraison(CreateBonLivraisonDto dto)
    {
        var bl = await _orderService.CreateBonLivraisonAsync(dto);
        return CreatedAtAction(nameof(GetBonLivraison), new { id = bl.Id }, bl);
    }

    /// <summary>Créer un BL à partir d'un BC (déclenche SORTIE stock depuis un dépôt spécifique).</summary>
    [HttpPost("bl/from-bc/{bcId:int}")]
    public async Task<ActionResult<BonLivraisonReadDto>> CreateBonLivraisonFromBc(int bcId, [FromQuery] int depotId)
    {
        try
        {
            var bl = await _orderService.CreateBonLivraisonFromBcAsync(bcId, depotId);
            return CreatedAtAction(nameof(GetBonLivraison), new { id = bl.Id }, bl);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = "BUSINESS_RULE", message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = "STOCK_ERROR", message = ex.Message });
        }
    }

    [HttpGet("bl/{id}")]
    public async Task<ActionResult<BonLivraisonReadDto>> GetBonLivraison(int id)
    {
        var bl = await _orderService.GetBonLivraisonAsync(id);
        if (bl == null) return NotFound();
        return Ok(bl);
    }
}
