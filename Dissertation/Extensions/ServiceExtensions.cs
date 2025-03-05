using Dissertation.Components.Pages.Learn;
using Dissertation.Models.Interfaces;
using Dissertation.Services;
using Dissertation.Services.Interfaces;
using Dissertation.View_Models;
using MudBlazor;
using MudBlazor.Services;

namespace Dissertation.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IQuestionProvider, QuestionProvider>();

        services.AddScoped<IChallengeService, ChallengeService>();

        services.AddScoped<IUserProgressService, UserProgressService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ChallengeDashboardViewModel>();

        services.AddSingleton<ITopicContentProvider, TopicContentProvider>();
        
        return services;
    }
}