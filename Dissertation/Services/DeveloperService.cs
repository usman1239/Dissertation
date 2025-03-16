using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Services;

public class DeveloperService(AppDbContext dbContext) : IDeveloperService
{
    public async Task AddDevelopersAsync(List<Developer> developers)
    {
        await dbContext.Developers.AddRangeAsync(developers);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveDeveloperAsync(Developer dev)
    {
        var existingDev = await dbContext.Developers.FindAsync(dev.Id);
        if (existingDev == null)
            return;

        dbContext.Developers.Remove(existingDev);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Developer>> GetDevelopersByNamesAsync(List<string> names)
    {
        return await dbContext.Developers
            .Where(d => names.Contains(d.Name))
            .ToListAsync();
    }
}