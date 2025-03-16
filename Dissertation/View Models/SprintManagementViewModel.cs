using System.Collections.ObjectModel;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services;
using Dissertation.Services.Interfaces;
using MudBlazor;

namespace Dissertation.View_Models;

public class SprintManagementViewModel(
    ProjectStateService projectStateService,
    ISprintService sprintService,
    IUserStoryService userStoryService,
    IDeveloperService developerService)
{
    public ChartOptions ChartOptions { get; set; } = new()
    {
        YAxisTicks = 5
    };

    public List<ChartSeries> Series =>
    [
        new()
        {
            Name = "Remaining Work",
            Data = SprintProgress.Select(p => (double)p).ToArray()
        },
        new()
        {
            Name = "Expected Progress",
            Data = ExpectedProgress.Select(p => (double)p).ToArray()
        }
    ];

    public string[] SprintLabels
    {
        get
        {
            var totalSprints = projectStateService.CurrentProject.Project.NumOfSprints + 1;
            return Enumerable.Range(0, totalSprints).Select(i => $"Sprint {i}").ToArray();
        }
    }

    public ObservableCollection<int> SprintProgress { get; set; } = [];
    public ObservableCollection<int> ExpectedProgress { get; set; } = [];

    public string SprintSummary { get; set; } = "";

    public bool CanShowSummary()
    {
        var allStoriesCompleted = projectStateService.CurrentProject.UserStoryInstances.All(usi => usi.IsComplete);
        var allSprintsCompleted =
            projectStateService.CurrentProject.Sprints.Count(s => s.IsCompleted) >=
            projectStateService.CurrentProject.Project.NumOfSprints;

        return allStoriesCompleted || allSprintsCompleted;
    }

    public bool CanStartSprint()
    {
        if (!projectStateService.Team.Any()) return false;

        var hasAssignedIncompleteStories = projectStateService.UserStoryInstances
            .Any(usi => usi is { DeveloperAssignedId: not null, IsComplete: false });

        var completedSprintsCount = projectStateService.Sprints.Count(s => s.IsCompleted);
        var totalSprints = projectStateService.CurrentProject.Project.NumOfSprints;

        return hasAssignedIncompleteStories && CanAffordSprint() && completedSprintsCount < totalSprints;
    }

    private bool CanAffordSprint()
    {
        var totalSalary = projectStateService.Team.Sum(dev => dev.Cost);
        return projectStateService.CurrentProject.Budget >= totalSalary;
    }

    public async Task StartSprint()
    {
        await AddNewDeveloperAsync();

        var totalSalary = GetTotalSalary();
        if (!CanAffordSprint(totalSalary)) return;

        var revenue = ProcessUserStories();

        UpdateBudget(revenue - totalSalary);

        await userStoryService.SaveUserStoryInstancesAsync(projectStateService.UserStoryInstances.ToList());
        await SaveSprint();
    }

    private static int CalculateDeveloperProgress(Developer dev, int numStories, Random random)
    {
        var workloadPenalty = Math.Max(1 - 0.1 * (numStories - 1), 0.5);

        var baseProgress = dev.ExperienceLevel switch
        {
            DeveloperExperienceLevel.Junior => random.Next(10, 30),
            DeveloperExperienceLevel.MidLevel => random.Next(30, 50),
            DeveloperExperienceLevel.Senior => random.Next(50, 80),
            _ => 0
        };

        return (int)(baseProgress * workloadPenalty);
    }

    private static int UpdateStoryProgress(List<UserStoryInstance> stories, int totalProgressIncrease)
    {
        var revenue = 0;
        var totalStoryPoints = stories.Sum(usi => usi.UserStory.StoryPoints);

        foreach (var usi in stories)
        {
            var weight = (double)usi.UserStory.StoryPoints / totalStoryPoints;
            var progressIncrease = (int)(totalProgressIncrease * (1 - weight));

            usi.Progress = Math.Min(usi.Progress + progressIncrease, 100);

            if (usi.Progress != 100) continue;
            usi.IsComplete = true;
            revenue += usi.UserStory.StoryPoints * 500;
        }

        return revenue;
    }

    private Dictionary<int, List<UserStoryInstance>> GetDeveloperAssignments()
    {
        return projectStateService.UserStoryInstances
            .Where(usi => usi is { DeveloperAssignedId: not null, IsComplete: false })
            .GroupBy(usi => usi.DeveloperAssignedId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private int ProcessUserStories()
    {
        var revenue = 0;
        Random random = new();

        var developerAssignments = GetDeveloperAssignments();

        foreach (var (developerId, stories) in developerAssignments)
        {
            var dev = projectStateService.Team.FirstOrDefault(d => d.Id == developerId);
            if (dev == null) continue;

            var progressIncrease = CalculateDeveloperProgress(dev, stories.Count, random);

            revenue += UpdateStoryProgress(stories, progressIncrease);
        }

        return revenue;
    }

    private bool CanAffordSprint(int totalSalary)
    {
        return projectStateService.CurrentProject.Budget >= totalSalary;
    }

    private int GetTotalSalary()
    {
        return projectStateService.Team.Sum(dev => dev.Cost);
    }

    private async Task AddNewDeveloperAsync()
    {
        var newDevelopers = projectStateService.Team.Where(dev => dev.Id == 0).ToList();
        if (newDevelopers.Any())
            await developerService.AddDevelopersAsync(newDevelopers);
    }

    private async Task SaveSprint()
    {
        var existingSprints = await sprintService.GetSprintsForProjectAsync(projectStateService.CurrentProject.Id);
        var completedStories = projectStateService.UserStoryInstances.Count(usi => usi.IsComplete);
        var remainingWork = projectStateService.UserStoryInstances.Where(usi => !usi.IsComplete)
            .Sum(usi => usi.UserStory.StoryPoints);

        var previousTotalWork = SprintProgress.Any() ? SprintProgress.Last() : remainingWork;
        var sprintProgress = previousTotalWork - remainingWork;

        await LoadSprintProgressAsync();

        var revenueEarned = projectStateService.UserStoryInstances.Where(usi => usi.IsComplete)
            .Sum(usi => usi.UserStory.StoryPoints * 500);

        var newSprint = new Sprint
        {
            ProjectInstanceId = projectStateService.CurrentProject.Id,
            ProjectInstance = projectStateService.CurrentProject,
            SprintNumber = existingSprints.Count + 1,
            Duration = 14,
            IsCompleted = true,
            Progress = sprintProgress,
            Summary = $"Sprint {existingSprints.Count + 1} completed.\n" +
                      $"- {completedStories} user stories completed.\n" +
                      $"- Total progress: {sprintProgress} story points.\n" +
                      $"- Revenue earned: £{revenueEarned:N0}.\n" +
                      $"- Remaining work: {remainingWork} story points.\n" +
                      $"- Remaining budget: £{projectStateService.CurrentProject.Budget:N0}."
        };

        projectStateService.Sprints.Add(newSprint);
        await sprintService.SaveSprintAsync(newSprint);
    }

    public async Task LoadSprintProgressAsync()
    {
        var savedSprints = await sprintService.GetSprintsForProjectAsync(projectStateService.CurrentProject.Id);

        SprintProgress.Clear();
        ExpectedProgress.Clear();

        var totalWork = projectStateService.UserStoryInstances.Sum(usi => usi.UserStory.StoryPoints);
        var remainingWork = totalWork;

        var completedSprints = projectStateService.CurrentProject.Sprints.Count;
        var totalSprints =
            projectStateService.CurrentProject.Project.NumOfSprints;
        var expectedDecreasePerSprint = totalSprints > 0 ? totalWork / totalSprints : 0;

        SprintProgress.Add(totalWork);
        ExpectedProgress.Add(totalWork);

        for (var i = 0; i < Math.Max(completedSprints, totalSprints); i++)
        {
            if (i < savedSprints.Count)
            {
                remainingWork -= savedSprints[i].Progress;
                SprintProgress.Add(remainingWork);
            }
            else if (i < completedSprints)
            {
                SprintProgress.Add(SprintProgress.Last());
            }

            if (i < totalSprints)
                ExpectedProgress.Add(Math.Max(totalWork - expectedDecreasePerSprint * (i + 1), 0));
        }
    }

    public void UpdateBudget(int amount)
    {
        var newBudget = projectStateService.CurrentProject.Budget + amount;
        projectStateService.CurrentProject.Budget = Math.Max(newBudget, 0);
    }
}