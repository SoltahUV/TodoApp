using Auth;
using BusinessLogic;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessLogic();
builder.Services.AddAuth(builder.Configuration);

var forwardedHeadersConfiguration = builder.Configuration.GetSection("ForwardedHeaders");
var knownProxyValues = forwardedHeadersConfiguration.GetSection("KnownProxies").Get<string[]>()
    ?? ["127.0.0.1", "::1"];
var knownNetworkValues = forwardedHeadersConfiguration.GetSection("KnownNetworks").Get<string[]>()
    ?? ["172.16.0.0/12", "fd00::/8"];

var knownProxies = new List<IPAddress>();
foreach (var value in knownProxyValues)
{
    if (!IPAddress.TryParse(value, out var proxy))
        throw new InvalidOperationException(
            $"ForwardedHeaders:KnownProxies contains an invalid IP address: '{value}'.");
    knownProxies.Add(proxy);
}

var knownNetworks = new List<System.Net.IPNetwork>();
foreach (var value in knownNetworkValues)
{
    if (!System.Net.IPNetwork.TryParse(value, out var network))
        throw new InvalidOperationException(
            $"ForwardedHeaders:KnownNetworks contains an invalid CIDR: '{value}'.");
    knownNetworks.Add(network);
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    options.KnownProxies.Clear();
    foreach (var proxy in knownProxies)
        options.KnownProxies.Add(proxy);

    options.KnownIPNetworks.Clear();
    foreach (var network in knownNetworks)
        options.KnownIPNetworks.Add(network);
});

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy("auth", ctx =>
    {
        var address = ctx.Connection.RemoteIpAddress;
        if (address is { IsIPv4MappedToIPv6: true })
            address = address.MapToIPv4();

        return RateLimitPartition.GetFixedWindowLimiter(
            address?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };
        return Task.CompletedTask;
    });
    options.AddOperationTransformer((operation, context, ct) =>
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<IAuthorizeData>().Any() && !metadata.OfType<IAllowAnonymous>().Any())
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });
        }
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseForwardedHeaders();
app.UseRateLimiter();

await app.Services.CheckDatabaseConnectionAsync();

if (args.Contains("--migrate"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<DataAccess.Data.AppDbContext>().Database.MigrateAsync();
    return;
}

if (app.Environment.IsDevelopment()
    && app.Configuration.GetValue<bool>("AutoMigrate"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<DataAccess.Data.AppDbContext>().Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
