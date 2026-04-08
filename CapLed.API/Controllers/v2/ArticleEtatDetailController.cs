using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs.Catalogue;
using StockManager.Core.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace StockManager.API.Controllers.v2;

[ApiController]
[Route("api/v2/articles/{articleId}/etat")]
public class ArticleEtatDetailController : ControllerBase
{
    private readonly IArticleEtatDetailService _service;

    public ArticleEtatDetailController(IArticleEtatDetailService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int articleId)
    {
        var result = await _service.GetByArticleIdAsync(articleId);
        if (result == null) return NotFound("Details not found for this article.");
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(int articleId, [FromBody] CreateOrUpdateArticleEtatDetailDto dto)
    {
        try
        {
            var result = await _service.CreateOrUpdateAsync(articleId, dto);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int articleId)
    {
        await _service.DeleteAsync(articleId);
        return NoContent();
    }
}
