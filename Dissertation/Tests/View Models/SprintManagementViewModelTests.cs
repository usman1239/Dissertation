using System.Collections.ObjectModel;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;
using Dissertation.View_Models;
using Moq;
using MudBlazor;
using NuGet.Packaging;
using Xunit;

namespace Dissertation.Tests.View_Models;

public class SprintManagementViewModelTests
{
    private readonly Mock<IDeveloperService> _mockDeveloperService;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<ISnackbar> _mockSnackbar;
    private readonly Mock<ISprintService> _mockSprintService;
    private readonly Mock<IUserStoryService> _mockUserStoryService;
    private readonly ProjectStateService _projectStateService;
    private readonly SprintManagementViewModel _viewModel;

    public SprintManagementViewModelTests()
    {
        _mockSprintService = new Mock<ISprintService>();
        _mockUserStoryService = new Mock<IUserStoryService>();
        _mockDeveloperService = new Mock<IDeveloperService>();
        _mockSnackbar = new Mock<ISnackbar>();
        _mockNavigationService = new Mock<INavigationService>();
        Mock<IBadgeService> mockBadgeService = new();

        _projectStateService = new ProjectStateService
        {
            CurrentProjectInstance = new ProjectInstance
            {
                Project = new Project { NumOfSprints = 3, Budget = 10000 },
                UserStoryInstances = new ObservableCollection<UserStoryInstance>(),
                Sprints = new ObservableCollection<Sprint>()
            }
        };

        _viewModel = new SprintManagementViewModel(
            _projectStateService,
            _mockSprintService.Object,
            _mockUserStoryService.Object,
            _mockDeveloperService.Object,
            mockBadgeService.Object,
            _mockSnackbar.Object,
            _mockNavigationService.Object
        );
    }

    [Fact]
    public void CanStartSprint_ShouldReturnFalse_WhenNoTeamMembers()
    {
        _projectStateService.Team.Add(new Developer { Cost = 500 });
        _projectStateService.CurrentProjectInstance.Budget = 10000;
        _projectStateService.Team.Clear();
        Assert.False(_viewModel.CanStartSprint());
    }

    [Fact]
    public void CanStartSprint_ShouldReturnFalse_WhenBudgetIsInsufficient()
    {
        _projectStateService.Team.Add(new Developer { Cost = 500 });
        _projectStateService.CurrentProjectInstance.Budget = 100;
        Assert.False(_viewModel.CanStartSprint());
    }

    [Fact]
    public void CanStartSprint_ShouldReturnTrue_WhenAllConditionsAreMet()
    {
        _projectStateService.Team.Add(new Developer
        {
            Id = 1,
            Cost = 500,
            IsSick = false,
            IsPermanentlyAbsent = false
        });

        _projectStateService.CurrentProjectInstance.Budget = 10000;
        _projectStateService.CurrentProjectInstance.Project = new Project
        {
            NumOfSprints = 3
        };
        _projectStateService.Sprints.Clear();

        _projectStateService.UserStoryInstances.Add(new UserStoryInstance
        {
            DeveloperAssignedId = 1,
            IsComplete = false
        });

        Assert.True(_viewModel.CanStartSprint());
    }

    [Fact]
    public async Task StartSprint_ShouldUpdateBudgetAndSaveProgress()
    {
        _projectStateService.Team.Add(new Developer { Cost = 500 });
        _projectStateService.CurrentProjectInstance.Budget = 10000;

        _mockSprintService
            .Setup(s => s.GetSprintsForProjectAsync(It.IsAny<int>()))
            .ReturnsAsync([
                new Sprint { Id = 1, IsCompleted = true },
                new Sprint { Id = 2, IsCompleted = false }
            ]);

        await _viewModel.StartSprint();

        _mockSnackbar.Verify(s => s.Add("Sprint started successfully!", Severity.Success, null, null), Times.Once);
        Assert.Equal(9500, _projectStateService.CurrentProjectInstance.Budget);
        Assert.Equal(3, _projectStateService.Sprints.LastOrDefault()!.SprintNumber);
    }

