using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using StockManager.API.Middleware;
using StockManager.Core.Domain.Exceptions;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CapLed.Tests.Unit;

/// <summary>
/// Tests unitaires pour ExceptionMappingMiddleware.
/// Vérifie que chaque type d'exception est mappé au bon code HTTP et au bon format JSON.
/// </summary>
public class ExceptionMappingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionMappingMiddleware>> _loggerMock;

    public ExceptionMappingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionMappingMiddleware>>();
    }

    private async Task<(int statusCode, string code, string message)> InvokeMiddlewareWithException(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw exception;
        var middleware = new ExceptionMappingMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var json = JsonDocument.Parse(body).RootElement;

        return (context.Response.StatusCode, json.GetProperty("code").GetString()!, json.GetProperty("message").GetString()!);
    }

    [Fact]
    public async Task NotFoundException_Returns404()
    {
        var (statusCode, code, message) = await InvokeMiddlewareWithException(
            new NotFoundException("ARTICLE_NOT_FOUND", "Article introuvable"));

        statusCode.Should().Be(404);
        code.Should().Be("ARTICLE_NOT_FOUND");
        message.Should().Be("Article introuvable");
    }

    [Fact]
    public async Task ConflictException_Returns409()
    {
        var (statusCode, code, message) = await InvokeMiddlewareWithException(
            new ConflictException("EMAIL_ALREADY_EXISTS", "Cet email existe déjà"));

        statusCode.Should().Be(409);
        code.Should().Be("EMAIL_ALREADY_EXISTS");
        message.Should().Be("Cet email existe déjà");
    }

    [Fact]
    public async Task ForbiddenException_Returns403()
    {
        var (statusCode, code, message) = await InvokeMiddlewareWithException(
            new ForbiddenException("EMAIL_NOT_CONFIRMED", "Confirmez votre email"));

        statusCode.Should().Be(403);
        code.Should().Be("EMAIL_NOT_CONFIRMED");
        message.Should().Be("Confirmez votre email");
    }

    [Fact]
    public async Task DomainException_Returns400()
    {
        var (statusCode, code, message) = await InvokeMiddlewareWithException(
            new DomainException("LEAD_EMPTY_CART", "Le panier est vide"));

        statusCode.Should().Be(400);
        code.Should().Be("LEAD_EMPTY_CART");
        message.Should().Be("Le panier est vide");
    }

    [Fact]
    public async Task UnhandledException_Returns500_WithGenericMessage()
    {
        var (statusCode, code, message) = await InvokeMiddlewareWithException(
            new InvalidOperationException("Internal bug details"));

        statusCode.Should().Be(500);
        code.Should().Be("INTERNAL_ERROR");
        message.Should().NotContain("Internal bug details"); // stack trace must not leak
        message.Should().Contain("erreur interne");
    }

    [Fact]
    public async Task NoException_Returns200()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = ctx => { ctx.Response.StatusCode = 200; return Task.CompletedTask; };
        var middleware = new ExceptionMappingMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
    }
}
