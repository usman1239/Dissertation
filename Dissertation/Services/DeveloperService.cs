using Dissertation.Data;
using Dissertation.Models.Challenge;
using Dissertation.Services.Interfaces;

namespace Dissertation.Services;

public class DeveloperService(AppDbContext dbContext) : IDeveloperService
{
    public async Task AddDeveloperAsync(Developer dev)
    {
        var existingDev = await dbContext.Developers.FindAsync(dev.Id);
        if (existingDev != null) return;

        await dbContext.Developers.AddAsync(dev);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveDeveloperAsync(Developer dev)
    {
        var existingDev = await dbContext.Developers.FindAsync(dev.Id);
        if (existingDev == null) return;

        dbContext.Developers.Remove(existingDev);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateDeveloperAbsence(Developer dev)
    {
        var existingDev = await dbContext.Developers.FindAsync(dev.Id);
        if (existingDev == null) return;

        existingDev.IsPermanentlyAbsent = dev.IsPermanentlyAbsent;
        existingDev.SickUntilSprint = dev.SickUntilSprint;
        existingDev.IsSick = dev.IsSick;

        await dbContext.SaveChangesAsync();
    }
}