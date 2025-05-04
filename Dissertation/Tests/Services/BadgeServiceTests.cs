using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dissertation.Tests.Services;

public class BadgeServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly IBadgeService _badgeService;

    public BadgeServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _badgeService = new BadgeService(_dbContext);
    }

    [Fact]
    public async Task AwardUserBadgeAsync_AddsBadge_WhenNotAlreadyExists()
    {
        // Arrange
        const string userId = "user123";
        const BadgeType badgeType = BadgeType.Versatile;

        // Act
        await _badgeService.AwardUserBadgeAsync(userId, badgeType);

        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == badgeType);


        // Assert
        Assert.NotNull(badge);
        Assert.Equal(userId, badge.UserId);
        Assert.Equal(badgeType, badge.BadgeType);
        Assert.False(string.IsNullOrWhiteSpace(badge.Description));
        Assert.False(string.IsNullOrWhiteSpace(badge.Icon));
    }

    [Fact]
    public async Task AwardUserBadgeAsync_DoesNotAddDuplicateBadge()
    {
        // Arrange
        const string userId = "user123";
        const BadgeType badgeType = BadgeType.ProblemSlayer;

        _dbContext.UserBadges.Add(new UserBadge
        {
            UserId = userId,
            BadgeType = badgeType,
            Description = "Already earned",
            Icon = "🔥"
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.AwardUserBadgeAsync(userId, badgeType);
        var badges = await _dbContext.UserBadges
            .Where(b => b.UserId == userId && b.BadgeType == badgeType)
            .ToListAsync();

        // Assert
        Assert.Single(badges);
    }


    [Fact]
    public async Task GetUserBadgesAsync_ReturnsOnlyUserBadges()
    {
        // Arrange
        const string userId = "user123";
        const string otherUserId = "user456";

        _dbContext.UserBadges.AddRange(
            new UserBadge { UserId = userId, BadgeType = BadgeType.SeasonedDeveloper },
            new UserBadge { UserId = otherUserId, BadgeType = BadgeType.Versatile }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _badgeService.GetUserBadgesAsync(userId);

        // Assert
        Assert.Single(result);
        Assert.Equal(userId, result[0].UserId);
        Assert.Equal(BadgeType.SeasonedDeveloper, result[0].BadgeType);
    }


    [Fact]
    public async Task CheckDailyBadges_AwardsFirstDailyChallenge_WhenCompletionsIsOne()
    {
        // Arrange
        const string userId = "dailyUser1";

        _dbContext.DailyChallengeCompletions.Add(new DailyChallengeCompletion
        {
            UserId = userId,
            ChallengeKey = "FirstDailyChallenge"
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckDailyBadges(userId);

        // Assert
        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == BadgeType.FirstDailyChallenge);

        Assert.NotNull(badge);
    }


    [Fact]
    public async Task CheckDailyBadges_AwardsDailyGrinder_WhenCompletionsIsFive()
    {
        // Arrange
        const string userId = "dailyUser5";

        for (var i = 0; i < 5; i++)
            _dbContext.DailyChallengeCompletions.Add(new DailyChallengeCompletion
            {
                UserId = userId,
                ChallengeKey = "DailyGrinder"
            });
        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckDailyBadges(userId);

        // Assert
        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == BadgeType.DailyGrinder);

        Assert.NotNull(badge);
    }


    [Fact]
    public async Task CheckDailyBadges_DoesNothing_WhenCompletionsAreNot1Or5()
    {
        // Arrange
        const string userId = "dailyUser3";

        for (var i = 0; i < 3; i++)
            _dbContext.DailyChallengeCompletions.Add(new DailyChallengeCompletion
            {
                UserId = userId,
                ChallengeKey = ""
            });
        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckDailyBadges(userId);

        // Assert
        var badges = await _dbContext.UserBadges
            .Where(b => b.UserId == userId)
            .ToListAsync();

        Assert.Empty(badges);
    }


    [Fact]
    public async Task CheckAndAwardBadgesAsync_AwardsFirstProjectCompleted_WhenOneProjectIsCompleted()
    {
        // Arrange
        const string userId = "user1";
        var project = new Project { Id = 1, NumOfSprints = 1, Title = "P1" };
        _dbContext.Projects.Add(project);
        var instance = new ProjectInstance { Id = 1, UserId = userId, ProjectId = project.Id, Project = project };
        var sprint = new Sprint { Id = 1, ProjectInstanceId = 1, IsCompleted = true };
        _dbContext.ProjectInstances.Add(instance);
        _dbContext.Sprints.Add(sprint);

        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckAndAwardBadgesAsync(userId);

        // Assert
        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == BadgeType.FirstProjectCompleted);

        Assert.NotNull(badge);
    }

    [Fact]
    public async Task CheckAndAwardBadgesAsync_AwardsSeasonedDeveloper_WhenFiveProjectsCompleted()
    {
        // Arrange
        const string userId = "user5";
        var project = new Project { Id = 1, NumOfSprints = 1, Title = "P5" };
        _dbContext.Projects.Add(project);

        for (var i = 1; i <= 5; i++)
        {
            var instance = new ProjectInstance { Id = i, UserId = userId, ProjectId = 1, Project = project };
            _dbContext.ProjectInstances.Add(instance);
            _dbContext.Sprints.Add(new Sprint { Id = i, ProjectInstanceId = i, IsCompleted = true });
        }

        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckAndAwardBadgesAsync(userId);

        // Assert
        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == BadgeType.SeasonedDeveloper);

        Assert.NotNull(badge);
    }


    [Fact]
    public async Task CheckAndAwardBadgesAsync_AwardsProblemSlayer_When30StoriesCompleted()
    {
        // Arrange
        const string userId = "user30";
        var project = new Project { Id = 1, NumOfSprints = 1, Title = "P30" };
        _dbContext.Projects.Add(project);
        var instance = new ProjectInstance { Id = 1, UserId = userId, ProjectId = 1, Project = project };
        _dbContext.ProjectInstances.Add(instance);
        _dbContext.Sprints.Add(new Sprint { Id = 1, ProjectInstanceId = 1, IsCompleted = true });

        for (var i = 0; i < 30; i++)
            _dbContext.UserStoryInstances.Add(new UserStoryInstance { ProjectInstanceId = 1, IsComplete = true });

        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckAndAwardBadgesAsync(userId);

        // Assert
        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == BadgeType.ProblemSlayer);

        Assert.NotNull(badge);
    }


    [Fact]
    public async Task CheckAndAwardBadgesAsync_AwardsVersatile_WhenAllDeveloperTypesUsed()
    {
        // Arrange
        const string userId = "userDevTypes";
        var project = new Project { Id = 1, NumOfSprints = 1 };
        _dbContext.Projects.Add(project);
        var instance = new ProjectInstance { Id = 1, UserId = userId, ProjectId = 1, Project = project };
        _dbContext.ProjectInstances.Add(instance);
        _dbContext.Sprints.Add(new Sprint { Id = 1, ProjectInstanceId = 1, IsCompleted = true });

        _dbContext.UserStoryInstances.AddRange(
            new UserStoryInstance
            {
                ProjectInstanceId = 1,
                DeveloperAssigned = new Developer { ExperienceLevel = DeveloperExperienceLevel.Junior, UserId = userId}
            },
            new UserStoryInstance
            {
                ProjectInstanceId = 1,
                DeveloperAssigned = new Developer { ExperienceLevel = DeveloperExperienceLevel.MidLevel, UserId = userId }
            },
            new UserStoryInstance
            {
                ProjectInstanceId = 1,
                DeveloperAssigned = new Developer { ExperienceLevel = DeveloperExperienceLevel.Senior, UserId = userId }
            }
        );

        await _dbContext.SaveChangesAsync();

        // Act
        await _badgeService.CheckAndAwardBadgesAsync(userId);

        // Assert
        var badge = await _dbContext.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BadgeType == BadgeType.Versatile);

        Assert.NotNull(badge);
    }
}