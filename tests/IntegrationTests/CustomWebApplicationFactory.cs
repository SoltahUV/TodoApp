using Auth.Services;
using BusinessLogic.Services.NoteService;
using BusinessLogic.Services.TaskService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            ReplaceService<ITaskService>(services, _ => new FakeTaskService());
            ReplaceService<INoteService>(services, _ => new FakeNoteService());
            ReplaceService<IAuthService>(services, _ => new FakeAuthService());

            services.AddAuthentication(TestAuth.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuth.Scheme, _ => { });
        });
    }

    private static void ReplaceService<T>(IServiceCollection services, Func<IServiceProvider, T> implementation)
        where T : class
    {
        services.RemoveAll<T>();
        services.AddSingleton(implementation);
    }
}
