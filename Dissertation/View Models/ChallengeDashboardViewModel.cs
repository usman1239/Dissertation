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
    public UserProjectInstance? CurrentProject { get; private set; }
    public ObservableCollection<UserProjectInstance> SavedProjects { get; set; } = [];

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

    #region Tasks Variables

    public ObservableCollection<UserStoryTask> Tasks { get; set; } = [];
    public string Task { get; set; } = "";

    public UserStoryTaskType SelectedTaskType = UserStoryTaskType.BacklogItem;

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

    private void LoadExistingProject(UserProjectInstance projectInstance)
    {
        CurrentProject = projectInstance;
        Sprints = new ObservableCollection<Sprint>(projectInstance.Sprints);
        UserStories = new ObservableCollection<UserStory>(projectInstance.UserStories);
        Tasks = new ObservableCollection<UserStoryTask>(UserStories.SelectMany(us => us.Tasks));

        Team = new ObservableCollection<Developer>(
            UserStories.Select(us => us.AssignedTo)
                .Concat(UserStories.SelectMany(us => us.Tasks.Select(t => t.AssignedTo)))
                .Where(dev => dev != null)
                .Distinct()
                .ToList()!
        );
    }

    private void InitializeNewProjectInstance(Project selectedProject)
    {
        CurrentProject = new UserProjectInstance
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
        Tasks.Clear();
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
        return UserStories.Count == 0 ||
               UserStories.Any(x => x.AssignedTo == null ||
                                    x.Tasks.Count == 0 ||
                                    x.Tasks.Any(t => t.AssignedTo == null));
    }

    private async Task UpdateDbAfterSprint()
    {
        if (CurrentProject != null)
        {
            await challengeService.AddDevelopersToDbAsync(Team.ToList());
            await challengeService.AddUserProjectInstanceAsync(CurrentProject);
            await challengeService.AddOrUpdateUserStoriesAndTasksAsync(UserStories.ToList());
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
                var allTasksCompleted = true;
                StringBuilder taskSummary = new();

                foreach (var task in story.Tasks.Where(t => t.AssignedTo == dev))
                {
                    var progressIncrease = GetTaskProgressIncrement(dev.ExperienceLevel, task.Difficulty, random);
                    task.Progress = Math.Min(100, task.Progress + progressIncrease);

                    var status = task.IsCompleted ? "✅ Completed" : $"⏳ {task.Progress}% done";
                    taskSummary.AppendLine(
                        $"   - 📌 Task: {task.Title} ({task.Type}, Difficulty: {task.Difficulty}) → {status}");

                    if (!task.IsCompleted) allTasksCompleted = false;
                }

                if (allTasksCompleted)
                {
                    completedUserStories.Add(story);
                    revenue += 5000;
                }

                sprintSummary.AppendLine($"📖 User Story: {story.Title} (Assigned to: {dev.Name})");
                sprintSummary.Append(taskSummary);
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


    private static int GetTaskProgressIncrement(DeveloperExperienceLevel experience, UserStoryTaskDifficulty difficulty,
        Random random)
    {
        return experience switch
        {
            DeveloperExperienceLevel.Junior => difficulty switch
            {
                UserStoryTaskDifficulty.Easy => random.Next(40, 60),
                UserStoryTaskDifficulty.Medium => random.Next(20, 40),
                UserStoryTaskDifficulty.Hard => random.Next(10, 20),
                _ => 30
            },
            DeveloperExperienceLevel.MidLevel => difficulty switch
            {
                UserStoryTaskDifficulty.Easy => random.Next(60, 80),
                UserStoryTaskDifficulty.Medium => random.Next(40, 60),
                UserStoryTaskDifficulty.Hard => random.Next(20, 40),
                _ => 50
            },
            DeveloperExperienceLevel.Senior => difficulty switch
            {
                UserStoryTaskDifficulty.Easy => random.Next(80, 100),
                UserStoryTaskDifficulty.Medium => random.Next(60, 80),
                UserStoryTaskDifficulty.Hard => random.Next(40, 60),
                _ => 70
            },
            _ => 30
        };
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

    #region Task Methods

    public void AddAndAssignTaskToUserStory()
    {
        var story = UserStories.FirstOrDefault(us => us.Title == SelectedUserStory);
        var developer = Team.FirstOrDefault(d => d.Name == SelectedDeveloper);

        if (story == null || developer == null) return;

        var newTask = new UserStoryTask
        {
            Title = Task,
            Type = SelectedTaskType,
            Difficulty = UserStoryTaskDifficulty.Medium,
            AssignedTo = developer,
            IsCompleted = false,
            UserStoryId = story.Id,
            AssignedToId = developer.Id,
            UserStory = story
        };
        
        story.Tasks.Add(newTask);

        Task = "";
        SelectedDeveloper = "";
        SelectedUserStory = "";
    }

    public bool CanAddTaskToUserStory()
    {
        return string.IsNullOrEmpty(Task) ||
               string.IsNullOrEmpty(SelectedUserStory) ||
               string.IsNullOrEmpty(SelectedDeveloper);
    }

    #endregion

    public void UpdateBudget(int amount)
    {
        if (CurrentProject != null)
            CurrentProject.Budget += amount;
    }
}