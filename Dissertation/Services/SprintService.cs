using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class SprintService(
    AppDbContext dbContext) : ISprintService
{
    public async Task<List<Sprint>> GetSprintsForProjectAsync(int projectInstanceId)
    {
        return await dbContext.Sprints
            .Where(s => s.ProjectInstanceId == projectInstanceId)
            .OrderBy(s => s.SprintNumber)
            .ToListAsync();
    }

    public async Task SaveSprintAsync(Sprint sprint)
    {
        try
        {
            dbContext.Attach(sprint.ProjectInstance);
        }
        catch
        {
            // ignored
        }

        await dbContext.Sprints.AddAsync(sprint);
        await dbContext.SaveChangesAsync();


        await dbContext.RemoveIdleDevelopersAsync();
    }
}