    [Fact]
    public async Task StartSprint_ShouldRecoverSickDevelopers()
    {
        var developer = new Developer { Id = 1, IsSick = true, SickUntilSprint = 1 };
        _projectStateService.Team.Add(developer);
        _projectStateService.Sprints.AddRange(
        [
            new Sprint { IsCompleted = true },
            new Sprint { IsCompleted = true }
        ]);

        await _viewModel.RecoverSickDevelopers();

        Assert.False(developer.IsSick);
        _mockDeveloperService.Verify(d => d.UpdateDeveloperAbsence(developer), Times.Once);
    }

    [Fact]
    public void ShowSummaryOrSprints_ShouldNavigateToSummary_WhenAllSprintsCompleted()
    {
        _projectStateService.UserStoryInstances.Add(new UserStoryInstance { IsComplete = true });
        _projectStateService.CurrentProjectInstance.Project.NumOfSprints = 1;
        _projectStateService.Sprints.Add(new Sprint { IsCompleted = true });

        _ = _viewModel.ShowSummaryOrSprints();

        _mockSnackbar.Verify(s => s.Add("All sprints completed!", Severity.Success, null, null), Times.Once);
        _mockNavigationService.Verify(n => n.NavigateTo("/challenge/summary", false), Times.Once);
    }

    [Fact]
    public void ShowSummaryOrSprints_ShouldNavigateToSprints_WhenSprintsRemain()
    {
        _projectStateService.UserStoryInstances.Add(new UserStoryInstance { IsComplete = false });
        _projectStateService.CurrentProjectInstance.Project.NumOfSprints = 2;
        _projectStateService.Sprints.Add(new Sprint { IsCompleted = true });

        _ = _viewModel.ShowSummaryOrSprints();

        _mockNavigationService.Verify(n => n.NavigateTo("/challenge/sprints", false), Times.Once);
    }

    [Fact]
    public void LoadSprintProgressAsync_ShouldUpdateProgressLists()
    {
        _projectStateService.UserStoryInstances.Add(new UserStoryInstance
            { UserStory = new UserStory { StoryPoints = 100 } });
        _projectStateService.CurrentProjectInstance.Project.NumOfSprints = 2;

        _viewModel.LoadSprintProgressAsync();

        Assert.NotEmpty(_viewModel.SprintProgress);
        Assert.NotEmpty(_viewModel.ExpectedProgress);
    }

    [Fact]
    public void UpdateBudget_ShouldAdjustBudgetCorrectly()
    {
        _projectStateService.CurrentProjectInstance.Budget = 5000;

        _viewModel.UpdateBudget(2000);

        Assert.Equal(7000, _projectStateService.CurrentProjectInstance.Budget);
    }

    [Theory]
    [InlineData(DeveloperExperienceLevel.Junior, 20, 35)]
    [InlineData(DeveloperExperienceLevel.MidLevel, 40, 55)]
    [InlineData(DeveloperExperienceLevel.Senior, 60, 75)]
    public void CalculateDeveloperProgress_ShouldReturnCorrectValues(DeveloperExperienceLevel experienceLevel,
        int minProgress, int maxProgress)
    {
        var dev = new Developer { ExperienceLevel = experienceLevel };
        var progress = _viewModel.CalculateDeveloperProgress(dev, 0, new Random());

        Assert.InRange(progress, minProgress, maxProgress);
    }

