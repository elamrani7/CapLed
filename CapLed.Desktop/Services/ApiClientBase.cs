using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CapLed.Desktop.Services;

/// <summary>
/// Base class for all API service classes.
/// Wraps HttpClient with typed helpers for GET / POST / PUT / DELETE.
/// All errors are caught and re-thrown as ApiException for uniform handling.
/// </summary>
public abstract class ApiClientBase
{
    protected readonly HttpClient Http;

    // ─── JSON options (match ASP.NET Core defaults: camelCase) ───────────────
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    protected ApiClientBase(HttpClient httpClient)
    {
        Http = httpClient;
    }

    private void EnsureAuthHeader()
    {
        var token = CapLed.Desktop.Core.AppSession.Current.JwtToken;
        if (!string.IsNullOrEmpty(token))
        {
            Http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    // ─── GET ─────────────────────────────────────────────────────────────────

    /// <summary>HTTP GET → deserialise to T. Returns null on 404.</summary>
    protected async Task<T?> GetAsync<T>(string url)
    {
        EnsureAuthHeader();
        var response = await Http.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }

        var error = await HandleErrorResponse(response, $"GET {url}");
        throw new ApiException(error);
    }

    // ─── POST ────────────────────────────────────────────────────────────────

    /// <summary>HTTP POST with body → deserialise response to TResponse.</summary>
    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body)
    {
        EnsureAuthHeader();
        var response = await Http.PostAsJsonAsync(url, body, JsonOptions);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
        }

        var error = await HandleErrorResponse(response, $"POST {url}");
        throw new ApiException(error);
    }

    /// <summary>HTTP POST with body — no response body expected.</summary>
    protected async Task<bool> PostAsync<TRequest>(string url, TRequest body)
    {
        EnsureAuthHeader();
        var response = await Http.PostAsJsonAsync(url, body, JsonOptions);

        if (response.IsSuccessStatusCode) return true;

        var error = await HandleErrorResponse(response, $"POST {url}");
        throw new ApiException(error);
    }

    protected async Task<string> HandleErrorResponse(HttpResponseMessage response, string context)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return "Session expirée ou non autorisée. Veuillez vous reconnecter.";
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            return "Accès refusé. Vous n'avez pas les permissions nécessaires pour cette action.";
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            try
            {
                // Try to parse ASP.NET Core Validation Problem JSON
                var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (problem.TryGetProperty("errors", out var errors))
                {
                    // Check for both ASP.NET Core default errors and our custom { errors: { Error: [...] } }
                    return $"{context} validation failed: {errors.ToString()}";
                }
                if (problem.TryGetProperty("detail", out var detail))
                {
                    return $"{context} failed: {detail.GetString()}";
                }
                if (problem.TryGetProperty("error", out var error))
                {
                    return $"{context} failed: {error.GetString()}";
                }
            }
            catch { /* fallback to generic error */ }
        }

        return $"{context} failed: {response.ReasonPhrase} ({(int)response.StatusCode})";
    }

    // ─── PUT ─────────────────────────────────────────────────────────────────

    /// <summary>HTTP PUT — returns true on success (204 No Content).</summary>
    protected async Task<bool> PutAsync<TRequest>(string url, TRequest body)
    {
        EnsureAuthHeader();
        System.Diagnostics.Debug.WriteLine($"API PUT: {url}");
        var response = await Http.PutAsJsonAsync(url, body, JsonOptions);

        if (response.IsSuccessStatusCode) return true;

        var error = await HandleErrorResponse(response, $"PUT {url}");
        throw new ApiException(error);
    }

    // ─── DELETE ──────────────────────────────────────────────────────────────

    /// <summary>HTTP DELETE — returns true on success.</summary>
    protected async Task<bool> DeleteAsync(string url)
    {
        EnsureAuthHeader();
        var response = await Http.DeleteAsync(url);

        if (response.IsSuccessStatusCode) return true;

        var error = await HandleErrorResponse(response, $"DELETE {url}");
        throw new ApiException(error);
    }
}

/// <summary>
/// Thrown by any service class when the API returns an error.
/// ViewModels catch this and set ErrorMessage.
/// </summary>
public class ApiException : Exception
{
    public ApiException(string message, Exception? inner = null)
        : base(message, inner) { }
}
