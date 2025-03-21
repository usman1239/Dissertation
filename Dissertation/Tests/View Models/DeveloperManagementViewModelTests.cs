using System.Collections.ObjectModel;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;
using Dissertation.View_Models;
using Moq;
using MudBlazor;
using Xunit;

namespace Dissertation.Tests.View_Models;

public class DeveloperManagementViewModelTests
{
    private readonly Mock<IDeveloperService> _mockDeveloperService;
    private readonly Mock<ISnackbar> _mockSnackbar;
    private readonly ProjectStateService _projectStateService;
    private readonly DeveloperManagementViewModel _viewModel;

    public DeveloperManagementViewModelTests()
    {
        _mockDeveloperService = new Mock<IDeveloperService>();
        _mockSnackbar = new Mock<ISnackbar>();

        _projectStateService = new ProjectStateService
        {
            Team = [],
            UserStoryInstances = []
        };

        _viewModel = new DeveloperManagementViewModel(
            _projectStateService,
            _mockDeveloperService.Object,
            _mockSnackbar.Object);
    }

    [Fact]
    public void CanAddDeveloper_ShouldReturnFalse_WhenDeveloperNameIsEmpty()
    {
        // Act
        var result = _viewModel.CanAddDeveloper();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddDeveloper_ShouldShowWarning_WhenNameIsEmpty()
    {
        // Arrange
        _viewModel.DeveloperName = "";

        // Act
        await _viewModel.AddDeveloper();


        // Assert
        _mockSnackbar.Verify(s =>
                s.Add(
                    "Developer name cannot be empty!",
                    Severity.Warning,
                    null,
                    null
                ),
            Times.Once);
    }

    [Fact]
    public async Task AddDeveloper_ShouldShowWarning_WhenDeveloperAlreadyExists()
    {
        // Arrange
        _viewModel.DeveloperName = "John Doe";
        _projectStateService.Team.Add(new Developer { Name = "John Doe" });

        // Act
        await _viewModel.AddDeveloper();

        // Assert
        _mockSnackbar.Verify(
            s => s.Add("A developer named John Doe already exists in your team!", Severity.Warning, null, null),
            Times.Once);
    }

    [Fact]
    public async Task AddDeveloper_ShouldAddDeveloper_WhenValid()
    {
        // Arrange
        _viewModel.DeveloperName = "Alice";
        _viewModel.SelectedDeveloperExperienceLevel = DeveloperExperienceLevel.MidLevel;

        _projectStateService.CurrentProjectInstance = new ProjectInstance
        {
            Project = new Project
            {
                DeveloperCosts = new Dictionary<DeveloperExperienceLevel, int>
                {
                    { DeveloperExperienceLevel.MidLevel, 5000 }
                }
            }
        };

        var teamList = new ObservableCollection<Developer>();
        _projectStateService.Team = teamList;

        // Act
        await _viewModel.AddDeveloper();

        // Assert
        Assert.Single(teamList);
        Assert.Equal("Alice", teamList[0].Name);
        Assert.Equal(5000, teamList[0].Cost);
        _mockSnackbar.Verify(s => s.Add("Developer Alice added successfully!", Severity.Success, null, null),
            Times.Once);
    }


    [Fact]
    public async Task AddDeveloper_ShouldSetCorrectCost_BasedOnExperienceLevel()
    {
        //Arrange
        _projectStateService.CurrentProjectInstance = new ProjectInstance
        {
            Project = new Project
            {
                DeveloperCosts = new Dictionary<DeveloperExperienceLevel, int>
                {
                    { DeveloperExperienceLevel.Junior, 2000 },
                    { DeveloperExperienceLevel.MidLevel, 5000 },
                    { DeveloperExperienceLevel.Senior, 8000 }
                }
            }
        };


        var expectedCosts = new Dictionary<DeveloperExperienceLevel, int>
        {
            { DeveloperExperienceLevel.Junior, 2000 },
            { DeveloperExperienceLevel.MidLevel, 5000 },
            { DeveloperExperienceLevel.Senior, 8000 }
        };

        foreach (var (experienceLevel, expectedCost) in expectedCosts)
        {
            _viewModel.DeveloperName = "Bob";
            _viewModel.SelectedDeveloperExperienceLevel = experienceLevel;
            var teamList = new ObservableCollection<Developer>();
            _projectStateService.Team = teamList;

            // Act
            await _viewModel.AddDeveloper();

            // Assert
            Assert.Single(teamList);
            Assert.Equal(expectedCost, teamList[0].Cost);
        }
    }

    [Fact]
    public async Task RemoveDeveloper_ShouldRemoveDeveloper_FromProjectStateService()
    {
        // Arrange
        var developer = new Developer { Id = 1, Name = "Charlie" };
        var teamList = new ObservableCollection<Developer> { developer };
        _projectStateService.Team = teamList;

        // Act
        await _viewModel.RemoveDeveloper(developer);

        // Assert
        Assert.Empty(teamList);
        _mockDeveloperService.Verify(ds => ds.RemoveDeveloperAsync(developer), Times.Once);
    }

    [Fact]
    public async Task RemoveDeveloper_ShouldUnassignDeveloper_FromUserStories()
    {
        // Arrange
        var developer = new Developer { Id = 1, Name = "Charlie" };
        var userStory = new UserStoryInstance { DeveloperAssignedId = 1 };
        var teamList = new ObservableCollection<Developer> { developer };
        var stories = new ObservableCollection<UserStoryInstance> { userStory };

        _projectStateService.Team = teamList;
        _projectStateService.UserStoryInstances = stories;

        // Act
        await _viewModel.RemoveDeveloper(developer);

        // Assert
        Assert.Null(userStory.DeveloperAssignedId);
        _mockDeveloperService.Verify(ds => ds.RemoveDeveloperAsync(developer), Times.Once);
    }
}