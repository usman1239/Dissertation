﻿using System.Collections.ObjectModel;
using Dissertation.Models;
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
    IDeveloperService developerService,
    ISnackbar snackbar,
    INavigationService navigationService)
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
            var totalSprints = projectStateService.CurrentProjectInstance.Project.NumOfSprints + 1;
            return Enumerable.Range(0, totalSprints).Select(i => $"{i}").ToArray();
        }
    }

    public ObservableCollection<int> SprintProgress { get; set; } = [];

    public ObservableCollection<int> ExpectedProgress { get; set; } = [];

    public string SprintSummary { get; set; } = "";

    public bool CanShowSummary()
    {
        var allStoriesCompleted =
            projectStateService.CurrentProjectInstance.UserStoryInstances.All(usi => usi.IsComplete);
        var allSprintsCompleted =
            projectStateService.CurrentProjectInstance.Sprints.Count(s => s.IsCompleted) >=
            projectStateService.CurrentProjectInstance.Project.NumOfSprints;

        return allStoriesCompleted || allSprintsCompleted;
    }

    public bool CanStartSprint()
    {
        if (!CanAffordSprint()) return false;

        if (!projectStateService.Team.Any()) return false;

        var hasAssignedIncompleteStories = projectStateService.UserStoryInstances
            .Any(usi => usi is { DeveloperAssignedId: not null, IsComplete: false });

        var completedSprintsCount = projectStateService.Sprints.Count(s => s.IsCompleted);
        var totalSprints = projectStateService.CurrentProjectInstance.Project.NumOfSprints;

        return hasAssignedIncompleteStories && completedSprintsCount < totalSprints;
    }

    private bool CanAffordSprint()
    {
        var totalSalary = GetTotalSalary();
        if (projectStateService.CurrentProjectInstance.Budget >= totalSalary) return true;

        snackbar.Add("Insufficient budget to start the next sprint!", Severity.Error);
        return false;
    }

    public async Task StartSprint()
    {
        snackbar.Add("Sprint started successfully!", Severity.Success);

        if (!CanAffordSprint()) return;

        await RecoverSickDevelopers();

        var revenue = ProcessUserStories();

        UpdateBudget(revenue - GetTotalSalary());

        await userStoryService.SaveUserStoryInstancesAsync(projectStateService.UserStoryInstances.ToList());
        await SaveSprint();

        ShowSummaryOrSprints();
    }

    public async Task RecoverSickDevelopers()
    {
        var completedSprintsCount = projectStateService.Sprints.Count(s => s.IsCompleted);

        foreach (var dev in projectStateService.Team)
        {
            if (!dev.IsSick || dev.SickUntilSprint >= completedSprintsCount) continue;
            dev.IsSick = false;
            dev.SickUntilSprint = 0;
            await developerService.UpdateDeveloperAbsence(dev);
        }
    }

    private async Task HandleRandomEvents()
    {
        var totalSprints = projectStateService.CurrentProjectInstance.Project.NumOfSprints;
        var completedSprintsCount = projectStateService.Sprints.Count(s => s.IsCompleted);

        if (completedSprintsCount < totalSprints * 2 / 3) return;

        Random random = new();
        var eventChoice = random.Next(1, 4);

        switch (eventChoice)
        {
            case 1:
                return;
            case 2:
                await HandleSickOrAbsentDeveloperEvent(random, completedSprintsCount);
                break;
            case 3:
                await HandleNewRandomUserStory();
                break;
        }
    }

    public async Task HandleNewRandomUserStory()
    {
        await userStoryService.TriggerRandomUserStoryEventAsync(projectStateService.CurrentProjectInstance.Id);

        var refreshedStories =
            await userStoryService.GetUserStoryInstancesForProjectAsync(projectStateService.CurrentProjectInstance.Id);
        projectStateService.UserStoryInstances.Clear();

        foreach (var story in refreshedStories)
            projectStateService.UserStoryInstances.Add(story);

        snackbar.Add("A new user story has been added!", Severity.Info);
    }

    public async Task HandleSickOrAbsentDeveloperEvent(Random random, int completedSprintsCount)
    {
        var sickDeveloper = GetRandomSickDeveloper();
        if (random.Next(0, 2) == 0)
        {
            sickDeveloper.SickUntilSprint = completedSprintsCount + 1;
            sickDeveloper.IsSick = true;
            sickDeveloper.IsPermanentlyAbsent = false;
            snackbar.Add($"{sickDeveloper.Name} is sick and will miss the next sprint.", Severity.Warning);
        }
        else
        {
            sickDeveloper.IsPermanentlyAbsent = true;
            sickDeveloper.IsSick = false;
            sickDeveloper.SickUntilSprint = 0;
            snackbar.Add($"{sickDeveloper.Name} is permanently absent!", Severity.Warning);
        }

        await developerService.UpdateDeveloperAbsence(sickDeveloper);
    }

    private Developer GetRandomSickDeveloper()
    {
        var random = new Random();
        var developers = projectStateService.Team.Where(x => x is { IsPermanentlyAbsent: false, IsSick: false })
            .ToList();
        return developers[random.Next(developers.Count)];
    }

    public Dictionary<int, List<UserStoryInstance>> GetDeveloperAssignments()
    {
        var developerDictionary = projectStateService.Team.ToDictionary(dev => dev.Id);

        return projectStateService.UserStoryInstances
            .Where(usi => usi is { DeveloperAssignedId: not null, IsComplete: false })
            .Where(usi =>
                !developerDictionary[usi.DeveloperAssignedId!.Value].IsSick &&
                !developerDictionary[usi.DeveloperAssignedId.Value].IsPermanentlyAbsent &&
                (developerDictionary[usi.DeveloperAssignedId.Value].SickUntilSprint == 0 ||
                 developerDictionary[usi.DeveloperAssignedId.Value].SickUntilSprint >
                 projectStateService.Sprints.Count))
            .GroupBy(usi => usi.DeveloperAssignedId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public int CalculateDeveloperProgress(Developer dev, int numStories, Random random)
    {
        var workloadPenalty = Math.Max(1 - 0.1 * (numStories - 1), 0.5);

        var baseProgress = dev.ExperienceLevel switch
        {
            DeveloperExperienceLevel.Junior => random.Next(20, 30),
            DeveloperExperienceLevel.MidLevel => random.Next(40, 50),
            DeveloperExperienceLevel.Senior => random.Next(60, 70),
            _ => 0
        };

        return (int)(baseProgress * workloadPenalty);
    }

    public int UpdateStoryProgress(List<UserStoryInstance> stories, int totalProgressIncrease)
    {
       var revenue = 0;
        var totalStoryPoints = stories.Sum(usi => usi.UserStory.StoryPoints);

        if (totalStoryPoints == 0)
            return revenue;

        foreach (var usi in stories)
        {
            var developer = projectStateService.Team.FirstOrDefault(d => d.Id == usi.DeveloperAssignedId);
            if (developer == null || developer.IsSick || developer.IsPermanentlyAbsent)
                continue;

            var weight = (double)usi.UserStory.StoryPoints / totalStoryPoints;
            var progressIncrease = (int)Math.Ceiling(totalProgressIncrease * weight);

            usi.Progress = Math.Min(usi.Progress + progressIncrease, 100);

            if (usi.Progress != 100) continue;
            usi.IsComplete = true;
            revenue += usi.UserStory.StoryPoints * 500;
        }

        return revenue;
    }

    private int ProcessUserStories()
    {
        var revenue = 0;
        Random random = new();

        var developerAssignments = GetDeveloperAssignments();

        foreach (var (developerId, stories) in developerAssignments)
        {
            var dev = projectStateService.Team.FirstOrDefault(d => d.Id == developerId);
            if (dev == null || dev.IsSick || dev.IsPermanentlyAbsent) continue;

            var progressIncrease = CalculateDeveloperProgress(dev, stories.Count, random);

            revenue += UpdateStoryProgress(stories, progressIncrease);
        }

        return revenue;
    }

    private int GetTotalSalary()
    {
        return projectStateService.Team.Sum(dev => dev.Cost);
    }

    private async Task SaveSprint()
    {
        var existingSprints =
            await sprintService.GetSprintsForProjectAsync(projectStateService.CurrentProjectInstance.Id);
        var completedStories = projectStateService.UserStoryInstances.Count(usi => usi.IsComplete);
        var remainingWork = projectStateService.UserStoryInstances.Where(usi => !usi.IsComplete)
            .Sum(usi => usi.UserStory.StoryPoints);

        var previousTotalWork = SprintProgress.Any() ? SprintProgress.Last() : remainingWork;
        var sprintProgress = previousTotalWork - remainingWork;

        LoadSprintProgressAsync();

        var revenueEarned = projectStateService.UserStoryInstances.Where(usi => usi.IsComplete)
            .Sum(usi => usi.UserStory.StoryPoints * 500);

        var newSprint = new Sprint
        {
            ProjectInstanceId = projectStateService.CurrentProjectInstance.Id,
            ProjectInstance = projectStateService.CurrentProjectInstance,
            SprintNumber = existingSprints.Count + 1,
            Duration = 14,
            IsCompleted = true,
            Progress = sprintProgress,
            Summary = $"Sprint {existingSprints.Count + 1} completed.\n" +
                      $"- {completedStories} user stories completed.\n" +
                      $"- Total progress: {sprintProgress} story points.\n" +
                      $"- Revenue earned: £{revenueEarned:N0}.\n" +
                      $"- Remaining work: {remainingWork} story points.\n" +
                      $"- Remaining budget: £{projectStateService.CurrentProjectInstance.Budget:N0}."
        };

        projectStateService.Sprints.Add(newSprint);
        await sprintService.SaveSprintAsync(newSprint);
    }

    public void LoadSprintProgressAsync()
    {
        var savedSprints = projectStateService.Sprints;

        SprintProgress.Clear();
        ExpectedProgress.Clear();

        var totalWork = projectStateService.UserStoryInstances.Sum(usi => usi.UserStory.StoryPoints);
        var remainingWork = totalWork;

        var completedSprints = projectStateService.CurrentProjectInstance.Sprints.Count;
        var totalSprints =
            projectStateService.CurrentProjectInstance.Project.NumOfSprints;
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
        var newBudget = projectStateService.CurrentProjectInstance.Budget + amount;
        projectStateService.CurrentProjectInstance.Budget = Math.Max(newBudget, 0);
    }

    public void ShowSummaryOrSprints()
    {
        var allStoriesCompleted = projectStateService.UserStoryInstances.All(usi => usi.IsComplete);
        var completedSprintsCount = projectStateService.Sprints.Count(s => s.IsCompleted);
        var totalSprints = projectStateService.CurrentProjectInstance.Project.NumOfSprints;

        if (allStoriesCompleted || completedSprintsCount >= totalSprints)
        {
            snackbar.Add("All sprints completed!", Severity.Success);
            navigationService.NavigateTo("/challenge/summary");
        }
        else
        {
            snackbar.Add($"{totalSprints - completedSprintsCount} Sprints Left", Severity.Success);
            navigationService.NavigateTo("/challenge/sprints");
            _ = HandleRandomEvents();
        }
    }
}