using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;

namespace Dissertation.Services.Interfaces;

public interface IBadgeService
{
    Task<List<UserBadge>> GetUserBadgesAsync(string userId);
    Task AwardUserBadgeAsync(string userId, BadgeType badgeType);
    Task CheckAndAwardBadgesAsync(string userId);
}