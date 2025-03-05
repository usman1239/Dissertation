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

    public async Task<List<UserProjectInstance>> LoadProjectsWithSavedProgressAsync(string? userId)
    {
        return await dbContext.UserProjectInstances
            .Where(x => x.UserId == userId)
            .Include(p => p.Project)
            .OrderByDescending(p => p.Budget)
            .ToListAsync();
    }

    public async Task<UserProjectInstance?> GetProjectInstanceAsync(int projectId, string? userId)
    {
        return await dbContext.UserProjectInstances
            .Include(p => p.Project)
            .Include(p => p.Sprints)
            .Include(p => p.UserStories)
            .ThenInclude(us => us.Tasks)
            .ThenInclude(t => t.AssignedTo)
            .Include(p => p.UserStories)
            .ThenInclude(us => us.AssignedTo)
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);
    }

    public async Task AddUserProjectInstanceAsync(UserProjectInstance userProjectInstance)
    {
        var existingUserProjectInstance = await dbContext.UserProjectInstances
            .FirstOrDefaultAsync(up => up.ProjectId == userProjectInstance.ProjectId &&
                                       up.UserId == userProjectInstance.UserId);

        if (existingUserProjectInstance == null)
            await dbContext.UserProjectInstances.AddAsync(userProjectInstance);
        else
            dbContext.Entry(existingUserProjectInstance).CurrentValues.SetValues(userProjectInstance);

        dbContext.Entry(userProjectInstance).Reference(p => p.Project).CurrentValue = userProjectInstance.Project;
        dbContext.Entry(userProjectInstance.Project).State = EntityState.Unchanged;

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

    public async Task AddOrUpdateUserStoriesAndTasksAsync(List<UserStory> userStories)
    {
        foreach (var story in userStories)
        {
            var existingStory = await dbContext.UserStories
                .Include(us => us.Tasks)
                .FirstOrDefaultAsync(us => us.Id == story.Id);

            if (existingStory != null)
            {
                dbContext.Entry(existingStory).CurrentValues.SetValues(story);

                foreach (var task in story.Tasks)
                {
                    var existingTask = existingStory.Tasks.FirstOrDefault(t => t.Id == task.Id);
                    if (existingTask != null)
                        dbContext.Entry(existingTask).CurrentValues.SetValues(task);
                    else
                        existingStory.Tasks.Add(task);
                }
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