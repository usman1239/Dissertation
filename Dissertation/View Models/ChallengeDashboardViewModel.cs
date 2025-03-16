using Dissertation.Services;
using Dissertation.Services.Interfaces;

namespace Dissertation.View_Models;

public class ChallengeDashboardViewModel(
    IProjectService projectService,
    IUserService userService,
    IDeveloperService developerService,
    IUserStoryService userStoryService,
    ISprintService sprintService,
    ProjectStateService projectStateService)
{
    public ProjectManagementViewModel ProjectViewModel { get; } =
        new(projectStateService, projectService, userService, userStoryService);

    public DeveloperManagementViewModel DeveloperViewModel { get; } = new(projectStateService);

    public SprintManagementViewModel SprintViewModel { get; } =
        new(projectStateService, sprintService, userStoryService, developerService);

    public UserStoryManagementViewModel UserStoryViewModel { get; } = new(projectStateService);
    public ProjectStateService ProjectStateService { get; } = projectStateService;
}