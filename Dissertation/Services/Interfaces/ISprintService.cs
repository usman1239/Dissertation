using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface ISprintService
{
    Task<List<Sprint>> GetSprintsForProjectAsync(int projectId);
    Task SaveSprintAsync(Sprint sprint);
}