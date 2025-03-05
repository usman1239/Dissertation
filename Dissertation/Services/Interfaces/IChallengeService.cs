using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IChallengeService
{
    Task<List<Project>> GetAvailableProjectsAsync();
    Task<List<UserProjectInstance>> LoadProjectsWithSavedProgressAsync(string? userId);
    Task<UserProjectInstance?> GetProjectInstanceAsync(int projectId, string? userId);
    Task AddUserProjectInstanceAsync(UserProjectInstance userProjectInstance);
    Task AddSprintsToDbAsync(List<Sprint> sprints);
    Task AddDevelopersToDbAsync(List<Developer> developers);
    Task AddOrUpdateUserStoriesAndTasksAsync(List<UserStory> userStories);
    Task<bool> DeleteSavedProjectInstanceAsync(int projectId, string? userId);
}