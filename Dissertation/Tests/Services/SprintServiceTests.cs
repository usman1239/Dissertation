using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dissertation.Tests.Services;

public class SprintServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly SprintService _sprintService;

    public SprintServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        _sprintService = new SprintService(_dbContext);
    }

    [Fact]
    public async Task GetSprintsForProjectAsync_ShouldReturnCorrectSprints()
    {
        // Arrange
        var projectInstance = new ProjectInstance { Id = 1, UserId = "user1", ProjectId = 1 };
        var sprint1 = new Sprint { SprintNumber = 1, ProjectInstanceId = 1 };
        var sprint2 = new Sprint { SprintNumber = 2, ProjectInstanceId = 1 };
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.Sprints.AddAsync(sprint1);
        await _dbContext.Sprints.AddAsync(sprint2);
        await _dbContext.SaveChangesAsync();

        // Act
        var sprints = await _sprintService.GetSprintsForProjectAsync(1);

        // Assert
        Assert.NotNull(sprints);
        Assert.Equal(2, sprints.Count);
        Assert.Equal(1, sprints.First().SprintNumber);
        Assert.Equal(2, sprints.Last().SprintNumber);
    }

    [Fact]
    public async Task SaveSprintAsync_ShouldAddSprintSuccessfully()
    {
        // Arrange
        var projectInstance = new ProjectInstance { Id = 1, UserId = "user1", ProjectId = 1 };
        var sprint = new Sprint { SprintNumber = 1, ProjectInstanceId = 1, ProjectInstance = projectInstance };
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sprintService.SaveSprintAsync(sprint);

        // Assert
        var savedSprint = await _dbContext.Sprints.FindAsync(sprint.SprintNumber);
        Assert.NotNull(savedSprint);
        Assert.Equal(1, savedSprint.SprintNumber);
        Assert.Equal(1, savedSprint.ProjectInstanceId);
    }

    [Fact]
    public async Task SaveSprintAsync_ShouldHandleInvalidProjectInstance()
    {
        // Arrange
        var sprint = new Sprint { SprintNumber = 1, ProjectInstanceId = 999 };

        // Act
        await _sprintService.SaveSprintAsync(sprint);

        // Assert
        var savedSprint = await _dbContext.Sprints.FindAsync(sprint.SprintNumber);
        Assert.Null(savedSprint);
    }
}