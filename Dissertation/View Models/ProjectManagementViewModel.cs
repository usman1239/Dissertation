using System.Collections.ObjectModel;
using Dissertation.Models;
using Dissertation.Models.Challenge;
using Dissertation.Services;
using Dissertation.Services.Interfaces;
using MudBlazor;
using NuGet.Packaging;

namespace Dissertation.View_Models;

public class ProjectManagementViewModel(
    ProjectStateService projectStateService,
    IProjectService projectService,
    IUserService userService,
    IUserStoryService userStoryService,
    ISnackbar snackbar,
    INavigationService navigationService)
{
    public string ErrorMessage { get; set; } = string.Empty;
    public ObservableCollection<Project?> AvailableProjects { get; set; } = [];
    public ObservableCollection<ProjectInstance> SavedProjects { get; set; } = [];

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
            navigationService.NavigateTo("/challenge/dashboard");
        }
    }

    private void LoadExistingProject(ProjectInstance projectInstance)
    {
        projectStateService.CurrentProjectInstance = projectInstance;
        projectStateService.Sprints = new ObservableCollection<Sprint>(projectInstance.Sprints);
        projectStateService.UserStoryInstances =
            new ObservableCollection<UserStoryInstance>(projectInstance.UserStoryInstances);
        projectStateService.Team = new ObservableCollection<Developer>(
            projectStateService.UserStoryInstances
                .Where(usi => usi.DeveloperAssigned != null)
                .Select(usi => usi.DeveloperAssigned!)
                .DistinctBy(dev => dev.Id)
                .ToList()
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
}