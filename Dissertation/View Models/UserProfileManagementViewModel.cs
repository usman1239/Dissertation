using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;

namespace Dissertation.View_Models;

public class UserProfileManagementViewModel(
    ProjectStateService projectStateService,
    IBadgeService badgeService,
    IProjectService projectService)
{
    public List<UserBadge> UserBadges { get; set; } = [];
    public List<ProjectInstance> ActiveProjects { get; private set; } = new();

    public async Task GetUserBadges()
    {
        UserBadges = await badgeService.GetUserBadgesAsync(projectStateService.UserId!);
    }

    public async Task GetActiveProjects()
    {
        ActiveProjects = await projectService.GetUserProjectsAsync(projectStateService.UserId!);
    }

}