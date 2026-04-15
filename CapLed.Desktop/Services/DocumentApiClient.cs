using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

public class DocumentApiClient : ApiClientBase
{
    public DocumentApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public async Task<PagedResult<BonCommandeModel>> GetBonsCommandeAsync(int page, int pageSize)
    {
        var res = await GetAsync<PagedResult<BonCommandeModel>>($"v2/bons-commande?page={page}&pageSize={pageSize}");
        return res ?? new PagedResult<BonCommandeModel>();
    }

    public async Task<PagedResult<BonLivraisonModel>> GetBonsLivraisonAsync(int page, int pageSize)
    {
        var res = await GetAsync<PagedResult<BonLivraisonModel>>($"v2/bons-livraison?page={page}&pageSize={pageSize}");
        return res ?? new PagedResult<BonLivraisonModel>();
    }

    public async Task<byte[]> DownloadDevisPdfAsync(int leadId)
    {
        return await Http.GetByteArrayAsync($"api/v2/documents/devis/{leadId}/pdf");
    }

    public async Task<byte[]> DownloadBcPdfAsync(int bcId)
    {
        return await Http.GetByteArrayAsync($"api/v2/documents/bons-commande/{bcId}/pdf");
    }

    public async Task<byte[]> DownloadBlPdfAsync(int blId)
    {
        return await Http.GetByteArrayAsync($"api/v2/documents/bons-livraison/{blId}/pdf");
    }
}
