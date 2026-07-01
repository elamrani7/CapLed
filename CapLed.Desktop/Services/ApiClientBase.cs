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

    /// <summary>Vérifie si la réponse est un succès, sinon lève une ApiException propre.</summary>
    protected async Task EnsureSuccessAsync(HttpResponseMessage response, string context = "API Call")
    {
        if (response.IsSuccessStatusCode) return;
        var error = await HandleErrorResponse(response, context);
        throw new ApiException(error);
    }

    protected void EnsureAuthHeader()
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
            try
            {
                return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ApiException($"Erreur lecture réponse JSON de {url}: {ex.Message}");
            }
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return default;

        var error = await HandleErrorResponse(response, $"GET {url}");
        throw new ApiException(error);
    }

    /// <summary>
    /// HTTP GET silencieux — utilisé pour les appels automatiques en arrière-plan.
    /// Retourne null sans lever d'exception pour 401, 403, 404, 405.
    /// Les erreurs sont loggées dans la console de débogage uniquement.
    /// </summary>
    protected async Task<T?> GetAsyncSilent<T>(string url)
    {
        try
        {
            EnsureAuthHeader();
            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                try { return await response.Content.ReadFromJsonAsync<T>(JsonOptions); }
                catch (JsonException ex) { System.Diagnostics.Debug.WriteLine($"[SILENT] JSON parse error {url}: {ex.Message}"); }
                return default;
            }

            // Ces statuts sont attendus en mode silencieux → pas de popup
            var silentStatuses = new[]
            {
                System.Net.HttpStatusCode.NotFound,
                System.Net.HttpStatusCode.Unauthorized,
                System.Net.HttpStatusCode.Forbidden,
                System.Net.HttpStatusCode.MethodNotAllowed
            };
            if (Array.IndexOf(silentStatuses, response.StatusCode) >= 0)
            {
                System.Diagnostics.Debug.WriteLine($"[SILENT] {(int)response.StatusCode} on GET {url} — ignored");
                return default;
            }

            // Autres erreurs : log uniquement, pas de throw
            System.Diagnostics.Debug.WriteLine($"[SILENT] Unexpected {(int)response.StatusCode} on GET {url}");
            return default;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SILENT] Network error on GET {url}: {ex.Message}");
            return default;
        }
    }

    // ─── POST ────────────────────────────────────────────────────────────────

    /// <summary>HTTP POST with body → deserialise response to TResponse.</summary>
    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body)
    {
        EnsureAuthHeader();
        var response = await Http.PostAsJsonAsync(url, body, JsonOptions);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ApiException($"Erreur lecture réponse JSON de {url}: {ex.Message}");
            }
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

    /// <summary>HTTP POST pour l'upload de fichiers multiples (multipart/form-data).</summary>
    protected async Task<TResponse> PostMultipartAsync<TResponse>(string url, IEnumerable<string> filePaths, string fileParamName = "files")
    {
        using var content = new MultipartFormDataContent();
        
        foreach (var filePath in filePaths)
        {
            var fileContent = new ByteArrayContent(await System.IO.File.ReadAllBytesAsync(filePath));
            var mimeType = "image/jpeg";
            var ext = System.IO.Path.GetExtension(filePath).ToLower();
            if (ext == ".png") mimeType = "image/png";
            else if (ext == ".gif") mimeType = "image/gif";
            else if (ext == ".webp") mimeType = "image/webp";

            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
            content.Add(fileContent, fileParamName, System.IO.Path.GetFileName(filePath));
        }

        var response = await Http.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, JsonOptions)!;
        }

        var error = await HandleErrorResponse(response, $"POST Multipart {url}");
        throw new ApiException(error);
    }

    protected async Task<string> HandleErrorResponse(HttpResponseMessage response, string context)
    {
        string rawContent = string.Empty;
        try
        {
            rawContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"API ERROR [{response.StatusCode}] {context}: {rawContent}");
        }
        catch { /* ignored */ }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return "Session expirée ou non autorisée. Veuillez vous reconnecter.";

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            return "Accès refusé. Vous n'avez pas les permissions nécessaires pour cette action.";

        // ── Parse the standardized backend error format: { code, message } ──
        if (!string.IsNullOrWhiteSpace(rawContent))
        {
            try
            {
                using var doc = JsonDocument.Parse(rawContent);
                var root = doc.RootElement;

                // Standard format: { "code": "...", "message": "..." }
                if (root.TryGetProperty("message", out var msgProp) && 
                    !string.IsNullOrWhiteSpace(msgProp.GetString()))
                {
                    return msgProp.GetString()!;
                }

                // Legacy ASP.NET validation format: { "errors": { "Field": ["msg"] } }
                if (root.TryGetProperty("errors", out var errors))
                {
                    var messages = new List<string>();
                    foreach (var field in errors.EnumerateObject())
                    {
                        if (field.Value.ValueKind == JsonValueKind.Array)
                            foreach (var msg in field.Value.EnumerateArray())
                                if (msg.GetString() is { } s) messages.Add(s);
                    }
                    if (messages.Count > 0) return string.Join(" ", messages);
                }

                // Legacy format: { "error": "..." }
                if (root.TryGetProperty("error", out var errorProp) &&
                    !string.IsNullOrWhiteSpace(errorProp.GetString()))
                {
                    return errorProp.GetString()!;
                }

                // Short raw content fallback
                if (rawContent.Length < 250 && !rawContent.TrimStart().StartsWith("{"))
                    return rawContent;
            }
            catch { /* Not valid JSON — fall through */ }
        }

        // HTTP status fallbacks
        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound           => "La ressource demandée est introuvable.",
            System.Net.HttpStatusCode.Conflict           => "Un conflit a été détecté. Vérifiez les données saisies.",
            System.Net.HttpStatusCode.BadRequest         => "La demande est invalide. Vérifiez les informations saisies.",
            System.Net.HttpStatusCode.InternalServerError => "Une erreur interne est survenue. Contactez l'administrateur.",
            _ => $"{context} a échoué ({(int)response.StatusCode})."
        };
    }

    // ─── PUT ─────────────────────────────────────────────────────────────────

    /// <summary>HTTP PUT — returns true on success (204 No Content).</summary>
    protected async Task<bool> PutAsync<TRequest>(string url, TRequest body)
    {
        EnsureAuthHeader();
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
