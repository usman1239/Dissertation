using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dissertation.Tests.Services;

public class UserStoryServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly UserStoryService _userStoryService;

    public UserStoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _userStoryService = new UserStoryService(_dbContext);
    }

    [Fact]
    public async Task SaveUserStoryInstancesAsync_ShouldAddNewUserStoryInstance()
    {
        // Arrange
        var projectInstance = new ProjectInstance { Id = 1, UserId = "user1" };
        var userStory = new UserStory { Id = 1, ProjectId = 1, IsRandomEvent = false };
        var userStoryInstance = new UserStoryInstance
        {
            UserStoryId = 1,
            UserStory = userStory,
            ProjectInstanceId = 1,
            ProjectInstance = projectInstance,
            DeveloperAssignedId = null
        };

        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.UserStories.AddAsync(userStory);
        await _dbContext.SaveChangesAsync();

        // Act
        await _userStoryService.SaveUserStoryInstancesAsync([userStoryInstance]);

        // Assert
        var savedInstance =
            await _dbContext.UserStoryInstances.FirstOrDefaultAsync(usi => usi.Id == userStoryInstance.Id);
        Assert.NotNull(savedInstance);
        Assert.Equal(1, savedInstance.UserStoryId);
    }

    [Fact]
    public async Task SaveUserStoryInstancesAsync_ShouldUpdateExistingUserStoryInstance()
    {
        // Arrange
        var projectInstance = new ProjectInstance { Id = 1, UserId = "user1" };
        var userStory = new UserStory { Id = 1, ProjectId = 1, IsRandomEvent = false };
        var dev = new Developer { Id = 2, UserId = "user1" };
        var existingInstance = new UserStoryInstance
        {
            Id = 1,
            UserStoryId = 1,
            ProjectInstanceId = 1,
            DeveloperAssigned = dev,
            DeveloperAssignedId = dev.Id
        };

        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.UserStories.AddAsync(userStory);
        await _dbContext.UserStoryInstances.AddAsync(existingInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        existingInstance.DeveloperAssignedId = 2;
        await _userStoryService.SaveUserStoryInstancesAsync([existingInstance]);

        // Assert
        var updatedInstance = await _dbContext.UserStoryInstances.FirstOrDefaultAsync(usi => usi.Id == 1);
        Assert.NotNull(updatedInstance);
        Assert.Equal(2, updatedInstance.DeveloperAssignedId);
    }

    [Fact]
    public async Task GetInitialUserStoriesForProject_ShouldReturnCorrectUserStories()
    {
        // Arrange
        var project = new Project { Id = 1, Budget = 1000 };
        var userStory1 = new UserStory { Id = 1, ProjectId = 1, IsRandomEvent = false };
        var userStory2 = new UserStory { Id = 2, ProjectId = 1, IsRandomEvent = false };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.UserStories.AddAsync(userStory1);
        await _dbContext.UserStories.AddAsync(userStory2);
        await _dbContext.SaveChangesAsync();

        // Act
        var userStories = await _userStoryService.GetInitialUserStoriesForProject(1);

        // Assert
        Assert.NotNull(userStories);
        Assert.Equal(2, userStories.Count);
        Assert.Contains(userStories, us => us.Id == 1);
        Assert.Contains(userStories, us => us.Id == 2);
    }

    [Fact]
    public async Task AttachProjectAndUserStories_ShouldAttachCorrectProjectAndUserStories()
    {
        // Arrange
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance { Id = 1, ProjectId = 1, UserId = "user1" };
        var userStory = new UserStory { Id = 1, ProjectId = 1, IsRandomEvent = false };
        var userStoryInstance = new UserStoryInstance { UserStoryId = 1, ProjectInstanceId = 1 };

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.UserStories.AddAsync(userStory);
        await _dbContext.UserStoryInstances.AddAsync(userStoryInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        await _userStoryService.AttachProjectAndUserStories(projectInstance);

        // Assert
        Assert.Equal(project, projectInstance.Project);
        Assert.Contains(userStoryInstance, projectInstance.UserStoryInstances);
    }

    [Fact]
    public async Task TriggerRandomUserStoryEventAsync_ShouldAssignRandomUserStory()
    {
        // Arrange
        var project = new Project { Id = 1, Budget = 1000 };
        var projectInstance = new ProjectInstance { Id = 1, ProjectId = 1, UserId = "user1" };
        var userStory1 = new UserStory { Id = 1, ProjectId = 1, IsRandomEvent = true };
        var userStory2 = new UserStory { Id = 2, ProjectId = 1, IsRandomEvent = true };
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.UserStories.AddAsync(userStory1);
        await _dbContext.UserStories.AddAsync(userStory2);
        await _dbContext.SaveChangesAsync();

        // Act
        await _userStoryService.TriggerRandomUserStoryEventAsync(1);

        // Assert
        var userStoryInstance =
            await _dbContext.UserStoryInstances.Include(userStoryInstance => userStoryInstance.UserStory)
                .FirstOrDefaultAsync(usi => usi.ProjectInstanceId == 1);
        Assert.NotNull(userStoryInstance);
        Assert.True(userStoryInstance.UserStory.IsRandomEvent);
    }

    [Fact]
    public async Task GetUserStoryInstancesForProjectAsync_ShouldReturnUserStoryInstancesForProject()
    {
        // Arrange
        var projectInstance = new ProjectInstance { Id = 1, UserId = "user1" };
        var userStory = new UserStory { Id = 1, ProjectId = 1, IsRandomEvent = false };
        var userStoryInstance = new UserStoryInstance
        {
            UserStoryId = 1,
            ProjectInstanceId = 1
        };

        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.UserStories.AddAsync(userStory);
        await _dbContext.UserStoryInstances.AddAsync(userStoryInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        var userStoryInstances = await _userStoryService.GetUserStoryInstancesForProjectAsync(1);

        // Assert
        Assert.NotNull(userStoryInstances);
        Assert.Contains(userStoryInstances, usi => usi.UserStoryId == 1);
    }

    [Fact]
    public async Task CreateAndAssignBugToProjectAsync_ShouldCreateBugUserStoryAndInstance()
    {
        // Arrange
        var project = new Project { Id = 1, Title = "Test Project", Budget = 1000 };
        var projectInstance = new ProjectInstance { Id = 1, ProjectId = 1, UserId = "user1", Project = project };

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.ProjectInstances.AddAsync(projectInstance);
        await _dbContext.SaveChangesAsync();

        // Act
        var bugInstance = await _userStoryService.CreateAndAssignBugToProjectAsync(projectInstance);

        // Assert
        var bugStory = await _dbContext.UserStories.FirstOrDefaultAsync(us => us.Id == bugInstance.UserStoryId);

        Assert.NotNull(bugInstance);
        Assert.NotNull(bugStory);
        Assert.Equal(project.Id, bugStory.ProjectId);
        Assert.True(bugStory.IsRandomEvent);
        Assert.Equal(UserStoryType.Bug, bugInstance.UserStoryType);
        Assert.Equal(0, bugInstance.Progress);
        Assert.False(bugInstance.IsComplete);
        Assert.InRange(bugStory.StoryPoints, 2, 5);
        Assert.Contains("Bug:", bugStory.Title);
        Assert.Null(bugInstance.DeveloperAssignedId);
    }
}