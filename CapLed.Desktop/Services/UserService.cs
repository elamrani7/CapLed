using System.Net.Http;
using CapLed.Desktop.Models;

namespace CapLed.Desktop.Services;

/// <summary>
/// Wraps User API calls.
/// Endpoints: GET/POST/PUT api/v1/Users
/// </summary>
public class UserService : ApiClientBase
{
    public UserService(HttpClient httpClient) : base(httpClient) { }

    /// <summary>GET api/v1/Users</summary>
    public async Task<List<UserModel>> GetAllAsync()
    {
        var result = await GetAsync<List<UserModel>>("api/v1/Users");
        return result ?? new List<UserModel>();
    }

    /// <summary>GET api/v1/Users/{id}</summary>
    public Task<UserModel?> GetByIdAsync(int id)
        => GetAsync<UserModel>($"api/v1/Users/{id}");

    /// <summary>POST api/v1/Users</summary>
    public async Task<bool> CreateAsync(UserCreateModel model)
    {
        var result = await PostAsync<UserCreateModel, UserModel>("api/v1/Users", model);
        return result != null;
    }

    /// <summary>PUT api/v1/Users/{id}</summary>
    public Task<bool> UpdateAsync(int id, UserUpdateModel model)
        => PutAsync($"api/v1/Users/{id}", model);
}
