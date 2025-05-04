using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class BadgeService(AppDbContext dbContext) : IBadgeService
{
    public async Task<List<UserBadge>> GetUserBadgesAsync(string userId)
    {
        return await dbContext.UserBadges
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task CheckAndAwardBadgesAsync(string userId)
    {
        var userProjects = await dbContext.ProjectInstances
            .Where(p => p.UserId == userId)
            .Include(p => p.Sprints)
            .Include(p => p.UserStoryInstances)
            .ThenInclude(userStoryInstance => userStoryInstance.DeveloperAssigned)
            .Include(projectInstance => projectInstance.Project)
            .ToListAsync();

        var completedProjects = userProjects
            .Where(p => p.Sprints.All(s => s.IsCompleted) && p.Sprints.Count >= p.Project.NumOfSprints).ToList();
        var completedUserStories = userProjects.Sum(p => p.UserStoryInstances.Count(usi => usi.IsComplete));
        var distinctDeveloperTypes = userProjects.SelectMany(p => p.UserStoryInstances)
            .Select(usi => usi.DeveloperAssigned?.ExperienceLevel)
            .Where(d => d.HasValue)
            .Distinct()
            .Count();

        switch (completedProjects.Count)
        {
            case 1:
                await AwardUserBadgeAsync(userId, BadgeType.FirstProjectCompleted);
                break;
            case >= 5:
                await AwardUserBadgeAsync(userId, BadgeType.SeasonedDeveloper);
                break;
        }

        if (completedProjects.Count >= 10)
            await AwardUserBadgeAsync(userId, BadgeType.MasterArchitect);

        if (distinctDeveloperTypes >= 3)
            await AwardUserBadgeAsync(userId, BadgeType.Versatile);

        if (completedUserStories >= 30)
            await AwardUserBadgeAsync(userId, BadgeType.ProblemSlayer);
    }

    public async Task AwardUserBadgeAsync(string userId, BadgeType badgeType)
    {
        var exists = await dbContext.UserBadges.AnyAsync(b => b.UserId == userId && b.BadgeType == badgeType);
        if (!exists)
        {
            var (description, icon) = GetBadgeDetails(badgeType);

            dbContext.UserBadges.Add(new UserBadge
            {
                UserId = userId,
                BadgeType = badgeType,
                Description = description,
                Icon = icon
            });

            await dbContext.SaveChangesAsync();
        }
    }

    private static (string Description, string Icon) GetBadgeDetails(BadgeType badgeType)
    {
        return badgeType switch
        {
            BadgeType.FirstProjectCompleted => ("Completed your first project!", "🏆"),
            BadgeType.SeasonedDeveloper => ("Completed 5 projects. Keep going!", "💼"),
            BadgeType.MasterArchitect => ("Completed 10 high-quality projects!", "🏗️"),
            BadgeType.Versatile => ("Worked with junior, mid, and senior developers!", "🔀"),
            BadgeType.ProblemSlayer => ("Completed 30 user stories across projects!", "⚔️"),
            BadgeType.FirstDailyChallenge => ("Completed your first daily challenge!", "🏅"),
            BadgeType.DailyGrinder => ("Completed 5 daily challenges!", "📅"),
            _ => ("Unknown Badge", "❓")
        };
    }

    public async Task CheckDailyBadges(string userId)
    {
        var totalCompletions = await dbContext.DailyChallengeCompletions.CountAsync(d => d.UserId == userId);

        switch (totalCompletions)
        {
            case 1:
                await AwardUserBadgeAsync(userId, BadgeType.FirstDailyChallenge);
                break;
            case 5:
                await AwardUserBadgeAsync(userId, BadgeType.DailyGrinder);
                break;
        }
    }
}