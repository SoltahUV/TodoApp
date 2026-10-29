using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;

namespace WebAPI.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
        }
        catch (Exception ex) when (!ctx.Response.HasStarted)
        {
            var (status, title) = ex switch
            {
                NotFoundException        => (404, "Not found"),
                UnauthorizedException => (401, "Not authorized"),
                ConflictException        => (409, "Conflict"),
                _                        => (500, "Internal error")
            };

            if (status == 500)
                logger.LogError(ex, "Unhandled exception");

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? null : ex.Message
            });
        }
    }
}