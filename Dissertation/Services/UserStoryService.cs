using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class UserStoryService(AppDbContext dbContext) : IUserStoryService
{
    public async Task SaveUserStoryInstancesAsync(List<UserStoryInstance> instances)
    {
        foreach (var instance in instances)
        {
            if (instance.DeveloperAssignedId.HasValue)
            {
                var developerExists =
                    await dbContext.Developers.AnyAsync(d => d.Id == instance.DeveloperAssignedId.Value);
                if (!developerExists)
                {
                    instance.DeveloperAssignedId = null;
                    instance.DeveloperAssigned = null;
                }
            }

            var existingInstance = await dbContext.UserStoryInstances
                .AsNoTracking()
                .FirstOrDefaultAsync(usi => usi.Id == instance.Id);

            if (existingInstance != null)
            {
                dbContext.Entry(instance).State = EntityState.Modified;
            }
            else
            {
                dbContext.Entry(instance.UserStory).State = EntityState.Unchanged;

                dbContext.Entry(instance.ProjectInstance).State = EntityState.Unchanged;

                if (instance.DeveloperAssigned != null)
                {
                    var developer = await dbContext.Developers.FindAsync(instance.DeveloperAssigned.Id);
                    if (developer != null)
                        instance.DeveloperAssignedId = developer.Id;
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
            .Where(us => us.ProjectId == projectId && !us.IsRandomEvent)
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

    public async Task TriggerRandomUserStoryEventAsync(int projectInstanceId)
    {
        var assignedUserStoryIds = await dbContext.UserStoryInstances
            .Where(usi => usi.ProjectInstanceId == projectInstanceId)
            .Select(usi => usi.UserStoryId)
            .ToListAsync();

        Console.WriteLine($"Assigned User Story IDs: {string.Join(", ", assignedUserStoryIds)}");

        // Get only unassigned random user stories for the correct project instance
        var availableUserStories = await dbContext.UserStories
            .Where(us => us.ProjectId == dbContext.ProjectInstances
                .Where(pi => pi.Id == projectInstanceId)
                .Select(pi => pi.ProjectId)
                .FirstOrDefault() && us.IsRandomEvent)
            .Where(us => !assignedUserStoryIds.Contains(us.Id)) // Filter out already assigned user stories
            .ToListAsync();

        Console.WriteLine($"Available User Stories Count: {availableUserStories.Count}");

        if (availableUserStories.Count == 0)
        {
            Console.WriteLine("No available random user stories found.");
            return;
        }

        var random = new Random();
        var randomUserStory = availableUserStories[random.Next(availableUserStories.Count)];

        var newUserStoryInstance = new UserStoryInstance
        {
            ProjectInstanceId = projectInstanceId,
            UserStoryId = randomUserStory.Id
        };

        await dbContext.UserStoryInstances.AddAsync(newUserStoryInstance);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<UserStoryInstance>> GetUserStoryInstancesForProjectAsync(int projectInstanceId)
    {
        return await dbContext.UserStoryInstances
            .Where(usi => usi.ProjectInstanceId == projectInstanceId)
            .Include(usi => usi.UserStory)
            .ToListAsync();
    }


    public async Task<UserStoryInstance> CreateAndAssignBugToProjectAsync(ProjectInstance projectInstance)
    {
        var random = new Random();
        var bugTemplates = new List<(string Title, string Description)>
        {
            ("Bug: Login Failure", "Users are randomly logged out during session."),
            ("Bug: UI Glitch", "Dropdown menus disappear when hovering."),
            ("Bug: Memory Leak", "Memory usage spikes after long activity."),
            ("Bug: Save Error", "Data not saved under certain network conditions."),
            ("Bug: Unexpected Crash", "App crashes when switching views quickly."),
            ("Bug: Missing Notification", "Some alerts are not triggered."),
            ("Bug: Incorrect Budget Display", "Budget values aren't updating properly."),
            ("Bug: Unassigned Developer", "Task appears with no developer linked.")
        };

        var (title, description) = bugTemplates[random.Next(bugTemplates.Count)];


        var bugStory = new UserStory
        {
            Title = title,
            Description = description,
            StoryPoints = random.Next(2, 6),
            Project = projectInstance.Project,
            ProjectId = projectInstance.ProjectId,
            IsRandomEvent = true,
            UserStoryInstances = new List<UserStoryInstance>()
        };

        dbContext.UserStories.Add(bugStory);
        await dbContext.SaveChangesAsync();


        var bugInstance = new UserStoryInstance
        {
            UserStoryId = bugStory.Id,
            UserStory = bugStory,
            ProjectInstanceId = projectInstance.Id,
            Progress = 0,
            IsComplete = false,
            DeveloperAssignedId = null,
            DeveloperAssigned = null,
            UserStoryType = UserStoryType.Bug
        };

        dbContext.UserStoryInstances.Add(bugInstance);
        await dbContext.SaveChangesAsync();

        return bugInstance;
    }
}