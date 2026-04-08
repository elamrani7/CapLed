using System.Threading.Tasks;

namespace StockManager.Core.Application.Interfaces.Services;

public interface IDocumentPdfService
{
    Task<byte[]> GenerateDevisPdfAsync(int leadId);
    Task<byte[]> GenerateBonCommandePdfAsync(int bcId);
    Task<byte[]> GenerateBonLivraisonPdfAsync(int blId);
}
