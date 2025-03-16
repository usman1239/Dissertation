using Dissertation.Models.Challenge;
using Dissertation.Services;

namespace Dissertation.View_Models;

public class UserStoryManagementViewModel(ProjectStateService projectStateService)
{
    public void AssignDeveloperToStory(UserStoryInstance story, int? developerId)
    {
        var developer = projectStateService.Team.FirstOrDefault(d => d.Id == developerId);
        if (developer == null) return;
        story.DeveloperAssignedId = developer.Id;
        story.DeveloperAssigned = developer;
    }
}