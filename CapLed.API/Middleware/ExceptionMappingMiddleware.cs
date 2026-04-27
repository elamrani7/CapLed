using System.Net;
using System.Text.Json;
using StockManager.Core.Domain.Exceptions;

namespace StockManager.API.Middleware;

/// <summary>
/// Middleware global de gestion des exceptions.
/// — Mappe les DomainException vers le bon code HTTP
/// — Retourne un format JSON standardisé {code, message}
/// — Ne jamais exposer le StackTrace dans la réponse HTTP
/// — Log les détails techniques uniquement en interne
/// </summary>
public class ExceptionMappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMappingMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionMappingMiddleware(RequestDelegate next, ILogger<ExceptionMappingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        string code;
        string message;
        int statusCode;

        switch (exception)
        {
            case NotFoundException nfe:
                statusCode = (int)HttpStatusCode.NotFound;
                code    = nfe.Code;
                message = nfe.Message;
                _logger.LogWarning("Not found: [{Code}] {Message}", code, message);
                break;

            case ConflictException cfe:
                statusCode = (int)HttpStatusCode.Conflict;
                code    = cfe.Code;
                message = cfe.Message;
                _logger.LogWarning("Conflict: [{Code}] {Message}", code, message);
                break;

            case ForbiddenException ffe:
                statusCode = (int)HttpStatusCode.Forbidden;
                code    = ffe.Code;
                message = ffe.Message;
                _logger.LogWarning("Forbidden: [{Code}] {Message}", code, message);
                break;

            case DomainException dex:
                statusCode = (int)HttpStatusCode.BadRequest;
                code    = dex.Code;
                message = dex.Message;
                _logger.LogWarning("Domain rule violation: [{Code}] {Message}", code, message);
                break;

            default:
                // Erreur inattendue — log complet en interne, message générique vers le client
                statusCode = (int)HttpStatusCode.InternalServerError;
                code    = "INTERNAL_ERROR";
                message = "Une erreur interne est survenue. Veuillez contacter l'administrateur.";
                _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new { code, message, detail = exception.Message };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }
}
