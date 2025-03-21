using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dissertation.Tests.Services;

public class DeveloperServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly DeveloperService _developerService;

    public DeveloperServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _developerService = new DeveloperService(_dbContext);
    }

    [Fact]
    public async Task AddDeveloperAsync_ShouldAddNewDeveloper_WhenNotExist()
    {
        // Arrange
        var dev = new Developer { Name = "John Doe", UserId = "testUser" };

        // Act
        await _developerService.AddDeveloperAsync(dev);

        // Assert
        var addedDev = await _dbContext.Developers.FindAsync(dev.Id);
        Assert.NotNull(addedDev);
        Assert.Equal(dev.Name, addedDev.Name);
    }

    [Fact]
    public async Task AddDeveloperAsync_ShouldNotAddExistingDeveloper()
    {
        // Arrange
        var dev = new Developer { Name = "John Doe", UserId = "testUser" };
        await _dbContext.Developers.AddAsync(dev);
        await _dbContext.SaveChangesAsync();

        // Act
        await _developerService.AddDeveloperAsync(dev);

        // Assert
        var developersCount = await _dbContext.Developers.CountAsync();
        Assert.Equal(1, developersCount);
    }

    [Fact]
    public async Task RemoveDeveloperAsync_ShouldRemoveDeveloper_WhenExists()
    {
        // Arrange
        var dev = new Developer { Name = "John Doe", UserId = "testUser" };
        await _dbContext.Developers.AddAsync(dev);
        await _dbContext.SaveChangesAsync();

        // Act
        await _developerService.RemoveDeveloperAsync(dev);

        // Assert
        var removedDev = await _dbContext.Developers.FindAsync(dev.Id);
        Assert.Null(removedDev);
    }

    [Fact]
    public async Task RemoveDeveloperAsync_ShouldDoNothing_WhenDeveloperNotExist()
    {
        // Arrange
        var dev = new Developer { Name = "John Doe", UserId = "testUser" };

        // Act
        await _developerService.RemoveDeveloperAsync(dev);

        // Assert
        var removedDev = await _dbContext.Developers.FindAsync(dev.Id);
        Assert.Null(removedDev);
    }

    [Fact]
    public async Task UpdateDeveloperAbsence_ShouldUpdateDeveloperAbsence_WhenExists()
    {
        // Arrange
        var dev = new Developer { Id = 1, Name = "John Doe", IsSick = true, UserId = "testUser" };
        await _dbContext.Developers.AddAsync(dev);
        await _dbContext.SaveChangesAsync();

        // Act
        await _developerService.UpdateDeveloperAbsence(dev);

        // Assert
        var updatedDev = await _dbContext.Developers.FindAsync(dev.Id);
        Assert.NotNull(updatedDev);
    }

    [Fact]
    public async Task UpdateDeveloperAbsence_ShouldDoNothing_WhenDeveloperNotExist()
    {
        // Arrange
        var dev = new Developer { Name = "John Doe", UserId = "testUser" };

        // Act
        await _developerService.UpdateDeveloperAbsence(dev);

        // Assert
        var removedDev = await _dbContext.Developers.FindAsync(dev.Id);
        Assert.Null(removedDev);
    }
}