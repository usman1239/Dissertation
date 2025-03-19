using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IDeveloperService
{
    Task AddDeveloperAsync(Developer dev);
    Task RemoveDeveloperAsync(Developer dev);
    Task UpdateDeveloperAbsence(Developer dev);
}