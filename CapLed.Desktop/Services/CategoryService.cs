using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

/// <summary>
/// Wraps all Category API calls.
/// Endpoints: GET/POST/PUT/DELETE api/v1/Category
/// </summary>
public class CategoryService : ApiClientBase
{
    public CategoryService(HttpClient httpClient) : base(httpClient) { }

    /// <summary>GET api/v1/Category — returns all categories.</summary>
    public async Task<List<CategoryModel>> GetAllAsync()
    {
        var result = await GetAsync<List<CategoryModel>>("api/v1/Category");
        return result ?? new List<CategoryModel>();
    }

    /// <summary>GET api/v1/Category/{id}</summary>
    public Task<CategoryModel?> GetByIdAsync(int id)
        => GetAsync<CategoryModel>($"api/v1/Category/{id}");

    /// <summary>POST api/v1/Category</summary>
    public async Task<bool> CreateAsync(CategoryModel model)
    {
        var result = await PostAsync<CategoryModel, CategoryModel>("api/v1/Category", model);
        return result != null;
    }

    /// <summary>PUT api/v1/Category/{id}</summary>
    public Task<bool> UpdateAsync(int id, CategoryModel model)
        => PutAsync($"api/v1/Category/{id}", model);

    /// <summary>DELETE api/v1/Category/{id}</summary>
    public Task<bool> DeleteAsync(int id)
        => DeleteAsync($"api/v1/Category/{id}");
}
