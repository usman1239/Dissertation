using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class ChallengeService(AppDbContext dbContext) : IChallengeService
{
    public async Task<List<Project>> GetAvailableProjectsAsync()
    {
        return await dbContext.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.Budget)
            .ToListAsync();
    }

    public async Task<List<ProjectInstance>> LoadProjectsWithSavedProgressAsync(string? userId)
    {
        return await dbContext.UserProjectInstances
            .Where(x => x.UserId == userId)
            .Include(p => p.Project)
            .OrderByDescending(p => p.Budget)
            .ToListAsync();
    }

    public async Task<ProjectInstance?> GetProjectInstanceAsync(int projectId, string? userId)
    {
        return await dbContext.UserProjectInstances
            .Include(p => p.Project)
            .Include(p => p.Sprints)
            .Include(p => p.UserStories)
            .Include(p => p.UserStories)
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);
    }

    public async Task AddUserProjectInstanceAsync(ProjectInstance projectInstance)
    {
        var existingUserProjectInstance = await dbContext.UserProjectInstances
            .FirstOrDefaultAsync(up => up.ProjectId == projectInstance.ProjectId &&
                                       up.UserId == projectInstance.UserId);

        if (existingUserProjectInstance == null)
            await dbContext.UserProjectInstances.AddAsync(projectInstance);
        else
            dbContext.Entry(existingUserProjectInstance).CurrentValues.SetValues(projectInstance);

        dbContext.Entry(projectInstance).Reference(p => p.Project).CurrentValue = projectInstance.Project;
        dbContext.Entry(projectInstance.Project).State = EntityState.Unchanged;

        await dbContext.SaveChangesAsync();
    }

    public async Task AddSprintsToDbAsync(List<Sprint> sprints)
    {
        foreach (var sprint in sprints)
        {
            var existingSprint = await dbContext.Sprints
                .FirstOrDefaultAsync(s => s.Id == sprint.Id);

            if (existingSprint != null)
                dbContext.Entry(existingSprint).CurrentValues.SetValues(sprint);
            else
                await dbContext.Sprints.AddAsync(sprint);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task AddDevelopersToDbAsync(List<Developer> developers)
    {
        var existingDeveloperIds = await dbContext.Developers
            .Where(d => developers.Select(dev => dev.Id).Contains(d.Id))
            .Select(d => d.Id)
            .ToListAsync();

        var newDevelopers = developers
            .Where(d => !existingDeveloperIds.Contains(d.Id))
            .ToList();

        if (newDevelopers.Count != 0)
        {
            await dbContext.Developers.AddRangeAsync(newDevelopers);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task AddOrUpdateUserStoriesAsync(List<UserStory> userStories)
    {
        foreach (var story in userStories)
        {
            var existingStory = await dbContext.UserStories
                .FirstOrDefaultAsync(us => us.Id == story.Id);

            if (existingStory != null)
            {
                dbContext.Entry(existingStory).CurrentValues.SetValues(story);

            }
            else
            {
                await dbContext.UserStories.AddAsync(story);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteSavedProjectInstanceAsync(int projectId, string? userId)
    {
        var existingUserProjectInstance = await dbContext.UserProjectInstances
            .Where(x => x.ProjectId == projectId &&
                        x.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingUserProjectInstance == null) return false;
        dbContext.UserProjectInstances.Remove(existingUserProjectInstance);
        await dbContext.SaveChangesAsync();
        return true;

    }
}