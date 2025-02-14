using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;

namespace Dissertation.View_Models;

public class ChallengeViewModel(IChallengeService challengeService)
{
    public Phase? CurrentPhase { get; private set; }
    public bool ShowPhaseSummary { get; private set; }
    public int Score { get; private set; }
    public int TotalQuestions { get; private set; }
    public List<Project> Projects { get; private set; } = [];
    public Project? SelectedProject { get; private set; }
    public Scenario? CurrentScenario { get; private set; }
    public List<Choice> Choices { get; private set; } = [];
    public bool IsProjectSelected => SelectedProject != null;
    public string ErrorMessage { get; private set; } = string.Empty;
    public int IncorrectAttempts { get; private set; }

    public async Task LoadProjectsAsync()
    {
        Projects = await challengeService.GetProjectsAsync();
    }

    public async Task SelectProjectAsync(int projectId)
    {
        SelectedProject = await challengeService.GetProjectWithScenariosAsync(projectId);

        if (SelectedProject?.Scenarios.Count > 0)
        {
            SelectedProject.Scenarios.Sort((x, y) => x.Id.CompareTo(y.Id));
            CurrentScenario = SelectedProject.Scenarios.First();
            CurrentPhase = CurrentScenario.Phase;
            Score = 0;
            IncorrectAttempts = 0;
            await LoadChoicesAsync();
        }
    }

    public async Task LoadChoicesAsync()
    {
        if (CurrentScenario != null) 
            Choices = await challengeService.GetChoicesForScenarioAsync(CurrentScenario.Id);
    }

    public async Task MakeChoiceAsync(int choiceId)
    {
        var choice = Choices.FirstOrDefault(c => c.Id == choiceId);
        if (choice == null) return;

        if (choice.IsCorrect)
        {
            Score++;
            ErrorMessage = string.Empty;
        }
        else
        {
            IncorrectAttempts += 1;
            ErrorMessage = "Incorrect answer. Please try again.";
        }

        if (choice.NextScenarioId != null)
        {
            CurrentScenario = await challengeService.GetScenarioByIdAsync(choice.NextScenarioId.Value);
            await LoadChoicesAsync();
        }
        else if (IsEndOfPhase())
        {
            ShowPhaseSummary = true;
            if (CurrentPhase != null)
                TotalQuestions = await challengeService.GetScenarioCountForPhaseAsync(CurrentPhase.Value);
        }
    }

    private bool IsEndOfPhase()
    {
        return CurrentScenario != null && Choices.All(c => c.NextScenarioId == null);
    }

    public async Task ProceedToNextPhase()
    {
        ShowPhaseSummary = false;
        Score = 0;

        if (CurrentPhase.HasValue && (int)CurrentPhase < (int)Phase.Feedback)
        {
            CurrentPhase = (Phase)((int)CurrentPhase + 1);
        }
        else
        {
            SelectedProject = null;
            CurrentScenario = null;
            return;
        }

        var nextPhaseScenarios = await challengeService.GetScenariosForPhaseAsync(CurrentPhase.Value);
        if (nextPhaseScenarios.Count > 0)
        {
            CurrentScenario = nextPhaseScenarios.First();
            await LoadChoicesAsync();
        }
        else
        {
            SelectedProject = null;
            CurrentScenario = null;
        }
    }
}