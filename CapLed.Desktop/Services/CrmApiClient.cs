using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

public class CrmApiClient : ApiClientBase
{
    public CrmApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public async Task<PagedResult<LeadModel>> GetLeadsAsync(int page, int pageSize, string? search = null, string? statut = null)
    {
        var url = $"v2/leads?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(search)) url += $"&search={search}";
        if (!string.IsNullOrEmpty(statut)) url += $"&statut={statut}";

        return await GetAsync<PagedResult<LeadModel>>(url);
    }

    public async Task UpdateLeadStatusAsync(int leadId, string statut)
    {
        await PutAsync($"v2/leads/{leadId}/statut", new UpdateLeadStatusModel { Statut = statut });
    }
}
