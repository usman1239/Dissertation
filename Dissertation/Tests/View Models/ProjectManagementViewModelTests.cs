﻿using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Dissertation.View_Models;
using Moq;
using MudBlazor;
using Xunit;

namespace Dissertation.Tests.View_Models;

public class ProjectManagementViewModelTests
{
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly Mock<ISnackbar> _mockSnackbar;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IUserStoryService> _mockUserStoryService;
    private readonly ProjectStateService _projectStateService;
    private readonly ProjectManagementViewModel _viewModel;

    public ProjectManagementViewModelTests()
    {
        _projectStateService = new ProjectStateService
        {
            Team = [],
            UserStoryInstances = [],
            Sprints = []
        };

        _mockProjectService = new Mock<IProjectService>();
        _mockUserService = new Mock<IUserService>();
        _mockUserStoryService = new Mock<IUserStoryService>();
        _mockSnackbar = new Mock<ISnackbar>();
        _mockNavigationService = new Mock<INavigationService>();

        _viewModel = new ProjectManagementViewModel(
            _projectStateService,
            _mockProjectService.Object,
            _mockUserService.Object,
            _mockUserStoryService.Object,
            _mockSnackbar.Object,
            _mockNavigationService.Object
        );
    }

    [Fact]
    public async Task GetUser_ShouldSetUserId()
    {
        // Arrange
        const string userId = "testUser";
        _mockUserService.Setup(us => us.GetUserIdAsync()).ReturnsAsync(userId);

        // Act
        await _viewModel.GetUser();

        // Assert
        Assert.Equal(userId, _projectStateService.UserId);
    }

