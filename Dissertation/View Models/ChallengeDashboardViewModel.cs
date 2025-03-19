using Dissertation.Services;
using Dissertation.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dissertation.View_Models;

public class ChallengeDashboardViewModel(
    IProjectService projectService,
    IUserService userService,
    IDeveloperService developerService,
    IUserStoryService userStoryService,
    ISprintService sprintService,
    ProjectStateService projectStateService,
    ISnackbar snackbar,
    NavigationManager navigationManager)
{
    public ProjectManagementViewModel ProjectViewModel { get; } =
        new(projectStateService, projectService, userService, userStoryService, snackbar, navigationManager);

    public DeveloperManagementViewModel DeveloperViewModel { get; } = new(projectStateService, developerService, snackbar);

    public SprintManagementViewModel SprintViewModel { get; } =
        new(projectStateService, sprintService, userStoryService, developerService, snackbar, navigationManager);

    public UserStoryManagementViewModel UserStoryViewModel { get; } = new(projectStateService);
    public ProjectStateService ProjectStateService { get; } = projectStateService;
}