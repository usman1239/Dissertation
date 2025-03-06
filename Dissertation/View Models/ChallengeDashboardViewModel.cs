using System.Collections.ObjectModel;
using System.Text;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;

namespace Dissertation.View_Models;

public class ChallengeDashboardViewModel(IChallengeService challengeService, IUserService userService)
{
    public string ErrorMessage { get; set; } = string.Empty;

    #region Project Variables

    public ObservableCollection<Project?> AvailableProjects { get; set; } = [];
    public ProjectInstance? CurrentProject { get; private set; }
    public ObservableCollection<ProjectInstance> SavedProjects { get; set; } = [];

    #endregion

    #region DevelopmentVariables

    public ObservableCollection<Developer> Team { get; set; } = [];
    public string DeveloperName { get; set; } = "";
    public string SelectedDeveloper { get; set; } = "";

    public DeveloperExperienceLevel SelectedDeveloperExperienceLevel { get; set; } = DeveloperExperienceLevel.Junior;

    #endregion

    #region Sprints Variables

    public ObservableCollection<Sprint> Sprints { get; set; } = [];
    public string SprintSummary { get; set; } = "";

    #endregion

    #region User Stories Variables

    public ObservableCollection<UserStory> UserStories { get; set; } = [];
    public string UserStory { get; set; } = "";
    public string SelectedUserStory { get; set; } = "";
    public int StoryPoints { get; set; }
    public string? UserId { get; set; }

    #endregion

    public async Task GetUser()
    {
        UserId = await userService.GetUserIdAsync();
    }

    public async Task LoadAvailableProjectsAsync()
    {
        var projects = await challengeService.GetAvailableProjectsAsync();
        AvailableProjects.Clear();
        foreach (var project in projects) AvailableProjects.Add(project);
    }

    public async Task LoadProjectsWithSavedProgress()
    {
        var projects = await challengeService.LoadProjectsWithSavedProgressAsync(UserId);
        SavedProjects.Clear();
        foreach (var project in projects) SavedProjects.Add(project);
    }

