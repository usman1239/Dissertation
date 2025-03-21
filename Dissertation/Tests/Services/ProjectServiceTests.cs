using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dissertation.Tests.Services;

public class ProjectServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _projectService = new ProjectService(_dbContext);
    }

    [Fact]
    public async Task GetAvailableProjectsAsync_ShouldReturnProjectsOrderedByBudget()
    {
        // Arrange
        var project1 = new Project { Id = 1, Budget = 1000 };
        var project2 = new Project { Id = 2, Budget = 2000 };
        await _dbContext.Projects.AddAsync(project1);
        await _dbContext.Projects.AddAsync(project2);
        await _dbContext.SaveChangesAsync();

        // Act
        var projects = await _projectService.GetAvailableProjectsAsync();

        // Assert
        Assert.Equal(2, projects.Count);
        Assert.Equal(2000, projects.First().Budget);
    }

    [Fact]
    public async Task LoadProjectsWithSavedProgressAsync_ShouldReturnSavedProjectsForUser()
    {
        // Arrange
        const string userId = "user1";
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance { ProjectId = 1, UserId = userId };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        var projectInstances = await _projectService.LoadProjectsWithSavedProgressAsync(userId);

        // Assert
        Assert.NotEmpty(projectInstances);
        Assert.Equal(userId, projectInstances.First().UserId);
        Assert.Equal(1, projectInstances.First().ProjectId);
    }

    [Fact]
    public async Task GetProjectInstanceAsync_ShouldReturnProjectInstance_WhenExists()
    {
        // Arrange
        const string userId = "user1";
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance
        {
            ProjectId = 1,
            UserId = userId,
            Sprints = new List<Sprint> { new() { Id = 1 } },
            UserStoryInstances = new List<UserStoryInstance> { new() { UserStoryId = 1 } }
        };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        var fetchedInstance = await _projectService.GetProjectInstanceAsync(1, userId);

        // Assert
        Assert.NotNull(fetchedInstance);
        Assert.Equal(userId, fetchedInstance.UserId);
        Assert.Equal(1, fetchedInstance.ProjectId);
    }


    [Fact]
    public async Task SaveNewProjectInstance_ShouldSaveWhenNotExist()
    {
        // Arrange
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance { ProjectId = 1, UserId = "user1" };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        // Act
        await _projectService.SaveNewProjectInstance(projectInstance);

        // Assert
        var savedInstance =
            await _dbContext.ProjectInstances.FindAsync(projectInstance.Id);
        Assert.NotNull(savedInstance);
    }

    [Fact]
    public async Task SaveNewProjectInstance_ShouldNotSaveWhenExists()
    {
        // Arrange
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance { ProjectId = 1, UserId = "user1" };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        await _projectService.SaveNewProjectInstance(projectInstance);

        // Assert
        var instancesCount = await _dbContext.ProjectInstances.CountAsync();
        Assert.Equal(1, instancesCount);
    }


    [Fact]
    public async Task DeleteSavedProjectInstanceAsync_ShouldReturnTrue_WhenInstanceExists()
    {
        // Arrange
        const string userId = "user1";
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance { ProjectId = 1, UserId = userId };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _projectService.DeleteSavedProjectInstanceAsync(1, userId);

        // Assert
        Assert.True(result);
        var deletedInstance = await _dbContext.ProjectInstances.FindAsync(projectInstance.Id);
        Assert.Null(deletedInstance);
    }

    [Fact]
    public async Task DeleteSavedProjectInstanceAsync_ShouldReturnFalse_WhenInstanceDoesNotExist()
    {
        // Act
        var result = await _projectService.DeleteSavedProjectInstanceAsync(1, "user1");

        // Assert
        Assert.False(result);
    }
}