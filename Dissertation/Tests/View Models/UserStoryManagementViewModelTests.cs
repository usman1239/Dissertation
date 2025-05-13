using Dissertation.Models.Challenge;
using Dissertation.View_Models;
using Xunit;

namespace Dissertation.Tests.View_Models;

public class UserStoryManagementViewModelTests
{
    private readonly ProjectStateService _projectStateService;
    private readonly UserStoryManagementViewModel _viewModel;

    public UserStoryManagementViewModelTests()
    {
        _projectStateService = new ProjectStateService
        {
            Team = [],
            UserStoryInstances = []
        };
        _viewModel = new UserStoryManagementViewModel(_projectStateService);
    }

    [Fact]
    public void AssignDeveloperToStory_ShouldClearDeveloper_WhenDeveloperIdIsNull()
    {
        // Arrange
        var story = new UserStoryInstance();
        var developerId = (int?)null;

        // Act
        _viewModel.AssignDeveloperToStory(story, developerId);

        // Assert
        Assert.Null(story.DeveloperAssignedId);
        Assert.Null(story.DeveloperAssigned);
    }

    [Fact]
    public void AssignDeveloperToStory_ShouldClearDeveloper_WhenDeveloperIdIsZero()
    {
        // Arrange
        var story = new UserStoryInstance();
        const int developerId = 0;

        // Act
        _viewModel.AssignDeveloperToStory(story, developerId);

        // Assert
        Assert.Null(story.DeveloperAssignedId);
        Assert.Null(story.DeveloperAssigned);
    }

    [Fact]
    public void AssignDeveloperToStory_ShouldNotAssignDeveloper_WhenDeveloperNotFound()
    {
        // Arrange
        var story = new UserStoryInstance();
        const int developerId = 1;
        _projectStateService.Team = [];

        // Act
        _viewModel.AssignDeveloperToStory(story, developerId);

        // Assert
        Assert.Null(story.DeveloperAssignedId);
        Assert.Null(story.DeveloperAssigned);
    }

    [Fact]
    public void AssignDeveloperToStory_ShouldAssignDeveloper_WhenDeveloperFound()
    {
        // Arrange
        var story = new UserStoryInstance();
        const int developerId = 1;
        var developer = new Developer { Id = 1, Name = "John Doe" };
        _projectStateService.Team = [developer];

        // Act
        _viewModel.AssignDeveloperToStory(story, developerId);

        // Assert
        Assert.Equal(developer.Id, story.DeveloperAssignedId);
        Assert.Equal(developer, story.DeveloperAssigned);
    }


    [Fact]
    public void IsDeveloperUnavailable_ShouldReturnFalse_WhenDeveloperNotFound()
    {
        // Arrange
        const int developerId = 999; // Not in team
        _projectStateService.Team = [];

        // Act
        var result = _viewModel.IsDeveloperUnavailable(developerId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDeveloperUnavailable_ShouldReturnTrue_WhenDeveloperIsSick()
    {
        // Arrange
        const int developerId = 1;
        var developer = new Developer { Id = developerId, IsSick = true };
        _projectStateService.Team = [developer];

        // Act
        var result = _viewModel.IsDeveloperUnavailable(developerId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDeveloperUnavailable_ShouldReturnTrue_WhenDeveloperIsPermanentlyAbsent()
    {
        // Arrange
        const int developerId = 2;
        var developer = new Developer { Id = developerId, IsPermanentlyAbsent = true };
        _projectStateService.Team = [developer];

        // Act
        var result = _viewModel.IsDeveloperUnavailable(developerId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDeveloperUnavailable_ShouldReturnFalse_WhenDeveloperIsAvailable()
    {
        // Arrange
        const int developerId = 3;
        var developer = new Developer { Id = developerId, IsSick = false, IsPermanentlyAbsent = false };
        _projectStateService.Team = [developer];

        // Act
        var result = _viewModel.IsDeveloperUnavailable(developerId);

        // Assert
        Assert.False(result);
    }
}