    public async Task SelectProject(int projectId, bool isSavedProject)
    {
        ErrorMessage = string.Empty;

        var existingProjectInstance = await challengeService.GetProjectInstanceAsync(projectId, UserId);

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

            InitializeNewProjectInstance(selectedProject);
        }
    }

    private void LoadExistingProject(ProjectInstance projectInstance)
    {
        CurrentProject = projectInstance;
        Sprints = new ObservableCollection<Sprint>(projectInstance.Sprints);
        UserStories = new ObservableCollection<UserStory>(projectInstance.UserStories);
    }

    private void InitializeNewProjectInstance(Project selectedProject)
    {
        CurrentProject = new ProjectInstance
        {
            ProjectId = selectedProject.Id,
            Project = selectedProject,
            Budget = selectedProject.Budget,
            Sprints = [],
            UserStories = [],
            UserId = UserId!
        };

        Team.Clear();
        Sprints.Clear();
        UserStories.Clear();
    }

    public async Task<bool> DeleteSavedProjectInstanceAsync(int projectId)
    {
        return await challengeService.DeleteSavedProjectInstanceAsync(projectId, UserId);
    }


    #region Developer Methods

    public void AddDeveloper()
    {
        if (Team.Any(x => x.Name == DeveloperName &&
                          x.ExperienceLevel == SelectedDeveloperExperienceLevel))
            return;

        var cost = SelectedDeveloperExperienceLevel switch
        {
            DeveloperExperienceLevel.Junior => 1000,
            DeveloperExperienceLevel.MidLevel => 2000,
            DeveloperExperienceLevel.Senior => 3000,
            _ => throw new ArgumentOutOfRangeException(nameof(SelectedDeveloperExperienceLevel))
        };

        var dev = new Developer
        {
            Name = DeveloperName,
            ExperienceLevel = SelectedDeveloperExperienceLevel,
            Cost = cost,
            UserId = UserId!
        };
        Team.Add(dev);
        DeveloperName = "";
    }

    public bool CanAddDeveloper()
    {
        return string.IsNullOrEmpty(DeveloperName);
    }

    public void RemoveDeveloper(Developer dev)
    {
        Team.Remove(dev);
    }

    #endregion

    #region Sprint Methods

    public bool CanStartSprint()
    {
        return true;
        //return UserStories.Count == 0 ||
        //       UserStories.Any(x => x.AssignedTo == null ||
        //                            x.Count == 0 ||
        //                            x.Any(t => t.AssignedTo == null));
    }

    private async Task UpdateDbAfterSprint()
    {
        if (CurrentProject != null)
        {
            await challengeService.AddDevelopersToDbAsync(Team.ToList());
            await challengeService.AddUserProjectInstanceAsync(CurrentProject);
            await challengeService.AddOrUpdateUserStoriesAsync(UserStories.ToList());
            await challengeService.AddSprintsToDbAsync(Sprints.ToList());
        }
    }

    public async Task StartSprint()
    {
        if (!Team.Any() || !UserStories.Any())
        {
            Console.WriteLine("Cannot start sprint without developers and user stories.");
            return;
        }

        Console.WriteLine("Sprint started...");

        var completedStories = 0;
        StringBuilder sprintSummary = new();
        var totalSalary = 0;
        var revenue = 0;
        Random random = new();

        foreach (var dev in Team)
        {
            totalSalary += dev.Cost;
            List<UserStory> completedUserStories = [];

            sprintSummary.AppendLine($"👨‍💻 Developer: {dev.Name} (Exp: {dev.ExperienceLevel})");

            foreach (var story in UserStories.Where(us => us.AssignedTo == dev))
            {

            }

            completedStories += completedUserStories.Count;
        }

        UpdateBudget(revenue - totalSalary);

        SprintSummary = $"🚀 Sprint Complete!\n" +
                        $"📌 User Stories Worked On: {UserStories.Count}\n" +
                        $"✅ Completed Stories: {completedStories}\n" +
                        $"💰 Budget Change: £{revenue - totalSalary} (Revenue: £{revenue}, Salaries: £{totalSalary})\n\n" +
                        sprintSummary;

        Console.WriteLine(SprintSummary);

        Sprints.Add(new Sprint
        {
            Duration = 14,
            IsCompleted = completedStories > 0,
            Summary = SprintSummary,
            ProjectInstanceId = CurrentProject!.Id,
            ProjectInstance = CurrentProject,
            SprintNumber = Sprints.Count + 1
        });

        await UpdateDbAfterSprint();
    }

    #endregion

    #region User Story Methods

    public void AddUserStory()
    {
        if (UserStories.Any(x => x.Title == UserStory))
            return;

        UserStories.Add(new UserStory
        {
            Title = UserStory,
            StoryPoints = StoryPoints,
            ProjectInstanceId = CurrentProject!.Id,
            ProjectInstance = CurrentProject
        });
        UserStory = "";
        StoryPoints = 0;
    }

    public void AssignUserStoryToDeveloper()
    {
        var developer = Team.FirstOrDefault(dev => dev.Name == SelectedDeveloper);
        var story = UserStories.FirstOrDefault(us => us.Title == SelectedUserStory);

        if (developer != null && story != null)
        {
            story.AssignedTo = developer;
            story.AssignedToId = developer.Id;
        }

        SelectedDeveloper = "";
        SelectedUserStory = "";
    }

    public bool CanAssignStory()
    {
        return string.IsNullOrEmpty(SelectedDeveloper) || string.IsNullOrEmpty(SelectedUserStory);
    }

    public bool CanAddUserStory()
    {
        return StoryPoints <= 0 || string.IsNullOrEmpty(UserStory);
    }

    #endregion

    public void UpdateBudget(int amount)
    {
        if (CurrentProject != null)
            CurrentProject.Budget += amount;
    }
}