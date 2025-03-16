using System.Collections.ObjectModel;
using Dissertation.Models.Challenge;
using Dissertation.Services;
using Dissertation.Services.Interfaces;
using NuGet.Packaging;

namespace Dissertation.View_Models;

public class ProjectManagementViewModel(
    ProjectStateService projectStateService,
    IProjectService projectService,
    IUserService userService,
    IUserStoryService userStoryService)
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
        foreach (var project in projects) SavedProjects.Add(project);
    }

    public async Task SelectProject(int projectId, bool isSavedProject)
    {
        ErrorMessage = string.Empty;
        var existingProjectInstance =
            await projectService.GetProjectInstanceAsync(projectId, projectStateService.UserId);

        if (isSavedProject && existingProjectInstance != null)
        {
            LoadExistingProject(existingProjectInstance);
            return;
        }

        var selectedProject = AvailableProjects.FirstOrDefault(p => p?.Id == projectId);

        if (selectedProject != null)
        {
            if (existingProjectInstance != null)
            {
                ErrorMessage = "You cannot select this project because an instance already exists.";
                return;
            }

            await InitializeNewProjectInstance(selectedProject);
        }
    }

    private void LoadExistingProject(ProjectInstance projectInstance)
    {
        projectStateService.CurrentProject = projectInstance;
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

        projectStateService.CurrentProject = new ProjectInstance
        {
            ProjectId = selectedProject.Id,
            Project = selectedProject,
            Budget = selectedProject.Budget,
            Sprints = [],
            UserStoryInstances = [],
            UserId = projectStateService.UserId!
        };

        projectStateService.CurrentProject.UserStoryInstances = userStories.Select(us => new UserStoryInstance
        {
            UserStoryId = us.Id,
            UserStory = us,
            ProjectInstanceId = projectStateService.CurrentProject.Id,
            Progress = 0,
            DeveloperAssignedId = null,
            IsComplete = false
        }).ToList();

        await userStoryService.AttachProjectAndUserStories(projectStateService.CurrentProject);

        await projectService.SaveNewProjectInstance(projectStateService.CurrentProject);

        projectStateService.Team.Clear();
        projectStateService.Sprints.Clear();
        projectStateService.UserStoryInstances.Clear();
        projectStateService.UserStoryInstances.AddRange(projectStateService.CurrentProject.UserStoryInstances);
    }


    public async Task<bool> DeleteSavedProjectInstanceAsync(int projectId)
    {
        return await projectService.DeleteSavedProjectInstanceAsync(projectId, projectStateService.UserId);
    }
}