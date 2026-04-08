using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace StockManager.API.Controllers.v2;

[ApiController]
[Route("api/v2/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentPdfService _pdfService;

    public DocumentsController(IDocumentPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpGet("devis/{id}/pdf")]
    public async Task<IActionResult> GetDevisPdf(int id)
    {
        var pdfBytes = await _pdfService.GenerateDevisPdfAsync(id);
        if (pdfBytes.Length == 0) return NotFound("Devis not found.");
        
        return File(pdfBytes, "application/pdf", $"DEVIS-{id}.pdf");
    }

    [HttpGet("bc/{id}/pdf")]
    public async Task<IActionResult> GetBonCommandePdf(int id)
    {
        var pdfBytes = await _pdfService.GenerateBonCommandePdfAsync(id);
        if (pdfBytes.Length == 0) return NotFound("Bon de Commande not found.");
        
        return File(pdfBytes, "application/pdf", $"BC-{id}.pdf");
    }

    [HttpGet("bl/{id}/pdf")]
    public async Task<IActionResult> GetBonLivraisonPdf(int id)
    {
        var pdfBytes = await _pdfService.GenerateBonLivraisonPdfAsync(id);
        if (pdfBytes.Length == 0) return NotFound("Bon de Livraison not found.");
        
        return File(pdfBytes, "application/pdf", $"BL-{id}.pdf");
    }
}
