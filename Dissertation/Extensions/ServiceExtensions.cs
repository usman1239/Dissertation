using Dissertation.Components.Pages;
using Dissertation.Components.Pages.Learn;
using Dissertation.Data;
using Dissertation.Models.Interfaces;
using Dissertation.Services.Interfaces;
using Dissertation.Services;
using Dissertation.View_Models;

namespace Dissertation.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Register custom services here
            services.AddScoped<IQuestionProvider, QuestionProvider>();

            services.AddScoped<IChallengeService, ChallengeService>();

            services.AddScoped<ChallengeViewModel>();

            services.AddSingleton<ITopicContentProvider, TopicContentProvider>();

            return services;
        }
    }
}
