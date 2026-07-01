using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

public class FamilleService : ApiClientBase
{
    public FamilleService(HttpClient httpClient) : base(httpClient) { }

    public async Task<List<FamilleModel>> GetAllAsync()
    {
        var result = await GetAsync<List<FamilleModel>>("api/v1/Famille");
        return result ?? new List<FamilleModel>();
    }

    public Task<FamilleModel?> GetByIdAsync(int id)
        => GetAsync<FamilleModel>($"api/v1/Famille/{id}");

    public async Task<bool> CreateAsync(FamilleModel model)
    {
        var result = await PostAsync<FamilleModel, FamilleModel>("api/v1/Famille", model);
        return result != null;
    }

    public Task<bool> UpdateAsync(int id, FamilleModel model)
        => PutAsync($"api/v1/Famille/{id}", model);

    public Task<bool> DeleteAsync(int id)
        => DeleteAsync($"api/v1/Famille/{id}");
}
