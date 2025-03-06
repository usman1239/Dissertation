using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IChallengeService
{
    Task<List<Project>> GetAvailableProjectsAsync();
    Task<List<ProjectInstance>> LoadProjectsWithSavedProgressAsync(string? userId);
    Task<ProjectInstance?> GetProjectInstanceAsync(int projectId, string? userId);
    Task AddUserProjectInstanceAsync(ProjectInstance projectInstance);
    Task AddSprintsToDbAsync(List<Sprint> sprints);
    Task AddDevelopersToDbAsync(List<Developer> developers);
    Task AddOrUpdateUserStoriesAsync(List<UserStory> userStories);
    Task<bool> DeleteSavedProjectInstanceAsync(int projectId, string? userId);
}