using Dissertation.Models.Challenge;

namespace Dissertation.View_Models;

public class UserStoryManagementViewModel(ProjectStateService projectStateService)
{
    public void AssignDeveloperToStory(UserStoryInstance story, int? developerId)
    {
        if (developerId is null or 0)
        {
            story.DeveloperAssignedId = null;
            story.DeveloperAssigned = null;
            return;
        }

        var developer = projectStateService.Team.FirstOrDefault(d => d.Id == developerId);
        if (developer == null) return;
        story.DeveloperAssignedId = developer.Id;
        story.DeveloperAssigned = developer;
    }

    public bool IsDeveloperUnavailable(int developerId)
    {
        var dev = projectStateService.Team.FirstOrDefault(d => d.Id == developerId);
        return dev is { IsSick: true } or { IsPermanentlyAbsent: true };
    }
}