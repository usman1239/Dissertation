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
}