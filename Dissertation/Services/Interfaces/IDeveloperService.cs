using Dissertation.Models.Challenge;

namespace Dissertation.Services.Interfaces;

public interface IDeveloperService
{
    Task AddDevelopersAsync(List<Developer> dev);
    Task RemoveDeveloperAsync(Developer dev);
    Task<List<Developer>> GetDevelopersByNamesAsync(List<string> names);
}