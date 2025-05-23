﻿using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class ProjectService(AppDbContext dbContext) : IProjectService
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
        return await dbContext.ProjectInstances
            .Include(p => p.Project)
            .Include(us => us.UserStoryInstances)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<ProjectInstance?> GetProjectInstanceAsync(int projectId, string? userId)
    {
        return await dbContext.ProjectInstances
            .Include(p => p.Project)
            .Include(p => p.Sprints)
            .Include(p => p.UserStoryInstances)
            .ThenInclude(usi => usi.UserStory) // Ensure UserStory is included
            .Include(p => p.UserStoryInstances)
            .ThenInclude(usi => usi.DeveloperAssigned)
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId);
    }

    public async Task SaveNewProjectInstance(ProjectInstance projectInstance)
    {
        var existingInstance = await dbContext.ProjectInstances
            .FirstOrDefaultAsync(pi =>
                pi.ProjectId == projectInstance.ProjectId && pi.UserId == projectInstance.UserId);

        if (existingInstance != null) return;


        await dbContext.ProjectInstances.AddAsync(projectInstance);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteSavedProjectInstanceAsync(int projectId, string? userId)
    {
        var instance = await dbContext.ProjectInstances
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.UserId == userId);

        if (instance == null) return false;

        dbContext.ProjectInstances.Remove(instance);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProjectInstance>> GetUserProjectsAsync(string userId)
    {
        return await dbContext.ProjectInstances
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Include(p => p.Project)
            .Include(p => p.Sprints)
            .ToListAsync();
    }

    public async Task UpdateProjectInstance(ProjectInstance instance)
    {
        dbContext.ProjectInstances.Update(instance);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasCompletedChallengeAsync(string userId, int projectId, DateOnly date)
    {
        return await dbContext.DailyChallengeCompletions
            .AnyAsync(d => d.UserId == userId && d.ProjectInstanceId == projectId && d.Date == date);
    }

    public async Task MarkChallengeCompletedAsync(string userId, int projectId, DateOnly date, string challengeKey)
    {
        dbContext.DailyChallengeCompletions.Add(new DailyChallengeCompletion
        {
            UserId = userId,
            ProjectInstanceId = projectId,
            Date = date,
            ChallengeKey = challengeKey
        });

        await dbContext.SaveChangesAsync();
    }
}