    [Fact]
    public async Task HandleNewRandomUserStory_ShouldAddNewStory()
    {
        _mockUserStoryService.Setup(us => us.TriggerRandomUserStoryEventAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        _mockUserStoryService.Setup(us => us.GetUserStoryInstancesForProjectAsync(It.IsAny<int>()))
            .ReturnsAsync([new UserStoryInstance()]);

        await _viewModel.HandleNewRandomUserStory();

        Assert.NotEmpty(_projectStateService.UserStoryInstances);
        _mockSnackbar.Verify(s => s.Add("A new user story has been added!", Severity.Info, null, null), Times.Once);
    }

    [Fact]
    public async Task HandleSickOrAbsentDeveloperEvent_ShouldMakeDeveloperSick()
    {
        var randomMock = new Mock<Random>();
        randomMock.SetupSequence(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(0)
            .Returns(1);

        var developer = new Developer { Id = 1, Name = "Alice", IsSick = false, IsPermanentlyAbsent = false };
        _projectStateService.Team.Add(developer);

        const int completedSprintsCount = 3;

        _mockDeveloperService.Setup(ds => ds.UpdateDeveloperAbsence(It.IsAny<Developer>())).Returns(Task.CompletedTask);

        await _viewModel.HandleSickOrAbsentDeveloperEvent(randomMock.Object, completedSprintsCount);

        Assert.True(developer.IsSick);
        Assert.Equal(completedSprintsCount + 1, developer.SickUntilSprint);
        Assert.False(developer.IsPermanentlyAbsent);

        _mockSnackbar.Verify(
            s => s.Add(
                $"{developer.Name} is sick and will miss until sprint ${completedSprintsCount + 1}",
                Severity.Warning, null, null),
            Times.Once);

        _mockDeveloperService.Verify(ds => ds.UpdateDeveloperAbsence(developer), Times.Once);
    }

    [Fact]
    public async Task HandleSickOrAbsentDeveloperEvent_ShouldMakeDeveloperPermanentlyAbsent()
    {
        var randomMock = new Mock<Random>();
        randomMock.Setup(r => r.Next(0, 2)).Returns(1);

        var developer = new Developer { Id = 1, Name = "Bob", IsSick = false, IsPermanentlyAbsent = false };
        _projectStateService.Team.Add(developer);

        const int completedSprintsCount = 3;

        _mockDeveloperService.Setup(ds => ds.UpdateDeveloperAbsence(It.IsAny<Developer>())).Returns(Task.CompletedTask);

        await _viewModel.HandleSickOrAbsentDeveloperEvent(randomMock.Object, completedSprintsCount);

        Assert.True(developer.IsPermanentlyAbsent);
        Assert.False(developer.IsSick);
        Assert.Equal(0, developer.SickUntilSprint);
        _mockSnackbar.Verify(s => s.Add($"{developer.Name} is permanently absent!", Severity.Warning, null, null),
            Times.Once);
        _mockDeveloperService.Verify(ds => ds.UpdateDeveloperAbsence(developer), Times.Once);
    }


    [Fact]
    public void CanShowSummary_ShouldReturnTrue_WhenAllUserStoriesAreCompleted()
    {
        var projectInstance = new ProjectInstance
        {
            UserStoryInstances =
            [
                new UserStoryInstance { IsComplete = true },
                new UserStoryInstance { IsComplete = true }
            ],
            Sprints =
            [
                new Sprint { IsCompleted = false },
                new Sprint { IsCompleted = false }
            ],
            Project = new Project { NumOfSprints = 2 }
        };

        _projectStateService.CurrentProjectInstance = projectInstance;

        var result = _viewModel.CanShowSummary();

        Assert.True(result);
    }

    [Fact]
    public void CanShowSummary_ShouldReturnTrue_WhenAllSprintsAreCompleted()
    {
        var projectInstance = new ProjectInstance
        {
            UserStoryInstances =
            [
                new UserStoryInstance { IsComplete = false },
                new UserStoryInstance { IsComplete = false }
            ],
            Sprints =
            [
                new Sprint { IsCompleted = true },
                new Sprint { IsCompleted = true }
            ],
            Project = new Project { NumOfSprints = 2 }
        };

        _projectStateService.CurrentProjectInstance = projectInstance;

        var result = _viewModel.CanShowSummary();

        Assert.True(result);
    }


    [Fact]
    public void CanShowSummary_ShouldReturnFalse_WhenNotAllUserStoriesOrSprintsAreCompleted()
    {
        var projectInstance = new ProjectInstance
        {
            UserStoryInstances =
            [
                new UserStoryInstance { IsComplete = false },
                new UserStoryInstance { IsComplete = false }
            ],
            Sprints =
            [
                new Sprint { IsCompleted = false },
                new Sprint { IsCompleted = false }
            ],
            Project = new Project { NumOfSprints = 2 }
        };

        _projectStateService.CurrentProjectInstance = projectInstance;

        var result = _viewModel.CanShowSummary();

        Assert.False(result);
    }

    [Fact]
    public void CanShowSummary_ShouldReturnTrue_WhenSomeUserStoriesAndSprintsAreCompleted()
    {
        var projectInstance = new ProjectInstance
        {
            UserStoryInstances =
            [
                new UserStoryInstance { IsComplete = true },
                new UserStoryInstance { IsComplete = false }
            ],
            Sprints =
            [
                new Sprint { IsCompleted = true },
                new Sprint { IsCompleted = true }
            ],
            Project = new Project { NumOfSprints = 2 }
        };

        _projectStateService.CurrentProjectInstance = projectInstance;

        var result = _viewModel.CanShowSummary();

        Assert.True(result);
    }


    [Fact]
    public void UpdateStoryProgress_ShouldReturnZero_WhenTotalStoryPointsIsZero()
    {
        var stories = new List<UserStoryInstance>
        {
            new() { UserStory = new UserStory { StoryPoints = 0 } }
        };
        const int totalProgressIncrease = 100;

        var revenue = _viewModel.UpdateStoryProgress(stories, totalProgressIncrease);

        Assert.Equal(0, revenue);
    }

    [Fact]
    public void UpdateStoryProgress_ShouldSkipSickDeveloper()
    {
        var developer = new Developer { Id = 1, IsSick = true };
        var stories = new List<UserStoryInstance>
        {
            new()
            {
                UserStory = new UserStory { StoryPoints = 10 },
                DeveloperAssignedId = 1,
                Progress = 50
            }
        };
        var totalProgressIncrease = 100;

        _projectStateService.Team = [developer];

        var revenue = _viewModel.UpdateStoryProgress(stories, totalProgressIncrease);

        Assert.Equal(0, revenue);
        Assert.Equal(50, stories[0].Progress);
    }

    [Fact]
    public void UpdateStoryProgress_ShouldSkipPermanentlyAbsentDeveloper()
    {
        var developer = new Developer { Id = 1, IsPermanentlyAbsent = true };
        var stories = new List<UserStoryInstance>
        {
            new()
            {
                UserStory = new UserStory { StoryPoints = 10 },
                DeveloperAssignedId = 1,
                Progress = 50
            }
        };
        const int totalProgressIncrease = 100;
        _projectStateService.Team = [developer];

        var revenue = _viewModel.UpdateStoryProgress(stories, totalProgressIncrease);

        Assert.Equal(0, revenue);
        Assert.Equal(50, stories[0].Progress);
    }

    [Fact]
    public void UpdateStoryProgress_ShouldUpdateProgressBasedOnStoryPoints()
    {
        var stories = new List<UserStoryInstance>
        {
            new() { UserStory = new UserStory { StoryPoints = 10 }, Progress = 50, DeveloperAssignedId = 1 }
        };
        const int totalProgressIncrease = 30;
        var developer = new Developer { Id = 1, IsSick = false, IsPermanentlyAbsent = false };
        _projectStateService.Team = [developer];

        var revenue = _viewModel.UpdateStoryProgress(stories, totalProgressIncrease);

        Assert.Equal(80, stories[0].Progress);
        Assert.Equal(0, revenue);
    }


    [Fact]
    public void UpdateStoryProgress_ShouldCompleteStoryAndAddRevenue()
    {
        var stories = new List<UserStoryInstance>
        {
            new() { UserStory = new UserStory { StoryPoints = 10 }, Progress = 90, DeveloperAssignedId = 1 }
        };
        const int totalProgressIncrease = 15;

        var developer = new Developer { Id = 1, IsSick = false, IsPermanentlyAbsent = false };
        _projectStateService.Team = [developer];

        var revenue = _viewModel.UpdateStoryProgress(stories, totalProgressIncrease);

        Assert.Equal(100, stories[0].Progress);
        Assert.True(stories[0].IsComplete);
        Assert.Equal(5000, revenue);
    }


    [Fact]
    public void GetDeveloperAssignments_ShouldIncludeDeveloper_WhenAvailable()
    {
        var developer = new Developer
        {
            Id = 1,
            IsSick = false,
            IsPermanentlyAbsent = false,
            SickUntilSprint = 0
        };

        var userStoryInstance = new UserStoryInstance
        {
            DeveloperAssignedId = 1,
            IsComplete = false
        };

        _projectStateService.Team = [developer];
        _projectStateService.UserStoryInstances = [userStoryInstance];
        _projectStateService.Sprints = [];

        var developerAssignments = _viewModel.GetDeveloperAssignments();

        Assert.Single(developerAssignments);
        Assert.Contains(1, developerAssignments.Keys);
    }

    [Fact]
    public void GetDeveloperAssignments_ShouldExcludeSickDeveloper()
    {
        var developer = new Developer
        {
            Id = 1,
            IsSick = true,
            IsPermanentlyAbsent = false,
            SickUntilSprint = 0
        };

        var userStoryInstance = new UserStoryInstance
        {
            DeveloperAssignedId = 1,
            IsComplete = false
        };

        _projectStateService.Team = [developer];
        _projectStateService.UserStoryInstances = [userStoryInstance];
        _projectStateService.Sprints = [];

        var developerAssignments = _viewModel.GetDeveloperAssignments();

        Assert.Empty(developerAssignments);
    }

    [Fact]
    public void GetDeveloperAssignments_ShouldExcludePermanentlyAbsentDeveloper()
    {
        var developer = new Developer
        {
            Id = 1,
            IsSick = false,
            IsPermanentlyAbsent = true,
            SickUntilSprint = 0
        };

        var userStoryInstance = new UserStoryInstance
        {
            DeveloperAssignedId = 1,
            IsComplete = false
        };

        _projectStateService.Team = [developer];
        _projectStateService.UserStoryInstances = [userStoryInstance];
        _projectStateService.Sprints = [];

        var developerAssignments = _viewModel.GetDeveloperAssignments();

        Assert.Empty(developerAssignments);
    }


    [Fact]
    public void GetDeveloperAssignments_ShouldExcludeDeveloperIfSickUntilSprint()
    {
        var developer = new Developer
        {
            Id = 1,
            IsPermanentlyAbsent = false,
            SickUntilSprint = 2
        };

        var userStoryInstance = new UserStoryInstance
        {
            DeveloperAssignedId = 1,
            IsComplete = false
        };

        _projectStateService.Team = [developer];
        _projectStateService.UserStoryInstances = [userStoryInstance];
        _projectStateService.Sprints =
        [
            new Sprint(),
            new Sprint(),
            new Sprint()
        ];

        var developerAssignments = _viewModel.GetDeveloperAssignments();

        Assert.Empty(developerAssignments);
    }

    [Fact]
    public void GetDeveloperAssignments_ShouldHandleMultipleDevelopersCorrectly()
    {
        var developer1 = new Developer
        {
            Id = 1,
            IsSick = false,
            IsPermanentlyAbsent = false,
            SickUntilSprint = 0
        };

        var developer2 = new Developer
        {
            Id = 2,
            IsSick = true,
            IsPermanentlyAbsent = false,
            SickUntilSprint = 0
        };

        var userStoryInstance1 = new UserStoryInstance
        {
            DeveloperAssignedId = 1,
            IsComplete = false
        };

        var userStoryInstance2 = new UserStoryInstance
        {
            DeveloperAssignedId = 2,
            IsComplete = false
        };

        _projectStateService.Team = [developer1, developer2];
        _projectStateService.UserStoryInstances = [userStoryInstance1, userStoryInstance2];
        _projectStateService.Sprints =
        [
            new Sprint(),
            new Sprint(),
            new Sprint()
        ];

        var developerAssignments = _viewModel.GetDeveloperAssignments();

        Assert.Single(developerAssignments);
        Assert.Contains(1, developerAssignments.Keys);
        Assert.DoesNotContain(2, developerAssignments.Keys);
    }


    [Fact]
    public void HandleTeamMoraleBoost_ShouldApplyMoraleBoostToTwoEligibleDevelopers()
    {
        var dev1 = new Developer { Name = "Alice", IsSick = false, IsPermanentlyAbsent = false };
        var dev2 = new Developer { Name = "Bob", IsSick = false, IsPermanentlyAbsent = false };
        var dev3 = new Developer { Name = "Charlie", IsSick = true, IsPermanentlyAbsent = false };
        _projectStateService.Team.Add(dev1);
        _projectStateService.Team.Add(dev2);
        _projectStateService.Team.Add(dev3);

        _viewModel.HandleTeamMoraleBoost();

        var boostedDevs = _projectStateService.Team.Where(d => d.MoraleBoost == 25).ToList();
        Assert.True(boostedDevs.Count <= 2);
        Assert.All(boostedDevs, d => Assert.False(d.IsSick));
        _mockSnackbar.Verify(s => s.Add(
                "✨ Morale boost! Some developers will be 25% more productive this sprint.",
                Severity.Success,
                null,
                null),
            Times.Once);
    }

    [Fact]
    public void HandleTeamMoraleBoost_ShouldDoNothing_WhenNoEligibleDevelopers()
    {
        var dev = new Developer { IsSick = true, IsPermanentlyAbsent = true };
        _projectStateService.Team.Add(dev);

        _viewModel.HandleTeamMoraleBoost();

        Assert.Equal(0, _projectStateService.Team.Count(d => d.MoraleBoost > 0));
        _mockSnackbar.Verify(s => s.Add(It.IsAny<string>(), It.IsAny<Severity>(), null, null), Times.Never);
    }

    [Fact]
    public async Task HandleBugInjectionEvent_ShouldAddBugAndShowSnackbar()
    {
        var bug = new UserStoryInstance
        {
            UserStory = new UserStory { Title = "Critical bug" }
        };

        _mockUserStoryService.Setup(s =>
                s.CreateAndAssignBugToProjectAsync(_projectStateService.CurrentProjectInstance))
            .ReturnsAsync(bug);

        await _viewModel.HandleBugInjectionEvent();

        Assert.Contains(bug, _projectStateService.UserStoryInstances);
        _mockSnackbar.Verify(s => s.Add(
                $"🐞 A new bug appeared: {bug.UserStory.Title}",
                Severity.Error,
                null,
                null),
            Times.Once);
    }

    [Fact]
    public void HandleRandomBudgetCut_ShouldReduceBudgetAndShowSnackbar()
    {
        var originalBudget = 10000;
        _projectStateService.CurrentProjectInstance.Budget = originalBudget;

        _viewModel.HandleRandomBudgetCut();

        var newBudget = _projectStateService.CurrentProjectInstance.Budget;
        Assert.InRange(newBudget, 7000, 9000); // 10%-30% cut
        _mockSnackbar.Verify(s => s.Add(
                It.Is<string>(msg => msg.Contains("Budget cut!")),
                Severity.Warning,
                null,
                null),
            Times.Once);
    }
}