using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IUserStoryService
{
    Task SaveUserStoryInstancesAsync(List<UserStoryInstance> instances);
    Task<List<UserStory>> GetInitialUserStoriesForProject(int projectId);
    Task<List<UserStoryInstance>> GetUserStoryInstancesForProjectAsync(int projectInstanceId);
    Task AttachProjectAndUserStories(ProjectInstance projectInstance);
    Task TriggerRandomUserStoryEventAsync(int projectInstanceId);
    Task<UserStoryInstance> CreateAndAssignBugToProjectAsync(ProjectInstance projectInstanceId);
}