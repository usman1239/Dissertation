using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IChallengeService
{
    public Task<List<Project>> GetProjectsAsync();
    public Task<Project?> GetProjectWithScenariosAsync(int projectId);
    public Task<List<Choice>> GetChoicesForScenarioAsync(int scenarioId);
    public Task<Scenario?> GetScenarioByIdAsync(int scenarioId);
    public Task<int> GetScenarioCountForPhaseAsync(Phase phase);
    public Task<List<Scenario>> GetScenariosForPhaseAsync(Phase phase);
}