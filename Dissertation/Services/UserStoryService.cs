using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class UserStoryService(AppDbContext dbContext) : IUserStoryService
{
    public async Task SaveUserStoryInstancesAsync(List<UserStoryInstance> instances)
    {
        foreach (var instance in instances)
        {
            // Check if the DeveloperAssignedId exists in the database
            if (instance.DeveloperAssignedId.HasValue)
            {
                var developerExists =
                    await dbContext.Developers.AnyAsync(d => d.Id == instance.DeveloperAssignedId.Value);
                if (!developerExists)
                {
                    // If the developer doesn't exist, set it to null to avoid FK violation
                    instance.DeveloperAssignedId = null;
                    instance.DeveloperAssigned = null;
                }
            }

            var existingInstance = await dbContext.UserStoryInstances
                .AsNoTracking()
                .FirstOrDefaultAsync(usi => usi.Id == instance.Id);

            if (existingInstance != null)
            {
                // If existing, update it
                dbContext.Entry(instance).State = EntityState.Modified;
            }
            else
            {
                // For new instances, ensure relations are properly set
                dbContext.Entry(instance.UserStory).State = EntityState.Unchanged;

                dbContext.Entry(instance.ProjectInstance).State = EntityState.Unchanged;

                // Handle developer reference
                if (instance.DeveloperAssigned != null)
                {
                    var developer = await dbContext.Developers.FindAsync(instance.DeveloperAssigned.Id);
                    if (developer != null)
                        instance.DeveloperAssignedId = developer.Id;
                    // Don't track the developer through this relationship
                    else
                        instance.DeveloperAssignedId = null;
                }

                await dbContext.UserStoryInstances.AddAsync(instance);
            }
        }

        await dbContext.SaveChangesAsync();
    }


    public async Task<List<UserStory>> GetInitialUserStoriesForProject(int projectId)
    {
        return await dbContext.UserStories
            .Where(us => us.ProjectId == projectId)
            .Include(us => us.Project)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AttachProjectAndUserStories(ProjectInstance projectInstance)
    {
        var existingProject = await dbContext.Projects.FindAsync(projectInstance.ProjectId);
        if (existingProject != null)
        {
            dbContext.Attach(existingProject);
            projectInstance.Project = existingProject;
        }

        foreach (var usi in projectInstance.UserStoryInstances)
        {
            var existingUserStory = await dbContext.UserStories.FindAsync(usi.UserStoryId);
            if (existingUserStory == null) continue;
            dbContext.Attach(existingUserStory);
            usi.UserStory = existingUserStory;
        }
    }

    public async Task<List<UserStoryInstance>> GetUserStoryInstancesAsync(int projectInstanceId)
    {
        return await dbContext.UserStoryInstances
            .Where(x => x.ProjectInstanceId == projectInstanceId)
            .Include(usi => usi.DeveloperAssigned)
            .ToListAsync();
    }
}