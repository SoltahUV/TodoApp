using DataAccess.Configuration;
using DataAccess.Data.NoteRepo;
using DataAccess.Data.TaskRepo;
using DataAccess.Data.UserRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using AppDbContext = DataAccess.Data.AppDbContext;

namespace DataAccess
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection servicesCollection, IConfiguration configuration)
        {
            var databaseConfiguration = configuration
                .GetSection(DatabaseConfiguration.SectionName)
                .Get<DatabaseConfiguration>()
                ?? throw new InvalidOperationException(
                    $"Configuration section '{DatabaseConfiguration.SectionName}' is missing.");

            DatabaseConfigurationValidator.Validate(databaseConfiguration);

            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = databaseConfiguration.Host,
                Port = databaseConfiguration.Port,
                Database = databaseConfiguration.Database,
                Username = databaseConfiguration.Username,
                Password = databaseConfiguration.Password
            }.ConnectionString;

            servicesCollection.AddScoped<INoteRepository, NoteRepository>();
            servicesCollection.AddScoped<ITaskRepository, TaskRepository>();
            servicesCollection.AddScoped<IUserRepository, UserRepository>();
            servicesCollection.AddDbContext<AppDbContext>(x =>
            {
                x.UseNpgsql(connectionString);
            });
            return servicesCollection;
        }
        public static async Task CheckDatabaseConnectionAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
            
            logger.LogInformation("Checking database connection... ");

            try
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.ExecuteSqlRawAsync("SELECT 1");
                logger.LogInformation("Database connection successful");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database connection failed");
            }
        }
    }
}
