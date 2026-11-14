using IntegrationTests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging.Abstractions;
using Shared.Exceptions;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Middleware;

namespace IntegrationTests.Middleware;

public class ExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task NotFoundException_Returns404()
    {
        var (status, body) = await InvokeMiddleware(ctx => throw new NotFoundException("Item not found"));

        Assert.Equal((int)HttpStatusCode.NotFound, status);
        Assert.Equal("Not found", body.GetProperty("title").GetString());
        Assert.Equal(404, body.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task UnauthorizedException_Returns401()
    {
        var (status, body) = await InvokeMiddleware(_ => throw new UnauthorizedException("Not authorized"));

        Assert.Equal((int)HttpStatusCode.Unauthorized, status);
        Assert.Equal("Not authorized", body.GetProperty("title").GetString());
        Assert.Equal(401, body.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task ConflictException_Returns409()
    {
        var (status, body) = await InvokeMiddleware(_ => throw new ConflictException("Already exists"));

        Assert.Equal((int)HttpStatusCode.Conflict, status);
        Assert.Equal("Conflict", body.GetProperty("title").GetString());
        Assert.Equal(409, body.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task UnhandledException_Returns500_WithoutDetails()
    {
        var (status, body) = await InvokeMiddleware(_ => throw new InvalidOperationException("boom"));

        Assert.Equal((int)HttpStatusCode.InternalServerError, status);
        Assert.Equal("Internal error", body.GetProperty("title").GetString());
        Assert.Equal(500, body.GetProperty("status").GetInt32());
        // For 500 responses Detail is null, so the property may be absent.
        var hasDetail = body.TryGetProperty("detail", out var detail);
        Assert.False(hasDetail || detail.ValueKind == JsonValueKind.Null);
    }

    private static async Task<(int status, JsonElement body)> InvokeMiddleware(RequestDelegate next)
    {
        var middleware = new ExceptionHandlingMiddleware(
            next,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();

        return (context.Response.StatusCode, JsonDocument.Parse(json).RootElement.Clone());
    }
}
