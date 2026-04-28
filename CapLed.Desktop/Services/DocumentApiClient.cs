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
        var res = await GetAsyncSilent<PagedResult<BonCommandeModel>>($"api/Orders/bc");
        return res ?? new PagedResult<BonCommandeModel>();
    }

    public async Task<PagedResult<BonLivraisonModel>> GetBonsLivraisonAsync(int page, int pageSize)
    {
        // Appel silencieux — l'endpoint BL peut être absent (405) ou vide : jamais affiché comme erreur.
        var res = await GetAsyncSilent<PagedResult<BonLivraisonModel>>($"api/Orders/bl");
        return res ?? new PagedResult<BonLivraisonModel>();
    }

    public async Task DeleteBonCommandeAsync(int bcId)
    {
        await DeleteAsync($"api/Orders/bc/{bcId}");
    }

    public async Task<BonLivraisonModel> CreateBonLivraisonFromBcAsync(int bcId, int depotId)
    {
        var response = await Http.PostAsync($"api/Orders/bl/from-bc/{bcId}?depotId={depotId}", null);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<BonLivraisonModel>() ?? new BonLivraisonModel();
    }

    public async Task<byte[]> DownloadDevisPdfAsync(int leadId)
    {
        return await Http.GetByteArrayAsync($"api/v2/documents/devis/{leadId}/pdf");
    }

    public async Task<byte[]> DownloadBcPdfAsync(int bcId)
    {
        return await Http.GetByteArrayAsync($"api/v2/documents/bc/{bcId}/pdf");
    }

    public async Task<byte[]> DownloadBlPdfAsync(int blId)
    {
        return await Http.GetByteArrayAsync($"api/v2/documents/bl/{blId}/pdf");
    }
}
