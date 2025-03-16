using Dissertation.Models.Challenge;
using Dissertation.Models.Challenge.Enums;
using Dissertation.Services;
using Microsoft.IdentityModel.Tokens;
using NuGet.DependencyResolver;

namespace Dissertation.View_Models;

public class DeveloperManagementViewModel(ProjectStateService projectStateService)
{
    public string DeveloperName { get; set; } = "";
    public DeveloperExperienceLevel SelectedDeveloperExperienceLevel { get; set; } = DeveloperExperienceLevel.Junior;
    public bool CanAddDeveloper()
    {
        return DeveloperName.IsNullOrEmpty();
    }

    public void AddDeveloper()
    {
        if (string.IsNullOrWhiteSpace(DeveloperName)) return;

        var cost = SelectedDeveloperExperienceLevel switch
        {
            DeveloperExperienceLevel.Junior => projectStateService.CurrentProject.Project.DeveloperCosts
                .TryGetValue(DeveloperExperienceLevel.Junior, out var juniorCost)
                ? juniorCost
                : 0,

            DeveloperExperienceLevel.MidLevel => projectStateService.CurrentProject.Project.DeveloperCosts
                .TryGetValue(DeveloperExperienceLevel.MidLevel, out var midLevelCost)
                ? midLevelCost
                : 0,

            DeveloperExperienceLevel.Senior => projectStateService.CurrentProject.Project.DeveloperCosts
                .TryGetValue(DeveloperExperienceLevel.Senior, out var seniorCost)
                ? seniorCost
                : 0,

            _ => throw new ArgumentOutOfRangeException(nameof(SelectedDeveloperExperienceLevel))
        };

        var dev = new Developer
        {
            Name = DeveloperName,
            ExperienceLevel = SelectedDeveloperExperienceLevel,
            Cost = cost,
            UserId = projectStateService.UserId!
        };

        projectStateService.Team.Add(dev);
        DeveloperName = "";
    }

    public void RemoveDeveloper(Developer dev)
    {
        var userStoryInstancesAssignedToDeveloper = projectStateService.UserStoryInstances.Where(usi => usi.DeveloperAssignedId == dev.Id).ToList();

        foreach (var usi in userStoryInstancesAssignedToDeveloper)
        {
            usi.DeveloperAssignedId = null;
            usi.DeveloperAssigned = null;
        }

        projectStateService.Team.Remove(dev);
    }
}