using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using MudBlazor;

namespace Dissertation.View_Models;

public class ChallengeDashboardViewModel(
    IProjectService projectService,
    IUserService userService,
    IDeveloperService developerService,
    IUserStoryService userStoryService,
    ISprintService sprintService,
    IBadgeService badgeService,
    ProjectStateService projectStateService,
    ISnackbar snackbar,
    INavigationService navigationService)
{
    public ProjectManagementViewModel ProjectViewModel { get; } =
        new(projectStateService, projectService, userService, userStoryService, snackbar,
            navigationService);

    public UserProfileManagementViewModel UserProfileManagementViewModel { get; } =
        new(projectStateService, badgeService, projectService);

    public DeveloperManagementViewModel DeveloperViewModel { get; } =
        new(projectStateService, developerService, snackbar);

    public SprintManagementViewModel SprintViewModel { get; } =
        new(projectStateService, sprintService, userStoryService, developerService, badgeService, snackbar,
            navigationService);

    public UserStoryManagementViewModel UserStoryViewModel { get; } = new(projectStateService);
    public ProjectStateService ProjectStateService { get; } = projectStateService;
}