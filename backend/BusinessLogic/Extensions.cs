using BusinessLogic.Services.NoteService;
using BusinessLogic.Services.TaskService;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection servicesCollection)
        {
            servicesCollection.AddScoped<INoteService, NoteService>();
            servicesCollection.AddScoped<ITaskService, TaskService>();
            return servicesCollection;
        }
    }
}
