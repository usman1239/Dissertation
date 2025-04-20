using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetAvailableProjectsAsync();
    Task<List<ProjectInstance>> LoadProjectsWithSavedProgressAsync(string? userId);
    Task<ProjectInstance?> GetProjectInstanceAsync(int projectId, string? userId);
    Task SaveNewProjectInstance(ProjectInstance projectInstance);
    Task<bool> DeleteSavedProjectInstanceAsync(int projectId, string? userId);
    Task<List<ProjectInstance>> GetUserProjectsAsync(string userId);
}