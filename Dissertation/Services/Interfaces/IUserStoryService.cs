using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IUserStoryService
{
    Task SaveUserStoryInstancesAsync(List<UserStoryInstance> instances);
    Task<List<UserStory>> GetInitialUserStoriesForProject(int projectId);
    Task AttachProjectAndUserStories(ProjectInstance projectInstance);
    Task<List<UserStoryInstance>> GetUserStoryInstancesAsync(int projectInstanceId);
}