    [Fact]
    public async Task LoadAvailableProjectsAsync_ShouldLoadProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = 1, Title = "Project 1" },
            new() { Id = 2, Title = "Project 2" }
        };

        _mockProjectService.Setup(ps => ps.GetAvailableProjectsAsync()).ReturnsAsync(projects);

        // Act
        await _viewModel.LoadAvailableProjectsAsync();

        // Assert
        Assert.Equal(2, _viewModel.AvailableProjects.Count);
        Assert.Equal("Project 1", _viewModel.AvailableProjects[0]?.Title);
        Assert.Equal("Project 2", _viewModel.AvailableProjects[1]?.Title);
    }

    [Fact]
    public async Task LoadProjectsWithSavedProgress_ShouldLoadSavedProjects()
    {
        // Arrange
        var savedProjects = new List<ProjectInstance>
        {
            new() { Id = 1, Project = new Project { Title = "Saved Project 1" } },
            new() { Id = 2, Project = new Project { Title = "Saved Project 2" } }
        };
        _mockProjectService.Setup(ps =>
            ps.LoadProjectsWithSavedProgressAsync(It.IsAny<string?>())).ReturnsAsync(savedProjects);

        // Act
        await _viewModel.LoadProjectsWithSavedProgress();

        // Assert
        Assert.Equal(2, _viewModel.SavedProjects.Count);
        Assert.Equal("Saved Project 1", _viewModel.SavedProjects[0].Project.Title);
        Assert.Equal("Saved Project 2", _viewModel.SavedProjects[1].Project.Title);
    }

    [Fact]
    public async Task DeleteSavedProjectInstanceAsync_ShouldDeleteProjectAndShowSuccessMessage()
    {
        // Arrange
        const int projectId = 1;

        var savedProjects = new List<ProjectInstance>
        {
            new() { Id = projectId, Project = new Project { Title = "Test Project" } }
        };
        _mockProjectService.Setup(ps => ps.LoadProjectsWithSavedProgressAsync(It.IsAny<string?>()))
            .ReturnsAsync(savedProjects);

        _mockProjectService.Setup(ps => ps.DeleteSavedProjectInstanceAsync(projectId, It.IsAny<string?>()))
            .ReturnsAsync(true);

        // Act
        await _viewModel.DeleteSavedProjectInstanceAsync(projectId);

        // Assert
        _mockSnackbar.Verify(s => s.Add("Project deleted successfully.", Severity.Success, null, null), Times.Once);

        _mockProjectService.Verify(ps => ps.LoadProjectsWithSavedProgressAsync(It.IsAny<string?>()), Times.Once);

        Assert.Single(_viewModel.SavedProjects);
        Assert.Equal("Test Project", _viewModel.SavedProjects[0].Project.Title);
    }

    [Fact]
    public async Task DeleteSavedProjectInstanceAsync_ShouldShowErrorMessage_WhenDeleteFails()
    {
        // Arrange
        const int projectId = 1;

        var savedProjects = new List<ProjectInstance>
        {
            new() { Id = projectId, Project = new Project { Title = "Test Project" } }
        };
        _mockProjectService.Setup(ps => ps.LoadProjectsWithSavedProgressAsync(It.IsAny<string?>()))
            .ReturnsAsync(savedProjects);

        _mockProjectService.Setup(ps => ps.DeleteSavedProjectInstanceAsync(projectId, It.IsAny<string?>()))
            .ReturnsAsync(false);

        // Act
        await _viewModel.DeleteSavedProjectInstanceAsync(projectId);

        // Assert
        _mockSnackbar.Verify(s => s.Add("Failed to delete the project.", Severity.Error, null, null), Times.Once);
    }

    [Fact]
    public async Task SelectProject_ShouldLoadExistingProjectInstance()
    {
        // Arrange
        const int projectId = 1;
        const bool isSavedProject = true;
        var project = new Project { Id = projectId };
        var existingProjectInstance = new ProjectInstance();

        var mockUserStories = new List<UserStory>
        {
            new() { Id = 1, Title = "User Story 1" },
            new() { Id = 2, Title = "User Story 2" }
        };

        _mockProjectService.Setup(ps => ps.GetProjectInstanceAsync(projectId, _projectStateService.UserId))
            .ReturnsAsync(existingProjectInstance);
        _mockProjectService.Setup(ps => ps.GetAvailableProjectsAsync()).ReturnsAsync([project]);

        _mockUserStoryService.Setup(us => us.GetInitialUserStoriesForProject(projectId))
            .ReturnsAsync(mockUserStories);

        // Act
        _viewModel.AvailableProjects.Add(project);
        await _viewModel.SelectProject(projectId, isSavedProject);

        _mockNavigationService.Verify(
            n => n.NavigateTo(It.Is<string>(s => s == "/challenge/dashboard"), It.Is<bool>(b => b == false)),
            Times.Once
        );

        Assert.Equal(_projectStateService.CurrentProjectInstance, existingProjectInstance);
    }

    [Fact]
    public async Task SelectProject_ShouldInitializeNewProjectInstance_WhenNoExistingInstance()
    {
        // Arrange
        const int projectId = 1;
        const bool isSavedProject = false;
        var project = new Project { Id = projectId };
        ProjectInstance? existingProjectInstance = null;

        var mockUserStories = new List<UserStory>
        {
            new() { Id = 1, Title = "User Story 1" },
            new() { Id = 2, Title = "User Story 2" }
        };

        _mockProjectService.Setup(ps => ps.GetProjectInstanceAsync(projectId, _projectStateService.UserId))
            .ReturnsAsync(existingProjectInstance);
        _mockProjectService.Setup(ps => ps.GetAvailableProjectsAsync()).ReturnsAsync([project]);

        _mockUserStoryService.Setup(us => us.GetInitialUserStoriesForProject(projectId))
            .ReturnsAsync(mockUserStories);

        // Act
        _viewModel.AvailableProjects.Add(project);
        await _viewModel.SelectProject(projectId, isSavedProject);

        // Assert
        _mockNavigationService.Verify(
            n => n.NavigateTo(It.Is<string>(s => s == "/challenge/dashboard"), It.Is<bool>(b => b == false)),
            Times.Once
        );

        _mockProjectService.Verify(ps => ps.SaveNewProjectInstance(It.IsAny<ProjectInstance>()), Times.Once);
    }

    [Fact]
    public async Task SelectProject_ShouldAlertUserOfExistingProjectInstance()
    {
        // Arrange
        const int projectId = 1;
        const bool isSavedProject = false;
        var project = new Project { Id = projectId };
        var existingProjectInstance = new ProjectInstance();

        _mockProjectService.Setup(ps => ps.GetProjectInstanceAsync(projectId, _projectStateService.UserId))
            .ReturnsAsync(existingProjectInstance);
        _mockProjectService.Setup(ps => ps.GetAvailableProjectsAsync()).ReturnsAsync([project]);


        // Act
        _viewModel.AvailableProjects.Add(project);
        await _viewModel.SelectProject(projectId, isSavedProject);

        // Assert
        _mockSnackbar.Verify(
            s => s.Add("You cannot select this project because an instance already exists.", Severity.Warning, null,
                null), Times.Once);
    }
}