//using System.Security.Claims;
//using Dissertation.Models.Challenge;
//using Dissertation.Services.Interfaces;
//using Microsoft.AspNetCore.Components.Authorization;

//namespace Dissertation.View_Models;

//public class ChallengeViewModel(IChallengeService challengeService, 
//    IUserProgressService userProgressService,
//    AuthenticationStateProvider authenticationStateProvider)
//{
//    public Phase? CurrentPhase { get; private set; }
//    public bool ShowPhaseSummary { get; private set; }
//    public int Score { get; private set; }
//    public int TotalQuestions { get; private set; }
//    public List<Project> Projects { get; private set; } = [];
//    public Project? SelectedProject { get; private set; }
//    public Scenario? CurrentScenario { get; private set; }
//    public List<Choice> Choices { get; private set; } = [];
//    public bool IsProjectSelected => SelectedProject != null;
//    public string ErrorMessage { get; private set; } = string.Empty;
//    public int IncorrectAttempts { get; private set; }

//    private async Task<string?> GetUserIdAsync()
//    {
//        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
//        var user = authState.User;

//        return user.Identity?.IsAuthenticated == true ? 
//            user.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
//    }

//    public async Task LoadProjectsAsync()
//    {
//        Projects = await challengeService.GetProjectsAsync();
//        await LoadUserProgressAsync();
//    }

//    private async Task LoadUserProgressAsync()
//    {
//        var userId = await GetUserIdAsync();
//        if (userId == null) return;

//        var progress = await userProgressService.GetUserProgressAsync(userId);

//        if (progress != null)
//        {
//            SelectedProject = await challengeService.GetProjectWithScenariosAsync(progress.ProjectId);
//            CurrentScenario = await challengeService.GetScenarioByIdAsync(progress.CurrentScenarioId);
//            Score = progress.Score;
//            CurrentPhase = CurrentScenario?.Phase;
//            await LoadChoicesAsync();

//        }

//    }

//    public async Task SelectProjectAsync(int projectId)
//    {
//        SelectedProject = await challengeService.GetProjectWithScenariosAsync(projectId);

//        if (SelectedProject?.Scenarios.Count > 0)
//        {
//            SelectedProject.Scenarios.Sort((x, y) => x.Id.CompareTo(y.Id));
//            CurrentScenario = SelectedProject.Scenarios.First();
//            CurrentPhase = CurrentScenario.Phase;
//            Score = 0;
//            IncorrectAttempts = 0;
//            await LoadChoicesAsync();
//        }
//    }

//    public async Task LoadChoicesAsync()
//    {
//        if (CurrentScenario != null) 
//            Choices = await challengeService.GetChoicesForScenarioAsync(CurrentScenario.Id);
//    }

//    public async Task MakeChoiceAsync(int choiceId)
//    {
//        var userId = await GetUserIdAsync();
//        if (userId == null) return;

//        var choice = Choices.FirstOrDefault(c => c.Id == choiceId);
//        if (choice == null) return;

//        if (choice.IsCorrect)
//        {
//            Score++;
//            ErrorMessage = string.Empty;
//        }
//        else
//        {
//            IncorrectAttempts += 1;
//            ErrorMessage = "Incorrect answer. Please try again.";
//        }

//        if (choice.NextScenarioId != null)
//        {
//            CurrentScenario = await challengeService.GetScenarioByIdAsync(choice.NextScenarioId.Value);
//            await LoadChoicesAsync();
//        }
//        else if (IsEndOfPhase())
//        {
//            ShowPhaseSummary = true;
//            if (CurrentPhase != null)
//                TotalQuestions = await challengeService.GetScenarioCountForPhaseAsync(CurrentPhase.Value);
//        }

//        await SaveUserProgressAsync(userId);
//    }

//    private bool IsEndOfPhase()
//    {
//        return CurrentScenario != null && Choices.All(c => c.NextScenarioId == null);
//    }

//    public async Task ProceedToNextPhase()
//    {
//        var userId = await GetUserIdAsync();
//        if (userId == null) return;

//        ShowPhaseSummary = false;
//        Score = 0;

//        if (CurrentPhase.HasValue && (int)CurrentPhase < (int)Phase.Feedback)
//        {
//            CurrentPhase = (Phase)((int)CurrentPhase + 1);
//        }
//        else
//        {
//            SelectedProject = null;
//            CurrentScenario = null;
//            return;
//        }

//        var nextPhaseScenarios = await challengeService.GetScenariosForPhaseAsync(CurrentPhase.Value);
//        if (nextPhaseScenarios.Count > 0)
//        {
//            CurrentScenario = nextPhaseScenarios.First();
//            await LoadChoicesAsync();
//        }
//        else
//        {
//            SelectedProject = null;
//            CurrentScenario = null;
//        }

//        await SaveUserProgressAsync(userId);
//    }

//    private async Task SaveUserProgressAsync(string userId)
//    {
//        var progress = new UserProgress
//        {
//            UserId = userId,
//            ProjectId = SelectedProject?.Id ?? 0,
//            CurrentScenarioId = CurrentScenario?.Id ?? 0,
//            Score = Score,
//        };

//        await userProgressService.SaveUserProgressAsync(progress);
//    }
//}