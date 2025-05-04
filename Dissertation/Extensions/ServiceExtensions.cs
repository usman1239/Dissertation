using Dissertation.Components.Pages.Learn;
using Dissertation.Models.Challenge;
using Dissertation.Models.Interfaces;
using Dissertation.Services;
using Dissertation.Services.Interfaces;
using Dissertation.View_Models;

namespace Dissertation.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<ISprintService, SprintService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserStoryService, UserStoryService>();
        services.AddScoped<IDeveloperService, DeveloperService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<IBadgeService, BadgeService>();
        services.AddScoped<IProjectAiService, ProjectAiService>();

        services.AddSingleton<IDailyChallengeService, DailyChallengeService>();

        services.AddSingleton<ProjectStateService>();
        services.AddScoped<ProjectManagementViewModel>();
        services.AddScoped<DeveloperManagementViewModel>();
        services.AddScoped<SprintManagementViewModel>();
        services.AddScoped<UserStoryManagementViewModel>();
        services.AddScoped<UserProfileManagementViewModel>();

        services.AddScoped<ChallengeDashboardViewModel>();


        services.AddScoped<IQuestionProvider, QuestionProvider>();
        services.AddSingleton<ITopicContentProvider, TopicContentProvider>();

        return services;
    }
}