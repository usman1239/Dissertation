using System.Collections.ObjectModel;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using MudBlazor;
using NuGet.Packaging;

namespace Dissertation.View_Models;

public class ProjectManagementViewModel(
    ProjectStateService projectStateService,
    IProjectService projectService,
    IUserService userService,
    IUserStoryService userStoryService,
    IDailyChallengeService dailyChallengeService,
    IBadgeService badgeService,
    ISnackbar snackbar,
    INavigationService navigationService)
{
    public bool IsNewProject { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public ObservableCollection<Project?> AvailableProjects { get; set; } = [];
    public ObservableCollection<ProjectInstance> SavedProjects { get; set; } = [];
    public string CurrentChallengeDescription { get; set; } = "";

    public async Task GetUser()
    {
        projectStateService.UserId = await userService.GetUserIdAsync();
    }

    public async Task LoadAvailableProjectsAsync()
    {
        var projects = await projectService.GetAvailableProjectsAsync();
        AvailableProjects.Clear();
        foreach (var project in projects) AvailableProjects.Add(project);
    }

    public async Task LoadProjectsWithSavedProgress()
    {
        var projects = await projectService.LoadProjectsWithSavedProgressAsync(projectStateService.UserId);
        SavedProjects.Clear();
        foreach (var project in projects)
            SavedProjects.Add(project);
    }

    public async Task SelectProject(int projectId, bool isSavedProject)
    {
        ErrorMessage = string.Empty;
        var existingProjectInstance =
            await projectService.GetProjectInstanceAsync(projectId, projectStateService.UserId);

        if (isSavedProject && existingProjectInstance != null)
        {
            LoadExistingProject(existingProjectInstance);
            IsNewProject = false;
            navigationService.NavigateTo("/challenge/dashboard");

            return;
        }

        var selectedProject = AvailableProjects.FirstOrDefault(p => p?.Id == projectId);

        if (selectedProject != null)
        {
            if (existingProjectInstance != null)
            {
                snackbar.Add("You cannot select this project because an instance already exists.", Severity.Warning);
                return;
            }

            await InitializeNewProjectInstance(selectedProject);
            IsNewProject = true;
            navigationService.NavigateTo("/challenge/dashboard");
        }
    }

    private void LoadExistingProject(ProjectInstance projectInstance)
    {
        projectStateService.CurrentProjectInstance = projectInstance;
        projectStateService.Sprints = [.. projectInstance.Sprints];
        projectStateService.UserStoryInstances =
            [.. projectInstance.UserStoryInstances];
        projectStateService.Team = new ObservableCollection<Developer>(
            [
                .. projectStateService.UserStoryInstances
                    .Where(usi => usi.DeveloperAssigned != null)
                    .Select(usi => usi.DeveloperAssigned!)
                    .DistinctBy(dev => dev.Id)
            ]
        );
    }

    private async Task InitializeNewProjectInstance(Project selectedProject)
    {
        var userStories = await userStoryService.GetInitialUserStoriesForProject(selectedProject.Id);

        projectStateService.CurrentProjectInstance = new ProjectInstance
        {
            ProjectId = selectedProject.Id,
            Project = selectedProject,
            Budget = selectedProject.Budget,
            Sprints = [],
            UserStoryInstances = [],
            UserId = projectStateService.UserId!
        };

        projectStateService.CurrentProjectInstance.UserStoryInstances =
        [
            .. userStories.Select(us => new UserStoryInstance
            {
                UserStoryId = us.Id,
                UserStory = us,
                ProjectInstanceId = projectStateService.CurrentProjectInstance.Id,
                Progress = 0,
                DeveloperAssignedId = null,
                IsComplete = false
            })
        ];

        await userStoryService.AttachProjectAndUserStories(projectStateService.CurrentProjectInstance);

        await projectService.SaveNewProjectInstance(projectStateService.CurrentProjectInstance);

        projectStateService.Team.Clear();
        projectStateService.Sprints.Clear();
        projectStateService.UserStoryInstances.Clear();
        projectStateService.UserStoryInstances.AddRange(projectStateService.CurrentProjectInstance.UserStoryInstances);
    }


    public async Task DeleteSavedProjectInstanceAsync(int projectId)
    {
        var deleteSavedProjectInstanceAsync =
            await projectService.DeleteSavedProjectInstanceAsync(projectId, projectStateService.UserId);

        if (deleteSavedProjectInstanceAsync)
            snackbar.Add("Project deleted successfully.", Severity.Success);
        else
            snackbar.Add("Failed to delete the project.", Severity.Error);

        await LoadProjectsWithSavedProgress();
    }

    public async Task<bool> IsChallengeActive()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var alreadyCompleted = await projectService.HasCompletedChallengeAsync(
            projectStateService.UserId!,
            projectStateService.CurrentProjectInstance.Id,
            today);

        if (alreadyCompleted)
        {
            snackbar.Add("Challenge was already taken on!");
            CurrentChallengeDescription = dailyChallengeService.GetTodayChallenge().Description;
        }
        return alreadyCompleted;
    }

    public async Task GetDailyChallenge()
    {
        var challenge = dailyChallengeService.GetTodayChallenge();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var alreadyCompleted = await projectService.HasCompletedChallengeAsync(
            projectStateService.UserId!,
            projectStateService.CurrentProjectInstance.Id,
            today);

        if (!alreadyCompleted)
        {
            challenge.Apply(projectStateService);
            projectStateService.ActiveChallenge = challenge;

            await projectService.UpdateProjectInstance(projectStateService.CurrentProjectInstance);

            await projectService.MarkChallengeCompletedAsync(
                projectStateService.UserId!,
                projectStateService.CurrentProjectInstance.Id,
                today,
                challenge.Id);

            await badgeService.CheckDailyBadges(projectStateService.UserId!);
            CurrentChallengeDescription = challenge.Description;
            snackbar.Add("Daily challenge applied: " + challenge.Description, Severity.Info);
        }
        else if (!IsNewProject)
        {
            CurrentChallengeDescription = $"Challenge Already Applied ({challenge.Description})";
        }
    }
}