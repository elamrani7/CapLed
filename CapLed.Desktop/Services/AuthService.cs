using CapLed.Desktop.Models;
using CapLed.Desktop.Core;
using System.Net.Http;
using System.Net.Http.Json;

namespace CapLed.Desktop.Services;

public class AuthService : ApiClientBase
{
    public AuthService(HttpClient httpClient) : base(httpClient) { }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await PostAsync<LoginRequest, LoginResponse>("api/v1/auth/login", request);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                var session = AppSession.Current;
                session.JwtToken = response.Token;
                session.Email = response.Email;
                session.FullName = response.FullName;
                session.Role = response.Role;
                session.UserName = response.Email; // Using email as username for now

                return true;
            }
            return false;
        }
        catch (ApiException)
        {
            // Re-throw so LoginViewModel can display the real API error message
            throw;
        }
        catch (Exception ex)
        {
            // Wrap connectivity/network errors with a clear description
            throw new ApiException($"Erreur de connexion au serveur : {ex.Message}", ex);
        }
    }

    public void Logout()
    {
        AppSession.Current.Clear();
    }
}
