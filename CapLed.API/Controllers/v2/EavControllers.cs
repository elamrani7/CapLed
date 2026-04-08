using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace StockManager.API.Controllers.v2;

[ApiController]
[Route("api/v2/categories/{id}/dynamic-fields")]
public class ChampSpecifiqueController : ControllerBase
{
    private readonly IChampSpecifiqueService _service;

    public ChampSpecifiqueController(IChampSpecifiqueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByCategorie(int id)
    {
        var result = await _service.GetByCategorieAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int id, [FromBody] CreateChampSpecifiqueDto dto)
    {
        dto.CategorieId = id;
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetByCategorie), new { id = result.CategorieId }, result);
    }

    [HttpDelete("/api/v2/dynamic-fields/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

[ApiController]
[Route("api/v2/articles/{id}/dynamic-fields")]
public class ArticleDynamicFieldsController : ControllerBase
{
    private readonly IArticleDynamicFieldService _service;

    public ArticleDynamicFieldsController(IArticleDynamicFieldService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetValues(int id)
    {
        var result = await _service.GetValuesAsync(id);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateValues(int id, [FromBody] UpdateArticleDynamicFieldsDto dto)
    {
        await _service.UpdateValuesAsync(id, dto.Values);
        return NoContent();
    }
}
