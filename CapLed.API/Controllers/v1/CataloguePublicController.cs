using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Services;
using System.Linq;
using System.Threading.Tasks;

namespace StockManager.API.Controllers.v1;

[ApiController]
[Route("api/v1/catalogue")]
public class CataloguePublicController : ControllerBase
{
    private readonly ICataloguePublicService _cataloguePublicService;

    public CataloguePublicController(ICataloguePublicService cataloguePublicService)
    {
        _cataloguePublicService = cataloguePublicService;
    }

    /// <summary>
    /// Search and filter catalog items with pagination.
    /// Used by React Front-office (Site Vitrine).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] CatalogueFilterDto filters)
    {
        // Extract generic dynamic fields starting with "spec_"
        var specFilters = Request.Query
            .Where(k => k.Key.StartsWith("spec_"))
            .ToDictionary(k => k.Key.Substring(5), v => v.Value.ToString());

        filters.DynamicSpecs = specFilters;

        var result = await _cataloguePublicService.SearchAsync(filters);
        return Ok(result);
    }

    /// <summary>
    /// Get details for a specific catalog item.
    /// Used by React Front-office (Site Vitrine).
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var result = await _cataloguePublicService.GetDetailsAsync(id);
        if (result == null) return NotFound();

        return Ok(result);
    }
}
