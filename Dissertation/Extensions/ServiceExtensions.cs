using Dissertation.Components.Pages;
using Dissertation.Components.Pages.Learn;
using Dissertation.Models.Interfaces;

namespace Dissertation.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Register custom services here
            services.AddScoped<IQuestionProvider, QuestionProvider>();


            services.AddSingleton<ITopicContentProvider, TopicContentProvider>();

            return services;
        }
    }
}
